using Entidades_SGBM;
using Datos_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    public class FacturasNegocio
    {
        /// <summary>
        /// Obtiene los medios de pago disponibles en el sistema.
        /// </summary>
        public static Resultado<List<MediosPagos>> GetMediosPago()
        {
            try
            {
                return FacturasDatos.GetMediosPago();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error en FacturasNegocio al obtener medios de pago: {ex.Message}");
                return Resultado<List<MediosPagos>>.Fail("Error al obtener medios de pago.");
            }
        }
    }
}
