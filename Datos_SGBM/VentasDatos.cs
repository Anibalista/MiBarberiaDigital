using EF_SGBM;
using Entidades_SGBM;
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

        /// <summary>
        /// Procesa el guardado completo de una venta en base de datos.
        /// </summary>
        public static Resultado<bool> RegistrarVentaCompleta(Ventas venta, List<DetallesVentas> carrito, MediosPagos medioPago, List<Cajas> cajasAbiertas)
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    // ==========================================================
                    // BLOQUE 1: GUARDADO DE VENTA, DETALLES, FACTURA Y CAJA
                    // ==========================================================

                    // 1. Asignamos los detalles a la venta
                    venta.DetallesVentas = carrito;
                    //venta.Estados = contexto.Estados.FirstOrDefault(e => e.Estado == "Finalizada" && e.Indole == "Ventas");

                    // 2. Creamos la Factura
                    var nuevaFactura = new Facturas
                    {
                        Tipo = "C",
                        NroFactura = venta.NroVenta,
                        TotalAbonado = venta.Total,
                        IdMedioPago = medioPago.IdMedioPago,
                        Transacciones = new List<Transacciones>() // Inicializamos la lista vacía
                    };

                    venta.Facturas = new List<Facturas> { nuevaFactura };

                    // 3. Manejo Dinámico del Tipo de Transacción (Tu idea aplicada)
                    var tipoTransaccion = contexto.TiposTransacciones.FirstOrDefault(t => t.Tipo.ToLower() == "venta");
                    if (tipoTransaccion == null)
                    {
                        tipoTransaccion = new TiposTransacciones { Tipo = "Venta" };
                        // No hace falta hacer contexto.TiposTransacciones.Add() explícitamente,
                        // al asignarlo a la transacción abajo, EF lo detecta e inserta solo.
                    }

                    // 4. Distribución a las Cajas (Múltiples Transacciones)
                    decimal totalProductos = carrito.Where(c => c.IdProducto != null).Sum(c => c.SubTotal);
                    decimal totalServicios = carrito.Where(c => c.IdServicio != null).Sum(c => c.SubTotal);

                    // -- Transacción para la Caja de Productos --
                    if (totalProductos > 0)
                    {
                        var cajaProd = cajasAbiertas.First(c => c.TiposCajas!.Tipo.Contains("Productos"));

                        var transProd = new Transacciones
                        {
                            Hora = DateTime.Now,
                            MontoIngreso = totalProductos,
                            MontoEgreso = 0, // Buena práctica explicitarlo
                            IdCaja = cajaProd.IdCaja.Value,
                            TiposTransacciones = tipoTransaccion // Usamos el objeto directo, EF Core extrae el ID mágico
                        };

                        // Agregamos a la lista de la factura
                        nuevaFactura.Transacciones.Add(transProd);
                    }

                    // -- Transacción para la Caja de Servicios --
                    if (totalServicios > 0)
                    {
                        var cajaServ = cajasAbiertas.First(c => c.TiposCajas!.Tipo.Contains("Servicios"));

                        var transServ = new Transacciones
                        {
                            Hora = DateTime.Now,
                            MontoIngreso = totalServicios,
                            MontoEgreso = 0,
                            IdCaja = cajaServ.IdCaja.Value,
                            TiposTransacciones = tipoTransaccion // Reutilizamos el mismo tipo
                        };

                        // Agregamos también a la lista de la factura
                        nuevaFactura.Transacciones.Add(transServ);
                    }

                    // 5. EL TRUCO DE MAGIA: EF Core guarda absolutamente TODO junto
                    contexto.Ventas.Add(venta);
                    contexto.SaveChanges();
                }
                return Resultado<bool>.Ok(true, "Operación exitosa");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Fallo Crítico al registrar venta: {ex.ToString()}");
                return Resultado<bool>.Fail("No se pudo registrar la venta en la base de datos.");
            }
        }

    }
}
