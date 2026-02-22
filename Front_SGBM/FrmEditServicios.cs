using Entidades_SGBM;
using Negocio_SGBM;
using Front_SGBM.UXDesign;
using System.Data;
using System.Globalization;
using Utilidades;

namespace Front_SGBM
{
    public partial class FrmEditServicios : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;
        public Servicios? _servicio;
        private CostosServicios? _costoServicio;
        private Productos? _productoSeleccionado;
        private bool cerrando = false;
        private bool cargando = false;
        private bool administrandoCostos = false;
        private List<Categorias>? _categorias;
        private List<Productos>? _productos;
        private List<CostosServicios>? _costos;
        private string[] columnas = { "IdCostoServicio", "IdProducto", "IdServicio", "Productos", "Servicios" };

        public FrmEditServicios()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gestiona el cierre seguro del formulario, pidiendo confirmación al usuario y restaurando la vista principal.
        /// </summary>
        /// <param name="sender">El origen del evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void CerrarFormulario(object sender, EventArgs e)
        {
            // 1. Pedimos confirmación al usuario antes de salir
            cerrando = Mensajes.ConfirmarCierre();

            // Si el usuario elige "No" cancelar/salir, abortamos el cierre
            if (!cerrando)
            {
                return;
            }

            try
            {
                // 2. Buscamos el menú principal activo para restaurar la vista
                FrmMenuPrincipal padre = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();

                if (padre != null)
                {
                    // Le indicamos al menú que vuelva a cargar la vista de gestión de Servicios
                    padre.AbrirAbmServicios(sender, e, EnumModoForm.Consulta);
                }
                else
                {
                    // Registro preventivo por si el menú se "pierde" en memoria
                    Logger.LogError("No se encontró FrmMenuPrincipal al intentar cerrar el editor de servicios.");
                }
            }
            catch (Exception ex)
            {
                // 3. Estandarización de errores (adiós al MessageBox crudo)
                Logger.LogError($"Error al intentar restaurar el menú principal desde Servicios: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un problema menor al intentar volver al menú principal.");
            }
            finally
            {
                // 4. Garantía de cierre: Pase lo que pase en el try o en el catch, esta línea se ejecutará siempre
                this.Close();
            }
        }

