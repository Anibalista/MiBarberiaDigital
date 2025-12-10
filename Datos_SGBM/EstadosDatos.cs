using EF_SGBM;
using Entidades_SGBM;

namespace Datos_SGBM
{
    public class EstadosDatos
    {
        static Contexto contexto;

        //Control de BD
        public static bool comprobarContexto(ref string mensaje)
        {
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return false;
            }
            if (contexto.Estados == null)
            {
                mensaje = "No se conecta a la BD (Estados)";
                return false;
            }
            return true;
        }

        //Control de null
        public static bool comprobarEstado(Estados? estado , bool registro, ref string mensaje)
        {
            if (estado == null)
            {
                mensaje = "No llega la información de estado a la capa datos";
                return false;
            }
            if (String.IsNullOrWhiteSpace(estado.Indole) || String.IsNullOrWhiteSpace(estado.Estado))
            {
                mensaje = "No llega la información de estado a la capa datos (valores de estado)";
                return false;
            }
            if (estado.IdEstado == null)
            {
                mensaje += !registro ? "No llega la información de estado (ID) a la capa datos" : "";
                return registro;
            }
            return true;
        }


        //Consultas
        public static Estados? getEstado(string? indole, string? descripcion, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(indole) || String.IsNullOrWhiteSpace(descripcion))
            {
                mensaje = "No llega la información de estados a la capa de datos";
                return null;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }

            Estados? estado = null;
            try
            {
                estado = contexto.Estados.FirstOrDefault(e => e.Indole.Equals(indole) && e.Estado.Equals(descripcion));
            } catch (Exception ex)
            {
                mensaje = ex.Message + "EstadosDatos";
                return null;
            }
            return estado;
        }

        public static List<Estados>? getEstadosPorIndole(string? indole, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(indole))
            {
                mensaje = "No llega la información de estados a la capa de datos";
                return null;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }

            List<Estados>? estados = null;
            try
            {
                estados = contexto.Estados.Where(e => e.Indole.Equals(indole)).ToList();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + "EstadosDatos";
                return null;
            }
            if (estados == null || estados.Count < 1)
            {
                return null;
            }
            return estados;
        }

        //Registros
        public static int registrarEstado(Estados? estado, ref string mensaje)
        {
            if (!comprobarEstado(estado, true, ref mensaje))
            {
                return -1;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return -1;
            }
            try
            {
                contexto.Estados.Add(estado);
                contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message + "EstadosDatos";
                return -1;
            }
            if (estado.IdEstado != null)
            {
                return (int)estado.IdEstado;
            }
            mensaje = "Problemas desconocidos en el registro de estados";
            return 0;
        }
    }
}
