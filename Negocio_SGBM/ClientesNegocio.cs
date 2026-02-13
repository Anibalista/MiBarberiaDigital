using Datos_SGBM;
using Entidades_SGBM;

namespace Negocio_SGBM
{
    public class ClientesNegocio
    {
        public static Clientes? ComprobarCliente(Clientes? cliente, bool registro, ref string mensaje)
        {
            if (cliente == null)
            {
                mensaje = "Problema al enviar datos de cliente entre capas";
                return null;
            }
            if (cliente.Personas == null)
            {
                mensaje = "Problema al enviar datos de la persona relacionada al cliente entre capas";
                return null;
            }
            if (cliente.IdEstado > 0)
                cliente.Estados = null;
            if (cliente.Estados != null)
                cliente.IdEstado = cliente.Estados.IdEstado ?? 0;
            if (cliente.IdEstado < 1 && !registro)
            {
                mensaje = "Error al asignar un estado al cliente en la capa negocio";
                return null;
            }
            if (!registro && cliente.IdCliente == null)
            {
                mensaje = "Error al mover el Id del cliente a la capa negocio";
                return null;
            }
            else if (registro)
                cliente.IdCliente = null;

            return cliente;
        }

        public static bool ImportarCliente(Clientes? cliente, Contactos? contacto,  ref string mensaje)
        {
            cliente = ComprobarCliente(cliente, true, ref mensaje);
            if (cliente == null)
                return false;

            Personas? persona = PersonasNegocio.GetPersonaPorDni(cliente.Personas.Dni, ref mensaje);
            if (persona == null)
                cliente.IdPersona = PersonasNegocio.RegistrarPersona(cliente.Personas, ref mensaje);
            else
            {
                cliente.IdPersona = (int)persona.IdPersona;
                cliente.Personas.IdPersona = persona.IdPersona;
                if (!PersonasNegocio.modificarPersona(cliente.Personas, ref mensaje))
                    return false;
                
            }
            if (cliente.IdPersona < 1)
                return false;
            
            Clientes? cl = null;
            cl = GetClientePorDni(cliente.Personas.Dni, ref mensaje);
            cliente.Personas = null;
            if (cl == null)
            {
                if (!RegistrarClienteBasico(cliente, ref mensaje))
                    return false;
            }
            if (contacto != null)
            {
                contacto.IdPersona = cliente.IdPersona;
                if (!ContactosNegocio.registrarContacto(contacto, ref mensaje))
                    mensaje += "\nNo se pudo registrar contactos para el dni " + cliente.Personas.Dni;
            }
            return true;
        }

        public static Clientes? GetClientePorDni(string? dni, ref string mensaje)
        {
            Personas? persona = null;
            persona = PersonasNegocio.GetPersonaPorDni(dni, ref mensaje);
            if (persona == null)
            {
                return null;
            }
            if (persona.IdPersona == null)
            {
                return null;
            }
            Clientes? cliente = ClientesDatos.GetClientePorIdPersona((int)persona.IdPersona, ref mensaje);
            if (cliente != null)
            {
                cliente.Personas = persona;
            }
            return cliente;
        }

        public static List<Clientes>? GetListadoDeClientes(string? criterioBusqueda, string? campo1, string? campo2, Localidades? localidad, bool incluirAnulados, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(criterioBusqueda))
            {
                mensaje = "No se ha seleccionado un criterio de búsqueda";
                return null;
            }
            List<Clientes>? clientes = null;

            if (string.IsNullOrWhiteSpace(campo1) && string.IsNullOrWhiteSpace(campo2) && localidad == null)
            {
                clientes = ClientesDatos.GetClientes(ref mensaje);
                return clientes;
            }
            if (criterioBusqueda == "Dni, Nombres")
            {
                clientes = ClientesDatos.GetClientesPorDniNombres(campo1, campo2, ref mensaje);
                return clientes;
            }
            if (criterioBusqueda == "Domicilio")
            {
                clientes = GetClientesPorDomicilio(campo1, campo2, localidad, incluirAnulados, ref mensaje);
                return clientes;
            }
            if (criterioBusqueda == "WhatsApp, Teléfono")
            {
                clientes = GetClientesPorContactos(campo1, campo2, ref mensaje);
                return clientes;
            }
            if (clientes == null)
            {
                mensaje = "No se pudo obtener el listado de clientes: " + mensaje;
            }
            return clientes;
        }

