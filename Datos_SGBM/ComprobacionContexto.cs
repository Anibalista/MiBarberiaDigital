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
        private readonly Contexto contexto;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="contexto">Instancia del contexto a validar.</param>
        public ComprobacionContexto(Contexto contexto)
        {
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
            if (contexto == null)
                return Resultado<T>.Fail("No se conecta a la BD");

            if (entidad == null)
                return Resultado<T>.Fail($"No se conecta al registro de {nombreEntidad}");

            return Resultado<T>.Ok(entidad);
        }

        /// <summary>
        /// Comprueba varias entidades a la vez. Recibe pares (entidad, nombre) y devuelve el primer fallo o Ok(true).
        /// Uso recomendado: new ComprobacionContexto(contexto).ComprobarEntidades((contexto.Domicilios, nameof(contexto.Domicilios)), (contexto.Localidades, nameof(contexto.Localidades)));
        /// </summary>
        /// <param name="entidades">Lista de tuplas (entidad, nombreEntidad).</param>
        /// <returns>Resultado con Ok(true) si todas las entidades existen, o Fail con el mensaje del primer error.</returns>
        public Resultado<bool> ComprobarEntidades(params (object entidad, string nombre)[] entidades)
        {
            if (contexto == null)
                return Resultado<bool>.Fail("No se conecta a la BD");

            if (entidades == null || entidades.Length == 0)
                return Resultado<bool>.Fail("No se especificaron entidades para comprobar.");

            foreach (var (entidad, nombre) in entidades)
            {
                if (entidad == null)
                    return Resultado<bool>.Fail($"No se conecta al registro de {nombre}");
            }

            return Resultado<bool>.Ok(true);
        }
    }
}
