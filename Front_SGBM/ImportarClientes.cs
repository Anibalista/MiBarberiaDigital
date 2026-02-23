using Entidades_SGBM;
using Negocio_SGBM;
using OfficeOpenXml;
using Utilidades;

namespace Front_SGBM
{
    internal class ImportarClientes
    {
        public bool hayArchivo {  get; }
        public string observaciones { get; set; }

        //Objetos
        private Clientes? _cliente = null;
        private Personas? _persona = null;
        private Contactos? _contacto = null;
        private Domicilios? _domicilio = null;
        private Localidades? _localidad = null;

        //Valores
        private string? _ruta = null;
        private string _dni = string.Empty;
        private string _apellido = string.Empty;
        private string _nombre = string.Empty;
        private string _direccion = string.Empty;
        private string _email = string.Empty;
        private string _fijo = string.Empty;
        private string _whatsapp = string.Empty;
        private bool _extranjeroWhatsapp = true;
        private DateTime? _nacimiento = null;

        private bool _localidadOk = false;
        private static readonly HashSet<string> codigosArea = new() { "3446", "3447", "3442", "3445", "3444" };


        /// <summary>
        /// Constructor: inicializa la ruta del archivo XLSX a importar.
        /// </summary>
        public ImportarClientes()
        {
            _ruta = ArchivosOfficce.SeleccionarArchivoXLSX();
            hayArchivo = !string.IsNullOrEmpty(_ruta);
            observaciones = "";
        }

        /// <summary>
        /// Vacía los campos internos antes de procesar una nueva fila del Excel.
        /// </summary>
        private void VaciarCampos()
        {
            _dni = string.Empty;
            _apellido = string.Empty;
            _nombre = string.Empty;
            _direccion = string.Empty;
            _email = string.Empty;
            _fijo = string.Empty;
            _whatsapp = string.Empty;
            _nacimiento = null;
            _domicilio = null;
            _cliente = null;
            _persona = null;
        }

        /// <summary>
        /// Convierte un string en fecha válida, o null si es inválida.
        /// </summary>
        private DateTime? ParseFecha(string fechaStr)
        {
            if (DateTime.TryParse(fechaStr, out DateTime fecha) && fecha.Year > 1900)
                return fecha;

            return null;
        }

        /// <summary>
        /// Construye un domicilio válido a partir de la dirección y localidad.
        /// </summary>
        private bool DomicilioCorrecto()
        {
            if (string.IsNullOrWhiteSpace(_direccion))
            {
                _domicilio = null;
                return false;
            }
            if (!_localidadOk) return false;

            _domicilio = new Domicilios();
            string[] partes = _direccion.Split(' ');
            _domicilio.Calle = string.Join(" ", partes.Take(partes.Length - 1));
            _domicilio.Altura = partes.Last();

            if (_localidad.IdLocalidad == null)
                _domicilio.Localidades = _localidad;
            else
                _domicilio.IdLocalidad = (int)_localidad.IdLocalidad;

            return true;
        }

