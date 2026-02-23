using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Front_SGBM
{
    public class Mensajes
    {
        public static void MensajeError(string error, string caption = "Error")
        {
            MessageBox.Show(error, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void MensajeExito(string mensaje, string caption = "Éxito")
        {
            MessageBox.Show(mensaje, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void MensajeAdvertencia(string mensaje, string caption = "Advertencia")
        {
            MessageBox.Show(mensaje, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static DialogResult Respuesta(string pregunta, string caption = "Confirmar", bool cancelar = false)
        {
            MessageBoxButtons botones = cancelar ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo;
            DialogResult resultado = MessageBox.Show(pregunta, caption, botones, MessageBoxIcon.Question);
            return resultado;
        }

        public static bool ConfirmarCierre()
        {

            DialogResult confirmacion = Respuesta("¿Seguro desea salir? Los cambios no guardados se perderán");
            return confirmacion == DialogResult.Yes;
        }

    }
}
