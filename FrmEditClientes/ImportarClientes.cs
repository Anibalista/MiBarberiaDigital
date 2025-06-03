using Entidades_SGBM;
using Negocio_SGBM;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public ImportarClientes()
        {
            _ruta = ArchivosOfficce.SeleccionarArchivoXLSX();
            hayArchivo = !String.IsNullOrEmpty(_ruta);
            observaciones = "";
        }

        private void vaciarCampos()
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

        private void localidadOk(ref string mensaje)
        {
            _localidad = DomiciliosNegocio.getLocalidadGenerica(_localidad, ref mensaje);
            if (_localidad == null)
            {
                _localidadOk = false;
                return;
            }
            _localidadOk = _localidad.IdLocalidad != null;
        }

        private bool domicilioCorrecto()
        {
            if (string.IsNullOrWhiteSpace(_direccion))
            {
                _domicilio = null;
                return false;
            }
            if (!_localidadOk)
            {
                return false;
            }
            _domicilio = new();
            string[] partes = _direccion.Split(' ');
            _domicilio.Calle = string.Join(" ", partes.Take(partes.Length - 1));
            _domicilio.Altura = partes.Last();
            if (_localidad.IdLocalidad == null)
            {
                _domicilio.Localidades = _localidad;
            } else
            {
                _domicilio.IdLocalidad = (int) _localidad.IdLocalidad;
            }
            
            return true;
            
        }

        private DateTime? ParseFecha(string fechaStr)
        {
            if (DateTime.TryParse(fechaStr, out DateTime fecha) && fecha.Year > 1900)
            {
                return fecha;
            }
            return null; // Si es inválida, asigna null
        }

        //Importar los clientes
        private int ProcesarExcel(ref string mensaje)
        {
            int exitosos = 0;
            int fracasos = 0;
            int contador = 0;
            localidadOk(ref mensaje);
            string desarrollo = "Observaciones:";
            FileInfo archivo;
            try
            {
                archivo = new FileInfo(_ruta);
            } catch (Exception ex)
            {
                mensaje = "Error :" + ex.Message;
                return 0;
            }
            
            // Establecer el contexto de licencia correctamente en EPPlus 7+
            ExcelPackage.License.SetNonCommercialPersonal("Anibal");

            using (ExcelPackage package = new ExcelPackage(archivo))
            {
                ExcelWorksheet hoja = package.Workbook.Worksheets[0];

                for (int fila = 2; fila <= hoja.Dimension.Rows; fila++)
                {
                    vaciarCampos();
                    contador++;
                    string mensajeInterno = "";
                    _apellido = hoja.Cells[fila, 1].Text.Trim();
                    _nombre = hoja.Cells[fila, 2].Text.Trim();
                    _dni = hoja.Cells[fila, 3].Text.Trim();
                    _direccion = hoja.Cells[fila, 4].Text.Trim();
                    _email = hoja.Cells[fila, 5].Text.Trim();
                    _fijo = hoja.Cells[fila, 6].Text.Trim();
                    _whatsapp = hoja.Cells[fila, 7].Text.Trim();
                    _nacimiento = ParseFecha(hoja.Cells[fila, 8].Text.Trim());

                    if (_apellido == "apellido")
                    {
                        contador--;
                        continue;
                    }
                    if (!armarCliente())
                    {
                        mensajeInterno = $"No fue posible interactuar con el cliente {_apellido} {_nombre}";
                        fracasos++;
                        continue;
                    }
                    if (!importarCliente(ref mensajeInterno))
                    {
                        mensajeInterno = $"No fue posible interactuar con el cliente n° {contador}";
                        fracasos++;
                    } else
                    {
                        exitosos++;
                    }
                    if (!String.IsNullOrWhiteSpace(mensajeInterno))
                    {
                        desarrollo += $"\n{mensajeInterno}";
                    }
                }
            }
            mensaje = $"Se procesaron {contador} lineas del archivo \nExitosos: {exitosos} | Fracasos: {fracasos}\n{mensaje}\n{desarrollo}";
            return exitosos;
        }

        private bool armarCliente()
        {
            if (String.IsNullOrWhiteSpace(_dni))
            {
                return false;
            }            
            _cliente = new Clientes();
            _persona = new Personas();
            _persona.Dni = _dni;
            _persona.Nombres = _nombre;
            _persona.Apellidos = _apellido;
            _persona.FechaNac = _nacimiento;
            _cliente.Personas = _persona;
            if (domicilioCorrecto())
            {
                _cliente.Personas.IdDomicilio = _domicilio?.IdDomicilio;
                _cliente.Personas.Domicilios = _domicilio;
            }
            armarContacto();
            return true;
        }

        private void armarContacto()
        {
            if (String.IsNullOrWhiteSpace(_fijo) && String.IsNullOrWhiteSpace(_whatsapp) && String.IsNullOrWhiteSpace(_email))
            {
                return;
            }
            _contacto = new Contactos();
            _contacto.Telefono = procesarFijo();
            _contacto.Whatsapp = procesarWhatsapp();
            _contacto.ExtranjeroWhatsapp = _contacto.Whatsapp != null ? _extranjeroWhatsapp : false;
            _contacto.Email = _email;
        }

        private string? procesarWhatsapp()
        {
            if (string.IsNullOrWhiteSpace(_whatsapp))
            {
                return null;
            }
            // Si empieza con "540", reemplazar por "549"
            if (_whatsapp.StartsWith("540"))
            {
                _whatsapp = "549" + _whatsapp.Substring(3);
            }

            // Si empieza con "5490", reemplazar por "549"
            if (_whatsapp.StartsWith("5490"))
            {
                _whatsapp = "549" + _whatsapp.Substring(4);
            }

            // Si empieza con "54" sin el 9, se lo agrega
            if (_whatsapp.StartsWith("54") && !_whatsapp.StartsWith("549"))
            {
                _whatsapp = _whatsapp.Insert(2, "9");
            }

            if (_whatsapp.Length < 7)
            {
                return null;
            }

            string area = _whatsapp.Substring(0, 4);
            if (codigosArea.Contains(area))
            {
                _whatsapp = "549" + _whatsapp;
            }

            // Si empieza con "549", insertar un guion después de los primeros 6 caracteres
            if (_whatsapp.StartsWith("549"))
            {
                _extranjeroWhatsapp = false;
                area = _whatsapp.Substring(3, 4); // Extraer los siguientes 4 dígitos
                if (_whatsapp.StartsWith("54911"))
                {
                    _whatsapp = _whatsapp.Insert(5, "-");
                }
                else if (codigosArea.Contains(area))
                {
                    _whatsapp = _whatsapp.Insert(7, "-"); // Separar código de área con guion
                }
                else
                {
                    _whatsapp = _whatsapp.Insert(6, "-"); // Para otros números, guion después de los primeros 6
                } 
                
            } else
            {
                _whatsapp = _whatsapp.Insert(3, "-");
            }

            return _whatsapp;
        }

        private string? procesarFijo()
        {
            if (string.IsNullOrWhiteSpace(_fijo))
            {
                return null;
            }
            //Si no tiene un largo suficiente no lo registra
            if (_fijo.Length < 7)
            {
                return null;
            }
            // Si empieza con "0", se lo quita
            if (_fijo.StartsWith("0"))
            {
                _fijo = _fijo.Substring(1);
            }

            // Manejo especial para el código de área 11 (Buenos Aires)
            if (_fijo.StartsWith("11"))
            {
                _fijo = _fijo.Insert(2, "-");
            }
            // Si los primeros 4 caracteres son un código de área conocido, se coloca el guion después de 4 dígitos
            else if (codigosArea.Contains(_fijo.Substring(0, 4)))
            {
                _fijo = _fijo.Insert(4, "-");
            }
            // Para otros números, se coloca el guion después de los primeros 3 dígitos
            else
            {
                _fijo = _fijo.Insert(3, "-");
            }

            return _fijo;

        }

        private bool importarCliente(ref string mensaje)
        {
            return ClientesNegocio.importarCliente(_cliente, _contacto, ref mensaje);
        }

        public bool importarArchivoClientes()
        {
            string mensaje = "";
            bool exito = false;
            if (ProcesarExcel(ref mensaje) > 0)
            {
                exito = true;
            }
            observaciones = mensaje;
            return exito;
        }
    }
}
