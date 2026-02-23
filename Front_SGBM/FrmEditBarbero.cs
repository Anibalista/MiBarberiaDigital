using Entidades_SGBM;
using Negocio_SGBM;
using Utilidades;

namespace Front_SGBM
{
    public partial class FrmEditBarbero : Form
    {
        #region Declaraciones iniciales

        #region Variables y Formulario

        /// <summary>
        /// Define el modo actual del formulario (Alta o Modificación).
        /// </summary>
        public EnumModoForm modo = EnumModoForm.Alta;

        // Listas principales utilizadas para cargar datos en el formulario.
        public List<Contactos>? _contactos = null;
        private List<Provincias>? _provincias = null;
        private List<Localidades>? _localidades = null;
        private List<Estados>? _estados = null;
        private List<string> _tiposEmpleados = new List<string> { "Barbero", "Cajero", "Encargado" };

        // Objetos importantes asociados al barbero.
        public Personas? _persona = null;
        public Empleados? _barbero = null;
        private Domicilios? _domicilio = null;
        private Provincias? _provincia = null;
        private Localidades? _localidad = null;
        private Estados? _estado = null;

        // Flags de control (se manejan internamente en los métodos).
        public bool cerrando = false;
        public bool editandoContactos = false;
        public bool cargando = false;

        /// <summary>
        /// Constructor del formulario de edición de barbero.
        /// Inicializa los componentes y configura las fechas.
        /// </summary>
        public FrmEditBarbero()
        {
            InitializeComponent();
            IniciarFechas(); // Configura límites de fechas al iniciar.
        }

        /// <summary>
        /// Evento de carga del formulario.
        /// Ejecuta la carga inicial de datos y configura fechas.
        /// </summary>
        private void FrmEditBarbero_Load(object sender, EventArgs e)
        {
            CargarFormulario(); // Carga datos iniciales según el modo.
            IniciarFechas();    // Reaplica límites de fechas.
        }

        #endregion

        #region Cargas de valores

        /// <summary>
        /// Limpia las referencias de los objetos principales del formulario,
        /// reiniciando las variables a <c>null</c>.
        /// </summary>
        /// <remarks>
        /// - Restablece las variables internas (_barbero, _persona, _estado, _domicilio).
        /// - Si <paramref name="limpiarContactos"/> es true, reinicia la lista de contactos.
        /// - Se utiliza para preparar el formulario antes de cargar nuevos datos o al resetear.
        /// </remarks>
        /// <param name="limpiarContactos">
        /// Indica si también se deben limpiar los contactos asociados.
        /// </param>
        private void LimpiarValores(bool limpiarContactos)
        {
            // Se limpian las referencias principales
            _barbero = null;
            _persona = null;
            _estado = null;
            _domicilio = null;

            // Si corresponde, se reinicia la colección de contactos
            if (limpiarContactos)
                _contactos = null;

            // Nota de mejora: se podría centralizar en un helper para mantener consistencia.
        }

        /// <summary>
        /// Configura el formulario de barbero según el modo actual (registro o modificación).
        /// Asigna el título correspondiente y carga los datos iniciales necesarios.
        /// </summary>
        private void CargarFormulario()
        {
            // Determina el título según el modo del formulario.
            string titulo = modo == EnumModoForm.Modificacion ? "Modificación" : "Registro";

            // Asigna el título al label principal.
            labelTitulo.Text = $"{titulo} de Barbero";

            // Inicializa la fecha en los controles correspondientes.
            FechaSeleccionadaGenerica();

            // Si el modo es modificación, carga los datos existentes del empleado.
            if (modo == EnumModoForm.Modificacion)
            {
                CargarDatosEmpleado(); // Carga datos del barbero.
                CargarContactos();     // Carga contactos asociados.
            }

            // Carga las listas de estados, tipos de empleados, provincias y localidades.
            CargarEstados();
            CargarTiposEmpleado();
            CargarProvincias();
            CargarLocalidades();
        }

        /// <summary>
        /// Inicializa los límites de fechas para nacimiento e ingreso.
        /// </summary>
        private void IniciarFechas()
        {
            DateTime fecha = DateTime.Now.AddDays(1); // Fecha máxima: mañana.
            dateTimeNacimiento.MaxDate = fecha;
            dateTimeNacimiento.MinDate = fecha.AddYears(-99); // Fecha mínima: 99 años atrás.
            dateTimeIngreso.MaxDate = fecha; // Fecha máxima de ingreso: mañana.
        }

        /// <summary>
        /// Configura valores por defecto en los controles de fecha.
        /// </summary>
        private void FechaSeleccionadaGenerica()
        {
            dateTimeNacimiento.Value = DateTime.Today; // Fecha de nacimiento: hoy.
            dateTimeIngreso.Value = DateTime.Now;      // Fecha de ingreso: ahora.
        }

        /// <summary>
        /// Carga la lista de estados para la índole "Empleados" gestionando la respuesta con el patrón Resultado.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Pasos de ejecución:</b>
        /// <list type="number">
        /// <item>Reinicia el binding y las referencias visuales previas.</item>
        /// <item>Consulta los estados a la capa de negocio mediante <see cref="EstadosNegocio.getEstadosPorIndole"/>.</item>
        /// <item><b>Fallback (Respaldo):</b> Si la consulta falla o devuelve nulo, crea un estado "Activo" por defecto, 
        /// lo enlaza a la UI de forma segura y deshabilita la selección para prevenir inconsistencias.</item>
        /// <item><b>Éxito:</b> Enlaza los datos reales, habilita el combo y delega la selección a <c>SeleccionarEstado</c>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarEstados()
        {
            cargando = true; // Bloqueamos eventos dependientes de la carga

            try
            {
                // 1. Limpieza inicial para evitar datos residuales
                bindingEstados.DataSource = null;
                _estado = null;
                _estados = null;

                // 2. Consulta a la capa de negocio (Patrón Resultado)
                var resultado = EstadosNegocio.GetEstadosPorIndole("Empleados");

                // 3. Evaluación de Errores o Lista Vacía
                if (!resultado.Success || resultado.Data == null || resultado.Data.Count == 0)
                {
                    // Creamos el estado mínimo en memoria
                    _estados = new List<Estados>
                    {
                        new Estados { Indole = "Empleados", Estado = "Activo", IdEstado = 0 }
                    };

                    // ¡CORRECCIÓN!: Enlazamos la lista a la interfaz antes de salir
                    bindingEstados.DataSource = _estados;

                    // Deshabilitamos el combo porque es un dato ficticio de rescate
                    cbEstados.Enabled = false;

                    // Mostramos el mensaje devuelto por el servidor o uno genérico
                    Mensajes.MensajeError(resultado.Mensaje ?? "No se encontraron estados configurados para Empleados.");
                    return;
                }

                // 4. Flujo Normal (Éxito)
                _estados = resultado.Data;
                bindingEstados.DataSource = _estados;

                SeleccionarEstado();

                // Habilitamos el combo si hay opciones válidas
                cbEstados.Enabled = bindingEstados.Count > 0;
            }
            catch (Exception ex)
            {
                // Registro del error técnico en el archivo Log
                Logger.LogError($"Error crítico en CargarEstados (Barberos): {ex.ToString()}");

                // Degradación elegante de la UI
                cbEstados.Enabled = false;
                Mensajes.MensajeError("Ocurrió un error inesperado al intentar cargar la lista de estados.");
            }
            finally
            {
                cargando = false; // Liberamos los eventos
            }
        }

