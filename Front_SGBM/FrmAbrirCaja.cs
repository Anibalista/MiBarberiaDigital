using Entidades_SGBM;
using Front_SGBM.UXDesign;
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
    public partial class FrmAbrirCaja : Form
    {
        List<TiposCajas>? _tiposCajas;
        List<Empleados>? _empleados;

        public FrmAbrirCaja()
        {
            InitializeComponent();
        }

        private void FrmAbrirCaja_Load(object sender, EventArgs e)
        {
            EstiloAplicacion.AplicarEstilo(this);

        }


    }
}
