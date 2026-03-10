using Entidades_SGBM;
using Front_SGBM.UXDesign;
using Negocio_SGBM;
using Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Front_SGBM
{
    public partial class FrmEditVentas : Form
    {
        // --- Clase interna para unificar Productos y Servicios en la grilla izquierda ---
        private class ItemSeleccion
        {
            public int? Id { get; set; }
            public string Tipo { get; set; } // "Producto" o "Servicio"
            public string Descripcion { get; set; }
            public decimal Precio { get; set; }
            public decimal Efectivo { get; set; }

            // Referencias originales ocultas por si las necesitamos al añadir al carrito
            public Productos? ProductoOrigen { get; set; }
            public Servicios? ServicioOrigen { get; set; }
        }

        // --- Variables de estado ---
        private List<ItemSeleccion> _catalogoItems = new List<ItemSeleccion>();
        private List<DetallesVentas> _carrito = new List<DetallesVentas>();
        private Clientes? _clienteActual = null;
        private Empleados? _vendedorActual = null;
        private bool cerrando = false; // Para controlar el cierre del formulario

        public FrmEditVentas()
        {
            InitializeComponent();
        }

        private void FrmEditVentas_Load(object sender, EventArgs e)
        {
            // 1. Cargar Barberos (Vendedores)
            if (!CargarVendedores())
            {
                Mensajes.MensajeError("No hay barberos/empleados registrados en el sistema. Debe registrar al menos uno para iniciar ventas.");
                this.Close();
                return;
            }

            // 2. Cargar Medios de Pago
            CargarMediosPago();

            // 3. Cargar la grilla izquierda (Catálogo unificado)
            CargarCatalogoSeleccion();

            // 4. Mostrar el catálogo en la grilla
            ActualizarGrillaSeleccion(_catalogoItems);

            // 5. Preparar la UI para empezar
            //LimpiarVenta();
        }

        private void CargarCatalogoSeleccion()
        {
            _catalogoItems = new List<ItemSeleccion>();
            var resServicios = ServiciosNegocio.ListarActivos();
            if (resServicios.Success && resServicios.Data != null)
            {
                _catalogoItems.AddRange(resServicios.Data.Select(s => new ItemSeleccion
                {
                    Id = s.IdServicio,
                    Tipo = "Servicio",
                    Descripcion = s.NombreServicio,
                    Precio = s.PrecioLista,
                    Efectivo = s.PrecioContado ?? s.PrecioLista,
                    ServicioOrigen = s
                }));
            }

            var resProductos = ProductosNegocio.GetProductosActivos(checkStock.Checked);
            if (resProductos.Success && resProductos.Data != null)
            {
                _catalogoItems.AddRange(resProductos.Data.Select(p => new ItemSeleccion
                {
                    Id = p.IdProducto,
                    Tipo = "Producto",
                    Descripcion = p.Descripcion,
                    Precio = p.PrecioVenta,
                    Efectivo = p.PrecioVenta,
                    ProductoOrigen = p
                }));
            }
        }

        /// <summary>
        /// Actualiza la grilla de selección (izquierda) con los productos y servicios activos.
        /// </summary>
        private void ActualizarGrillaSeleccion(List<ItemSeleccion> catalogoFiltrado)
        {
            try
            {
                dataGridSeleccion.DataSource = null;
                dataGridSeleccion.DataSource = catalogoFiltrado;
                OcultarColumnasSeleccion();
                FormatearColumnasSeleccion();
                dataGridSeleccion.Refresh();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al actualizar grilla de selección: {ex.ToString()}");
            }

        }

        /// <summary>
        /// Ocultar columnas innecesarias en la grilla de selección para el usuario final.
        /// </summary>
        private void OcultarColumnasSeleccion()
        {
            try
            {
                dataGridSeleccion.Columns["Id"].Visible = false;
                dataGridSeleccion.Columns["Tipo"].Visible = false;
                dataGridSeleccion.Columns["ProductoOrigen"].Visible = false;
                dataGridSeleccion.Columns["ServicioOrigen"].Visible = false;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al ocultar columnas en grilla de selección: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Aplica formato de moneda a las columnas de precio en la grilla de selección.
        /// </summary>
        private void FormatearColumnasSeleccion()
        {
            try
            {
                EstiloAplicacion.ApplyFormats(dataGridSeleccion, new Dictionary<string, string>
                {
                    { "Precio", "C2" },
                    { "Efectivo", "C2" }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al formatear columnas de precio en grilla de selección: {ex.ToString()}");
            }
        }



        /// <summary>
        /// Carga los empleados. Retorna false si la lista viene vacía o hay error.
        /// </summary>
        private bool CargarVendedores()
        {
            try
            {
                var resultado = EmpleadosNegocio.GetEmpleados(false); // Sin anulados
                if (!resultado.Success || resultado.Data == null || !resultado.Data.Any())
                    return false;

                bindingEmpleados.DataSource = resultado.Data;
                cbVendedor.SelectedIndex = -1; // Obligamos a que lo elijan

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al cargar vendedores en Ventas: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Carga medios de pago. La regla de negocio de crearlos si no existen está en la capa Negocio.
        /// </summary>
        private void CargarMediosPago()
        {
            try
            {
                // El método en Negocio se encargará de crear "Efectivo" y "Transferencia MP" si la tabla está vacía
                var resultado = FacturasNegocio.GetMediosPago();

                if (resultado.Success && resultado.Data != null)
                {
                    bindingMedios.DataSource = resultado.Data;

                    // Seleccionar "Efectivo" por defecto
                    var indexEfectivo = resultado.Data.FindIndex(m => m.Medio.Contains("Efectivo"));
                    if (indexEfectivo >= 0) cbMedioPago.SelectedIndex = indexEfectivo;
                }
                else
                {
                    Mensajes.MensajeError("No se pudieron cargar los medios de pago. Verifique la configuración del sistema.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al cargar medios de pago en Ventas: {ex.ToString()}");
            }
        }

        private void TxtDni_KeyPress(object sender, KeyPressEventArgs e) => e.Handled = !Validaciones.EsDigitoNumerico(e.KeyChar);

        private void TxtTotalAbonado_KeyPress(object sender, KeyPressEventArgs e) => e.Handled = !Validaciones.EsDigitoDecimal(e.KeyChar);

        private void Filtro_TextChanged(object sender, EventArgs e)
        {
            if (cerrando) return; // Evitamos que el filtro intente acceder a controles mientras se cierra el formulario
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            cerrando = Mensajes.ConfirmarCierre();
            if (cerrando)
            {
                this.Close();
            }
        }
    }
}
