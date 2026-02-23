using Entidades_SGBM;
using Front_SGBM.UXDesign;
using Negocio_SGBM;
using Utilidades;

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

        /// <summary>
        /// Ejecuta el cierre controlado del formulario.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Establece la bandera global <c>cerrando = true</c> antes de invocar a <see cref="Form.Close"/>.
        /// Esto actúa como un mecanismo de seguridad ("Circuit Breaker") para detener la ejecución
        /// de eventos pendientes en la cola de mensajes de Windows (como SelectionChanged o TextChanged)
        /// y evitar excepciones del tipo <see cref="ObjectDisposedException"/>.
        /// </para>
        /// </remarks>
        private void CerrarFormulario()
        {
            cerrando = true; // Activamos el escudo de seguridad
            this.Close();
        }

        /// <summary>
        /// Evento de carga principal que configura los textos y los datos iniciales.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Nota:</b> La aplicación de estilos visuales es gestionada externamente por el formulario contenedor (Menú Principal),
        /// por lo que no se invoca aquí para evitar redundancia y parpadeos.
        /// </para>
        /// <para>
        /// Pasos de ejecución:
        /// <list type="number">
        /// <item><b>Adaptación de Contexto:</b> Ajusta el texto del botón principal según si se abrió para <see cref="EnumModoForm.Venta"/> o Administración.</item>
        /// <item><b>Carga de Datos:</b> Invoca al orquestador <see cref="CargarFormulario"/> para llenar las grillas y combos.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void FrmAbmServicios_Load(object sender, EventArgs e)
        {
            // 1. Adaptar la interfaz según el propósito de apertura
            // Si es Venta, el botón dice "Seleccionar", si no, "Modificar Servicio".
            btnSeleccionar.Text = (modo == EnumModoForm.Venta)
                ? "Seleccionar"
                : "Modificar Servicio";

            // 2. Iniciar la carga masiva de datos
            CargarFormulario();
        }

        /// <summary>
        /// Orquesta la carga inicial de todos los datos y configuraciones necesarios para el formulario.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método actúa como un inicializador maestro ejecutando las cargas en orden lógico:
        /// <list type="number">
        /// <item><b>Catálogos:</b> Carga las categorías (dependencia para filtros y combos).</item>
        /// <item><b>Datos:</b> Carga el listado principal de servicios.</item>
        /// <item><b>Configuración:</b> Inicializa los campos de búsqueda disponibles.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Se gestiona el cursor de espera (<see cref="Cursors.WaitCursor"/>) para mejorar la experiencia de usuario
        /// durante el proceso de inicialización masiva.
        /// </para>
        /// </remarks>
        private void CargarFormulario()
        {
            // UX: Indicamos al usuario que el sistema está trabajando
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // 1. Carga de dependencias (Combos)
                CargarCategorias();

                // 2. Carga de datos principales (Grilla)
                CargarServicios();

                // 3. Configuración de filtros de búsqueda
                CargarCampos();

            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Error crítico al inicializar los datos del formulario.\nDetalle Técnico:\n{ex.ToString()}");
            }
            finally
            {
                // Siempre restauramos el cursor, ocurra un error o no
                this.Cursor = Cursors.Default;
            }
        }

        //Cargas de datos
        /// <summary>
        /// Carga las categorías de la indole "Servicios" desde la capa de negocio
        /// y las asigna al ComboBox <see cref="cbCategorias"/>.
        /// </summary>
        /// <remarks>
        /// - Utiliza <see cref="CategoriasNegocio.ListarPorIndole(string)"/> que devuelve <see cref="Resultado{T}"/>.
        /// - Si <see cref="Resultado{T}.Success"/> es false se muestra el error y se asigna una lista vacía.
        /// - Si <see cref="Resultado{T}.Mensaje"/> contiene advertencias (aunque Success sea true) se muestran al usuario.
        /// - El flag <c>cargando</c> evita disparar eventos mientras se actualiza el control.
        /// </remarks>
        private void CargarCategorias()
        {
            cargando = true;

            try
            {
                _categoriaSeleccionada = null;
                cbCategorias.DataSource = null;

                // Llamada a la capa de negocio (devuelve Resultado<List<Categorias>>)
                var resultado = CategoriasNegocio.ListarPorIndole("Servicios");

                // Si la capa negocio devolvió un error, mostrarlo y asignar lista vacía
                if (!resultado.Success)
                {
                    Mensajes.MensajeError(resultado.Mensaje);
                    cbCategorias.DataSource = new List<Categorias>();
                    cbCategorias.SelectedIndex = -1;
                    return;
                }

                // Si hay mensaje de advertencia lo mostramos (puede venir en resultado.Mensaje aunque Success sea true)
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);

                // Asignar la lista (si Data es null, usar lista vacía)
                var categorias = resultado.Data ?? new List<Categorias>();
                cbCategorias.DataSource = categorias;
                cbCategorias.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                var msg = "Error inesperado al cargar categorías: " + ex.ToString();
                Mensajes.MensajeError(msg);
                Logger.LogError(msg);
            }
            finally
            {
                cargando = false;
            }
        }

        /// <summary>
        /// Obtiene el listado de servicios desde la capa de negocio y actualiza la fuente de datos de la grilla.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método gestiona el estado de carga (<c>cargando</c>) para evitar eventos de redibujado innecesarios
        /// mientras se procesan los datos. Utiliza el patrón <see cref="Resultado{T}"/> para manejar respuestas exitosas o fallidas.
        /// </para>
        /// <para>
        /// Lógica de filtrado:
        /// <list type="bullet">
        /// <item>Si <c>checkAnulados.Checked</c> es <c>true</c>: Muestra TODOS los servicios (Activos e Inactivos).</item>
        /// <item>Si <c>checkAnulados.Checked</c> es <c>false</c>: Filtra y muestra solo los servicios Activos.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarServicios()
        {
            // Reiniciamos estado visual
            cargando = true;
            _servicioSeleccionado = null;

            // Desvinculamos momentáneamente para evitar errores de pintura
            bindingSourceServicios.DataSource = null;

            try
            {
                // 1. Llamada a la capa de Negocio usando el nuevo patrón Resultado<T>
                // Ya no pasamos 'ref mensaje', recibimos un objeto con toda la info.
                var resultado = ServiciosNegocio.Listar();

                if (!resultado.Success)
                {
                    // Si falló, mostramos el error que viene del Negocio/Datos y salimos.
                    Mensajes.MensajeError(resultado.Mensaje);
                    return;
                }

                // 2. Asignación de la lista maestra
                // Aseguramos que no sea nula (si Data es null, usamos lista vacía)
                _servicios = resultado.Data ?? new List<Servicios>();

                // 3. Aplicación de Filtros de UI
                List<Servicios> listaFiltrada;

                // checkAnulados.Checked == true  -> Quiere ver todo (incluyendo anulados/inactivos)
                // checkAnulados.Checked == false -> Quiere ver solo lo activo
                if (checkAnulados.Checked)
                {
                    listaFiltrada = _servicios;
                }
                else
                {
                    // Filtramos solo los que tienen el estado/flag activo
                    listaFiltrada = _servicios.Where(s => s.Activo).ToList();
                }

                // 4. Advertencias no bloqueantes (Opcional)
                // Si el resultado fue exitoso pero trajo un mensaje (ej: "Listado parcial"), lo mostramos.
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                {
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);
                }

                // 5. Enlace de datos
                bindingSourceServicios.DataSource = listaFiltrada;
                RefrescarGrillaServicios();
            }
            catch (Exception ex)
            {
                // Captura de errores no controlados en la capa de UI (ej: error en el BindingSource)
                Mensajes.MensajeError($"Ocurrió un error inesperado al cargar los servicios.\nDetalle: {ex.Message}");
            }
            finally
            {
                // Siempre liberamos el flag de carga, pase lo que pase
                cargando = false;
            }
        }

        /// <summary>
        /// Aplica el formato visual específico a las columnas numéricas de la grilla de servicios.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método utiliza <see cref="EstiloAplicacion.ApplyFormats"/> para estandarizar la presentación de los datos:
        /// <list type="bullet">
        /// <item><c>costos</c>, <c>precioVenta</c>, <c>margen</c>: Formato Moneda (C2).</item>
        /// <item><c>comision</c>: Formato Porcentaje (P2).</item>
        /// </list>
        /// </para>
        /// <para>
        /// También limpia la selección automática de la primera fila para mejorar la UX.
        /// Se omite la ejecución si la bandera <c>cargando</c> es verdadera.
        /// </para>
        /// </remarks>
        private void RefrescarGrillaServicios()
        {
            // Evitamos formatear si los datos aún se están cargando o vinculando
            if (cargando) return;

            // Diccionario de configuración: "NombreColumna" -> "Formato"
            var formatos = new Dictionary<string, string>
            {
                { "costos", "C2" },
                { "precioVenta", "C2" },
                { "margen", "C2" },
                { "comision", "P2" }
            };

            // Aplicamos estilos visuales centralizados
            EstiloAplicacion.ApplyFormats(dataGridServicios, formatos);

            // Forzamos el redibujado y limpiamos la selección para que no quede azul la primera fila
            dataGridServicios.Refresh();
            dataGridServicios.ClearSelection();
        }

        /// <summary>
        /// Obtiene y muestra los costos (insumos) asociados al servicio actualmente seleccionado.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método realiza las siguientes acciones:
        /// <list type="number">
        /// <item>Valida que exista un servicio seleccionado válido.</item>
        /// <item>Consulta a la capa de Negocio utilizando <see cref="CostosNegocio.ObtenerInsumosPorIdServicio"/>.</item>
        /// <item>Actualiza el <see cref="bindingSourceCostos"/> con la lista recuperada (o una lista vacía si es nula).</item>
        /// <item>Refresca la grilla aplicando los formatos visuales correspondientes.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Si la operación de negocio falla, se muestra un mensaje de error y se detiene la ejecución.
        /// </para>
        /// </remarks>
        private void CargarCostos()
        {
            // 1. Cláusulas de Guarda: Validamos que haya algo seleccionado antes de intentar ir a la BD.
            // Usamos el operador ?. para seguridad por si _servicioSeleccionado es null.
            if (_servicioSeleccionado?.IdServicio == null || _servicioSeleccionado?.IdServicio <= 0)
                return;

            try
            {
                // Limpiamos el origen de datos momentáneamente
                bindingSourceCostos.DataSource = null;

                // 2. Llamada a Negocio con el nuevo patrón Resultado<T>
                // Asumo que el método en negocio ahora firma así: public static Resultado<List<Costos>> ObtenerInsumosPorIdServicio(int id)
                var resultado = CostosNegocio.ObtenerInsumosPorIdServicio((int)_servicioSeleccionado.IdServicio);

                if (!resultado.Success)
                {
                    Mensajes.MensajeError(resultado.Mensaje);
                    return;
                }

                // 3. Asignación de datos (Si Data es null, usamos lista vacía para evitar errores en la grilla)
                _costos = resultado.Data ?? new List<CostosServicios>();

                // 4. Advertencias opcionales (si el proceso fue exitoso pero con observaciones)
                if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                {
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);
                }

                // 5. Enlace y Refresco Visual
                bindingSourceCostos.DataSource = _costos;
                RefrescarGrillaCostos();
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Error inesperado al cargar los costos del servicio.\nDetalle: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza la presentación visual de la grilla de costos, aplicando formatos de moneda.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método actúa como un refresco visual. Incluye una cláusula de guarda para evitar
        /// ejecuciones si la lista subyacente <c>_costos</c> es nula.
        /// </para>
        /// <para>
        /// Se aplica formato de moneda ("C2") específicamente a la columna "costo".
        /// </para>
        /// </remarks>
        private void RefrescarGrillaCostos()
        {
            // Cláusula de guarda: Si la lista de datos es nula, no hay nada que formatear ni refrescar.
            if (_costos == null) return;

            // Aplicamos el formato visual específico para moneda en la columna indicada
            EstiloAplicacion.ApplyCurrencyFormat(dataGridCostos, "costo");

            // Forzamos el redibujado del control para asegurar que los cambios de estilo se apliquen inmediatamente
            dataGridCostos.Refresh();
        }

        /// <summary>
        /// Maneja el evento de cambio de selección en la grilla de servicios (Maestro).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Implementa la lógica del patrón Maestro-Detalle:
        /// <list type="bullet">
        /// <item>Identifica el servicio seleccionado actualmente en el <see cref="bindingSourceServicios"/>.</item>
        /// <item>Actualiza la variable de estado <c>_servicioSeleccionado</c>.</item>
        /// <item>Invoca a <see cref="CargarCostos"/> para actualizar la grilla de detalles (Insumos).</item>
        /// </list>
        /// </para>
        /// <para>
        /// Incluye cláusulas de guarda para evitar ejecuciones innecesarias durante el cierre del formulario
        /// o durante procesos de carga masiva (<c>cargando</c>).
        /// </para>
        /// </remarks>
        /// <param name="sender">El origen del evento (DataGrid).</param>
        /// <param name="e">Los argumentos del evento.</param>
        private void DataGridServicios_SelectionChanged(object sender, EventArgs e)
        {
            // 1. Cláusulas de Guarda
            // Evitamos ejecutar lógica si el formulario se está cerrando o si estamos cargando datos masivamente.
            if (cerrando || cargando) return;

            // Si la lista está vacía o es nula, no hay nada que seleccionar.
            if (_servicios == null || _servicios.Count == 0) return;

            // 2. Obtención Segura del Objeto
            // Usamos Pattern Matching 'is' para verificar el tipo y asignar la variable en un solo paso.
            if (bindingSourceServicios.Current is Servicios servicio)
            {
                _servicioSeleccionado = servicio;

                // 3. Actualización del Detalle
                // Llamamos al método que carga la grilla inferior. 
                // No necesitamos try-catch aquí porque CargarCostos ya maneja sus excepciones internamente.
                CargarCostos();
            }
        }

        //Botones
        /// <summary>
        /// Maneja el evento de clic en el botón "Salir" o "Cancelar".
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método actúa como una barrera de seguridad:
        /// <list type="bullet">
        /// <item>Invoca a <see cref="Mensajes.ConfirmarCierre"/> para solicitar confirmación al usuario.</item>
        /// <item>Solo si el usuario confirma afirmativamente, se procede a llamar a <see cref="CerrarFormulario"/>.</item>
        /// </list>
        /// </para>
        /// Esto previene el cierre accidental y la pérdida de datos no guardados.
        /// </remarks>
        /// <param name="sender">El botón que originó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnSalir_Click(object sender, EventArgs e)
        {
            // Evaluamos directamente la respuesta del usuario
            if (Mensajes.ConfirmarCierre())
                CerrarFormulario();
        }

        /// <summary>
        /// Inicia el proceso de alta de un nuevo servicio delegando la navegación al menú principal.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método busca la instancia activa de <see cref="FrmMenuPrincipal"/> en la colección de formularios abiertos.
        /// Si la encuentra, invoca el método <c>AbrirEditServicios</c> pasando el modo <see cref="EnumModoForm.Alta"/>.
        /// </para>
        /// <para>
        /// <b>Nota de Diseño:</b> Se delega la apertura al formulario padre para que este gestione correctamente
        /// el sistema de paneles/contenedores donde se incrustan las vistas.
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón que originó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                // Intentamos localizar la instancia única del menú principal
                var menuPrincipal = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();

                // Cláusula de guarda defensiva: Si no existe el menú (caso raro), avisamos.
                if (menuPrincipal == null)
                {
                    Mensajes.MensajeError("No se pudo localizar el Menú Principal para gestionar la navegación.");
                    return;
                }

                // Delegamos la acción al contenedor principal
                menuPrincipal.AbrirEditServicios(sender, e, EnumModoForm.Alta);
            }
            catch (Exception ex)
            {
                // Capturamos cualquier error de instanciación o navegación
                Mensajes.MensajeError($"Ocurrió un error crítico al intentar abrir el formulario de registro.\nDetalle: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Gestiona la solicitud de consulta de detalles del servicio seleccionado.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Realiza las siguientes validaciones y acciones:
        /// <list type="number">
        /// <item>Verifica que exista una instancia activa del menú principal.</item>
        /// <item>Valida que el usuario haya seleccionado un registro (<c>_servicioSeleccionado</c> no nulo).</item>
        /// <item>Delega la navegación al menú principal invocando <c>AbrirEditServicios</c> en modo <see cref="EnumModoForm.Consulta"/>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón que originó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Obtener referencia al contenedor principal
                var menuPrincipal = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();

                if (menuPrincipal == null)
                {
                    Mensajes.MensajeError("No se pudo localizar el Menú Principal para realizar la navegación.");
                    return;
                }

                // 2. Validación de Selección
                // Es vital asegurar que tenemos un objeto para mostrar antes de intentar abrir el form.
                if (_servicioSeleccionado == null)
                {
                    Mensajes.MensajeAdvertencia("Debe seleccionar un servicio de la lista para ver sus detalles.");
                    return;
                }

                // 3. Navegación
                // Pasamos el objeto seleccionado para que el formulario hijo lo cargue en sus controles.
                menuPrincipal.AbrirEditServicios(sender, e, EnumModoForm.Consulta, _servicioSeleccionado);
            }
            catch (Exception ex)
            {
                // Nueva regla: Usamos ToString() para obtener el detalle completo (Stack Trace) del error.
                Mensajes.MensajeError($"Ocurrió un error al intentar abrir la consulta.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Gestiona la acción de seleccionar un servicio, ya sea para modificarlo o para agregarlo a una venta.
        /// </summary>
        /// <remarks>
        /// <para>
        /// El comportamiento depende del estado actual (<c>modo</c>) del formulario:
        /// <list type="bullet">
        /// <item>
        ///     <b>Modo No-Venta (Administración):</b> Navega al formulario de edición (<c>AbrirEditServicios</c>) 
        ///     en modo <see cref="EnumModoForm.Modificacion"/> pasando la entidad seleccionada.
        /// </item>
        /// <item>
        ///     <b>Modo Venta:</b> (Pendiente de implementación) Debería retornar el servicio seleccionado 
        ///     al formulario de facturación o agregarlo al carrito.
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón que originó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnSeleccionar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Obtener referencia al contenedor principal
                var menuPrincipal = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();

                if (menuPrincipal == null)
                {
                    Mensajes.MensajeError("No se pudo localizar el Menú Principal.");
                    return;
                }

                // 2. Validación de Selección
                if (_servicioSeleccionado == null)
                {
                    Mensajes.MensajeAdvertencia("Debe seleccionar un servicio antes de continuar.");
                    return;
                }

                // 3. Lógica según el Modo del Formulario
                if (modo != EnumModoForm.Venta)
                {
                    // Caso habitual: Estamos en el ABM y queremos modificar el servicio
                    menuPrincipal.AbrirEditServicios(sender, e, EnumModoForm.Modificacion, _servicioSeleccionado);
                }
                else
                {
                    // TODO: Implementar lógica para retornar el servicio al formulario de Ventas/Carrito.
                    // Probablemente aquí cerremos este form con DialogResult.OK o disparemos un evento.
                    Mensajes.MensajeAdvertencia("La selección para ventas está en desarrollo.");
                }
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error crítico al procesar la selección.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Ejecuta la búsqueda avanzada basada en los criterios y filtros seleccionados actualmente.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este evento actúa como disparador para el método <c>BuscarAvanzado</c>, el cual procesa:
        /// <list type="bullet">
        /// <item>El campo seleccionado (Nombre, Precio, etc.).</item>
        /// <item>El criterio de comparación (Contiene, Mayor a, etc.).</item>
        /// <item>El valor ingresado por el usuario.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón que originó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnBuscar_Click(object sender, EventArgs e) => BuscarAvanzado();

        /// <summary>
        /// Restaura el formulario a su estado inicial, limpiando filtros y recargando el listado completo.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Realiza una limpieza integral de la interfaz:
        /// <list type="bullet">
        /// <item>Restablece los campos de búsqueda mediante <see cref="CargarCampos"/>.</item>
        /// <item>Limpia la selección del filtro de categorías.</item>
        /// <item>Elimina cualquier alerta visual del <see cref="ErrorProvider"/>.</item>
        /// <item>Recarga la grilla de servicios llamando a <see cref="CargarServicios"/>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón que originó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // 1. Reset de Controles de Filtro
            CargarCampos(); // Reinicia el combo de campos y su lógica asociada
            cbCategorias.SelectedIndex = -1; // Quita el filtro de categoría específica

            // 2. Limpieza de Validaciones Visuales
            // Es importante borrar los iconos rojos de error para no confundir al usuario al reiniciar.
            errorProvider1.Clear();

            // 3. Recarga de Datos
            // Trae nuevamente la lista completa desde la base de datos sin filtros aplicados.
            CargarServicios();
        }

        //Carga de campos y criterios

        /// <summary>
        /// Configura el origen de datos del ComboBox de campos de búsqueda/filtrado.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método realiza la vinculación de datos:
        /// <list type="number">
        /// <item>Valida que la lista de origen <c>campos</c> no sea nula.</item>
        /// <item>Asigna la lista al <see cref="ComboBox.DataSource"/>.</item>
        /// <item>Establece el <see cref="ComboBox.SelectedIndex"/> en -1 para que no haya ninguna opción preseleccionada visualmente.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarCampos()
        {
            try
            {
                // Cláusula de guarda: Si la lista no está inicializada, no intentamos vincular nada.
                if (campos == null) return;

                // Asignación de la fuente de datos
                cbCampos.DataSource = campos;

                // Reseteo de selección (UX: El usuario ve el combo vacío o el texto placeholder)
                cbCampos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Aplicada la regla de ex.ToString() para detalle técnico completo
                Mensajes.MensajeError($"Error inesperado al cargar los campos de filtro.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Carga la lista de criterios de búsqueda (operadores) según el tipo de dato del campo seleccionado.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Lógica de Negocio:</b>
        /// <list type="bullet">
        /// <item><b>Campos de Texto:</b> (Ej: Nombre, Descripción) Asigna operadores como "Contiene", "Comienza con", etc.</item>
        /// <item><b>Campos Numéricos:</b> (Ej: Precio, Stock) Asigna operadores matemáticos como "Mayor a", "Igual a", etc.</item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Efectos en UI:</b>
        /// Al cambiar los criterios, se reinicia y deshabilita el cuadro de búsqueda (<c>txtBusqueda</c>)
        /// y el botón de buscar para obligar al usuario a redefinir su consulta.
        /// </para>
        /// </remarks>
        /// <param name="campo">El nombre del campo seleccionado en el combo de filtros.</param>
        private void CargarCriterios(string? campo)
        {
            cargando = true; // Bloqueamos eventos en cascada
            try
            {
                // 1. Inicialización de listas
                // Siempre instanciamos una lista nueva para evitar referencias nulas
                criterios = new List<string>();

                // 2. Determinación del Tipo de Dato
                if (string.IsNullOrWhiteSpace(campo))
                {
                    // Si no hay campo, la lista queda vacía
                    buscarNumerico = false;
                }
                else if (campo.Contains("descrip", StringComparison.OrdinalIgnoreCase) ||
                         campo.Contains("nombre", StringComparison.OrdinalIgnoreCase))
                {
                    // Lógica para cadenas de texto
                    criterios.AddRange(new[] { "Contiene", "Comienza con", "Termina con", "Igual a", "No Contiene" });
                    buscarNumerico = false;
                }
                else
                {
                    // Lógica por defecto para números (Precios, IDs, Stock)
                    criterios.AddRange(new[] { "Mayor a", "Menor a", "Igual a", "Mayor o Igual a", "Menor o Igual a" });
                    buscarNumerico = true;
                }

                // 3. Limpieza de Errores Visuales
                // Si llegamos aquí, es porque seleccionó un campo válido, borramos el error rojo si existía.
                if (cbCampos.SelectedIndex != -1)
                {
                    SetError(cbCampos, "");
                }

                // 4. Actualización de la Interfaz (Data Binding)
                cbCriterios.DataSource = null; // Limpiamos primero para evitar conflictos de índices
                cbCriterios.DataSource = criterios;
                cbCriterios.SelectedIndex = -1; // Sin selección inicial

                // 5. Gestión de Estado de Controles Dependientes
                cbCriterios.Enabled = criterios.Count > 0;

                // Reseteamos el input de texto porque el criterio cambió
                txtBusqueda.Clear();
                txtBusqueda.Enabled = false;
                btnBuscar.Enabled = false;
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Error inesperado al cargar los criterios de búsqueda.\nDetalle Técnico:\n{ex.ToString()}");
            }
            finally
            {
                cargando = false; // Liberamos la UI
            }
        }

        //Combos
        /// <summary>
        /// Gestiona el cambio de campo de búsqueda para actualizar los criterios disponibles.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este evento detecta qué columna eligió el usuario (ej: "Nombre", "Precio") e invoca a
        /// <see cref="CargarCriterios"/> para llenar el combo de operadores con las opciones lógicas correspondientes
        /// (Texto vs Numérico).
        /// </para>
        /// <para>
        /// Incluye protección contra ejecución durante la carga inicial o cierre del formulario.
        /// </para>
        /// </remarks>
        /// <param name="sender">El ComboBox de campos.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void CbCampos_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cláusula de Guarda:
            // 1. cerrando: Evita errores al cerrar el form.
            // 2. cargando: CRUCIAL. Evita que este código corra cuando asignamos el DataSource en el Load.
            if (cerrando || cargando) return;

            try
            {
                // Obtenemos el texto seleccionado y delegamos la lógica de negocio
                string campoSeleccionado = cbCampos.Text;
                CargarCriterios(campoSeleccionado);
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Error al procesar el cambio de campo.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Gestiona la habilitación de los controles de búsqueda según la selección de un criterio.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este evento responde a la selección de un operador (ej: "Mayor a", "Contiene"):
        /// <list type="bullet">
        /// <item>Si se selecciona un criterio válido, habilita la caja de texto <c>txtBusqueda</c> y el botón <c>btnBuscar</c>.</item>
        /// <item>Si no hay selección, deshabilita estos controles para guiar el flujo del usuario.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Incluye protección contra ejecución durante la carga de datos (<c>cargando</c>).
        /// </para>
        /// </remarks>
        /// <param name="sender">El ComboBox de criterios.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void CbCriterios_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cláusula de guarda: Evitamos ejecución durante el cierre o la carga de datos (DataBinding)
            if (cerrando || cargando) return;

            try
            {
                // Determinamos si hay un criterio válido seleccionado
                bool habilitar = !string.IsNullOrWhiteSpace(cbCriterios.Text);

                // Aplicamos el estado a los controles dependientes
                txtBusqueda.Enabled = habilitar;
                btnBuscar.Enabled = habilitar;

                // UX: Si se habilitó, podríamos limpiar el texto anterior o poner el foco (Opcional)
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Error al actualizar el estado de los controles de búsqueda.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Actualiza la variable de estado local cuando el usuario selecciona una categoría distinta.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método mantiene sincronizada la variable <c>_categoriaSeleccionada</c> con la interfaz visual.
        /// Utiliza el operador <c>as</c> para realizar una conversión segura:
        /// <list type="bullet">
        /// <item>Si hay un objeto seleccionado, lo asigna.</item>
        /// <item>Si la selección está vacía o es nula, asigna <c>null</c> automáticamente.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Incluye cláusulas de guarda para evitar actualizaciones durante la carga inicial o el cierre.
        /// </para>
        /// </remarks>
        /// <param name="sender">El ComboBox de categorías.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void CbCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cláusula de guarda: Evitamos lógica innecesaria durante la carga masiva o el cierre.
            if (cerrando || cargando) return;

            try
            {
                // Sintaxis moderna y segura:
                // Intentamos tratar el item como 'Categorias'. Si es null o no es del tipo, devuelve null.
                _categoriaSeleccionada = cbCategorias.SelectedItem as Categorias;
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Error al actualizar la selección de categoría.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Establece o borra un mensaje de validación visual asociado a un control.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método encapsula la interacción con el <see cref="ErrorProvider"/> del formulario.
        /// </para>
        /// <para>
        /// <b>Comportamiento:</b>
        /// <list type="bullet">
        /// <item>Si <paramref name="error"/> tiene texto, muestra el icono de error rojo junto al control.</item>
        /// <item>Si <paramref name="error"/> es <c>string.Empty</c> o nulo, limpia el error y oculta el icono.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="campo">El control de interfaz (TextBox, ComboBox, etc.) a validar.</param>
        /// <param name="error">El mensaje de error a mostrar o cadena vacía para limpiar.</param>
        private void SetError(Control campo, string error) => errorProvider1.SetError(campo, error);

        //Métodos de búsqueda y filtros
        /// <summary>
        /// Filtra el listado de servicios en memoria según el texto ingresado y el estado de los registros.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Reglas de Negocio:</b>
        /// <list type="bullet">
        /// <item><b>Filtro de Estado:</b> Si <c>checkAnulados</c> está marcado, muestra todos. Si no, solo los Activos.</item>
        /// <item><b>Filtro de Texto:</b> Solo se aplica si el texto tiene 3 o más caracteres.</item>
        /// <item><b>Búsqueda:</b> Es insensible a mayúsculas/minúsculas (OrdinalIgnoreCase).</item>
        /// </list>
        /// </para>
        /// <para>
        /// Se gestionan las banderas de carga (<c>cargando</c>) para evitar parpadeos o eventos indeseados.
        /// </para>
        /// </remarks>
        /// <param name="nombre">El texto a buscar dentro del nombre del servicio.</param>
        private void FiltroRapidoNombre(string nombre)
        {
            cargando = true;
            try
            {
                // 1. Limpieza de Selección Previa
                // Al filtrar, la posición de los índices cambia, por lo que debemos limpiar la selección actual y sus detalles.
                _costos = null;
                _servicioSeleccionado = null;
                bindingSourceServicios.DataSource = null;

                // Cláusula de guarda: Si no hay datos base, no hacemos nada.
                if (_servicios == null) return;

                // 2. Inicio de la consulta LINQ (Query diferida)
                IEnumerable<Servicios> query = _servicios;

                // 3. Aplicar Filtro de Estado (Activos / Inactivos)
                // Lógica simplificada: Si NO quiere ver anulados, filtramos solo los activos.
                // Si quiere ver anulados (Checked = true), no aplicamos ningún filtro de estado (pasan todos).
                if (!checkAnulados.Checked)
                {
                    query = query.Where(s => s.Activo);
                }

                // 4. Aplicar Filtro de Texto (Nombre)
                // Solo filtramos si escribió algo coherente (3 caracteres o más) para evitar resultados irrelevantes.
                if (!string.IsNullOrWhiteSpace(nombre) && nombre.Length >= 3)
                {
                    query = query.Where(s => s.NombreServicio.Contains(nombre, StringComparison.OrdinalIgnoreCase));
                }

                // 5. Materialización y Enlace
                bindingSourceServicios.DataSource = query.ToList();

                RefrescarGrillaServicios();
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Error inesperado al aplicar el filtro rápido.\nDetalle Técnico:\n{ex.ToString()}");
            }
            finally
            {
                // El bloque finally garantiza que la UI se desbloquee incluso si hubo error.
                cargando = false;
            }
        }

        /// <summary>
        /// Ejecuta el filtrado en tiempo real a medida que el usuario escribe en la caja de búsqueda rápida.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este evento captura cada pulsación de tecla y delega la lógica al método <see cref="FiltroRapidoNombre"/>.
        /// </para>
        /// <para>
        /// Incluye protección (<c>cerrando</c> || <c>cargando</c>) para evitar filtrados innecesarios 
        /// si el texto cambia programáticamente durante la carga o cierre del formulario.
        /// </para>
        /// </remarks>
        /// <param name="sender">La caja de texto de filtro.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void TxtFiltroRapido_TextChanged(object sender, EventArgs e)
        {
            // Cláusula de guarda estándar
            if (cerrando || cargando) return;

            // Invocamos el filtro con el texto actual
            FiltroRapidoNombre(txtFiltroRapido.Text);
        }

        /// <summary>
        /// Ejecuta la consulta avanzada de servicios contra la base de datos y actualiza la grilla.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de Ejecución:</b>
        /// <list type="number">
        /// <item><b>Validación:</b> Verifica que los campos requeridos estén completos (<see cref="ValidarBusqueda"/>).</item>
        /// <item><b>Preparación:</b> Limpia la grilla y bloquea la UI (<c>WaitCursor</c>).</item>
        /// <item><b>Consulta:</b> Invoca a la capa de negocio enviando campo, criterio, valor y categoría.</item>
        /// <item><b>Respuesta:</b> Procesa el objeto <see cref="Resultado{T}"/>. Si falla, muestra error.</item>
        /// <item><b>Visualización:</b> Si tiene éxito, actualiza la lista en memoria <c>_servicios</c> y llama a 
        /// <see cref="FiltroRapidoNombre"/> para aplicar los filtros locales y enlazar los datos.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void BuscarAvanzado()
        {
            // 1. Validación de Entrada (Fail-Fast)
            // Usamos el texto crudo para validar
            string valorIngresado = txtBusqueda.Text.Trim();
            if (!ValidarBusqueda(valorIngresado)) return;

            // 2. Preparación de UI
            cargando = true;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Limpiamos referencias visuales previas
                bindingSourceServicios.DataSource = null;
                _servicios = null;

                // 3. Normalización de Parámetros
                // Convertimos a minúsculas para coincidir con la lógica esperada por el backend (si aplica)
                string campo = cbCampos.Text.Trim().ToLower();
                string criterio = cbCriterios.Text.Trim().ToLower();
                string valor = valorIngresado.ToLower(); // Asumimos búsqueda case-insensitive

                // Manejo seguro de nulos para la categoría (0 = Todas/Ignorar)
                int idCategoria = _categoriaSeleccionada?.IdCategoria ?? 0;

                // 4. Llamada a Negocio (Patrón Resultado<T>)
                var resultado = ServiciosNegocio.BuscarServiciosAvanzado(campo, criterio, valor, idCategoria);

                if (!resultado.Success)
                {
                    // Si la consulta falló en la BD (ej: Timeout, Error SQL)
                    Mensajes.MensajeError(resultado.Mensaje);

                    // Inicializamos lista vacía para evitar nulos en pasos siguientes
                    _servicios = new List<Servicios>();
                }
                else
                {
                    // Éxito: Guardamos los datos en memoria
                    _servicios = resultado.Data ?? new List<Servicios>();

                    // Advertencia opcional si el negocio devolvió un mensaje (ej: "Búsqueda truncada a 100 reg")
                    if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
                    {
                        Mensajes.MensajeAdvertencia(resultado.Mensaje);
                    }
                }

                // 5. Enlace de Datos y Filtros Locales
                // Reutilizamos el filtro rápido para aplicar:
                // a) El filtro de "Anulados/Activos" (Checkbox)
                // b) Cualquier texto que haya quedado en la barra de filtro rápido (aunque usualmente estará vacía)
                // c) El refresco de la grilla (DataSource)
                FiltroRapidoNombre(txtFiltroRapido.Text);

            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error inesperado durante la búsqueda avanzada.\nDetalle Técnico:\n{ex.ToString()}");
            }
            finally
            {
                // 6. Restauración de Estado
                cargando = false;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Verifica que todos los criterios necesarios para realizar una búsqueda avanzada estén completos y sean válidos.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Realiza las siguientes validaciones secuenciales (Fail-Fast):
        /// <list type="number">
        /// <item><b>Campo:</b> Debe haber una columna seleccionada en <c>cbCampos</c>.</item>
        /// <item><b>Criterio:</b> Debe haber un operador lógico seleccionado en <c>cbCriterios</c>.</item>
        /// <item><b>Valor:</b> El texto de búsqueda no puede estar vacío.</item>
        /// <item><b>Tipo de Dato:</b> Si la búsqueda es numérica, valida el formato decimal.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Si alguna validación falla, muestra el mensaje correspondiente y activa el <see cref="ErrorProvider"/> en el control afectado.
        /// </para>
        /// </remarks>
        /// <param name="valorBuscado">El texto ingresado por el usuario para buscar.</param>
        /// <returns><c>true</c> si todos los datos son válidos para proceder; de lo contrario, <c>false</c>.</returns>
        private bool ValidarBusqueda(string valorBuscado)
        {
            // 1. Limpieza inicial: Borramos errores visuales de intentos anteriores
            errorProvider1.Clear();

            // 2. Validación de Campo (Columna)
            if (cbCampos.SelectedIndex < 0)
            {
                Mensajes.MensajeError("Por favor, seleccione el campo por el cual desea buscar.");
                SetError(cbCampos, "Campo requerido");
                return false;
            }

            // 3. Validación de Criterio (Operador)
            if (cbCriterios.SelectedIndex < 0)
            {
                Mensajes.MensajeError("Por favor, seleccione un criterio de comparación (ej: Contiene, Igual a).");
                SetError(cbCriterios, "Criterio requerido");
                return false;
            }

            // 4. Validación de Valor (Texto)
            if (string.IsNullOrWhiteSpace(valorBuscado))
            {
                Mensajes.MensajeError("Debe ingresar un valor para realizar la búsqueda.");
                SetError(txtBusqueda, "Valor requerido");
                txtBusqueda.Focus();
                return false;
            }

            // 5. Validación de Formato Numérico (Si aplica)
            if (buscarNumerico)
            {
                string mensajeError = string.Empty;
                // Asumimos que Validaciones.EsNumeroDecimal devuelve false si falla y llena el mensaje
                if (!Validaciones.EsNumeroDecimal(valorBuscado, ref mensajeError))
                {
                    Mensajes.MensajeError($"El valor ingresado no tiene un formato numérico válido.\n{mensajeError}");
                    SetError(txtBusqueda, "Formato numérico incorrecto");
                    return false;
                }
            }

            // Si pasó todas las barreras, es válido.
            return true;
        }

        /// <summary>
        /// Controla la entrada de texto para permitir solo números cuando el criterio es numérico.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este evento se dispara con cada tecla presionada en la caja de búsqueda:
        /// <list type="bullet">
        /// <item>Si <c>buscarNumerico</c> es falso, permite cualquier caracter.</item>
        /// <item>Si es verdadero, delega la validación a <see cref="Validaciones.EsDigitoDecimal"/> 
        /// para bloquear letras o símbolos no válidos.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">La caja de texto de búsqueda.</param>
        /// <param name="e">Argumentos de la tecla presionada.</param>
        private void TxtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si estamos buscando por texto (Nombre, Descripción), no restringimos nada.
            if (!buscarNumerico) return;

            // Si buscamos por Precio/Stock, solo dejamos pasar números y control (Backspace).
            e.Handled = !Validaciones.EsDigitoDecimal(e.KeyChar);
        }

        /// <summary>
        /// Refresca el listado de servicios cuando se cambia el estado del filtro de "Anulados".
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este evento invoca nuevamente al filtro rápido conservando el texto que el usuario
        /// haya escrito en <c>txtFiltroRapido</c>, para aplicar o quitar la restricción de registros inactivos.
        /// </para>
        /// <para>
        /// Incluye protección contra ejecución durante la carga inicial (<c>cargando</c>).
        /// </para>
        /// </remarks>
        /// <param name="sender">El checkbox "Ver Anulados".</param>
        /// <param name="e">Argumentos del evento.</param>
        private void CheckAnulados_CheckedChanged(object sender, EventArgs e)
        {
            // Cláusula de guarda: Evita disparos accidentales durante la carga del Form
            if (cerrando || cargando) return;

            // Reutilizamos la lógica centralizada de filtrado.
            // No hace falta try-catch aquí porque FiltroRapidoNombre ya lo tiene internamente.
            FiltroRapidoNombre(txtFiltroRapido.Text);
        }
    }
}
