using Entidades_SGBM;
using Negocio_SGBM;
using OfficeOpenXml;
using Utilidades;

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

        public static string? GuardarArchivoXLSX(string nombrePorDefecto = "Clientes_Exportados")
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    saveFileDialog.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
                    saveFileDialog.Title = "Guardar archivo Excel";
                    saveFileDialog.FileName = $"{nombrePorDefecto}_{DateTime.Now:dd-MM-yyyy}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        return saveFileDialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al abrir el diálogo de guardado: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Exporta la lista actual de clientes a un archivo Excel (.xlsx) usando EPPlus,
        /// optimizado para evitar el problema N+1 en las consultas de contactos.
        /// </summary>
        public static Resultado<bool> ExportarClientesAExcel(List<Clientes> listaClientes)
        {
            if (listaClientes == null || !listaClientes.Any())
                return Resultado<bool>.Fail("No hay datos para exportar.");

            string? rutaDestino = ArchivosOfficce.GuardarArchivoXLSX("MisClientes");

            if (string.IsNullOrWhiteSpace(rutaDestino))
                return Resultado<bool>.Fail("Operación de exportación cancelada.");

            try
            {
                // 1. OPTIMIZACIÓN (Batching): Extraer todos los IDs y buscar los contactos en 1 solo viaje
                var idsPersonas = listaClientes
                    .Where(c => c.IdPersona > 0)
                    .Select(c => c.IdPersona)
                    .Distinct()
                    .ToList();

                var resultadoContactos = ContactosNegocio.GetContactosPorListaPersonas(idsPersonas);
                var todosLosContactos = resultadoContactos.Data ?? new List<Contactos>();

                // EPPlus requiere establecer licencia en versiones 7+
                ExcelPackage.License.SetNonCommercialPersonal("Anibal");

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Clientes");

                    // 2. Armar Encabezados
                    string[] headers = { "DNI", "Apellidos", "Nombres", "Fecha Nac.", "WhatsApp", "Email", "Teléfono", "Calle", "Altura", "Barrio" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    // 3. Llenar los datos
                    int fila = 2;
                    foreach (var cliente in listaClientes)
                    {
                        var p = cliente.Personas;
                        if (p == null) continue;

                        var d = p.Domicilios;

                        // CRUCE EN MEMORIA: Buscamos el contacto de esta persona en la lista que ya trajimos
                        var c = todosLosContactos.FirstOrDefault(x => x.IdPersona == p.IdPersona);

                        worksheet.Cells[fila, 1].Value = p.Dni;
                        worksheet.Cells[fila, 2].Value = p.Apellidos;
                        worksheet.Cells[fila, 3].Value = p.Nombres;
                        worksheet.Cells[fila, 4].Value = p.FechaNac?.ToString("dd/MM/yyyy") ?? "";

                        worksheet.Cells[fila, 5].Value = c?.Whatsapp ?? "";
                        worksheet.Cells[fila, 6].Value = c?.Email ?? "";
                        worksheet.Cells[fila, 7].Value = c?.Telefono ?? "";

                        worksheet.Cells[fila, 8].Value = d?.Calle ?? "";
                        worksheet.Cells[fila, 9].Value = d?.Altura ?? "";
                        worksheet.Cells[fila, 10].Value = d?.Barrio ?? "";

                        fila++;
                    }

                    // Autoajustar columnas para que quede prolijo
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // 4. Guardar el archivo físico
                    package.SaveAs(new FileInfo(rutaDestino));

                    return Resultado<bool>.Ok(true, $"Exportación exitosa. Archivo guardado en:\n{rutaDestino}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al exportar clientes a Excel: {ex.ToString()}");
                return Resultado<bool>.Fail("Ocurrió un error inesperado al intentar exportar el archivo. Verifique que no esté abierto en otro programa.");
            }
        }
    }
}
