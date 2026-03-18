using EF_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;

namespace Datos_SGBM
{
    public class VentasDatos
    {
        /// <summary>
        /// Obtiene el último número de venta registrado en el día actual.
        /// Si no hay ventas hoy, retorna null.
        /// </summary>
        public static Resultado<string?> GetUltimoNroVentaHoy()
        {
            // En lugar de usar .Date (que anula los índices),
            // buscamos entre las 00:00 de hoy y las 00:00 de mañana.
            DateTime inicioHoy = DateTime.Today;
            DateTime inicioManana = inicioHoy.AddDays(1);

            try
            {
                using (var contexto = new Contexto())
                {
                    var ultimoNro = contexto.Ventas
                        .Where(v => v.FechaVenta >= inicioHoy && v.FechaVenta < inicioManana)
                        .OrderByDescending(v => v.IdVenta) // Ordenamos del más nuevo al más viejo
                        .Select(v => v.NroVenta) // Solo traemos la columna del número
                        .FirstOrDefault(); // Tomamos el TOP 1 (Si no hay, devuelve null)

                    return Resultado<string?>.Ok(ultimoNro);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener el último nro de venta: {ex.ToString()}");
                return Resultado<string?>.Fail("Error técnico al consultar el correlativo de ventas.");
            }
        }


    }
}
