using Entidades_SGBM;
using Negocio_SGBM;
using OfficeOpenXml;
using Utilidades;
using System.Text;

namespace Front_SGBM
{
    internal class ImportarClientes
    {
        public bool hayArchivo { get; }
        public string observaciones { get; set; } = string.Empty;

        private string? _ruta = null;
        private Localidades? _localidadGenerica = null;
        private Estados? _estadoActivo = null;

        // HashSet para validación local rápida
        private static readonly HashSet<string> codigosArea = new() { "3446", "3447", "3442", "3445", "3444" };

        /// <summary>
        /// Constructor: inicializa la ruta del archivo a importar.
        /// </summary>
        public ImportarClientes(string ruta)
        {
            _ruta = ruta;
            hayArchivo = !string.IsNullOrWhiteSpace(ruta) && File.Exists(ruta);
        }

        /// <summary>
        /// Ejecuta la importación de clientes desde archivo Excel/CSV.
        /// </summary>
        public Resultado<bool> ImportarArchivoClientes()
        {
            try
            {
                if (!hayArchivo) return Resultado<bool>.Fail("No se encontró el archivo.");

                // 1. Preparamos la Localidad Genérica ANTES de leer el Excel (Un solo viaje a BD)
                var resLoc = DomiciliosNegocio.GetLocalidadGenerica(null);
                if (!resLoc.Success || resLoc.Data == null)
                    return Resultado<bool>.Fail("No se pudo obtener ni crear la localidad genérica (Gualeguaychú) para la importación.");

                _localidadGenerica = resLoc.Data;

                // 2. Preparamos el Estado Activo ANTES de leer el Excel (Un solo viaje a BD)
                var resEstado = EstadosNegocio.GetEstadoActivo("Clientes");
                if (!resEstado.Success || resEstado.Data == null)
                    return Resultado<bool>.Fail("No se pudo obtener el estado 'Activo' para clientes.");

                _estadoActivo = resEstado.Data;

                // 3. Traemos todos los DNIs existentes (Un solo viaje a BD)
                var resDnis = PersonasNegocio.GetTodosLosDnis();
                HashSet<string> dnisExistentes = resDnis.Success && resDnis.Data != null ? resDnis.Data : new HashSet<string>();

                // 4. Procesamos el Excel
                return ProcesarExcel(dnisExistentes);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al importar archivo de clientes:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail("Ocurrió un error fatal durante la importación.");
            }
        }

