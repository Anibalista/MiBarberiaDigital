using Entidades_SGBM;
using Front_SGBM.UXDesign;
using Negocio_SGBM;
using Utilidades;

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
            CargarProductos();          // Carga la lista de productos desde la base de datos.
            CargarCategorias();         // Carga las categorías disponibles.
            CargarUnidadesDeMedida();   // Carga las unidades de medida disponibles.
            SugerirCodigo();            // Sugiere un código para un nuevo producto.
        }

        #endregion

        #region Cargas de Listados

        /// <summary>
        /// Carga la lista de productos desde la capa de negocio.
        /// </summary>
        private void CargarProductos()
        {
            _productos = null; // Se inicializa la lista en null

            try
            {
                // Se obtiene la lista de productos desde la capa negocio
                var resultado = ProductosNegocio.ListaSimple();

                // Si la operación no fue exitosa, se muestra el mensaje de error
                if (!resultado.Success || resultado.Data == null)
                {
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);
                    return;
                }

                // Si hubo algún mensaje de advertencia, se muestra
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);

                // Asignar la lista de productos obtenida
                _productos = resultado.Data ?? new List<Productos>();

                // Refresca la grilla con los productos cargados
                RefrescarGrilla();
            }
            catch (Exception ex)
            {
                // Manejo de errores inesperados
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
            }
        }

        /// <summary>
        /// Refresca la grilla de productos aplicando filtros de stock, estado y categoría.
        /// </summary>
        /// <param name="filtro">Texto de búsqueda para filtrar por descripción.</param>
        private void RefrescarGrilla(string filtro = "")
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
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
            }
            finally
            {
                cargando = false; // Finaliza la carga
            }
        }

        /// <summary>
        /// Carga las unidades de medida desde la capa de negocio.
        /// </summary>
        private void CargarUnidadesDeMedida()
        {
            cargando = true;

            try
            {
                bindingSourceMedidas.DataSource = null;

                // Se obtiene la lista de unidades de medida desde la capa negocio
                var resultado = ProductosNegocio.ListarUnidadesMedida();

                // Si la operación no fue exitosa, se muestra el mensaje de error
                if (!resultado.Success || resultado.Data == null)
                {
                    Mensajes.MensajeError(resultado.Mensaje);
                    return;
                }

                // Si hubo algún mensaje de advertencia, se muestra
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);

                // Se asigna la lista de unidades de medida al binding source
                _unidadesMedidas = resultado.Data;
                bindingSourceMedidas.DataSource = _unidadesMedidas;

                // Se limpia la selección del combo
                cbUdMedida.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Manejo de errores inesperados
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Carga las categorías disponibles desde la capa de negocio.
        /// </summary>
        private void CargarCategorias()
        {
            cargando = true;

            try
            {
                _categorias = null;
                bindingSourceCategorias.DataSource = null;

                // Se obtiene la lista de categorías para productos desde la capa negocio
                var resultado = CategoriasNegocio.ListarPorIndole("Productos");

                // Si la operación no fue exitosa, se muestra el mensaje de error
                if (!resultado.Success || resultado.Data == null)
                {
                    Mensajes.MensajeError(resultado.Mensaje);
                    return;
                }

                // Si hubo algún mensaje de advertencia, se muestra
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);

                // Se asigna la lista de categorías al combo de edición
                _categorias = resultado.Data;
                cbEditCategoria.DataSource = _categorias;
                cbEditCategoria.SelectedIndex = -1;

                // Carga el combo de filtro de categorías
                CargarComboCatFiltro();
            }
            catch (Exception ex)
            {
                // Manejo de errores inesperados
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Carga el combo de categorías para filtro, incluyendo opciones especiales.
        /// </summary>
        private void CargarComboCatFiltro()
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
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
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
            CerrarFormulario();
        }

        /// <summary>
        /// Evento click del botón Guardar.
        /// Valida y modifica un producto existente si el modo es Modificación.
        /// </summary>
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (modo != EnumModoForm.Modificacion)
                {
                    Mensajes.MensajeError("Modo incorrecto en el formulario");
                    return;
                }

                var resultadoValidacion = ValidarProducto();
                if (!resultadoValidacion.Success)
                {
                    Mensajes.MensajeError("Modificación Fallida (Verifique los campos con error)\n" + resultadoValidacion.Mensaje);
                    return;
                }

                var resultadoModificar = ModificarProducto();
                if (!resultadoModificar.Success)
                {
                    Mensajes.MensajeError("Modificación Fallida (Verifique los campos con error)\n" + resultadoModificar.Mensaje);
                    return;
                }

                Mensajes.MensajeExito("Modificación exitosa");
                CargarProductos();
                CargarCategorias();
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al guardar modificación: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Evento click del botón Nuevo.
        /// Registra un nuevo producto si el modo es Alta.
        /// </summary>
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                if (modo != EnumModoForm.Alta)
                {
                    Mensajes.MensajeError("Modo incorrecto en el formulario");
                    return;
                }

                var resultadoValidacion = ValidarProducto();
                if (!resultadoValidacion.Success)
                {
                    Mensajes.MensajeError("Registro Fallido (Verifique los campos con error)\n" + resultadoValidacion.Mensaje);
                    return;
                }

                var resultadoRegistrar = RegistrarProducto();
                if (!resultadoRegistrar.Success)
                {
                    Mensajes.MensajeError("Registro Fallido (Verifique los campos con error)\n" + resultadoRegistrar.Mensaje);
                    return;
                }

                Mensajes.MensajeExito("Registro exitoso");
                CargarProductos();
                CargarCategorias();
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al registrar producto: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
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
            try
            {
                string accion = btnBaja.Text;
                if (_productoSeleccionado == null)
                {
                    Mensajes.MensajeError($"Seleccione un producto para {accion} antes de continuar");
                    return;
                }

                string pregunta = $"¿Desea {accion} el producto {_productoSeleccionado.Descripcion}?";
                DialogResult respuesta = Mensajes.Respuesta(pregunta);
                if (respuesta == DialogResult.No) return;

                // Convierte "Activar"/"Anular" a "Activado"/"Anulado" para el mensaje final
                string accionPasado = accion.Length > 2 ? accion.Substring(0, accion.Length - 2) + "do" : accion + "do";

                var resultadoCambio = ProductosNegocio.CambiarEstadoProducto(_productoSeleccionado);
                if (!resultadoCambio.Success)
                {
                    Mensajes.MensajeError(resultadoCambio.Mensaje);
                }
                else
                {
                    Mensajes.MensajeExito($"¡Producto {accionPasado} exitosamente!");
                }

                CargarProductos();
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al cambiar estado del producto: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Evento click del botón Modificar.
        /// Habilita la edición de un producto seleccionado.
        /// </summary>
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null)
            {
                Mensajes.MensajeError("Seleccione un producto a modificar");
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
                RefrescarGrilla(txtFiltro.Text.Trim());
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
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
            RefrescarGrilla(txtFiltro.Text.Trim());
        }

        /// <summary>
        /// Evento cambio de estado en check de Anulados.
        /// Refresca la grilla aplicando el filtro correspondiente.
        /// </summary>
        private void checkAnulados_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            RefrescarGrilla(txtFiltro.Text.Trim());
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
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
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
                RefrescarGrilla(texto);
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
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
            e.Handled = !Validaciones.EsDigitoNumerico(e.KeyChar);
        }

        /// <summary>
        /// Evento KeyPress para campos decimales.
        /// Permite solo dígitos decimales.
        /// </summary>
        private void txtDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cerrando)
                return;
            e.Handled = !Validaciones.EsDigitoDecimal(e.KeyChar);
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
                decimal margen = CalcularMargen(txtPrecioVenta.Text, txtCosto.Text, txtComision.Text);
                txtMargen.Text = margen == 0 ? "" : margen.ToString("0.00");
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
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
                decimal margen = CalcularMargen(txtPrecioVenta.Text, txtCosto.Text, txtComision.Text);
                txtMargen.Text = margen == 0 ? "" : margen.ToString("0.00");
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
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
                decimal margen = CalcularMargen(txtPrecioVenta.Text, txtCosto.Text, txtComision.Text);
                txtMargen.Text = margen == 0 ? "" : margen.ToString("0.00");
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError("Error inesperado: " + ex.Message);
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
            try
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
            catch (Exception ex)
            {
                var msg = "Error inesperado al actualizar estado del botón de baja: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Limpia los campos de edición y resetea la selección de producto.
        /// </summary>
        /// <param name="nuevo">Indica si se debe sugerir un nuevo código al limpiar.</param>
        private void limpiarSeleccion(bool nuevo = true)
        {
            try
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
                    SugerirCodigo();

                // Limpia cualquier error previo en el formulario.
                errorProvider1.Clear();
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al limpiar selección: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
            finally
            {
                cargando = false; // Se habilitan nuevamente los eventos.
            }
        }

        /// <summary>
        /// Activa o desactiva los controles de edición según el modo del formulario.
        /// </summary>
        private void activarControlesEdit()
        {
            try
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
                    cbEditCategoria.Text = string.Empty;

                // Configuración de botones según el modo.
                btnGuardar.Enabled = activo;
                btnNuevo.Enabled = modo == EnumModoForm.Alta;
                btnModificar.Enabled = !activo;
                btnBaja.Enabled = !activo;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al activar/desactivar controles: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Activa o desactiva los campos relacionados con medidas.
        /// </summary>
        /// <param name="activo">True para habilitar, False para deshabilitar.</param>
        private void activarCamposMedida(bool activo)
        {
            try
            {
                txtMedida.Enabled = activo;
                txtCantMedida.Enabled = activo;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al activar/desactivar campos de medida: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
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
                txtCodigo.Text = _productoSeleccionado.CodProducto ?? string.Empty;
                txtDescripcion.Text = _productoSeleccionado.Descripcion ?? string.Empty;
                checkActivo.Checked = _productoSeleccionado.Activo;
                txtPrecioVenta.Text = _productoSeleccionado.PrecioVenta.ToString("0.00");

                // Conversión de comisión a porcentaje.
                decimal? comision = null;
                if (_productoSeleccionado.Comision != null)
                    comision = _productoSeleccionado.Comision * 100;

                txtComision.Text = comision?.ToString("0.00") ?? string.Empty;

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
                txtCantMedida.Text = _productoSeleccionado.CantidadMedida?.ToString("0.00") ?? string.Empty;
                txtMedida.Text = _productoSeleccionado.Medida?.ToString() ?? string.Empty;

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
                var msg = "Error inesperado al cargar campos del producto: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        #endregion

        #region Validaciones

        /// <summary>
        /// Valida que el texto ingresado en un control sea correcto.
        /// Aplica capitalización y muestra errores en el ErrorProvider.
        /// Devuelve Resultado{bool}. Data = true si el texto es válido.
        /// </summary>
        private Resultado<bool> ValidarTexto(Control controlTexto)
        {
            try
            {
                string texto = controlTexto.Text.Trim();
                string mensajeValidacion = string.Empty;

                // Nota: Validaciones.textoCorrecto mantiene su contrato; adaptamos su salida a Resultado.
                if (!Validaciones.TextoCorrecto(texto, ref mensajeValidacion))
                {
                    errorProvider1.SetError(controlTexto, mensajeValidacion);
                    return Resultado<bool>.Fail(mensajeValidacion);
                }

                // Limpia errores y capitaliza el texto
                errorProvider1.SetError(controlTexto, string.Empty);
                controlTexto.Text = Validaciones.CapitalizarTexto(texto, true);
                return Resultado<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al validar texto: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Valida que un campo numérico contenga un valor correcto.
        /// Devuelve Resultado{decimal?}. Data = número si válido, null si no válido.
        /// </summary>
        private Resultado<decimal?> ValidarCampoNumerico(TextBox campo, bool esDecimal, bool obligatorio)
        {
            try
            {
                string texto = campo.Text.Trim();
                string mensajeValidacion = string.Empty;

                if (!Validaciones.EsNumeroDecimal(texto, ref mensajeValidacion, obligatorio))
                {
                    errorProvider1.SetError(campo, mensajeValidacion);
                    return Resultado<decimal?>.Fail(mensajeValidacion);
                }

                errorProvider1.SetError(campo, string.Empty);

                decimal numero = 0;
                if (!decimal.TryParse(texto, out numero))
                {
                    // Si no se pudo parsear aunque la validación haya pasado, devolver error controlado
                    var msg = "El valor numérico no tiene un formato válido.";
                    errorProvider1.SetError(campo, msg);
                    return Resultado<decimal?>.Fail(msg);
                }

                campo.Text = esDecimal ? numero.ToString("0.00") : ((int)numero).ToString();
                return Resultado<decimal?>.Ok(numero);
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al validar campo numérico: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return Resultado<decimal?>.Fail(msg);
            }
        }

        /// <summary>
        /// Verifica si el código ingresado ya existe en otro producto.
        /// Devuelve Resultado{bool}. Data = true si el código ya existe.
        /// </summary>
        private Resultado<bool> CodigoExiste()
        {
            try
            {
                string codigo = txtCodigo.Text.Trim();
                var resultado = ProductosNegocio.BuscarPorCodigo(codigo);

                if (!resultado.Success && modo == EnumModoForm.Modificacion)
                {
                    // Si la búsqueda falló, devolvemos el mensaje de la capa negocio
                    return Resultado<bool>.Fail(resultado.Mensaje);
                }

                Productos? existente = resultado.Data;
                if (existente != null)
                {
                    string mensaje = $"El código {codigo} ya pertenece al producto {existente.Descripcion}";
                    return Resultado<bool>.Ok(true, mensaje);
                }

                return Resultado<bool>.Ok(false);
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al verificar código: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Valida todos los campos del producto antes de registrar o modificar.
        /// Devuelve Resultado{bool}. Data = true si el producto es válido.
        /// </summary>
        private Resultado<bool> ValidarProducto()
        {
            try
            {
                if (_productoSeleccionado == null)
                    _productoSeleccionado = new Productos();

                bool valido = true;
                string mensajeLocal = string.Empty;

                // Código
                var resultadoCodigo = ValidarCampoNumerico(txtCodigo, false, true);
                if (!resultadoCodigo.Success)
                {
                    errorProvider1.SetError(txtCodigo, resultadoCodigo.Mensaje);
                    return Resultado<bool>.Fail(resultadoCodigo.Mensaje);
                }

                decimal? codigo = resultadoCodigo.Data;
                if (codigo != null)
                {
                    var resultadoExiste = CodigoExiste();
                    if (!resultadoExiste.Success && modo == EnumModoForm.Modificacion)
                        return Resultado<bool>.Fail(resultadoExiste.Mensaje);

                    bool existe = resultadoExiste.Data;
                    if (existe)
                    {
                        // Si existe y estamos en alta, o el código cambió en modificación, es inválido
                        if (modo == EnumModoForm.Alta)
                            valido = false;
                        else if (txtCodigo.Text.Trim() != _productoSeleccionado.CodProducto)
                            valido = false;
                    }

                    if (valido)
                        _productoSeleccionado.CodProducto = txtCodigo.Text.Trim();
                    else
                    {
                        mensajeLocal = resultadoExiste.Mensaje ?? $"El código {txtCodigo.Text.Trim()} ya existe.";
                        Mensajes.MensajeError(mensajeLocal);
                        errorProvider1.SetError(txtCodigo, mensajeLocal);
                        return Resultado<bool>.Fail(mensajeLocal);
                    }
                }

                // Descripción
                var resultadoTexto = ValidarTexto(txtDescripcion);
                if (!resultadoTexto.Success)
                    return Resultado<bool>.Fail(resultadoTexto.Mensaje);

                _productoSeleccionado.Descripcion = txtDescripcion.Text;

                // Estado activo
                _productoSeleccionado.Activo = checkActivo.Checked;

                // Precio de venta
                var resultadoPrecio = ValidarCampoNumerico(txtPrecioVenta, true, true);
                if (!resultadoPrecio.Success)
                    return Resultado<bool>.Fail(resultadoPrecio.Mensaje);

                _productoSeleccionado.PrecioVenta = resultadoPrecio.Data ?? 0;

                // Costo (opcional)
                var resultadoCosto = ValidarCampoNumerico(txtCosto, true, false);
                if (!resultadoCosto.Success && !string.IsNullOrWhiteSpace(resultadoCosto.Mensaje))
                    return Resultado<bool>.Fail(resultadoCosto.Mensaje);

                if (resultadoCosto.Data.HasValue)
                {
                    _productoSeleccionado.Costo = resultadoCosto.Data.Value;
                    // Si se requiere que costo > 0 para considerarlo válido
                    if (resultadoCosto.Data <= 0)
                        valido = false;
                }

                // Comisión (porcentaje)
                var resultadoComision = ValidarCampoNumerico(txtComision, true, false);
                if (!resultadoComision.Success && !string.IsNullOrWhiteSpace(resultadoComision.Mensaje))
                    return Resultado<bool>.Fail(resultadoComision.Mensaje);

                if (resultadoComision.Data.HasValue)
                {
                    decimal com = resultadoComision.Data.Value;
                    if (com > 100 || com < 0)
                    {
                        var msg = $"El porcentaje {com}% no es una comisión válida";
                        errorProvider1.SetError(txtComision, msg);
                        return Resultado<bool>.Fail(msg);
                    }
                    _productoSeleccionado.Comision = com != 0 ? com / 100 : com;
                }

                // Stock
                var resultadoStock = ValidarCampoNumerico(txtStockUds, false, true);
                if (!resultadoStock.Success)
                    return Resultado<bool>.Fail(resultadoStock.Mensaje);

                _productoSeleccionado.Stock = (int)(resultadoStock.Data ?? 0);

                // Categoría
                var resultadoCategoria = ValidarCategoria();
                if (!resultadoCategoria.Success)
                    return Resultado<bool>.Fail(resultadoCategoria.Mensaje);

                // Dosificación
                var resultadoDosificacion = ValidarDosificacion();
                if (!resultadoDosificacion.Success)
                {
                    // Si la dosificación devolvió Fail, preguntar al usuario si desea continuar
                    DialogResult respuesta = Mensajes.Respuesta("La dosificación se anuló por errores en los campos\n¿Desea continuar igualmente?");
                    if (respuesta == DialogResult.No)
                        return Resultado<bool>.Fail("El usuario canceló por errores en la dosificación.");
                }

                return valido ? Resultado<bool>.Ok(true) : Resultado<bool>.Fail("Validaciones detectaron valores no permitidos.");
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al validar producto: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Valida la categoría seleccionada o crea una nueva si no existe.
        /// Devuelve Resultado{bool}. Data = true si la categoría quedó asignada correctamente.
        /// </summary>
        private Resultado<bool> ValidarCategoria()
        {
            try
            {
                string texto = cbEditCategoria.Text.Trim();

                if (string.IsNullOrWhiteSpace(texto))
                {
                    _productoSeleccionado.Categorias = null;
                    _productoSeleccionado.idCategoria = null;
                    errorProvider1.SetError(cbEditCategoria, string.Empty);
                    return Resultado<bool>.Ok(true);
                }

                Categorias? categoria = _categorias?.FirstOrDefault(c => c.Descripcion.Equals(texto, StringComparison.OrdinalIgnoreCase));

                if (categoria == null)
                {
                    texto = Validaciones.CapitalizarTexto(texto, true);
                    categoria = new Categorias { Indole = "Productos", Descripcion = texto };
                }

                _productoSeleccionado!.Categorias = categoria;
                _productoSeleccionado.idCategoria = categoria.IdCategoria;
                errorProvider1.SetError(cbEditCategoria, string.Empty);
                return Resultado<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al validar categoría: " + ex.Message;
                Mensajes.MensajeError(msg);
                errorProvider1.SetError(cbEditCategoria, "Excepción no controlada");
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Valida la dosificación del producto (unidad de medida y cantidad).
        /// Devuelve Resultado{bool}. Data = true si la dosificación es válida o no aplica.
        /// </summary>
        private Resultado<bool> ValidarDosificacion()
        {
            try
            {
                // Si no se seleccionó unidad de medida, se permite continuar
                if (cbUdMedida.SelectedIndex < 0)
                {
                    errorProvider1.SetError(txtMedida, string.IsNullOrEmpty(txtMedida.Text) ? string.Empty : "Debe Seleccionar una unidad de Medida");
                    errorProvider1.SetError(txtCantMedida, string.IsNullOrEmpty(txtCantMedida.Text) ? string.Empty : "Debe Seleccionar una unidad de Medida");
                    return Resultado<bool>.Ok(true);
                }

                bool cancelar = false;
                string mensajeLocal = string.Empty;

                var resultadoMedida = ValidarCampoNumerico(txtMedida, false, false);
                if (!resultadoMedida.Success)
                    return Resultado<bool>.Fail(resultadoMedida.Mensaje);

                decimal? medida = resultadoMedida.Data;
                decimal? cantidad = null;

                var resultadoCantidad = ValidarCampoNumerico(txtCantMedida, true, false);
                if (resultadoCantidad.Success)
                    cantidad = resultadoCantidad.Data;

                
                
                int medidaEntero = 0;

                if (medida == null)
                {
                    errorProvider1.SetError(txtCantMedida, string.IsNullOrEmpty(txtCantMedida.Text) ? string.Empty : "Debe ingresar la dosificación de Medida");
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
                    UnidadesMedidas? unidad = cbUdMedida.SelectedItem as UnidadesMedidas;
                    _productoSeleccionado.IdUnidadMedida = unidad?.IdUnidadMedida;
                    _productoSeleccionado.UnidadesMedidas = unidad;
                    _productoSeleccionado.CantidadMedida = cantidad;
                    _productoSeleccionado.Medida = (int)medida;
                    return Resultado<bool>.Ok(true);
                }
                else
                {
                    _productoSeleccionado.IdUnidadMedida = null;
                    _productoSeleccionado.UnidadesMedidas = null;
                    _productoSeleccionado.CantidadMedida = null;
                    _productoSeleccionado.Medida = null;
                    return Resultado<bool>.Fail("Errores en la dosificación.");
                }
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al validar dosificación: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #endregion

        #region Acciones

        /// <summary>
        /// Sugiere un nuevo código de producto desde la capa de negocio
        /// y lo asigna al campo de texto.
        /// </summary>
        private void SugerirCodigo()
        {
            try
            {
                var resultado = ProductosNegocio.SugerirCodigo();
                if (!resultado.Success)
                {
                    Mensajes.MensajeError(resultado.Mensaje);
                    return;
                }

                txtCodigo.Text = resultado.Data ?? string.Empty;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al sugerir código: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Cierra el formulario previa confirmación del usuario.
        /// </summary>
        private void CerrarFormulario()
        {
            bool confirmar = Mensajes.ConfirmarCierre();

            if (!confirmar)
            {
                cerrando = false;
                return;
            }

            cerrando = true;
            this.Close();
        }

        /// <summary>
        /// Registra un nuevo producto en la base de datos usando la capa de negocio.
        /// Devuelve Resultado{bool} con Data = true si el registro fue exitoso.
        /// </summary>
        private Resultado<bool> RegistrarProducto()
        {
            if (_productoSeleccionado == null)
                return Resultado<bool>.Fail("No llega el producto a registrar.");

            try
            {
                var resultado = ProductosNegocio.RegistrarProducto(_productoSeleccionado);
                if (!resultado.Success)
                    return Resultado<bool>.Fail(resultado.Mensaje);

                // Asignar Id generado si corresponde
                if (resultado.Data > 0)
                    _productoSeleccionado.IdProducto = resultado.Data;

                return Resultado<bool>.Ok(true, "Producto registrado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al registrar producto: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un producto existente en la base de datos usando la capa de negocio.
        /// Devuelve Resultado{bool} con Data = true si la modificación fue exitosa.
        /// </summary>
        private Resultado<bool> ModificarProducto()
        {
            if (_productoSeleccionado?.IdProducto == null)
                return Resultado<bool>.Fail("No llega el producto a modificar.");

            if (modo != EnumModoForm.Modificacion)
                return Resultado<bool>.Fail("Modo incorrecto en el formulario.");

            try
            {
                var resultado = ProductosNegocio.ModificarProducto(_productoSeleccionado);
                if (!resultado.Success)
                    return Resultado<bool>.Fail(resultado.Mensaje);

                return Resultado<bool>.Ok(true, "Producto modificado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al modificar producto: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Calcula el margen de ganancia de un producto.
        /// precio: Precio de venta como string.
        /// costo: Costo de compra como string.
        /// comision: Comisión como string (en porcentaje o decimal).
        /// Devuelve el margen calculado como decimal.
        /// </summary>
        private decimal CalcularMargen(string? precio, string? costo, string? comision)
        {
            decimal resultado = 0;
            decimal pvp = 0;          // Precio de venta
            decimal cometa = 0;       // Comisión
            decimal costoCompra = 0;  // Costo de compra

            try
            {
                decimal.TryParse(precio, out pvp);
                decimal.TryParse(costo, out costoCompra);

                if (decimal.TryParse(comision, out cometa))
                {
                    cometa = cometa > 0 ? cometa / 100 : 0;
                }

                // Precio neto después de comisión menos costo
                resultado = pvp * (1 - cometa);
                resultado -= costoCompra;

                return resultado;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al calcular margen: " + ex.Message;
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return 0;
            }
        }

        #endregion

        #endregion

    }
}
