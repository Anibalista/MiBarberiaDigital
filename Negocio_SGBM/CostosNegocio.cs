using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class CostosNegocio
    {
        static bool comprobarCosto(CostosServicios? costo, bool registro, ref string mensaje)
        {
            if (costo == null)
            {
                mensaje = "No llega el costo del servicio a la consulta";
                return false;
            }
            if (costo.Costo <= 0)
            {
                mensaje = "No llega el monto del costo del servicio a la consulta";
                return false;
            }
            if (string.IsNullOrWhiteSpace(costo.Descripcion))
            {
                mensaje = "No llega la descipción del costo del servicio a la consulta";
                return false;
            }
            if (costo.IdCostoServicio == null && !registro)
            {
                mensaje = "Error al ligar el costo al servicio";
                return false;
            }
            return true;
        }

        public static List<CostosServicios>? ObtenerInsumosPorIdServicio(int id, ref string mensaje)
        {
            if (id < 1) 
                return null;
            try
            {
                List<CostosServicios>? lista = CostosDatos.ObtenerCostosPorIdServicio(id, ref mensaje);
                return lista;
            }
            catch (Exception ex)
            {
                mensaje = "Error al obtener los insumos-costos:\n" + ex.Message;
                return null;
            }
        }

        public static bool RegistrarListaCostos(List<CostosServicios>? costos, int idServicio, ref string mensaje)
        {
            if (costos == null)
            {
                mensaje = "no llegan los costos-insumos a registrar a la capa negocio";
                return false;
            }
            if (idServicio <= 0)
            {
                mensaje = "no llega el id del servicio a registrar a la capa negocio";
                return false;
            }
            try
            {
                mensaje = string.Empty;
                foreach (CostosServicios costo in costos)
                {
                    string errores = string.Empty;
                    costo.IdCostoServicio = null;
                    costo.IdServicio = idServicio;
                    if (!comprobarCosto(costo, true, ref errores))
                    {
                        mensaje += "\n" + errores;
                        continue;
                    }

                    if (costo.Productos != null)
                        costo.IdProducto = costo.Productos.IdProducto;
                    if (costo.IdProducto != null)
                        costo.Productos = null;

                    costo.IdCostoServicio = CostosDatos.Registrar(costo, ref errores);
                    if (costo.IdCostoServicio <= 0)
                        mensaje += "\n" + errores;
                }
                bool exito = string.IsNullOrWhiteSpace(mensaje);
                if (!exito)
                    mensaje = $"Problemas al registrar los costos insumos";
                return exito;
            }
            catch (Exception ex)
            {
                mensaje = $"Error inesperado al registrar costos-insumo\n{ex.Message}";
                return false;
            }
        }
    }
}
