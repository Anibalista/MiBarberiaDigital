using Entidades_SGBM;
using Front_SGBM.UXDesign;
using Negocio_SGBM;

namespace Front_SGBM
{
    public partial class FrmAbmProductos : Form
    {
        #region Declaraciones Iniciales

        // Indica si el formulario se está cerrando (para evitar procesos redundantes).
        private bool cerrando = false;

        // Modo actual del formulario (Alta, Modificación, Consulta, etc.).
        public EnumModoForm modo = EnumModoForm.Alta;

        // Flag que indica si el formulario está en proceso de carga de datos.
        private bool cargando = false;

        // Producto actualmente seleccionado en la grilla o formulario.
        private Productos? _productoSeleccionado;

        // Lista completa de productos cargados desde la base de datos.
        private List<Productos>? _productos;

        // Unidad de medida actualmente seleccionada.
        private UnidadesMedidas? _medidaSeleccionada;

        // Lista de unidades de medida disponibles.
        private List<UnidadesMedidas>? _unidadesMedidas;

        // Categoría actualmente seleccionada (por defecto se inicializa con Id = 0).
        private Categorias? _categoriaSeleccionada = new Categorias { IdCategoria = 0 };

        // Lista de categorías disponibles.
        private List<Categorias>? _categorias;

        /// <summary>
        /// Constructor del formulario de ABM de Productos.
        /// Inicializa los componentes gráficos.
        /// </summary>
        public FrmAbmProductos()
        {
            InitializeComponent(); // Método generado por el diseñador de Windows Forms.
        }

        /// <summary>
        /// Evento que se ejecuta al cargar el formulario.
        /// Se encarga de inicializar datos: productos, categorías, unidades de medida y sugerir código.
        /// </summary>
        private void FrmAbmProductos_Load(object sender, EventArgs e)
        {
            cargarProductos();          // Carga la lista de productos desde la base de datos.
            cargarCategorias();         // Carga las categorías disponibles.
            cargarUnidadesDeMedida();   // Carga las unidades de medida disponibles.
            sugerirCodigo();            // Sugiere un código para un nuevo producto.
        }

        #endregion

        #region Cargas de Listados

