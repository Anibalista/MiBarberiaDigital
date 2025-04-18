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

        public FrmEditClientes()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
