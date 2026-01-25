
using Entidades_SGBM;
using Front_SGBM.UXDesign;
using Negocio_SGBM;
using System.Windows.Forms;

namespace Front_SGBM
{
    public partial class FrmAbmProductos : Form
    {
        #region Declaraciones Iniciales
        private bool cerrando = false;
        public EnumModoForm modo = EnumModoForm.Consulta;
        private bool cargando = false;
        private Productos? _productoSeleccionado;
        private List<Productos>? _productos;
        private UnidadesMedidas? _medidaSeleccionada;
        private List<UnidadesMedidas>? _unidadesMedidas;
        private Categorias? _categoriaSeleccionada = new Categorias { IdCategoria = 0};
        private List<Categorias>? _categorias;
        #endregion

        public FrmAbmProductos()
        {
            InitializeComponent();
        }
        
        private void FrmAbmProductos_Load(object sender, EventArgs e)
        {
            cargarProductos();
            cargarCategorias();
            cargarUnidadesDeMedida();
            sugerirCodigo();
        }

        #region Cargas de Listados
        private void cargarProductos()
        {
            _productos = null;
            string mensaje = string.Empty;
            try
            {
                _productos = ProductosNegocio.ListaSimple(ref mensaje);
                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.mensajeAdvertencia(mensaje);
                refrescarGrilla();
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }

        private void refrescarGrilla(string filtro = "")
        {
            cargando = true;
            try
            {
                bindingSourceProductos.DataSource = null;
                int stock = checkSinStock.Checked ? int.MinValue : 0;
                int? idCategoria = _categoriaSeleccionada?.IdCategoria;

                List<Productos>? lista;
                if (_productos == null)
                    lista = new List<Productos>();
                else
                {
                    lista = _productos.Where(p => ((p.CantidadMedida ?? 0) > stock || p.Stock > stock
                                && (p.Activo || p.Activo != checkActivo.Checked)) && p.Descripcion.Contains(filtro)
                                && (p.idCategoria == idCategoria || idCategoria == 0)).OrderBy(p => p.Descripcion).ToList();
                }

                bindingSourceProductos.DataSource = lista;
                dataGridProductos.Refresh();
                dataGridProductos.ClearSelection();
                limpiarSeleccion();
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

        private void cargarUnidadesDeMedida()
        {
            cargando = true;
            string mensaje = string.Empty;
            try
            {
                bindingSourceMedidas.DataSource = null;
                _unidadesMedidas = ProductosNegocio.ListarUnidadesMedida(ref mensaje);
                bindingSourceMedidas.DataSource = _unidadesMedidas;
                cbUdMedida.SelectedIndex = -1;
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

        private void cargarCategorias()
        {
            string mensaje = string.Empty;
            cargando = true;
            try
            {
                _categorias = null;
                bindingSourceCategorias.DataSource = null;
                _categorias = CategoriasNegocio.ListarPorIndole("Categorias", ref mensaje);
                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.mensajeAdvertencia(mensaje);
                cbEditCategoria.DataSource = _categorias;
                cbEditCategoria.SelectedIndex = -1;
                cargarComboCatFiltro();
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

        private void cargarComboCatFiltro()
        {
            try
            {
                List<Categorias>? lista = new();
                lista.Add(new Categorias { Descripcion = "Todas las Categorías", Indole = "Categorias", IdCategoria = 0 });
                lista.Add(new Categorias { Descripcion = "Sin Categoría", Indole = "Categorias", IdCategoria = null });
                cbCategoríaAbm.DataSource = null;
                if (_categorias != null)
                    lista.AddRange(_categorias);
                cbCategoríaAbm.DataSource = lista;
                cbCategoríaAbm.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }
        #endregion

        #region Eventos de formulario
        #region Botones
        private void btnSalir_Click(object sender, EventArgs e)
        {
            cerrarFormulario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarSeleccion();
            activarControlesEdit(true);
            activarCamposMedida(true);
        }

        private void btnBaja_Click(object sender, EventArgs e)
        {

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #region Controles seleccionables
        private void cbCategoríaAbm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            try
            {
                _categoriaSeleccionada = cbCategoríaAbm.SelectedItem as Categorias;
                refrescarGrilla(txtFiltro.Text.Trim());
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }

        private void checkSinStock_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            refrescarGrilla(txtFiltro.Text.Trim());
        }

        private void checkAnulados_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            refrescarGrilla(txtFiltro.Text.Trim());
        }

        private void dataGridProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            try
            {
                limpiarSeleccion(false);

                if (bindingSourceProductos.Count > 0)
                    _productoSeleccionado = bindingSourceProductos.Current as Productos;
                
                bool nulo = _productoSeleccionado == null;
                if (!nulo)
                {
                    cargarCamposProducto();
                }
                activarControlesEdit(nulo);
                activarCamposMedida(nulo);
                btnModificar.Enabled = !nulo;
                btnBaja.Enabled = !nulo;
                activarOAnular();
                btnGuardar.Enabled = false;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }

        }
        #endregion
        #region Controles de texto
        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            try
            {
                string texto = txtFiltro.Text.Trim();
                if (texto.Length > 0 && texto.Length < 3)
                    return;
                refrescarGrilla(texto);
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }

        private void txtNumerico_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cerrando)
                return;
            e.Handled = !Validaciones.esDigitoNumerico(e.KeyChar);
        }

        private void txtDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cerrando)
                return;
            e.Handled = !Validaciones.esDigitoDecimal(e.KeyChar);
        }

        #endregion
        #endregion

        #region Métodos

        #region Interacción con Campos
        private void activarOAnular()
        {
            bool activo = _productoSeleccionado?.Activo ?? true;
            if (activo)
            {
                btnBaja.Tag = "btnNormalR";
                btnBaja.Text = "Activar";
            } else
            {
                btnBaja.Tag = "btnNormalV";
                btnBaja.Text = "Anular";
            }
            EstiloAplicacion.StyleButton(btnBaja);
        }

        private void limpiarSeleccion(bool nuevo = true)
        {
            cargando = true;
            _productoSeleccionado = null;
            txtCodigo.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            checkActivo.Checked = true;
            txtPrecioVenta.Text = string.Empty;
            txtComision.Text = string.Empty;
            txtCosto.Text = string.Empty;
            txtMargen.Text = string.Empty;
            txtStockUds.Text = string.Empty;
            cbUdMedida.SelectedIndex = -1;
            txtCantMedida.Text = string.Empty;
            txtMedida.Text = string.Empty;
            cbEditCategoria.SelectedIndex = -1;
            btnGuardar.Enabled = false;
            if (nuevo)
                sugerirCodigo();
            cargando = false;
        }

        private void activarControlesEdit(bool activo)
        {
            txtCodigo.Enabled = activo;
            txtDescripcion.Enabled = activo;
            checkActivo.Enabled = activo;
            txtPrecioVenta.Enabled = activo;
            txtComision.Enabled = activo;
            txtCosto.Enabled = activo;
            txtStockUds.Enabled = activo;
            cbUdMedida.Enabled = activo;
            cbEditCategoria.Enabled = activo;
            btnGuardar.Enabled = !activo;
            btnNuevo.Enabled = activo && _productoSeleccionado != null;
            btnModificar.Enabled = !activo;
            btnBaja.Enabled = !activo;
        }

        private void activarCamposMedida(bool activo)
        {
            txtMedida.Enabled = activo;
            txtCantMedida.Enabled = activo;
        }

        private void cargarCamposProducto()
        {
            if (_productoSeleccionado == null)
                return;
            try
            {
                txtCodigo.Text = _productoSeleccionado.CodProducto;
                txtDescripcion.Text = _productoSeleccionado.Descripcion;
                checkActivo.Checked = _productoSeleccionado.Activo;
                txtPrecioVenta.Text = _productoSeleccionado.PrecioVenta.ToString("0.00");
                txtComision.Text = _productoSeleccionado.Comision?.ToString("0.00") ?? "";
                txtCosto.Text = _productoSeleccionado.Costo.ToString("0.00");
                decimal margen = _productoSeleccionado.PrecioVenta * (1 - (_productoSeleccionado.Comision ?? 0)) - _productoSeleccionado.Costo;
                txtMargen.Text = margen.ToString("0.00");
                txtStockUds.Text = _productoSeleccionado.Stock.ToString();
                _medidaSeleccionada = _productoSeleccionado.UnidadesMedidas;
                if (_medidaSeleccionada == null)
                    cbUdMedida.SelectedIndex = -1;
                else
                    cbUdMedida.SelectedValue = _medidaSeleccionada.IdUnidadMedida;

                txtCantMedida.Text = _productoSeleccionado.CantidadMedida?.ToString("0.00");
                txtMedida.Text = _productoSeleccionado.Medida?.ToString();

                Categorias? cat = _productoSeleccionado.Categorias;
                if (cat != null)
                    cbEditCategoria.SelectedValue = cat.IdCategoria;
                else
                    cbEditCategoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }
        #endregion

        #region Validaciones
        private bool validarDescripcion(TextBox descripcion, ref string mensaje)
        {
            mensaje = string.Empty;
            string texto = descripcion.Text.Trim();
            if (!Validaciones.textoCorrecto(texto, ref mensaje))
            {
                errorProvider1.SetError(descripcion, mensaje);
                return false;
            }
            else
            {
                errorProvider1.SetError(descripcion, "");
                descripcion.Text = Validaciones.capitalizarTexto(texto, true);
                return true;
            }
        }

        private bool validarCampoNumerico(TextBox campo, bool esDecimal, bool obligatorio , ref string mensaje)
        {
            mensaje = string.Empty;
            string texto = campo.Text.Trim();
            try
            {
                decimal numero = 0;
                if (!Validaciones.esNumeroDecimal(texto, ref mensaje, obligatorio))
                {
                    errorProvider1.SetError(campo, mensaje);
                    return false;
                }
                numero = decimal.Parse(texto);
                campo.Text = esDecimal ? numero.ToString("0.00") : ((int)numero).ToString();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool codigoExiste(ref string mensaje)
        {
            string codigo = txtCodigo.Text.Trim();
            try
            {
                Productos? existente = ProductosNegocio.BuscarPorCodigo(codigo, ref mensaje);
                if (existente != null)
                {
                    Mensajes.mensajeError($"El código {codigo} ya pertenece al producto {existente.Descripcion}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Acciones

        private void sugerirCodigo()
        {
            string mensaje = string.Empty;
            txtCodigo.Text = ProductosNegocio.SugerirCodigo(ref mensaje);
            if (!string.IsNullOrWhiteSpace(mensaje))
                Mensajes.mensajeError(mensaje);
        }

        #endregion

        private void cerrarFormulario()
        {
            bool confirmar = Mensajes.confirmarCierre();
            if (!confirmar)
            {
                cerrando = false;
                return;
            }
            cerrando = true;
            this.Close();
        }

        #endregion
    }
}
