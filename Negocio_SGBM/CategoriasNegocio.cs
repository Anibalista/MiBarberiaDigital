using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class CategoriasNegocio
    {
        public static List<Categorias>? Listar(ref string mensaje)
        {
            try
            {
                List<Categorias>? lista = CategoriasDatos.ListaCategorias(ref mensaje);
                return lista;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la búsqueda de Categorías:\n{ex.Message}";
                return null;
            }
        }
    }
}
