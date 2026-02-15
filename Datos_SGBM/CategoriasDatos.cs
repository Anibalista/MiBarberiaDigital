using EF_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Categorias.
    /// Contiene métodos de comprobación de contexto y operaciones CRUD.
    /// </summary>
    public class CategoriasDatos
    {
        /// <summary>
        /// Comprueba que el contexto y la entidad Categorias estén disponibles.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <returns>
        /// Resultado indicando éxito o fallo en la comprobación.
        /// </returns>
        public static Resultado<bool> ComprobarContexto(Contexto contexto)
        {
            ComprobacionContexto comprobacion = new ComprobacionContexto(contexto);
            var resultado = comprobacion.ComprobarEntidad(contexto.Categorias);
            if (!resultado.Success)
                return Resultado<bool>.Fail(resultado.Mensaje);
            return Resultado<bool>.Ok(true);
        }

        /// <summary>
        /// Obtiene una lista de categorías filtradas por índole.
        /// </summary>
        /// <param name="indole">Texto de índole para filtrar. Si está vacío, devuelve todas.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>
        /// Resultado con la lista de categorías o mensaje de error.
        /// </returns>
        public static Resultado<List<Categorias>> ListaCategoriasPorIndole(string indole, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<List<Categorias>>.Fail(resultadoContexto.Mensaje);

                    var query = contexto.Categorias
                                        .Where(c => c.Indole.ToLower() == indole.ToLower()
                                                 || string.IsNullOrWhiteSpace(indole))
                                        .OrderBy(c => c.Descripcion);

                    return Resultado<List<Categorias>>.Ok(query.ToList());
                }
            }
            catch (Exception ex)
            {
                return Resultado<List<Categorias>>.Fail($"Error al obtener categorías:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Registra una nueva categoría en la base de datos.
        /// </summary>
        /// <param name="categoria">Objeto Categorias a registrar.</param>
        /// <returns>
        /// Resultado con el Id de la categoría registrada o mensaje de error.
        /// </returns>
        public static Resultado<int> RegistrarCategoria(Categorias? categoria)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<int>.Fail(resultadoContexto.Mensaje);

                    if (categoria == null)
                        return Resultado<int>.Fail("La categoría no puede ser nula.");

                    categoria.IdCategoria = null;
                    contexto.Categorias.Add(categoria);
                    contexto.SaveChanges();

                    if (categoria.IdCategoria != null)
                        return Resultado<int>.Ok((int)categoria.IdCategoria);
                    else
                        return Resultado<int>.Fail("No se pudo obtener el Id de la categoría registrada.");
                }
            }
            catch (Exception ex)
            {
                return Resultado<int>.Fail($"Error al registrar la categoría:\n{ex.ToString()}");
            }
        }
    }
}