        /// <summary>
        /// Carga la lista de productos desde la capa de negocio.
        /// </summary>
        private void cargarProductos()
        {
            _productos = null; // Se inicializa la lista en null
            string mensaje = string.Empty;
            try
            {
                // Se obtiene la lista de productos
                _productos = ProductosNegocio.ListaSimple(ref mensaje);

                // Si hubo algún mensaje de advertencia, se muestra
                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.mensajeAdvertencia(mensaje);

                // Refresca la grilla con los productos cargados
                refrescarGrilla();
            }
            catch (Exception ex)
            {
                // Manejo de errores inesperados
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }

        /// <summary>
        /// Refresca la grilla de productos aplicando filtros de stock, estado y categoría.
        /// </summary>
        /// <param name="filtro">Texto de búsqueda para filtrar por descripción.</param>
        private void refrescarGrilla(string filtro = "")
        {
            cargando = true; // Indica que se está cargando la grilla
            try
            {
                bindingSourceProductos.DataSource = null;

                // Si se selecciona "sin stock", se usa int.MinValue para incluir todos
                int stock = checkSinStock.Checked ? int.MinValue : 0;

                // Se obtiene la categoría seleccionada
                int? idCategoria = _categoriaSeleccionada?.IdCategoria;

                List<Productos>? lista;

                // Si no hay productos cargados, se inicializa lista vacía
                if (_productos == null)
                    lista = new List<Productos>();
                else
                {
                    // Se filtra la lista de productos según stock, estado, descripción y categoría
                    lista = _productos.Where(p => ((p.CantidadMedida ?? 0) > stock || p.Stock > stock)
                                && ((p.Activo || p.Activo != checkAnulados.Checked))
                                && p.Descripcion.Contains(filtro)
                                && (p.idCategoria == idCategoria || idCategoria == 0))
                                .OrderBy(p => p.Descripcion)
                                .ToList();
                }

                // Se asigna la lista filtrada al binding source
                bindingSourceProductos.DataSource = lista;

                // Refresca la grilla y limpia selección
                dataGridProductos.Refresh();
                dataGridProductos.ClearSelection();

                // Si el modo no es Alta, se reinicia la selección
                if (modo != EnumModoForm.Alta)
                {
                    limpiarSeleccion();
                    modo = EnumModoForm.Alta;
                }

                // Activa o anula controles según estado
                activarOAnular();

                // === Aplicación de formatos a columnas ===
                var formatos = new Dictionary<string, string>
                {
                    { "precioVenta", "C2" }, // Moneda con 2 decimales
                    { "costo", "C2" },       // Moneda con 2 decimales
                    { "Comision", "P2" },    // Porcentaje con 2 decimales
                    { "medida", "N2" }       // Número decimal con 2 decimales
                };

                EstiloAplicacion.ApplyFormats(dataGridProductos, formatos);
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
            finally
            {
                cargando = false; // Finaliza la carga
            }
        }

        /// <summary>
        /// Carga las unidades de medida desde la capa de negocio.
        /// </summary>
        private void cargarUnidadesDeMedida()
        {
            cargando = true;
            string mensaje = string.Empty;
            try
            {
                bindingSourceMedidas.DataSource = null;

                // Se obtiene la lista de unidades de medida
                _unidadesMedidas = ProductosNegocio.ListarUnidadesMedida(ref mensaje);

                // Se asigna al binding source
                bindingSourceMedidas.DataSource = _unidadesMedidas;

                // Se limpia la selección del combo
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

        /// <summary>
        /// Carga las categorías disponibles desde la capa de negocio.
        /// </summary>
        private void cargarCategorias()
        {
            string mensaje = string.Empty;
            cargando = true;
            try
            {
                _categorias = null;
                bindingSourceCategorias.DataSource = null;

                // Se obtiene la lista de categorías para productos
                _categorias = CategoriasNegocio.ListarPorIndole("Productos", ref mensaje);

                // Si hay advertencias, se muestran
                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.mensajeAdvertencia(mensaje);

                // Se asigna al combo de edición
                cbEditCategoria.DataSource = _categorias;
                cbEditCategoria.SelectedIndex = -1;

                // Carga el combo de filtro de categorías
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

        /// <summary>
        /// Carga el combo de categorías para filtro, incluyendo opciones especiales.
        /// </summary>
        private void cargarComboCatFiltro()
        {
            try
            {
                // Se inicializa la lista con opciones especiales
                List<Categorias>? lista = new();
                lista.Add(new Categorias { Descripcion = "Todas las Categorías", Indole = "Categorias", IdCategoria = 0 });
                lista.Add(new Categorias { Descripcion = "Sin Categoría", Indole = "Categorias", IdCategoria = null });

                cbCategoríaAbm.DataSource = null;

                // Se agregan las categorías existentes
                if (_categorias != null)
                    lista.AddRange(_categorias);

                // Se asigna al combo y se selecciona la primera opción
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

        /// <summary>
        /// Evento click del botón Salir.
        /// Cierra el formulario actual.
        /// </summary>
        private void btnSalir_Click(object sender, EventArgs e)
        {
            cerrarFormulario();
        }

        /// <summary>
        /// Evento click del botón Guardar.
        /// Valida y modifica un producto existente si el modo es Modificación.
        /// </summary>
        /// <remarks>
        /// - Verifica que el formulario esté en modo Modificación.
        /// - Valida los datos del producto.
        /// - Si la validación es correcta, intenta modificar el producto.
        /// - Muestra mensajes de éxito o error según el resultado.
        /// </remarks>
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            if (modo != EnumModoForm.Modificacion)
            {
                Mensajes.mensajeError("Modo incorrecto en el formulario");
                return;
            }
            if (validarProducto(ref mensaje))
            {
                if (modificarProducto(ref mensaje))
                {
                    Mensajes.mensajeExito("Modificación exitosa");
                    cargarProductos();
                    return;
                }
            }
            Mensajes.mensajeError("Modificación Fallida (Verifique los campos con error)" + mensaje);
        }

        /// <summary>
        /// Evento click del botón Nuevo.
        /// Registra un nuevo producto si el modo es Alta.
        /// </summary>
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            if (modo != EnumModoForm.Alta)
            {
                Mensajes.mensajeError("Modo incorrecto en el formulario");
                return;
            }
            if (validarProducto(ref mensaje))
            {
                if (registrarProducto(ref mensaje))
                {
                    Mensajes.mensajeExito("Registro exitoso");
                    cargarProductos();
                    return;
                }
            }
            Mensajes.mensajeError("Registro Fallido (Verifique los campos con error)" + mensaje);
        }

        /// <summary>
        /// Evento click del botón Limpiar.
        /// Limpia la selección actual y prepara el formulario para un nuevo producto.
        /// </summary>
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarSeleccion();
            modo = EnumModoForm.Alta;
            activarControlesEdit();
            activarCamposMedida(true);
        }

        /// <summary>
        /// Evento click del botón Baja.
        /// Activa o anula el producto seleccionado según su estado actual.
        /// </summary>
        private void btnBaja_Click(object sender, EventArgs e)
        {
            string accion = btnBaja.Text;
            if (_productoSeleccionado == null)
            {
                Mensajes.mensajeError($"Seleccione un producto para {accion} antes de continuar");
                return;
            }

            string pregunta = $"¿Desea {accion} el producto {_productoSeleccionado.Descripcion}?";
            DialogResult respuesta = Mensajes.respuesta(pregunta);

            if (respuesta == DialogResult.No)
                return;

            pregunta = string.Empty;
            accion = accion.Substring(0, accion.Length - 2) + "do";

            if (!ProductosNegocio.CambiarEstadoProducto(_productoSeleccionado, ref pregunta))
                Mensajes.mensajeError(pregunta);
            else
                Mensajes.mensajeExito($"¡Producto {accion} exitosamente!");

            cargarProductos();
        }

        /// <summary>
        /// Evento click del botón Modificar.
        /// Habilita la edición de un producto seleccionado.
        /// </summary>
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null)
            {
                Mensajes.mensajeError("Seleccione un producto a modificar");
                return;
            }
            modo = EnumModoForm.Modificacion;
            activarControlesEdit();
            activarCamposMedida(cbUdMedida.SelectedIndex >= 0);
        }

        #endregion

        #region Controles seleccionables

        /// <summary>
        /// Evento cambio de selección en combo de Unidad de Medida.
        /// Activa o desactiva los campos de medida según la selección.
        /// </summary>
        private void cbUdMedida_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            activarCamposMedida(cbUdMedida.SelectedIndex != -1);
        }

        /// <summary>
        /// Evento cambio de selección en combo de Categoría.
        /// Actualiza la grilla de productos según la categoría seleccionada.
        /// </summary>
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

        /// <summary>
        /// Evento cambio de estado en check de Sin Stock.
        /// Refresca la grilla aplicando el filtro correspondiente.
        /// </summary>
        private void checkSinStock_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            refrescarGrilla(txtFiltro.Text.Trim());
        }

        /// <summary>
        /// Evento cambio de estado en check de Anulados.
        /// Refresca la grilla aplicando el filtro correspondiente.
        /// </summary>
        private void checkAnulados_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            refrescarGrilla(txtFiltro.Text.Trim());
        }

