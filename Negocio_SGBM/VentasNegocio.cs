using Datos_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;

namespace Negocio_SGBM
{
    public class VentasNegocio
    {
        /// <summary>
        /// Genera el próximo número de venta del día con el formato YYMMDD-1XXXX
        /// Ejemplo: 260318-10001
        /// </summary>
        public static Resultado<string> GenerarProximoNroVenta()
        {
            try
            {
                // 1. Traemos el último número de la base de datos
                var resDatos = VentasDatos.GetUltimoNroVentaHoy();

                if (!resDatos.Success)
                    return Resultado<string>.Fail(resDatos.Mensaje);

                string? ultimoNro = resDatos.Data; // Ej: "260318-10001" o null si es la primera

                // 2. Preparamos las partes fijas
                string prefijoFecha = DateTime.Today.ToString("yyMMdd"); // Ej: "260318"
                string sucursal = "1"; // Valor por defecto
                int correlativo = 1; // Si es la primera venta, arranca en 1

                // 3. Si ya hubo ventas hoy, extraemos el acumulador y le sumamos 1
                if (!string.IsNullOrWhiteSpace(ultimoNro) && ultimoNro.Contains("-"))
                {
                    // Dividimos por el guión. parteDerecha = "10001"
                    string parteDerecha = ultimoNro.Split('-')[1];

                    // Quitamos el '1' de la sucursal (el primer caracter) para quedarnos con "0001"
                    if (parteDerecha.Length > 1)
                    {
                        string strAcumulador = parteDerecha.Substring(1);

                        if (int.TryParse(strAcumulador, out int ultimoAcumulador))
                        {
                            correlativo = ultimoAcumulador + 1;
                        }
                    }
                }

                // 4. Ensamblamos el nuevo número final (D4 asegura que se rellene con ceros: 1 -> 0001)
                string nuevoNroVenta = $"{prefijoFecha}-{sucursal}{correlativo:D4}";

                return Resultado<string>.Ok(nuevoNroVenta);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al generar próximo nro de venta: {ex.ToString()}");
                return Resultado<string>.Fail("Error al calcular el número de comprobante.");
            }
        }

    }
}