        /// <summary>
        /// Carga la lista de provincias utilizando el patrón Resultado y la asigna al binding de la interfaz.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Pasos de ejecución:</b>
        /// <list type="number">
        /// <item>Bloquea eventos de la UI activando la bandera <c>cargando</c>.</item>
        /// <item>Limpia los orígenes de datos previos para evitar inconsistencias visuales.</item>
        /// <item>Consulta las provincias a la capa de negocio mediante <see cref="DomiciliosNegocio.GetProvincias"/>.</item>
        /// <item><b>Fallback:</b> Si falla la consulta o no hay datos, crea una lista de rescate con "Entre Ríos" por defecto.</item>
        /// <item>Si el barbero (empleado) ya tiene una localidad asignada en memoria, pre-selecciona la provincia correspondiente.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarProvincias()
        {
            cargando = true; // Flag para evitar que los eventos SelectedIndexChanged se disparen prematuramente

            try
            {
                // 1. Reinicio de bindings y variables de estado
                bindingProvincias.DataSource = null;
                _provincias = null;
                _provincia = null;

                // 2. Llamada a la capa de negocio (Patrón Resultado<T>)
                var resultado = DomiciliosNegocio.GetProvincias();

                // 3. Manejo del resultado y Fallback (Respaldo)
                if (!resultado.Success || resultado.Data == null || resultado.Data.Count == 0)
                {
                    // Fallback de seguridad: Creamos "Entre Ríos" como genérico para que la UI no explote.
                    // Es buena práctica asignarle un ID (ej: 0) para que el SelectedValue del ComboBox no falle.
                    _provincias = new List<Provincias>
                    {
                        new Provincias { Provincia = "Entre Ríos", IdProvincia = 0 }
                    };

                    // Informamos al usuario si la falla vino acompañada de un mensaje del servidor
                    if (!resultado.Success)
                    {
                        Mensajes.MensajeError(resultado.Mensaje ?? "No se pudieron cargar las provincias desde la base de datos.");
                    }
                }
                else
                {
                    // Éxito: asignamos los datos reales provenientes de la BD
                    _provincias = resultado.Data;
                }

                // 4. Enlace a la interfaz
                bindingProvincias.DataSource = _provincias;

                // 5. Pre-selección de provincia (Si estamos editando un barbero que ya tiene localidad)
                if (_localidad != null)
                {
                    _provincia = _provincias.FirstOrDefault(p => p.IdProvincia == _localidad.IdProvincia);
                }

                // 6. Configuración visual del ComboBox
                if (_provincia != null)
                {
                    cbProvincia.SelectedValue = _provincia.IdProvincia;
                }
                else
                {
                    // Deja el combo en blanco si no hay coincidencia, obligando al usuario a elegir
                    cbProvincia.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                // Registro del error completo en el log para diagnóstico posterior
                Logger.LogError($"Error crítico en CargarProvincias (Barberos): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado al intentar inicializar las provincias.");
            }
            finally
            {
                cargando = false; // Reactiva los eventos de UI
            }
        }

        /// <summary>
        /// Carga las localidades correspondientes a la provincia seleccionada utilizando el patrón Resultado.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de ejecución:</b>
        /// <list type="number">
        /// <item>Limpia el origen de datos de localidades y bloquea los eventos visuales.</item>
        /// <item>Valida que haya una provincia válida seleccionada o escrita en el combo.</item>
        /// <item>Consulta las localidades a la capa de negocio mediante <see cref="DomiciliosNegocio.GetLocalidadesPorProvincia"/>.</item>
        /// <item>Evalúa el <c>Resultado</c>: Si falla, inicializa una lista vacía y notifica el error. Si tiene éxito, enlaza los datos reales.</item>
        /// <item>Pre-selecciona la localidad en memoria (si se está editando un registro existente).</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarLocalidades()
        {
            // ¡CORRECCIÓN!: Faltaba activar la bandera al inicio para bloquear los eventos SelectedIndexChanged
            cargando = true;

            try
            {
                // 1. Reinicio de binding y referencias
                bindingLocalidades.DataSource = null;
                _localidades = null;

                // 2. Validaciones tempranas (Cláusulas de guarda)
                // Si no hay provincias cargadas en memoria, no podemos buscar localidades
                if (_provincias == null || _provincias.Count < 1)
                {
                    cbLocalidad.Enabled = false;
                    return;
                }

                string nombreProvincia = cbProvincia.Text.Trim();

                // Si el combo está vacío, limpiamos y cortamos la ejecución
                if (string.IsNullOrWhiteSpace(nombreProvincia))
                {
                    cbLocalidad.Enabled = false;
                    return;
                }

                // 3. Búsqueda de la provincia en la lista en memoria
                _provincia = _provincias.FirstOrDefault(p =>
                    p.Provincia.Equals(nombreProvincia, StringComparison.OrdinalIgnoreCase));

                // Si el usuario escribió una provincia que no existe en la lista
                if (_provincia == null)
                {
                    cbLocalidad.Enabled = false;
                    _localidades = new List<Localidades>();
                    bindingLocalidades.DataSource = _localidades;
                    return;
                }

                // 4. Llamada a la capa de negocio (Patrón Resultado)
                var resultado = DomiciliosNegocio.GetLocalidadesPorProvincia(_provincia);

                // 5. Manejo de la respuesta
                if (!resultado.Success || resultado.Data == null)
                {
                    // Fallback: Dejamos la lista vacía para no romper el DataBinding
                    _localidades = new List<Localidades>();

                    if (!resultado.Success)
                    {
                        Mensajes.MensajeError(resultado.Mensaje ?? $"No se pudieron cargar las localidades para {_provincia.Provincia}.");
                    }
                }
                else
                {
                    // Éxito: Asignamos los datos reales
                    _localidades = resultado.Data;
                }

                // 6. Enlace a la interfaz
                bindingLocalidades.DataSource = _localidades;

                // Habilitamos el combo solo si la provincia tiene localidades asociadas
                cbLocalidad.Enabled = _localidades.Count > 0;

                // 7. Configuración visual (Pre-selección)
                if (_localidad != null && cbLocalidad.Enabled)
                {
                    cbLocalidad.SelectedValue = _localidad.IdLocalidad;
                }
                else
                {
                    cbLocalidad.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                // Registro del error completo en el log para diagnóstico
                Logger.LogError($"Error crítico en CargarLocalidades (Barberos): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado al intentar cargar las localidades.");

                // Degradación elegante
                cbLocalidad.Enabled = false;
            }
            finally
            {
                // Liberamos el flag para que la UI vuelva a reaccionar a los eventos del usuario
                cargando = false;
            }
        }

        /// <summary>
        /// Carga los tipos de empleado en el combo,
        /// asegurando que el tipo del barbero actual esté presente en la lista.
        /// </summary>
        /// <remarks>
        /// - Reinicia la fuente de datos del combo.
        /// - Si el barbero tiene un tipo definido y no está en la lista, lo inserta al inicio.
        /// - Asigna la lista final al combo y selecciona el primer elemento o el tipo del empleado.
        /// - En caso de error, registra el log y deshabilita el control.
        /// </remarks>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la carga y se registra en el log.
        /// </exception>
        private void CargarTiposEmpleado()
        {
            // Reinicia la fuente de datos del combo antes de asignar
            cbTipo.DataSource = null;

            try
            {
                // Si el barbero tiene un tipo definido y no está en la lista, se agrega al inicio
                if (_barbero?.TipoEmpleado != null && !_tiposEmpleados.Contains(_barbero.TipoEmpleado))
                    _tiposEmpleados.Insert(0, _barbero.TipoEmpleado);

                // Asigna la lista de tipos de empleado al combo
                cbTipo.DataSource = _tiposEmpleados;

                // Selecciona el primer elemento o el tipo del empleado
                cbTipo.SelectedItem = _barbero?.TipoEmpleado ?? _tiposEmpleados.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Registra el error en el log para diagnóstico
                Logger.LogError(ex.Message);

                // Deshabilita el combo en caso de error y limpia los datos para evitar seleccionar alguno
                cbTipo.Enabled = false;
                cbTipo.DataSource = null;
                _tiposEmpleados = new();
            }
        }

        /// <summary>
        /// Carga la información del empleado (barbero) en las variables locales.
        /// Valida que las referencias necesarias no sean nulas antes de asignarlas.
        /// </summary>
        /// <returns>
        /// true si se pudo cargar correctamente la información del empleado; 
        /// false si faltan datos esenciales.
        /// </returns>
        private bool CargarEmpleado()
        {
            // Valida que exista un barbero.
            if (_barbero == null) return false;

            // Valida que el barbero tenga asociada una persona.
            if (_barbero.Personas == null) return false;

            // Asigna el estado del barbero si existe.
            _estado = _barbero.Estados;

            // Asigna la persona asociada al barbero.
            _persona = _barbero.Personas;

            // Obtiene el domicilio de la persona.
            _domicilio = _persona.Domicilios;

            // Obtiene la localidad del domicilio si existe.
            _localidad = _domicilio?.Localidades ?? null;

            // Obtiene la provincia de la localidad si existe.
            _provincia = _localidad?.Provincias ?? null;

            CargarContactos();

            return true;
        }

        /// <summary>
        /// Carga los contactos asociados a la persona actual utilizando el patrón Resultado y actualiza la grilla.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de ejecución:</b>
        /// <list type="number">
        /// <item>Limpia el origen de datos actual en memoria para evitar mostrar contactos antiguos.</item>
        /// <item><b>Cláusula de guarda:</b> Si no hay barbero o persona instanciada, aborta la consulta.</item>
        /// <item>Consulta los contactos a la capa de negocio mediante <see cref="ContactosNegocio.getContactosPorPersona"/>.</item>
        /// <item>Evalúa el <c>Resultado</c>: Si falla, registra el error en el log e inicializa una lista vacía para no romper la UI.</item>
        /// <item><b>Garantía visual:</b> El bloque <c>finally</c> asegura que <see cref="RefrescarGrilla"/> se ejecute siempre, haya ocurrido un error o no.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarContactos()
        {
            try
            {
                // 1. Reinicio seguro de referencias
                bindingContactos.DataSource = null;
                _contactos = null;

                // 2. Cláusula de guarda: No hay a quién buscarle contactos
                if (_barbero == null || _persona == null) return;

                // 3. Consulta a la capa de negocio (Patrón Resultado)
                var resultado = ContactosNegocio.GetContactosPorPersona(_persona);

                // 4. Evaluación del resultado
                if (!resultado.Success)
                {
                    // Registro silencioso del error en el log (podrías agregar un MensajeError si lo deseas)
                    Logger.LogError($"Error al cargar contactos de persona ID {_persona.IdPersona}: {resultado.Mensaje}");

                    // Fallback: Asignamos lista vacía para evitar nulos en la grilla
                    _contactos = new List<Contactos>();
                }
                else
                {
                    // Éxito: Obtenemos los datos (protegiendo contra nulos desde la BD)
                    _contactos = resultado.Data ?? new List<Contactos>();
                }
            }
            catch (Exception ex)
            {
                // Registro del error completo con stack trace
                Logger.LogError($"Error crítico en CargarContactos (Barberos): {ex.ToString()}");

                // Fallback preventivo ante caídas inesperadas
                _contactos = new List<Contactos>();
            }
            finally
            {
                // 5. Garantía de actualización visual
                // Al estar en el finally, la grilla se refrescará siempre.
                // Si hubo éxito mostrará los datos, si hubo error o no había persona, se limpiará correctamente.
                RefrescarGrilla();
            }
        }

        #endregion

        #endregion

        #region Eventos de formulario

        #region Automáticos (o disparados con tecleo)

        /// <summary>
        /// Restringe la entrada de un control de texto para que solo acepte caracteres numéricos.
        /// </summary>
        /// <remarks>
        /// - Permite únicamente dígitos y teclas de control (ej. Backspace).
        /// - Se utiliza en eventos <c>KeyPress</c> de controles de entrada.
        /// - Si se presiona un carácter no válido, se marca como manejado y no se ingresa.
        /// </remarks>
        /// <param name="sender">Control que dispara el evento.</param>
        /// <param name="e">Argumentos del evento <c>KeyPress</c>, incluyendo la tecla presionada.</param>
        private void KeyPress_Numerico(object sender, KeyPressEventArgs e)
        {
            // Si la tecla no es dígito ni tecla de control, se bloquea la entrada
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Maneja el evento de cambio de texto en el combo de provincias.
        /// </summary>
        /// <remarks>
        /// - Si el formulario está cerrando o cargando, no realiza ninguna acción.
        /// - En caso contrario, actualiza las localidades disponibles según la provincia seleccionada.
        /// </remarks>
        /// <param name="sender">Control <c>ComboBox</c> que dispara el evento.</param>
        /// <param name="e">Argumentos del evento <c>TextChanged</c>.</param>
        private void cbProvincia_TextChanged(object sender, EventArgs e)
        {
            // Evita ejecutar lógica si el formulario está cerrando o cargando datos
            if (cerrando || cargando)
            {
                return;
            }

            // Carga las localidades correspondientes a la provincia seleccionada
            CargarLocalidades();

            // Nota de mejora: si se agregan más combos dependientes (ej. país → provincia → localidad),
            // convendría centralizar la lógica en un método genérico de carga jerárquica.
            // Pero queda para consultar con el cliente
        }

        #endregion

        #region Botones (o eventos click)

        /// <summary>
        /// Maneja el evento de clic en el botón Cancelar.
        /// </summary>
        /// <remarks>
        /// - Invoca el método <c>CerrarFormulario()</c> para realizar el cierre controlado.
        /// - Se utiliza como manejador directo del botón Cancelar.
        /// </remarks>
        /// <param name="sender">El botón que dispara el evento.</param>
        /// <param name="e">Argumentos del evento <c>Click</c>.</param>
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Llama al método de cierre centralizado
            CerrarFormulario();
        }

        /// <summary>
        /// Maneja el evento de clic en el enlace de contactos.
        /// </summary>
        /// <remarks>
        /// - Determina el modo de formulario de contactos según si existen contactos previos.
        /// - Marca la bandera <c>editandoContactos</c> como activa.
        /// - Busca el formulario principal (<c>FrmMenuPrincipal</c>) entre los formularios abiertos.
        /// - Si no se encuentra el formulario principal, registra un error en el log y detiene la ejecución.
        /// - Si se encuentra, abre el formulario de contactos en el panel de contenido.
        /// </remarks>
        /// <param name="sender">El control <c>LinkLabel</c> que dispara el evento.</param>
        /// <param name="e">Argumentos del evento <c>LinkClicked</c>.</param>
        private void LinkContactos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Determina el modo de formulario: Modificación si ya hay contactos, Alta si no
            EnumModoForm modoCont = _contactos?.Count > 0 ? EnumModoForm.Modificacion : EnumModoForm.Alta;

            // Marca que se está editando contactos
            editandoContactos = true;

            // Método de seguridad: busca el formulario principal abierto
            FrmMenuPrincipal? principal = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (principal == null)
            {
                // Si no se encuentra, se registra el error y se detiene
                Logger.LogError("Error al encontrar el Formulario principal");
                return;
            }

            // Abre el formulario de contactos en el panel de contenido
            principal.AbrirFrmContactos(modoCont, "Barberos", _contactos, pnlContent);

            // Nota de mejora: en el futuro parametrizar el texto "Barberos" para reutilizar este mismo método.
        }

        /// <summary>
        /// Maneja el evento de clic en el botón Buscar para localizar un empleado por su DNI.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Al haber delegado la gestión de alertas y validaciones directamente a <see cref="BuscarEmpleado"/>,
        /// este evento simplemente invoca la búsqueda y delega el flujo de la interfaz (habilitar/deshabilitar controles, limpiar datos)
        /// a <see cref="ProcesarBusquedaEmpleado"/>.
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón que dispara el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Invoca la búsqueda (BuscarEmpleado maneja internamente sus propios errores visuales)
                bool existe = BuscarEmpleado();

                // 2. Delega la lógica de la UI según el resultado
                // (Ej: Si existe y es Alta, bloquear; si no existe, habilitar campos para carga nueva)
                ProcesarBusquedaEmpleado(existe);
            }
            catch (Exception ex)
            {
                // 3. Red de seguridad para evitar caídas de la aplicación por eventos no controlados
                Logger.LogError($"Error crítico en BtnBuscar_Click: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado al intentar realizar la búsqueda.");
            }
        }

