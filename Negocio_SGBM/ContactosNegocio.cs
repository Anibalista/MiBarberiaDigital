using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class ContactosNegocio
    {
        //Consultas
        public static List<Contactos>? getContactosPorPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "La información de la persona no llega a la capa negocio de contactos";
                return null;
            }
            if (persona.IdPersona == null)
            {
                mensaje = "La información de la persona (Id) no llega a la capa negocio de contactos";
                return null;
            }

            List<Contactos>? contactos = null;
            try
            {
                contactos = ContactosDatos.GetContactosPorPersona(persona, ref mensaje);
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return null;
            }
            return contactos;
        }

        public static List<Contactos>? getContactosPorNumero(string? fijo, string? whatsapp, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(fijo))
            {
                fijo = null;
            }
            if (String.IsNullOrWhiteSpace(whatsapp))
            {
                whatsapp = null;
            }
            if (fijo == null && whatsapp == null)
            {
                return null;
            }
            List<Contactos>? contactos = ContactosDatos.GetContactosPorNumero(fijo, whatsapp, ref mensaje);
            return contactos;
        }

        //Registros
        public static bool registrarContacto(Contactos? contacto, ref string mensaje)
        {
            if (contacto == null)
            {
                mensaje = "La información de contacto no llega a la capa negocio";
                return false;
            }
            int id = ContactosDatos.RegistrarContacto(contacto, ref mensaje);
            
            return id > 0;
        }

        //Modificaciones
        public static bool modificarContacto(Contactos? contacto, ref string mensaje)
        {
            if (contacto == null)
            {
                mensaje = "La información de contacto no llega a la capa negocio";
                return false;
            }
            if (contacto.IdContacto == null)
            {
                mensaje = "La información de contacto (Id) no llega a la capa negocio";
                return false;
            }
            bool exito = false;
            try
            {
                exito = ContactosDatos.ModificarContacto(contacto, ref mensaje);
            } catch (Exception ex)
            {
                mensaje += "\n" + ex.Message;
                return false;
            }
            return exito;
        }

        //Eliminaciones
        public static bool eliminarContacto(Contactos? contacto, ref string mensaje)
        {
            if (contacto == null)
            {
                mensaje = "La información de contacto no llega a la capa negocio";
                return false;
            }
            if (contacto.IdContacto == null)
            {
                mensaje = "La información de contacto (Id) no llega a la capa negocio";
                return false;
            }
            bool exito = false;
            try
            {
                exito = ContactosDatos.EliminarContacto(contacto, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje += "\n" + ex.Message;
                return false;
            }
            return exito;
        }

    }
}
