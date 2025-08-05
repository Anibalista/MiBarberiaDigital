using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class ClientesNegocio
    {
        //Comprobaciones
        public static bool comprobarCliente(Clientes? cliente, ref string mensaje)
        {
            if (cliente == null)
            {
                mensaje = "Problema al enviar datos de cliente entre capas";
                return false;
            }
            if (cliente.Personas == null)
            {
                mensaje = "Problema al enviar datos de cliente entre capas";
                return false;
            }
            return true;
        }

        public static bool importarCliente(Clientes? cliente, Contactos? contacto,  ref string mensaje)
        {
            if (!comprobarCliente(cliente, ref mensaje))
            {
                return false;
            }
            Personas? persona = PersonasNegocio.getPersonaPorDni(cliente.Personas.Dni, ref mensaje);
            if (persona == null)
            {
                cliente.IdPersona = PersonasNegocio.registrarPersona(cliente.Personas, ref mensaje);
            } else
            {
                cliente.IdPersona = (int)persona.IdPersona;
                cliente.Personas.IdPersona = persona.IdPersona;
                if (!PersonasNegocio.modificarPersona(cliente.Personas, ref mensaje))
                {
                    return false;
                }
            }
            if (cliente.IdPersona < 1)
            {
                return false;
            }
            Clientes? cl = null;
            cl = getClientePorDni(cliente.Personas.Dni, ref mensaje);
            cliente.Personas = null;
            if (cl == null)
            {
                if (!registrarClienteBasico(cliente, ref mensaje))
                {
                    return false;
                }
            }
            if (contacto != null)
            {
                contacto.IdPersona = cliente.IdPersona;
                if (!ContactosNegocio.registrarContacto(contacto, ref mensaje))
                {
                    mensaje += "\nNo se pudo registrar contactos para el dni " + cliente.Personas.Dni;
                }
            }
            return true;

        }


        //Consultas
        public static Clientes? getClientePorDni(string? dni, ref string mensaje)
        {
            Personas? persona = null;
            persona = PersonasNegocio.getPersonaPorDni(dni, ref mensaje);
            if (persona == null)
            {
                return null;
            }
            if (persona.IdPersona == null)
            {
                return null;
            }
            Clientes? cliente = ClientesDatos.getClientePorIdPersona((int)persona.IdPersona, ref mensaje);
            if (cliente != null)
            {
                cliente.Personas = persona;
            }
            return cliente;
        }

        public static List<Clientes>? getListadoDeClientes(string? criterioBusqueda, string? campo1, string? campo2, Localidades? localidad, bool incluirAnulados, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(criterioBusqueda))
            {
                mensaje = "No se ha seleccionado un criterio de búsqueda";
                return null;
            }
            List<Clientes>? clientes = null;

            if (string.IsNullOrWhiteSpace(campo1) && string.IsNullOrWhiteSpace(campo2) && localidad == null)
            {
                clientes = ClientesDatos.getClientes(ref mensaje);
                return clientes;
            }
            if (criterioBusqueda == "Dni, Nombres")
            {
                clientes = ClientesDatos.getClientesPorDniNombres(campo1, campo2, ref mensaje);
                return clientes;
            }
            if (criterioBusqueda == "Domicilio")
            {
                clientes = getClientesPorDomicilio(campo1, campo2, localidad, incluirAnulados, ref mensaje);
                return clientes;
            }
            if (criterioBusqueda == "WhatsApp, Teléfono")
            {
                clientes = getClientesPorContactos(campo1, campo2, ref mensaje);
                return clientes;
            }
            if (clientes == null)
            {
                mensaje = "No se pudo obtener el listado de clientes: " + mensaje;
            }
            return clientes;
        }

        public static List<Clientes>? getClientesPorDomicilio(string? calle, string? barrio, Localidades? localidad, bool incluirAnulados, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(calle) && string.IsNullOrWhiteSpace(barrio) && localidad == null)
            {
                mensaje = "No se han enviado datos de búsqueda";
                return null;
            }
            List<Domicilios>? domicilios = DomiciliosDatos.getDomiciliosPorCampos(calle, barrio, localidad, ref mensaje);
            if (domicilios == null)
            {
                return null;
            }
            List<int> IdDomicilios = domicilios.Select(d => (int)d.IdDomicilio).ToList();
            List<Clientes>? clientesTodos = ClientesDatos.getClientes(ref mensaje);
            if (clientesTodos == null)
            {
                return null;
            }
            List<Clientes>? clientes = clientesTodos.Where(c => c.Personas != null && c.Personas.Domicilios != null &&
                                                        IdDomicilios.Contains((int)c.Personas.IdDomicilio)).ToList();
            return clientes;
        }

        public static List<Clientes>? getClientesPorContactos(string? telefono, string? whatsapp, ref string mensaje)
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
            List<Clientes>? clientesTodos = ClientesDatos.getClientes(ref mensaje);
            if (clientesTodos == null)
            {
                return null;
            }
            List<Clientes>? clientes = clientesTodos.Where(c => c.Personas != null && IdPersonas.Contains((int)c.IdPersona)).ToList();
            return clientes;
        }

        //Registros
        public static bool registrarCliente(Clientes? cliente, List<Contactos>? contactos, ref string mensaje)
        {
            if (!comprobarCliente(cliente, ref mensaje))
            {
                return false;
            }

            Personas? persona = PersonasNegocio.getPersonaPorDni(cliente.Personas.Dni, ref mensaje);
            if (persona != null)
            {
                cliente.Personas.IdPersona = persona.IdPersona;
                if (persona.IdPersona == null)
                {
                    mensaje = "Problemas con el id de la persona en la BD";
                    return false;
                }
                if (!PersonasNegocio.modificarPersona(cliente.Personas, ref mensaje))
                {
                    return false;
                }
                else
                {
                    cliente.Personas = null;
                    cliente.IdPersona = (int)persona.IdPersona;
                }
            } else
            {
                cliente.IdPersona = PersonasNegocio.registrarPersona(cliente.Personas, ref mensaje);
                persona = cliente.Personas;
                persona.IdPersona = cliente.IdPersona;
            }
            if (cliente.IdPersona < 1)
            {
                return false;
            }
            Estados? estado = EstadosNegocio.getEstado("Clientes", "Activo", ref mensaje);
            if (estado == null)
            {
                estado = new();
                estado.Indole = "Clientes";
                estado.Estado = "Activo";

            }
            if (estado.IdEstado == null)
            {
                cliente.IdEstado = EstadosNegocio.registrarEstado(estado, ref mensaje);
            }
            if (cliente.IdEstado < 1)
            {
                return false;
            }
            cliente.Estados = null;
            cliente.Personas = null;

            int idCliente = 0;

            try
            {
                idCliente = ClientesDatos.registrarCliente(cliente, ref mensaje);
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            string mensajeContactos = "";
            if (!PersonasNegocio.gestionarContactosPorPersona(persona, contactos, ref mensajeContactos))
            {
                mensaje += mensajeContactos;
            }
            return idCliente > 0;

        }

        public static bool registrarClienteBasico(Clientes? cliente, ref string mensaje)
        {
            if (cliente == null)
            {
                return false;
            }
            if (cliente.IdPersona < 1)
            {
                return false;
            }
            Estados? estado = EstadosNegocio.getEstado("Clientes", "Activo", ref mensaje);
            if (estado == null)
            {
                estado = new();
                estado.Indole = "Clientes";
                estado.Estado = "Activo";

            }
            if (estado.IdEstado == null)
            {
                cliente.IdEstado = EstadosNegocio.registrarEstado(estado, ref mensaje);
            }
            if (cliente.IdEstado < 1)
            {
                return false;
            }
            cliente.Estados = null;
            cliente.Personas = null;

            int idCliente = 0;

            try
            {
                idCliente = ClientesDatos.registrarCliente(cliente, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            return idCliente > 0;
        }

        //Modificaciones
        public static bool modificarCliente(Clientes? cliente, List<Contactos>? contactos, ref string mensaje)
        {
            if (!comprobarCliente(cliente, ref mensaje))
            {
                return false;
            }
            if (cliente.IdEstado < 1)
            {
                mensaje = "El estado del cliente no se ha podido encontrar";
                return false;
            }

            Personas? persona = PersonasNegocio.getPersonaPorDni(cliente.Personas.Dni, ref mensaje);
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
                exito = ClientesDatos.modificarCliente(cliente, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            string mensajeContactos = "";
            if (!PersonasNegocio.gestionarContactosPorPersona(persona, contactos, ref mensajeContactos))
            {
                mensaje += mensajeContactos;
            }
            return exito;
        }
    }
}
