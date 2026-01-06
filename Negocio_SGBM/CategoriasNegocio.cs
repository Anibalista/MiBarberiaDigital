using Datos_SGBM;
using Entidades_SGBM;

namespace Negocio_SGBM
{
    public class CategoriasNegocio
    {

        static bool ComprobarCategoria(Categorias? categoria, bool registro, ref string mensaje)
        {
            if (categoria == null)
            {
                mensaje = "La información de la categoría no llega a la consulta";
                return false;
            }
            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
            {
                mensaje = "La descripción de la categoría llegó vacía a la consulta";
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

        public static int Registrar(Categorias? categoria, ref string mensaje)
        {
            if (!ComprobarCategoria(categoria, true, ref mensaje))
                return 0;
            try
            {
                int exito = CategoriasDatos.RegistrarCategoria(categoria, ref mensaje);
                mensaje = exito > 0 ? "\nCategoría Registrada Correctamente" : "\nCategoría No Registrada";
                return exito;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al registrar la categoría:\n{ex.Message}";
                return 0;
            }
        }
    }
}
