using Entidades_SGBM;
using Front_SGBM.UXDesign;
using Negocio_SGBM;
using System.Windows.Forms;

namespace Front_SGBM
{
    public partial class FrmAbmServicios : Form
    {
        public EnumModoForm modo = EnumModoForm.Consulta;
        private bool cerrando = false;
        private readonly List<string> campos = new List<string> { "Descripción", "Precio Venta", "Puntaje", "Duración", "Costo Total", "Nombre Costo" };
        private List<string>? criterios;
        private List<Servicios>? _servicios;
        private List<CostosServicios>? _costos;
        public Servicios? _servicioSeleccionado;
        private Categorias? _categoriaSeleccionada;
        private bool cargando = false;
        private bool buscarNumerico = false;

        public FrmAbmServicios()
        {
            InitializeComponent();
        }

        private void cerrarFormulario()
        {
            cerrando = true;
            Close();
        }

        private void FrmAbmServicios_Load(object sender, EventArgs e)
        {
            btnSeleccionar.Text = modo == EnumModoForm.Venta ? "Seleccionar" : "Modificar Servicio";
            cargarFormulario();
        }

        private void cargarFormulario()
        {
            cargarCategorias();
            cargarServicios();
            cargarCampos();
            cargarCategorias();
        }


        //Cargas de datos
        private void cargarCategorias()
        {
            cargando = true;
            string mensaje = string.Empty;
            try
            {
                _categoriaSeleccionada = null;
                cbCategorias.DataSource = null;
                List<Categorias>? categorias = CategoriasNegocio.ListarPorIndole("Servicios", ref mensaje);
                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.mensajeAdvertencia(mensaje);
                if (categorias == null)
                    categorias = new List<Categorias>();

                cbCategorias.DataSource = categorias;
                cbCategorias.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                cargando = false;
            }
            finally
            {
                cargando = false;
            }
        }

