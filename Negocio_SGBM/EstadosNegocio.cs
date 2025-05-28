using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class EstadosNegocio
    {
        //Consultas
        public static Estados? getEstado(string? indole, string? descripcion, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(indole) || String.IsNullOrWhiteSpace(descripcion))
            {
                mensaje = "Problemas con la información de estados en la capa negocio";
                return null;
            }
            Estados? estado = EstadosDatos.getEstado(indole, descripcion, ref mensaje);
            return estado;
        }

        public static List<Estados>? getEstadosPorIndole(string? indole, ref string mensaje)
        {
            if (!String.IsNullOrWhiteSpace(indole))
            {
                mensaje = "Problemas con la información de estados en la capa negocio";
                return null;
            }

            List<Estados>? estados = EstadosDatos.getEstadosPorIndole(indole, ref mensaje);
            return estados;
        }

        //Registros
        public static int registrarEstado(Estados? estado, ref string mensaje)
        {
            if (estado == null)
            {
                mensaje = "Problemas al enviar información de estado a la capa negocio";
                return -1;
            }
            estado.IdEstado = null;
            int idEstado = 0;
            try
            {

            }  catch (Exception ex)
            {
                mensaje = ex.Message;
                return -1;
            }
            if (idEstado < 0)
            {
                mensaje = "Problemas al registrar el estado";
                return -1;
            }
            return idEstado;
        }
    }
}