        /// <summary>
        /// Valida y obtiene la localidad genérica para el cliente.
        /// Devuelve un <see cref="Resultado{T}"/> con true si la localidad es válida,
        /// o con el mensaje de error en caso contrario.
        /// </summary>
        private Resultado<bool> LocalidadOk()
        {
            try
            {
                var resultado = DomiciliosNegocio.GetLocalidadGenerica(_localidad);

                if (!resultado.Success || resultado.Data == null)
                {
                    _localidadOk = false;
                    return Resultado<bool>.Fail(resultado.Mensaje);
                }

                _localidad = resultado.Data;
                _localidadOk = _localidad.IdLocalidad != null;

                return Resultado<bool>.Ok(_localidadOk, "Localidad validada correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al validar localidad:\n{ex.ToString()}";
                Logger.LogError(msg);
                _localidadOk = false;
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Procesa el archivo Excel y genera clientes a partir de sus filas.
        /// Devuelve un <see cref="Resultado{T}"/> con la cantidad de clientes importados exitosamente,
        /// junto con un mensaje detallado de observaciones.
        /// </summary>
        public Resultado<int> ProcesarExcel()
        {
            int exitosos = 0, fracasos = 0, contador = 0;

            // Validar localidad antes de procesar
            var resultadoLocalidad = LocalidadOk();
            if (!resultadoLocalidad.Success)
                return Resultado<int>.Fail(resultadoLocalidad.Mensaje);

            string desarrollo = "Observaciones:";

            FileInfo archivo;
            try
            {
                archivo = new FileInfo(_ruta);
            }
            catch (Exception ex)
            {
                return Resultado<int>.Fail("Error al abrir archivo: " + ex.Message);
            }

            // EPPlus requiere establecer licencia en versiones 7+
            ExcelPackage.License.SetNonCommercialPersonal("Anibal");

            try
            {
                using (ExcelPackage package = new ExcelPackage(archivo))
                {
                    ExcelWorksheet hoja = package.Workbook.Worksheets[0];

                    for (int fila = 2; fila <= hoja.Dimension.Rows; fila++)
                    {
                        VaciarCampos();
                        contador++;

                        _apellido = hoja.Cells[fila, 1].Text.Trim();
                        _nombre = hoja.Cells[fila, 2].Text.Trim();
                        _dni = hoja.Cells[fila, 3].Text.Trim();
                        _direccion = hoja.Cells[fila, 4].Text.Trim();
                        _email = hoja.Cells[fila, 5].Text.Trim();
                        _fijo = hoja.Cells[fila, 6].Text.Trim();
                        _whatsapp = hoja.Cells[fila, 7].Text.Trim();
                        _nacimiento = ParseFecha(hoja.Cells[fila, 8].Text.Trim());

                        // Saltar encabezado
                        if (_apellido == "apellido")
                        {
                            contador--;
                            continue;
                        }

                        // Construir cliente
                        if (!ArmarCliente())
                        {
                            desarrollo += $"\nNo fue posible armar cliente {_apellido} {_nombre}";
                            fracasos++;
                            continue;
                        }

                        // Importar cliente
                        var resultadoImportar = ImportarCliente();
                        if (!resultadoImportar.Success)
                        {
                            desarrollo += $"\n{resultadoImportar.Mensaje}";
                            fracasos++;
                        }
                        else
                        {
                            exitosos++;
                        }
                    }
                }

                string mensajeFinal = $"Se procesaron {contador} líneas del archivo \nExitosos: {exitosos} | Fracasos: {fracasos}\n{desarrollo}";
                return Resultado<int>.Ok(exitosos, mensajeFinal);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al procesar Excel:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Construye la entidad Cliente y Persona a partir de los datos cargados.
        /// </summary>
        private bool ArmarCliente()
        {
            if (string.IsNullOrWhiteSpace(_dni)) return false;

            _cliente = new Clientes();
            _persona = new Personas
            {
                Dni = _dni,
                Nombres = _nombre,
                Apellidos = _apellido,
                FechaNac = _nacimiento
            };
            _cliente.Personas = _persona;

            if (DomicilioCorrecto())
            {
                _cliente.Personas.IdDomicilio = _domicilio?.IdDomicilio;
                _cliente.Personas.Domicilios = _domicilio;
            }

            ArmarContacto();
            return true;
        }

        /// <summary>
        /// Construye el contacto asociado al cliente si hay datos disponibles.
        /// </summary>
        private void ArmarContacto()
        {
            if (string.IsNullOrWhiteSpace(_fijo) && string.IsNullOrWhiteSpace(_whatsapp) && string.IsNullOrWhiteSpace(_email))
                return;

            _contacto = new Contactos
            {
                Telefono = ProcesarFijo(),
                Whatsapp = ProcesarWhatsapp(),
                ExtranjeroWhatsapp = _contacto?.Whatsapp != null ? _extranjeroWhatsapp : false,
                Email = _email
            };
        }

        /// <summary>
        /// Procesa y normaliza el número de WhatsApp.
        /// </summary>
        private string? ProcesarWhatsapp()
        {
            if (string.IsNullOrWhiteSpace(_whatsapp)) return null;

            if (_whatsapp.StartsWith("540"))
                _whatsapp = "549" + _whatsapp.Substring(3);

            if (_whatsapp.StartsWith("5490"))
                _whatsapp = "549" + _whatsapp.Substring(4);

            if (_whatsapp.StartsWith("54") && !_whatsapp.StartsWith("549"))
                _whatsapp = _whatsapp.Insert(2, "9");

            if (_whatsapp.Length < 7) return null;

            string area = _whatsapp.Substring(0, 4);
            if (codigosArea.Contains(area))
                _whatsapp = "549" + _whatsapp;

            if (_whatsapp.StartsWith("549"))
            {
                _extranjeroWhatsapp = false;
                area = _whatsapp.Substring(3, 4);

                if (_whatsapp.StartsWith("54911"))
                    _whatsapp = _whatsapp.Insert(5, "-");
                else if (codigosArea.Contains(area))
                    _whatsapp = _whatsapp.Insert(7, "-");
                else
                    _whatsapp = _whatsapp.Insert(6, "-");
            }
            else
            {
                _whatsapp = _whatsapp.Insert(3, "-");
            }

            return _whatsapp;
        }

        /// <summary>
        /// Procesa y normaliza el número de teléfono fijo.
        /// </summary>
        private string? ProcesarFijo()
        {
            if (string.IsNullOrWhiteSpace(_fijo)) return null;
            if (_fijo.Length < 7) return null;

            if (_fijo.StartsWith("0"))
                _fijo = _fijo.Substring(1);

            if (_fijo.StartsWith("11"))
                _fijo = _fijo.Insert(2, "-");
            else if (codigosArea.Contains(_fijo.Substring(0, 4)))
                _fijo = _fijo.Insert(4, "-");
            else
                _fijo = _fijo.Insert(3, "-");

            return _fijo;
        }

        /// <summary>
        /// Importa un cliente usando la capa negocio.
        /// Devuelve un <see cref="Resultado{T}"/> con true si la importación fue exitosa,
        /// o con el mensaje de error en caso contrario.
        /// </summary>
        private Resultado<bool> ImportarCliente()
        {
            if (_cliente == null)
                return Resultado<bool>.Fail("No se pudo construir la entidad Cliente para importar.");

            try
            {
                var resultado = ClientesNegocio.ImportarCliente(_cliente, _contacto);
                if (!resultado.Success)
                    return Resultado<bool>.Fail(resultado.Mensaje);

                return Resultado<bool>.Ok(true, "Cliente importado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al importar cliente:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Ejecuta la importación de clientes desde archivo Excel.
        /// Devuelve un <see cref="Resultado{T}"/> con true si la importación fue exitosa,
        /// junto con las observaciones generadas durante el proceso.
        /// </summary>
        public Resultado<bool> ImportarArchivoClientes()
        {
            try
            {
                var resultado = ProcesarExcel();

                if (!resultado.Success)
                {
                    observaciones = resultado.Mensaje;
                    return Resultado<bool>.Fail(resultado.Mensaje);
                }

                observaciones = resultado.Mensaje;
                bool exito = resultado.Data > 0;

                if (!exito)
                    return Resultado<bool>.Fail("No se pudo importar ningún cliente.\n" + observaciones);

                return Resultado<bool>.Ok(true, observaciones);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al importar archivo de clientes:\n{ex.ToString()}";
                Logger.LogError(msg);
                observaciones = msg;
                return Resultado<bool>.Fail(msg);
            }
        }
    }
}