        /// <summary>
        /// Orquesta el guardado del barbero, validando los datos, ejecutando la persistencia y gestionando la UI post-guardado.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo del Guardado:</b>
        /// <list type="number">
        /// <item>Ejecuta todas las validaciones previas agrupadas en <see cref="ContinuarGuardando"/>.</item>
        /// <item>Realiza la vinculación final de la entidad Persona con el Barbero en memoria.</item>
        /// <item>Invoca la operación de base de datos correspondiente (Alta o Modificación) capturando el resultado con <c>out</c>.</item>
        /// <item>Si falla, muestra el error. Si tiene éxito, pregunta al usuario si desea continuar cargando más registros.</item>
        /// <item>Resetea la ventana para un nuevo ingreso o la cierra según la respuesta del usuario.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón Guardar que dispara el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Orquestación de validaciones (las alertas visuales ya se manejan internamente)
                if (!ContinuarGuardando()) return;

                // 2. Vinculación final de entidades para la base de datos
                // Aseguramos que el barbero tenga asignada su persona y copiamos el ID (0 si es nuevo)
                _barbero.Personas = _persona;
                _barbero.IdPersona = _persona?.IdPersona ?? 0;

                bool operacionExitosa = false;
                string mensajeResultado; // Variable para recibir el parámetro 'out'

                // 3. Ejecución de la Persistencia
                if (modo == EnumModoForm.Alta)
                {
                    operacionExitosa = RegistrarEmpleado(out mensajeResultado);
                }
                else
                {
                    operacionExitosa = ModificarEmpleado(out mensajeResultado);
                }

                // 4. Manejo de Fallos
                if (!operacionExitosa)
                {
                    // El mensaje de error viene directo de la capa de negocio gracias al 'out'
                    Mensajes.MensajeError(mensajeResultado);
                    return;
                }

                // 5. Manejo del Éxito y Experiencia de Usuario (UX)
                DialogResult respuesta = Mensajes.Respuesta($"{mensajeResultado}\n\n¿Desea registrar un nuevo Barbero?");

                if (respuesta == DialogResult.Yes)
                {
                    // Reseteo completo del formulario para volver a empezar
                    LimpiarValores(true);
                    LimpiarCampos();
                    modo = EnumModoForm.Alta;
                    CargarFormulario();
                    ActivarCamposEditables(false);

                    return; // Cortamos la ejecución para no cerrar la ventana
                }

