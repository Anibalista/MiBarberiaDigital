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
        /// Obtiene una lista de categorías filtradas por índole.
        /// </summary>
        /// <param name="indole">Texto de índole para filtrar. Si está vacío o null, devuelve todas.</param>
        /// <returns>Resultado con la lista de categorías o mensaje de error.</returns>
        public static Resultado<List<Categorias>> ListaCategoriasPorIndole(string? indole)
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Categorias, nameof(contexto.Categorias));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Categorias>>.Fail(rc.Mensaje);
                    }

                    var indoleCrit = string.IsNullOrWhiteSpace(indole) ? null : indole.Trim().ToLower();

                    IQueryable<Categorias> query = contexto.Categorias.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(indoleCrit))
                    {
                        query = query.Where(c => c.Indole != null && c.Indole.ToLower() == indoleCrit);
                    }

                    var lista = query.OrderBy(c => c.Descripcion).ToList();

                    if (lista == null || lista.Count == 0)
                    {
                        var msg = string.IsNullOrWhiteSpace(indoleCrit)
                            ? "No se encontraron categorías."
                            : $"No se encontraron categorías con índole '{indole}'.";
                        return Resultado<List<Categorias>>.Ok(new List<Categorias>(), msg);
                    }
                    return Resultado<List<Categorias>>.Ok(lista);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener categorías:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Categorias>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra una nueva categoría en la base de datos.
        /// </summary>
        /// <param name="categoria">Objeto Categorias a registrar.</param>
        /// <returns>Resultado con el Id de la categoría registrada o mensaje de error.</returns>
        public static Resultado<int> RegistrarCategoria(Categorias? categoria)
        {
            if (categoria == null)
                return Resultado<int>.Fail("La categoría no puede ser nula.");

            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                return Resultado<int>.Fail("El campo Descripción es obligatorio.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Categorias, nameof(contexto.Categorias));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<int>.Fail(rc.Mensaje);
                    }

                    // Preparar entidad para inserción
                    categoria.IdCategoria = null;
                    categoria.Descripcion = categoria.Descripcion.Trim();
                    categoria.Indole = string.IsNullOrWhiteSpace(categoria.Indole) ? null : categoria.Indole.Trim();

                    contexto.Categorias.Add(categoria);
                    contexto.SaveChanges();

                    if (categoria.IdCategoria != null && categoria.IdCategoria > 0)
                        return Resultado<int>.Ok(categoria.IdCategoria.Value);

                    var msg = "No se pudo obtener el Id de la categoría registrada.";
                    Logger.LogError(msg);
                    return Resultado<int>.Fail(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar la categoría:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }
    }
}
