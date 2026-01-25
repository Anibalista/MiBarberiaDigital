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
    public class ProductosDatos
    {
        static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);
            if (!comprobar.ComprobarEntidad(contexto.UnidadesMedidas, ref mensaje))
                return false;
            if (!comprobar.ComprobarEntidad(contexto.Productos, ref mensaje))
                return false;
            return comprobar.ComprobarEntidad(contexto.Proveedores, ref mensaje);
        }

        public static List<Productos>? ListarSimple(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Productos.Include("Categorias").Include("UnidadesMedidas")
                                   .OrderBy(p => p.Descripcion)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener productos:\n{ex.Message}";
                return null;
            }
        }

        public static string? ObtenerCodigoMayorSugerido(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Productos.Where(p => p.CodProducto.Length > 6 && p.CodProducto.Length < 11).Max(p => p.CodProducto);
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener códigos de productos:\n{ex.Message}";
                return null;
            }
        }

        public static Productos? ObtenerProductoPorCodigo(string codigo, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Productos.FirstOrDefault(p => p.CodProducto == codigo);
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener producto por código:\n{ex.Message}";
                return null;
            }
        }

        public static List<UnidadesMedidas>? ListarUnidadesMedida(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.UnidadesMedidas
                                   .OrderBy(u => u.Unidad)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener Unidades de Medida:\n{ex.Message}";
                return null;
            }
        }

        public static bool CambiarEstadoProducto(Productos producto, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    Productos? prod = contexto.Productos.FirstOrDefault(p => p.IdProducto == producto.IdProducto);
                    if (prod == null)
                    {
                        mensaje = "Producto no encontrado";
                        return false;
                    }
                    prod.Activo = producto.Activo;
                    contexto.Update(prod);
                    int exito = contexto.SaveChanges();
                    return exito > 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener producto por código:\n{ex.Message}";
                return false;
            }
        }
    }
}
