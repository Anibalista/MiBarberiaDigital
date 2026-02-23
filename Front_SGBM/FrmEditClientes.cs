using Entidades_SGBM;
using Negocio_SGBM;
using Utilidades;

namespace Front_SGBM
{
    public partial class FrmEditClientes : Form
    {
        #region Declaraciones y eventos

        #region Variables de clase

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
        public bool editandoContactos = false;
        private bool cargando = false;

        #endregion

        public FrmEditClientes()
        {
            InitializeComponent();
            // La configuración de fechas la movemos a FechaInicial() para tener toda la lógica junta.
        }

        #region Eventos de Formulario

        #region Listeners y Load

        private void FrmEditClientes_Load(object sender, EventArgs e)
        {
            // 1. Comprobamos que el modo de apertura sea coherente con los datos en memoria y ajustamos si es necesario
            if (!ValidarModo())
            {
                CerrarFormulario(sender, e);
            }

            // 2. Práctica mantener el Load con una sola línea que llame al orquestador.
            CargarFormulario();
        }

        /// <summary>
        /// Restringe la entrada del TextBox exclusivamente a caracteres numéricos y teclas de control.
        /// </summary>
        /// <param name="sender">El TextBox del DNI.</param>
        /// <param name="e">Argumentos del evento de teclado.</param>
        private void Numeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si la tecla presionada NO es un número y NO es una tecla de control (como borrar)...
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // ... marcamos el evento como "manejado", lo que anula la pulsación y no la escribe.
                e.Handled = true;
            }
        }

        /// <summary>
        /// Detecta el cambio de provincia y dispara la carga de sus localidades correspondientes.
        /// </summary>
        /// <param name="sender">El ComboBox de provincias.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void CbProvincia_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ¡CLAVE!: Si el formulario se está cerrando o si estamos llenando los combos por código, 
            // abortamos para evitar bucles o consultas inútiles a la BD.
            if (cerrando || cargando)
            {
                return;
            }
            _provincia = null; // Limpiamos la provincia en memoria antes de intentar asignar la nueva selección
            try
            {
                _provincia = CbProvincia.SelectedItem as Provincias;
                CargarLocalidades();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error en CbProvincia_SelectedIndexChanged: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error al intentar cargar las localidades para la provincia seleccionada.");
            }
        }

        #endregion

        #region Accionables

        /// <summary>
        /// Abre el panel/formulario de gestión de contactos asociado al cliente actual.
        /// </summary>
        /// <param name="sender">El control LinkLabel que disparó el evento.</param>
        /// <param name="e">Argumentos del evento del link.</param>
        private void LinkContactos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 1. Levantamos la bandera para saber que estamos en sub-edición
            editandoContactos = true;

            // 2. Determinamos el modo: Si ya hay contactos cargados, abrimos en Modificación; si no, en Alta.
            EnumModoForm modoCont = ModoContactos() ? EnumModoForm.Modificacion : EnumModoForm.Alta;

            // 3. Buscamos el contenedor principal (MDI o Pseudo-MDI)
            FrmMenuPrincipal? principal = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();

            // 4. Cláusula de guarda con registro preventivo
            if (principal == null)
            {
                Logger.LogError("Se intentó abrir contactos, pero no se encontró el FrmMenuPrincipal activo.");
                return;
            }

            // 5. Apertura delegada al menú principal
            principal.AbrirFrmContactos(modoCont, "Clientes", _contactos, pnlContent);
        }

        /// <summary>
        /// Maneja el clic en el botón Cancelar / Salir, delegando la lógica a <see cref="CerrarFormulario"/>.
        /// </summary>
        /// <param name="sender">El botón que dispara el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            // Código súper limpio que solo delega la acción
            CerrarFormulario(sender, e);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {

        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion

        #endregion

        #region Cargas de valores

        /// <summary>
        /// Orquesta la inicialización de la interfaz, carga de catálogos y volcado de datos según el modo de apertura.
        /// </summary>
        private void CargarFormulario()
        {
            try
            {
                // Bloqueamos la UI para que la carga de combos y datos no dispare eventos indeseados
                cargando = true;

                // 1. Configuración inicial del selector de fecha
                FechaInicial();

                // 2. Carga de catálogos para ComboBoxes
                CargarProvincias();
                CargarLocalidades(); // Carga vacía inicialmente
                CargarEstados();

                // 3. Volcado de datos si aplica
                if (modo == EnumModoForm.Modificacion || modo == EnumModoForm.Consulta)
                {
                    CargarDatosCliente();
                }

                // 4. Configuración visual final
                ActivarCampos(modo != EnumModoForm.Consulta);
                labelTitulo.Text = $"{GetTitulo()} de Cliente";
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error crítico al inicializar FrmEditClientes: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un problema técnico al intentar preparar la pantalla del cliente.");
                this.Close();
            }
            finally
            {
                // Liberamos la UI para que el usuario interactúe
                cargando = false;
            }
        }

        /// <summary>
        /// Consulta la base de datos y carga el catálogo completo de Provincias en el ComboBox.
        /// </summary>
        private void CargarProvincias()
        {
            try
            {
                cargando = true; // Apagamos el listener de CbProvincia_SelectedIndexChanged

                _provincia = null;

                var resultado = DomiciliosNegocio.GetProvincias();

                if (!resultado.Success)
                {
                    Logger.LogError($"Error de negocio al obtener provincias: {resultado.Mensaje}");
                    Mensajes.MensajeAdvertencia("No se pudieron cargar las provincias. Intente nuevamente más tarde.");
                }

                _provincias = resultado.Data ?? new List<Provincias>();

                // El DataSource = null previo asegura que WinForms refresque visualmente el control
                bindingProvincias.DataSource = null;
                bindingProvincias.DataSource = _provincias;
                CbProvincia.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error técnico al cargar provincias en FrmEditClientes: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado al intentar cargar la lista de provincias.");
            }
            finally
            {
                cargando = false; // Volvemos a encender el listener
            }
        }

        /// <summary>
        /// Carga las localidades correspondientes a la provincia actualmente seleccionada en el ComboBox.
        /// </summary>
        /// <remarks>
        /// Este método es invocado por el evento SelectedIndexChanged de CbProvincia.
        /// </remarks>
        private void CargarLocalidades()
        {
            try
            {
                // 1. ¡CLAVE!: Actualizamos el objeto _provincia leyendo lo que el usuario acaba de seleccionar
                _provincia = CbProvincia.SelectedItem as Provincias;

                // 2. Limpiamos la lista y la selección visual anterior
                bindingLocalidades.DataSource = null;
                _localidad = null;
                _localidades = new List<Localidades>();
                CbLocalidad.SelectedIndex = -1;

                // 3. Cláusula de guarda: Si deseleccionó la provincia, no buscamos localidades
                if (_provincia == null || _provincia.IdProvincia < 1)
                {
                    return;
                }

                // 4. Búsqueda en la capa de negocio
                var resultado = DomiciliosNegocio.GetLocalidadesPorProvincia(_provincia);

                if (!resultado.Success)
                {
                    Logger.LogError($"Error al obtener localidades para la provincia ID {_provincia.IdProvincia}: {resultado.Mensaje}");
                    Mensajes.MensajeAdvertencia("No se pudieron cargar las localidades de la provincia seleccionada.");
                }

                // 5. Asignación segura
                _localidades = resultado.Data ?? new List<Localidades>();
                bindingLocalidades.DataSource = _localidades;
                CbLocalidad.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error técnico al cargar localidades (Provincia ID: {_provincia?.IdProvincia}): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error al intentar cargar la lista de localidades.");
            }
        }

        /// <summary>
        /// Carga la lista de Estados aplicables a la índole "Clientes".
        /// </summary>
        /// <remarks>
        /// Posee un mecanismo de "fallback": si falla la conexión, inyecta un estado "Activo" en memoria 
        /// para evitar que el alta de clientes quede totalmente bloqueada.
        /// </remarks>
        private void CargarEstados()
        {
            try
            {
                _estado = null;

                var resultado = EstadosNegocio.GetEstadosPorIndole("Clientes");

                // Validación de éxito y de que la lista no venga vacía
                if (!resultado.Success || resultado.Data == null || resultado.Data.Count == 0)
                {
                    Logger.LogError($"Fallo al obtener estados. Motivo: {resultado.Mensaje}");
                    Mensajes.MensajeAdvertencia("No se pudieron cargar los estados del sistema. Se usará un estado por defecto.");

                    // RED DE SEGURIDAD (Fallback)
                    _estados = new List<Estados> { new Estados { Estado = "Activo", Indole = "Clientes" } };
                }
                else
                {
                    _estados = resultado.Data;
                }

                bindingEstados.DataSource = null;
                bindingEstados.DataSource = _estados;
                CbEstados.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error técnico al cargar estados en FrmEditClientes: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error al intentar cargar la lista de estados.");
            }
        }

        /// <summary>
        /// Orquesta la carga completa de un cliente existente, recuperando sus dependencias y volcándolas en la interfaz.
        /// </summary>
        private void CargarDatosCliente()
        {
            if (_cliente == null)
            {
                Logger.LogError("CargarDatosCliente fue llamado pero _cliente es null.");
                Mensajes.MensajeError("No se pudo cargar la información del cliente porque no se detectó un cliente en memoria.");
                return;
            }

            try
            {
                _persona = _cliente.Personas;
                _domicilio = _persona?.Domicilios;
                _localidad = _domicilio?.Localidades;
                _provincia = _localidad?.Provincias;
                _estado = _cliente.Estados;

                // Carga de listas anexas
                CargarContactos();

                if (_persona != null)
                {
                    CargarCamposClientes(); // Este método vuelca las variables a los TextBoxes
                }
                else
                {
                    Logger.LogError($"El cliente ID {_cliente.IdCliente} no tiene una persona asociada.");
                    Mensajes.MensajeError("Faltan datos esenciales de este cliente en la base de datos.");
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error crítico al cargar los datos completos del cliente en UI: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error técnico inesperado al intentar cargar la información del cliente.");
                LimpiarCampos();
            }
        }

        private void CargarContactos()
        {
            try
            {
                _contactos = new List<Contactos>();

                if (_persona == null)
                {
                    RefrescarGrilla();
                    return;
                }

                var resultado = ContactosNegocio.GetContactosPorPersona(_persona);

                if (!resultado.Success)
                {
                    Logger.LogError($"Error al obtener contactos para la persona ID {_persona.IdPersona}: {resultado.Mensaje}");
                    // No mostramos MensajeError al usuario para no ser molestos, solo dejamos la lista vacía
                }
                else
                    _contactos = resultado.Data ?? new List<Contactos>();
                

                RefrescarGrilla();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error técnico al cargar contactos en FrmEditClientes: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error al intentar cargar la lista de contactos del cliente.");
            }
        }

        /// <summary>
        /// Recibe una lista de contactos externa, la asigna a la memoria del formulario y actualiza la grilla visual.
        /// </summary>
        /// <remarks>
        /// Este método suele ser invocado desde un formulario secundario de búsqueda o carga de contactos 
        /// para inyectar los datos en este formulario de edición.
        /// </remarks>
        /// <param name="contactos">La lista de entidades <see cref="Contactos"/> a cargar.</param>
        /// <returns><c>true</c> si la lista contiene elementos y se asignó correctamente; de lo contrario, <c>false</c>.</returns>
        public bool TraerContactos(List<Contactos>? contactos)
        {
            // 1. Cláusula de guarda: Si la lista es nula o está vacía, no hay nada que cargar
            if (contactos == null || contactos.Count == 0)
            {
                return false;
            }

            // 2. Asignación directa de la lista en memoria (eliminamos la doble instanciación)
            _contactos = contactos;

            // 3. Reflejo de los datos en la interfaz de usuario
            RefrescarGrilla();

            return true;
        }

        #endregion

        #region Comprobaciones y validaciones
        
        private string GetTitulo()
        {
            return modo switch
            {
                EnumModoForm.Alta => "Alta",
                EnumModoForm.Modificacion => "Modificación",
                EnumModoForm.Consulta => "Consulta",
                _ => "Gestión"
            };
        }

        private bool ValidarModo()
        {
            if (_cliente != null && modo == EnumModoForm.Alta)
            {
                DialogResult respuesta = Mensajes.Respuesta($"Está en modo Nuevo Registro y se detectó un cliente en memoria \n¿Quiere cambiar de modo para modificar a {_cliente.NombreCompleto}?");
                if (respuesta == DialogResult.Yes)
                    modo = EnumModoForm.Modificacion;
                else
                    _cliente = null;
                
                return true;
            }

            if (_cliente == null && modo != EnumModoForm.Alta)
            {
                DialogResult respuesta = Mensajes.Respuesta($"No se detectó un cliente en memoria \n¿Quiere cambiar de modo para dar de alta un nuevo cliente?");
                if (respuesta == DialogResult.Yes)
                    modo = EnumModoForm.Alta;
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determina si existen contactos cargados en la lista de memoria.
        /// </summary>
        /// <returns><c>true</c> si la lista está instanciada y tiene al menos un contacto; de lo contrario, <c>false</c>.</returns>
        private bool ModoContactos()
        {
            // Simplificación moderna: si es null devuelve false, si no, evalúa el Count
            return _contactos?.Count > 0;
        }


        #endregion

        #region Armado de objetos para BD y desarmado para UI

        /// <summary>
        /// Vuelca los datos de la entidad cliente y sus relaciones en los controles visuales del formulario.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Mejoras aplicadas:</b>
        /// <list type="bullet">
        /// <item>Protección contra fechas inválidas en el DateTimePicker (evita crasheos si la BD tiene fechas corruptas).</item>
        /// <item>Manejo de nulos seguro al asignar textos a los TextBox.</item>
        /// <item>Uso recomendado de la bandera de carga para evitar disparar eventos visuales en cascada al cambiar ComboBoxes.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarCamposClientes()
        {
            // Cláusula de guarda: Si no hay entidades instanciadas, cortamos la ejecución.
            if (_cliente == null || _persona == null) return;

            cargando = true; // Activamos la bandera de carga para evitar eventos no deseados al asignar valores a los ComboBoxes

            try
            {
                // 1. Datos Personales (Ya sabemos que _persona no es null gracias a la cláusula de guarda)
                TxtDni.Text = _persona.Dni ?? string.Empty;
                txtNombre.Text = _persona.Nombres ?? string.Empty;
                txtApellido.Text = _persona.Apellidos ?? string.Empty;

                // 2. Fecha de Nacimiento con protección anti-crasheo
                DateTime fechaNacimiento = _persona.FechaNac ?? DateTime.Today;

                // Si la fecha que viene de la BD es menor a lo que soporta el control, usamos la actual o la mínima
                if (fechaNacimiento < dateTimePicker1.MinDate)
                {
                    fechaNacimiento = DateTime.Today;
                }

                dateTimePicker1.Value = fechaNacimiento;

                // 3. Estado
                if (_estado != null)
                {
                    CbEstados.SelectedItem = _estado;
                }

                // 4. Domicilio y Geografía
                if (_domicilio != null)
                {
                    // Asignamos string.Empty si el campo viene nulo de la BD, es más limpio para WinForms
                    txtCalle.Text = _domicilio.Calle ?? string.Empty;
                    txtBarrio.Text = _domicilio.Barrio ?? string.Empty;
                    txtNro.Text = _domicilio.Altura ?? string.Empty;
                    txtPiso.Text = _domicilio.Piso ?? string.Empty;
                    txtDepto.Text = _domicilio.Depto ?? string.Empty;

                    // Vinculación de ComboBoxes geográficos
                    if (_provincia != null) CbProvincia.SelectedItem = _provincia;
                    if (_localidad != null) CbLocalidad.SelectedItem = _localidad;
                }
            }
            catch (Exception ex)
            {
                // Registro del error en caso de que alguna asignación visual falle
                Logger.LogError($"Error al cargar los datos en la UI (Clientes): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error menor al intentar mostrar todos los datos del cliente en pantalla.");
            }
            finally
            {
                cargando = false; // Desactivamos la bandera de carga al finalizar la asignación de valores
            }
        }

        /// <summary>
        /// Limpia las variables de estado en memoria para preparar un nuevo registro o evitar cruces de datos.
        /// </summary>
        /// <param name="limpiarContactos">Indica si también se debe vaciar la lista de contactos en memoria.</param>
        /// <param name="vaciarCliente"> Indica si se debe eliminar el objeto cliente (se puede dejar para modificaciones)</param>
        private void LimpiarValores(bool limpiarContactos, bool vaciarCliente = true)
        {
            if (vaciarCliente)
                _cliente = null;
            _persona = null;
            _estado = null;
            _domicilio = null;
            _localidad = null;

            if (limpiarContactos)
                _contactos = new(); // Instancia una lista vacía en lugar de null para no romper el binding de la grilla
        }

        #endregion

        #region Interacciones con campos visuales (limpieza, activación, refresco de grillas, etc)

        /// <summary>
        /// Muestra u oculta el ícono de alerta (rojo) en un control de la interfaz.
        /// </summary>
        /// <param name="control">El control visual (ej: TextBox, ComboBox) que se va a marcar.</param>
        /// <param name="mensaje">El mensaje que verá el usuario al pasar el mouse. Si se deja vacío, el ícono desaparece.</param>
        private void ErrorCampo(Control control, string mensaje = "") => errorProvider1.SetError(control, mensaje);

        /// <summary>
        /// Alterna el estado de los controles del formulario entre modo edición y modo solo lectura/bloqueado.
        /// </summary>
        /// <remarks>
        /// También ajusta dinámicamente la visibilidad de los botones de acción y cambia el contexto 
        /// del botón de escape ("Cancelar" durante edición, "Salir" durante bloqueo).
        /// </remarks>
        /// <param name="activar"><c>true</c> para habilitar los campos y botones de guardado; <c>false</c> para bloquearlos.</param>
        private void ActivarCampos(bool activar)
        {
            // 1. Controles de Datos Personales y Estado
            TxtDni.Enabled = activar;
            txtNombre.Enabled = activar;
            txtApellido.Enabled = activar;
            dateTimePicker1.Enabled = activar;
            CbEstados.Enabled = activar;

            // 2. Controles de Domicilio y Geografía
            txtCalle.Enabled = activar;
            txtNro.Enabled = activar;
            txtPiso.Enabled = activar;
            txtDepto.Enabled = activar;
            txtBarrio.Enabled = activar;

            if (!activar)
            {
                // Reemplazamos Text = "" por SelectedIndex = -1, que es la forma segura 
                // de limpiar visualmente ComboBoxes con DataBinding en WinForms
                CbProvincia.SelectedIndex = -1;
                CbLocalidad.SelectedIndex = -1;
            }

            CbProvincia.Enabled = activar;
            CbLocalidad.Enabled = activar;

            // 3. Controles de Interacción y Acciones (Botones/Links)
            LinkContactos.Visible = activar;
            BtnBuscar.Visible = activar;
            BtnGuardar.Visible = activar;

            // Adaptación semántica del botón secundario
            BtnCancelar.Text = activar ? "Cancelar" : "Salir";
        }

        /// <summary>
        /// Actualiza el origen de datos de la grilla de contactos para reflejar los cambios en memoria.
        /// </summary>
        private void RefrescarGrilla()
        {
            try
            {
                // Limpiamos los enlaces previos y reasignamos la lista actual
                bindingContactos.DataSource = null;
                bindingContactos.DataSource = _contactos;

                // Forzamos el redibujado visual de la grilla
                DataGridContactos.Refresh();
            }
            catch (Exception ex)
            {
                // Reemplazamos el MessageBox genérico por nuestro estándar de arquitectura
                Logger.LogError($"Error visual al refrescar la grilla de contactos: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un problema al intentar actualizar la lista de contactos en pantalla.");
            }
        }

        /// <summary>
        /// Configura los límites y el valor por defecto del selector de fecha de nacimiento.
        /// </summary>
        /// <remarks>
        /// Restringe la fecha máxima al día de ayer para ser coherente con la validación 
        /// de <see cref="ComprobarNacimiento"/>, evitando que el usuario seleccione el día actual.
        /// </remarks>
        private void FechaInicial()
        {
            // Usamos DateTime.Today en lugar de DateTime.Now para ignorar las horas/minutos
            DateTime hoy = DateTime.Today;

            // Limitamos el calendario para que no se puedan elegir fechas futuras
            dateTimePicker1.MaxDate = hoy.AddDays(1);
            dateTimePicker1.MinDate = hoy.AddYears(-149); // Fecha mínima: 149 años atrás.

            // Establecemos el valor por defecto
            dateTimePicker1.Value = hoy;
        }

        /// <summary>
        /// Restablece todos los controles visuales del formulario a su estado por defecto.
        /// </summary>
        private void LimpiarCampos()
        {
            // Limpiamos cualquier mensaje de error visible en la interfaz
            errorProvider1.Clear(); 

            // Refresca la grilla (si _contactos se limpió antes, la grilla quedará vacía visualmente)
            RefrescarGrilla();

            // Activamos la bandera de carga para evitar eventos no deseados al limpiar los campos
            cargando = true; 

            TxtDni.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtCalle.Text = string.Empty;
            txtNro.Text = string.Empty;
            txtPiso.Text = string.Empty;
            txtDepto.Text = string.Empty;
            txtBarrio.Text = string.Empty;

            dateTimePicker1.Value = DateTime.Today;

            CbProvincia.SelectedIndex = -1;
            CbLocalidad.SelectedIndex = -1;
            CbEstados.SelectedIndex = -1;

            // Desactivamos la bandera de carga al finalizar la limpieza de campos
            cargando = false; 
        }

        #endregion

        #region Métodos privados de lógica y orquestación

        /// <summary>
        /// Orquesta la validación, construcción y registro de un nuevo cliente en la base de datos.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de ejecución:</b>
        /// <list type="number">
        /// <item>Ejecuta <see cref="ComprobarCliente"/> para validar los datos vitales (DNI, Nombres, Nacimiento).</item>
        /// <item>Evalúa el domicilio. Si está incompleto o vacío, se anula para no guardar basura en la BD.</item>
        /// <item>Empaqueta todas las entidades en memoria mediante <see cref="ArmarObjetoCliente"/>.</item>
        /// <item>Envía el paquete a la capa de negocio y evalúa la respuesta.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) que contendrá el mensaje de éxito o el detalle del error.</param>
        /// <returns><c>true</c> si el registro fue exitoso; de lo contrario, <c>false</c>.</returns>
        private bool RegistrarCliente(out string mensaje)
        {
            // 1. Inicialización obligatoria del parámetro de salida
            mensaje = string.Empty;

            try
            {
                return true; // Placeholder para indicar que el proceso se completó sin errores. Reemplazar con la lógica real.
            }
            catch (Exception ex)
            {
                // Red de seguridad
                Logger.LogError($"Error crítico en RegistrarCliente: {ex.ToString()}");
                mensaje = "Ocurrió un error técnico inesperado al intentar guardar el cliente.";
                return false;
            }
        }

        /// <summary>
        /// Orquesta la validación, actualización en memoria y modificación de un cliente existente en la base de datos.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de ejecución:</b>
        /// <list type="number">
        /// <item>Ejecuta <see cref="ComprobarCliente"/> para validar los datos vitales en modo Modificación (<c>false</c>).</item>
        /// <item>Intenta reconstruir el domicilio con <see cref="ArmarObjetoDomicilio"/>. Si falla, lo anula.</item>
        /// <item>Actualiza el empaquetado de las entidades en memoria mediante <see cref="ArmarObjetoCliente"/>.</item>
        /// <item>Envía el paquete a la capa de negocio y evalúa la respuesta mediante el patrón Resultado.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) que contendrá el mensaje de éxito o el detalle del error.</param>
        /// <returns><c>true</c> si la modificación fue exitosa; de lo contrario, <c>false</c>.</returns>
        private bool ModificarCliente(out string mensaje)
        {
            // 1. Inicialización obligatoria del parámetro de salida
            mensaje = string.Empty;

            try
            {
                return true; // Placeholder para indicar que el proceso se completó sin errores. Reemplazar con la lógica real.
            }
            catch (Exception ex)
            {
                // Red de seguridad
                Logger.LogError($"Error crítico en ModificarCliente: {ex.ToString()}");
                mensaje = "Ocurrió un error técnico inesperado al intentar actualizar el cliente.";
                return false;
            }
        }

        /// <summary>
        /// Gestiona el cierre del formulario actual y notifica al menú principal para que restaure la vista de clientes.
        /// </summary>
        /// <param name="sender">El origen del evento (generalmente el botón que disparó la acción).</param>
        /// <param name="e">Argumentos del evento.</param>
        private void CerrarFormulario(object sender, EventArgs e)
        {
            try
            {
                // 1. Levantamos la bandera de cierre
                cerrando = true;

                // 2. Buscamos la instancia activa del menú principal
                var frmMenu = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();

                // 3. Notificamos al menú si existe
                if (frmMenu != null)
                {
                    // Le pedimos al menú que vuelva a cargar la grilla general de clientes
                    frmMenu.AbrirAbmClientes(sender, e, modo);
                }
                else
                {
                    // Opcional: Dejamos un rastro en el log por si esto pasa seguido y es síntoma de otro problema
                    Logger.LogError("No se encontró FrmMenuPrincipal al intentar cerrar el editor de clientes.");
                }

                // 4. Siempre debemos cerrar esta ventana, encuentre o no el menú
                this.Close();
            }
            catch (Exception ex)
            {
                // 5. Manejo de catástrofes estandarizado
                Logger.LogError($"Error fatal al intentar cerrar el formulario de clientes: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un problema inesperado al intentar cerrar la ventana.");

                // Forzamos el cierre para no dejar al usuario atrapado en una pantalla rota
                this.Close();
            }
        }

        #endregion

    }
}
