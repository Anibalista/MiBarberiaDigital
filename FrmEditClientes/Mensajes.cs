using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front_SGBM
{
    public class Mensajes
    {
        public static void mensajeError(string error, string caption = "Error")
        {
            MessageBox.Show(error, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void mensajeExito(string mensaje, string caption = "Éxito")
        {
            MessageBox.Show(mensaje, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void mensajeAdvertencia(string mensaje, string caption = "Advertencia")
        {
            MessageBox.Show(mensaje, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static DialogResult respuesta(string pregunta, string caption = "Confirmar", bool cancelar = false)
        {
            MessageBoxButtons botones = cancelar ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo;
            DialogResult resultado = MessageBox.Show(pregunta, caption, botones, MessageBoxIcon.Question);
            return resultado;
        }
    }
}
