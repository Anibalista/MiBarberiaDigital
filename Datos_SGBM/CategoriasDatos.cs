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
            return comprobar.ComprobarEntidad(contexto.Categorias, ref mensaje);
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

        public static int RegistrarCategoria(Categorias? categoria, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return 0;
                    categoria.IdCategoria = null;
                    contexto.Categorias.Add(categoria);
                    contexto.SaveChanges();
                    if (categoria.IdCategoria != null)
                        return (int)categoria.IdCategoria;
                    else
                    {
                        mensaje = "No se pudo obtener el Id de la categoría registrada.";
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al registrar la categoría:\n{ex.Message}";
                return 0;
            }
        }

    }
}
