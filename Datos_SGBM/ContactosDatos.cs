using EF_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class ContactosDatos
    {
        static Contexto contexto;
        
        //Consultas
        public static List<Contactos>? getContactosPorPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "La información de persona no llega a la consulta de contactos";
                return null;
            }
            if (persona.IdPersona == null)
            {
                mensaje = "La información de persona no llega a la consulta de contactos";
                return null;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "Problemas de conexión a la BD";
                return null;
            }
            if (contexto.Contactos == null)
            {
                mensaje = "Problemas de conexión a la BD (contactos)";
                return null;
            }
            List<Contactos>? contactos = null;
            try
            {
                contactos = contexto.Contactos.Where(c => c.IdPersona == persona.IdPersona).ToList();
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return null;
            }
            return contactos;
        }

        //Registros
        public static int registrarContacto(Contactos? contacto, ref string mensaje)
        {
            if (contacto == null)
            {
                mensaje = "No llegan la información de contacto a la capa datos";
                return -1;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "Problemas de conexión a la BD";
                return -1;
            }
            if (contexto.Contactos == null)
            {
                mensaje = "Problemas de conexión a la BD (contactos)";
                return -1;
            }
            contacto.Personas = null;
            contacto.Proveedores = null;
            contacto.IdContacto = null;
            int exito = 0;
            try
            {
                contexto.Contactos.Add(contacto);
                 exito = contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return -1;
            }
            if (contacto.IdContacto != null)
            {
                exito = (int)contacto.IdContacto;
            }
            return exito;
        }

        //Modificaciones
        public static bool modificarContacto(Contactos? contacto, ref string mensaje)
        {
            contexto = new Contexto();
            Contactos? cont = null;
            if (comprobarContactoYContexto(contacto, ref mensaje))
            {
                try
                {
                    cont = contexto.Contactos.FirstOrDefault(c => c.IdContacto == contacto.IdContacto);
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    return false;
                }
            }
            if (cont == null)
            {
                return false;
            }
            cont.Whatsapp = contacto.Whatsapp;
            cont.Telefono = contacto.Telefono;
            cont.Email = contacto.Email;
            cont.Facebook = contacto.Facebook;
            cont.Instagram = contacto.Instagram;
            cont.IdPersona = contacto.IdPersona;
            cont.IdProveedor = contacto.IdProveedor;
            int exito = 0;
            try
            {
                contexto.Contactos.Update(cont);
                exito = contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            return exito > 0;
        }

        //Eliminaciones
        public static bool eliminarContacto(Contactos? contacto, ref string mensaje)
        {
            contexto = new Contexto();
            Contactos? cont = null;
            if (comprobarContactoYContexto(contacto, ref mensaje))
            {
                try
                {
                    cont = contexto.Contactos.FirstOrDefault(c => c.IdContacto == contacto.IdContacto);
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    return false;
                }
            }
            
            if (cont == null)
            {
                mensaje = "No se encuentra el contacto a eliminar";
                return false;
            }
            int exito = 0;
            try
            {
                contexto.Contactos.Remove(cont);
                exito = contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            return exito > 0;
        }

        public static bool comprobarContactoYContexto(Contactos? contacto, ref string mensaje)
        {
            if (contacto == null)
            {
                mensaje = "No llegan la información de contacto a la capa datos";
                return false;
            }
            if (contacto.IdContacto == null)
            {
                mensaje = "No llegan la información de contacto (Id) a la capa datos";
                return false;
            }
            if (contexto == null)
            {
                mensaje = "Problemas de conexión a la BD";
                return false;
            }
            if (contexto.Contactos == null)
            {
                mensaje = "Problemas de conexión a la BD (contactos)";
                return false;
            }
            return true;
        }
    }
}
