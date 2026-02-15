using EF_SGBM;
using System.Runtime.CompilerServices;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase auxiliar para comprobar que el contexto y sus entidades estén disponibles.
    /// </summary>
    public class ComprobacionContexto
    {
        // Contexto de base de datos inyectado en la clase.
        private readonly Contexto contexto;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="contexto">Instancia del contexto a validar.</param>
        public ComprobacionContexto(Contexto contexto)
        {
            // Se asigna el contexto recibido a la variable interna.
            this.contexto = contexto;
        }

        /// <summary>
        /// Comprueba si la entidad existe en el contexto y devuelve un resultado tipado.
        /// </summary>
        /// <typeparam name="T">Tipo genérico de la entidad a comprobar.</typeparam>
        /// <param name="entidad">Entidad a validar (ejemplo: Productos, UnidadesMedidas).</param>
        /// <param name="nombreEntidad">
        /// Nombre de la entidad, capturado automáticamente por CallerArgumentExpression.
        /// </param>
        /// <returns>
        /// Resultado con Success = true si la entidad existe, 
        /// o Success = false con un mensaje de error en caso contrario.
        /// </returns>
        public Resultado<T> ComprobarEntidad<T>(
            T entidad,
            [CallerArgumentExpression("entidad")] string nombreEntidad = "")
        {
            // Si el contexto es nulo, significa que no hay conexión con la base de datos.
            if (contexto == null)
                return Resultado<T>.Fail("No se conecta a la BD");

            // Si la entidad es nula, significa que no se pudo acceder a esa tabla/colección.
            if (entidad == null)
                return Resultado<T>.Fail($"No se conecta al registro de {nombreEntidad}");

            // Si pasa todas las validaciones, se devuelve éxito con la entidad.
            return Resultado<T>.Ok(entidad);
        }
    }
}
