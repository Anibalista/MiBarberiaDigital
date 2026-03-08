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
            public int Id { get; set; }
            public string Tipo { get; set; } // "Producto" o "Servicio"
            public string Descripcion { get; set; }
            public decimal PrecioLista { get; set; }
            public decimal PrecioEfectivo { get; set; }

            // Referencias originales ocultas por si las necesitamos al añadir al carrito
            public Productos? ProductoOrigen { get; set; }
            public Servicios? ServicioOrigen { get; set; }
        }

        // --- Variables de estado ---
        private List<ItemSeleccion> _catalogoItems = new List<ItemSeleccion>();
        private List<DetallesVentas> _carrito = new List<DetallesVentas>();
        private Clientes? _clienteActual = null;
        private Empleados? _vendedorActual = null;


        public FrmEditVentas()
        {
            InitializeComponent();
        }

        private void FrmEditVentas_Load(object sender, EventArgs e)
        {
            // 1. Estilos visuales
            EstiloAplicacion.AplicarEstilo(this);

            // 2. Cargar Barberos (Vendedores)
            if (!CargarVendedores())
            {
                Mensajes.MensajeError("No hay barberos/empleados registrados en el sistema. Debe registrar al menos uno para iniciar ventas.");
                this.Close();
                return;
            }

            // 3. Cargar Medios de Pago
            CargarMediosPago();

            // 4. Cargar la grilla izquierda (Catálogo unificado)
            CargarCatalogoSeleccion();

            // 5. Preparar la UI para empezar
            LimpiarVenta();
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
                cbVendedor.DataSource = bindingEmpleados;
                cbVendedor.DisplayMember = "NombreCompleto"; // Ajustar a tu entidad
                cbVendedor.ValueMember = "IdEmpleado";
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
                // Asumo que tu método en Negocio se encargará de crear "Efectivo" y "Transferencia MP" si la tabla está vacía
                var resultado = MediosPagosNegocio.GetMediosPagos();

                if (resultado.Success && resultado.Data != null)
                {
                    bindingMedios.DataSource = resultado.Data;
                    // Supongo que el ComboBox de medios se llama cbMediosPago (ajusta el nombre si es distinto)
                    // cbMediosPago.DataSource = bindingMedios;
                    // cbMediosPago.DisplayMember = "Medio";
                    // cbMediosPago.ValueMember = "IdMedioPago";

                    // Seleccionar "Efectivo" por defecto
                    // var indexEfectivo = resultado.Data.FindIndex(m => m.Medio.Contains("Efectivo"));
                    // if (indexEfectivo >= 0) cbMediosPago.SelectedIndex = indexEfectivo;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al cargar medios de pago en Ventas: {ex.Message}");
            }
        }
    }
}
