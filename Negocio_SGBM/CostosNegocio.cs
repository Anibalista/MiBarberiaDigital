using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public static bool ModificarCosto(CostosServicios? costo, ref string mensaje)
        {
            if (costo == null)
            {
                mensaje = "no llega el costo-insumo a modificar a la capa negocio";
                return false;
            }
            try
            {
                string error = string.Empty;
                mensaje = string.Empty;
                if (!comprobarCosto(costo, false, ref error))
                {
                    mensaje += "\n" + error;
                }
                if (costo.Productos != null)
                    costo.IdProducto = costo.Productos.IdProducto;
                if (costo.IdProducto != null)
                    costo.Productos = null;

                if (!CostosDatos.Modificar(costo, ref error))
                    mensaje += "\n" + error;

                bool exito = string.IsNullOrWhiteSpace(mensaje);
                if (!exito)
                    mensaje = $"Problemas al eliminar el costo-insumo";
                return exito;
            }
            catch (Exception ex)
            {
                mensaje = $"Error inesperado al eliminar el costo-insumo\n{ex.Message}";
                return false;
            }
        }

        public static bool EliminarCosto(CostosServicios? costo, ref string mensaje)
        {
            if (costo == null)
            {
                mensaje = "no llega el costo-insumo a eliminar a la capa negocio";
                return false;
            }
            try
            {
                string error = string.Empty;
                mensaje = string.Empty;
                if (!comprobarCosto(costo, false, ref error))
                {
                    mensaje += "\n" + error;
                }
                if (costo.Productos != null)
                    costo.IdProducto = costo.Productos.IdProducto;
                if (costo.IdProducto != null)
                    costo.Productos = null;

                if (!CostosDatos.EliminarFisico(costo, ref error))
                    mensaje += "\n" + error;

                bool exito = string.IsNullOrWhiteSpace(mensaje);
                if (!exito)
                    mensaje = $"Problemas al eliminar el costo-insumo";
                return exito;
            }
            catch (Exception ex)
            {
                mensaje = $"Error inesperado al eliminar el costo-insumo\n{ex.Message}";
                return false;
            }
        }

        public static bool GestionarCostosServicios(List<CostosServicios>? costos, int idServicio, ref string mensaje)
        {
            if (costos == null)
            {
                mensaje = "No llegan los costos-insumos a gestionar en la capa negocio";
                return false;
            }
            if (idServicio == 0)
            {
                mensaje = "No llega el id del servicio a gestionar en la capa negocio de costos-insumos";
                return false;
            }

            try
            {
                bool exito = true;
                string error = string.Empty;
                mensaje = string.Empty;

                var registrar = costos.Where(c => c.IdCostoServicio == null || c.IdCostoServicio <= 0).ToList();
                List<CostosServicios>? anteriores = null;

                // Solo traigo anteriores si no son todos nuevos
                if (registrar.Count != costos.Count)
                {
                    anteriores = ObtenerInsumosPorIdServicio(idServicio, ref error);
                }

                if (anteriores != null)
                {
                    var ids = costos.Select(c => c.IdCostoServicio).ToList();

                    foreach (var costo in anteriores)
                    {
                        error = string.Empty;
                        bool correcto = false;

                        if (!ids.Contains(costo.IdCostoServicio))
                        {
                            correcto = EliminarCosto(costo, ref error);
                        }
                        else
                        {
                            var modificar = costos.FirstOrDefault(c => c.IdCostoServicio == costo.IdCostoServicio);
                            if (modificar != null)
                            {
                                modificar.IdServicio = idServicio;
                                correcto = ModificarCosto(modificar, ref error);
                            }
                        }

                        exito = exito && correcto;

                        if (!string.IsNullOrWhiteSpace(error))
                            mensaje += "\n" + error;
                    }
                }

                if (registrar.Any())
                {
                    error = string.Empty;
                    bool correcto = RegistrarListaCostos(registrar, idServicio, ref error);

                    if (!string.IsNullOrWhiteSpace(error))
                        mensaje += "\n" + error;

                    exito = exito && correcto;
                }

                return exito;
            }
            catch (Exception ex)
            {
                mensaje = $"Error inesperado al gestionar costos-insumos\n{ex.Message}";
                return false;
            }
        }
    }
}
