using EF_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class CategoriasDatos
    {

        static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);
            if (!comprobar.Comprobar(ref mensaje))
                return false;
            if (!comprobar.ComprobarCategorias(ref mensaje)) 
                return false;
            return true;
        }

        static bool ComprobarCategoria(Categorias? categoria, bool registro, ref string mensaje)
        {
            if (categoria == null)
            {
                mensaje = "La información de la categoría no llega a la capa de datos";
                return false;
            }
            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
            {
                mensaje = "La descripción de la categoría llegó vacía a la capa datos";
                return false;
            }
            if (registro)
                return true;
            if (categoria.IdCategoria == null || categoria.IdCategoria < 1)
            {
                mensaje = "Error con el Id de la Categoría a manipular";
                return false;
            }
            return true;
        }

        public static List<Categorias>? ListaCategorias(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Categorias
                                   .OrderBy(c => c.Descripcion)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener categorías:\n{ex.Message}";
                return null;
            }
        }

    }
}
