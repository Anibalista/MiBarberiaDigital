using EF_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase que contiene métodos relacionados con la obtención y validación de cajas.
    /// </summary>
    public class CajasDatos
    {
        /// <summary>
        /// Comprueba que las entidades necesarias existan en el contexto.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <returns>
        /// Resultado con Success = true si todas las entidades están disponibles,
        /// o Success = false con un mensaje de error en caso contrario.
        /// </returns>
        public static Resultado<bool> ComprobarContexto(Contexto contexto)
        {
            var comprobacion = new ComprobacionContexto(contexto);

            // Validar TiposCajas
            var resultadoTipos = comprobacion.ComprobarEntidad(contexto.TiposCajas);
            if (!resultadoTipos.Success)
                return Resultado<bool>.Fail(resultadoTipos.Mensaje);

            // Validar Cajas
            var resultadoCajas = comprobacion.ComprobarEntidad(contexto.Cajas);
            if (!resultadoCajas.Success)
                return Resultado<bool>.Fail(resultadoCajas.Mensaje);

            // Si ambas validaciones pasaron, devolvemos éxito
            return Resultado<bool>.Ok(true);
        }

        /// <summary>
        /// Obtiene la lista de cajas filtradas por fecha.
        /// </summary>
        /// <param name="fecha">Fecha de las cajas a consultar. Si es nula, se devuelve un error.</param>
        /// <param name="incluirCerradas">
        /// Indica si se deben incluir cajas cerradas en la consulta.
        /// Si es false, solo se devuelven cajas abiertas.
        /// </param>
        /// <returns>
        /// Resultado con Success = true y la lista de cajas encontradas,
        /// o Success = false con un mensaje de error en caso de fallo.
        /// </returns>
        public static Resultado<List<Cajas>> GetCajasPorFecha(DateTime? fecha, bool incluirCerradas = false)
        {
            // Validación inicial: la fecha es obligatoria.
            if (fecha == null)
                return Resultado<List<Cajas>>.Fail("La fecha de las cajas no llega a la consulta");

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    // Comprobamos que el contexto y sus entidades estén disponibles.
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<List<Cajas>>.Fail(resultadoContexto.Mensaje);

                    // Construimos la consulta:
                    // - Filtramos por rango de fecha (>= fecha y < fecha + 1 día).
                    //   Esto evita problemas con la propiedad Date y aprovecha índices en SQL.
                    // - Filtramos por estado de apertura: si incluirCerradas es true,
                    //   no se aplica restricción; si es false, solo se devuelven abiertas.
                    var query = contexto.Cajas.Where(c =>
                        c.Fecha >= fecha.Value.Date &&
                        c.Fecha < fecha.Value.Date.AddDays(1) &&
                        (incluirCerradas || c.Abierta));

                    // Devolvemos éxito con la lista resultante.
                    return Resultado<List<Cajas>>.Ok(query.ToList());
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción inesperada.
                // Se loguea el detalle completo con ex.ToString() para diagnóstico.
                Logger.LogError(ex.ToString());

                // Se devuelve un resultado de error genérico para la capa de negocio.
                return Resultado<List<Cajas>>.Fail("Error en la consulta de cajas en la base de datos");
            }
        }
    }
}
