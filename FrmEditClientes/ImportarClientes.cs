using Entidades_SGBM;
using Negocio_SGBM;
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
        private DateTime? _nacimiento = null;

        private bool _localidadOk = false;

        public ImportarClientes()
        {
            _ruta = ArchivosOfficce.SeleccionarArchivoCSV();
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
        private int ProcesarCSV(ref string mensaje)
        {
            int exitosos = 0;
            int fracasos = 0;
            int contador = 0;
            localidadOk(ref mensaje);
            string desarrollo = "Observaciones:";
            using (StreamReader reader = new StreamReader(_ruta))
            {
                while (!reader.EndOfStream)
                {
                    vaciarCampos();
                    contador++;
                    string mensajeInterno = "";
                    string linea = reader.ReadLine();
                    string[] valores = linea.Split(',');

                    // Asegurar que hay suficientes columnas antes de asignar valores
                    if (valores.Length >= 8)
                    {
                        _apellido = valores[0].Trim();
                        _nombre = valores[1].Trim();
                        _dni = valores[2].Trim();
                        _direccion = valores[3].Trim();
                        _email = valores[4].Trim();
                        _fijo = valores[5].Trim();
                        _whatsapp = valores[6].Trim();
                        _nacimiento = ParseFecha(valores[7].Trim());
                    } else
                    {
                        mensajeInterno = $"No fue posible interactuar con el cliente n° {contador}";
                        fracasos++;
                        continue;
                    }
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
            _contacto.Telefono = _fijo;
            _contacto.Whatsapp = _whatsapp;
            _contacto.Email = _email;
        } 

        private bool importarCliente(ref string mensaje)
        {
            return ClientesNegocio.importarCliente(_cliente, _contacto, ref mensaje);
        }

        public bool importarArchivoClientes()
        {
            string mensaje = "";
            bool exito = false;
            if (ProcesarCSV(ref mensaje) > 0)
            {
                exito = true;
            }
            observaciones = mensaje;
            return exito;
        }
    }
}
