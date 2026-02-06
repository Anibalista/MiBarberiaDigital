
namespace Front_SGBM
{
    public class ArchivosOfficce
    {

        public static string? SeleccionarArchivoCSV()
        {
            try
            {

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    openFileDialog.Filter = "Archivos CSV (*.csv)|*.csv";
                    openFileDialog.Title = "Seleccionar un archivo CSV";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        return openFileDialog.FileName;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        public static string? SeleccionarArchivoXLSX()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    openFileDialog.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
                    openFileDialog.Title = "Seleccionar un archivo Excel";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        return openFileDialog.FileName;
                    }
                }
            } catch (Exception)
            {
                return null;
            }
            
            return null;
        }
    }
}
