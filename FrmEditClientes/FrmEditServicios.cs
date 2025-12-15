using Entidades_SGBM;
using Negocio_SGBM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Front_SGBM
{
    public partial class FrmEditServicios : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;
        public Servicios? _servicio;
        private bool cerrando = false;
        private bool cargando = false;
        private bool administrandoInsumos = false;
        private List<Categorias>? _categorias;
        private List<Productos>? _productos;
        private List<ServiciosInsumos>? _insumos;
        private List<ServiciosInsumos>? _insumosNuevos;

        public FrmEditServicios()
        {
            InitializeComponent();
        }

        private void cerrarFormulario()
        {
            cerrando = Validaciones.confirmarCierre();
            if (cerrando)
                Close();
        }

        private void FrmEditServicios_Load(object sender, EventArgs e)
        {
            cargarProductos();
            cargarCategorias();
            cargarInsumos();
        }

        private void cargarProductos()
        {
            if (cerrando)
                return;
            string mensaje = string.Empty;
            try
            {
                bindingSourceProductos.DataSource = null;
                _productos = null;
                _productos = ProductosNegocio.ListaSimple(ref mensaje);
                if (string.IsNullOrEmpty( mensaje ))
                    bindingSourceProductos.DataSource = _productos;
                else
                    MessageBox.Show("Error al traer productos de la BD", "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbProductos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al traer productos de la BD\n" + ex.Message, "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbProductos.Enabled = false;
            }
        }
        private void cargarCategorias()
        {
            if (cerrando)
                return;
            try
            {
                bindingSourceCategorias.DataSource = null;
                _categorias = null;
                string mensaje = string.Empty;
                _categorias = CategoriasNegocio.Listar(ref mensaje);
                if (!string.IsNullOrWhiteSpace(mensaje))
                    MessageBox.Show(mensaje, "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                bindingSourceCategorias.DataSource = _categorias;
                cbCategoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al traer categorías de la BD\n" + ex.Message, "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbCategoria.Enabled = false;
            }
        }

        private void ocultarColumnasGrilla(DataGridView dgv, string[] nombresColumnas)
        {
            try
            {
                if (nombresColumnas == null || nombresColumnas.Length == 0 || dgv.Columns == null)
                    return;

                // Uso HashSet para búsquedas rápidas O(1)
                var columnasOcultar = new HashSet<string>(
                    nombresColumnas.Select(c => c.ToLower())
                );

                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.Visible = !columnasOcultar.Contains(column.Name.ToLower());
                }

            }
            catch
            {
                throw;
            }
        }

        private void cargarInsumos()
        {
            if (cerrando)
                return;
            try
            {
                cargando = true;
                dataGridInsumos.DataSource = null;
                _insumos = null;
                _insumos = new();
                if (_servicio != null && _servicio.IdServicio != null)
                {
                    //Buscar en BD
                    ///
                    ///
                }

                if (_insumosNuevos != null)
                    _insumos.AddRange(_insumosNuevos);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al traer Costos e Insumos de la BD\n" + ex.Message, "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
                refrescarGrilla();
            }
        }

        private void refrescarGrilla()
        {
            if (cargando)
                return;
            try
            {
                cargando = true;
                List<ServiciosInsumos> lista = new List<ServiciosInsumos>();
                dataGridInsumos.DataSource = null;
                txtTotalCostos.Clear();
                if (_insumos != null)
                {
                    lista = _insumos.OrderBy(i => i.Descripcion).ToList();
                    decimal totalCostos = lista.Sum(i => i.Costo);
                    txtTotalCostos.Text = totalCostos.ToString("N2", CultureInfo.GetCultureInfo("es-AR"));
                }
                txtCostosServicio.Text = txtTotalCostos.Text;
                dataGridInsumos.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al refrescar la grilla de Costos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
            }
        }

        //Eventos de botones
        private void btnSalir_Click(object sender, EventArgs e)
        {
            cerrarFormulario();
        }

        private void btnAdminCostos_Click(object sender, EventArgs e)
        {
            activarCamposInsumos();
        }

        private void btnNuevoInsumo_Click(object sender, EventArgs e)
        {

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        private void cbProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;

        }

        private void dataGridInsumos_SelectionChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;

        }

        //Carga de selecciones
        private Productos? productoSeleccionado()
        {
            if (cbProductos.SelectedIndex < 0) return null;
            try
            {
                return (Productos)cbProductos.SelectedItem;
            }
            catch
            {
                throw;
            }
        }

        private ServiciosInsumos? insumoSeleccionado()
        {
            try
            {
                if (dataGridInsumos == null || dataGridInsumos.CurrentRow == null)
                    return null;
                return dataGridInsumos.CurrentRow.DataBoundItem as ServiciosInsumos;
            }
            catch
            {
                throw;
            }
        }

        private Categorias? categoriaSeleccionada(ref string mensaje)
        {
            try
            {
                if (!Validaciones.textoCorrecto(cbCategoria.Text.Trim(), ref mensaje))
                    return null;
                Categorias? seleccionada = _categorias.FirstOrDefault(c => c.Descripcion.ToLower()  == cbCategoria.Text.Trim().ToLower());
                if (seleccionada == null)
                {
                    seleccionada = new Categorias();
                    seleccionada.Descripcion = Validaciones.capitalizarTexto(cbCategoria.Text.Trim());
                }
                return seleccionada;
            }
            catch
            {
                throw;
            }
        }

        //Activación y Limpieza de campos
        private void activarCamposInsumos()
        {
            try
            {
                limpiarCamposInsumos();
                txtDescripcionInsumo.Enabled = !administrandoInsumos;
                txtMontoInsumo.Enabled = !administrandoInsumos;
                cbProductos.Enabled = !administrandoInsumos;
                txtCantidad.Enabled = !administrandoInsumos;
                txtUnidades.Enabled = !administrandoInsumos;
                btnEliminar.Enabled = !administrandoInsumos;
                btnLimpiar.Enabled = !administrandoInsumos;
                btnModificar.Enabled = !administrandoInsumos;
                btnNuevoInsumo.Enabled = !administrandoInsumos;
                cargarInsumos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar los campos de Insumos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void limpiarCamposInsumos()
        {
            cargando = true;
            try
            {
                txtDescripcionInsumo.Clear();
                txtMontoInsumo.Clear();
                txtCantidad.Clear();
                txtUnidades.Clear();
                cbProductos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar los campos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
            }
        }

        private void limpiarCampos()
        {
            cargando = true;
            try
            {
                _servicio = null;
                _insumos = null;
                _insumosNuevos = null;
                txtServicio.Clear();
                txtDescripcionServicio.Clear();
                txtDuracion.Clear();
                txtPrecio.Clear();
                txtPuntaje.Clear();
                txtComision.Clear();
                txtCostosServicio.Clear();
                txtMargen.Clear();
                txtTotalCostos.Clear();
                checkComision.Checked = true;
                administrandoInsumos = true;
                activarCamposInsumos();
                cargando = false;
                refrescarGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar los campos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
            }
        }
    }
}
