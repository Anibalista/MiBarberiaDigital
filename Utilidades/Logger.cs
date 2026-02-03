using System.Runtime.CompilerServices;
using System.Text;

namespace Utilidades
{
    /// <summary>
    /// Clase estática para registrar errores en archivos de texto.
    /// </summary>
    /// <remarks>
    /// - Crea automáticamente la carpeta C:\Mi_Barberia_Digital\Logs si no existe.
    /// - Genera un archivo diario con nombre yy_MM_dd.txt.
    /// - Si el archivo está bloqueado, crea uno nuevo con sufijo incremental (-2, -3, etc.).
    /// - Escribe los errores con formato legible para revisión posterior.
    /// </remarks>
    public static class Logger
    {
        private static readonly string basePath = @"C:\Mi_Barberia_Digital\Logs";

        /// <summary>
        /// Registra un evento de interés (cocmo errores o login) en el archivo de logs.
        /// </summary>
        /// <param name="origen">Nombre del formulario o clase donde ocurrió el evento.</param>
        /// <param name="metodo">Nombre del método que llamó.</param>
        /// <param name="mensaje">Mensaje específico.</param>
        public static void Log(string origen, string metodo, string mensaje)
        {
            try
            {
                // Asegura que existan las carpetas
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                // Nombre base del archivo
                string fecha = DateTime.Now.ToString("yy_MM_dd");
                string fileName = fecha + ".txt";
                string filePath = Path.Combine(basePath, fileName);

                // Obtiene un archivo disponible (manejo de concurrencia)
                filePath = GetAvailableFile(filePath);

                // Escribe el log
                using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine("----------------------------------------");
                    sw.WriteLine($"Fecha: {DateTime.Now:dd/MM/yyyy} | Hora: {DateTime.Now:HH:mm:ss}");
                    sw.WriteLine($"Origen: {origen}");
                    sw.WriteLine($"Método fallido: {metodo}");
                    sw.WriteLine($"Error: {mensaje}");
                    sw.WriteLine("----------------------------------------");
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                // En caso extremo, se puede loguear en EventViewer o ignorar
                Console.WriteLine("Error al escribir log: " + ex.Message);
            }
        }

        /// <summary>
        /// Registra un evento de interés en el archivo de logs sin necesidad de especificar
        /// manualmente el origen y el método. Estos se capturan automáticamente mediante
        /// atributos de compilador.
        /// </summary>
        /// <param name="error">
        /// Mensaje específico del evento (puede ser un error, advertencia o información relevante).
        /// </param>
        /// <param name="origen">
        /// Nombre del archivo de código que invoca el log. Se obtiene automáticamente con CallerFilePath.
        /// </param>
        /// <param name="metodo">
        /// Nombre del método que invoca el log. Se obtiene automáticamente con CallerMemberName.
        /// </param>
        public static void LogError(string error,
            [CallerFilePath] string origen = "",
            [CallerMemberName] string metodo = "")
        {
            // Extraemos solo el nombre del archivo/clase del path completo
            string clase = Path.GetFileNameWithoutExtension(origen);
            Log(clase, metodo, error);
        }


        /// <summary>
        /// Obtiene un archivo disponible para escritura, creando sufijos si el original está bloqueado.
        /// </summary>
        /// <param name="filePath">Ruta base del archivo.</param>
        /// <returns>Ruta de un archivo disponible para escritura.</returns>
        private static string GetAvailableFile(string filePath)
        {
            int contador = 2;
            string directorio = Path.GetDirectoryName(filePath);
            string nombreBase = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            while (true)
            {
                try
                {
                    // Intentar abrir el archivo en modo Append exclusivo
                    using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        return filePath; // Si abre, está disponible
                    }
                }
                catch (IOException)
                {
                    // Archivo bloqueado, iterar con sufijo
                    string nuevoNombre = $"{nombreBase}-{contador}{extension}";
                    filePath = Path.Combine(directorio, nuevoNombre);
                    contador++;
                }
            }
        }
    }
}
