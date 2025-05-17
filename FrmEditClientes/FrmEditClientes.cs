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
    public partial class FrmEditClientes : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;
        public Personas? _persona = null;
        public Clientes? _cliente = null;
        public List<Contactos>? _contactos = null;

        public FrmEditClientes()
        {
            InitializeComponent();
        }

        //Comprobaciones
        private bool modoContactos()
        {
            if (_contactos == null)
            {
                return false;
            }
            return _contactos.Count > 0;
        }

        //Métodos
        private void refrescarGrilla()
        {
            bindingContactos.Clear();
            bindingContactos.DataSource = _contactos;
            dataGridContactos.Refresh();
        }


        public bool traerContactos(List<Contactos>? contactos)
        {
            if (contactos == null)
            {
                return false;
            }
            if (contactos.Count < 1)
            {
                return false;
            }
            _contactos = new List<Contactos>();
            _contactos = contactos;
            refrescarGrilla();
            return true;

        }

        //Botones
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

        }

        private void linkContactos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EnumModoForm modoCont = EnumModoForm.Alta;
            if (modoContactos())
            {
                modoCont = EnumModoForm.Modificacion;
            }

            //Método por si acaso pero no está de más
            FrmMenuPrincipal? principal = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (principal == null)
            {
                return;
            }

            FrmContactos? abierto = Application.OpenForms.OfType<FrmContactos>().FirstOrDefault();
            if (abierto != null)
            {
                abierto.Close();
            }

            FrmContactos formulario = new FrmContactos();
            formulario.modo = modoCont;
            formulario._contactos = _contactos;
            formulario.MdiParent = principal;
            formulario.Show();
        }
        //Filtros
        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
