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

                    // "Limpiamos" los objetos anidados del carrito para que EF Core solo lea los IDs
                    // y no intente insertar Productos, Servicios o Categorías de nuevo.
                    foreach (var detalle in carrito)
                    {
                        detalle.Productos = null;
                        detalle.Servicios = null;
                    }

                    // 1. Asignamos los detalles a la venta
                    venta.DetallesVentas = carrito;

                    // Buscamos el estado y asignamos SOLO el ID, no el objeto completo.
                    var estadoFinalizada = contexto.Estados.FirstOrDefault(e => e.Estado == "Finalizada" && e.Indole == "Ventas");
                    if (estadoFinalizada == null)
                    {
                        return Resultado<bool>.Fail("Error interno: No se encontró el estado 'Finalizada' en la base de datos.");
                    }

                    venta.IdEstado = estadoFinalizada.IdEstado; // Usamos la propiedad int explícitamente
                    venta.Estados = null; // Cortamos el objeto para evitar que intente re-insertarlo

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
                    var tiposTransacciones = contexto.TiposTransacciones.Where(t => t.Tipo.ToLower().Contains("venta")).ToList();
                    TiposTransacciones? tipoTransProd = null;
                    TiposTransacciones? tipoTransServ = null;

                    if (tiposTransacciones != null)
                    {
                        tipoTransProd = tiposTransacciones.FirstOrDefault(t => t.Tipo.ToLower().Contains("productos"));
                        tipoTransServ = tiposTransacciones.FirstOrDefault(t => t.Tipo.ToLower().Contains("servicios"));
                    }

                    if (tipoTransProd == null || tipoTransServ == null)
                    {
                        Logger.LogError("No se encontraron tipos de transacción adecuados para 'Productos' o 'Servicios'.");
                        return Resultado<bool>.Fail("Configuración de tipos de transacción incompleta en la base de datos.");
                    }

                    var medioPagoBD = contexto.MediosPagos.Find(medioPago.IdMedioPago);

                    bool esEfectivo = medioPagoBD != null && medioPagoBD.Medio.Equals("efectivo", StringComparison.OrdinalIgnoreCase);

                    // 4. Distribución a las Cajas (Múltiples Transacciones)
                    decimal totalProductos = carrito.Where(c => c.IdProducto != null).Sum(c => c.SubTotal);
                    decimal totalServicios = carrito.Where(c => c.IdServicio != null).Sum(c => c.SubTotal);

                    var tiposCajas = contexto.TiposCajas.ToList();
                    int idTipoProductos = tiposCajas.FirstOrDefault(tc => tc.Tipo == "Productos")?.IdTipo ?? 0;
                    int idTipoServicios = tiposCajas.FirstOrDefault(tc => tc.Tipo == "Servicios")?.IdTipo ?? 0;

                    // -- Transacción para la Caja de Productos --
                    if (totalProductos > 0)
                    {
                        var cajaProd = cajasAbiertas.First(c => c.IdTipo == idTipoProductos || c.IdTipo == 1);

                        cajaProd.TotalMP += esEfectivo ? 0 : totalProductos;
                        cajaProd.TotalEfectivo += esEfectivo ? totalProductos : 0;

                        contexto.Cajas.Update(cajaProd); // Marcamos la caja como modificada para que EF Core la actualice

                        var transProd = new Transacciones
                        {
                            Hora = DateTime.Now,
                            MontoIngreso = totalProductos,
                            MontoEgreso = 0, // Buena práctica explicitarlo
                            IdCaja = cajaProd.IdCaja.Value,
                            IdTipoTransaccion = tipoTransProd.IdTipoTransaccion // Asignamos el ID directamente
                        };
                        
                        // Agregamos a la lista de la factura
                        nuevaFactura.Transacciones.Add(transProd);
                    }

                    // -- Transacción para la Caja de Servicios --
                    if (totalServicios > 0)
                    {
                        var cajaServ = cajasAbiertas.First(c => c.IdTipo == idTipoServicios || c.IdTipo == 2);

                        cajaServ.TotalMP += esEfectivo ? 0 : totalServicios;
                        cajaServ.TotalEfectivo += esEfectivo ? totalServicios : 0;

                        contexto.Cajas.Update(cajaServ); // Marcamos la caja como modificada para que EF Core la actualice

                        var transServ = new Transacciones
                        {
                            Hora = DateTime.Now,
                            MontoIngreso = totalServicios,
                            MontoEgreso = 0,
                            IdCaja = cajaServ.IdCaja.Value,
                            IdTipoTransaccion = tipoTransServ.IdTipoTransaccion
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

        /// <summary>
        /// Procesa el stock de los productos vendidos
        /// restando las cantidades vendidas a los productos correspondientes
        /// y los insumos de los servicios prestados.
        /// </summary>
        public static string ProcesarStockVenta(List<DetallesVentas>? detalles)
        {
            if (detalles == null || !detalles.Any())
            {
                return "No hay detalles de venta para procesar el stock.";
            }

            try
            {
                using (var contexto = new Contexto())
                {
                    // ==========================================================
                    // 1. DESCONTAR STOCK DE PRODUCTOS VENDIDOS DIRECTAMENTE
                    // ==========================================================
                    var detallesProductos = detalles.Where(d => d.IdProducto != null).ToList();

                    foreach (var det in detallesProductos)
                    {
                        // Buscamos el producto en la BD
                        var productoBD = contexto.Productos.Find(det.IdProducto);
                        if (productoBD != null)
                        {
                            // Restamos la cantidad que se vendió en este detalle
                            productoBD.Stock -= det.Cantidad;
                        }
                    }

                    // ==========================================================
                    // 2. DESCONTAR STOCK DE INSUMOS USADOS EN SERVICIOS
                    // ==========================================================
                    var detallesServicios = detalles.Where(d => d.IdServicio != null).ToList();

                    if (detallesServicios.Any())
                    {
                        // Sacamos todos los IDs de los servicios vendidos
                        var idsServicios = detallesServicios.Select(d => d.IdServicio).Distinct().ToList();

                        // Traemos de la BD todos los insumos (CostosServicios) asociados a esos servicios
                        // Asumo que tu clase CostosServicios enlaza IdServicio con IdProducto y tiene una "Cantidad"
                        var insumos = contexto.CostosServicios
                                              .Where(cs => idsServicios.Contains(cs.IdServicio) && cs.IdProducto != null)
                                              .ToList();

                        foreach (var detServ in detallesServicios)
                        {
                            // Filtramos los insumos específicos de ESTE servicio
                            var insumosDelServicio = insumos.Where(cs => cs.IdServicio == detServ.IdServicio);

                            foreach (var insumo in insumosDelServicio)
                            {
                                var productoInsumo = contexto.Productos.Find(insumo.IdProducto);
                                if (productoInsumo != null)
                                {
                                    int cantidadADescontar = 0;
                                    decimal medidaADescontar = 0;
                                    if (insumo.CantidadMedida != null && insumo.CantidadMedida > 0 && productoInsumo.Medida != null)
                                    {
                                        int medidaProducto = (int)(productoInsumo.Medida == 0 ? 1 : productoInsumo.Medida);

                                        // Si el insumo se mide en cantidad (ej: litros, kg), multiplicamos por la cantidad vendida del servicio
                                        medidaADescontar = (decimal)insumo.CantidadMedida * detServ.Cantidad;
                                        decimal cantidadTotalMedida = (productoInsumo.CantidadMedida ?? 0) + medidaProducto * productoInsumo.Stock;

                                        cantidadTotalMedida -= medidaADescontar;

                                        productoInsumo.Stock = (int)(cantidadTotalMedida / medidaProducto);

                                        productoInsumo.CantidadMedida = cantidadTotalMedida % medidaProducto;

                                    }
                                    if (insumo.Unidades != null && insumo.Unidades > 0)
                                    {
                                        // Si el insumo se mide en unidades, multiplicamos por la cantidad vendida del servicio
                                        cantidadADescontar += (int)insumo.Unidades * detServ.Cantidad;
                                    }
                                    
                                    productoInsumo.Stock -= cantidadADescontar;
                                }
                            }
                        }
                    }

                    // ==========================================================
                    // 3. GUARDAR TODOS LOS CAMBIOS EN LA BASE DE DATOS
                    // ==========================================================
                    // EF Core es inteligente: arma un solo bloque UPDATE para todos los productos modificados
                    contexto.SaveChanges();

                    return "Stock procesado correctamente.";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al procesar stock de venta: {ex.ToString()}");
                return "Ocurrió un error interno al intentar descontar el stock.";
            }
        }

    }
}
