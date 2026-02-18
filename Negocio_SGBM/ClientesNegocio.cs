using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    public class ClientesNegocio
    {
        /// <summary>
        /// Valida la información de un cliente antes de registrar o modificar.
        /// </summary>
        private static Resultado<Clientes?> ComprobarCliente(Clientes? cliente, bool registro)
        {
            if (cliente == null)
                return Resultado<Clientes?>.Fail("Problema al enviar datos de cliente entre capas.");

            if (cliente.Personas == null)
                return Resultado<Clientes?>.Fail("Problema al enviar datos de la persona relacionada al cliente entre capas.");

            if (cliente.IdEstado > 0)
                cliente.Estados = null;

            if (cliente.Estados != null)
                cliente.IdEstado = cliente.Estados.IdEstado > 0 ? cliente.Estados.IdEstado : 0;

            if (cliente.IdEstado < 1 && !registro)
                return Resultado<Clientes?>.Fail("Error al asignar un estado al cliente en la capa negocio.");

            if (!registro && cliente.IdCliente == null)
                return Resultado<Clientes?>.Fail("Error al mover el Id del cliente a la capa negocio.");
            else if (registro)
                cliente.IdCliente = null;

            return Resultado<Clientes?>.Ok(cliente);
        }

        /// <summary>
        /// Importa un cliente: valida persona, registra o modifica según corresponda,
        /// y registra contacto si se proporciona.
        /// </summary>
        public static Resultado<bool> ImportarCliente(Clientes? cliente, Contactos? contacto)
        {
            var validacion = ComprobarCliente(cliente, true);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            cliente = validacion.Data!;

            try
            {
                // Buscar persona por DNI
                var resultadoPersona = PersonasNegocio.GetPersonaPorDni(cliente.Personas!.Dni);
                Personas? persona = resultadoPersona.Data;

                if (persona == null)
                {
                    var resultadoRegistro = PersonasNegocio.RegistrarPersona(cliente.Personas);
                    if (!resultadoRegistro.Success || resultadoRegistro.Data <= 0)
                        return Resultado<bool>.Fail(resultadoRegistro.Mensaje);

                    cliente.IdPersona = resultadoRegistro.Data;
                }
                else
                {
                    cliente.IdPersona = persona.IdPersona ?? 0;
                    cliente.Personas.IdPersona = persona.IdPersona;

                    var resultadoModPersona = PersonasNegocio.ModificarPersona(cliente.Personas);
                    if (!resultadoModPersona.Success)
                        return Resultado<bool>.Fail(resultadoModPersona.Mensaje);
                }

                if (cliente.IdPersona < 1)
                    return Resultado<bool>.Fail("No se pudo asignar persona al cliente.");

                // Buscar cliente por DNI
                var resultadoCliente = GetClientePorDni(cliente.Personas.Dni);
                if (resultadoCliente.Data == null)
                {
                    cliente.Personas = null;
                    var resultadoRegistroCliente = RegistrarClienteBasico(cliente);
                    if (!resultadoRegistroCliente.Success)
                        return Resultado<bool>.Fail(resultadoRegistroCliente.Mensaje);
                }

                // Registrar contacto si corresponde
                if (contacto != null)
                {
                    contacto.IdPersona = cliente.IdPersona;
                    var resultadoContacto = ContactosNegocio.RegistrarContacto(contacto);
                    if (!resultadoContacto.Success)
                        Logger.LogError($"No se pudo registrar contacto para el DNI {cliente.Personas.Dni}: {resultadoContacto.Mensaje}");
                }

                return Resultado<bool>.Ok(true, "Cliente importado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al importar cliente:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene un cliente a partir de su DNI.
        /// </summary>
        public static Resultado<Clientes?> GetClientePorDni(string? dni)
        {
            var resultadoPersona = PersonasNegocio.GetPersonaPorDni(dni);
            if (!resultadoPersona.Success || resultadoPersona.Data == null)
                return Resultado<Clientes?>.Fail(resultadoPersona.Mensaje);

            var persona = resultadoPersona.Data;
            if (persona.IdPersona == null)
                return Resultado<Clientes?>.Fail("La persona no tiene Id válido.");

            try
            {
                var resultadoCliente = ClientesDatos.GetClientePorIdPersona(persona.IdPersona.Value);
                if (!resultadoCliente.Success)
                    return Resultado<Clientes?>.Fail(resultadoCliente.Mensaje);

                var cliente = resultadoCliente.Data;
                if (cliente != null)
                    cliente.Personas = persona;

                return Resultado<Clientes?>.Ok(cliente);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener cliente por DNI:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Clientes?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene un listado de clientes según criterio de búsqueda.
        /// </summary>
        public static Resultado<List<Clientes>> GetListadoDeClientes(string? criterioBusqueda, string? campo1, string? campo2, Localidades? localidad, bool incluirAnulados)
        {
            if (string.IsNullOrWhiteSpace(criterioBusqueda))
                return Resultado<List<Clientes>>.Fail("No se ha seleccionado un criterio de búsqueda.");

            try
            {
                if (string.IsNullOrWhiteSpace(campo1) && string.IsNullOrWhiteSpace(campo2) && localidad == null)
                    return ClientesDatos.GetClientes();

                if (criterioBusqueda == "Dni, Nombres")
                    return ClientesDatos.GetClientesPorDniNombres(campo1, campo2);

                if (criterioBusqueda == "Domicilio")
                    return GetClientesPorDomicilio(campo1, campo2, localidad, incluirAnulados);

                if (criterioBusqueda == "WhatsApp, Teléfono")
                    return GetClientesPorContactos(campo1, campo2);

                return Resultado<List<Clientes>>.Fail("No se pudo obtener el listado de clientes.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener listado de clientes:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Clientes>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene clientes filtrados por domicilio (calle, barrio, localidad).
        /// </summary>
        public static Resultado<List<Clientes>> GetClientesPorDomicilio(string? calle, string? barrio, Localidades? localidad, bool incluirAnulados)
        {
            if (string.IsNullOrWhiteSpace(calle) && string.IsNullOrWhiteSpace(barrio) && localidad == null)
                return Resultado<List<Clientes>>.Fail("No se han enviado datos de búsqueda.");

            try
            {
                var resultadoDomicilios = DomiciliosDatos.GetDomiciliosPorCampos(calle, barrio, localidad);
                if (!resultadoDomicilios.Success || resultadoDomicilios.Data == null)
                    return Resultado<List<Clientes>>.Fail(resultadoDomicilios.Mensaje);

                var idsDomicilios = resultadoDomicilios.Data.Select(d => d.IdDomicilio ?? 0).ToList();

                var resultadoClientes = ClientesDatos.GetClientes();
                if (!resultadoClientes.Success || resultadoClientes.Data == null)
                    return Resultado<List<Clientes>>.Fail(resultadoClientes.Mensaje);

                var clientes = resultadoClientes.Data
                    .Where(c => c.Personas != null && c.Personas.Domicilios != null && idsDomicilios.Contains(c.Personas.IdDomicilio ?? 0))
                    .ToList();

                return Resultado<List<Clientes>>.Ok(clientes);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener clientes por domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Clientes>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene clientes filtrados por contactos (teléfono, WhatsApp).
        /// </summary>
        public static Resultado<List<Clientes>> GetClientesPorContactos(string? telefono, string? whatsapp)
        {
            if (string.IsNullOrWhiteSpace(telefono) && string.IsNullOrWhiteSpace(whatsapp))
                return Resultado<List<Clientes>>.Fail("No se han enviado datos de búsqueda.");

            try
            {
                var resultadoContactos = ContactosNegocio.GetContactosPorNumero(telefono, whatsapp);
                if (!resultadoContactos.Success || resultadoContactos.Data == null)
                    return Resultado<List<Clientes>>.Fail(resultadoContactos.Mensaje);

                var idsPersonas = resultadoContactos.Data.Where(c => c.IdPersona != null).Select(c => c.IdPersona!.Value).ToList();

                var resultadoClientes = ClientesDatos.GetClientes();
                if (!resultadoClientes.Success || resultadoClientes.Data == null)
                    return Resultado<List<Clientes>>.Fail(resultadoClientes.Mensaje);

                var clientes = resultadoClientes.Data
                    .Where(c => c.Personas != null && idsPersonas.Contains(c.IdPersona))
                    .ToList();

                return Resultado<List<Clientes>>.Ok(clientes);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener clientes por contactos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Clientes>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo cliente junto con sus contactos asociados.
        /// </summary>
        public static Resultado<int> RegistrarCliente(Clientes? cliente, List<Contactos>? contactos)
        {
            var validacion = ComprobarCliente(cliente, true);
            if (!validacion.Success)
                return Resultado<int>.Fail(validacion.Mensaje);

            cliente = validacion.Data!;
            try
            {
                // Validar persona
                var resultadoPersona = PersonasNegocio.GetPersonaPorDni(cliente.Personas!.Dni);
                Personas? persona = resultadoPersona.Data;

                if (persona != null)
                {
                    cliente.Personas.IdPersona = persona.IdPersona;
                    if (persona.IdPersona == null)
                        return Resultado<int>.Fail("Problemas con el Id de la persona en la BD.");

                    var resultadoModPersona = PersonasNegocio.ModificarPersona(cliente.Personas);
                    if (!resultadoModPersona.Success)
                        return Resultado<int>.Fail(resultadoModPersona.Mensaje);

                    cliente.Personas = null;
                    cliente.IdPersona = persona.IdPersona!.Value;
                }
                else
                {
                    var resultadoRegistroPersona = PersonasNegocio.RegistrarPersona(cliente.Personas);
                    if (!resultadoRegistroPersona.Success || resultadoRegistroPersona.Data <= 0)
                        return Resultado<int>.Fail(resultadoRegistroPersona.Mensaje);

                    cliente.IdPersona = resultadoRegistroPersona.Data;
                    persona = cliente.Personas;
                    persona.IdPersona = cliente.IdPersona;
                }

                if (cliente.IdPersona < 1)
                    return Resultado<int>.Fail("No se pudo asignar persona al cliente.");

                // Estado
                var resultadoEstado = EstadosNegocio.GetEstado("Clientes", "Activo");
                var estado = resultadoEstado.Data ?? new Estados { Indole = "Clientes", Estado = "Activo" };

                if (estado.IdEstado == null || estado.IdEstado < 1)
                {
                    var resultadoRegistroEstado = EstadosNegocio.RegistrarEstado(estado);
                    if (!resultadoRegistroEstado.Success || resultadoRegistroEstado.Data < 1)
                        return Resultado<int>.Fail(resultadoRegistroEstado.Mensaje);

                    cliente.IdEstado = resultadoRegistroEstado.Data;
                }
                else
                {
                    cliente.IdEstado = estado.IdEstado!;
                }

                cliente.Estados = null;
                cliente.Personas = null;

                var resultadoCliente = ClientesDatos.RegistrarCliente(cliente);
                if (!resultadoCliente.Success || resultadoCliente.Data < 1)
                    return Resultado<int>.Fail(resultadoCliente.Mensaje);

                // Gestionar contactos
                if (!PersonasNegocio.GestionarContactosPorPersona(persona, contactos).Success)
                    Logger.LogError("Problemas al gestionar contactos del cliente.");

                return Resultado<int>.Ok(resultadoCliente.Data, "Cliente registrado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al registrar cliente:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un cliente básico (sin contactos).
        /// </summary>
        public static Resultado<int> RegistrarClienteBasico(Clientes? cliente)
        {
            var validacion = ComprobarCliente(cliente, true);
            if (!validacion.Success)
                return Resultado<int>.Fail(validacion.Mensaje);

            cliente = validacion.Data!;
            try
            {
                var resultadoEstado = EstadosNegocio.GetEstado("Clientes", "Activo");
                var estado = resultadoEstado.Data ?? new Estados { Indole = "Clientes", Estado = "Activo" };

                if (estado.IdEstado == null || estado.IdEstado < 1)
                {
                    var resultadoRegistroEstado = EstadosNegocio.RegistrarEstado(estado);
                    if (!resultadoRegistroEstado.Success || resultadoRegistroEstado.Data < 1)
                        return Resultado<int>.Fail(resultadoRegistroEstado.Mensaje);

                    cliente.IdEstado = resultadoRegistroEstado.Data;
                }
                else
                {
                    cliente.IdEstado = estado.IdEstado!;
                }

                cliente.Estados = null;
                cliente.Personas = null;

                var resultadoCliente = ClientesDatos.RegistrarCliente(cliente);
                if (!resultadoCliente.Success || resultadoCliente.Data < 1)
                    return Resultado<int>.Fail(resultadoCliente.Mensaje);

                return Resultado<int>.Ok(resultadoCliente.Data, "Cliente básico registrado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al registrar cliente básico:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un cliente existente junto con sus contactos asociados.
        /// </summary>
        public static Resultado<bool> ModificarCliente(Clientes? cliente, List<Contactos>? contactos)
        {
            var validacion = ComprobarCliente(cliente, false);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            cliente = validacion.Data!;
            try
            {
                if (cliente.IdEstado < 1)
                    return Resultado<bool>.Fail("El estado del cliente no se ha podido encontrar.");

                var resultadoPersona = PersonasNegocio.GetPersonaPorDni(cliente.Personas!.Dni);
                var persona = resultadoPersona.Data;
                if (persona == null || persona.IdPersona == null)
                    return Resultado<bool>.Fail("No se pudo encontrar información de la persona a modificar.");

                cliente.Personas.IdPersona = persona.IdPersona;

                var resultadoModPersona = PersonasNegocio.ModificarPersona(cliente.Personas);
                if (!resultadoModPersona.Success)
                    return Resultado<bool>.Fail(resultadoModPersona.Mensaje);

                cliente.Personas = null;
                cliente.IdPersona = persona.IdPersona!.Value;
                cliente.Estados = null;

                var resultadoCliente = ClientesDatos.ModificarCliente(cliente);
                if (!resultadoCliente.Success)
                    return Resultado<bool>.Fail(resultadoCliente.Mensaje);

                var resultadoContactos = PersonasNegocio.GestionarContactosPorPersona(persona, contactos);
                if (!resultadoContactos.Success)
                    Logger.LogError($"Problemas al gestionar contactos del cliente: {resultadoContactos.Mensaje}");

                return Resultado<bool>.Ok(true, "Cliente modificado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al modificar cliente:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }
    }
}
