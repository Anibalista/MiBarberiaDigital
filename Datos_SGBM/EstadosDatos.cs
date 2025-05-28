using EF_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class EstadosDatos
    {
        static Contexto contexto;

        //Consultas
        public static Estados? getEstado(string? indole, string? descripcion, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(indole) || String.IsNullOrWhiteSpace(descripcion))
            {
                mensaje = "No llega la información de estados a la capa de datos";
                return null;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return null;
            }
            if (contexto.Estados == null)
            {
                mensaje = "No se conecta a la BD (Estados)";
                return null;
            }
            Estados? estado = null;
            try
            {
                estado = contexto.Estados.FirstOrDefault(e => e.Indole.Equals(indole) && e.Estado.Equals(estado));
            } catch (Exception ex)
            {
                mensaje = ex.Message;
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
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return null;
            }
            if (contexto.Estados == null)
            {
                mensaje = "No se conecta a la BD (Estados)";
                return null;
            }
            List<Estados>? estados = null;
            try
            {
                estados = contexto.Estados.Where(e => e.Indole.Equals(indole)).ToList();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return null;
            }
            if (estados == null || estados.Count < 1)
            {
                return null;
            }
            return estados;
        }
    }
}
