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

        //Listas para comboBox
        public List<Contactos>? _contactos = null;
        private List<Provincias>? _provincias = null;
        private List<Localidades>? _localidades = null;
        private List<Estados>? _estados = null;

        //Objetos importantes
        public Personas? _persona = null;
        public Clientes? _cliente = null;
        private Domicilios? _domicilio = null;
        private Provincias? _provincia = null;
        private Localidades? _localidad = null;
        private Estados? _estado = null;
        public bool cerrando = false;
        public bool venta = false;
        public bool editandoContactos = false;

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
            dateTimePicker1.MaxDate = DateTime.Now.AddDays(2);
            dateTimePicker1.MinDate = DateTime.Now.AddYears(-140);
        }
        private void FrmEditClientes_Load(object sender, EventArgs e)
        {
            cargarFormulario();
        }

        private void cargarFormulario()
        {
            string titulo = "Registro";
            fechaInicial();
            if (modo != EnumModoForm.Alta)
            {
                cargarDatosCliente();
                cargarContactos();
                titulo = modo == EnumModoForm.Modificacion ? "Modificación" : "Detalles";
            }
            cargarEstados();
            cargarProvincias();
            cargarLocalidades();
            activarCampos(modo != EnumModoForm.Consulta);

            labelTitulo.Text = $"{titulo} de Cliente";
        }

        private void cargarDatosCliente()
        {
            if (!cargarCliente())
                return;
            string mensaje = string.Empty;
            try
            {
                _contactos = null;
                _contactos = ContactosNegocio.getContactosPorPersona(_persona, ref mensaje);
                cargarCamposClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del cliente\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cargarCamposClientes()
        {
            if (_cliente == null || _persona == null)
                return;
            txtApellido.Text = _persona?.Apellidos;
            txtDni.Text = _persona?.Dni;
            txtNombre.Text = _persona?.Nombres;
            dateTimePicker1.Value = _persona?.FechaNac ?? DateTime.Today;
            if (_estado != null)
                cbEstados.SelectedItem = _estado;
            if (_domicilio != null)
            {
                txtCalle.Text = _domicilio.Calle;
                txtBarrio.Text = _domicilio.Barrio;
                txtNro.Text = _domicilio.Altura;
                txtPiso.Text = _domicilio.Piso;
                txtDepto.Text = _domicilio.Depto;
                if (_provincia != null)
                    cbProvincia.SelectedItem = _provincia;
                if (_localidad != null)
                    cbLocalidad.SelectedItem = _localidad;
            }
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
            _cliente = ClientesNegocio.GetClientePorDni(_dni, ref mens);
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

        private void comprobarNacimiento()
        {
            try
            {
                // Obtener la fecha seleccionada en el DateTimePicker
                DateTime fechaSeleccionada = dateTimePicker1.Value;

                // Verificar si la fecha es menor a la actual
                if (fechaSeleccionada < DateTime.Today.AddDays(-1))
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
            if (!provinciaIngresada())
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
                }
                if (_localidad == null)
                {
                    _localidad = new();
                    _localidad.Localidad = localidad;

                }
                if (_provincia.IdProvincia != null)
                {
                    _localidad.IdProvincia = (int)_provincia.IdProvincia;
                }
                else
                {
                    _localidad = new();
                    _localidad.Localidad = localidad;
                    _localidad.Provincias = _provincia;
                }

            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return _provincia != null;
        }

        private bool comprobarCliente(ref string mensaje, bool registro)
        {
            if (buscarCliente(ref mensaje))
            {
                if (registro)
                    return false;
            }
            if (!comprobarNombres(ref mensaje))
            {
                return false;
            }
            comprobarNacimiento();
            try
            {
                _estado = (Estados)cbEstados.SelectedItem;
                if (_estado.IdEstado == null && modo == EnumModoForm.Modificacion)
                {
                    mensaje = "Problemas con la recuperación de estados de clietes de la BD";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }

            return true;
        }

        //Métodos
        private void activarCampos(bool activar)
        {
            txtDni.Enabled = activar;
            txtNombre.Enabled = activar;
            txtApellido.Enabled = activar;
            dateTimePicker1.Enabled = activar;
            cbEstados.Enabled = activar;
            linkContactos.Visible = activar;
            txtCalle.Enabled = activar;
            txtNro.Enabled = activar;
            txtPiso.Enabled = activar;
            txtDepto.Enabled = activar;
            txtBarrio.Enabled = activar;
            if (!activar)
            {
                cbLocalidad.Text = "";
                cbProvincia.Text = "";
            }
            cbLocalidad.Enabled = activar;
            cbProvincia.Enabled = activar;
            btnBuscar.Visible = activar;
            btnGuardar.Visible = activar;
            btnCancelar.Text = activar ? "Cancelar" : "Salir";

        }

        private void refrescarGrilla()
        {
            try
            {
                bindingContactos.Clear();
                bindingContactos.DataSource = _contactos;
                dataGridContactos.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void fechaInicial()
        {
            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker1.Value = DateTime.Today;
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
            if (_localidad != null)
            {
                try
                {
                    _provincia = _provincias.FirstOrDefault(p => p.IdProvincia == _localidad.IdProvincia);
                    cbProvincia.SelectedItem = _provincia;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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
            if (_provincias == null)
            {
                return;
            }

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
            if (_localidad == null)
            {
                return;
            }
            try
            {
                cbLocalidad.SelectedItem = _localidad;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void cargarEstados()
        {
            _estado = null;
            _estados = null;
            bindingEstados.Clear();
            string mensaje = "";
            _estados = EstadosNegocio.getEstadosPorIndole("Clientes", ref mensaje);
            if (!String.IsNullOrWhiteSpace(mensaje) || _estados == null)
            {
                cbEstados.Enabled = false;
                Console.WriteLine(mensaje);
                return;
            }
            bindingEstados.DataSource = _estados;
            if (modo == EnumModoForm.Alta || _cliente == null)
            {
                _estado = _estados.FirstOrDefault(e => e.Estado.Equals("Activo"));
                if (_estado == null)
                {
                    _estado = new();
                    _estado.Indole = "Clientes";
                    _estado.Estado = "Activo";
                }
                bindingEstados.DataSource = _estado;
            }
            else
            {
                _estado = _estados.FirstOrDefault(e => e.IdEstado == _cliente.IdEstado);
            }
            try
            {
                cbEstados.SelectedItem = _estado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cbEstados.Enabled = false;
                return;
            }
        }

        private void cargarContactos()
        {
            _contactos = null;
            refrescarGrilla();
            if (_cliente == null)
            {
                return;
            }
            if (_persona == null)
            {
                return;
            }
            string error = "";
            _contactos = ContactosNegocio.getContactosPorPersona(_persona, ref error);
            if (!String.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine(error);
                return;
            }
            if (_contactos == null)
            {
                return;
            }
            refrescarGrilla();
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
                if (_localidad.IdLocalidad != null)
                {
                    _domicilio.IdLocalidad = (int)_localidad.IdLocalidad;
                }
                else
                {
                    _domicilio.Localidades = _localidad;
                }
            }
            return true;
        }

        private void armarObjetoCliente()
        {
            if (modo == EnumModoForm.Alta || _cliente == null)
            {
                _cliente = new();
                _persona = new();
            }
            if (_persona == null)
            {
                _persona = new();
            }
            _persona.Dni = _dni;
            _persona.Nombres = _nombres;
            _persona.Apellidos = _apellidos;
            _persona.FechaNac = _nacimiento;
            _persona.Domicilios = _domicilio;
            _cliente.Personas = _persona;
            if (_estado.IdEstado != null)
            {
                _cliente.IdEstado = (int)_estado.IdEstado;
            }
        }

        private void limpiarValores(bool limpiarContactos)
        {
            _cliente = null;
            _persona = null;
            _estado = null;
            _domicilio = null;
            if (limpiarContactos)
                _contactos = new();
        }

        private void limpiarCampos()
        {
            refrescarGrilla();
            txtDni.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtCalle.Text = string.Empty;
            txtNro.Text = string.Empty;
            txtPiso.Text = string.Empty;
            txtDepto.Text = string.Empty;
            txtBarrio.Text = string.Empty;
            dateTimePicker1.Value = DateTime.Today;
        }

        private bool registrarCliente(ref string mensaje)
        {
            if (!comprobarCliente(ref mensaje, true))
            {
                return false;
            }
            if (!ingresarDomicilio(ref mensaje))
            {
                _domicilio = null;
            }
            armarObjetoCliente();
            if (!ClientesNegocio.RegistrarCliente(_cliente, _contactos, ref mensaje))
            {
                return false;
            }

            return true;
        }

        private bool cargarCliente()
        {
            if (_cliente == null)
            {
                return false;
            }
            if (_cliente.Personas == null)
            {
                return false;
            }
            if (_cliente.Estados != null)
                _estado = _cliente.Estados;
            _persona = _cliente.Personas;
            _domicilio = _persona.Domicilios;
            if (_domicilio != null)
            {
                _localidad = _domicilio.Localidades;
                if (_localidad != null)
                {
                    _provincia = _localidad.Provincias;
                }
            }
            return true;

        }

        private bool modificarCliente(ref string mensaje)
        {
            if (!comprobarCliente(ref mensaje, false))
            {
                return false;
            }
            if (!ingresarDomicilio(ref mensaje))
            {
                _domicilio = null;
            }
            armarObjetoCliente();
            if (!ClientesNegocio.modificarCliente(_cliente, _contactos, ref mensaje))
            {
                return false;
            }
            return true;
        }

        private void cerrarFormulario(object sender, EventArgs e)
        {
            try
            {
                cerrando = true;
                FrmMenuPrincipal frm = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
                if (frm != null)
                {
                    frm.AbrirAbmClientes(sender, e, modo);
                    Close();
                } 
                else
                {
                    return;
                }

            } 
            catch (Exception ex)
            {
                MessageBox.Show("Error fatal al cerrar\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        //Botones
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            cerrarFormulario(sender, e);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            limpiarValores(false);
            string mensaje = "";
            bool existe = buscarCliente(ref mensaje);
            if (!String.IsNullOrWhiteSpace(mensaje))
            {
                MessageBox.Show($"Error: {mensaje}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (modo == EnumModoForm.Alta)
            {
                if (existe)
                {
                    DialogResult res = MessageBox.Show($"Error!!! El Dni ingresado corresponde al cliente:\n{_cliente.NombreCompleto} \n¿Quiere modificar al cliente con ese Dni?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (res == DialogResult.No)
                        return;
                    else
                    {
                        DialogResult confirmacion = MessageBox.Show("¿Está Seguro?", "Verificación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirmacion == DialogResult.No)
                            return;
                        else
                        {
                            modo = EnumModoForm.Modificacion;
                            limpiarCampos();
                            cargarFormulario();
                            return;
                        }
                    }
                }
                if (!registrarCliente(ref mensaje))
                {
                    MessageBox.Show($"Ocurrió un error en el registro\n{mensaje}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string mensajeExito = "Registro exitoso";
                if (!String.IsNullOrWhiteSpace(mensaje))
                {
                    mensajeExito += $"\nDetalles de la operación{mensaje}";
                }
                MessageBox.Show(mensajeExito, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (!existe)
                {
                    DialogResult res = MessageBox.Show($"{mensaje}\n¿Desea registrar un cliente nuevo?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (res == DialogResult.Yes)
                    {
                        modo = EnumModoForm.Alta;
                        cargarFormulario();
                    }
                    else
                    {
                        btnGuardar.Enabled = false;
                    }
                    return;
                }
                if (!modificarCliente(ref mensaje))
                {
                    MessageBox.Show($"Error: {mensaje}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show($"Modificación exitosa, detalles: {mensaje}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cerrarFormulario(sender, e);
            }
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
                }
                else
                {
                    DialogResult res = MessageBox.Show($"{mensaje}\n¿Desea registrar un cliente nuevo?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (res == DialogResult.Yes)
                    {
                        modo = EnumModoForm.Alta;
                        cargarFormulario();
                        return;
                    }
                    else
                    {
                        btnGuardar.Enabled = false;
                    }

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
                    DialogResult res = MessageBox.Show($"Error!!! El Dni ingresado corresponde al cliente:\n{_cliente.NombreCompleto} \n¿Quiere modificar al cliente con ese Dni?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (res == DialogResult.No)
                        return;
                    else
                    {
                        DialogResult confirmacion = MessageBox.Show("¿Está Seguro?", "Verificación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirmacion == DialogResult.No)
                            return;
                        else
                        {
                            modo = EnumModoForm.Modificacion;
                            limpiarCampos();
                            cargarFormulario();
                            return;
                        }
                    }
                }
                return;
            }
        }

        private void linkContactos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EnumModoForm modoCont = EnumModoForm.Alta;
            editandoContactos = true;
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

            principal.AbrirFrmContactos(modoCont, "Clientes", _contactos, pnlContent);
            return;
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