        /// <summary>
        /// Evento cambio de selección en la grilla de productos.
        /// Carga los datos del producto seleccionado en el formulario.
        /// </summary>
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
                    modo = EnumModoForm.Consulta;
                }
                else
                {
                    modo = EnumModoForm.Alta;
                }
                activarControlesEdit();
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

        /// <summary>
        /// Evento cambio de texto en filtro.
        /// Refresca la grilla según el texto ingresado.
        /// </summary>
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

        /// <summary>
        /// Evento KeyPress para campos numéricos.
        /// Permite solo dígitos numéricos.
        /// </summary>
        private void txtNumerico_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cerrando)
                return;
            e.Handled = !Validaciones.esDigitoNumerico(e.KeyChar);
        }

        /// <summary>
        /// Evento KeyPress para campos decimales.
        /// Permite solo dígitos decimales.
        /// </summary>
        private void txtDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cerrando)
                return;
            e.Handled = !Validaciones.esDigitoDecimal(e.KeyChar);
        }

        /// <summary>
        /// Evento cambio de texto en Precio de Venta.
        /// Recalcula el margen automáticamente.
        /// </summary>
        private void txtPrecioVenta_TextChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            try
            {
                decimal margen = calcularMargen(txtPrecioVenta.Text, txtCosto.Text, txtComision.Text);
                txtMargen.Text = margen == 0 ? "" : margen.ToString("0.00");
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }

        /// <summary>
        /// Evento cambio de texto en Comisión.
        /// Recalcula el margen automáticamente.
        /// </summary>
        private void txtComision_TextChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            try
            {
                decimal margen = calcularMargen(txtPrecioVenta.Text, txtCosto.Text, txtComision.Text);
                txtMargen.Text = margen == 0 ? "" : margen.ToString("0.00");
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }

        /// <summary>
        /// Evento cambio de texto en Costo.
        /// Recalcula el margen automáticamente.
        /// </summary>
        private void txtCosto_TextChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            try
            {
                decimal margen = calcularMargen(txtPrecioVenta.Text, txtCosto.Text, txtComision.Text);
                txtMargen.Text = margen == 0 ? "" : margen.ToString("0.00");
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region Métodos

        #region Interacción con Campos

        /// <summary>
        /// Cambia el estado del botón de baja según si el producto está activo o no.
        /// </summary>
        private void activarOAnular()
        {
            // Verifica si el producto seleccionado está activo (si no hay producto, se asume false).
            bool activo = _productoSeleccionado?.Activo ?? false;

            if (!activo)
            {
                // Si no está activo, el botón se configura para "Activar".
                btnBaja.Tag = "btnNormalV";
                btnBaja.Text = "Activar";
            }
            else
            {
                // Si está activo, el botón se configura para "Anular".
                btnBaja.Tag = "btnNormalR";
                btnBaja.Text = "Anular";
            }

            // Aplica estilo visual al botón según la configuración.
            EstiloAplicacion.StyleButton(btnBaja);
        }

        /// <summary>
        /// Limpia los campos de edición y resetea la selección de producto.
        /// </summary>
        /// <param name="nuevo">Indica si se debe sugerir un nuevo código al limpiar.</param>
        private void limpiarSeleccion(bool nuevo = true)
        {
            cargando = true; // Flag para evitar eventos mientras se limpian los campos.

            _productoSeleccionado = null; // Se elimina la referencia al producto actual.

            // Se limpian todos los campos de texto y controles.
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

            // Se deshabilita el botón Guardar hasta que haya datos válidos.
            btnGuardar.Enabled = false;

            // Si es un nuevo producto, se sugiere un código automáticamente.
            if (nuevo)
                sugerirCodigo();

            // Limpia cualquier error previo en el formulario.
            errorProvider1.Clear();

            cargando = false; // Se habilitan nuevamente los eventos.
        }

        /// <summary>
        /// Activa o desactiva los controles de edición según el modo del formulario.
        /// </summary>
        private void activarControlesEdit()
        {
            // Se habilitan controles si el modo no es "Consulta".
            bool activo = modo != EnumModoForm.Consulta;

            txtCodigo.Enabled = activo;
            txtDescripcion.Enabled = activo;
            checkActivo.Enabled = activo;
            txtPrecioVenta.Enabled = activo;
            txtComision.Enabled = activo;
            txtCosto.Enabled = activo;
            txtStockUds.Enabled = activo;
            cbUdMedida.Enabled = activo;
            cbEditCategoria.Enabled = activo;

            // Si no hay categoría seleccionada, se limpia el texto.
            if (cbEditCategoria.SelectedIndex < 0)
                cbEditCategoria.Text = "";

            // Configuración de botones según el modo.
            btnGuardar.Enabled = activo;
            btnNuevo.Enabled = modo == EnumModoForm.Alta;
            btnModificar.Enabled = !activo;
            btnBaja.Enabled = !activo;
        }

        /// <summary>
        /// Activa o desactiva los campos relacionados con medidas.
        /// </summary>
        /// <param name="activo">True para habilitar, False para deshabilitar.</param>
        private void activarCamposMedida(bool activo)
        {
            txtMedida.Enabled = activo;
            txtCantMedida.Enabled = activo;
        }

        /// <summary>
        /// Carga los datos del producto seleccionado en los campos del formulario.
        /// </summary>
        private void cargarCamposProducto()
        {
            if (_productoSeleccionado == null)
                return;

            try
            {
                // Asignación de valores básicos.
                txtCodigo.Text = _productoSeleccionado.CodProducto;
                txtDescripcion.Text = _productoSeleccionado.Descripcion;
                checkActivo.Checked = _productoSeleccionado.Activo;
                txtPrecioVenta.Text = _productoSeleccionado.PrecioVenta.ToString("0.00");

                // Conversión de comisión a porcentaje.
                decimal? comision = null;
                if (_productoSeleccionado.Comision != null)
                    comision = _productoSeleccionado.Comision * 100;

                txtComision.Text = comision?.ToString("0.00") ?? "";

                // Costo y margen calculado.
                txtCosto.Text = _productoSeleccionado.Costo.ToString("0.00");
                decimal margen = _productoSeleccionado.PrecioVenta * (1 - (_productoSeleccionado.Comision ?? 0)) - _productoSeleccionado.Costo;
                txtMargen.Text = margen.ToString("0.00");

                // Stock disponible.
                txtStockUds.Text = _productoSeleccionado.Stock.ToString();

                // Unidad de medida seleccionada.
                _medidaSeleccionada = _productoSeleccionado.UnidadesMedidas;
                if (_medidaSeleccionada == null)
                    cbUdMedida.SelectedIndex = -1;
                else
                    cbUdMedida.SelectedValue = _medidaSeleccionada.IdUnidadMedida;

                // Cantidad y medida.
                txtCantMedida.Text = _productoSeleccionado.CantidadMedida?.ToString("0.00");
                txtMedida.Text = _productoSeleccionado.Medida?.ToString();

                // Categoría del producto.
                Categorias? cat = _productoSeleccionado.Categorias;
                if (cat != null)
                    cbEditCategoria.SelectedValue = cat.IdCategoria;
                else
                    cbEditCategoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Manejo de errores inesperados.
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
            }
        }

        #endregion

        #region Validaciones

        /// <summary>
        /// Valida que el texto ingresado en un control sea correcto.
        /// Aplica capitalización y muestra errores en el ErrorProvider.
        /// </summary>
        private bool validarTexto(Control controlTexto, ref string mensaje)
        {
            mensaje = string.Empty;
            string texto = controlTexto.Text.Trim();

            if (!Validaciones.textoCorrecto(texto, ref mensaje))
            {
                // Si el texto no es válido, se muestra el error
                errorProvider1.SetError(controlTexto, mensaje);
                return false;
            }
            else
            {
                // Limpia errores y capitaliza el texto
                errorProvider1.SetError(controlTexto, "");
                controlTexto.Text = Validaciones.capitalizarTexto(texto, true);
                return true;
            }
        }

        /// <summary>
        /// Valida que un campo numérico contenga un valor correcto.
        /// </summary>
        private decimal? validarCampoNumerico(TextBox campo, bool esDecimal, bool obligatorio, ref string mensaje)
        {
            mensaje = string.Empty;
            string texto = campo.Text.Trim();
            try
            {
                decimal numero = 0;

                if (!Validaciones.esNumeroDecimal(texto, ref mensaje, obligatorio))
                {
                    errorProvider1.SetError(campo, mensaje);
                    return null;
                }
                else
                    errorProvider1.SetError(campo, "");

                numero = decimal.Parse(texto);
                campo.Text = esDecimal ? numero.ToString("0.00") : ((int)numero).ToString();

                return numero;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Verifica si el código ingresado ya existe en otro producto.
        /// </summary>
        private bool codigoExiste(ref string mensaje)
        {
            string codigo = txtCodigo.Text.Trim();
            try
            {
                Productos? existente = ProductosNegocio.BuscarPorCodigo(codigo, ref mensaje);

                if (existente != null)
                {
                    mensaje = $"El código {codigo} ya pertenece al producto {existente.Descripcion}";
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Valida todos los campos del producto antes de registrar o modificar.
        /// </summary>
        private bool validarProducto(ref string mensaje)
        {
            mensaje = string.Empty;

            if (_productoSeleccionado == null)
                _productoSeleccionado = new();

            try
            {
                bool valido = true;

                // Validación del código
                decimal? codigo = validarCampoNumerico(txtCodigo, false, true, ref mensaje);
                mensaje = string.Empty;

                if (codigo != null)
                {
                    bool existe = codigoExiste(ref mensaje);
                    if (existe)
                    {
                        if (modo == EnumModoForm.Alta)
                            valido = false;
                        else if (txtCodigo.Text.Trim() != _productoSeleccionado.CodProducto)
                            valido = false;
                    }

                    if (valido)
                        _productoSeleccionado.CodProducto = txtCodigo.Text.Trim();
                    else
                        Mensajes.mensajeError(mensaje);

                    errorProvider1.SetError(txtCodigo, mensaje);
                }

                // Validación de descripción
                mensaje = string.Empty;
                if (validarTexto(txtDescripcion, ref mensaje))
                    _productoSeleccionado.Descripcion = txtDescripcion.Text;
                else
                    valido = false;

                // Estado activo
                _productoSeleccionado.Activo = checkActivo.Checked;

                // Precio de venta
                mensaje = string.Empty;
                decimal? numero = validarCampoNumerico(txtPrecioVenta, true, true, ref mensaje);
                if (numero.HasValue)
                    _productoSeleccionado.PrecioVenta = numero ?? 0;
                else
                    valido = false;

                // Costo
                mensaje = string.Empty;
                numero = validarCampoNumerico(txtCosto, true, false, ref mensaje);
                if (numero.HasValue)
                {
                    _productoSeleccionado.Costo = numero ?? 0;
                    valido = valido ? numero > 0 : false;
                }

                // Comisión
                mensaje = string.Empty;
                _productoSeleccionado.Comision = validarCampoNumerico(txtComision, true, false, ref mensaje);
                if (_productoSeleccionado.Comision != null)
                {
                    if (_productoSeleccionado.Comision > 100 || _productoSeleccionado.Comision < 0)
                    {
                        errorProvider1.SetError(txtComision, $"El porcentaje {numero}% no es una comisión válida");
                        valido = false;
                    }
                    _productoSeleccionado.Comision = _productoSeleccionado.Comision != 0 ? _productoSeleccionado.Comision / 100 : _productoSeleccionado.Comision;
                }

                // Stock
                mensaje = string.Empty;
                numero = validarCampoNumerico(txtStockUds, false, true, ref mensaje);
                if (numero != null)
                    _productoSeleccionado.Stock = (int)numero;
                else
                    valido = false;

                // Categoría
                if (!validarCategoria())
                    valido = false;

                // Dosificación
                if (!validarDosificacion())
                {
                    DialogResult respuesta = Mensajes.respuesta("La dosificación se anuló por errores en los campos\n¿Desea continuar igualmente?");
                    if (respuesta == DialogResult.No)
                        valido = false;
                }

                return valido;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Valida la categoría seleccionada o crea una nueva si no existe.
        /// </summary>
        private bool validarCategoria()
        {
            try
            {
                string texto = cbEditCategoria.Text.Trim();

                if (string.IsNullOrWhiteSpace(texto))
                {
                    _productoSeleccionado.Categorias = null;
                    _productoSeleccionado.idCategoria = null;
                    return true;
                }

                Categorias? categoria = _categorias.FirstOrDefault(c => c.Descripcion == texto);

                if (categoria == null)
                {
                    texto = Validaciones.capitalizarTexto(texto, true);
                    categoria = new Categorias { Indole = "Productos", Descripcion = texto };
                }

                _productoSeleccionado.Categorias = categoria;
                _productoSeleccionado.idCategoria = categoria.IdCategoria;
                errorProvider1.SetError(cbEditCategoria, "");
                return true;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                errorProvider1.SetError(cbEditCategoria, "Excepción no controlada");
                return false;
            }
        }

        /// <summary>
        /// Valida la dosificación del producto (unidad de medida y cantidad).
        /// </summary>
        private bool validarDosificacion()
        {
            try
            {
                bool cancelar = false;
                string mensaje = string.Empty;

                // Si no se seleccionó unidad de medida, se permite continuar
                if (cbUdMedida.SelectedIndex < 0)
                {
                    errorProvider1.SetError(txtMedida, string.IsNullOrEmpty(txtMedida.Text) ? "" : "Debe Seleccionar una unidad de Medida");
                    errorProvider1.SetError(txtCantMedida, string.IsNullOrEmpty(txtCantMedida.Text) ? "" : "Debe Seleccionar una unidad de Medida");
                    return true;
                }

                // Validación de medida y cantidad
                decimal? medida = validarCampoNumerico(txtMedida, false, false, ref mensaje);
                mensaje = string.Empty;
                decimal? cantidad = validarCampoNumerico(txtCantMedida, true, false, ref mensaje);
                int medidaEntero = 0;

                if (medida == null)
                {
                    errorProvider1.SetError(txtCantMedida, string.IsNullOrEmpty(txtCantMedida.Text) ? "" : "Debe ingresar la dosificación de Medida");
                    cancelar = true;
                }
                else
                    medidaEntero = (int)medida;

                if (medidaEntero <= 0)
                {
                    errorProvider1.SetError(txtMedida, "Valor no permitido");
                    cancelar = true;
                }

                if (!cancelar && cantidad != null)
                {
                    if (cantidad < 0)
                    {
                        errorProvider1.SetError(txtCantMedida, "Valor no permitido");
                        cancelar = true;
                    }
                }

                if (!cancelar)
                {
                    // Se asignan los valores al producto
                    UnidadesMedidas? unidad = cbUdMedida.SelectedItem as UnidadesMedidas;
                    _productoSeleccionado.IdUnidadMedida = unidad?.IdUnidadMedida;
                    _productoSeleccionado.UnidadesMedidas = unidad;
                    _productoSeleccionado.CantidadMedida = cantidad;
                    _productoSeleccionado.Medida = (int)medida;
                }
                else
                {
                    // Si hay errores, se limpian los valores
                    _productoSeleccionado.IdUnidadMedida = null;
                    _productoSeleccionado.UnidadesMedidas = null;
                    _productoSeleccionado.CantidadMedida = null;
                    _productoSeleccionado.Medida = null;
                }

                return !cancelar;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Acciones

        /// <summary>
        /// Sugiere un nuevo código de producto desde la capa de negocio
        /// y lo asigna al campo de texto.
        /// </summary>
        private void sugerirCodigo()
        {
            string mensaje = string.Empty;

            // Obtiene el código sugerido desde la capa de negocio
            txtCodigo.Text = ProductosNegocio.SugerirCodigo(ref mensaje);

            // Si hubo algún mensaje de error, se muestra al usuario
            if (!string.IsNullOrWhiteSpace(mensaje))
                Mensajes.mensajeError(mensaje);
        }

        /// <summary>
        /// Cierra el formulario previa confirmación del usuario.
        /// </summary>
        private void cerrarFormulario()
        {
            // Solicita confirmación al usuario
            bool confirmar = Mensajes.confirmarCierre();

            // Si el usuario cancela, se evita el cierre
            if (!confirmar)
            {
                cerrando = false;
                return;
            }

            // Marca el estado de cierre y procede a cerrar el formulario
            cerrando = true;
            this.Close();
        }

        /// <summary>
        /// Registra un nuevo producto en la base de datos.
        /// </summary>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si el producto se registró correctamente, False en caso contrario.</returns>
        private bool registrarProducto(ref string mensaje)
        {
            // Validación: debe existir un producto (Se utiliza _productoSeleccionado para reciclar)
            if (_productoSeleccionado == null)
            {
                mensaje = "No llega el producto a registrar";
                return false;
            }

            try
            {
                // Se registra el producto y se asigna el Id generado
                _productoSeleccionado.IdProducto = ProductosNegocio.RegistrarProducto(_productoSeleccionado, ref mensaje);

                // Devuelve true si el Id es válido (>0)
                return _productoSeleccionado.IdProducto > 0;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Modifica un producto existente en la base de datos.
        /// </summary>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si el producto se modificó correctamente, False en caso contrario.</returns>
        private bool modificarProducto(ref string mensaje)
        {
            // Validación: debe existir un producto con Id válido
            if (_productoSeleccionado?.IdProducto == null)
            {
                mensaje = "No llega el producto a modificar";
                return false;
            }

            // Validación: el formulario debe estar en modo Modificación
            if (modo != EnumModoForm.Modificacion)
            {
                mensaje = "Modo incorrecto en el formulario";
                return false;
            }

            try
            {
                // Se envía el producto a la capa de negocio para modificarlo
                return ProductosNegocio.ModificarProducto(_productoSeleccionado, ref mensaje);
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Calcula el margen de ganancia de un producto.
        /// </summary>
        /// <param name="precio">Precio de venta como string.</param>
        /// <param name="costo">Costo de compra como string.</param>
        /// <param name="comision">Comisión como string (en porcentaje o decimal).</param>
        /// <returns>Margen calculado como decimal.</returns>
        private decimal calcularMargen(string? precio, string? costo, string? comision)
        {
            decimal resultado = 0;
            decimal pvp = 0;          // Precio de venta
            decimal cometa = 0;       // Comisión
            decimal costoCompra = 0;  // Costo de compra

            try
            {
                // Intenta convertir los valores a decimal
                bool precioCorrecto = decimal.TryParse(precio, out pvp);
                bool costoCorrecto = decimal.TryParse(costo, out costoCompra);

                // Convierte la comisión a decimal y normaliza si es porcentaje
                if (decimal.TryParse(comision, out cometa))
                {
                    cometa = cometa > 0 ? cometa / 100 : 0;
                }

                // Calcula el margen: precio neto - costo
                resultado = pvp * (1 - cometa);
                resultado -= costoCompra;

                return resultado;
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado: " + ex.Message);
                return 0;
            }
        }

        #endregion

        #endregion

    }
}
