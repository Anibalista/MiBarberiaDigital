using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class CostosDatos
    {
        static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);
            if (!comprobar.Comprobar(ref mensaje))
                return false;
            if (!comprobar.ComprobarCostosServicios(ref mensaje))
                return false;
            if (!comprobar.ComprobarProductos(ref mensaje))
                return false;
            return true;
        }

        public static List<CostosServicios>? ObtenerCostosPorIdServicio(int id, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.CostosServicios.Where(c => c.IdServicio == id)
                                   .Include(c => c.Productos)
                                   .OrderBy(c => c.Descripcion)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener insumos - costos:\n{ex.Message}";
                return null;
            }
        }

        public static int Registrar(CostosServicios? costo, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return 0;
                    costo.IdCostoServicio = null;
                    contexto.Add(costo);
                    contexto.SaveChanges();
                    if (costo.IdCostoServicio != null)
                        return (int)costo.IdCostoServicio;
                    else
                    {
                        mensaje = "No se pudo obtener el Id del costo-insumo registrado.";
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al registrar costo-insumo{costo.Descripcion}:\n{ex.Message}";
                return 0;
            }
        }

        public static bool Modificar(CostosServicios? costo, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    contexto.CostosServicios.Update(costo);
                    int exito = contexto.SaveChanges();
                    mensaje = exito > 0 ? "" : "No se pudo modificar el costo-insumo";
                    return exito > 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar costo-insumo:\n{ex.Message}";
                return false;
            }
        }

        public static bool EliminarFisico(CostosServicios? costo, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    contexto.CostosServicios.Remove(costo);
                    int exito = contexto.SaveChanges();
                    mensaje = exito > 0 ? "" : "No se pudo eliminar el costo-insumo";
                    return exito > 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al eliminar costo-insumo:\n{ex.Message}";
                return false;
            }
        }
    }
}