        /// <summary>
        /// Inicialización del formulario: carga catálogos, servicio e insumos.
        /// Nota: CargarInsumos comunica con la capa de negocio y espera un Resultado en la respuesta.
        /// </summary>
        private void FrmEditServicios_Load(object sender, EventArgs e)
        {
            // Evitar ejecutar lógica si el formulario está cerrándose
            if (cerrando)
                return;

            try
            {
                cargando = true;

                // Cargar catálogos necesarios para la UI
                CargarProductos();
                CargarCategorias();

                // Si no es alta, traer el servicio existente
                if (modo != EnumModoForm.Alta)
                    CargarServicio();

                // Cargar insumos desde negocio (ahora devuelve Resultado)
                CargarInsumos();

                // Si estamos en modo consulta, deshabilitar edición
                if (modo == EnumModoForm.Consulta)
                    ActivarCampos(false);
            }
            catch (Exception ex)
            {
                var msg = "Error al inicializar el formulario: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error al cargar el formulario\n" + ex.Message);
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Carga los datos del servicio seleccionado en los controles del formulario.
        /// </summary>
        /// <remarks>
        /// - Usa cláusulas de guarda para evitar trabajo innecesario cuando no hay servicio válido.
        /// - Formatea valores numéricos para presentación en los controles.
        /// - Delegar la selección de categoría a <see cref="SeleccionarCategoria"/>.
        /// - En caso de excepción se registra el error con <c>Logger.LogError</c> y se muestra al usuario.
        /// </remarks>
        private void CargarServicio()
        {
            // Guard clauses: si no hay servicio o el Id no es válido, no hacemos nada
            if (_servicio == null || _servicio.IdServicio == null || _servicio.IdServicio < 1)
                return;

            try
            {
                txtServicio.Text = _servicio.NombreServicio ?? string.Empty;
                txtDescripcionServicio.Text = _servicio.Descripcion ?? string.Empty;

                // Duración y puntaje son enteros; mostrar sin decimales
                txtDuracion.Text = _servicio.DuracionMinutos.ToString();
                txtPuntaje.Text = _servicio.Puntaje.ToString();

                // Valores monetarios y porcentuales con dos decimales
                txtPrecio.Text = _servicio.PrecioVenta.ToString("0.00");

                txtComision.Text = (_servicio.Comision * 100).ToString("0.00");

                checkActivo.Checked = _servicio.Activo;

                // Seleccionar categoría en el combo (método maneja sus propias validaciones)
                SeleccionarCategoria();
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al cargar servicio: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Habilita o deshabilita los controles principales del formulario según el parámetro.
        /// Actualiza también el modo de administración de costos y sincroniza los campos de insumos.
        /// </summary>
        /// <param name="activos">True para habilitar los controles; false para deshabilitarlos.</param>
        private void ActivarCampos(bool activos)
        {
            // Evitar reentradas visuales mientras se actualiza la UI
            cargando = true;
            try
            {
                // Campos principales del servicio
                txtServicio.Enabled = activos;
                txtDescripcionServicio.Enabled = activos;
                txtDuracion.Enabled = activos;
                txtPrecio.Enabled = activos;
                txtPuntaje.Enabled = activos;
                txtComision.Enabled = activos;

                // Categoría y estado
                cbCategoria.Enabled = activos;
                checkActivo.Enabled = activos;

                // Botones y modo de administración de costos
                btnAdminCostos.Enabled = activos;
                administrandoCostos = activos;

                // Sincronizar campos relacionados a insumos (habilita/deshabilita controles de insumos)
                ActivarCamposInsumos();

                // Guardar
                btnGuardar.Enabled = activos;

                // Si se deshabilita la edición, limpiar errores visuales para evitar confusión
                if (!activos)
                {
                    errorProvider1?.Clear();
                }
            }
            catch (Exception ex)
            {
                var msg = "Error al activar/desactivar campos: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error de interfaz");
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Selecciona en el ComboBox la categoría asociada al servicio actual.
        /// </summary>
        /// <remarks>
        /// - Usa cláusulas de guarda para evitar trabajo innecesario cuando el formulario está cerrando,
        ///   cuando no hay categorías cargadas o cuando el servicio no tiene categoría válida.
        /// - Si la selección no se puede realizar se muestra una advertencia al usuario.
        /// - En caso de excepción se registra el error con <c>Logger.LogError</c> y se muestra el mensaje completo.
        /// </remarks>
        private void SeleccionarCategoria()
        {
            // Guard clauses: nada que hacer si no hay categorías o si no hay servicio válido
            if (_categorias == null || !_categorias.Any())
                return;

            if (_servicio == null || _servicio.IdCategoria <= 0)
                return;

            try
            {
                cbCategoria.SelectedValue = _servicio.IdCategoria;

                if (cbCategoria.SelectedIndex == -1)
                    Mensajes.MensajeAdvertencia("Problema al seleccionar la categoría");
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al seleccionar categoría: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Carga la lista de productos desde la capa de negocio y la asigna al BindingSource.
        /// </summary>
        /// <remarks>
        /// - Usa ProductosNegocio.ListaSimple() que devuelve <see cref="Resultado{T}"/>.
        /// - Si <see cref="Resultado{T}.Success"/> es false se muestra el error y se asigna lista vacía.
        /// - Si <see cref="Resultado{T}.Mensaje"/> contiene advertencias (aunque Success sea true) se muestran al usuario.
        /// - Se aplica salida temprana si el formulario está cerrando para evitar trabajo innecesario.
        /// - En caso de excepción se registra el error con <c>Logger.LogError</c> y se deshabilita el combo.
        /// </remarks>
        private void CargarProductos()
        {
            // Guard clause: si el formulario está cerrando no hacemos nada
            if (cerrando)
                return;

            cargando = true;
            try
            {
                bindingSourceProductos.DataSource = null;
                _productos = null;

                // Llamada a la capa de negocio (devuelve Resultado<List<Productos>>)
                var resultado = ProductosNegocio.ListaSimple();

                // Si la capa negocio devolvió un error, mostrarlo y asignar lista vacía
                if (!resultado.Success)
                {
                    bindingSourceProductos.DataSource = new List<Productos>();
                    cbProductos.SelectedIndex = -1;
                    return;
                }

                // Si hay mensaje de advertencia lo mostramos (puede venir en resultado.Mensaje aunque Success sea true)
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);

                // Asignar la lista (si Data es null, usar lista vacía)
                _productos = resultado.Data ?? new List<Productos>();
                bindingSourceProductos.DataSource = _productos;
                cbProductos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al traer productos de la BD: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                cbProductos.Enabled = false;
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Carga las categorías de la índole "Servicios" desde la capa de negocio
        /// y las asigna al BindingSource <see cref="bindingSourceCategorias"/> y al ComboBox <see cref="cbCategoria"/>.
        /// </summary>
        /// <remarks>
        /// - Utiliza <see cref="CategoriasNegocio.ListarPorIndole(string)"/> que devuelve <see cref="Resultado{T}"/>.
        /// - Si <see cref="Resultado{T}.Success"/> es false se muestra el error y se asigna una lista vacía.
        /// - Si <see cref="Resultado{T}.Mensaje"/> contiene advertencias (aunque Success sea true) se muestran al usuario.
        /// - Se aplica salida temprana si el formulario está cerrando para evitar trabajo innecesario.
        /// - El flag <c>cargando</c> evita disparar eventos mientras se actualiza el control.
        /// - En caso de excepción se registra el error con <c>Logger.LogError</c> y se deshabilita el combo.
        /// </remarks>
        private void CargarCategorias()
        {
            // Guard clause: si el formulario está cerrando no hacemos nada
            if (cerrando)
                return;

            cargando = true;
            try
            {
                bindingSourceCategorias.DataSource = null;
                _categorias = null;

                // Llamada a la capa de negocio (devuelve Resultado<List<Categorias>>)
                var resultado = CategoriasNegocio.ListarPorIndole("Servicios");

                // Si la capa negocio devolvió un error, mostrarlo y asignar lista vacía
                if (!resultado.Success)
                {
                    bindingSourceCategorias.DataSource = new List<Categorias>();
                    cbCategoria.SelectedIndex = -1;
                    return;
                }

                // Si hay mensaje de advertencia lo mostramos (puede venir en resultado.Mensaje aunque Success sea true)
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);

                // Asignar la lista (si Data es null, usar lista vacía)
                _categorias = resultado.Data ?? new List<Categorias>();
                bindingSourceCategorias.DataSource = _categorias;
                cbCategoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al traer categorías de la BD: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                cbCategoria.Enabled = false;
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Oculta en la grilla las columnas cuyo nombre aparece en el arreglo <c>columnas</c>.
        /// </summary>
        /// <remarks>
        /// - Usa un <see cref="HashSet{T}"/> con comparación insensible a mayúsculas para búsquedas O(1).
        /// - Aplica cláusulas de guarda para evitar trabajo innecesario cuando no hay columnas a ocultar
        ///   o cuando el DataGridView no tiene columnas cargadas.
        /// - En caso de excepción se registra el error y se muestra un mensaje al usuario.
        /// </remarks>
        private void OcultarColumnas(DataGridView dgv)
        {
            // Guard clauses
            if (dgv == null || dgv.Columns == null)
                return;

            if (columnas == null || columnas.Length == 0)
                return;

            try
            {
                // Comparador insensible a mayúsculas para evitar ToLower/ToUpper
                var columnasOcultar = new HashSet<string>(columnas, StringComparer.OrdinalIgnoreCase);

                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    // Si el nombre de la columna está en el conjunto, la ocultamos
                    column.Visible = !columnasOcultar.Contains(column.Name);
                }
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al ocultar columnas: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Recupera los insumos/costos asociados al servicio desde la capa de negocio y actualiza la interfaz.
        /// </summary>
        private void CargarInsumos()
        {
            // 1. Cláusula de guarda para evitar ejecuciones innecesarias al cerrar
            if (cerrando) return;

            try
            {
                cargando = true;

                // Limpiamos la lista local antes de la nueva carga para evitar residuos
                _costos = new List<CostosServicios>();

                // 2. Validación de entidad: Si el servicio es nuevo (Id null), no hay nada que buscar en la BD
                if (_servicio?.IdServicio == null)
                {
                    return;
                }

                // 3. Llamada a la capa de negocio
                var resultado = CostosNegocio.ObtenerInsumosPorIdServicio((int)_servicio.IdServicio);

                // 4. Manejo del resultado de negocio
                if (!resultado.Success)
                {
                    Logger.LogError($"Error de negocio al obtener insumos para el servicio {_servicio.IdServicio}: {resultado.Mensaje}");
                    return;
                }

                // 5. Asignación segura de datos
                _costos = resultado.Data ?? new List<CostosServicios>();

                // 6. Recalcular totales (Ya sin el parámetro 'ref' que quitamos antes)
                SumarCostos();

                // 7. Si el negocio envió un mensaje informativo (aunque sea exitoso), lo mostramos
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                {
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);
                }

                
            }
            catch (Exception ex)
            {
                // 8. Registro de errores fatales
                Logger.LogError($"Error inesperado en CargarInsumos (UI): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error crítico al intentar cargar los insumos del servicio.");
            }
            finally
            {
                // 9. Restablecer estados y refrescar la vista
                cargando = false;
                RefrescarGrilla();
            }
        }

        /// <summary>
        /// Suma los costos de la colección en memoria, actualiza los campos de la UI y dispara el recalculo del margen.
        /// </summary>
        /// <remarks>
        /// Se eliminó el parámetro 'ref' ya que el método gestiona sus propios errores de forma visual y por log.
        /// </remarks>
        private void SumarCostos()
        {
            try
            {
                // 1. Cálculo del total (Null-safe)
                // Si _costos es null, el total será 0 gracias al operador de coalescencia ??
                decimal total = _costos?.Sum(c => c.Costo) ?? 0m;

                // 2. Redondeo a 2 decimales para precisión financiera estándar
                total = Math.Round(total, 2);

                // 3. Actualización de la interfaz
                // Usamos "N2" que respeta la cultura del sistema para miles y decimales
                string totalTxt = total.ToString("N2");

                txtTotalCostos.Text = totalTxt;
                txtCostosServicio.Text = totalTxt;

                // 4. Recalcular margen
                // Se asume que CalcularMargen seguirá este mismo patrón sin 'ref'
                CalcularMargen();
            }
            catch (Exception ex)
            {
                // 5. Gestión de errores centralizada
                Logger.LogError($"Error crítico al sumar costos en FrmEditServicios: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error al intentar recalcular los totales de costos.");

                // En caso de error, aseguramos que los campos no queden con valores basura
                txtTotalCostos.Text = "0,00";
                txtCostosServicio.Text = "0,00";
            }
        }

        /// <summary>
        /// Refresca la grilla de insumos (costos) mostrando la lista ordenada,
        /// calculando el total y aplicando formato y ocultación de columnas.
        /// </summary>
        /// <remarks>
        /// - Usa cláusulas de guarda para evitar trabajo innecesario cuando ya se está cargando.
        /// - Si <c>_costos</c> es null se inicializa como lista vacía.
        /// - Calcula el total de costos con LINQ y lo muestra con formato local <c>es-AR</c>.
        /// - Actualiza <see cref="dataGridInsumos"/>, aplica ocultación de columnas y formato visual.
        /// - En caso de excepción se registra el error con <c>Logger.LogError</c> y se muestra al usuario.
        /// </remarks>
        private void RefrescarGrilla()
        {
            // Guard clause: si ya estamos en proceso de carga, no hacemos nada
            if (cargando)
                return;

            cargando = true;
            try
            {
                // Asegurar que la colección exista
                _costos ??= new List<CostosServicios>();

                // Preparar la grilla
                dataGridInsumos.DataSource = null;
                txtTotalCostos.Clear();

                // Ordenar y obtener lista a mostrar
                var lista = _costos.Count > 0
                    ? _costos.OrderBy(i => i.Descripcion).ToList()
                    : new List<CostosServicios>();

                // Calcular total y formatearlo para Argentina
                var cultura = CultureInfo.GetCultureInfo("es-AR");
                decimal totalCostos = lista.Sum(i => i.Costo);
                txtTotalCostos.Text = totalCostos.ToString("N2", cultura);

                // Asignar datos a la grilla y sincronizar campo de costos del servicio
                dataGridInsumos.DataSource = lista;
                txtCostosServicio.Text = txtTotalCostos.Text;

                // Ajustes visuales
                OcultarColumnas(dataGridInsumos);
                FormatoGrilla();

                // Refrescar y limpiar selección
                dataGridInsumos.Refresh();
                dataGridInsumos.ClearSelection();
            }
            catch (Exception ex)
            {
                var msg = $"Error al refrescar la grilla de Costos:\n{ex.ToString()}";
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Aplica el formato visual y de ancho a las columnas de la grilla de insumos.
        /// </summary>
        /// <remarks>
        /// - Formatea la columna "Costo" como moneda mediante <see cref="EstiloAplicacion.ApplyCurrencyFormat"/>.
        /// - Ajusta <see cref="DataGridViewColumn.FillWeight"/> y <see cref="DataGridViewColumn.MinimumWidth"/>
        ///   para columnas específicas: "Costo", "Descripción", "Unidades", "CantidadMedida".
        /// - Usa manejo de errores consistente: registra la excepción con <c>Logger.LogError</c>
        ///   y muestra el mensaje al usuario con <c>Mensajes.MensajeError</c>.
        /// - Se evita lógica innecesaria y se extrae la asignación de tamaños a una pequeña función local
        ///   para mejorar legibilidad y facilitar pruebas futuras.
        /// </remarks>
        private void FormatoGrilla()
        {
            // Si la grilla no está inicializada, no hacemos nada
            if (dataGridInsumos == null || dataGridInsumos.Columns == null || dataGridInsumos.Columns.Count == 0)
                return;

            try
            {
                // Aplica formato de moneda a la columna "Costo"
                EstiloAplicacion.ApplyCurrencyFormat(dataGridInsumos, "Costo");

                // Helper local para asignar FillWeight y MinimumWidth
                static void SetColumnSizing(DataGridViewColumn col, int fillWeight, int minWidth)
                {
                    col.FillWeight = fillWeight;
                    col.MinimumWidth = minWidth;
                }

                // Ajusta el FillWeight según el nombre de la columna
                foreach (DataGridViewColumn col in dataGridInsumos.Columns)
                {
                    // Comparación insensible a mayúsculas
                    var name = col.Name ?? string.Empty;

                    switch (name)
                    {
                        case "Costo":
                            SetColumnSizing(col, 20, 40);
                            break;

                        case "Descripción":
                            SetColumnSizing(col, 50, 120);
                            break;

                        case "Unidades":
                            SetColumnSizing(col, 15, 50);
                            break;

                        case "CantidadMedida":
                            SetColumnSizing(col, 15, 55);
                            break;

                        default:
                            // Para otras columnas no hacemos cambios aquí
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al formatear la grilla: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        //Eventos de cálculos
        /// <summary>
        /// Calcula el margen del servicio en tiempo real basándose en el precio, comisión y costos.
        /// Actualiza la interfaz y gestiona las alertas visuales.
        /// </summary>
        /// <returns><c>true</c> si los datos son válidos y el cálculo fue exitoso; de lo contrario, <c>false</c>.</returns>
        private bool CalcularMargen()
        {
            // 1. Instancia temporal para el cálculo (si no existe la principal)
            _servicio ??= new Servicios();

            // 2. Limpieza de errores previos
            errorProvider1?.Clear();
            bool datosValidos = true;

            try
            {
                // 3. Validación de Precio de Venta (Obligatorio y > 0)
                decimal precioVenta = ValidarCampoNumerico(txtPrecio, true);
                if (precioVenta <= 0m)
                {
                    ErrorCampo(txtPrecio, "Ingrese un precio de venta válido.");
                    datosValidos = false;
                }

                // 4. Validación de Comisión (0% a 100%)
                decimal porcentajeComision = ValidarCampoNumerico(txtComision, false, 0, 100);
                if (porcentajeComision < 0m)
                {
                    ErrorCampo(txtComision, "El porcentaje debe estar entre 0 y 100.");
                    datosValidos = false;
                }

                // 5. Validación de Costos (Cargados automáticamente desde la grilla)
                decimal costos = ValidarCampoNumerico(txtCostosServicio, false);
                if (costos < 0m)
                {
                    ErrorCampo(txtCostosServicio, "El costo no puede ser negativo.");
                    datosValidos = false;
                }

                // 6. Si hay errores, limpiamos el margen y salimos
                if (!datosValidos)
                {
                    txtMargen.Text = "0,00";
                    return false;
                }

                // 7. Cálculo de la Comisión (Solo si el Check está marcado)
                decimal montoComision = 0m;
                if (checkComision != null && checkComision.Checked && porcentajeComision > 0m)
                {
                    // Convertimos el porcentaje a valor monetario
                    montoComision = precioVenta * (porcentajeComision / 100m);
                }

                // 8. Cálculo Final del Margen
                // Margen = Precio Venta - Comisión (dinero) - Costos Totales
                decimal margenFinal = precioVenta - montoComision - costos;

                // 9. Actualización de la UI
                txtMargen.Text = margenFinal.ToString("N2");

                // Asignamos el valor al objeto para tenerlo listo
                _servicio.PrecioVenta = precioVenta;
                _servicio.Comision = porcentajeComision / 100m; // Guardamos como fracción (0.15 en vez de 15)
                _servicio.Costos = costos;
                _servicio.Margen = margenFinal;

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al calcular margen en FrmEditServicios: {ex.Message}");
                txtMargen.Text = "Error";
                return false;
            }
        }

        //Eventos de botones
        /// <summary>
        /// Cierra el formulario delegando en el manejador centralizado.
        /// </summary>
        private void BtnSalir_Click(object sender, EventArgs e)
        {
            try
            {
                // Delegar la lógica de cierre (maneja validaciones, confirmaciones y limpieza)
                CerrarFormulario(sender, e);
            }
            catch (Exception ex)
            {
                var msg = "Error al cerrar el formulario: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error al cerrar el formulario");
            }
        }

        /// <summary>
        /// Alterna el modo de administración de costos y actualiza la UI.
        /// </summary>
        private void BtnAdminCostos_Click(object sender, EventArgs e)
        {
            try
            {
                // Evitar cambios mientras el formulario se está cerrando o cargando
                if (cerrando || cargando)
                    return;

                // Alternar el estado y actualizar los campos relacionados
                administrandoCostos = !administrandoCostos;
                ActivarCamposInsumos();

                // Opcional: actualizar texto/estado del botón para feedback visual
                // btnAdminCostos.Text = administrandoCostos ? "Cancelar administración" : "Administrar costos";
            }
            catch (Exception ex)
            {
                var msg = "Error al alternar administración de costos: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error inesperado al cambiar el modo de administración de costos");
            }
        }

        /// <summary>
        /// Crea un nuevo costo en la grilla usando la lógica existente.
        /// </summary>
        private void BtnNuevoInsumo_Click(object sender, EventArgs e)
        {
            // Evitar reentradas y operaciones durante cierre o carga
            if (cerrando || cargando)
                return;

            try
            {
                cargando = true;
                NuevoCostoEnGrilla();
            }
            catch (Exception ex)
            {
                var msg = "Error al agregar nuevo insumo: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("No se pudo agregar el insumo");
            }
            finally
            {
                cargando = false;
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (_costoServicio == null || _costos == null)
            {
                Mensajes.MensajeError("Seleccione un costo a modificar");
                return;
            }
            if (_costoServicio.IdCostoServicio == null)
            {
                Mensajes.MensajeError("Problemas con e ID del costo a modificar");
                return;
            }
            cargando = true;
            try
            {
                string mensaje = string.Empty;
                if (!ArmarCostoServicio(ref mensaje))
                {
                    Mensajes.MensajeError(mensaje);
                    return;
                }
                int indice = _costos.FindIndex(c => c.IdCostoServicio == _costoServicio.IdCostoServicio);
                if (indice < 0)
                {
                    Mensajes.MensajeError("No se encuentra el Costo a modificar");
                    return;
                }
                _costos[indice] = _costoServicio;

                cargando = false;
                RefrescarGrilla();
                SumarCostos();
                LimpiarCamposInsumos();
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError("Error al cargar el insumo-resultado:\n" + ex.Message);
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Elimina el costo de servicio seleccionado de la lista temporal en memoria.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso:</b>
        /// <list type="number">
        /// <item>Valida que haya un costo seleccionado y una lista instanciada.</item>
        /// <item>Solicita confirmación al usuario.</item>
        /// <item>Remueve el objeto de la lista <c>_costos</c> y actualiza los totales de la interfaz.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            // 1. Cláusulas de guarda
            if (_costoServicio == null || _costos == null)
            {
                Mensajes.MensajeError("Debe seleccionar un costo de la lista para poder eliminarlo.");
                return;
            }

            // Validación de ID (siguiendo tu lógica: si es 0 es nuevo, pero debe ser un objeto válido)
            if (_costoServicio.IdCostoServicio < 0)
            {
                Mensajes.MensajeError("El costo seleccionado no posee un identificador válido.");
                return;
            }

            // 2. Confirmación de usuario
            if (Mensajes.Respuesta("¿Confirma eliminar el costo seleccionado?\nEsta acción no se puede deshacer.") == DialogResult.No)
            {
                return;
            }

            try
            {
                // Bloqueamos eventos visuales (como el SelectionChanged de la grilla)
                cargando = true;

                // 3. Eliminación del objeto
                // Intentamos remover por referencia directa, que es más rápido y limpio que buscar el índice
                bool eliminado = _costos.Remove(_costoServicio);

                if (!eliminado)
                {
                    // Si por alguna razón la referencia falló, buscamos por ID como respaldo
                    int indice = _costos.FindIndex(c => c.IdCostoServicio == _costoServicio.IdCostoServicio);
                    if (indice >= 0)
                    {
                        _costos.RemoveAt(indice);
                    }
                    else
                    {
                        Mensajes.MensajeError("No se encontró el costo en la lista actual.");
                        return;
                    }
                }

                // 4. Limpieza de selección y actualización de UI
                _costoServicio = null;
                cargando = false; // Desbloqueamos para que la grilla pueda actualizarse sin interferencias
                RefrescarGrilla();

                // Refactorización sugerida: SumarCostos ya no debería necesitar un 'ref mensaje' 
                // si es una operación aritmética interna.
                SumarCostos();

                LimpiarCamposInsumos();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al eliminar costo en FrmEditServicios: {ex.Message}");
                Mensajes.MensajeError("Ocurrió un error inesperado al intentar procesar la eliminación.");
            }
            finally
            {
                // El finally asegura que el formulario siempre vuelva a ser interactivo
                cargando = false;
            }
        }

        /// <summary>
        /// Maneja el evento de clic en el botón "Limpiar" para los insumos (costos) asociados al servicio.
        /// </summary>
        /// <remarks>
        /// Si el formulario está en proceso de cierre o carga, 
        /// se evita ejecutar la lógica para prevenir errores o trabajo innecesario.
        /// </remarks>
        /// <param name="sender">The source of the event, typically the 'Limpiar' button control.</param>
        /// <param name="e">The event data associated with the button click.</param>
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;

            try
            {
                cargando = true;
                LimpiarCamposInsumos();
            }
            catch (Exception ex)
            {
                var msg = "Error al limpiar insumos: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error de interfaz");
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Orquesta el proceso de guardado (Alta o Modificación) del servicio y sus costos asociados,
        /// procesando la respuesta mediante el patrón <see cref="Resultado{T}"/>.
        /// </summary>
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Contexto para mensajes
            string accion = modo == EnumModoForm.Alta ? "registrar" : "modificar";

            if (Mensajes.Respuesta($"¿Confirma que desea {accion} el servicio?") == DialogResult.No)
            {
                return;
            }

            try
            {
                // 2. Validación de UI (Mantenemos el 'out' aquí porque es validación interna del Form)
                if (!ValidarServicio(out string mensajeValidacion))
                {
                    Mensajes.MensajeError(mensajeValidacion);
                    return;
                }

                string mensajeFinalExito = string.Empty;

                // 3. Ejecución según el Modo
                if (modo == EnumModoForm.Alta)
                {
                    // Negocio devuelve: Resultado<int> (el ID generado) o Resultado<bool>
                    var resultadoAlta = ServiciosNegocio.Registrar(_servicio, _costos);

                    if (!resultadoAlta.Success)
                    {
                        Mensajes.MensajeError(resultadoAlta.Mensaje);
                        return;
                    }
                    mensajeFinalExito = resultadoAlta.Mensaje;
                }
                else
                {
                    // MODO MODIFICACIÓN
                    // A. Actualizar datos básicos del servicio
                    var resultadoMod = ServiciosNegocio.Modificar(_servicio);

                    if (!resultadoMod.Success)
                    {
                        Mensajes.MensajeError(resultadoMod.Mensaje);
                        return;
                    }

                    // B. Si el servicio se modificó bien, gestionamos los costos/insumos
                    var resultadoCostos = CostosNegocio.GestionarCostosServicios(_costos, (int)_servicio.IdServicio);

                    if (!resultadoCostos.Success)
                    {
                        // El servicio se guardó, pero los costos fallaron: es una Advertencia
                        Mensajes.MensajeAdvertencia($"{resultadoMod.Mensaje}\n\nNota: Hubo un problema con los costos: {resultadoCostos.Mensaje}");
                    }

                    mensajeFinalExito = resultadoMod.Mensaje;
                }

                // 4. Flujo de éxito final
                string consultaCierre = $"{mensajeFinalExito}\n\n¿Desea registrar otro servicio nuevo?";

                if (Mensajes.Respuesta(consultaCierre) == DialogResult.Yes)
                {
                    PrepararNuevoRegistro();
                }
                else
                {
                    CerrarFormulario(sender, e);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error crítico al {accion} el servicio: {ex.Message}");
                Mensajes.MensajeError("Ocurrió un error inesperado. Los detalles técnicos han sido registrados.");
            }
        }

        /// <summary>
        /// Limpia el formulario y recarga los catálogos para un nuevo registro sin cerrar la ventana.
        /// </summary>
        private void PrepararNuevoRegistro()
        {
            modo = EnumModoForm.Alta;
            LimpiarCampos();
            CargarProductos();
            CargarCategorias();
            CargarInsumos(); // Esto limpiará la grilla de costos al ser un servicio nuevo
        }

        /// <summary>
        /// Valida la integridad de los datos del formulario y construye el objeto <see cref="_servicio"/>.
        /// </summary>
        /// <param name="mensaje">Parámetro de salida que contiene el error detallado si la validación falla.</param>
        /// <returns><c>true</c> si todos los campos son válidos; de lo contrario, <c>false</c>.</returns>
        private bool ValidarServicio(out string mensaje)
        {
            // 1. Inicialización de salida y entidad
            mensaje = string.Empty;
            _servicio ??= new Servicios();

            // Bandera de control para no detener la validación en el primer error y marcar todos los campos
            bool esValido = true;

            try
            {
                // 2. Limpieza visual de errores previos
                errorProvider1?.Clear();

                // 3. Validación de Nombre (Obligatorio)
                if (!ValidarTexto(txtServicio, obligatorio: true, capitalizarTodo: true))
                {
                    esValido = false;
                }
                else
                {
                    _servicio.NombreServicio = txtServicio.Text.Trim();
                }

                // 4. Descripción (Opcional)
                if (ValidarTexto(txtDescripcionServicio, obligatorio: false, capitalizarTodo: false))
                {
                    _servicio.Descripcion = txtDescripcionServicio.Text.Trim();
                }

                // 5. Duración y Puntaje (Conversión segura a int)
                // Usamos (int) porque ValidarCampoNumerico devuelve decimal
                int duracion = (int)ValidarCampoNumerico(txtDuracion, obligatorio: false);
                if (duracion < 0)
                {
                    ErrorCampo(txtDuracion, "La duración no puede ser negativa.");
                    esValido = false;
                }
                _servicio.DuracionMinutos = duracion;

                int puntaje = (int)ValidarCampoNumerico(txtPuntaje, obligatorio: false);
                if (puntaje < 0)
                {
                    ErrorCampo(txtPuntaje, "El puntaje no puede ser negativo.");
                    esValido = false;
                }
                _servicio.Puntaje = puntaje;

                // 6. Integración con Cálculos Numéricos
                // Llamamos a CalcularMargen() sin 'ref' como refactorizamos en el paso anterior
                if (!CalcularMargen())
                {
                    esValido = false;
                    mensaje = "Error en el cálculo de márgenes y precios.";
                }

                // 7. Gestión de Categoría
                // Nota: Si CategoriaSeleccionada sigue usando 'ref', pásale una variable temporal
                Categorias? seleccionada = CategoriaSeleccionada(ref mensaje);

                if (seleccionada != null)
                {
                    // Siguiendo tu lógica de IDs manuales (no autoincrementales)
                    if (seleccionada.IdCategoria == 0 || seleccionada.IdCategoria == null)
                    {
                        // Es una categoría nueva: vinculamos el objeto completo
                        _servicio.Categorias = seleccionada;
                        _servicio.IdCategoria = 0;
                    }
                    else
                    {
                        // Es una categoría existente: vinculamos solo por ID
                        _servicio.IdCategoria = (int)seleccionada.IdCategoria;
                        _servicio.Categorias = null; // Evitamos redundancia circular
                    }
                }

                // 8. Estado Activo
                _servicio.Activo = checkActivo?.Checked ?? false;

                // 9. Resultado final: Si hay errores marcados pero no hay mensaje, ponemos uno genérico
                if (!esValido && string.IsNullOrWhiteSpace(mensaje))
                {
                    mensaje = "Por favor, revise los campos marcados en rojo.";
                }

                return esValido;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error crítico en ValidarServicio: {ex.Message}");
                mensaje = "Ocurrió un error inesperado al procesar los datos del servicio.";
                return false;
            }
        }

        /// <summary>
        /// Establece o limpia el error visual asociado a un TextBox mediante el ErrorProvider.
        /// </summary>
        /// <param name="campo">Control TextBox al que se le asigna el error.</param>
        /// <param name="error">Mensaje de error; si es vacío se limpia el error.</param>
        private void ErrorCampo(TextBox campo, string error = "")
        {
            if (campo == null)
                return;

            errorProvider1.SetError(campo, error ?? string.Empty);
        }

        /// <summary>
        /// Validador genérico de campos de texto.
        /// </summary>
        /// <param name="campo">TextBox a validar.</param>
        /// <param name="obligatorio">Indica si el campo es obligatorio.</param>
        /// <param name="capitalizarTodo">Si es true, aplica capitalización completa según la regla del helper.</param>
        /// <returns>True si el campo es válido (o no obligatorio y vacío); false si hay error.</returns>
        private bool ValidarTexto(TextBox campo, bool obligatorio, bool capitalizarTodo)
        {
            if (campo == null)
                return false;

            try
            {
                // Obtener y normalizar texto
                string texto = campo.Text.Trim();
                string mensajeError = string.Empty;

                // Nota: Validaciones mantiene la firma con ref string mensaje
                if (Validaciones.TextoCorrecto(texto, ref mensajeError))
                {
                    // Capitalizar según la regla existente (se mantiene la inversión de la bandera original)
                    campo.Text = Validaciones.CapitalizarTexto(texto, !capitalizarTodo);
                    ErrorCampo(campo, string.Empty); // limpiar error
                    return true;
                }

                // No es correcto
                if (obligatorio)
                {
                    ErrorCampo(campo, mensajeError);
                    return false;
                }

                // No es obligatorio: limpiar el campo y el error
                campo.Text = string.Empty;
                ErrorCampo(campo, string.Empty);
                return true;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al validar texto: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return false;
            }
        }

        /// <summary>
        /// Valida y convierte el contenido de un TextBox a decimal respetando obligatoriedad y rango.
        /// </summary>
        /// <param name="campo">TextBox a validar.</param>
        /// <param name="obligatorio">Indica si el campo es obligatorio.</param>
        /// <param name="minimo">Valor mínimo permitido (inclusive).</param>
        /// <param name="maximo">Valor máximo permitido (inclusive).</param>
        /// <returns>
        /// Valor decimal convertido si es válido.
        /// Devuelve <see cref="decimal.MinValue"/> cuando hay error de validación o conversión.
        /// Devuelve 0 cuando no es obligatorio y el campo está vacío.
        /// </returns>
        private decimal ValidarCampoNumerico(TextBox campo, bool obligatorio, int minimo = 0, int maximo = int.MaxValue)
        {
            if (campo == null)
                return decimal.MinValue;

            try
            {
                string texto = campo.Text.Trim();

                // Campo vacío
                if (string.IsNullOrWhiteSpace(texto))
                {
                    if (obligatorio)
                    {
                        const string msgObligatorio = "¡Campo obligatorio!";
                        ErrorCampo(campo, msgObligatorio);
                        return decimal.MinValue;
                    }

                    // No es obligatorio: limpiar error y devolver 0
                    ErrorCampo(campo, string.Empty);
                    return 0m;
                }

                // Intentar obtener decimal usando el helper (firma con ref string)
                string mensajeError = string.Empty;
                decimal resultado = ObtenerDecimal(texto, ref mensajeError);

                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    ErrorCampo(campo, mensajeError);
                    return decimal.MinValue;
                }

                // Validar rango
                if (resultado < minimo)
                {
                    var msg = $"No puede ser menor a {minimo}";
                    ErrorCampo(campo, msg);
                    return decimal.MinValue;
                }

                if (resultado > maximo)
                {
                    var msg = $"No puede ser mayor a {maximo}";
                    ErrorCampo(campo, msg);
                    return decimal.MinValue;
                }

                // Todo OK: limpiar error y devolver valor
                ErrorCampo(campo, string.Empty);
                return resultado;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al validar campo numérico: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return decimal.MinValue;
            }
        }

        /// <summary>
        /// Maneja el cambio de selección en cbProductos: actualiza descripción, recalcula costo y sincroniza referencias.
        /// </summary>
        private void CbProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            // No procesar si el formulario está cerrando o estamos actualizando controles
            if (cerrando || cargando)
                return;

            // Asegurar modelo temporal
            _costoServicio ??= new CostosServicios();

            try
            {
                // Obtener producto seleccionado de forma segura
                _productoSeleccionado = ProductoSeleccionado();
                if (_productoSeleccionado == null)
                {
                    // Si no hay producto seleccionado, limpiar descripción y monto
                    txtDescripcionInsumo.Text = string.Empty;
                    txtMontoInsumo.Text = "0.00";
                    return;
                }

                // Mostrar descripción del producto
                txtDescripcionInsumo.Text = _productoSeleccionado.ToString() ?? string.Empty;

                // Intentar calcular el costo del insumo con el producto seleccionado
                string error = string.Empty;
                if (!CalcularCostoInsumo(ref error))
                {
                    // Si hubo mensaje de error, mostrarlo; si no, mostrar mensaje genérico
                    if (!string.IsNullOrWhiteSpace(error))
                        Mensajes.MensajeError(error);
                    else
                        Mensajes.MensajeError("No se pudo calcular el costo del insumo.");
                    return;
                }

                // Si el cálculo fue exitoso, sincronizar referencias y mostrar monto formateado
                _costoServicio.Productos = _productoSeleccionado;
                _costoServicio.IdProducto = _productoSeleccionado.IdProducto;
                txtMontoInsumo.Text = _costoServicio.Costo.ToString("0.00", CultureInfo.CurrentCulture);

                // Limpiar posibles errores visuales relacionados
                ErrorCampo(txtCantidad, string.Empty);
                ErrorCampo(txtUnidades, string.Empty);
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al seleccionar producto: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error inesperado\n" + ex.Message);
            }
        }

        /// <summary>
        /// Maneja el cambio de selección en la grilla de insumos.
        /// </summary>
        private void DataGridInsumos_SelectionChanged(object sender, EventArgs e)
        {
            // Si el formulario está cerrando o ya estamos en un proceso de carga, no procesar el evento
            if (cerrando || cargando)
                return;

            try
            {
                // Limpiar errores y controles relacionados a insumos antes de cargar la nueva selección
                LimpiarErroresInsumos();

                // Si no hay lista de costos, no hay nada que seleccionar
                if (_costos == null || _costos.Count == 0)
                    return;

                // Obtener el insumo seleccionado de forma segura
                _costoServicio = InsumoSeleccionado();

                // Si no hay insumo seleccionado, limpiar campos y salir
                if (_costoServicio == null)
                {
                    LimpiarCamposInsumos();
                    return;
                }

                // Cargar los valores del insumo en los controles (usa el flag 'cargando' internamente)
                CargarInsumoACampos();
            }
            catch (Exception ex)
            {
                var msg = "Ocurrió un problema con la selección de costos: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Ocurrió un problema con la selección de costos\n" + ex.Message);
            }
        }

        /// <summary>
        /// Carga los datos del insumo seleccionado (_costoServicio) en los controles del formulario.
        /// </summary>
        /// <remarks>
        /// - Usa cláusulas de guarda para evitar trabajo innecesario cuando no hay insumo seleccionado.
        /// - Si el insumo referencia un producto intenta seleccionarlo en <see cref="cbProductos"/>.
        /// - Si el producto ya no existe en la lista se limpia la referencia y se notifica al usuario con una advertencia.
        /// - Maneja el flag <c>cargando</c> para evitar disparar eventos mientras se actualizan controles.
        /// - En caso de excepción se registra el error con <c>Logger.LogError</c> y se muestra al usuario el mensaje completo.
        /// </remarks>
        private void CargarInsumoACampos()
        {
            // Guard clause: nada que hacer si no hay insumo seleccionado
            if (_costoServicio == null)
                return;

            string mensaje = string.Empty;
            cargando = true;

            try
            {
                // Descripción textual del insumo (ToString() debe estar implementado en CostosServicios)
                txtDescripcionInsumo.Text = _costoServicio.ToString() ?? string.Empty;

                // Si el insumo tiene asociado un producto y la lista de productos está cargada, intentar seleccionarlo
                if (_costoServicio.IdProducto != null && _productos != null && _productos.Count > 0)
                {
                    cbProductos.SelectedValue = _costoServicio.IdProducto;

                    if (cbProductos.SelectedIndex != -1)
                    {
                        // El producto existe en el BindingSource: sincronizar referencias
                        _costoServicio.Productos = cbProductos.SelectedItem as Productos;
                        _productoSeleccionado = _costoServicio.Productos;
                    }
                    else
                    {
                        // El producto ya no existe en la BD: limpiar referencia y avisar
                        mensaje = "Problemas al encontrar el producto del insumo. Se ha limpiado la referencia al producto.";
                        _costoServicio.Productos = null;
                        _costoServicio.IdProducto = null;
                        _productoSeleccionado = null;
                    }
                }
                else
                {
                    // No hay producto asociado o no hay lista de productos: asegurar que los controles reflejen eso
                    cbProductos.SelectedIndex = -1;
                    _productoSeleccionado = null;
                }

                // Cantidad, unidades y monto
                txtCantidad.Text = _costoServicio.CantidadMedida.ToString();
                txtUnidades.Text = _costoServicio.Unidades.ToString();
                txtMontoInsumo.Text = _costoServicio.Costo.ToString("0.00");
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al cargar insumo en campos: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
            finally
            {
                cargando = false;

                // Si hubo algún mensaje no crítico, mostrarlo como advertencia (no lanzar excepción)
                if (!string.IsNullOrWhiteSpace(mensaje))
                    Mensajes.MensajeAdvertencia(mensaje);
            }
        }

        //Carga de selecciones
        /// <summary>
        /// Devuelve el producto actualmente seleccionado en el ComboBox <see cref="cbProductos"/>.
        /// </summary>
        /// <remarks>
        /// - Usa cláusulas de guarda para evitar excepciones cuando el control no está inicializado
        ///   o no hay selección.
        /// - Emplea el operador <c>as</c> para un casteo seguro; si el elemento no es del tipo esperado
        ///   devuelve <c>null</c>.
        /// - No captura excepciones innecesarias: la lógica es simple y segura con las guard clauses.
        /// </remarks>
        private Productos? ProductoSeleccionado()
        {
            // Guard clauses: si el combo no existe o no hay selección, no hacemos nada
            if (cbProductos == null)
                return null;

            if (cbProductos.SelectedIndex < 0)
                return null;

            // Casteo seguro; devuelve null si el SelectedItem no es Productos
            return cbProductos.SelectedItem as Productos;
        }

        /// <summary>
        /// Devuelve el insumo (CostosServicios) actualmente seleccionado en la grilla <see cref="dataGridInsumos"/>.
        /// </summary>
        /// <remarks>
        /// - Usa cláusulas de guarda para evitar excepciones cuando la grilla o la fila actual no están disponibles.
        /// - Realiza un casteo seguro con <c>as</c>; si el elemento enlazado no es del tipo esperado devuelve <c>null</c>.
        /// - Se evita un try/catch innecesario porque la lógica es simple y las guard clauses previenen la mayoría de errores.
        /// </remarks>
        private CostosServicios? InsumoSeleccionado()
        {
            // Guard clauses: si la grilla no existe o no hay fila actual, no hay selección
            if (dataGridInsumos == null)
                return null;

            if (dataGridInsumos.CurrentRow == null)
                return null;

            // Casteo seguro del objeto enlazado; devuelve null si no es del tipo esperado
            return dataGridInsumos.CurrentRow.DataBoundItem as CostosServicios;
        }

        /// <summary>
        /// Obtiene la categoría seleccionada en el ComboBox <see cref="cbCategoria"/>.
        /// </summary>
        /// <param name="mensaje">
        /// Mensaje de salida usado por las validaciones (firma con ref por compatibilidad con Validaciones).
        /// </param>
        /// <returns>
        /// La <see cref="Categorias"/> encontrada en <c>_categorias</c> cuyo nombre coincide con el texto del combo
        /// (comparación insensible a mayúsculas). Si no existe, devuelve una nueva instancia con <c>Indole = "Servicios"</c>.
        /// Devuelve <c>null</c> si el texto no pasa las validaciones o si ocurre un error.
        /// </returns>
        private Categorias? CategoriaSeleccionada(ref string mensaje)
        {
            // Inicializar mensaje para evitar valores residuales
            mensaje = string.Empty;

            // Guard clauses: control y texto
            if (cbCategoria == null)
                return null;

            var texto = (cbCategoria.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            try
            {
                // Validaciones mantiene la firma con ref string mensaje
                if (!Validaciones.TextoCorrecto(texto, ref mensaje))
                    return null;

                // Buscar categoría existente (comparación insensible a mayúsculas)
                var seleccionada = _categorias?
                    .FirstOrDefault(c => string.Equals(c.Descripcion?.Trim(), texto, StringComparison.OrdinalIgnoreCase));

                if (seleccionada != null)
                    return seleccionada;

                // Si no existe, crear una nueva categoría en memoria (no persiste en BD aquí)
                seleccionada = new Categorias
                {
                    Descripcion = Validaciones.CapitalizarTexto(texto),
                    Indole = "Servicios"
                };

                return seleccionada;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al obtener la categoría seleccionada: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                return null;
            }
        }

        //Activación y Limpieza de campos
        /// <summary>
        /// Activa o desactiva los controles relacionados a la administración de insumos
        /// según el flag <c>administrandoCostos</c>.
        /// </summary>
        private void ActivarCamposInsumos()
        {
            // Guard clauses: si los controles principales no están inicializados, no hacemos nada
            if (dataGridInsumos == null && cbProductos == null && txtDescripcionInsumo == null)
                return;

            // Lista de controles de entrada que comparten el mismo estado
            var controles = new Control?[]
            {
                txtDescripcionInsumo,
                txtMontoInsumo,
                cbProductos,
                txtCantidad,
                txtUnidades
            };

            // Aplicar el estado a los controles (null-safe)
            foreach (var ctrl in controles)
            {
                if (ctrl != null)
                    ctrl.Enabled = administrandoCostos;
            }

            // Botones (null-safe)
            if (btnEliminar != null) btnEliminar.Enabled = administrandoCostos;
            if (btnLimpiar != null) btnLimpiar.Enabled = administrandoCostos;
            if (btnModificar != null) btnModificar.Enabled = administrandoCostos;
            if (btnNuevoInsumo != null) btnNuevoInsumo.Enabled = administrandoCostos;
        }

        /// <summary>
        /// Limpia los controles relacionados al insumo y resetea el estado interno.
        /// </summary>
        private void LimpiarCamposInsumos()
        {
            // Si no hay controles relevantes inicializados, no hacemos nada
            if (txtDescripcionInsumo == null
                && txtMontoInsumo == null
                && txtCantidad == null
                && txtUnidades == null
                && cbProductos == null)
            {
                return;
            }

            cargando = true;
            try
            {
                txtDescripcionInsumo?.Clear();
                txtMontoInsumo?.Clear();
                txtCantidad?.Clear();
                txtUnidades?.Clear();

                if (cbProductos != null)
                    cbProductos.SelectedIndex = -1;

                _productoSeleccionado = null;
                _costoServicio = null;

                LimpiarErroresInsumos();
            }
            catch (Exception ex)
            {
                var msg = $"Error al limpiar los campos de insumos: {ex.ToString()}";
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Limpia los errores visuales asociados a los controles de insumos.
        /// </summary>
        private void LimpiarErroresInsumos()
        {
            // ErrorCampo ya es null-safe, así que simplemente llamamos para limpiar cada control.
            ErrorCampo(txtDescripcionInsumo);
            ErrorCampo(txtMontoInsumo);
            ErrorCampo(txtCantidad);
            ErrorCampo(txtUnidades);
        }

        /// <summary>
        /// Resetea el formulario de servicio a su estado inicial: limpia modelos, controles y estado de administración de insumos.
        /// </summary>
        /// <remarks>
        /// - Usa cláusulas de guarda y llamadas null-safe para evitar excepciones cuando controles no están inicializados.
        /// - Mantiene el flag <c>cargando</c> para evitar disparos de eventos mientras se actualizan controles.
        /// - Registra errores con <c>Logger.LogError</c> y muestra mensajes al usuario con <c>Mensajes.MensajeError</c>.
        /// - Llama a <see cref="RefrescarGrilla"/> al final para asegurar que la vista refleje el estado actual.
        /// </remarks>
        private void LimpiarCampos()
        {
            // Si no hay controles principales inicializados, no hacemos nada
            if (txtServicio == null
                && txtDescripcionServicio == null
                && txtDuracion == null
                && txtPrecio == null
                && txtPuntaje == null
                && txtComision == null
                && txtCostosServicio == null
                && txtMargen == null
                && txtTotalCostos == null
                && checkComision == null)
            {
                // Aun así reseteamos el estado interno
                _servicio = null;
                _costos = null;
                administrandoCostos = false;
                return;
            }

            cargando = true;
            try
            {
                // Reset del modelo
                _servicio = null;
                _costos = null;

                // Limpiar controles (null-safe)
                txtServicio?.Clear();
                txtDescripcionServicio?.Clear();
                txtDuracion?.Clear();
                txtPrecio?.Clear();
                txtPuntaje?.Clear();
                txtComision?.Clear();
                txtCostosServicio?.Clear();
                txtMargen?.Clear();
                txtTotalCostos?.Clear();

                // Estado por defecto
                if (checkComision != null) checkComision.Checked = true;
                administrandoCostos = false;

                // Limpiar insumos y actualizar disponibilidad de campos
                LimpiarCamposInsumos();
                ActivarCamposInsumos();
            }
            catch (Exception ex)
            {
                var msg = $"Error al limpiar los campos: {ex.ToString()}";
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
            finally
            {
                // Asegurar que el flag quede en false antes de refrescar la grilla
                cargando = false;

                // Refrescar la grilla para que la vista quede consistente con el estado interno
                try
                {
                    cargando = false; // Asegurar que el flag quede en false antes de refrescar la grilla
                    RefrescarGrilla();
                }
                catch (Exception ex)
                {
                    var msg = $"Error al refrescar la grilla después de limpiar campos: {ex.ToString()}";
                    Mensajes.MensajeError(msg);
                    Logger.LogError(msg);
                }
            }
        }

        //Carga de insumos (Costos) al servicio
        /// <summary>
        /// Construye y valida el objeto _costoServicio a partir de los controles del formulario.
        /// Reutiliza los validadores y calculadores existentes.
        /// </summary>
        /// <param name="mensaje">Salida con el mensaje de error en caso de fallo.</param>
        /// <returns>True si el objeto quedó armado y válido; false en caso contrario.</returns>
        private bool ArmarCostoServicio(ref string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                _costoServicio ??= new CostosServicios();

                bool error = false;

                // Descripción (obligatoria)
                if (!ValidarTexto(txtDescripcionInsumo, true, false))
                    error = true;

                _costoServicio.Descripcion = (txtDescripcionInsumo.Text ?? string.Empty).Trim();

                // Calcular costo en base a producto, unidades y medida
                string msgCalculo = string.Empty;
                if (!CalcularCostoInsumo(ref msgCalculo))
                {
                    if (!string.IsNullOrWhiteSpace(msgCalculo))
                    {
                        mensaje = msgCalculo;
                        error = true;
                    }
                }

                // Validar y normalizar el monto ingresado por el usuario
                decimal monto = 0m;
                string msgMonto = ValidarCampoDecimal(txtMontoInsumo, true, ref monto);
                if (!string.IsNullOrWhiteSpace(msgMonto))
                {
                    // Priorizar mensaje de validación del campo
                    mensaje = string.IsNullOrWhiteSpace(mensaje) ? msgMonto : mensaje + " " + msgMonto;
                    error = true;
                }
                else
                {
                    // Si el usuario ingresó un monto válido, usarlo; si no, mantener el calculado
                    if (monto > 0m)
                        _costoServicio.Costo = monto;
                    // si monto == 0 y ya hubo cálculo previo, se mantiene lo calculado por CalcularCostoInsumo
                }

                // Si hubo error, asegurar que el campo muestre el error correspondiente
                if (error && string.IsNullOrWhiteSpace(mensaje))
                    mensaje = "Antes de continuar verifique los campos con error";

                return !error;
            }
            catch (Exception ex)
            {
                mensaje = "Error inesperado\n" + ex.Message;
                Logger.LogError("ArmarCostoServicio: " + ex.ToString());
                Mensajes.MensajeError(mensaje);
                return false;
            }
        }

        /// <summary>
        /// Agrega un nuevo costo a la colección en memoria y actualiza la grilla.
        /// </summary>
        private void NuevoCostoEnGrilla()
        {
            cargando = true;
            try
            {
                // Asegurar instancia del modelo temporal
                _costoServicio ??= new CostosServicios();

                // No permitir re-agregar un costo ya persistido
                if (_costoServicio.IdCostoServicio != null)
                {
                    Mensajes.MensajeError("No puede volver a agregar un Costo existente, presione modificar");
                    return;
                }

                // Armar el costo desde los controles; ArmarCostoServicio debe llenar _costoServicio
                string mensaje = string.Empty;
                if (!ArmarCostoServicio(ref mensaje))
                {
                    Mensajes.MensajeError(mensaje);
                    return;
                }

                // Asegurar colección de costos en memoria
                _costos ??= new List<CostosServicios>();

                // Generar un Id temporal negativo para nuevos items en memoria
                int idMinimo = _costos.Count > 0 ? _costos.Min(c => c.IdCostoServicio ?? 0) : 0;
                int id = idMinimo > 0 ? -1 : idMinimo - 1;
                _costoServicio.IdCostoServicio = id;

                // Agregar y refrescar vista
                _costos.Add(_costoServicio);
                cargando = false; // Asegurar que el flag quede en false antes de refrescar la grilla
                RefrescarGrilla();

                // Recalcular totales y limpiar campos de insumo
                SumarCostos();
                LimpiarCamposInsumos();
            }
            catch (Exception ex)
            {
                var msg = "Error al cargar el insumo-resultado: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
        }

        /// <summary>
        /// Calcula el costo del insumo actual usando los valores de los controles y los helpers de conversión.
        /// </summary>
        /// <param name="mensaje">Salida con mensaje de error en caso de fallo.</param>
        /// <returns>True si el cálculo fue exitoso y el costo resultante es mayor a 0; false en caso contrario.</returns>
        private bool CalcularCostoInsumo(ref string mensaje)
        {
            mensaje = string.Empty;

            if (_costoServicio == null)
                return false;

            if (_productoSeleccionado == null)
                return false;

            try
            {
                // Validar y obtener cantidad de medida
                string msgCantidad = string.Empty;
                int medida = ObtenerNumero(txtCantidad.Text, ref msgCantidad);
                _costoServicio.CantidadMedida = medida > 0 ? medida : null;

                // Validar y obtener unidades
                string msgUnidades = string.Empty;
                int unidades = ObtenerNumero(txtUnidades.Text, ref msgUnidades);
                // Asegurar valor no negativo
                _costoServicio.Unidades = unidades < 0 ? 0 : unidades;
                txtUnidades.Text = _costoServicio.Unidades.ToString();

                // Si hubo errores en las conversiones, marcarlos y devolver fallo
                if (!string.IsNullOrWhiteSpace(msgCantidad) || !string.IsNullOrWhiteSpace(msgUnidades))
                {
                    if (!string.IsNullOrWhiteSpace(msgCantidad))
                        ErrorCampo(txtCantidad, msgCantidad);

                    if (!string.IsNullOrWhiteSpace(msgUnidades))
                        ErrorCampo(txtUnidades, msgUnidades);

                    mensaje = "Antes de continuar verifique los campos con error";
                    return false;
                }

                // Resetear errores de cantidad/unidades
                ErrorCampo(txtCantidad, string.Empty);
                ErrorCampo(txtUnidades, string.Empty);

                // Calcular costo: costo por unidades completas
                decimal costoProducto = _productoSeleccionado.Costo;
                decimal costoTotal = costoProducto * (_costoServicio.Unidades ?? 0);

                // Si el producto tiene medida (por ejemplo, paquete + fracción), agregar la parte proporcional
                if (_productoSeleccionado.Medida != null && _productoSeleccionado.Medida > 0 && _costoServicio.CantidadMedida != null)
                {
                    int medidaProducto = (int)_productoSeleccionado.Medida;
                    int medidaInsumo = (int)_costoServicio.CantidadMedida;
                    // Cálculo proporcional: (costo / medidaProducto) * medidaInsumo
                    costoTotal += (costoProducto / medidaProducto) * medidaInsumo;
                }

                // Asignar referencias
                _costoServicio.Costo = Math.Round(costoTotal, 2);
                _costoServicio.IdProducto = _productoSeleccionado.IdProducto;
                _costoServicio.Productos = _productoSeleccionado;

                // Mostrar monto formateado
                txtMontoInsumo.Text = _costoServicio.Costo.ToString("0.00", CultureInfo.CurrentCulture);

                // Resultado: verdadero si el costo es mayor a cero
                return _costoServicio.Costo > 0m;
            }
            catch (Exception ex)
            {
                mensaje = "Error excepcional: " + ex.Message;
                Logger.LogError("CalcularCostoInsumo: " + ex.ToString());
                Mensajes.MensajeError(mensaje);
                return false;
            }
        }

        /// <summary>
        /// Valida el contenido de un TextBox como decimal, reutilizando las validaciones existentes.
        /// </summary>
        /// <param name="campo">TextBox a validar.</param>
        /// <param name="obligatorio">Indica si el campo es obligatorio.</param>
        /// <param name="resultado">Salida con el valor decimal parseado (0 si está vacío y no es obligatorio).</param>
        /// <returns>Cadena vacía si todo OK; mensaje de error en caso contrario.</returns>
        private string ValidarCampoDecimal(TextBox campo, bool obligatorio, ref decimal resultado)
        {
            string mensaje = string.Empty;
            try
            {
                resultado = 0m;

                if (campo == null)
                    return "Control de entrada no provisto";

                string texto = (campo.Text ?? string.Empty).Trim();

                // Si hay texto, validar que sea decimal usando el wrapper que mantiene la firma de Validaciones
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    // EsnumeroDecimal delega a Validaciones.EsNumeroDecimal(ref mensaje)
                    if (!EsNumeroDecimal(texto, ref mensaje))
                    {
                        ErrorCampo(campo, mensaje);
                        return "Antes de continuar verifique los campos con error";
                    }

                    // Parse seguro usando TryParse con la cultura actual
                    if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out resultado))
                    {
                        // Intentar con InvariantCulture por si el separador viene en otro formato
                        if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out resultado))
                        {
                            mensaje = "Formato numérico inválido";
                            ErrorCampo(campo, mensaje);
                            return "Antes de continuar verifique los campos con error";
                        }
                    }

                    // Normalizar el texto del campo con formato de dos decimales
                    campo.Text = resultado.ToString("0.00", CultureInfo.CurrentCulture);
                }

                // Si es obligatorio y el resultado quedó en 0, marcar error
                if (obligatorio && resultado == 0m)
                {
                    ErrorCampo(campo, "¡Campo obligatorio!");
                    return "Antes de continuar verifique los campos con error";
                }

                // Todo OK: limpiar error y devolver cadena vacía
                ErrorCampo(campo, string.Empty);
                return string.Empty;
            }
            catch (Exception ex)
            {
                var msg = "Error excepcional: " + ex.Message;
                Logger.LogError("ValidarCampoDecimal: " + ex.ToString());
                return "Error excepcional" + ex.Message;
            }
        }

        /// <summary>
        /// Convierte un texto a entero seguro, reportando el error en el parámetro mensaje (firma con ref).
        /// </summary>
        /// <param name="cantidadTexto">Texto a convertir.</param>
        /// <param name="mensaje">Mensaje de salida; se concatena información de error si ocurre.</param>
        /// <returns>Entero convertido o 0 si falla la conversión.</returns>
        private int ObtenerNumero(string cantidadTexto, ref string mensaje)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cantidadTexto))
                {
                    mensaje += "Texto vacío al convertir a número entero.";
                    return 0;
                }

                if (!int.TryParse(cantidadTexto.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out int cantidad))
                {
                    // Intentar con InvariantCulture por si viene con otro separador/región
                    if (!int.TryParse(cantidadTexto.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out cantidad))
                    {
                        mensaje += " Error al convertir las unidades a número entero.";
                        return 0;
                    }
                }

                return cantidad;
            }
            catch (Exception ex)
            {
                var err = "Excepción al convertir a entero: " + ex.ToString();
                Logger.LogError(err);
                Mensajes.MensajeError(err);
                mensaje += " Error inesperado al convertir a número entero.";
                return 0;
            }
        }

        /// <summary>
        /// Convierte un texto a decimal seguro, usando la validación previa y reportando errores en el parámetro mensaje (firma con ref).
        /// </summary>
        /// <param name="cantidadTexto">Texto a convertir.</param>
        /// <param name="mensaje">Mensaje de salida; se concatena información de error si ocurre.</param>
        /// <returns>Decimal convertido o 0 si falla la validación/conversión.</returns>
        private decimal ObtenerDecimal(string cantidadTexto, ref string mensaje)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cantidadTexto))
                {
                    mensaje += "Texto vacío al convertir a decimal.";
                    return 0m;
                }

                // Validar formato decimal usando la función existente que mantiene la firma con ref
                if (!EsNumeroDecimal(cantidadTexto.Trim(), ref mensaje))
                {
                    // EsnumeroDecimal ya dejó el mensaje apropiado
                    return 0m;
                }

                // Intentar parsear con la cultura actual
                if (!decimal.TryParse(cantidadTexto.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal cantidad))
                {
                    // Intentar con InvariantCulture como fallback
                    if (!decimal.TryParse(cantidadTexto.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out cantidad))
                    {
                        mensaje += " Error al convertir el texto a número decimal.";
                        return 0m;
                    }
                }

                return cantidad;
            }
            catch (Exception ex)
            {
                var err = "Excepción al convertir a decimal: " + ex.ToString();
                Logger.LogError(err);
                Mensajes.MensajeError(err);
                mensaje += " Error inesperado al convertir a decimal.";
                return 0m;
            }
        }

        //Validaciones numéricas
        /// <summary>
        /// Wrapper seguro para Validaciones.EsNumeroDecimal manteniendo la firma con ref string.
        /// </summary>
        /// <param name="numero">Texto a validar como número decimal.</param>
        /// <param name="mensaje">Mensaje de salida usado por Validaciones (ref).</param>
        /// <returns>True si el texto representa un número decimal válido; false en caso contrario.</returns>
        private bool EsNumeroDecimal(string numero, ref string mensaje)
        {
            // Inicializar mensaje para evitar valores residuales
            mensaje = mensaje ?? string.Empty;

            // Guard clause: si no hay texto, devolver false y mensaje apropiado
            if (string.IsNullOrWhiteSpace(numero))
            {
                mensaje = "Valor vacío o nulo";
                return false;
            }

            // Delegar a la clase de validaciones (mantiene la firma con ref)
            try
            {
                return Validaciones.EsNumeroDecimal(numero, ref mensaje);
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al validar número decimal: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
                mensaje = "Error al validar número";
                return false;
            }
        }

        //Eventos de validación de campos numéricos
        /// <summary>
        /// Maneja KeyPress para permitir solo dígitos numéricos (enteros).
        /// </summary>
        private void ValidarSoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e == null)
                return;

            try
            {
                e.Handled = !Validaciones.EsDigitoNumerico(e.KeyChar);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error en ValidarSoloNumeros_KeyPress: " + ex.ToString());
                e.Handled = true;
            }
        }

        /// <summary>
        /// Maneja KeyPress para permitir solo dígitos decimales (números con separador decimal).
        /// </summary>
        private void ValidarSoloDecimales_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e == null)
                return;

