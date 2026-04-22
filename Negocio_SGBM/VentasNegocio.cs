using Datos_SGBM;
using Entidades_SGBM;
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

        private static Resultado<Ventas> ValidarVenta(Ventas? venta)
        {
            if (venta == null) return Resultado<Ventas>.Fail("La venta no puede ser nula.");
            if (venta.Total < 0) return Resultado<Ventas>.Fail("El total de la venta no puede ser negativo.");
            if (venta.IdCliente < 0)
            {
                var resClienteGenerico = ClientesDatos.GetClienteGenerico();
                if (!resClienteGenerico.Success || resClienteGenerico.Data?.IdCliente == null)
                {
                    return Resultado<Ventas>.Fail("Error interno: No se pudo obtener el cliente genérico para ventas sin cliente.");
                }
                venta.IdCliente = resClienteGenerico.Data.IdCliente ?? 0;
                if (venta.IdCliente == 0)
                {
                    return Resultado<Ventas>.Fail("Error interno: El cliente genérico no tiene un Id válido.");
                }
            }
            if (venta.IdCliente > 0)
                venta.Clientes = null; // Para evitar que se intente insertar un nuevo cliente si el IdCliente es válido           
            if (venta.Clientes?.Personas != null && venta.Clientes.IdPersona > 0)
                venta.Clientes.Personas = null; // Para evitar que se intente insertar una nueva persona si el cliente ya tiene un IdCliente válido
            if (venta.IdEstado > 0)
                venta.Estados = null; // Para evitar que se intente insertar un nuevo estado si el IdEstado es válido
            if (venta.IdEmpleado > 0)
                venta.Empleados = null; // Para evitar que se intente insertar un nuevo empleado si el IdEmpleado es válido

            if (string.IsNullOrWhiteSpace(venta.NroVenta)) return Resultado<Ventas>.Fail("El número de venta es obligatorio.");
            
            return Resultado<Ventas>.Ok(venta);
        }

        public static Resultado<bool> ProcesarCobroVenta(Ventas venta, List<DetallesVentas> carrito, MediosPagos medioPago)
        {
            try
            {
                // 1. Validaciones previas de negocio (opcionales)
                var resValidacion = ValidarVenta(venta);
                if (!resValidacion.Success)
                {
                    return Resultado<bool>.Fail(resValidacion.Mensaje);
                }

                // 2. Buscar las entidades de las Cajas abiertas de HOY para pasarlas a Datos
                // Asumiendo que tienes un método en CajasDatos que te trae las cajas abiertas:
                var resCajasAbiertas = CajasDatos.GetCajasPorFecha(DateTime.Today);

                if (!resCajasAbiertas.Success || resCajasAbiertas.Data == null || !resCajasAbiertas.Data.Any())
                {
                    return Resultado<bool>.Fail("Error interno: No se detectaron cajas abiertas en el sistema.");
                }

                List<Cajas> cajasAbiertas = resCajasAbiertas.Data;

                // 3. Enviamos todo el "paquete" a la capa de Datos para que ejecute el guardado Graph (Transacción)
                var resVenta = VentasDatos.RegistrarVentaCompleta(venta, carrito, medioPago, cajasAbiertas);

                // 3.1 Si la venta no se pudo registrar, devolvemos el error
                if (!resVenta.Success)
                {
                    return Resultado<bool>.Fail(resVenta.Mensaje);
                }
                
                string mensajeStock = VentasDatos.ProcesarStockVenta(carrito);

                return Resultado<bool>.Ok(true, mensajeStock);

            }
            catch (Exception ex)
            {
                Logger.LogError($"Error en ProcesarCobroVenta Negocio: {ex.ToString()}");
                return Resultado<bool>.Fail("Ocurrió un error en las reglas de negocio al intentar cobrar.");
            }
        }

        
    }
}
