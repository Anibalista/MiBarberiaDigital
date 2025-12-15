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

                // Usar HashSet para búsquedas rápidas O(1)
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
            } finally
            {
                refrescarGrilla();
            }
        }

        private void refrescarGrilla()
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            cerrarFormulario();
        }
    }
}
