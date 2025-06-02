using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class ClientesDatos
    {
        static Contexto contexto;
        //Comprobaciones
        public static bool comprobarContexto(ref string mensaje)
        {
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return false;
            }
            if (contexto.Estados == null || contexto.Clientes == null)
            {
                mensaje = "No se conecta a la BD (Clientes o estados)";
                return false;
            }
            if (contexto.Personas == null)
            {
                mensaje = "No se conecta a la BD (Personas)";
                return false;
            }
            return true;
        }

        public static bool comprobarCliente(Clientes? cliente, bool registro, ref string mensaje)
        {
            if (cliente == null)
            {
                mensaje = "La información del cliente no llega a la consulta de la BD";
                return false;
            }
            if (cliente.IdEstado < 1)
            {
                mensaje = "La información del cliente (Estado) no llega a la consulta de la BD";
                return false;
            }
            if (!registro)
            {
                if (cliente.IdCliente == null)
                {
                    mensaje = "La información del cliente (Id) no llega a la consulta de la BD";
                    return false;
                }
                return true;
            }
            if (cliente.Personas == null && cliente.IdPersona < 1)
            {
                mensaje = "La informacion personal del cliente no llega a la capa de datos";
                return false;
            }
            return true;
        }

        //Consulta
        public static Clientes? getClientePorIdPersona(int idPersona, ref string mensaje)
        {
            if (idPersona < 1)
            {
                mensaje = "El ID de la persona no llega a la consulta";
                return null;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }
            Clientes? cliente = null;
            try
            {
                cliente = contexto.Clientes.Include("Estados").Where(c => c.IdPersona == idPersona).FirstOrDefault();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + "ClientesDatos";
                return null;
            }

            return cliente;
        }


        //Registro
        public static int registrarCliente (Clientes? cliente, ref string mensaje)
        {
            if (!comprobarCliente(cliente, true, ref mensaje))
            {
                return -1;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return -1;
            }
            int id = 0;
            try
            {
                contexto.Clientes.Add(cliente);
                id = contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message + "ClientesDatos";
                return -1;
            }
            return id;
        }

        //Modificaciones
        public static bool modificarCliente(Clientes? cliente, ref string mensaje)
        {
            if (!comprobarCliente(cliente, false, ref mensaje))
            {
                return false;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return false;
            }
            Clientes? c = null;
            try
            {
                c = contexto.Clientes.FirstOrDefault(cl => cl.IdCliente == cliente.IdCliente);
            } catch (Exception ex)
            {
                mensaje = ex.Message + "ClientesDatos";
                return false;
            }
            if (c == null)
            {
                mensaje = "Problemas al obtener la información de registro del cliente";
                return false;
            }
            c.IdEstado = cliente.IdEstado;
            c.IdPersona = cliente.IdPersona;
            int exito = 0;
            try
            {
                contexto.Clientes.Update(c);
                exito = contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message + "ClientesDatos";
                return false;
            }
            return exito > 0;
        }
    }
}