        public static List<Clientes>? GetClientesPorDomicilio(string? calle, string? barrio, Localidades? localidad, bool incluirAnulados, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(calle) && string.IsNullOrWhiteSpace(barrio) && localidad == null)
            {
                mensaje = "No se han enviado datos de búsqueda";
                return null;
            }
            List<Domicilios>? domicilios = DomiciliosDatos.GetDomiciliosPorCampos(calle, barrio, localidad, ref mensaje);
            if (domicilios == null)
            {
                return null;
            }
            List<int> IdDomicilios = domicilios.Select(d => (int)d.IdDomicilio).ToList();
            List<Clientes>? clientesTodos = ClientesDatos.GetClientes(ref mensaje);
            if (clientesTodos == null)
            {
                return null;
            }
            List<Clientes>? clientes = clientesTodos.Where(c => c.Personas != null && c.Personas.Domicilios != null &&
                                                        IdDomicilios.Contains((int)c.Personas.IdDomicilio)).ToList();
            return clientes;
        }

        public static List<Clientes>? GetClientesPorContactos(string? telefono, string? whatsapp, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(telefono) && string.IsNullOrWhiteSpace(whatsapp))
            {
                mensaje = "No se han enviado datos de búsqueda";
                return null;
            }
            List<Contactos>? contactos = ContactosNegocio.getContactosPorNumero(telefono, whatsapp, ref mensaje);
            if (contactos == null)
            {
                return null;
            }
            List<int> IdPersonas = contactos.Where(c => c.IdPersona != null).Select(c => (int)c.IdPersona).ToList();
            List<Clientes>? clientesTodos = ClientesDatos.GetClientes(ref mensaje);
            if (clientesTodos == null)
            {
                return null;
            }
            List<Clientes>? clientes = clientesTodos.Where(c => c.Personas != null && IdPersonas.Contains((int)c.IdPersona)).ToList();
            return clientes;
        }

        public static bool RegistrarCliente(Clientes? cliente, List<Contactos>? contactos, ref string mensaje)
        {
            cliente = ComprobarCliente(cliente, true, ref mensaje);
            if (cliente == null)
                return false;
            
            Personas? persona = PersonasNegocio.GetPersonaPorDni(cliente.Personas.Dni, ref mensaje);
            if (persona != null)
            {
                cliente.Personas.IdPersona = persona.IdPersona;
                if (persona.IdPersona == null)
                {
                    mensaje = "Problemas con el id de la persona en la BD";
                    return false;
                }
                if (!PersonasNegocio.modificarPersona(cliente.Personas, ref mensaje))
                    return false;
                else
                {
                    cliente.Personas = null;
                    cliente.IdPersona = (int)persona.IdPersona;
                }
            }
            else
            {
                cliente.IdPersona = PersonasNegocio.RegistrarPersona(cliente.Personas, ref mensaje);
                persona = cliente.Personas;
                persona.IdPersona = cliente.IdPersona;
            }
            if (cliente.IdPersona < 1)
                return false;
            
            Estados? estado = EstadosNegocio.getEstado("Clientes", "Activo", ref mensaje);
            if (estado == null)
                estado = new Estados { Indole = "Clientes", Estado = "Activo"};

            if (estado.IdEstado == null)
                cliente.IdEstado = EstadosNegocio.registrarEstado(estado, ref mensaje);
            
            if (cliente.IdEstado < 1)
                return false;
            
            cliente.Estados = null;
            cliente.Personas = null;

            int idCliente = 0;

            try
            {
                idCliente = ClientesDatos.RegistrarCliente(cliente, ref mensaje);
            } 
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            string mensajeContactos = "";
            if (!PersonasNegocio.GestionarContactosPorPersona(persona, contactos, ref mensajeContactos))
                mensaje += mensajeContactos;
            
            return idCliente > 0;
        }

        public static bool RegistrarClienteBasico(Clientes? cliente, ref string mensaje)
        {
            cliente = ComprobarCliente(cliente, true, ref mensaje);
            if (cliente == null)
                return false;

            cliente.Estados = EstadosNegocio.getEstado("Clientes", "Activo", ref mensaje);
            if (cliente.Estados != null)
                cliente.IdEstado = cliente.Estados.IdEstado ?? 0;

            if (cliente.IdEstado < 1)
                cliente.IdEstado = EstadosNegocio.registrarEstado(new Estados { Indole = "Clientes", Estado = "Activo" }, ref mensaje);
            
            if (cliente.IdEstado < 1)
                return false;
            
            cliente.Estados = null;
            cliente.Personas = null;

            int idCliente = 0;

            try
            {
                idCliente = ClientesDatos.RegistrarCliente(cliente, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = "Error inesperado en la capa de negocio " + ex.Message;
                return false;
            }
            return idCliente > 0;
        }

        public static bool modificarCliente(Clientes? cliente, List<Contactos>? contactos, ref string mensaje)
        {
            cliente = ComprobarCliente(cliente, false, ref mensaje);

            if (cliente == null)
                return false;
            
            if (cliente.IdEstado < 1)
            {
                mensaje = "El estado del cliente no se ha podido encontrar";
                return false;
            }

            Personas? persona = PersonasNegocio.GetPersonaPorDni(cliente.Personas.Dni, ref mensaje);
            if (persona == null)
            {
                mensaje = "No se pudo encontrar información de la persona a modificar";
                return false;
            }
            if (persona.IdPersona == null)
            {
                mensaje = "Problemas con el id de la persona en la BD";
                return false;
            }

            cliente.Personas.IdPersona = persona.IdPersona;
            
            if (!PersonasNegocio.modificarPersona(cliente.Personas, ref mensaje))
            {
                return false;
            }
            cliente.Personas = null;
            cliente.IdPersona = (int)persona.IdPersona;
            cliente.Estados = null;

            bool exito = false;

            try
            {
                exito = ClientesDatos.ModificarCliente(cliente, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            string mensajeContactos = "";
            if (!PersonasNegocio.GestionarContactosPorPersona(persona, contactos, ref mensajeContactos))
            {
                mensaje += mensajeContactos;
            }
            return exito;
        }
    }
}
