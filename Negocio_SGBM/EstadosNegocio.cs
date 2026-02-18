using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de Estados.
    /// 
    /// Responsabilidades:
    /// - Validar la información de estados antes de operar en la capa de datos.
    /// - Delegar operaciones CRUD a <see cref="EstadosDatos"/>.
    /// - Devolver resultados uniformes mediante <see cref="Resultado{T}"/>.
    /// 
    /// Nota importante:
    /// - La entidad Estados no es autoincremental, por lo que el IdEstado debe
    ///   asignarse y controlarse explícitamente en la capa de datos.
    /// </summary>
    public class EstadosNegocio
    {
        #region Consultas

        /// <summary>
        /// Obtiene un estado según índole y descripción.
        /// </summary>
        public static Resultado<Estados?> GetEstado(string? indole, string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(indole) || string.IsNullOrWhiteSpace(descripcion))
                return Resultado<Estados?>.Fail("Problemas con la información de estados en la capa negocio.");

            try
            {
                return EstadosDatos.GetEstado(indole, descripcion);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener estado:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Estados?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene todos los estados de una índole específica.
        /// </summary>
        public static Resultado<List<Estados>> GetEstadosPorIndole(string? indole)
        {
            if (string.IsNullOrWhiteSpace(indole))
                return Resultado<List<Estados>>.Fail("Problemas con la información de estados en la capa negocio.");

            try
            {
                return EstadosDatos.GetEstadosPorIndole(indole);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener estados por índole:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Estados>>.Fail(msg);
            }
        }

        #endregion

        #region Registros

        /// <summary>
        /// Registra un nuevo estado en la base de datos.
        /// </summary>
        /// <remarks>
        /// - El IdEstado no es autoincremental, por lo que debe ser asignado explícitamente.
        /// </remarks>
        public static Resultado<int> RegistrarEstado(Estados? estado)
        {
            if (estado == null)
                return Resultado<int>.Fail("Problemas al enviar información de estado a la capa negocio.");

            try
            {
                return EstadosDatos.RegistrarEstado(estado);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al registrar estado:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        #endregion
    }
}
