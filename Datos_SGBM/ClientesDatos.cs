using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;

namespace Datos_SGBM
{
    public class ClientesDatos
    {
        static Contexto contexto;
        //Comprobaciones
        public static bool comprobarContexto(ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);
            if (!comprobar.Comprobar(ref mensaje))
                return false;
            if (!comprobar.ComprobarEstados(ref mensaje))
                return false;
            if (!comprobar.ComprobarClientes(ref mensaje)) 
                return false;
            if (!comprobar.ComprobarPersonas(ref mensaje)) 
                return false;
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

        public static List<Clientes>? getClientes(ref string mensaje)
        {
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }
            List<Clientes>? clientes = null;
            try
            {
                clientes = contexto.Clientes.Include(c => c.Estados)
                            .Include(c => c.Personas)
                                .ThenInclude(p => p.Domicilios)
                                    .ThenInclude(d => d.Localidades).ToList();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + "ClientesDatos";
                return null;
            }
            return clientes;
        }

        public static List<Clientes>? getClientesPorDniNombres(string? dni, string? nombres, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(dni) && String.IsNullOrWhiteSpace(nombres))
            {
                mensaje = "No llegan los datos de búsqueda";
                return null;
            }

            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }

            List<Clientes>? clientes = null;
            try
            {
                clientes = contexto.Clientes.Include(c => c.Estados)
                            .Include(c => c.Personas)
                                .ThenInclude(p => p.Domicilios)
                                    .ThenInclude(d => d.Localidades)
                            .Where(c => c.Personas != null && 
                                    ((c.Personas.Dni.Contains(dni ?? "")) &&
                                     (c.Personas.Nombres.Contains(nombres ?? ""))))
                            .OrderBy(c => c.Personas.Apellidos)
                            .ThenBy(c => c.Personas.Nombres)
                            .ToList();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + "ClientesDatos";
                return null;
            }
            return clientes;
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