        /// <summary>
        /// Lee el archivo, procesa cada fila, ensambla los objetos y separa en listas.
        /// </summary>
        private Resultado<bool> ProcesarExcel(HashSet<string> dnisExistentes)
        {
            int errores = 0;
            var observacionesBld = new StringBuilder();

            // Usamos Tuplas para empaquetar al Cliente junto con su Contacto (ya que no van anidados)
            var clientesNuevos = new List<(Clientes cliente, Contactos? contacto)>();
            var clientesModificados = new List<(Clientes cliente, Contactos? contacto)>();

            try
            {
                // EPPlus requiere establecer licencia en versiones 7+
                ExcelPackage.License.SetNonCommercialPersonal("Anibal");

                using (var package = new ExcelPackage(new FileInfo(_ruta!)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null) return Resultado<bool>.Fail("El archivo está vacío.");

                    int totalFilas = worksheet.Dimension?.Rows ?? 0;

                    // Asumimos Fila 1 = Encabezados. CSV: apellido, nombre, documento, domicilio, email, Tel., Whatsapp, fechanacimiento
                    for (int fila = 2; fila <= totalFilas; fila++)
                    {
                        try
                        {
                            string apellido = (worksheet.Cells[fila, 1].Value?.ToString() ?? "").Trim();
                            string nombre = (worksheet.Cells[fila, 2].Value?.ToString() ?? "").Trim();
                            string dni = (worksheet.Cells[fila, 3].Value?.ToString() ?? "").Trim();

                            // Ignoramos si DNI o Apellido están vacíos
                            if (string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(apellido))
                            {
                                errores++;
                                observacionesBld.AppendLine($"Fila {fila}: Omitida (DNI o Apellido vacío).");
                                continue;
                            }

                            // --- PERSONA ---
                            var persona = new Personas
                            {
                                Dni = dni,
                                Apellidos = Validaciones.CapitalizarTexto(apellido),
                                Nombres = Validaciones.CapitalizarTexto(nombre)
                            };

                            string fechaStr = (worksheet.Cells[fila, 8].Value?.ToString() ?? "").Trim();
                            if (!string.IsNullOrWhiteSpace(fechaStr) && fechaStr != "0000-00-00")
                            {
                                if (DateTime.TryParse(fechaStr, out DateTime fn))
                                    persona.FechaNac = fn;
                            }

                            // --- DOMICILIO ---
                            string domicilioStr = (worksheet.Cells[fila, 4].Value?.ToString() ?? "").Trim();
                            if (!string.IsNullOrWhiteSpace(domicilioStr))
                            {
                                persona.Domicilios = new Domicilios
                                {
                                    Calle = Validaciones.CapitalizarTexto(domicilioStr),
                                    IdLocalidad = _localidadGenerica!.IdLocalidad
                                };
                            }

                            // --- CONTACTO ---
                            string email = (worksheet.Cells[fila, 5].Value?.ToString() ?? "").Trim();
                            string tel = (worksheet.Cells[fila, 6].Value?.ToString() ?? "").Trim();
                            string wsp = (worksheet.Cells[fila, 7].Value?.ToString() ?? "").Trim();

                            Contactos? contacto = null;
                            if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(tel) || !string.IsNullOrWhiteSpace(wsp))
                            {
                                contacto = new Contactos
                                {
                                    Email = email,
                                    Telefono = tel,
                                    Whatsapp = LimpiarWhatsapp(wsp),
                                    ExtranjeroWhatsapp = false // Por defecto falso
                                };
                            }

                            // --- CLIENTE FINAL ---
                            var cliente = new Clientes
                            {
                                Personas = persona,
                                IdEstado = _estadoActivo!.IdEstado,
                            };

                            // Separador Mágico O(1)
                            if (dnisExistentes.Contains(dni))
                                clientesModificados.Add((cliente, contacto));
                            else
                                clientesNuevos.Add((cliente, contacto));
                        }
                        catch (Exception filaEx)
                        {
                            errores++;
                            observacionesBld.AppendLine($"Fila {fila}: Error -> {filaEx.Message}");
                        }
                    }

                    // =========================================================================
                    // DELEGACIÓN A LA CAPA DE NEGOCIO
                    // =========================================================================
                    // Como el requerimiento es "comparando todo en la capa negocios",
                    // enviamos estas listas a ClientesNegocio para que procese todo el lote.

                    var resMasivo = ClientesNegocio.ImportarLote(clientesNuevos, clientesModificados);

                    observacionesBld.Insert(0, $"Total Nuevos: {clientesNuevos.Count} | Total a Actualizar: {clientesModificados.Count} | Errores lectura: {errores}\n\n");
                    observaciones = observacionesBld.ToString() + "\n" + resMasivo.Mensaje;

                    return Resultado<bool>.Ok(true, observaciones);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al procesar el Excel: {ex.Message}");
                return Resultado<bool>.Fail("Ocurrió un error al leer las celdas del archivo.");
            }
        }

        /// <summary>
        /// Limpia el código internacional del Whatsapp si es de Argentina (549).
        /// </summary>
        private string LimpiarWhatsapp(string wspOriginal)
        {
            if (string.IsNullOrWhiteSpace(wspOriginal)) return "";

            string limpio = wspOriginal.Replace("+", "").Replace(" ", "").Replace("-", "");

            // Si es un celular argentino con el 549 por delante, se lo quitamos
            if (limpio.StartsWith("549") && limpio.Length > 10)
            {
                return limpio.Substring(3); // Ejemplo: 5493446664141 -> 3446664141
            }
            return limpio;
        }
    }
}
