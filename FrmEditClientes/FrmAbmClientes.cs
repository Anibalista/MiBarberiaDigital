using Entidades_SGBM;
using Negocio_SGBM;
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
    public partial class FrmAbmClientes : Form
    {
        EnumModoForm modo = EnumModoForm.Consulta;
        private List<Personas>? _personas = null;
        private List<Clientes>? _clientes = null;
        private List<Contactos>? _contactos = null;
        public Clientes? _cliente = null;
        private List<Localidades>? _localidades = null;
        private List<string> opcionesBuscar = new List<string> { "Dni, Nombres", "Domicilio", "WhatsApp, Teléfono" };
        private bool cerrando = false;

        //Campos
        string _campo1 = string.Empty;
        string _campo2 = string.Empty;
        Localidades? _localidadBuscada = null;

        public FrmAbmClientes()
        {
            InitializeComponent();
        }

        private void FrmAbmClientes_Load(object sender, EventArgs e)
        {
            cargarFormulario();
        }

        private void cargarFormulario()
        {
            cbBusqueda.Items.Clear();
            cbBusqueda.DataSource = opcionesBuscar;
            cbBusqueda.SelectedIndex = 0;
        }

        private void buscarLocalidades()
        {
            if (cerrando)
            {
                return;
            }
            Localidades localidadCero = new();
            localidadCero.Localidad = "Seleccionar";
            List<Localidades>? localidades = new List<Localidades>();
            localidades.Add(localidadCero);
            string error = string.Empty;
            _localidades = DomiciliosNegocio.getLocalidades(ref error);
            if (_localidades != null)
            {
                localidades.AddRange(_localidades);
            }
            bindingLocalidades.Clear();
            bindingLocalidades.DataSource = localidades;
            try
            {
                cbLocalidad.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + error);
            }
        }

        private void cambiarNombresCampos(string lbl1, string lbl2, bool localidad)
        {
            labelCampo1.Text = lbl1;
            labelCampo2.Text = lbl2;
            cbLocalidad.Visible = localidad;
            labelLocalidad.Visible = localidad;
        }
        //Comprobaciones
        private void llenarCampos()
        {
            _campo1 = txtCampo1.Text;
            _campo2 = txtCampo2.Text;
            if (!cbLocalidad.Visible)
            {
                _localidadBuscada = null;
                return;
            }
            try
            {
                _localidadBuscada = (Localidades)cbLocalidad.SelectedItem;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            if (_localidadBuscada.IdLocalidad == null)
            {
                _localidadBuscada = null;
            }
        }

        //Botones
        private void btnSalir_Click(object sender, EventArgs e)
        {
            cerrando = true;
            this.Close();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {

        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {

        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            ImportarClientes importados = new ImportarClientes();
            if (!importados.hayArchivo)
            {
                return;
            }
            if (!importados.importarArchivoClientes())
            {
                MessageBox.Show($"Error {importados.observaciones}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                MessageBox.Show(importados.observaciones, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void btnExportar_Click(object sender, EventArgs e)
        {

        }

        private void cbBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (cerrando)
            {
                return;
            }
            string opcion = cbBusqueda.Text;
            if (String.IsNullOrEmpty(opcion))
            {
                return;
            }
            if (opcion == "Dni, Nombres")
            {
                cambiarNombresCampos("Dni", "Nombres", false);
            }
            else if (opcion == "Domicilio")
            {
                cambiarNombresCampos("Calle", "Barrio", true);
            }
            else
            {
                cambiarNombresCampos("Whatsapp", "Teléfono", false);
            }
        }

        private void cbLocalidad_VisibleChanged(object sender, EventArgs e)
        {
            if (cerrando)
            {
                return;
            }
            if (!cbLocalidad.Visible)
            {
                buscarLocalidades();
                return;
            }
            _localidades = null;
        }
    }
}
