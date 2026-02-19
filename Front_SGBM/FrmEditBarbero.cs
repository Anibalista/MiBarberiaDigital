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
            cargarFormulario(); // Carga datos iniciales según el modo.
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
        private void cargarFormulario()
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
        /// Carga la lista de estados para la índole "Empleados"
        /// y asegura que siempre exista al menos un estado "Activo".
        /// </summary>
        /// <remarks>
        /// - Reinicia el binding y las referencias antes de cargar.
        /// - Obtiene los estados desde la capa de negocio.
        /// - Si ocurre un error o no hay estados válidos, crea uno por defecto.
        /// - Asigna la lista al binding y delega la selección al método correspondiente.
        /// - Configura la habilitación del combo según disponibilidad.
        /// </remarks>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la carga y se registra en el log.
        /// </exception>
        private void CargarEstados()
        {
            cargando = true;     // Activa la bandera que detiene los eventos
            string mensaje = ""; // Mensaje de error o advertencia devuelto por la capa de negocio
            try
            {
                // Reinicia las referencias y el binding antes de cargar nuevos datos
                bindingEstados.DataSource = null;
                _estado = null;
                _estados = null;

                // Obtiene la lista de estados desde la capa de negocio
                _estados = EstadosNegocio.getEstadosPorIndole("Empleados", ref mensaje);

                // Si hubo un mensaje de error o no se obtuvieron estados válidos
                if (!String.IsNullOrWhiteSpace(mensaje) || _estados == null)
                {
                    // Se crea una lista mínima con un estado "Activo" por defecto
                    _estados = new List<Estados> { new Estados { Indole = "Empleados", Estado = "Activo", IdEstado = 0 } };

                    // Se muestra el mensaje de error al usuario
                    Mensajes.MensajeError(mensaje);

                    // Se corta la ejecución para evitar continuar con datos inválidos
                    return;
                }

                // Asigna la lista de estados al binding para que se refleje en la UI
                bindingEstados.DataSource = _estados;

                // Delegar la lógica de selección al método especializado
                SeleccionarEstado();

                // Habilita el combo solo si hay estados disponibles
                cbEstados.Enabled = bindingEstados.Count > 0;
            }
            catch (Exception ex)
            {
                // Registra el error en el log para diagnóstico
                Logger.LogError(ex.Message);

                // Deshabilita el combo en caso de error
                cbEstados.Enabled = false;
                return;
            }
            finally
            {
                // Desactiva la bandera que detiene los eventos
                cargando = false;
            }
        }

        /// <summary>
        /// Carga la lista de provincias desde la capa de negocio y la asigna al binding.
        /// </summary>
        /// <remarks>
        /// - Reinicia el binding y las referencias antes de cargar.
        /// - Obtiene las provincias desde <c>DomiciliosNegocio</c>.
        /// - Si no se obtienen provincias, inicializa una lista vacía.
        /// - Asigna la provincia correspondiente a la localidad actual si existe.
        /// - Configura la selección del combo de provincias.
        /// </remarks>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la carga y se registra en el log.
        /// </exception>
        private void CargarProvincias()
        {
            cargando = true; // Flag para indicar que se está cargando información

            // Reinicia binding y referencias
            bindingProvincias.DataSource = null;
            _provincias = null;
            _provincia = null;

            try
            {
                // Obtiene la lista de provincias o inicializa una nueva si es null con Entre Ríos como genérica
                _provincias = DomiciliosNegocio.GetProvincias()
                    ?? new List<Provincias> { new Provincias { Provincia = "Entre Ríos" } };

                // Asigna la lista al binding
                bindingProvincias.DataSource = _provincias;

                // Si existe una localidad, intenta recuperar su provincia
                if (_localidad != null)
                {
                    _provincia = _provincias.FirstOrDefault(p => p.IdProvincia == _localidad.IdProvincia);
                }

                // Configura la selección del combo según la provincia encontrada
                if (_provincia != null)
                    cbProvincia.SelectedValue = _provincia.IdProvincia;
                else
                    cbProvincia.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Registra el error en el log
                Logger.LogError(ex.Message);
            }
            finally
            {
                cargando = false; //Se desactiva la bandera para que los eventos sigan su ejecución
            }
        }

        /// <summary>
        /// Carga las localidades correspondientes a la provincia seleccionada
        /// y las asigna al binding del combo de localidades.
        /// </summary>
        /// <remarks>
        /// - Reinicia el binding y la lista de localidades antes de cargar.
        /// - Si no hay provincias cargadas, finaliza inmediatamente.
        /// - Obtiene la provincia seleccionada en el combo.
        /// - Si la provincia es válida, carga sus localidades desde la capa de negocio.
        /// - Asigna la lista de localidades al binding.
        /// - Configura la selección del combo de localidades según la localidad actual.
        /// - Maneja errores registrándolos en el log y asegura que el flag <c>cargando</c> se libere.
        /// </remarks>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la carga y se registra en el log.
        /// </exception>
        private void CargarLocalidades()
        {
            // Reinicia binding y referencias
            bindingLocalidades.DataSource = null;
            _localidades = null;

            // Si no hay provincias cargadas, no se puede continuar
            if (_provincias?.Count < 1)
            {
                cbLocalidad.Enabled = false;
                return;
            }
            string provincia = cbProvincia.Text; // Provincia seleccionada en el combo

            // Si no hay texto de provincia, se asigna lista vacía y se corta
            if (String.IsNullOrWhiteSpace(provincia))
            {
                bindingLocalidades.DataSource = null;
                cbLocalidad.Enabled = false;
                return;
            }

            try
            {
                // Busca la provincia seleccionada en la lista
                _provincia = _provincias?
                    .FirstOrDefault(p => p.Provincia.Equals(provincia, StringComparison.OrdinalIgnoreCase));

                // Si se encontró la provincia, carga sus localidades
                if (_provincia != null)
                    _localidades = DomiciliosNegocio.GetLocalidadesPorProvincia(_provincia) ?? new List<Localidades>();

                // Asigna la lista de localidades al binding
                bindingLocalidades.DataSource = _localidades;

                //Activa o desactiva el combo box según si se seleccionó o escribió una provincia
                cbLocalidad.Enabled = _provincia != null;

                // Configura la selección del combo según la localidad actual
                if (_localidad != null)
                    cbLocalidad.SelectedValue = _localidad.IdLocalidad;
                else
                    cbLocalidad.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Registra el error en el log
                Logger.LogError(ex.Message);
            }
            finally
            {
                // Libera el flag de carga siempre
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
        /// Carga los contactos asociados a la persona actual y los asigna al binding.
        /// </summary>
        /// <remarks>
        /// - Reinicia el binding y la lista de contactos antes de cargar.
        /// - Si no hay barbero o persona, refresca la grilla y finaliza.
        /// - Obtiene los contactos desde la capa de negocio.
        /// - Registra en el log cualquier error devuelto por la capa de negocio.
        /// - Siempre refresca la grilla al finalizar, incluso si ocurre una excepción.
        /// </remarks>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la carga y se registra en el log.
        /// </exception>
        private void CargarContactos()
        {
            // Reinicia el binding y la lista de contactos
            bindingContactos.DataSource = null;
            _contactos = null;

            // Si no hay barbero o persona, refresca la grilla y termina
            if (_barbero == null || _persona == null)
            {
                RefrescarGrilla();
                return;
            }

            try
            {
                string error = ""; // Variable para capturar errores desde la capa de negocio

                // Obtiene los contactos de la persona
                _contactos = ContactosNegocio.getContactosPorPersona(_persona, ref error);

                // Si hubo error, se registra en el log
                if (!String.IsNullOrWhiteSpace(error))
                    Logger.LogError(error);

                //Refresca la grilla
                RefrescarGrilla();
            }
            catch (Exception ex)
            {
                // Registra cualquier excepción inesperada
                Logger.LogError(ex.Message);
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
        /// Maneja el evento de clic en el botón Buscar.
        /// </summary>
        /// <remarks>
        /// - Invoca la búsqueda de un empleado por DNI.
        /// - Si la búsqueda devuelve un mensaje de error, lo muestra al usuario y detiene la ejecución.
        /// - Si no hay errores, delega la lógica en <c>ProcesarBusquedaEmpleado</c>.
        /// </remarks>
        /// <param name="sender">El botón que dispara el evento.</param>
        /// <param name="e">Argumentos del evento <c>Click</c>.</param>
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string mensaje = "";

            // Busca al empleado y obtiene un mensaje de error si corresponde
            bool existe = BuscarEmpleado(ref mensaje);

            // Si hay mensaje de error, se muestra y se detiene
            if (!String.IsNullOrWhiteSpace(mensaje))
            {
                Mensajes.MensajeError($"Error: {mensaje}");
                return;
            }

            // Procesa la búsqueda según el modo actual del formulario
            ProcesarBusquedaEmpleado(existe);
        }

        /// <summary>
        /// Handler del botón Guardar: valida, asigna datos y ejecuta registro o modificación.
        /// </summary>
        /// <param name="sender">Origen del evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Valida condiciones previas al guardado (confirmaciones, validaciones).
            if (!ContinuarGuardando())
                return;

            // Asigna la persona al modelo y su Id (manejo seguro de null).
            _barbero.Personas = _persona;
            _barbero.IdPersona = _persona?.IdPersona ?? 0;

            string mensaje = string.Empty;
            bool exito = false;

            // Decide si registra o modifica según el modo del formulario.
            if (modo == EnumModoForm.Alta)
                exito = RegistrarEmpleado(ref mensaje);
            else
                exito = ModificarEmpleado(ref mensaje);

            // Si la operación falló, muestra el error y corta el flujo.
            if (!exito)
            {
                Mensajes.MensajeError(mensaje);
                return;
            }

            // Si fue exitosa, pregunta si el usuario quiere registrar otro barbero.
            DialogResult respuesta = Mensajes.Respuesta(mensaje + "\n¿Desea registrar un nuevo Barbero?");
            if (respuesta == DialogResult.Yes)
            {
                LimpiarValores(true);
                LimpiarCampos();
                modo = EnumModoForm.Alta;
                cargarFormulario();
                ActivarCamposEditables(false);
                return;
            }

            // Cierra el formulario si no se desea continuar.
            cerrando = true;
            this.Close();
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
        /// Comprueba que el DNI ingresado sea válido.
        /// </summary>
        /// <param name="mensaje">Mensaje de error si la validación falla.</param>
        /// <returns>True si el DNI es válido, False en caso contrario.</returns>
        private bool ComprobarDni(ref string mensaje)
        {
            if (!ValidarCampoNumerico(txtDni, true))//Valida y resalta el error en el campo con el provider o lo limpia
                return false;

            // Valida la longitud del DNI (entre 6 y 9 caracteres)
            if (txtDni.Text.Length > 9 || txtDni.Text.Length < 6)
            {
                mensaje = "La longitud del Dni es menor o mayor a lo esperado";
                ErrorCampo(txtDni, mensaje); //Resalta el error en el campo con el provider
                return false;
            }

            ErrorCampo(txtDni); //Si todo está correcto limpia el error en el provider

            return true; // DNI válido
        }

        /// <summary>
        /// Comprueba que los nombres y apellidos sean correctos y los capitaliza.
        /// </summary>
        /// <param name="mensaje">Mensaje de error si la validación falla.</param>
        /// <returns>True si los nombres son válidos, False en caso contrario.</returns>
        private bool ComprobarNombres(ref string mensaje)
        {
            _persona = _persona ?? new(); // Inicializa la persona si es null

            // Asigna nombres y apellidos desde los textbox
            _persona.Nombres = txtNombre.Text.Trim();
            _persona.Apellidos = txtApellido.Text.Trim();

            string mensApellido = string.Empty;
            mensaje = string.Empty;

            // Valida nombres y apellidos usando helper de Validaciones
            if (!Validaciones.TextoCorrecto(_persona.Nombres, ref mensaje) ||
                !Validaciones.TextoCorrecto(_persona.Apellidos, ref mensApellido))
            {
                //Carga los errores en el provider y devuelve el mensaje de error
                ErrorCampo(txtNombre, mensaje);
                ErrorCampo(txtApellido, mensApellido);
                mensaje = "Se detectaron problemas con los nombres o apellidos";
                return false;
            }
            else
            {
                //Si no existen errores limpia el error de los campos
                ErrorCampo(txtNombre);
                ErrorCampo(txtApellido);
            }
            // Capitaliza nombres y apellidos para uniformidad
            _persona.Nombres = Validaciones.CapitalizarTexto(_persona.Nombres);
            _persona.Apellidos = Validaciones.CapitalizarTexto(_persona.Apellidos);

            return true; // Nombres válidos
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
                Logger.LogError(ex.Message);
            }
            finally
            {
                // Asigna el mensaje de error al control DateTimePicker
                ErrorCampo(dateTimeNacimiento, mensaje);
            }
        }

        /// <summary>
        /// Comprueba la validez de los datos de un empleado (barbero) antes de registrar o modificar.
        /// Verifica duplicidad de DNI, nombres, fecha de nacimiento y estado seleccionado.
        /// </summary>
        /// <param name="mensaje">
        /// Referencia a un string donde se almacenará un mensaje de error o advertencia en caso de que la validación falle.
        /// </param>
        /// <param name="registro">
        /// Indica si la operación corresponde a un registro nuevo (true) o a una modificación existente (false).
        /// </param>
        /// <returns>
        /// True si todas las comprobaciones fueron exitosas; False si se detecta algún error o inconsistencia.
        /// </returns>
        private bool ComprobarEmpleado(ref string mensaje, bool registro)
        {
            // Se busca si ya existe un empleado con el mismo DNI
            if (BuscarEmpleado(ref mensaje))
            {
                if (registro)
                {
                    // Si es un registro nuevo y el DNI ya existe, se rechaza
                    mensaje = "El Dni ingresado ya pertenece a otro barbero";
                    return false;
                }
                else if (_barbero.Personas?.Dni != _persona?.Dni)
                {
                    // Si es una modificación y el nuevo DNI coincide con otro empleado, se rechaza
                    mensaje = $"El Dni nuevo que ingresó ya pertenece a {_barbero.NombreCompleto}";
                    return false;
                }
            }

            // Se asigna la persona asociada al barbero actual
            _persona = _barbero.Personas;

            // Se comprueban los nombres; si falla, se retorna false con mensaje
            if (!ComprobarNombres(ref mensaje))
            {
                return false;
            }

            // Se comprueba la fecha de nacimiento (sin retorno, solo validación interna)
            ComprobarNacimiento();

            try
            {
                // Se obtiene el estado seleccionado en el combo
                _estado = (Estados)cbEstados.SelectedItem;

                // Si el estado no se recupera correctamente en modo modificación, se rechaza
                if (_estado?.IdEstado == null && modo == EnumModoForm.Modificacion)
                {
                    mensaje = "Problemas con la recuperación de estados de empleados de la BD";
                    return false;
                }

                //Asigna el estado al barbero asi sea nuevo
                _barbero.Estados = _estado;
                _barbero.IdEstado = _estado?.IdEstado ?? 0;
            }
            catch (Exception ex)
            {
                // Si ocurre una excepción, se captura el mensaje y se retorna false
                mensaje = ex.Message;
                return false;
            }

            // Si todas las comprobaciones fueron exitosas, se retorna true
            return true;
        }

        /// <summary>
        /// Verifica si se ha ingresado una dirección válida en los campos de texto
        /// y asigna el objeto <c>_domicilio</c> en consecuencia.
        /// </summary>
        /// <remarks>
        /// - Considera válida la dirección si al menos se ingresó calle o barrio.
        /// - Valida que se haya ingresado o seleccionado una localidad.
        /// - Construye un objeto <c>Domicilios</c> con los valores de los controles.
        /// - Valida cada campo con <c>Validaciones.textoCorrecto</c>, asignando null si no es correcto.
        /// - Si no se ingresó información suficiente, muestra advertencia y no registra domicilio.
        /// - Registra en el log cualquier excepción y muestra un mensaje de error al usuario.
        /// </remarks>
        /// <param name="mensaje">
        /// Referencia a un string donde se almacenará un mensaje de error o advertencia en caso de validación fallida.
        /// </param>
        /// <returns>
        /// <c>true</c> si se ingresó una dirección válida (calle o barrio y localidad);
        /// <c>false</c> si no se ingresó información suficiente o ocurre una excepción.
        /// </returns>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la validación y se registra en el log.
        /// </exception>
        private bool DireccionIngresada(ref string mensaje)
        {
            // Se considera válido si al menos hay calle o barrio
            if (ValidarTextoObligatorio(txtCalle) || ValidarTextoObligatorio(txtBarrio))
            {
                // Valida que se haya ingresado o seleccionado una localidad
                if (!LocalidadIngresada(ref mensaje))
                {
                    ErrorCampo(cbLocalidad, mensaje);
                    ErrorCampo(cbProvincia, mensaje);
                    return false;
                }
                else
                {
                    // Limpia errores previos si la localidad es válida
                    ErrorCampo(cbLocalidad);
                    ErrorCampo(cbProvincia);
                }
            }
            else
            {
                // Si no hay calle ni barrio, no se registra domicilio
                _domicilio = null;
                mensaje = "No se registrará ningún domicilio";
                return false;
            }

            try
            {
                // Construye el objeto domicilio con los valores validados
                Domicilios domicilioIngresado = new Domicilios
                {
                    IdDomicilio = _domicilio?.IdDomicilio,
                    Calle = string.IsNullOrWhiteSpace(txtCalle.Text) ? null : txtCalle.Text,
                    Barrio = string.IsNullOrWhiteSpace(txtBarrio.Text) ? null : txtBarrio.Text,
                    Altura = Validaciones.TextoCorrecto(txtNro.Text.Trim(), ref mensaje)
                        ? txtNro.Text.Trim() : null,
                    Piso = Validaciones.TextoCorrecto(txtPiso.Text.Trim(), ref mensaje)
                        ? txtPiso.Text.Trim() : null,
                    Depto = Validaciones.TextoCorrecto(txtDepto.Text.Trim(), ref mensaje)
                        ? txtDepto.Text.Trim() : null,
                    IdLocalidad = _localidad?.IdLocalidad ?? 0,
                    Localidades = _localidad?.IdLocalidad > 0
                        ? _localidad : null
                };

                // Asigna el domicilio si fue válido
                _domicilio = domicilioIngresado;
                return true;
            }
            catch (Exception ex)
            {
                // Registra el error y muestra mensaje al usuario
                Logger.LogError(ex.Message);
                mensaje = $"Error inesperado en el domicilio \n{ex.Message}\nNo se registrará ningún domicilio";
                _domicilio = null;
                return false;
            }
        }

        /// <summary>
        /// Verifica si se ha ingresado una localidad válida en el combo
        /// y asigna el objeto <c>_localidad</c> en consecuencia.
        /// </summary>
        /// <remarks>
        /// - Requiere que previamente se haya ingresado una provincia válida.
        /// - Si el texto de localidad está vacío, devuelve <c>false</c> y asigna un mensaje.
        /// - Si existen localidades cargadas, busca coincidencia por nombre.
        /// - Si no encuentra coincidencia, crea una nueva localidad capitalizando el texto.
        /// - Asigna la provincia correspondiente a la localidad.
        /// - Registra en el log cualquier excepción y devuelve <c>false</c>.
        /// </remarks>
        /// <param name="mensaje">
        /// Mensaje de error o advertencia que se devuelve al usuario en caso de fallo.
        /// </param>
        /// <returns>
        /// <c>true</c> si se ingresó una localidad válida o se creó una nueva; 
        /// <c>false</c> si no hay provincia, el texto está vacío o ocurre una excepción.
        /// </returns>
        /// <exception cref="Exception">
        /// Se captura cualquier excepción durante la validación y se registra en el log.
        /// </exception>
        private bool LocalidadIngresada(ref string mensaje)
        {
            _localidad = null;
            mensaje = "";

            // Verifica que haya una provincia válida
            if (!ProvinciaIngresada())
            {
                mensaje = "Para ingresar un domicilio escriba o seleccione una localidad y provincia";
                return false;
            }

            string localidad = cbLocalidad.Text.Trim();

            // Si el texto de localidad está vacío, no hay localidad válida
            if (String.IsNullOrWhiteSpace(localidad))
            {
                mensaje = "Para ingresar un domicilio escriba o seleccione una localidad y provincia";
                return false;
            }

            try
            {
                // Si hay localidades cargadas, busca coincidencia por nombre
                if (_localidades?.Count > 0)
                    _localidad = _localidades.FirstOrDefault(l => l.Localidad.Equals(localidad, StringComparison.OrdinalIgnoreCase));

                // Si no se encontró, se crea una nueva capitalizando el texto
                _localidad ??= new Localidades { Localidad = Validaciones.CapitalizarTexto(localidad, true) };

                // Asigna la provincia correspondiente
                _localidad.IdProvincia = _provincia?.IdProvincia ?? 0;

                // Si la provincia no tiene Id, asigna el objeto provincia directamente
                _localidad.Provincias = _localidad.IdProvincia == 0 ? _provincia : null;

                return _localidad != null;
            }
            catch (Exception ex)
            {
                // Registra el error y devuelve false
                Logger.LogError(ex.Message);
                mensaje = "Error inesperado " + ex.Message;
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
        /// Valida las condiciones necesarias antes de guardar los datos de contactos.
        /// </summary>
        /// <returns>
        /// Devuelve true si las validaciones se cumplen y se puede continuar con el guardado.
        /// Devuelve false si alguna validación crítica falla.
        /// </returns>
        private bool ContinuarGuardando()
        {
            // Confirma si se puede continuar, verificando edición activa de contactos
            if (!ConfirmarContactos())
                return false;

            string mensaje = string.Empty;

            // Comprueba que el empleado sea válido según el modo actual (Alta o Edición)
            if (!ComprobarEmpleado(ref mensaje, modo == EnumModoForm.Alta))
            {
                // Muestra error si la validación del empleado falla
                Mensajes.MensajeError($"Error: {mensaje}");
                return false;
            }

            // Verifica si la dirección fue ingresada correctamente
            if (!DireccionIngresada(ref mensaje))
                // Muestra advertencia si la dirección no es válida, pero permite continuar
                Mensajes.MensajeAdvertencia(mensaje);

            // Asigna el domicilio a la persona así sea null 
            _persona.Domicilios = _domicilio;
            _persona.IdDomicilio = _domicilio?.IdDomicilio;

            // Si todas las validaciones críticas se cumplen, permite continuar
            return true;
        }

        #endregion

        #region Acciones y Métodos

        #region Búsquedas
        /// <summary>
        /// Busca un empleado por DNI y asigna el resultado a _barbero.
        /// </summary>
        /// <param name="mensaje">Mensaje de error o validación.</param>
        /// <returns>True si se encontró el empleado, False en caso contrario.</returns>
        private bool BuscarEmpleado(ref string mensaje)
        {
            if (modo == EnumModoForm.Alta)
            {
                // Reinicia la referencia antes de buscar cuando es un nuevo registro 
                _barbero = null;
                _persona = null;
            }

            // Valida el DNI antes de continuar
            if (!ComprobarDni(ref mensaje))
                return false;

            // Busca el empleado en la capa de negocio usando el DNI
            _barbero = EmpleadosNegocio.GetEmpleadoPorDni(txtDni.Text, ref mensaje);

            // Retorna true si se encontró un empleado válido
            return _barbero?.IdEmpleado != null;
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
        /// Inicializa las variables internas del formulario a partir de los datos del objeto <c>_barbero</c>.
        /// </summary>
        /// <remarks>
        /// - Si <c>_barbero</c> o <c>_barbero.Personas</c> son nulos, no realiza ninguna acción.
        /// - Asigna la persona asociada al barbero, creando una nueva si no existe.
        /// - Asigna el domicilio de la persona, creando uno nuevo si no existe.
        /// - Recupera el estado del barbero.
        /// - Obtiene la localidad y provincia asociadas al domicilio.
        /// </remarks>
        private void LlenarVariables()
        {
            // Si no hay barbero cargado, no se procede
            if (_barbero == null || _barbero?.Personas == null)
            {
                LimpiarValores(false);
                return;
            }

            // Asigna la persona asociada al barbero
            _persona = _barbero.Personas;

            // Asigna el domicilio de la persona, o crea uno nuevo si no existe
            _domicilio = _persona.Domicilios ?? new();

            // Recupera el estado actual del barbero
            _estado = _barbero.Estados;

            // Obtiene la localidad asociada al domicilio
            _localidad = _domicilio?.Localidades;

            // Obtiene la provincia asociada a la localidad del domicilio
            _provincia = _domicilio == null ? null : _domicilio.Localidades?.Provincias;

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
        /// Intenta modificar el empleado existente usando los datos actuales.
        /// </summary>
        /// <param name="mensaje">Mensaje de salida con detalle del resultado o error.</param>
        /// <returns>True si la modificación fue exitosa; false en caso contrario.</returns>
        private bool ModificarEmpleado(ref string mensaje)
        {
            // Llama al negocio para modificar el empleado con la lista de contactos.
            // Si falla, antepone un texto explicativo al mensaje devuelto por la capa de negocio.
            if (!EmpleadosNegocio.modificarEmpleado(_barbero, _contactos, ref mensaje))
            {
                mensaje = "Error en la modificación\n" + mensaje;
                return false;
            }
            else
            {
                // Si la operación fue exitosa, informa el éxito y concatena detalles adicionales.
                mensaje = "Modificación exitosa\n" + mensaje;
                return true;
            }
        }

        /// <summary>
        /// Intenta registrar un nuevo empleado usando los datos actuales.
        /// </summary>
        /// <param name="mensaje">Mensaje de salida con detalle del resultado o error.</param>
        /// <returns>True si el registro fue exitoso; false en caso contrario.</returns>
        private bool RegistrarEmpleado(ref string mensaje)
        {
            // Llama al negocio para registrar el empleado.
            // Si falla, antepone un texto explicativo al mensaje devuelto por la capa de negocio.
            if (!EmpleadosNegocio.RegistrarEmpleado(_barbero, _contactos, ref mensaje))
            {
                mensaje = "Error en el registro\n" + mensaje;
                return false;
            }
            else
            {
                // Si la operación fue exitosa, informa el éxito y concatena detalles adicionales.
                mensaje = "Registro exitoso\n" + mensaje;
                return true;
            }
        }

        #endregion

        #endregion

    }
}
