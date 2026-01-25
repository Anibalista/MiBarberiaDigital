using EF_SGBM;
using Entidades_SGBM;

namespace Datos_SGBM
{
    public class CategoriasDatos
    {

        static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);
            return comprobar.ComprobarEntidad(contexto.Categorias, ref mensaje);
        }


        public static List<Categorias>? ListaCategoriasPorIndole(string indole, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Categorias.Where(c => c.Indole.ToLower() == indole.ToLower()
                                    || string.IsNullOrWhiteSpace(indole))
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
