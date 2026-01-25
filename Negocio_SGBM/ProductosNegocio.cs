using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class ProductosNegocio
    {
        static bool ComprobarProducto(Productos? producto, bool registro, ref string mensaje)
        {
            if (producto == null)
            {
                mensaje = "La información del producto no llega a la consulta";
                return false;
            }
            if (string.IsNullOrWhiteSpace(producto.CodProducto))
            {
                mensaje = "El Código del producto no llega a la consulta";
                return false;
            }
            if (string.IsNullOrWhiteSpace(producto.Descripcion))
            {
                mensaje = "La descripción del producto no llega a la consulta";
                return false;
            }
            if (registro)
                return true;
            if (producto.IdProducto == null || producto.IdProducto < 1)
            {
                mensaje = "Error con el Id del producto a manipular";
                return false;
            }
            return true;
        }

        public static List<Productos>? ListaSimple(ref string mensaje)
        {
            try
            {
                List<Productos>? lista = ProductosDatos.ListarSimple(ref mensaje);
                return lista;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la búsqueda de Productos:\n{ex.Message}";
                return null;
            }
        }

        public static string SugerirCodigo(ref string mensaje)
        {
            string codigo = "1000000";
            try
            {
                string? mayor = ProductosDatos.ObtenerCodigoMayorSugerido(ref mensaje);
                if (string.IsNullOrWhiteSpace(mayor))
                    return codigo;
                int codNumeico = 0;
                if (!int.TryParse(mayor, out codNumeico))
                {
                    mensaje += "Error al convertir código a número";
                    return codigo;
                }
                codNumeico++;
                return codNumeico.ToString();
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la búsqueda de Productos:\n{ex.Message}";
                return "1000000";
            }
        }

        public static Productos? BuscarPorCodigo(string codigo, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                mensaje = "El código llegó vacío a la capa negocio";
                return null;
            }
            try
            {
                Productos? producto = ProductosDatos.ObtenerProductoPorCodigo(codigo,  ref mensaje);
                return producto;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la consulta de disponibilidad del código:\n{ex.Message}";
                return null;
            }
        }

        public static List<UnidadesMedidas>? ListarUnidadesMedida(ref string mensaje)
        {
            try
            {
                List<UnidadesMedidas>? lista = ProductosDatos.ListarUnidadesMedida(ref mensaje);
                return lista;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la búsqueda de Unidades de Medida:\n{ex.Message}";
                return null;
            }
        }

        public static bool CambiarEstadoProducto(Productos? producto, ref string mensaje)
        {
            if (!ComprobarProducto(producto, false, ref mensaje))
                return false;
            try
            {
                producto.Activo = !producto.Activo;
                return ProductosDatos.CambiarEstadoProducto(producto, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la modificación del Productos:\n{ex.Message}";
                return false;
            }
        }
    }
}
