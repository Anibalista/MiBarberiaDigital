using EF_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Datos_SGBM
{
    public class FacturasDatos
    {
        ///<summary>
        /// Obtiene los medios de pago disponibles en el sistema.
        /// </summary>
        public static Resultado<List<MediosPagos>> GetMediosPago()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    // Comprobamos que el contexto y sus entidades estén disponibles.
                    var comprobacion = new ComprobacionContexto(contexto);
                    if (!comprobacion.ComprobarEntidad(contexto.MediosPagos, "MediosPagos").Success)
                        return Resultado<List<MediosPagos>>.Fail("No se pudieron cargar los medios de pago.");
                    var mediosPago = contexto.MediosPagos.OrderBy(m => m.Medio).ToList();
                    if (mediosPago == null || mediosPago.Count == 0)
                        return Resultado<List<MediosPagos>>.Fail("No se encontraron medios de pago.");
                    return Resultado<List<MediosPagos>>.Ok(mediosPago);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener medios de pago: {ex.Message}");
                return Resultado<List<MediosPagos>>.Fail("Error al obtener medios de pago.");
            }
        }


    }
}
