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
                var resultado = FacturasDatos.GetMediosPago();
                if (resultado.Data == null)
                {
                    resultado.Data = new List<MediosPagos>
                    {
                        new MediosPagos { IdMedioPago = null, Medio = "Efectivo", Observaciones = "Contado Efectivo" },
                        new MediosPagos { IdMedioPago = null, Medio = "Mercado Pago (QR)", Observaciones = "QR Mercado Pago" },
                        new MediosPagos { IdMedioPago = null, Medio = "Mercado Pago (Transferencia)", Observaciones = "Transferencia Mercado Pago" },
                        new MediosPagos { IdMedioPago = null, Medio = "Tarjeta de Crédito", Observaciones = "Pago con tarjeta de crédito" }
                    };
                    return Resultado<List<MediosPagos>>.Ok(resultado.Data);
                }
                MediosPagos? efectivo = resultado.Data.FirstOrDefault(m => m.Medio.Equals("Efectivo", StringComparison.OrdinalIgnoreCase));
                if (efectivo == null)
                {
                    resultado.Data.Insert(0, new MediosPagos { IdMedioPago = null, Medio = "Efectivo", Observaciones = "Contado Efectivo" });
                }
                MediosPagos? mercadoPagoQr = resultado.Data.FirstOrDefault(m => m.Medio.Equals("Mercado Pago (QR)", StringComparison.OrdinalIgnoreCase));
                if (mercadoPagoQr == null)
                {
                    resultado.Data.Add(new MediosPagos { IdMedioPago = null, Medio = "Mercado Pago (QR)", Observaciones = "QR Mercado Pago" });
                }
                MediosPagos? mercadoPagoTransferencia = resultado.Data.FirstOrDefault(m => m.Medio.Equals("Mercado Pago (Transferencia)", StringComparison.OrdinalIgnoreCase));
                if (mercadoPagoTransferencia == null)
                {
                    resultado.Data.Add(new MediosPagos { IdMedioPago = null, Medio = "Mercado Pago (Transferencia)", Observaciones = "Transferencia Mercado Pago" });
                }
                return Resultado<List<MediosPagos>>.Ok(resultado.Data);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error en FacturasNegocio al obtener medios de pago: {ex.Message}");
                return Resultado<List<MediosPagos>>.Fail("Error al obtener medios de pago.");
            }
        }
    }
}
