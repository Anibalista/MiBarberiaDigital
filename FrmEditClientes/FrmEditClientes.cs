using Entidades_SGBM;
using Negocio_SGBM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Front_SGBM
{
    public partial class FrmEditClientes : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;
        public Personas? _persona = null;
        public Clientes? _cliente = null;
        private Domicilios? _domicilio = null;
        public List<Contactos>? _contactos = null;
        private List<Provincias>? _provincias = null;
        private List<Localidades>? _localidades = null;
        private Provincias? _provincia = null;
        private Localidades? _localidad = null;
        public bool cerrando = false;

        //Valores de campos
        private string _dni = string.Empty;
        private string _nombres = string.Empty;
        private string _apellidos = string.Empty;
        private string? _calle = string.Empty;
        private string? _barrio = string.Empty;
        private string? _altura = string.Empty;
        private string? _depto = string.Empty;
        private string? _piso = string.Empty;
        private DateTime? _nacimiento = null;

        public FrmEditClientes()
        {
            InitializeComponent();
        }
        private void FrmEditClientes_Load(object sender, EventArgs e)
        {
            cargarProvincias();
            cargarLocalidades();
            
        }

        //Comprobaciones
        private bool modoContactos()
        {
            if (_contactos == null)
            {
                return false;
            }
            return _contactos.Count > 0;
        }

        private bool buscarCliente(ref string mensaje)
        {
            _cliente = null;
            string mens = "";
            if (!comprobarDni(ref mensaje))
            {
                return false;
            }
            _cliente = ClientesNegocio.getClientePorDni(_dni, ref mens);
            if (!String.IsNullOrWhiteSpace(mens))
            {
                mensaje = mens;
            }
            return _cliente != null;
        }

        private bool comprobarDni(ref string mensaje)
        {
            _dni = txtDni.Text;
            if (String.IsNullOrWhiteSpace(_dni))
            {
                mensaje = "Ingrese un Dni antes de continuar";
                return false;
            }
            if (_dni.Length > 9 || _dni.Length < 6)
            {
                mensaje = "La longitud del Dni es menor o mayor a lo esperado";
                return false;
            }
            return true;
        }

        private bool comprobarNombres(ref string mensaje)
        {
            _nombres = txtNombre.Text;
            _apellidos = txtApellido.Text;
            if (String.IsNullOrWhiteSpace(_nombres) || String.IsNullOrWhiteSpace(_apellidos))
            {
                mensaje += "Falta Ingresar Nombres o Apellidos";
                return false;
            }
            return true;
        }

        private void cargarNacimiento()
        {
            try
            {
                // Obtener la fecha seleccionada en el DateTimePicker
                DateTime fechaSeleccionada = dateTimePicker1.Value;

                // Verificar si la fecha es menor a la actual
                if (fechaSeleccionada < DateTime.Now)
                {
                    _nacimiento = fechaSeleccionada;
                }
                else
                {
                    _nacimiento = null;
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("Error al comprobar la fecha de nacimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool domicilioCorrecto(ref string mensaje)
        {
            if (!direccionIngresada()) { return false; }
            if (!localidadIngresada(ref mensaje))
            {
                return false;
            }
            return true;
        }

        private bool direccionIngresada()
        {
            _calle = String.IsNullOrWhiteSpace(txtCalle.Text) ? null : txtCalle.Text;
            _barrio = String.IsNullOrWhiteSpace(txtBarrio.Text) ? null : txtBarrio.Text;
            _altura = String.IsNullOrWhiteSpace(txtNro.Text) ? null : txtNro.Text;
            _piso = String.IsNullOrWhiteSpace(txtPiso.Text) ? null : txtPiso.Text;
            _depto = String.IsNullOrWhiteSpace(txtDepto.Text) ? null : txtDepto.Text;
            return !String.IsNullOrWhiteSpace(_calle) || !String.IsNullOrWhiteSpace(_barrio);
        }

        private bool localidadIngresada(ref string mensaje)
        {
            _localidad = null;
            if (provinciaIngresada())
            {
                mensaje = "Para ingresar un domicilio escriba o seleccione una localidad y provincia";
                return false;
            }
            string localidad = cbLocalidad.Text;
            if (String.IsNullOrWhiteSpace(localidad))
            {
                mensaje = "Para ingresar un domicilio escriba o seleccione una localidad y provincia";
                return false;
            }
            string mens = "";

            try
            {
                if (_localidades != null && _localidades.Count > 0)
                {
                    _localidad = _localidades.Where(l => l.Localidad == localidad).FirstOrDefault();
                } else
                {
                    _localidad = new();
                    _localidad.Localidad = localidad;
                    
                }
                if (_provincia.IdProvincia != null && _provincia.IdProvincia > 0)
                {
                    _localidad.IdProvincia = _provincia.IdProvincia;
                }
                else
                {
                    _localidad = new();
                    _localidad.Localidad = localidad;
                    _localidad.Provincias = _provincia;
                }

            } catch (Exception ex)
            {
                mens = ex.Message;
            }
            return _localidad != null;
        }

        private bool provinciaIngresada()
        {
            _provincia = null;
            string provincia = cbProvincia.Text;
            if (String.IsNullOrWhiteSpace(provincia))
            {
                return false;
            }
            if (_provincias == null || _provincias.Count < 1)
            {
                _provincia = new();
                _provincia.Provincia = provincia;
                return true;
            }
            try
            {
                _provincia = bindingProvincias.Current as Provincias;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return _provincia != null;
        }

        //Métodos
        private void refrescarGrilla()
        {
            bindingContactos.Clear();
            bindingContactos.DataSource = _contactos;
            dataGridContactos.Refresh();
        }

        private void cargarProvincias()
        {
            _provincias = DomiciliosNegocio.getProvincias(); ;
            if (_provincias == null)
            {
                _provincias = new List<Provincias>();
            }
            bindingProvincias.DataSource = _provincias;
            cbProvincia.SelectedIndex = 0;
        }

        private void cargarLocalidades()
        {
            bindingLocalidades.Clear();
            _localidades = new List<Localidades>();
            string provincia = cbProvincia.Text;
            if (String.IsNullOrWhiteSpace(provincia))
            {
                bindingLocalidades.DataSource = _localidades;
                return;
            }
            _provincia = null;
            try
            {
                _provincia = _provincias.Where(p => p.Provincia == provincia).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (_provincia != null)
            {
                _localidades = DomiciliosNegocio.getLocalidadesPorProvincia(_provincia);
            }
            bindingLocalidades.DataSource = _localidades;
        }

        public bool traerContactos(List<Contactos>? contactos)
        {
            if (contactos == null)
            {
                return false;
            }
            if (contactos.Count < 1)
            {
                return false;
            }
            _contactos = new List<Contactos>();
            _contactos = contactos;
            refrescarGrilla();
            return true;
        }

        private bool ingresarDomicilio(ref string mensaje)
        {
            if (!domicilioCorrecto(ref mensaje))
            {
                return false;
            }

            if (modo == EnumModoForm.Alta || _domicilio == null)
            {
                _domicilio = new();
            }
            
            _domicilio.Calle = _calle;
            _domicilio.Altura = _altura;
            _domicilio.Piso = _piso;
            _domicilio.Barrio = _barrio;
            _domicilio.Depto = _depto;
            if (_localidad != null)
            {
                if (_localidad.IdLocalidad != null && _localidad.IdLocalidad > 0)
                {
                    _domicilio.IdLocalidad = _localidad.IdLocalidad;
                } else
                {
                    _domicilio.Localidades = _localidad;
                }
            }
            return true;
        }

        

        //Botones
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            cerrando = true;
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string mensaje = "";
            bool existe = buscarCliente(ref mensaje);
            if (!String.IsNullOrWhiteSpace(mensaje))
            {
                MessageBox.Show($"Error: {mensaje}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            mensaje = existe ? $"El Dni ingresado corresponde al cliente:\n{_cliente.NombreCompleto}" : "El Dni ingresado no corresponde a ningún cliente registrado";

            if (!existe)
            {
                if (modo == EnumModoForm.Alta)
                {
                    MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnGuardar.Enabled = true;
                } else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnGuardar.Enabled = false;
                }
                return;
            }
            else
            {
                if (modo == EnumModoForm.Modificacion)
                {
                    MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnBuscar.Enabled = true;
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnGuardar.Enabled = false;
                }
                return;
            }
        }

        private void linkContactos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EnumModoForm modoCont = EnumModoForm.Alta;
            if (modoContactos())
            {
                modoCont = EnumModoForm.Modificacion;
            }

            //Método por si acaso pero no está de más
            FrmMenuPrincipal? principal = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (principal == null)
            {
                return;
            }

            FrmContactos? abierto = Application.OpenForms.OfType<FrmContactos>().FirstOrDefault();
            if (abierto != null)
            {
                abierto.Close();
            }

            FrmContactos formulario = new FrmContactos();
            formulario.modo = modoCont;
            formulario._contactos = _contactos;
            formulario.MdiParent = principal;
            formulario.Show();
        }
        private void cbProvincia_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando)
            {
                return;
            }
            cargarLocalidades();
            
        }

        //Filtros
        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
