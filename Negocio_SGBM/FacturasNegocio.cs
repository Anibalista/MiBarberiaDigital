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
                        new MediosPagos { IdMedioPago = 1, Medio = "Efectivo", Observaciones = "Contado efectivo"},
                        new MediosPagos { IdMedioPago = 2, Medio = "Mercado Pago (Transferencia)", Observaciones = "Transferencia a la cuenta de mercado pago" },
                        new MediosPagos { IdMedioPago = 3, Medio = "Mercado Pago (QR)", Observaciones = "Pago con código Qr a mercado pago" },
                        new MediosPagos { IdMedioPago = 4, Medio = "Tarjeta de Débito", Observaciones = "Pago con Tarjeta de débito" },
                        new MediosPagos { IdMedioPago = 5, Medio = "Tarjeta de Crédito", Observaciones = "Pago con Tarjeta de crédito" }
                    };
                    return Resultado<List<MediosPagos>>.Ok(resultado.Data);
                }
                MediosPagos? efectivo = resultado.Data.FirstOrDefault(m => m.Medio.Equals("Efectivo", StringComparison.OrdinalIgnoreCase));
                if (efectivo == null)
                {
                    resultado.Data.Insert(0, new MediosPagos { IdMedioPago = 1, Medio = "Efectivo", Observaciones = "Contado Efectivo" });
                }
                MediosPagos? mercadoPagoTransferencia = resultado.Data.FirstOrDefault(m => m.Medio.Equals("Mercado Pago (Transferencia)", StringComparison.OrdinalIgnoreCase));
                if (mercadoPagoTransferencia == null)
                {
                    resultado.Data.Add(new MediosPagos { IdMedioPago = 2, Medio = "Mercado Pago (Transferencia)", Observaciones = "Transferencia Mercado Pago" });
                }
                MediosPagos? mercadoPagoQr = resultado.Data.FirstOrDefault(m => m.Medio.Equals("Mercado Pago (QR)", StringComparison.OrdinalIgnoreCase));
                if (mercadoPagoQr == null)
                {
                    resultado.Data.Add(new MediosPagos { IdMedioPago = 3, Medio = "Mercado Pago (QR)", Observaciones = "QR Mercado Pago" });
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
