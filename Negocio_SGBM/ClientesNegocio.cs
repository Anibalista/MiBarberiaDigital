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


        //Registros
        public static bool registrarCliente(Clientes? cliente, List<Contactos>? contactos, ref string mensaje)
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

        
    }
}
