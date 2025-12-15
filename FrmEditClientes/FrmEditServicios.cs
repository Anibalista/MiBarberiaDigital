using Entidades_SGBM;
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
    public partial class FrmEditServicios : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;
        public Servicios? _servicio;
        private bool cerrando = false;
        private bool cargando = false;
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
            try
            {
                bindingSourceProductos.DataSource = null;
                _productos = null;
                //Buscar Productos
                ///
                ///
                bindingSourceProductos.DataSource = _productos;
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
                //Buscar Categorias
                ///
                ///
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

        }

        //Eventos de botones
        private void btnSalir_Click(object sender, EventArgs e)
        {
            cerrarFormulario();
        }

        private void btnAdminCostos_Click(object sender, EventArgs e)
        {

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
    }
}
