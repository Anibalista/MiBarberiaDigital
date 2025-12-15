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
    }
}