                // 6. Cierre del formulario
                cerrando = true; // Variable de estado (probablemente usada en FormClosing)
                this.Close();
            }
            catch (Exception ex)
            {
                // 7. Red de seguridad final
                Logger.LogError($"Error crítico no controlado al intentar guardar el barbero: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error técnico inesperado al procesar el guardado.");
            }
        }

        #endregion

        #endregion

        #region Interacción con campos

        #region Activaciones y ErrorProvider

        /// <summary>
        /// Asigna un mensaje de error a un control utilizando el <see cref="ErrorProvider"/>.
        /// </summary>
        /// <remarks>
        /// - Centraliza la lógica de validación visual.
        /// - Si el parámetro <c>error</c> está vacío, limpia el error del control.
        /// - Permite reutilizar la misma llamada en todos los métodos de comprobación.
        /// </remarks>
        /// <param name="campo">El control al que se le asignará el error.</param>
        /// <param name="error">El mensaje de error a mostrar; vacío para limpiar el error.</param>
        private void ErrorCampo(Control campo, string error = "")
        {
            errorProvider1.SetError(campo, error);
        }

        /// <summary>
        /// Activa o desactiva los campos editables del formulario
        /// según el estado indicado.
        /// </summary>
        /// <remarks>
        /// - Habilita o deshabilita los controles de texto, combos y botones.
        /// - Los combos de tipo y estado solo se habilitan si existen elementos cargados.
        /// - Se utiliza para alternar una vez validado el dni.
        /// </remarks>
        /// <param name="activo">
        /// Indica si los campos deben estar habilitados (true) o deshabilitados (false).
        /// </param>
        private void ActivarCamposEditables(bool activo)
        {
            // Campos de texto básicos
            txtApellido.Enabled = activo;
            txtNombre.Enabled = activo;

            // Combo de tipos de empleado: solo habilitado si hay elementos cargados
            cbTipo.Enabled = activo && _tiposEmpleados.Count > 0;

            // Combo de estados: solo habilitado si la lista no es nula y tiene elementos
            cbEstados.Enabled = activo && _estados?.Count > 0;

            // Fecha de nacimiento
            dateTimeNacimiento.Enabled = activo;

            // Link de contactos
            linkContactos.Enabled = activo;

            // Dirección
            txtCalle.Enabled = activo;
            txtNro.Enabled = activo;
            txtPiso.Enabled = activo;
            txtDepto.Enabled = activo;
            txtBarrio.Enabled = activo;

            // Provincia
            cbProvincia.Enabled = activo;

            // Botón de guardar
            btnGuardar.Enabled = activo;

            //Botón de búsqueda
            btnBuscar.Enabled = !activo;
        }

        /// <summary>
        /// Prepara el formulario para el modo Alta.
        /// </summary>
        /// <remarks>
        /// - Desactiva los campos editables.
        /// - Limpia los valores previos.
        /// - Cambia el modo del formulario a <c>Alta</c>.
        /// </remarks>
        private void PrepararAlta()
        {
            ActivarCamposEditables(false);
            LimpiarValores(true);
            modo = EnumModoForm.Alta;
        }

        /// <summary>
        /// Prepara el formulario para el modo Modificación.
        /// </summary>
        /// <remarks>
        /// - Habilita los campos editables para permitir cambios en los datos del empleado.
        /// - Carga los datos actuales del empleado en los controles del formulario y en las variables internas,
        ///   asegurando que el usuario pueda ver y editar la información existente.
        /// </remarks>
        private void PrepararModificacion()
        {
            // Habilita los campos editables para permitir modificaciones
            ActivarCamposEditables(true);

            // Carga los datos del empleado en los controles y variables
            CargarDatosEmpleado();
        }

        #endregion

        #region Lectura y escritura

        /// <summary>
        /// Limpia todos los campos del formulario y reinicia la grilla.
        /// </summary>
        /// <remarks>
        /// - Refresca la grilla principal para mostrar datos actualizados.
        /// - Vacía los campos de texto (DNI, nombre, apellido, dirección).
        /// - Reinicia las fechas de nacimiento e ingreso al día actual.
        /// - Desselecciona los combos de tipo y estado.
        /// Se utiliza para preparar el formulario en un nuevo registro o al reiniciar la vista.
        /// </remarks>
        private void LimpiarCampos()
        {
            // Refresca la grilla para mostrar datos actualizados
            RefrescarGrilla();

            errorProvider1.Clear();

            // Vacía los campos de texto
            txtDni.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtCalle.Text = string.Empty;
            txtNro.Text = string.Empty;
            txtPiso.Text = string.Empty;
            txtDepto.Text = string.Empty;
            txtBarrio.Text = string.Empty;

            // Reinicia las fechas al día actual
            dateTimeNacimiento.Value = DateTime.Today;
            dateTimeIngreso.Value = DateTime.Today;

            // Desselecciona los combos
            cbTipo.SelectedIndex = -1;
            cbEstados.SelectedIndex = -1;
        }

        /// <summary>
        /// Carga los datos del empleado en los controles de la interfaz.
        /// Valida que existan las referencias necesarias antes de asignar valores.
        /// </summary>
        private void CargarCamposEmpleado()
        {
            // Valida que exista un barbero y una persona antes de continuar.
            if (_barbero == null || _persona == null)
                return;

            // Asigna los datos personales básicos.
            txtApellido.Text = _persona?.Apellidos;
            txtDni.Text = _persona?.Dni;
            txtNombre.Text = _persona?.Nombres;

            // Asigna la fecha de nacimiento, o la fecha actual si no existe.
            dateTimeNacimiento.Value = _persona?.FechaNac ?? DateTime.Today;

            //Asigna la fecha de ingreso
            dateTimeIngreso.Value = _barbero.IdEmpleado != null ? _barbero.FechaIngreso : DateTime.Now;

            // Asigna el estado si existe.
            SeleccionarEstado();

            // Asigna el tipo de empleado si existe.
            SeleccionarTipoEmpleado();

            // Asigna los datos de domicilio si existen.
            if (_domicilio != null)
            {
                txtCalle.Text = _domicilio.Calle;
                txtBarrio.Text = _domicilio.Barrio;
                txtNro.Text = _domicilio.Altura;
                txtPiso.Text = _domicilio.Piso;
                txtDepto.Text = _domicilio.Depto;

                // Asigna la provincia si existe.
                if (_provincia?.IdProvincia != null)
                    cbProvincia.SelectedValue = _provincia.IdProvincia;
                //La localidad deberia cargarse si con el evento TextChanged de provincia
            }
        }

        /// <summary>
        /// Selecciona el estado actual en el combo,
        /// priorizando el estado del barbero cargado o un estado "Activo" por defecto.
        /// </summary>
        /// <remarks>
        /// - Recupera el estado del barbero si está disponible.
        /// - Si no existe, intenta asignar el estado "Activo".
        /// - Si tampoco existe, crea uno por defecto y lo agrega a la lista.
        /// - Actualiza el binding y selecciona el estado en el combo.
        /// </remarks>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la selección y se registra en el log.
        /// </exception>
        private void SeleccionarEstado()
        {
            if (_estados == null)
                return;

            try
            {
                // Si existe un barbero cargado, se intenta recuperar su estado
                if (_barbero != null)
                {
                    if (_barbero.Estados != null)
                        _estado = _barbero.Estados; // Usa el objeto estado directamente
                    else if (_barbero.IdEstado > 0)
                        _estado = _estados.FirstOrDefault(e => e.IdEstado == _barbero.IdEstado); // Busca por IdEstado
                }

                // Si no se pudo determinar el estado del barbero
                if (_estado == null)
                {
                    // Se intenta asignar el estado "Activo" por defecto
                    _estado = _estados.FirstOrDefault(e => e.Estado.Equals("Activo"));

                    // Si tampoco existe un estado "Activo" en la lista
                    if (_estado == null)
                    {
                        // Se reinicia el binding
                        bindingEstados.DataSource = null;

                        // Se crea un estado "Activo" por defecto
                        _estado = new Estados { Indole = "Empleados", Estado = "Activo", IdEstado = 0 };

                        // Se arma una nueva lista que incluye el estado por defecto más los estados obtenidos
                        List<Estados> lista = new List<Estados> { _estado };
                        lista.AddRange(_estados);

                        // Se asigna la nueva lista al binding
                        bindingEstados.DataSource = lista;
                    }
                }

                // Selecciona en el combo el estado actual
                cbEstados.SelectedValue = _estado.IdEstado;
            }
            catch (Exception ex)
            {
                // Registra el error en el log para diagnóstico
                Logger.LogError(ex.Message);

                // Deshabilita el combo en caso de error
                cbEstados.Enabled = false;
                return;
            }
        }

        private void SeleccionarTipoEmpleado()
        {
            if (_tiposEmpleados == null || _barbero == null)
                return;
            try
            {
                // Intenta seleccionar el tipo de empleado del barbero
                if (string.IsNullOrWhiteSpace(_barbero.TipoEmpleado))
                    _barbero.TipoEmpleado = _tiposEmpleados.FirstOrDefault(); // Asigna el primer tipo disponible si no tiene
                string? tipo = _tiposEmpleados.FirstOrDefault(t => t.Equals(_barbero.TipoEmpleado, StringComparison.OrdinalIgnoreCase));
                int indice = 0;
                if (tipo == null)
                    _tiposEmpleados.Insert(0, _barbero.TipoEmpleado ?? "No Definido"); // Si el tipo del barbero no está en la lista, lo agrega al inicio
                else
                    indice = _tiposEmpleados.IndexOf(tipo);
                cbTipo.SelectedIndex = indice;

            }
            catch (Exception ex)
            {
                // Registra el error en el log para diagnóstico
                Logger.LogError(ex.Message);
                // Deshabilita el combo en caso de error
                cbTipo.Enabled = false;
            }
        }

        /// <summary>
        /// Refresca la grilla de contactos asignando la lista actual al binding
        /// y actualizando la vista del <see cref="dataGridContactos"/>.
        /// </summary>
        /// <remarks>
        /// - Asigna la colección <c>_contactos</c> al <c>bindingContactos</c>.
        /// - Llama a <c>Refresh()</c> para actualizar la grilla en pantalla.
        /// - En caso de error, registra el log y muestra un mensaje al usuario.
        /// </remarks>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la actualización y se informa al usuario.
        /// </exception>
        private void RefrescarGrilla()
        {
            try
            {
                // Asigna la lista de contactos al binding
                bindingContactos.DataSource = _contactos;

                // Refresca la grilla para mostrar los cambios
                dataGridContactos.Refresh();
            }
            catch (Exception ex)
            {
                // Registra el error en el log
                Logger.LogError(ex.Message);

                // Muestra un mensaje de error genérico al usuario
                Mensajes.MensajeError("Error inesperado" + ex.Message);
            }
        }

        #endregion

        #endregion

        #region Validaciones
        /// <summary>
        /// Valida que el texto ingresado en un control no esté vacío y cumpla con las reglas de formato.
        /// Aplica capitalización si la validación es exitosa.
        /// </summary>
        /// <param name="campo">Control de entrada cuyo texto se validará.</param>
        /// <param name="capitalizarTodo">Indica si se debe capitalizar todo el texto (true) o solo la primera letra (false).</param>
        /// <returns>True si la validación fue exitosa; False en caso contrario.</returns>
        private bool ValidarTextoObligatorio(Control campo, bool capitalizarTodo = false)
        {
            // Se obtiene el texto sin espacios iniciales ni finales
            string texto = campo.Text.Trim();

            // Variable para almacenar un posible mensaje de error
            string error = string.Empty;

            // Se valida el texto utilizando las reglas definidas en Validaciones.textoCorrecto
            bool exito = Validaciones.TextoCorrecto(texto, ref error);

            // Se muestra el error en el control si corresponde
            ErrorCampo(campo, error);

            // Si la validación fue exitosa, se capitaliza el texto según el parámetro
            if (exito)
                campo.Text = Validaciones.CapitalizarTexto(texto, capitalizarTodo);

            // Retorna el resultado de la validación
            return exito;
        }

        /// <summary>
        /// Valida que el texto ingresado en un control sea un número válido.
        /// Permite definir si el campo es obligatorio.
        /// </summary>
        /// <param name="campo">Control de entrada cuyo texto se validará.</param>
        /// <param name="obligatorio">Indica si el campo debe contener obligatoriamente un valor.</param>
        /// <returns>
        /// True si el número es válido.
        /// False si el campo es obligatorio y está vacío, o si el formato numérico es incorrecto.
        /// </returns>
        private bool ValidarCampoNumerico(Control campo, bool obligatorio)
        {
            // Se obtiene el texto sin espacios iniciales ni finales
            string texto = campo.Text.Trim();

            // Se asigna el texto limpio al campo
            campo.Text = texto;

            // Se inicializa la variable número: 1 si hay texto, 0 si está vacío
            int numero = !string.IsNullOrWhiteSpace(texto) ? 1 : 0;

            // Variable para almacenar un posible mensaje de error
            string error = string.Empty;

            // Si hay texto, se intenta convertir a número
            if (numero > 0)
                error = int.TryParse(texto, out numero) ? "" : "Formato de número incorrecto";
            else
                // Si no hay texto y el campo es obligatorio, se genera mensaje de error
                error = obligatorio ? "Debe ingresar un valor" : "";

            // Se muestra el error en el control si corresponde
            ErrorCampo(campo, error);

            // Retorna true si el error está vacío
            return string.IsNullOrWhiteSpace(error);
        }

        /// <summary>
        /// Comprueba que el DNI ingresado tenga un formato y longitud válidos.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Validaciones:</b>
        /// <list type="bullet">
        /// <item>Verifica que el campo contenga solo números (a través de <c>ValidarCampoNumerico</c>).</item>
        /// <item>Verifica que la longitud esté entre 6 y 9 dígitos.</item>
        /// </list>
        /// Si alguna validación falla, se marca el control con <see cref="ErrorCampo"/>.
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si el DNI es válido; de lo contrario, <c>false</c>.</returns>
        private bool ComprobarDni()
        {
            // 1. Validación de caracteres (solo números)
            if (!ValidarCampoNumerico(txtDni, true))
                return false;

            // 2. Validación de longitud
            string dni = txtDni.Text.Trim();
            if (dni.Length < 6 || dni.Length > 9)
            {
                ErrorCampo(txtDni, "La longitud del DNI debe tener entre 6 y 9 dígitos.");
                return false;
            }

            // 3. Éxito: Limpieza de errores visuales
            ErrorCampo(txtDni);

            return true;
        }

        /// <summary>
        /// Comprueba que los nombres y apellidos cumplan con las reglas de texto, los capitaliza y los asigna a la entidad.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Se evalúan tanto el nombre como el apellido simultáneamente para mostrar todos los errores posibles de una sola vez.
        /// Si la validación es exitosa, se capitaliza el texto (ej: "juan" -> "Juan") y se actualiza la interfaz y el objeto en memoria.
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si los nombres y apellidos son válidos; de lo contrario, <c>false</c>.</returns>
        private bool ComprobarNombres()
        {
            // Inicialización moderna: Si _persona es null, crea una nueva instancia
            _persona ??= new Personas();

            string nombresIngresados = txtNombre.Text.Trim();
            string apellidosIngresados = txtApellido.Text.Trim();

            // Variables temporales para atrapar los mensajes de tu clase Validaciones
            string errorNombres = string.Empty;
            string errorApellidos = string.Empty;

            // Evaluamos ambos campos por separado para no hacer un "corto circuito" 
            // y poder marcar ambos TextBoxes en rojo si ambos están mal.
            bool nombreValido = Validaciones.TextoCorrecto(nombresIngresados, ref errorNombres);
            bool apellidoValido = Validaciones.TextoCorrecto(apellidosIngresados, ref errorApellidos);

            if (!nombreValido || !apellidoValido)
            {
                // Encendemos el ErrorProvider con sus respectivos mensajes o lo limpiamos si uno de los dos está bien
                if (!nombreValido) ErrorCampo(txtNombre, errorNombres); else ErrorCampo(txtNombre);
                if (!apellidoValido) ErrorCampo(txtApellido, errorApellidos); else ErrorCampo(txtApellido);

                return false;
            }

            // Limpieza general si todo está correcto
            ErrorCampo(txtNombre);
            ErrorCampo(txtApellido);

            // Capitalización para mantener la uniformidad en la base de datos
            _persona.Nombres = Validaciones.CapitalizarTexto(nombresIngresados);
            _persona.Apellidos = Validaciones.CapitalizarTexto(apellidosIngresados);

            // Feedback visual: Actualizamos las cajas de texto para que el usuario vea la corrección
            txtNombre.Text = _persona.Nombres;
            txtApellido.Text = _persona.Apellidos;

            return true;
        }

        /// <summary>
        /// Comprueba la validez de la fecha de nacimiento seleccionada en el <see cref="dateTimeNacimiento"/>.
        /// </summary>
        /// <remarks>
        /// - Obtiene la fecha seleccionada en el control.
        /// - Si la fecha es menor a la actual (ayer o anterior), se asigna a <c>_persona.FechaNac</c>.
        /// - Si la fecha es inválida (hoy o futura), se nulifica <c>_persona.FechaNac</c>.
        /// - En caso de error, se registra en el log y se muestra un mensaje en el <c>ErrorProvider</c>.
        /// </remarks>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la comprobación y se registra en el log.
        /// </exception>
        private void ComprobarNacimiento()
        {
            string mensaje = ""; // Mensaje para el ErrorProvider

            try
            {
                // Obtener la fecha seleccionada en el DateTimePicker
                DateTime fechaSeleccionada = dateTimeNacimiento.Value;

                // Verificar si la fecha es menor a la actual (ayer o anterior), sino nulificar
                _persona.FechaNac = fechaSeleccionada < DateTime.Today.AddDays(-1) ? fechaSeleccionada : null;

            }
            catch (Exception ex)
            {
                // Manejo de errores
                mensaje = "Error al comprobar la fecha de nacimiento";
                Logger.LogError(ex.ToString());
            }
            finally
            {
                // Asigna el mensaje de error al control DateTimePicker
                ErrorCampo(dateTimeNacimiento, mensaje);
            }
        }

        /// <summary>
        /// Comprueba la validez integral de los datos de un empleado (barbero) antes de registrarlo o modificarlo.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso de Validación:</b>
        /// <list type="number">
        /// <item><b>Duplicidad de DNI:</b> Verifica que el DNI no pertenezca a otro empleado en la base de datos.</item>
        /// <item><b>Sincronización:</b> Asegura que las instancias de <c>_barbero</c> y <c>_persona</c> estén creadas en memoria.</item>
        /// <item><b>Nombres:</b> Delega la validación de formato a <see cref="ComprobarNombres"/>.</item>
        /// <item><b>Nacimiento:</b> Delega la validación de fechas a <see cref="ComprobarNacimiento"/>.</item>
        /// <item><b>Estado:</b> Verifica que el combo de estados tenga una selección válida, especialmente en modificaciones.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="esNuevoRegistro">Indica si la operación corresponde a un registro nuevo (<c>true</c>) o a una modificación (<c>false</c>).</param>
        /// <returns><c>true</c> si todas las comprobaciones fueron exitosas; <c>false</c> si se detecta algún error o inconsistencia.</returns>
        private bool ComprobarEmpleado(bool esNuevoRegistro)
        {
            try
            {
                // Guardamos el DNI actual en memoria por si BuscarEmpleado() sobreescribe la entidad
                string dniActualMemoria = _persona?.Dni;

                // 1. Verificación de Duplicidad de DNI en la Base de Datos
                if (BuscarEmpleado())
                {
                    if (esNuevoRegistro)
                    {
                        // Es alta y el DNI ya existe
                        ErrorCampo(txtDni, "El DNI ingresado ya pertenece a otro barbero registrado.");
                        return false;
                    }
                    else if (_barbero?.Personas?.Dni != dniActualMemoria)
                    {
                        // Es modificación, pero el DNI nuevo introducido choca con otro existente
                        ErrorCampo(txtDni, $"El DNI nuevo que ingresó ya pertenece a {_barbero?.NombreCompleto}.");
                        return false;
                    }
                }
                else
                {
                    // Si el DNI está libre o es válido, limpiamos cualquier error visual previo
                    ErrorCampo(txtDni);
                }

                // 2. Sincronización Segura de Entidades (Prevención de NullReferenceException)
                _barbero ??= new Empleados();

                // Asignamos la persona del barbero encontrado, o mantenemos la actual, o creamos una nueva
                _persona = _barbero.Personas ?? _persona ?? new Personas();

                // 3. Validación de Nombres y Apellidos
                if (!ComprobarNombres())
                {
                    return false;
                }

                // 4. Validación de Fecha de Nacimiento (asumimos que gestiona sus propios errores visuales)
                ComprobarNacimiento();

                _barbero.TipoEmpleado = cbTipo.SelectedItem?.ToString() ?? _barbero.TipoEmpleado; // Asigna el tipo seleccionado o mantiene el actual

                // 5. Validación y Asignación del Estado Laboral
                // Uso de 'as' para casteo seguro sin excepciones
                _estado = cbEstados.SelectedItem as Estados;

                if (_estado == null && modo == EnumModoForm.Modificacion)
                {
                    Mensajes.MensajeError("Problemas con la recuperación de estados de empleados desde la base de datos.");
                    return false;
                }

                // Vinculación final de datos
                _barbero.Estados = _estado;
                _barbero.IdEstado = _estado?.IdEstado ?? 0;

                // Si pasó todas las barreras, los datos son válidos
                return true;
            }
            catch (Exception ex)
            {
                // 6. Manejo de Errores Inesperados
                Logger.LogError($"Error inesperado en ComprobarEmpleado: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error técnico al intentar validar los datos del barbero.");
                return false;
            }
        }

        /// <summary>
        /// Verifica si se ingresaron datos suficientes para conformar una dirección válida y construye la entidad.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Criterios de validación:</b>
        /// <list type="bullet">
        /// <item>Se requiere como mínimo que exista una Calle o un Barrio.</item>
        /// <item>Si se ingresó Calle o Barrio, es estrictamente obligatorio seleccionar o escribir una Localidad válida.</item>
        /// <item>Si los campos obligatorios están vacíos, se asume que el usuario no desea registrar un domicilio.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) que contendrá avisos o errores si el método devuelve <c>false</c>.</param>
        /// <returns><c>true</c> si se construyó un domicilio válido; <c>false</c> si no se ingresaron datos suficientes o hubo un error.</returns>
        private bool DireccionIngresada(out string mensaje)
        {
            // 1. Inicialización obligatoria de salida
            mensaje = string.Empty;

            // 2. Evaluación de intención: ¿El usuario quiere registrar una dirección?
            // Asumimos que ValidarTextoObligatorio devuelve true si el TextBox tiene texto.
            bool tieneCalle = ValidarTextoObligatorio(txtCalle);
            bool tieneBarrio = ValidarTextoObligatorio(txtBarrio);

            if (!tieneCalle && !tieneBarrio)
            {
                // El usuario dejó todo vacío. No es un error crítico, pero no hay dirección que guardar.
                _domicilio = null;
                mensaje = "No se registrará ningún domicilio (campos vacíos).";
                return false;
            }

            // 3. Si hay intención, la Localidad es obligatoria
            // Invocamos a LocalidadIngresada usando su nueva firma 'out'
            if (!LocalidadIngresada(out string errorLocalidad))
            {
                ErrorCampo(cbLocalidad, errorLocalidad);
                ErrorCampo(cbProvincia, errorLocalidad);
                mensaje = "Faltan datos geográficos para completar el domicilio.";
                return false;
            }

            // Limpieza de errores visuales si todo está bien
            ErrorCampo(cbLocalidad);
            ErrorCampo(cbProvincia);

            try
            {
                // 4. Construcción segura de la entidad Domicilios
                // Extraemos y limpiamos los textos para evitar guardar puros espacios en blanco en la BD
                string calleLimpia = txtCalle.Text.Trim();
                string barrioLimpio = txtBarrio.Text.Trim();
                string alturaLimpia = txtNro.Text.Trim();
                string pisoLimpio = txtPiso.Text.Trim();
                string deptoLimpio = txtDepto.Text.Trim();

                _domicilio = new Domicilios
                {
                    // Mantenemos el ID original si estábamos editando
                    IdDomicilio = _domicilio?.IdDomicilio,

                    // Operadores condicionales para asignar null si el string está vacío
                    Calle = string.IsNullOrWhiteSpace(calleLimpia) ? null : calleLimpia,
                    Barrio = string.IsNullOrWhiteSpace(barrioLimpio) ? null : barrioLimpio,
                    Altura = string.IsNullOrWhiteSpace(alturaLimpia) ? null : alturaLimpia,
                    Piso = string.IsNullOrWhiteSpace(pisoLimpio) ? null : pisoLimpio,
                    Depto = string.IsNullOrWhiteSpace(deptoLimpio) ? null : deptoLimpio,

                    // Vinculación relacional con Localidad
                    IdLocalidad = _localidad?.IdLocalidad ?? 0,
                    Localidades = _localidad != null && _localidad.IdLocalidad == 0 ? _localidad : null
                };

                return true;
            }
            catch (Exception ex)
            {
                // 5. Fallo técnico durante la instanciación
                Logger.LogError($"Error crítico al construir la entidad Domicilio: {ex.ToString()}");
                _domicilio = null;
                mensaje = "Ocurrió un error técnico inesperado al intentar procesar el domicilio.";
                return false;
            }
        }

        /// <summary>
        /// Verifica si se ha ingresado una localidad válida en el combo, creándola si es nueva.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso de validación y asignación:</b>
        /// <list type="number">
        /// <item>Requiere que previamente se haya ingresado una provincia válida (vía <see cref="ProvinciaIngresada"/>).</item>
        /// <item>Verifica que el campo de localidad no esté vacío.</item>
        /// <item>Busca la localidad en la lista cargada en memoria (ignorando mayúsculas/minúsculas).</item>
        /// <item>Si no existe, instancia una nueva capitalizando el texto ingresado.</item>
        /// <item>Vincula la provincia correspondiente a la localidad (por ID o por referencia de objeto).</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) que contendrá el motivo del fallo si el método devuelve <c>false</c>.</param>
        /// <returns><c>true</c> si se validó o creó exitosamente la localidad; de lo contrario, <c>false</c>.</returns>
        private bool LocalidadIngresada(out string mensaje)
        {
            // 1. Inicialización obligatoria del parámetro 'out' y limpieza de estado
            mensaje = string.Empty;
            _localidad = null;

            // 2. Validación de dependencias (Provincia)
            if (!ProvinciaIngresada())
            {
                mensaje = "Para ingresar un domicilio, primero debe especificar una provincia válida.";
                return false;
            }

            // 3. Validación de entrada (Localidad)
            string nombreLocalidad = cbLocalidad.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreLocalidad))
            {
                mensaje = "Por favor, escriba o seleccione una localidad.";
                return false;
            }

            try
            {
                // 4. Búsqueda de coincidencia en memoria
                if (_localidades != null && _localidades.Count > 0)
                {
                    _localidad = _localidades.FirstOrDefault(l => l.Localidad.Equals(nombreLocalidad, StringComparison.OrdinalIgnoreCase));
                }

                // 5. Creación de nueva localidad si no fue encontrada
                if (_localidad == null)
                {
                    _localidad = new Localidades
                    {
                        Localidad = Validaciones.CapitalizarTexto(nombreLocalidad, true)
                    };
                }

                // 6. Vinculación Relacional (Localidad -> Provincia)
                _localidad.IdProvincia = _provincia?.IdProvincia ?? 0;

                // Si la provincia es nueva (Id = 0) o no está en BD, enlazamos el objeto completo para que Entity Framework / la BD lo sepa manejar
                if (_localidad.IdProvincia == 0)
                {
                    _localidad.Provincias = _provincia;
                }

                // Si llegamos hasta aquí, todo fue un éxito
                return true;
            }
            catch (Exception ex)
            {
                // 7. Manejo de Errores Críticos
                Logger.LogError($"Error inesperado en LocalidadIngresada: {ex.ToString()}");
                mensaje = "Ocurrió un error técnico al procesar la localidad. Revise los registros del sistema.";
                return false;
            }
        }

        /// <summary>
        /// Verifica si se ha ingresado una provincia válida en el combo
        /// y asigna el objeto <c>_provincia</c> en consecuencia.
        /// </summary>
        /// <remarks>
        /// - Si el texto de provincia está vacío, devuelve <c>false</c>.
        /// - Si no hay provincias cargadas, crea una nueva con el texto ingresado.
        /// - Si hay provincias cargadas, busca coincidencia por nombre.
        /// - Si no encuentra coincidencia, crea una nueva provincia capitalizando el texto.
        /// - Registra en el log cualquier excepción y devuelve <c>false</c>.
        /// </remarks>
        /// <returns>
        /// <c>true</c> si se ingresó una provincia válida o se creó una nueva; 
        /// <c>false</c> si el texto está vacío o ocurre una excepción.
        /// </returns>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la validación y se registra en el log.
        /// </exception>
        private bool ProvinciaIngresada()
        {
            _provincia = null;
            string provincia = cbProvincia.Text.Trim();

            // Si el texto está vacío, no hay provincia válida
            if (String.IsNullOrWhiteSpace(provincia))
                return false;

            // Si no hay provincias cargadas, se crea una nueva con el texto ingresado
            if (_provincias == null || _provincias.Count < 1)
            {
                _provincia = new Provincias { Provincia = provincia };
                return true;
            }

            try
            {
                // Busca la provincia en la lista, ignorando mayúsculas/minúsculas
                _provincia = _provincias
                    .FirstOrDefault(p => p.Provincia.Equals(provincia, StringComparison.OrdinalIgnoreCase));

                // Si no se encontró, se crea una nueva capitalizando el texto
                _provincia ??= new Provincias { Provincia = Validaciones.CapitalizarTexto(provincia, true) };

                return true;
            }
            catch (Exception ex)
            {
                // Registra el error y devuelve false
                Logger.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Pregunta al usuario si desea modificar los datos de un empleado existente.
        /// </summary>
        /// <remarks>
        /// - Muestra un mensaje con el DNI, tipo de empleado y nombre completo.
        /// - Retorna true si el usuario confirma la modificación.
        /// </remarks>
        /// <param name="dni">DNI del empleado.</param>
        /// <param name="tipoEmpleado">Tipo de empleado (ej. barbero).</param>
        /// <param name="nombreCompleto">Nombre completo del empleado.</param>
        /// <returns>True si el usuario confirma la modificación; False en caso contrario.</returns>
        private bool ConfirmarModificacion(string dni, string tipoEmpleado, string nombreCompleto)
        {
            DialogResult respuesta = Mensajes.Respuesta(
                $"El DNI {dni} pertenece al {tipoEmpleado}\n{nombreCompleto}\n¿Desea modificar sus datos?");
            return respuesta == DialogResult.Yes;
        }

        /// <summary>
        /// Pregunta al usuario si desea registrar un nuevo empleado.
        /// </summary>
        /// <remarks>
        /// - Muestra un mensaje indicando que el DNI no pertenece a ningún empleado.
        /// - Retorna true si el usuario confirma el registro.
        /// </remarks>
        /// <param name="dni">DNI ingresado por el usuario.</param>
        /// <returns>True si el usuario confirma el registro; False en caso contrario.</returns>
        private bool ConfirmarRegistroNuevo(string dni)
        {
            DialogResult respuesta = Mensajes.Respuesta(
                $"El DNI: {dni} no pertenece a ningún empleado\n¿Desea registrar uno nuevo?");
            return respuesta == DialogResult.Yes;
        }

        /// <summary>
        /// Confirma si se puede continuar con la operación de contactos.
        /// </summary>
        /// <returns>
        /// Devuelve true si no hay edición activa o si el usuario confirma continuar.
        /// Devuelve false si el usuario decide cancelar la operación.
        /// </returns>
        private bool ConfirmarContactos()
        {
            // Verifica si la edición de contactos está activa
            if (editandoContactos)
            {
                // Solicita confirmación al usuario, advirtiendo que los cambios no se guardarán
                DialogResult respuesta = Mensajes.Respuesta(
                    "La edición de contactos está activa, si continúa no se guardarán los cambios\n¿Desea continuar igualmente?"
                );

                // Retorna true solo si el usuario acepta continuar
                return respuesta == DialogResult.Yes;
            }

            // Si no hay edición activa, permite continuar directamente
            return true;
        }

        /// <summary>
        /// Orquesta las validaciones finales antes de proceder con el guardado del registro en la base de datos.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de Validación:</b>
        /// <list type="number">
        /// <item>Verifica que no haya ventanas de edición de contactos abiertas y pendientes (<see cref="ConfirmarContactos"/>).</item>
        /// <item>Valida integralmente los datos del empleado (<see cref="ComprobarEmpleado"/>).</item>
        /// <item>Intenta construir el domicilio. Si los datos son insuficientes, emite una advertencia pero no bloquea el flujo.</item>
        /// <item>Vincula el domicilio validado (o nulo) a la entidad persona en memoria.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si las validaciones críticas se cumplen y es seguro guardar; de lo contrario, <c>false</c>.</returns>
        private bool ContinuarGuardando()
        {
            try
            {
                // 1. Verificación de ventanas hijas (Contactos)
                if (!ConfirmarContactos()) return false;

                // 2. Validación central del Empleado (DNI, Nombres, Estado)
                // Como ComprobarEmpleado ya gestiona sus propias alertas de UI, solo verificamos su resultado booleano.
                bool esAlta = (modo == EnumModoForm.Alta);
                if (!ComprobarEmpleado(esAlta)) return false;

                // 3. Validación y extracción de la Dirección
                // Usamos el nuevo patrón 'out' para capturar el aviso si la validación no es completa
                if (!DireccionIngresada(out string avisoDireccion))
                {
                    // Como el domicilio no bloquea el guardado (no es crítico), 
                    // mostramos la advertencia que nos devolvió el método.
                    if (!string.IsNullOrWhiteSpace(avisoDireccion))
                    {
                        Mensajes.MensajeAdvertencia(avisoDireccion);
                    }
                }

                // 4. Vinculación relacional de entidades
                // Protegemos contra nulos por si _persona no llegó a instanciarse por algún motivo extraño
                if (_persona != null)
                {
                    _persona.Domicilios = _domicilio;
                    _persona.IdDomicilio = _domicilio?.IdDomicilio;
                    _persona.Dni = _persona.Dni ?? txtDni.Text.Trim(); // Asegura que el DNI esté asignado a la persona
                }

                // Si pasó todas las barreras críticas, luz verde para guardar
                return true;
            }
            catch (Exception ex)
            {
                // 5. Manejo de errores imprevistos
                Logger.LogError($"Error crítico en ContinuarGuardando: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado al verificar los datos antes de guardar.");
                return false;
            }
        }

        #endregion

        #region Acciones y Métodos

        #region Búsquedas
        /// <summary>
        /// Busca un empleado por su DNI en la base de datos y actualiza las referencias en memoria.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de ejecución:</b>
        /// <list type="number">
        /// <item>Si el formulario está en modo Alta, limpia las entidades previas.</item>
        /// <item>Delega la validación de formato a <see cref="ComprobarDni"/>.</item>
        /// <item>Consulta a la capa de negocio utilizando el patrón <c>Resultado</c>.</item>
        /// <item>Si la consulta falla, muestra el error y retorna falso. Si tiene éxito, asigna el objeto.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si se encontró y asignó un empleado válido; de lo contrario, <c>false</c>.</returns>
        private bool BuscarEmpleado()
        {
            try
            {
                if (modo == EnumModoForm.Alta)
                {
                    // Reiniciamos las referencias para evitar cruce de datos en un nuevo registro
                    _barbero = null;
                    _persona = null;
                }

                // 1. Validación local (Asumiendo que ComprobarDni también se refactorizará para no usar 'ref')
                // Si ComprobarDni falla, este mismo se encargará de mostrar el SetError o MensajeError.
                if (!ComprobarDni()) return false;

                // 2. Consulta a la capa de negocio (Patrón Resultado)
                string dniIngresado = txtDni.Text.Trim();
                var resultado = EmpleadosNegocio.GetEmpleadoPorDni(dniIngresado);

                // 3. Manejo de errores de negocio o base de datos
                if (!resultado.Success && modo == EnumModoForm.Modificacion)
                {
                    Mensajes.MensajeError(resultado.Mensaje ?? "Ocurrió un problema al intentar buscar el empleado.");
                    return false;
                }

                // 4. Éxito: Asignación de datos
                _barbero = resultado.Data;

                // Retorna true solo si realmente nos devolvió un objeto instanciado y con ID válido
                return _barbero != null && _barbero.IdEmpleado > 0;
            }
            catch (Exception ex)
            {
                // 5. Manejo de excepciones imprevistas
                Logger.LogError($"Error crítico en BuscarEmpleado (por DNI): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado durante la búsqueda del empleado.");
                return false;
            }
        }
        #endregion

        #region Valores de objetos

        /// <summary>
        /// Carga los datos completos del empleado en la interfaz.
        /// Valida previamente que el empleado pueda ser obtenido y 
        /// maneja posibles excepciones durante la carga.
        /// </summary>
        private void CargarDatosEmpleado()
        {
            // Valida que el empleado pueda ser cargado correctamente.
            if (!CargarEmpleado())
            {
                Mensajes.MensajeError("Error: En el modo modificación no se puede obtener al sujeto a modificar");
                Logger.LogError("Error: En el modo modificación no se puede obtener al sujeto a modificar");
                return;
            }

            try
            {
                // Carga los contactos asociados al empleado.
                CargarContactos();

                // Carga los campos del empleado en los controles de la interfaz.
                CargarCamposEmpleado();
            }
            catch (Exception ex)
            {
                // Registra el error en el log.
                Logger.LogError(ex.Message);

                // Muestra un mensaje de error al usuario.
                Mensajes.MensajeError("Error al cargar los datos del empleado\n" + ex.Message);
            }
        }

        /// <summary>
        /// Trae y asigna la lista de contactos al origen de datos.
        /// </summary>
        /// <param name="contactos">Lista de contactos a mostrar en la grilla.</param>
        /// <returns>
        /// Devuelve true si la lista es válida y se asigna correctamente; 
        /// false si la lista es nula o está vacía.
        /// </returns>
        public bool TraerContactos(List<Contactos>? contactos)
        {
            // Valida que la lista no sea nula
            if (contactos == null)
            {
                return false;
            }

            // Valida que la lista contenga al menos un elemento
            if (contactos.Count < 1)
            {
                return false;
            }

            // Limpia el origen de datos previo
            bindingContactos.DataSource = null;
            _contactos = null;

            // Asigna la nueva lista de contactos
            _contactos = contactos;

            // Refresca la grilla para mostrar los datos actualizados
            RefrescarGrilla();

            editandoContactos = false;

            return true;
        }

        #endregion

        #region Acciones directas

        /// <summary>
        /// Cierra el formulario actual previa confirmación del usuario.
        /// </summary>
        /// <remarks>
        /// - Solicita confirmación mediante <c>Mensajes.confirmarCierre()</c>.
        /// - Si el usuario confirma, marca la bandera <c>cerrando</c> y procede a cerrar el formulario.
        /// - Si el usuario cancela, no realiza ninguna acción.
        /// </remarks>
        private void CerrarFormulario()
        {
            // Solicita confirmación antes de cerrar
            if (!Mensajes.ConfirmarCierre())
                return;

            // Marca el estado de cierre y cierra el formulario
            cerrando = true;
            this.Close();
        }

        /// <summary>
        /// Controla la lógica para permitir el registro de un nuevo empleado.
        /// </summary>
        /// <remarks>
        /// - Si el DNI no existe, habilita los campos editables y limpia valores previos.
        /// - Si el DNI ya existe, cambia el modo a <c>Modificación</c> y delega en <c>PermitirModificacion</c>.
        /// </remarks>
        /// <param name="existe">Indica si el DNI ya pertenece a un empleado existente.</param>
        private void PermitirRegistroNuevo(bool existe)
        {
            if (!existe)
            {
                if (_barbero?.Personas != null)
                {
                    Mensajes.MensajeExito($"El DNI: {txtDni.Text} ya es un cliente registrado. Cargando datos");
                    PrepararModificacion();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDni.Text.Trim()))
                {
                    Mensajes.MensajeError("El campo DNI no puede estar vacío para registrar un nuevo empleado.");
                    return;
                }

                // Si el DNI está libre, se habilita el registro
                Mensajes.MensajeExito($"El DNI: {txtDni.Text} está libre para utilizar. Habilitando registro");
                ActivarCamposEditables(true);
                LimpiarValores(true);
                return;
            }

            // Si el DNI ya existe, se cambia a modo modificación
            modo = EnumModoForm.Modificacion;
            PermitirModificacion(existe);

            // Nota de mejora: podría simplificarse la lógica evitando el else,
            // ya que el return en el primer bloque corta la ejecución.
        }

        /// <summary>
        /// Controla la lógica para permitir la modificación de un empleado existente.
        /// </summary>
        /// <remarks>
        /// - Si el DNI existe, pregunta al usuario si desea modificar los datos.
        ///   - Si responde No, prepara el formulario para Alta.
        ///   - Si responde Sí, habilita los campos editables.
        /// - Si el DNI no existe, pregunta si desea registrar un nuevo empleado.
        ///   - Si responde No, desactiva campos y limpia valores.
        ///   - Si responde Sí, delega en <c>PermitirRegistroNuevo</c>.
        /// </remarks>
        /// <param name="existe">Indica si el DNI ya pertenece a un empleado existente.</param>
        private void PermitirModificacion(bool existe)
        {
            if (existe)
            {
                // Pregunta al usuario si desea modificar los datos del empleado existente
                if (!ConfirmarModificacion(txtDni.Text, _barbero?.TipoEmpleado ?? "empleado", _barbero?.NombreCompleto))
                {
                    // Si no desea modificar, se prepara el formulario para Alta
                    PrepararAlta();
                    return;
                }

                // Si desea modificar, se habilitan los campos
                PrepararModificacion();
            }
            else
            {
                // Pregunta al usuario si desea registrar un nuevo empleado
                if (!ConfirmarRegistroNuevo(txtDni.Text))
                {
                    // Si no desea registrar, se desactivan campos y se limpian valores
                    ActivarCamposEditables(false);
                    LimpiarValores(true);
                    return;
                }

                // Si desea registrar, se delega en PermitirRegistroNuevo
                PermitirRegistroNuevo(existe);
            }
        }

        /// <summary>
        /// Procesa la búsqueda de un empleado según el modo actual del formulario.
        /// </summary>
        /// <remarks>
        /// - Si el modo es <c>Alta</c>, delega en <c>PermitirRegistroNuevo</c>.
        /// - Si el modo es <c>Modificación</c>, delega en <c>PermitirModificacion</c>.
        /// </remarks>
        /// <param name="existe">Indica si el DNI ya pertenece a un empleado existente.</param>
        private void ProcesarBusquedaEmpleado(bool existe)
        {
            if (modo == EnumModoForm.Alta)
                PermitirRegistroNuevo(existe);
            else
                PermitirModificacion(existe);
        }

        /// <summary>
        /// Intenta modificar un empleado existente en la base de datos enviando las entidades actualizadas a la capa de negocio.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método actúa como puente entre la interfaz de usuario y la lógica de negocio para la actualización.
        /// Empaqueta la entidad principal (<c>_barbero</c>) junto con sus dependencias (<c>_contactos</c>).
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) que contendrá el detalle del éxito o el motivo del error.</param>
        /// <returns><c>true</c> si la modificación en la base de datos fue exitosa; de lo contrario, <c>false</c>.</returns>
        private bool ModificarEmpleado(out string mensaje)
        {
            // 1. Inicialización obligatoria del parámetro de salida
            mensaje = string.Empty;

            try
            {
                // 2. Llamada a la capa de negocio (Patrón Resultado)
                var resultado = EmpleadosNegocio.ModificarEmpleado(_barbero, _contactos);

                // 3. Evaluación de la respuesta
                if (!resultado.Success)
                {
                    // Fallo en la actualización (ej: problemas de concurrencia, validaciones de negocio)
                    mensaje = $"Error en la modificación:\n{resultado.Mensaje}";
                    return false;
                }

                // 4. Éxito
                mensaje = $"Modificación exitosa.\n{resultado.Mensaje}";
                return true;
            }
            catch (Exception ex)
            {
                // 5. Manejo de excepciones no controladas durante la transacción
                Logger.LogError($"Error crítico en ModificarEmpleado: {ex.ToString()}");
                mensaje = "Ocurrió un error técnico inesperado al intentar actualizar el barbero en la base de datos.";
                return false;
            }
        }

        /// <summary>
        /// Intenta registrar un nuevo empleado en la base de datos enviando las entidades a la capa de negocio.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método actúa como puente entre la interfaz de usuario y la lógica de negocio para la inserción.
        /// Empaqueta la entidad principal (<c>_barbero</c>) junto con sus dependencias (<c>_contactos</c>).
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) que contendrá el detalle del éxito o el motivo del error.</param>
        /// <returns><c>true</c> si el registro en la base de datos fue exitoso; de lo contrario, <c>false</c>.</returns>
        private bool RegistrarEmpleado(out string mensaje)
        {
            // 1. Inicialización obligatoria del parámetro de salida
            mensaje = string.Empty;

            try
            {
                // 2. Llamada a la capa de negocio (Asumiendo que ahora devuelve un objeto Resultado)
                var resultado = EmpleadosNegocio.RegistrarEmpleado(_barbero, _contactos);

                // 3. Evaluación de la respuesta del servidor/negocio
                if (!resultado.Success)
                {
                    // Fallo en la inserción (ej: violaciones de clave foránea, timeout, etc.)
                    mensaje = $"Error en el registro:\n{resultado.Mensaje}";
                    return false;
                }

                // 4. Éxito
                mensaje = $"Registro exitoso.\n{resultado.Mensaje}";
                return true;
            }
            catch (Exception ex)
            {
                // 5. Manejo de catástrofes imprevistas durante la transacción
                Logger.LogError($"Error crítico en RegistrarEmpleado: {ex.ToString()}");
                mensaje = "Ocurrió un error técnico inesperado al intentar guardar el barbero en la base de datos.";
                return false;
            }
        }

        #endregion

        #endregion

    }
}
