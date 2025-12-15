using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front_SGBM
{
    public static class Validaciones
    {
        public static bool confirmarCierre()
        {
            DialogResult respuesta = MessageBox.Show("¿Seguro desea salir? Los cambios no guardados se perderán", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return respuesta == DialogResult.Yes;
        }
    }
}
