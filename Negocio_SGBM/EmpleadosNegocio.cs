using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    public class EmpleadosNegocio
    {
        /// <summary>
        /// Valida la información de un empleado antes de registrar o modificar.
        /// </summary>
        private static Resultado<Empleados?> ComprobarEmpleado(Empleados? empleado, bool registro)
        {
            if (empleado == null)
                return Resultado<Empleados?>.Fail("Problema al enviar datos de empleado entre capas.");

            if (empleado.Personas == null)
                return Resultado<Empleados?>.Fail("Problema al enviar datos de la persona relacionada al empleado entre capas.");

            if (empleado.IdEstado > 0)
                empleado.Estados = null;

            if (empleado.Estados != null)
                empleado.IdEstado = empleado.Estados.IdEstado;

            if (empleado.IdEstado < 1 && !registro)
                return Resultado<Empleados?>.Fail("Error al asignar un estado al empleado en la capa negocio.");

            if (!registro && empleado.IdEmpleado == null)
                return Resultado<Empleados?>.Fail("Error al mover el Id del empleado a la capa negocio.");
            else if (registro)
                empleado.IdEmpleado = null;

            return Resultado<Empleados?>.Ok(empleado);
        }

        /// <summary>
        /// Importa un empleado: valida persona, registra o modifica según corresponda,
        /// y registra contacto si se proporciona.
        /// </summary>
        public static Resultado<bool> ImportarEmpleado(Empleados? empleado, Contactos? contacto)
        {
            var validacion = ComprobarEmpleado(empleado, true);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            empleado = validacion.Data!;
            try
            {
                var resultadoPersona = PersonasNegocio.GetPersonaPorDni(empleado.Personas!.Dni);
                Personas? persona = resultadoPersona.Data;

                if (persona == null)
                {
                    var resultadoRegistro = PersonasNegocio.RegistrarPersona(empleado.Personas);
                    if (!resultadoRegistro.Success || resultadoRegistro.Data <= 0)
                        return Resultado<bool>.Fail(resultadoRegistro.Mensaje);

                    empleado.IdPersona = resultadoRegistro.Data;
                }
                else
                {
                    empleado.IdPersona = persona.IdPersona ?? 0;
                    empleado.Personas.IdPersona = persona.IdPersona;

                    var resultadoModPersona = PersonasNegocio.ModificarPersona(empleado.Personas);
                    if (!resultadoModPersona.Success)
                        return Resultado<bool>.Fail(resultadoModPersona.Mensaje);
                }

                if (empleado.IdPersona < 1)
                    return Resultado<bool>.Fail("No se pudo asignar persona al empleado.");

                var resultadoEmpleado = GetEmpleadoPorDni(empleado.Personas.Dni);
                if (resultadoEmpleado.Data == null)
                {
                    empleado.Personas = null;
                    var resultadoRegistroEmpleado = RegistrarEmpleadoBasico(empleado);
                    if (!resultadoRegistroEmpleado.Success)
                        return Resultado<bool>.Fail(resultadoRegistroEmpleado.Mensaje);
                }

                if (contacto != null)
                {
                    contacto.IdPersona = empleado.IdPersona;
                    var resultadoContacto = ContactosNegocio.RegistrarContacto(contacto);
                    if (!resultadoContacto.Success)
                        Logger.LogError($"No se pudo registrar contacto para el DNI {empleado.Personas.Dni}: {resultadoContacto.Mensaje}");
                }

                return Resultado<bool>.Ok(true, "Empleado importado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al importar empleado:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene un empleado a partir de su DNI.
        /// </summary>
        public static Resultado<Empleados?> GetEmpleadoPorDni(string? dni)
        {
            var resultadoPersona = PersonasNegocio.GetPersonaPorDni(dni);
            var persona = resultadoPersona.Data;
            if (persona?.IdPersona == null)
                return Resultado<Empleados?>.Fail("No se encontró persona con el DNI proporcionado.");

            try
            {
                var resultadoEmpleado = EmpleadosDatos.GetEmpleadoPorIdPersona(persona.IdPersona.Value);
                if (!resultadoEmpleado.Success)
                    return Resultado<Empleados?>.Fail(resultadoEmpleado.Mensaje);

                var empleado = resultadoEmpleado.Data ?? new Empleados();
                empleado.Personas = persona;
                empleado.IdPersona = persona.IdPersona.Value;

                return Resultado<Empleados?>.Ok(empleado);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener empleado por DNI:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Empleados?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene empleados filtrados por domicilio (calle, barrio, localidad).
        /// </summary>
        public static Resultado<List<Empleados>> GetEmpleadosPorDomicilio(string? calle, string? barrio, Localidades? localidad, bool incluirAnulados)
        {
            if (string.IsNullOrWhiteSpace(calle) && string.IsNullOrWhiteSpace(barrio) && localidad == null)
                return Resultado<List<Empleados>>.Fail("No se han enviado datos de búsqueda.");

            try
            {
                var resultadoDomicilios = DomiciliosDatos.GetDomiciliosPorCampos(calle, barrio, localidad);
                if (!resultadoDomicilios.Success || resultadoDomicilios.Data == null)
                    return Resultado<List<Empleados>>.Fail(resultadoDomicilios.Mensaje);

                var idsDomicilios = resultadoDomicilios.Data.Select(d => d.IdDomicilio ?? 0).ToList();

                var resultadoEmpleados = EmpleadosDatos.GetEmpleados();
                if (!resultadoEmpleados.Success || resultadoEmpleados.Data == null)
                    return Resultado<List<Empleados>>.Fail(resultadoEmpleados.Mensaje);

                var empleados = resultadoEmpleados.Data
                    .Where(e => e.Personas != null && e.Personas.Domicilios != null && idsDomicilios.Contains(e.Personas.IdDomicilio ?? 0))
                    .ToList();

                return Resultado<List<Empleados>>.Ok(empleados);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener empleados por domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Empleados>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene empleados filtrados por contactos (teléfono, WhatsApp).
        /// </summary>
        public static Resultado<List<Empleados>> GetEmpleadosPorContactos(string? telefono, string? whatsapp)
        {
            if (string.IsNullOrWhiteSpace(telefono) && string.IsNullOrWhiteSpace(whatsapp))
                return Resultado<List<Empleados>>.Fail("No se han enviado datos de búsqueda.");

            try
            {
                var resultadoContactos = ContactosNegocio.GetContactosPorNumero(telefono, whatsapp);
                if (!resultadoContactos.Success || resultadoContactos.Data == null)
                    return Resultado<List<Empleados>>.Fail(resultadoContactos.Mensaje);

                var idsPersonas = resultadoContactos.Data
                    .Where(c => c.IdPersona != null)
                    .Select(c => c.IdPersona!.Value)
                    .ToList();

                var resultadoEmpleados = EmpleadosDatos.GetEmpleados();
                if (!resultadoEmpleados.Success || resultadoEmpleados.Data == null)
                    return Resultado<List<Empleados>>.Fail(resultadoEmpleados.Mensaje);

                var empleados = resultadoEmpleados.Data
                    .Where(e => e.Personas != null && idsPersonas.Contains(e.IdPersona))
                    .ToList();

                return Resultado<List<Empleados>>.Ok(empleados);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener empleados por contactos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Empleados>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo empleado junto con sus contactos asociados.
        /// </summary>
        public static Resultado<int> RegistrarEmpleado(Empleados? empleado, List<Contactos>? contactos)
        {
            var validacion = ComprobarEmpleado(empleado, true);
            if (!validacion.Success)
                return Resultado<int>.Fail(validacion.Mensaje);

            empleado = validacion.Data!;
            try
            {
                // Validar persona
                var resultadoPersona = PersonasNegocio.GetPersonaPorDni(empleado.Personas!.Dni);
                Personas? persona = resultadoPersona.Data;

                if (persona != null)
                {
                    empleado.Personas.IdPersona = persona.IdPersona;
                    if (persona.IdPersona == null)
                        return Resultado<int>.Fail("Problemas con el Id de la persona en la BD.");

                    var resultadoModPersona = PersonasNegocio.ModificarPersona(empleado.Personas);
                    if (!resultadoModPersona.Success)
                        return Resultado<int>.Fail(resultadoModPersona.Mensaje);

                    empleado.Personas = null;
                    empleado.IdPersona = persona.IdPersona!.Value;
                }
                else
                {
                    var resultadoRegistroPersona = PersonasNegocio.RegistrarPersona(empleado.Personas);
                    if (!resultadoRegistroPersona.Success || resultadoRegistroPersona.Data <= 0)
                        return Resultado<int>.Fail(resultadoRegistroPersona.Mensaje);

                    empleado.IdPersona = resultadoRegistroPersona.Data;
                    persona = empleado.Personas;
                    persona.IdPersona = empleado.IdPersona;
                }

                if (empleado.IdPersona < 1)
                    return Resultado<int>.Fail("No se pudo asignar persona al empleado.");

                // Estado
                var resultadoEstado = EstadosNegocio.GetEstado("Empleados", "Activo");
                var estado = resultadoEstado.Data ?? new Estados { Indole = "Empleados", Estado = "Activo" };

                if (estado.IdEstado == null || estado.IdEstado < 1)
                {
                    var resultadoRegistroEstado = EstadosNegocio.RegistrarEstado(estado);
                    if (!resultadoRegistroEstado.Success || resultadoRegistroEstado.Data < 1)
                        return Resultado<int>.Fail(resultadoRegistroEstado.Mensaje);

                    empleado.IdEstado = resultadoRegistroEstado.Data;
                }
                else
                {
                    empleado.IdEstado = estado.IdEstado!;
                }

                empleado.Estados = null;
                empleado.Personas = null;

                var resultadoEmpleado = EmpleadosDatos.RegistrarEmpleado(empleado);
                if (!resultadoEmpleado.Success || resultadoEmpleado.Data < 1)
                    return Resultado<int>.Fail(resultadoEmpleado.Mensaje);

                // Gestionar contactos
                var resultadoContactos = PersonasNegocio.GestionarContactosPorPersona(persona, contactos);
                if (!resultadoContactos.Success)
                    Logger.LogError($"Problemas al gestionar contactos del empleado: {resultadoContactos.Mensaje}");

                return Resultado<int>.Ok(resultadoEmpleado.Data, "Empleado registrado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al registrar empleado:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un empleado básico (sin contactos).
        /// </summary>
        public static Resultado<int> RegistrarEmpleadoBasico(Empleados? empleado)
        {
            var validacion = ComprobarEmpleado(empleado, true);
            if (!validacion.Success)
                return Resultado<int>.Fail(validacion.Mensaje);

            empleado = validacion.Data!;
            try
            {
                var resultadoEstado = EstadosNegocio.GetEstado("Empleados", "Activo");
                var estado = resultadoEstado.Data ?? new Estados { Indole = "Empleados", Estado = "Activo" };

                if (estado.IdEstado == null || estado.IdEstado < 1)
                {
                    var resultadoRegistroEstado = EstadosNegocio.RegistrarEstado(estado);
                    if (!resultadoRegistroEstado.Success || resultadoRegistroEstado.Data < 1)
                        return Resultado<int>.Fail(resultadoRegistroEstado.Mensaje);

                    empleado.IdEstado = resultadoRegistroEstado.Data;
                }
                else
                {
                    empleado.IdEstado = estado.IdEstado!;
                }

                empleado.Estados = null;
                empleado.Personas = null;

                var resultadoEmpleado = EmpleadosDatos.RegistrarEmpleado(empleado);
                if (!resultadoEmpleado.Success || resultadoEmpleado.Data < 1)
                    return Resultado<int>.Fail(resultadoEmpleado.Mensaje);

                return Resultado<int>.Ok(resultadoEmpleado.Data, "Empleado básico registrado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al registrar empleado básico:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un empleado existente junto con sus contactos asociados.
        /// </summary>
        public static Resultado<bool> ModificarEmpleado(Empleados? empleado, List<Contactos>? contactos)
        {
            var validacion = ComprobarEmpleado(empleado, false);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            empleado = validacion.Data!;
            try
            {
                if (empleado.IdEstado < 1)
                    return Resultado<bool>.Fail("El estado del empleado no se ha podido encontrar.");

                var resultadoPersona = PersonasNegocio.GetPersonaPorDni(empleado.Personas!.Dni);
                var persona = resultadoPersona.Data;
                if (persona == null || persona.IdPersona == null)
                    return Resultado<bool>.Fail("No se pudo encontrar información de la persona a modificar.");

                empleado.Personas.IdPersona = persona.IdPersona;

                var resultadoModPersona = PersonasNegocio.ModificarPersona(empleado.Personas);
                if (!resultadoModPersona.Success)
                    return Resultado<bool>.Fail(resultadoModPersona.Mensaje);

                empleado.Personas = null;
                empleado.IdPersona = persona.IdPersona!.Value;
                empleado.Estados = null;

                var resultadoEmpleado = EmpleadosDatos.ModificarEmpleado(empleado);
                if (!resultadoEmpleado.Success && !resultadoModPersona.Success)
                {
                    Logger.LogError($"No se pudo modificar el empleado ni revertir los cambios en la persona. Detalles:\nModificar empleado: {resultadoEmpleado.Mensaje}\nDatos Persona: {resultadoModPersona.Mensaje}");
                    return Resultado<bool>.Fail("Error en la modificación (revise Logs)");                     
                }
                
                var resultadoContactos = PersonasNegocio.GestionarContactosPorPersona(persona, contactos);
                if (!resultadoContactos.Success)
                    Logger.LogError($"Problemas al gestionar contactos del empleado: {resultadoContactos.Mensaje}");

                return Resultado<bool>.Ok(true, "Empleado modificado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al modificar empleado:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }
    }
}