        private void cargarServicios()
        {
            string mensaje = string.Empty;
            try
            {
                cargando = true;
                _servicioSeleccionado = null;
                bindingSourceServicios.DataSource = null;
                List<Servicios>? lista;
                _servicios = ServiciosNegocio.Listar(ref mensaje);
                if (_servicios != null && !checkAnulados.Checked)
                    lista = _servicios;
                else
                    lista = _servicios.Where(s => s.activo == true).ToList();

                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.mensajeAdvertencia(mensaje);

                bindingSourceServicios.DataSource = lista;
                refrescarGrillaServicios();
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
            finally
            {
                cargando = false;
            }
        }

        private void refrescarGrillaServicios()
        {
            if (cargando)
                return;
            try
            {
                EstiloAplicacion.ApplyFormats(dataGridServicios, new Dictionary<string, string>
                {
                    { "costos", "C2" },
                    { "precioVenta", "C2" },
                    { "margen", "C2" },
                    { "comision", "P2" }
                });
                dataGridServicios.Refresh();
                dataGridServicios.ClearSelection();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void cargarCostos()
        {
            if (_servicioSeleccionado == null)
                return;
            if (_servicioSeleccionado.IdServicio == null)
                return;
            try
            {
                string mensaje = string.Empty;
                bindingSourceCostos.DataSource = null;
                _costos = CostosNegocio.ObtenerInsumosPorIdServicio((int)_servicioSeleccionado.IdServicio, ref mensaje);

                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.mensajeAdvertencia(mensaje);

                bindingSourceCostos.DataSource = _costos;
                refrescarGrillaCostos();
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                cargando = false;
            }
        }

        private void refrescarGrillaCostos()
        {
            if (_costos == null)
                return;
            try
            {
                EstiloAplicacion.ApplyCurrencyFormat(dataGridCostos, "costo");
                dataGridCostos.Refresh();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void dataGridServicios_SelectionChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            if (_servicios?.Count < 1)
                return;
            try
            {
                if (bindingSourceServicios.Current is Servicios servicio)
                {
                    _servicioSeleccionado = servicio;
                    cargarCostos();
                }
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                cargando = false;
            }
        }

        //Botones
        private void btnSalir_Click(object sender, EventArgs e)
        {
            bool confirmar = Mensajes.confirmarCierre();
            if (confirmar)
                cerrarFormulario();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            FrmMenuPrincipal padre = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (padre == null)
                return;
            try
            {
                padre.AbrirEditServicios(sender, e, EnumModoForm.Alta);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sobre el menu principal" + ex.Message, "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            FrmMenuPrincipal padre = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (padre == null)
                return;
            if (_servicioSeleccionado == null)
            {
                Mensajes.mensajeAdvertencia("Debe Seleccionar un servicio antes de continuar");
                return;
            }
            try
            {
                padre.AbrirEditServicios(sender, e, EnumModoForm.Consulta, _servicioSeleccionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sobre el menu principal" + ex.Message, "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            FrmMenuPrincipal padre = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (padre == null)
                return;
            if (_servicioSeleccionado == null)
            {
                Mensajes.mensajeAdvertencia("Debe Seleccionar un servicio antes de continuar");
                return;
            }
            try
            {
                if (modo != EnumModoForm.Venta)
                    padre.AbrirEditServicios(sender, e, EnumModoForm.Modificacion, _servicioSeleccionado);

                ///
                //Programar para la venta

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sobre el menu principal" + ex.Message, "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            buscarAvanzado();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cargarCampos();
            cbCategorias.SelectedIndex = -1;
            errorProvider1.Clear();
            cargarServicios();
        }

        //Carga de campos y criterios

        private void cargarCampos()
        {
            try
            {
                cbCampos.DataSource = campos;
                cbCampos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                cargando = false;
            }
        }

        private void cargarCriterios(string? campo)
        {
            cargando = true;
            try
            {
                criterios = null;
                if (string.IsNullOrWhiteSpace(campo))
                    criterios = new List<string>();
                else if (campo.Contains("descrip", StringComparison.OrdinalIgnoreCase) || campo.Contains("nombre", StringComparison.OrdinalIgnoreCase))
                {
                    criterios = new List<string> { "Contiene", "Comienza con", "Termina con", "Igual a", "No Contiene" };
                    buscarNumerico = false;
                }
                else
                {
                    criterios = new List<string> { "Mayor a", "Menor a ", "Igual a", "Mayor o Igual a", "Menor o Igual a" };
                    buscarNumerico = true;
                }

                if (cbCampos.SelectedIndex != -1)
                    setError(cbCampos, "");

                cbCriterios.DataSource = criterios;
                cbCriterios.SelectedIndex = -1;
                cbCriterios.Enabled = criterios.Count > 0;
                txtBusqueda.Clear();
                txtBusqueda.Enabled = false;
                btnBuscar.Enabled = false;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
            finally
            {
                cargando = false;
            }
        }

        //Combos
        private void cbCampos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            string campo = cbCampos.Text;
            try
            {
                cargarCriterios(campo);
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                cargando = false;
            }
        }

        private void cbCriterios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            try
            {
                string criterio = cbCriterios.Text;
                txtBusqueda.Enabled = !string.IsNullOrWhiteSpace(criterio);
                btnBuscar.Enabled = !string.IsNullOrWhiteSpace(criterio);
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                cargando = false;
            }
        }

        private void cbCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            try
            {
                if (cbCategorias.SelectedIndex >= 0)
                    _categoriaSeleccionada = (Categorias)cbCategorias.SelectedItem;
                else
                    _categoriaSeleccionada = null;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                cargando = false;
            }
        }

        private void setError(Control campo, string error)
        {
            errorProvider1.SetError(campo, error);
        }

        //Métodos de búsqueda y filtros
        private void filtroRapidoNombre(string nombre)
        {
            cargando = true;
            try
            {
                _costos = null;
                _servicioSeleccionado = null;
                bindingSourceServicios.DataSource = null;

                List<Servicios>? lista;

                if (nombre.Length < 3)
                {
                    lista = _servicios.Where(s => s.activo == true || s.activo == !checkAnulados.Checked).ToList();
                }
                else
                {
                    lista = _servicios.Where(s => s.NombreServicio.Contains(nombre, StringComparison.OrdinalIgnoreCase)
                            && (s.activo == true || s.activo == !checkAnulados.Checked)).ToList();
                }

                bindingSourceServicios.DataSource = lista;
                refrescarGrillaServicios();
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
            finally { cargando = false; }
        }

        private void txtFiltroRapido_TextChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            filtroRapidoNombre(txtFiltroRapido.Text);
        }

        private void buscarAvanzado()
        {
            string valorBuscado = txtBusqueda.Text.Trim();
            if (!validarBusqueda(valorBuscado))
                return;
            cargando = true;
            try
            {
                string mensaje = string.Empty;
                bindingSourceServicios.DataSource = null;
                _servicios = null;

                string campo = cbCampos.Text.Trim().ToLower();
                string criterio = cbCriterios.Text.Trim().ToLower();
                string valor = txtBusqueda.Text.Trim().ToLower();
                int idCategoria = _categoriaSeleccionada?.IdCategoria ?? 0;

                _servicios = ServiciosNegocio.BuscarAvanzado(campo, criterio, valor, idCategoria, ref mensaje);

                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.mensajeError("Error al buscar" + mensaje);

                filtroRapidoNombre(txtFiltroRapido.Text);

            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
            finally { cargando = false; }
        }

        private bool validarBusqueda(string valorBuscado)
        {
            string mensaje = string.Empty;
            try
            {
                if (cbCategorias.SelectedIndex < 0 || cbCategorias.Text.Equals("Sin Categoría"))
                    _categoriaSeleccionada = null;
                else
                    _categoriaSeleccionada = (Categorias)cbCategorias.SelectedItem;

                if (cbCampos.SelectedIndex < 0)
                {
                    Mensajes.mensajeError("Falta seleccionar el campo a buscar");
                    setError(cbCampos, "Seleccionar un campo");
                    return false;
                }

                if (cbCriterios.SelectedIndex < 0)
                {
                    Mensajes.mensajeError("Falta seleccionar el criterio de búsqueda");
                    setError(cbCriterios, "Seleccionar un criterio");
                    return false;
                }

                if (string.IsNullOrEmpty(valorBuscado))
                {
                    Mensajes.mensajeError("Ingrese el valor a buscar");
                    setError(txtBusqueda, "Ingrese un valor");
                    return false;
                }

                if (buscarNumerico && !Validaciones.esNumeroDecimal(valorBuscado, ref mensaje))
                {
                    Mensajes.mensajeError("Error en el valor buscado\n" + mensaje);
                    setError(txtBusqueda, "Formato Incorrecto");
                    return false;
                }

                errorProvider1.Clear();
                return true;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return false;
            }
        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!buscarNumerico)
                return;
            e.Handled = !Validaciones.esDigitoDecimal(e.KeyChar);
        }

        private void checkAnulados_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            try
            {
                filtroRapidoNombre(txtFiltroRapido.Text);
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return;
            }
        }
    }
}
