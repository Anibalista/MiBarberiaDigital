using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de Categorías.
    /// 
    /// Responsabilidades:
    /// - Validar datos de entrada antes de invocar la capa de datos.
    /// - Centralizar reglas mínimas de negocio (ej. descripción obligatoria).
    /// - Delegar operaciones CRUD a <see cref="CategoriasDatos"/>.
    /// - Devolver resultados uniformes mediante <see cref="Resultado{T}"/>.
    /// 
    /// Buenas prácticas:
    /// - No usar parámetros por referencia para mensajes; todos los mensajes de error o éxito
    ///   se devuelven dentro de <see cref="Resultado{T}"/>.
    /// - Mantener validaciones simples en esta capa; reglas más complejas pueden residir en una
    ///   capa superior de servicios si el proyecto escala.
    /// - Registrar mensajes técnicos en <c>Logger</c> en la capa de datos; aquí solo se devuelven
    ///   mensajes amigables.
    /// </summary>
    public class CategoriasNegocio
    {
        /// <summary>
        /// Valida una categoría antes de registrar o modificar.
        /// </summary>
        /// <param name="categoria">Entidad <see cref="Categorias"/> a validar.</param>
        /// <param name="registro">True si es validación para registro, False si es para modificación.</param>
        /// <returns><see cref="Resultado{T}"/> con true si la validación es correcta, o mensaje de error en caso contrario.</returns>
        private static Resultado<bool> ComprobarCategoria(Categorias? categoria, bool registro)
        {
            if (categoria == null)
                return Resultado<bool>.Fail("La información de la categoría no llega a la consulta.");

            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                return Resultado<bool>.Fail("La descripción de la categoría llegó vacía a la consulta.");

            if (!registro && (categoria.IdCategoria == null || categoria.IdCategoria < 1))
                return Resultado<bool>.Fail("Error con el Id de la Categoría a manipular.");

            return Resultado<bool>.Ok(true);
        }

        /// <summary>
        /// Lista las categorías filtradas por índole.
        /// </summary>
        /// <param name="indole">Índole de la categoría.</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con la lista de <see cref="Categorias"/> o mensaje de error en caso contrario.
        /// </returns>
        public static Resultado<List<Categorias>> ListarPorIndole(string indole)
        {
            if (string.IsNullOrWhiteSpace(indole))
                return Resultado<List<Categorias>>.Fail("No llega el índole de la categoría a la consulta.");

            try
            {
                return CategoriasDatos.ListaCategoriasPorIndole(indole);
            }
            catch (Exception ex)
            {
                var msg = $"Error en la búsqueda de Categorías:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Categorias>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra una nueva categoría en la base de datos.
        /// </summary>
        /// <param name="categoria">Entidad <see cref="Categorias"/> a registrar.</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con el Id generado de la categoría en caso de éxito,
        /// o con el mensaje de error en caso contrario.
        /// </returns>
        public static Resultado<int> Registrar(Categorias? categoria)
        {
            var validacion = ComprobarCategoria(categoria, true);
            if (!validacion.Success)
                return Resultado<int>.Fail(validacion.Mensaje);

            try
            {
                return CategoriasDatos.RegistrarCategoria(categoria);
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
