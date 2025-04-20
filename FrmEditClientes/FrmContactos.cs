using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Front_SGBM
{
    public partial class FrmContactos : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;
        public List<Contactos>? _contactos = null;
        private List<Contactos>? _contactsEliminados = null;

        public FrmContactos()
        {
            InitializeComponent();
        }

        private void FrmContactos_Load(object sender, EventArgs e)
        {

        }

        //Botones
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        private void btnMenos_Click(object sender, EventArgs e)
        {

        }

        private void btnMas_Click(object sender, EventArgs e)
        {

        }

        //Validadores de Campos
        private void numeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            TextBox currenttb = (TextBox)sender;
            if (currenttb.Text.Length > 0)
            {
                Regex emailRegex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
                if (!emailRegex.IsMatch(currenttb.Text))
                {
                    MessageBox.Show("El E-mail ingresado no tiene el formato correcto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

    }
}