            try
            {
                e.Handled = !Validaciones.EsDigitoDecimal(e.KeyChar);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error en ValidarSoloDecimales_KeyPress: " + ex.ToString());
                e.Handled = true;
            }
        }

        /// <summary>
        /// Evento Leave para el campo Cantidad: recalcula el costo del insumo y actualiza el monto mostrado.
        /// </summary>
        private void TxtCantidad_Leave(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;

            _costoServicio ??= new CostosServicios();

            try
            {
                string mensaje = string.Empty;
                if (!CalcularCostoInsumo(ref mensaje))
                {
                    Mensajes.MensajeError(string.IsNullOrWhiteSpace(mensaje) ? "No se pudo calcular el costo del insumo." : mensaje);
                    return;
                }

                txtMontoInsumo.Text = _costoServicio.Costo.ToString("0.00", CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                var msg = "Error al procesar Cantidad: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Evento Leave para el campo Unidades: recalcula el costo del insumo y actualiza el monto mostrado.
        /// </summary>
        private void TxtUnidades_Leave(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;

            _costoServicio ??= new CostosServicios();

            try
            {
                string mensaje = string.Empty;
                if (!CalcularCostoInsumo(ref mensaje))
                {
                    Mensajes.MensajeError(string.IsNullOrWhiteSpace(mensaje) ? "No se pudo calcular el costo del insumo." : mensaje);
                    return;
                }

                txtMontoInsumo.Text = _costoServicio.Costo.ToString("0.00", CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                var msg = "Error al procesar Unidades: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Evento Leave del textbox de precio: normaliza el valor y recalcula el margen.
        /// </summary>
        private void TxtPrecio_Leave(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;

            try
            {
                // Normalizar y validar el campo precio usando el helper existente
                decimal valor = 0m;
                string msg = ValidarCampoDecimal(txtPrecio, false, ref valor);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    // Mostrar el mensaje de validación (si corresponde) y no impedir el cálculo posterior
                    Mensajes.MensajeAdvertencia(msg);
                }
                else
                {
                    // Si ValidarCampoDecimal devolvió un valor válido, aseguramos el formato en pantalla
                    txtPrecio.Text = valor.ToString("0.00", CultureInfo.CurrentCulture);
                }

                // Recalcular margen (ObtenerMargen maneja mensajes y excepciones)
                ObtenerMargen();
            }
            catch (Exception ex)
            {
                var err = "Error inesperado al salir del campo Precio: " + ex.ToString();
                Logger.LogError(err);
                Mensajes.MensajeError("Error inesperado\n" + ex.Message);
            }
        }

        /// <summary>
        /// Gatilla el proceso de cálculo de márgenes y gestiona errores técnicos de forma centralizada.
        /// </summary>
        /// <remarks>
        /// Este método es ideal para ser llamado desde eventos de la UI (TextBox_Leave, CheckBox_CheckedChanged).
        /// Los errores de validación de entrada se muestran a través del ErrorProvider gestionado por el método interno.
        /// </remarks>
        private void ObtenerMargen()
        {
            // 1. Cláusulas de guarda: Evitamos cálculos innecesarios si el formulario está en procesos internos
            if (cerrando || cargando) return;

            try
            {
                // 2. Ejecución del cálculo
                // CalcularMargen ya se encarga de: 
                // - Validar los números.
                // - Activar el ErrorProvider en el control correspondiente.
                // - Actualizar el objeto _servicio y el txtMargen.
                if (!CalcularMargen())
                {
                    // Opcional: Si quieres un aviso sonoro o en barra de estado, este es el lugar.
                    // No recomendamos un MensajeError (pop-up) aquí si se llama desde eventos de escritura
                    // para no interrumpir el flujo del usuario.
                }
            }
            catch (Exception ex)
            {
                // 3. Gestión de errores técnicos inesperados
                Logger.LogError($"Error crítico al obtener margen en FrmEditServicios: {ex.Message}");
                Mensajes.MensajeError("Ocurrió un error técnico al intentar procesar los cálculos del servicio.");
            }
        }

        /// <summary>
        /// Maneja el cambio del checkbox de comisión y recalcula el margen si corresponde.
        /// </summary>
        private void CheckComision_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;

            try
            {
                // Evitar recalcular si estamos en proceso de carga
                if (cargando)
                    return;

                ObtenerMargen();
            }
            catch (Exception ex)
            {
                var msg = "Error al procesar cambio de comisión: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error inesperado\n" + ex.Message);
            }
        }

        /// <summary>
        /// Maneja el evento Leave del textbox de comisión y recalcula el margen.
        /// </summary>
        private void TxtComision_Leave(object sender, EventArgs e)
        {
            if (cerrando)
                return;

            try
            {
                // Evitar recalcular si estamos en proceso de carga
                if (cargando)
                    return;

                // Normalizar el texto del control antes de calcular (si aplica)
                if (sender is TextBox tb)
                {
                    var texto = (tb.Text ?? string.Empty).Trim();
                    if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal valor))
                        tb.Text = valor.ToString("0.00", CultureInfo.CurrentCulture);
                }

                ObtenerMargen();
            }
            catch (Exception ex)
            {
                var msg = "Error al procesar salida del campo comisión: " + ex.ToString();
                Logger.LogError(msg);
                Mensajes.MensajeError("Error inesperado\n" + ex.Message);
            }
        }

    }
}
