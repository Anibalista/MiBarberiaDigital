using System.Globalization;

namespace Front_SGBM
{
    public static class Validaciones
    {
        public static bool EsDigitoDecimal(char c) => char.IsDigit(c) || c == '.' || c == ',' || c == '\b';

        public static bool EsDigitoNumerico(char c) => char.IsNumber(c) || c == '\b';
        

        public static bool EsNumeroDecimal(string numero, ref string mensaje, bool obligatorio = true)
        {
            if (string.IsNullOrEmpty(numero))
            {
                mensaje = "Campo Vacío";
                return false;
            }
            int repetido = 0;
            foreach (char c in numero)
            {
                repetido += c.Equals(",") || c.Equals(".") ? 1 : 0;
            }
            if (repetido > 1)
            {
                mensaje = "Número incorrecto (revise los símbolos ',' y '.'";
                return false;
            }
            decimal validador = 0;
            if (!decimal.TryParse(numero, out validador))
            {
                mensaje = "El valor no es un número decimal";
                return false;
            }
            return true;
        }

        public static bool TextoCorrecto(string texto, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                mensaje = "Debe ingresar";
                return false;
            }
            if (texto.Length < 3)
            {
                mensaje = "Ingrese más de 2 caracteres";
                return false;
            }
            return true;
        }

        public static string CapitalizarTexto(string texto, bool soloInicio = false)
        {
            if (soloInicio)
            {
                return char.ToUpper(texto[0])+texto.Substring(1).ToLower();
            }
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(texto.ToLower());
        }

    }
}
