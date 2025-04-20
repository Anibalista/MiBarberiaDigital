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
        public List<Contactos>? _contacts = null;
        public List<Contactos>? _contactosEliminados = null;

        public FrmEditClientes()
        {
            InitializeComponent();
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
