using Entidades_SGBM;
using Negocio_SGBM;
using Utilidades;

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
        private bool cargando = false;

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
            // La configuración de fechas la movemos a FechaInicial() para tener toda la lógica junta.
        }

        private void FrmEditClientes_Load(object sender, EventArgs e)
        {
            // Práctica mantener el Load con una sola línea que llame al orquestador.
            CargarFormulario();
        }

        /// <summary>
        /// Orquesta la inicialización de la interfaz, carga de catálogos y volcado de datos según el modo de apertura.
        /// </summary>
        private void CargarFormulario()
        {
            try
            {
                // 1. Apagamos los eventos visuales (ej: SelectedIndexChanged) mientras armamos la pantalla
                cargando = true;

                // 2. Configuración base
                string titulo = "Registro";
                FechaInicial();

                // 3. Carga de Catálogos (Combos)
                // ¡CRÍTICO!: Las listas deben cargarse antes que los datos del cliente, 
                // para que cuando el cliente intente seleccionarse a sí mismo, las opciones ya existan.
                CargarEstados();
                CargarProvincias();
                CargarLocalidades();

                // 4. Carga de Entidades (Solo si no es un registro nuevo)
                if (modo != EnumModoForm.Alta)
                {
                    // CargarDatosCliente ya se encarga de extraer la persona, el domicilio, los contactos
                    // y volcarlos visualmente en los controles.
                    CargarDatosCliente();

                    titulo = modo == EnumModoForm.Modificacion ? "Modificación" : "Detalles";
                }

                // 5. Configuración visual final
                ActivarCampos(modo != EnumModoForm.Consulta);
                labelTitulo.Text = $"{titulo} de Cliente";
            }
            catch (Exception ex)
            {
                // 6. Red de seguridad si algo falla al abrir la ventana
                Logger.LogError($"Error crítico al inicializar FrmEditClientes: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un problema técnico al intentar preparar la pantalla del cliente.");

                // Opcional: Si el formulario no se puede usar, lo cerramos
                // this.Close(); 
            }
            finally
            {
                // 7. Volvemos a encender los eventos visuales una vez que todo está cargado
                cargando = false;
            }
        }

        /// <summary>
        /// Orquesta la carga completa de un cliente existente, recuperando sus dependencias y volcándolas en la interfaz.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de ejecución:</b>
        /// <list type="number">
        /// <item>Desempaqueta las entidades base en la memoria usando <see cref="CargarCliente"/>.</item>
        /// <item>Consulta la capa de negocio para recuperar la lista de contactos asociados a la persona.</item>
        /// <item>Mapea todos los datos recuperados a los controles visuales mediante <see cref="CargarCamposClientes"/>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarDatosCliente()
        {
            // 1. Cláusula de guarda: Extrae Persona, Domicilio, etc. Si falla o es nulo, cortamos.
            if (!CargarCliente())
            {
                return;
            }

            try
            {
                // 2. Recuperación de Contactos (Asumiendo que la capa de negocio usa el Patrón Resultado)
                // Ya no declaramos variables en 'null' primero, hacemos una asignación directa y segura.
                var resultadoContactos = ContactosNegocio.GetContactosPorPersona(_persona);

                if (resultadoContactos.Success && resultadoContactos.Data != null)
                {
                    _contactos = resultadoContactos.Data;
                }
                else
                {
                    // Protegemos la variable contra nulos instanciando una lista vacía
                    _contactos = new List<Contactos>();

                    // Opcional: Si el fallo es de conexión y no simplemente que "no tiene contactos", lo registramos
                    if (!resultadoContactos.Success)
                    {
                        Logger.LogError($"Aviso al cargar cliente: {resultadoContactos.Mensaje}");
                    }
                }
                               
                // 3. Volcado visual de datos en el formulario
                CargarCamposClientes();

                // Como acabamos de actualizar la lista de contactos en memoria, es buena idea refrescar la grilla
                RefrescarGrilla();
            }
            catch (Exception ex)
            {
                // 4. Estandarización de errores (Adiós al MessageBox suelto)
                Logger.LogError($"Error crítico al cargar los datos completos del cliente en UI: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error técnico inesperado al intentar cargar la información del cliente.");
            }
        }

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
                txtDni.Text = _persona.Dni ?? string.Empty;
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
                    cbEstados.SelectedItem = _estado;
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
                    if (_provincia != null) cbProvincia.SelectedItem = _provincia;
                    if (_localidad != null) cbLocalidad.SelectedItem = _localidad;
                }
            }
            catch (Exception ex)
            {
                // Registro del error en caso de que alguna asignación visual falle
                Logger.LogError($"Error al cargar los datos en la UI (Clientes): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error menor al intentar mostrar todos los datos del cliente en pantalla.");
            } finally
            {
                cargando = false; // Desactivamos la bandera de carga al finalizar la asignación de valores
            }
        }

        //Comprobaciones
        /// <summary>
        /// Determina si existen contactos cargados en la lista de memoria.
        /// </summary>
        /// <returns><c>true</c> si la lista está instanciada y tiene al menos un contacto; de lo contrario, <c>false</c>.</returns>
        private bool ModoContactos()
        {
            // Simplificación moderna: si es null devuelve false, si no, evalúa el Count
            return _contactos?.Count > 0;
        }

        /// <summary>
        /// Muestra u oculta el ícono de alerta (rojo) en un control de la interfaz.
        /// </summary>
        /// <param name="control">El control visual (ej: TextBox, ComboBox) que se va a marcar.</param>
        /// <param name="mensaje">El mensaje que verá el usuario al pasar el mouse. Si se deja vacío, el ícono desaparece.</param>
        private void ErrorCampo(Control control, string mensaje = "") => errorProvider1.SetError(control, mensaje);

        /// <summary>
        /// Busca un cliente por su DNI en la base de datos y actualiza la entidad en memoria.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Delega la validación de formato a <see cref="ComprobarDni"/> y gestiona internamente
        /// las alertas visuales si la base de datos devuelve un error.
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si el cliente se encontró y asignó correctamente; de lo contrario, <c>false</c>.</returns>
        private bool BuscarCliente()
        {
            try
            {
                _cliente = null;

                // 1. Validación local (ComprobarDni ya marca el TextBox en rojo si falla)
                if (!ComprobarDni()) return false;

                // 2. Consulta a la capa de negocio refactorizada (Patrón Resultado)
                var resultado = ClientesNegocio.GetClientePorDni(_dni);

                // 3. Manejo de respuesta de la BD
                if (!resultado.Success)
                {
                    Mensajes.MensajeError(resultado.Mensaje ?? "Ocurrió un problema al buscar el cliente en la base de datos.");
                    return false;
                }

                // 4. Éxito: Asignación de datos
                _cliente = resultado.Data;

                // Retorna true solo si realmente trajo una instancia válida
                return _cliente != null;
            }
            catch (Exception ex)
            {
                // 5. Red de seguridad ante caídas inesperadas
                Logger.LogError($"Error crítico en BuscarCliente (por DNI): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado durante la búsqueda del cliente.");
                return false;
            }
        }

        /// <summary>
        /// Comprueba que el DNI del cliente ingresado no esté vacío y tenga una longitud válida.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Validaciones:</b>
        /// <list type="bullet">
        /// <item>Verifica que el campo no esté en blanco y sea numérico.</item>
        /// <item>Verifica que la longitud esté comprendida entre 6 y 9 caracteres.</item>
        /// </list>
        /// Las alertas visuales se manejan internamente mediante <c>ErrorCampo</c>.
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si el DNI es válido; de lo contrario, <c>false</c>.</returns>
        private bool ComprobarDni()
        {
            // 1. Validación de campo vacío y si es número, también establece el texto limpio en el campo
            if (!ValidarCampoNumerico(txtDni, true))
                return false;

            // 2. Asignación del valor limpio a la variable para su uso posterior
            _dni = txtDni.Text;

            // 3. Validación de longitud
            if (_dni.Length < 6 || _dni.Length > 9)
            {
                ErrorCampo(txtDni, "La longitud del DNI debe tener entre 6 y 9 dígitos.");
                return false;
            }

            // 4. Éxito: Limpieza del ícono de error en la UI
            ErrorCampo(txtDni);

            return true;
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
        /// Comprueba que los campos de nombres y apellidos del cliente no estén vacíos.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Evalúa ambos campos simultáneamente para poder mostrar múltiples alertas visuales si ambos están incompletos, 
        /// mejorando así la experiencia del usuario. Las alertas se gestionan internamente mediante <c>ErrorCampo</c>.
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si ambos campos contienen texto; de lo contrario, <c>false</c>.</returns>
        private bool ComprobarNombres()
        {
            // Limpiamos espacios en blanco al inicio o final
            _nombres = txtNombre.Text.Trim();
            _apellidos = txtApellido.Text.Trim();

            bool formularioValido = true;

            // 1. Validación del Nombre
            if (string.IsNullOrWhiteSpace(_nombres))
            {
                ErrorCampo(txtNombre, "Por favor, ingrese el o los nombres del cliente.");
                formularioValido = false;
            }
            else
            {
                ErrorCampo(txtNombre); // Limpia la alerta si está correcto
            }

            // 2. Validación del Apellido
            if (string.IsNullOrWhiteSpace(_apellidos))
            {
                ErrorCampo(txtApellido, "Por favor, ingrese el o los apellidos del cliente.");
                formularioValido = false;
            }
            else
            {
                ErrorCampo(txtApellido); // Limpia la alerta si está correcto
            }

            // 3. Capitalización de los nombres y apellidos para estandarizar el formato
            _nombres = Validaciones.CapitalizarTexto(_nombres);
            _apellidos = Validaciones.CapitalizarTexto(_apellidos);

            return formularioValido;
        }

        /// <summary>
        /// Comprueba que la fecha de nacimiento seleccionada sea válida (anterior al día de hoy).
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Comportamiento:</b>
        /// <list type="bullet">
        /// <item>Si la fecha es válida, se almacena en la variable <c>_nacimiento</c> y se limpian las alertas.</item>
        /// <item>Si la fecha es inválida (hoy o una fecha futura), se asigna <c>null</c> y se muestra una alerta visual en el control.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void ComprobarNacimiento()
        {
            try
            {
                // Usamos .Date para asegurarnos de ignorar cualquier componente de hora que pueda tener el control
                DateTime fechaSeleccionada = dateTimePicker1.Value.Date;

                // Validamos que la fecha sea estrictamente menor a la fecha actual
                if (fechaSeleccionada < DateTime.Today)
                {
                    _nacimiento = fechaSeleccionada;

                    // Éxito: Limpiamos la alerta visual por si antes se había equivocado
                    ErrorCampo(dateTimePicker1);
                }
                else
                {
                    _nacimiento = null;

                    // Feedback visual: Le indicamos al usuario por qué no se guardará la fecha
                    ErrorCampo(dateTimePicker1, "La fecha de nacimiento debe ser anterior al día de hoy.");
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores técnicos (ej: el control devuelve un formato corrupto)
                Logger.LogError($"Error crítico en ComprobarNacimiento (Clientes): {ex.ToString()}");
                Mensajes.MensajeAdvertencia("Ocurrió un error inesperado al validar la fecha de nacimiento.");
            }
        }

        /// <summary>
        /// Verifica si los datos ingresados conforman un domicilio válido y completo.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Lógica de evaluación:</b>
        /// <list type="number">
        /// <item>Verifica si el usuario ingresó al menos calle o barrio (<see cref="DireccionIngresada"/>).</item>
        /// <item>Si hay intención de guardar dirección, exige que la localidad (y por ende la provincia) sean válidas.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) que contendrá el error si los datos geográficos están incompletos.</param>
        /// <returns>
        /// <c>true</c> si el domicilio está completo y listo para guardarse; 
        /// <c>false</c> si el usuario no ingresó dirección, o si la ingresó de forma incompleta.
        /// </returns>
        private bool DomicilioCorrecto(out string mensaje)
        {
            // Inicialización obligatoria del parámetro de salida
            mensaje = string.Empty;

            // 1. ¿El usuario escribió algo en Calle o Barrio?
            if (!DireccionIngresada())
            {
                // Devuelve false pero con mensaje vacío, indicando que simplemente "no hay domicilio" (no es un error)
                return false;
            }

            // 2. Si escribió calle o barrio, la Localidad pasa a ser obligatoria
            // Usamos el 'out mensaje' que implementamos en el paso anterior
            if (!LocalidadIngresada(out mensaje))
            {
                // Devuelve false y llena la variable 'mensaje' con el error (ej: "Falta seleccionar provincia")
                return false;
            }

            // Si pasó ambas validaciones, el domicilio está perfecto
            return true;
        }

        /// <summary>
        /// Extrae y normaliza los datos de la dirección, verificando si se ingresó información suficiente.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Comportamiento:</b>
        /// <list type="bullet">
        /// <item>Limpia los valores ingresados eliminando espacios en blanco accidentales (<c>Trim</c>).</item>
        /// <item>Si un campo está vacío o solo contiene espacios, se asigna como <c>null</c> para mantener limpia la base de datos.</item>
        /// <item>Considera que hay una intención real de guardar un domicilio si al menos existe la Calle o el Barrio.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si se ingresó calle o barrio; de lo contrario, <c>false</c>.</returns>
        private bool DireccionIngresada()
        {
            // Extraemos y limpiamos los valores, asignando null de forma segura si están vacíos
            _calle = string.IsNullOrWhiteSpace(txtCalle.Text) ? null : txtCalle.Text.Trim();
            _barrio = string.IsNullOrWhiteSpace(txtBarrio.Text) ? null : txtBarrio.Text.Trim();
            _altura = string.IsNullOrWhiteSpace(txtNro.Text) ? null : txtNro.Text.Trim();
            _piso = string.IsNullOrWhiteSpace(txtPiso.Text) ? null : txtPiso.Text.Trim();
            _depto = string.IsNullOrWhiteSpace(txtDepto.Text) ? null : txtDepto.Text.Trim();

            // Como ya normalizamos los vacíos a null arriba, la validación final es súper limpia
            return _calle != null || _barrio != null;
        }

        /// <summary>
        /// Verifica si se ha ingresado una localidad válida, la busca en memoria o la crea si es nueva.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso:</b>
        /// <list type="number">
        /// <item>Verifica que exista una provincia válida seleccionada previamente.</item>
        /// <item>Busca la localidad tipeada en la lista actual (ignorando mayúsculas/minúsculas).</item>
        /// <item>Si no existe, crea una nueva localidad y capitaliza el texto.</item>
        /// <item><b>Relación sin Autoincremental:</b> Si la provincia tiene ID, lo asigna. Si la provincia es nueva (ID null), vincula el objeto completo para que la capa de datos/ORM gestione la inserción en cascada.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) con el motivo del fallo si el método devuelve <c>false</c>.</param>
        /// <returns><c>true</c> si se procesó la localidad exitosamente; de lo contrario, <c>false</c>.</returns>
        private bool LocalidadIngresada(out string mensaje)
        {
            // 1. Inicialización de salida
            mensaje = string.Empty;
            _localidad = null;

            // 2. Validación de dependencias
            if (!ProvinciaIngresada())
            {
                mensaje = "Para ingresar un domicilio, primero escriba o seleccione una provincia válida.";
                return false;
            }

            // 3. Validación de entrada de Localidad
            string localidadInput = cbLocalidad.Text.Trim();
            if (string.IsNullOrWhiteSpace(localidadInput))
            {
                mensaje = "Para ingresar un domicilio, escriba o seleccione una localidad.";
                return false;
            }

            // Capitalizamos el texto para mantener la uniformidad en la base de datos
            string nombreLocalidad = Validaciones.CapitalizarTexto(localidadInput);

            try
            {
                // 4. Búsqueda en memoria
                if (_localidades != null && _localidades.Count > 0)
                {
                    // Usamos StringComparison para evitar duplicados como "Gualeguaychú" y "gualeguaychú"
                    _localidad = _localidades.FirstOrDefault(l =>
                        l.Localidad.Equals(nombreLocalidad, StringComparison.OrdinalIgnoreCase));
                }

                // 5. Creación si no existe (usando el operador ??=)
                _localidad ??= new Localidades { Localidad = nombreLocalidad };

                // 6. Vinculación Relacional (Adaptado para IDs no autoincrementales)
                if (_provincia?.IdProvincia > 0)
                {
                    // Si la provincia ya existe en la BD, enlazamos por ID
                    _localidad.IdProvincia = _provincia.IdProvincia;
                }
                else
                {
                    // ¡BUG CORREGIDO!: Si la provincia es nueva, solo vinculamos el objeto.
                    // Ya no hacemos un "new()" de _localidad aquí para no perder la que ya encontramos o creamos.
                    _localidad.Provincias = _provincia;
                }

                return true; // Si llegamos aquí, _localidad siempre tendrá un objeto válido
            }
            catch (Exception ex)
            {
                // 7. Manejo de errores
                Logger.LogError($"Error crítico en LocalidadIngresada (Clientes): {ex.ToString()}");
                mensaje = "Ocurrió un error inesperado al procesar la localidad.";
                return false;
            }
        }

        /// <summary>
        /// Verifica si se ha ingresado o seleccionado una provincia válida en el ComboBox.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Lógica de extracción:</b>
        /// <list type="number">
        /// <item>Verifica que el campo no esté vacío.</item>
        /// <item>Si la memoria está vacía (<c>_provincias</c> nulo), asume que es una provincia nueva y la instancia.</item>
        /// <item>Intenta recuperar la provincia seleccionada del <see cref="BindingSource"/>.</item>
        /// <item>Si el usuario escribió un texto manualmente, lo busca en la lista o crea una nueva provincia capitalizada.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si se obtuvo una entidad provincia válida; de lo contrario, <c>false</c>.</returns>
        private bool ProvinciaIngresada()
        {
            _provincia = null;

            // Limpieza de espacios por seguridad
            string nombreProvincia = cbProvincia.Text.Trim();

            // 1. Cláusula de guarda: Si está vacío, no hay nada que procesar
            if (string.IsNullOrWhiteSpace(nombreProvincia))
            {
                return false;
            }

            // 1.5 Normalización: Capitalizamos el texto para mejorar la consistencia en la creación de nuevas provincias
            nombreProvincia = Validaciones.CapitalizarTexto(nombreProvincia);

            // 2. Fallback: Si no hay lista de provincias cargada en memoria, creamos una nueva
            if (_provincias == null || _provincias.Count == 0)
            {
                // Opcional: Reutilizamos tu capitalizador si lo deseas
                _provincia = new Provincias { Provincia = nombreProvincia };
                return true;
            }

            try
            {
                // 3. Extracción desde el BindingSource
                _provincia = bindingProvincias.Current as Provincias;

                // 4. Mejora de UX (Cruce de datos): 
                // Si el Current es nulo o el usuario tipeó un nombre distinto al que está seleccionado
                if (_provincia == null || !_provincia.Provincia.Equals(nombreProvincia, StringComparison.OrdinalIgnoreCase))
                {
                    // Lo buscamos en la lista por nombre exacto
                    _provincia = _provincias.FirstOrDefault(p =>
                        p.Provincia.Equals(nombreProvincia, StringComparison.OrdinalIgnoreCase));

                    // Si definitivamente no existe en la base de datos, lo creamos como nuevo
                    _provincia ??= new Provincias { Provincia = nombreProvincia };
                }
            }
            catch (Exception ex)
            {
                // Reemplazamos Console.WriteLine por el Logger oficial del sistema
                Logger.LogError($"Error crítico en ProvinciaIngresada (Clientes): {ex.ToString()}");
                return false;
            }

            return _provincia != null;
        }

        /// <summary>
        /// Comprueba la validez integral de los datos del cliente antes de continuar con el guardado.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso de Validación:</b>
        /// <list type="number">
        /// <item><b>Duplicidad:</b> Verifica mediante <see cref="BuscarCliente"/> si el DNI ya existe. Lo rechaza si es un alta nueva.</item>
        /// <item><b>Nombres:</b> Delega la validación de formato a <see cref="ComprobarNombres"/>.</item>
        /// <item><b>Nacimiento:</b> Ejecuta <see cref="ComprobarNacimiento"/> para asegurar una fecha coherente.</item>
        /// <item><b>Estado:</b> Verifica que se haya podido recuperar un estado válido del ComboBox, especialmente vital en modificaciones.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="esNuevoRegistro">Indica si la operación es un alta (<c>true</c>) o una modificación (<c>false</c>).</param>
        /// <returns><c>true</c> si el cliente superó todas las validaciones; de lo contrario, <c>false</c>.</returns>
        private bool ComprobarCliente(bool esNuevoRegistro)
        {
            try
            {
                // 1. Verificación de Duplicidad de DNI en la BD
                // BuscarCliente ya no usa 'ref' y marca el TextBox si hay problemas de conexión.
                if (BuscarCliente())
                {
                    if (esNuevoRegistro)
                    {
                        // Es un Alta y el DNI ya pertenece a alguien más
                        ErrorCampo(txtDni, "El DNI ingresado ya pertenece a otro cliente registrado.");
                        return false;
                    }
                }
                else
                {
                    // Limpieza visual si el DNI está libre
                    ErrorCampo(txtDni);
                }

                // 2. Validación de Nombres y Apellidos
                // Las alertas visuales se encienden solas dentro de este método
                if (!ComprobarNombres())
                {
                    return false;
                }

                // 3. Validación de Nacimiento
                ComprobarNacimiento();

                // 4. Validación del Estado (Extracción segura con 'as')
                _estado = cbEstados.SelectedItem as Estados;

                // Corrección: Como IdEstado es 'int' (no nullable), validamos si el objeto es nulo o su ID es 0.
                if ((_estado == null || _estado.IdEstado == 0) && modo == EnumModoForm.Modificacion)
                {
                    Mensajes.MensajeError("Problemas con la recuperación de estados de clientes desde la base de datos.");
                    return false;
                }

                // Si pasó todos los filtros, está listo
                return true;
            }
            catch (Exception ex)
            {
                // 5. Red de seguridad ante excepciones
                Logger.LogError($"Error crítico en ComprobarCliente: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error técnico inesperado al validar los datos del cliente.");
                return false;
            }
        }

        //Métodos
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
            txtDni.Enabled = activar;
            txtNombre.Enabled = activar;
            txtApellido.Enabled = activar;
            dateTimePicker1.Enabled = activar;
            cbEstados.Enabled = activar;

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
                cbProvincia.SelectedIndex = -1;
                cbLocalidad.SelectedIndex = -1;
            }

            cbProvincia.Enabled = activar;
            cbLocalidad.Enabled = activar;

            // 3. Controles de Interacción y Acciones (Botones/Links)
            linkContactos.Visible = activar;
            btnBuscar.Visible = activar;
            btnGuardar.Visible = activar;

            // Adaptación semántica del botón secundario
            btnCancelar.Text = activar ? "Cancelar" : "Salir";
        }

        /// <summary>
        /// Actualiza el origen de datos de la grilla de contactos para reflejar los cambios en memoria.
        /// </summary>
        private void RefrescarGrilla()
        {
            try
            {
                // Limpiamos los enlaces previos y reasignamos la lista actual
                bindingContactos.Clear();
                bindingContactos.DataSource = _contactos;

                // Forzamos el redibujado visual de la grilla
                dataGridContactos.Refresh();
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
            DateTime ayer = DateTime.Today.AddDays(-1);

            // Limitamos el calendario para que no se puedan elegir fechas futuras ni el día de hoy
            dateTimePicker1.MaxDate = ayer;

            // Establecemos el valor por defecto
            dateTimePicker1.Value = ayer;
        }

        /// <summary>
        /// Carga la lista completa de provincias desde la base de datos y la enlaza al ComboBox.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Mejoras aplicadas:</b>
        /// <list type="bullet">
        /// <item>Uso de operador de coalescencia nula (<c>??</c>) para instanciar la lista de forma directa y limpia.</item>
        /// <item>Protección contra desbordamientos al intentar seleccionar el índice 0 en una lista vacía.</item>
        /// <item>Reemplazo de la salida de consola por el sistema centralizado de logging.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarProvincias()
        {
            try
            {
                // 1. Obtención de datos y protección contra nulos
                var resultado = DomiciliosNegocio.GetProvincias();
                if (resultado.Success && resultado.Data != null)
                {
                    _provincias = resultado.Data ?? new List<Provincias>();
                }
                
                // 2. Enlace de datos a la interfaz
                bindingProvincias.DataSource = _provincias;

                // 3. Selección por defecto (¡Protegida contra listas vacías!)
                if (_provincias.Count > 0)
                {
                    cbProvincia.SelectedIndex = 0;
                }

                // 4. Lógica de Modificación: Si el cliente ya tenía una localidad, seleccionamos su provincia
                if (_localidad != null)
                {
                    // Como confirmamos que IdProvincia es int, la comparación es totalmente segura
                    _provincia = _provincias.FirstOrDefault(p => p.IdProvincia == _localidad.IdProvincia);

                    if (_provincia != null)
                    {
                        cbProvincia.SelectedItem = _provincia;
                    }
                }

                //5. Si el resultado de la consulta no fue exitoso, lo registramos para su análisis
                if (!resultado.Success)
                {
                    Logger.LogError($"Error al cargar provincias: {resultado.Mensaje}");
                    Mensajes.MensajeAdvertencia("Ocurrió un problema al cargar la lista de provincias.");
                }
            }
            catch (Exception ex)
            {
                // 5. Red de seguridad centralizada
                Logger.LogError($"Error crítico al intentar cargar las provincias: {ex.ToString()}");

                // Le podemos avisar al usuario si esto es crítico para el flujo
                Mensajes.MensajeAdvertencia("Ocurrió un problema al cargar la lista de provincias.");
            }
        }

        /// <summary>
        /// Carga la lista de localidades correspondientes a la provincia seleccionada actualmente en la interfaz.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso:</b>
        /// <list type="number">
        /// <item>Limpia el origen de datos de las localidades.</item>
        /// <item>Busca la provincia actual en la memoria, ignorando diferencias de mayúsculas/minúsculas.</item>
        /// <item>Si la encuentra, solicita sus localidades a la capa de negocio y las enlaza al ComboBox.</item>
        /// <item>Intenta re-seleccionar la localidad previamente guardada del cliente, si existe.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarLocalidades()
        {
            try
            {
                // 1. Limpieza inicial segura para proteger el DataBinding
                bindingLocalidades.Clear();
                _localidades = new List<Localidades>();

                string nombreProvincia = cbProvincia.Text.Trim();

                // 2. Cláusula de guarda: Si no hay texto o no hay provincias en memoria, dejamos el combo vacío
                if (string.IsNullOrWhiteSpace(nombreProvincia) || _provincias == null)
                {
                    bindingLocalidades.DataSource = _localidades;
                    return;
                }

                // 3. Búsqueda de la provincia en memoria (robusta ante mayúsculas/minúsculas)
                _provincia = _provincias.FirstOrDefault(p =>
                    p.Provincia.Equals(nombreProvincia, StringComparison.OrdinalIgnoreCase));

                // 4. Consulta a la capa de negocio
                if (_provincia != null)
                {
                    // Asumiendo que DomiciliosNegocio usa el patrón Resultado:
                    var resultado = DomiciliosNegocio.GetLocalidadesPorProvincia(_provincia);

                    if (resultado.Success && resultado.Data != null)
                    {
                        _localidades = resultado.Data;
                    }
                }

                // 5. Enlace de datos a la interfaz
                bindingLocalidades.DataSource = _localidades;

                // 6. Selección visual de la localidad guardada (si existe)
                if (_localidad != null)
                {
                    cbLocalidad.SelectedItem = _localidad;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error crítico al intentar cargar las localidades: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Carga la lista de estados disponibles para los clientes en el ComboBox y selecciona el correspondiente.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso:</b>
        /// <list type="number">
        /// <item>Limpia la memoria y el BindingSource.</item>
        /// <item>Consulta los estados a la capa de negocio. Si falla, deshabilita el control.</item>
        /// <item>Si es un Alta, busca el estado "Activo". Si no existe en la BD, lo crea y lo inyecta en la lista.</item>
        /// <item>Si es Modificación, busca el estado que ya tiene asignado el cliente.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarEstados()
        {
            try
            {
                // 1. Limpieza inicial segura
                _estado = null;
                _estados = new List<Estados>();
                bindingEstados.Clear();

                // 2. Consulta a la capa de negocio (Asumiendo patrón Resultado o que devuelve la lista directamente)
                var resultado = EstadosNegocio.GetEstadosPorIndole("Clientes");

                // Evaluamos si falló o si la lista vino vacía
                if (!resultado.Success || resultado.Data == null || !resultado.Data.Any())
                {
                    cbEstados.Enabled = false;
                    Logger.LogError($"Error al cargar estados de clientes: {resultado.Mensaje}");
                    return;
                }

                // 3. Asignación del DataSource principal
                _estados = resultado.Data;
                bindingEstados.DataSource = _estados;
                cbEstados.Enabled = true;

                // 4. Lógica de selección según el Modo del formulario
                if (modo == EnumModoForm.Alta || _cliente == null)
                {
                    // Buscamos "Activo" ignorando mayúsculas/minúsculas por seguridad
                    _estado = _estados.FirstOrDefault(e => e.Estado.Equals("Activo", StringComparison.OrdinalIgnoreCase));

                    if (_estado == null)
                    {
                        // Si la BD no tenía el estado "Activo", lo creamos en memoria
                        _estado = new Estados { Indole = "Clientes", Estado = "Activo" };

                        // ¡BUG CORREGIDO!: Agregamos el nuevo estado a la lista en lugar de pisar el DataSource
                        _estados.Add(_estado);
                        bindingEstados.ResetBindings(false); // Le avisa al ComboBox que la lista creció
                    }
                }
                else
                {
                    // Es modificación: buscamos el estado exacto del cliente
                    // Como IdEstado es int (no nullable), la comparación es directa
                    _estado = _estados.FirstOrDefault(e => e.IdEstado == _cliente.IdEstado);
                }

                // 5. Selección visual en el ComboBox
                if (_estado != null)
                {
                    cbEstados.SelectedItem = _estado;
                }
            }
            catch (Exception ex)
            {
                // 6. Manejo de excepciones (Reemplaza al Console.WriteLine)
                Logger.LogError($"Error crítico al enlazar los estados en la UI: {ex.ToString()}");
                cbEstados.Enabled = false; // Deshabilitamos por seguridad si algo explota
            }
        }

        /// <summary>
        /// Recupera los contactos asociados a la persona actual desde la base de datos y actualiza la grilla.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso:</b>
        /// <list type="number">
        /// <item>Limpia la grilla instanciando una lista vacía (evitando nulos que rompan el DataBinding).</item>
        /// <item>Verifica que exista una persona válida en memoria.</item>
        /// <item>Consulta a la capa de negocio y maneja los errores mediante el Logger en lugar de la consola.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private void CargarContactos()
        {
            try
            {
                // 1. Limpieza segura: Instanciamos una lista vacía en lugar de null y refrescamos
                _contactos = new List<Contactos>();
                RefrescarGrilla();

                // 2. Cláusula de guarda combinada: Si no hay cliente o persona, cortamos aquí
                if (_cliente == null || _persona == null)
                {
                    return;
                }

                // 3. Consulta a la capa de negocio refactorizada (Patrón Resultado)
                var resultado = ContactosNegocio.GetContactosPorPersona(_persona);

                if (!resultado.Success)
                {
                    // Reemplazamos el viejo Console.WriteLine por el Logger oficial
                    Logger.LogError($"Error en capa de negocio al cargar contactos: {resultado.Mensaje}");

                    // Opcional: Le avisamos al usuario que algo falló sin cerrar el formulario
                    Mensajes.MensajeAdvertencia("No se pudieron cargar los contactos del cliente.");
                    return;
                }

                // 4. Asignación de datos reales (Protegemos con ?? por si la BD devuelve null en vez de lista vacía)
                _contactos = resultado.Data ?? new List<Contactos>();

                // 5. Reflejo de los datos en la UI
                RefrescarGrilla();
            }
            catch (Exception ex)
            {
                // 6. Red de seguridad
                Logger.LogError($"Error crítico en CargarContactos (UI): {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado al intentar recuperar los contactos.");
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

        /// <summary>
        /// Valida los datos geográficos y, si son correctos, construye o actualiza la entidad Domicilio en memoria.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso:</b>
        /// <list type="number">
        /// <item>Delega la validación a <see cref="DomicilioCorrecto"/>.</item>
        /// <item>Instancia un nuevo domicilio si es Alta o si no existía previamente.</item>
        /// <item>Vuelca las variables locales (<c>_calle</c>, <c>_barrio</c>, etc.) en la entidad.</item>
        /// <item><b>Relación sin Autoincremental:</b> Vincula la localidad por ID si ya existe, o por objeto si es nueva.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="mensaje">Parámetro de salida (<c>out</c>) que contendrá el error si la validación falla.</param>
        /// <returns><c>true</c> si el domicilio se armó correctamente; de lo contrario, <c>false</c>.</returns>
        private bool ArmarObjetoDomicilio(out string mensaje)
        {
            // 1. Validación previa delegada
            // Si DomicilioCorrecto devuelve false, pasamos el mensaje hacia arriba y cortamos.
            if (!DomicilioCorrecto(out mensaje))
            {
                return false;
            }

            // 2. Instanciación o reutilización
            if (modo == EnumModoForm.Alta || _domicilio == null)
            {
                _domicilio = new Domicilios();
            }

            // 3. Asignación de datos primitivos (las variables ya están limpias gracias a DireccionIngresada)
            _domicilio.Calle = _calle;
            _domicilio.Altura = _altura;
            _domicilio.Piso = _piso;
            _domicilio.Barrio = _barrio;
            _domicilio.Depto = _depto;

            // 4. Vinculación Relacional
            if (_localidad != null)
            {
                // Recordando que los IDs son 'int' y no autoincrementales: 
                // Si el ID no es 0, significa que la localidad ya está persistida en la BD.
                // Nota: Si en tu clase IdLocalidad es int? (nullable), cambia esto por '!= null'
                if (_localidad.IdLocalidad != 0)
                {
                    _domicilio.IdLocalidad = _localidad.IdLocalidad;
                }
                else
                {
                    // Si la localidad es nueva, pasamos el objeto completo para que el motor de BD lo gestione
                    _domicilio.Localidades = _localidad;
                }
            }

            return true;
        }

        /// <summary>
        /// Construye y empaqueta la entidad Cliente con todos los datos y relaciones recolectados en memoria.
        /// </summary>
        /// <remarks>
        /// Se encarga de instanciar nuevas entidades si es un Alta, y de vincular correctamente
        /// las referencias cruzadas (Cliente -> Persona -> Domicilio / Cliente -> Estado).
        /// </remarks>
        private void ArmarObjetoCliente()
        {
            // 1. Inicialización según el modo del formulario
            if (modo == EnumModoForm.Alta)
            {
                // Si es alta, forzamos entidades nuevas para limpiar cualquier residuo
                _cliente = new();
                _persona = new();
            }
            else
            {
                // Si es modificación, nos aseguramos de que no sean nulas (por seguridad)
                _cliente ??= new();
                _persona ??= new();
            }

            // 2. Asignación de datos primitivos a la Persona
            _persona.Dni = _dni;
            _persona.Nombres = _nombres;
            _persona.Apellidos = _apellidos;
            _persona.FechaNac = _nacimiento;

            // 3. Vinculación de relaciones
            _persona.Domicilios = _domicilio;
            _cliente.Personas = _persona;

            // 4. Asignación segura del Estado (Protección contra NullReferenceException)
            // Como confirmamos que los IDs ahora son 'int' no anulables, asignamos directamente
            if (_estado != null)
            {
                _cliente.Estados = _estado;
                _cliente.IdEstado = _estado.IdEstado;
            }
        }

        /// <summary>
        /// Limpia las variables de estado en memoria para preparar un nuevo registro o evitar cruces de datos.
        /// </summary>
        /// <param name="limpiarContactos">Indica si también se debe vaciar la lista de contactos en memoria.</param>
        private void LimpiarValores(bool limpiarContactos)
        {
            _cliente = null;
            _persona = null;
            _estado = null;
            _domicilio = null;

            if (limpiarContactos)
            {
                // Instancia una lista vacía en lugar de null para no romper el binding de la grilla
                _contactos = new();
            }
        }

        /// <summary>
        /// Restablece todos los controles visuales del formulario a su estado por defecto.
        /// </summary>
        private void LimpiarCampos()
        {
            // Refresca la grilla (si _contactos se limpió antes, la grilla quedará vacía visualmente)
            RefrescarGrilla();

            txtDni.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtCalle.Text = string.Empty;
            txtNro.Text = string.Empty;
            txtPiso.Text = string.Empty;
            txtDepto.Text = string.Empty;
            txtBarrio.Text = string.Empty;

            dateTimePicker1.Value = DateTime.Today;

            cbProvincia.SelectedIndex = -1;
            cbLocalidad.SelectedIndex = -1;
            cbEstados.SelectedIndex = -1;
        }

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
                // 2. Comprobación integral del cliente (Alta = true)
                // Ya no necesita el 'ref mensaje' porque los controles se marcan en rojo solos
                if (!ComprobarCliente(true))
                {
                    mensaje = "Por favor, revise los campos marcados en rojo antes de continuar.";
                    return false;
                }

                // 3. Verificación del Domicilio
                if (!ArmarObjetoDomicilio(out string avisoDomicilio))
                {
                    // Si falla, anulamos el objeto para no vincular un domicilio roto
                    _domicilio = null;

                    if (!string.IsNullOrWhiteSpace(avisoDomicilio))
                    {
                        Mensajes.MensajeAdvertencia(avisoDomicilio);
                    }
                }

                // 4. Empaquetado final de las entidades
                ArmarObjetoCliente();

                // 5. Persistencia en la capa de negocio (Patrón Resultado)
                var resultado = ClientesNegocio.RegistrarCliente(_cliente, _contactos);

                if (!resultado.Success)
                {
                    mensaje = $"Error en el registro:\n{resultado.Mensaje}";
                    return false;
                }

                // 6. Éxito
                mensaje = $"Registro exitoso.\n{resultado.Mensaje}";
                return true;
            }
            catch (Exception ex)
            {
                // 7. Red de seguridad
                Logger.LogError($"Error crítico en RegistrarCliente: {ex.ToString()}");
                mensaje = "Ocurrió un error técnico inesperado al intentar guardar el cliente.";
                return false;
            }
        }

        /// <summary>
        /// Desempaqueta el grafo de entidades del cliente actual en las variables locales del formulario.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso:</b>
        /// <list type="bullet">
        /// <item>Valida que el cliente y su persona asociada existan en memoria.</item>
        /// <item>Extrae en cascada el Domicilio, Localidad y Provincia usando operadores de navegación segura (<c>?.</c>).</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns><c>true</c> si el cliente y su persona son válidos; de lo contrario, <c>false</c>.</returns>
        private bool CargarCliente()
        {
            // 1. Cláusula de guarda combinada: Si el cliente es nulo o no tiene una persona asociada, cortamos.
            if (_cliente?.Personas == null)
            {
                return false;
            }

            // 2. Extracción de datos principales
            _persona = _cliente.Personas;

            // Si _cliente.Estados es null, _estado simplemente se vuelve null, 
            // lo cual es correcto para limpiar selecciones previas.
            _estado = _cliente.Estados;

            // 3. Extracción en cascada de la geografía
            _domicilio = _persona.Domicilios;

            // Con el operador '?.', si _domicilio es null, _localidad recibe null automáticamente 
            _localidad = _domicilio?.Localidades;
            _provincia = _localidad?.Provincias;

            return true;
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
                // 2. Comprobación integral del cliente (Alta = false)
                if (!ComprobarCliente(false))
                {
                    mensaje = "Por favor, revise los campos marcados en rojo antes de continuar.";
                    return false;
                }

                // 3. Verificación y Armado del Domicilio (con el nuevo nombre)
                if (!ArmarObjetoDomicilio(out string avisoDomicilio))
                {
                    // Si devuelve false, anulamos el objeto para no vincular un domicilio roto o vacío
                    _domicilio = null;

                    // Opcional: Le avisamos al usuario si hubo un problema geográfico, pero sin frenar la modificación
                    if (!string.IsNullOrWhiteSpace(avisoDomicilio))
                    {
                        Mensajes.MensajeAdvertencia(avisoDomicilio);
                    }
                }

                // 4. Empaquetado final de las entidades actualizadas
                ArmarObjetoCliente();

                // 5. Persistencia en la capa de negocio (Patrón Resultado)
                var resultado = ClientesNegocio.ModificarCliente(_cliente, _contactos);

                if (!resultado.Success)
                {
                    mensaje = $"Error en la modificación:\n{resultado.Mensaje}";
                    return false;
                }

                // 6. Éxito
                mensaje = $"Modificación exitosa.\n{resultado.Mensaje}";
                return true;
            }
            catch (Exception ex)
            {
                // 7. Red de seguridad
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

                // 4. ¡Corrección de Bug!: Siempre debemos cerrar esta ventana, encuentre o no el menú
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

        /// <summary>
        /// Orquesta el proceso de guardado (Alta o Modificación) validando la existencia del cliente y gestionando el flujo de pantallas.
        /// </summary>
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Limpieza de memoria (manteniendo los contactos actuales) para no mezclar datos de intentos previos
                LimpiarValores(false);

                // 2. Verificación de estado actual en la BD
                // (BuscarCliente ya se encarga de alertar si hay error de conexión a la BD)
                bool existe = BuscarCliente();

                // =================================================================
                // FLUJO A: MODO ALTA
                // =================================================================
                if (modo == EnumModoForm.Alta)
                {
                    if (existe)
                    {
                        // Prevención de duplicados: El cliente ya existe, ofrecemos editarlo
                        string nombreEncontrado = _cliente?.NombreCompleto ?? "Cliente Encontrado";
                        DialogResult res = MessageBox.Show(
                            $"¡Atención! El DNI ingresado ya pertenece a:\n{nombreEncontrado}\n\n¿Desea modificar al cliente existente?",
                            "DNI Duplicado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (res == DialogResult.Yes)
                        {
                            DialogResult confirmacion = MessageBox.Show(
                                "¿Está seguro que desea descartar el registro nuevo y pasar a modo edición?",
                                "Confirmación",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (confirmacion == DialogResult.Yes)
                            {
                                modo = EnumModoForm.Modificacion;
                                LimpiarCampos();
                                CargarFormulario();
                            }
                        }
                        return; // Cortamos la ejecución aquí, no intentamos registrar
                    }

                    // Si NO existe, el camino está libre para registrar
                    if (!RegistrarCliente(out string mensajeRegistro))
                    {
                        Mensajes.MensajeError($"No se pudo completar el registro:\n{mensajeRegistro}");
                        return;
                    }

                    // Éxito en el Alta
                    Mensajes.MensajeExito(mensajeRegistro); // El mensaje ya viene formateado desde RegistrarCliente

                    // Opcional: Podrías llamar a LimpiarCampos() aquí para dejar el form listo para otro registro
                    return;
                }

                // =================================================================
                // FLUJO B: MODO MODIFICACIÓN
                // =================================================================
                if (!existe)
                {
                    // Anomalía: Quería modificar, pero el cliente ya no existe (ej: fue borrado por otro usuario o le cambió el DNI)
                    DialogResult res = MessageBox.Show(
                        "El cliente que intenta modificar ya no existe en la base de datos o el DNI fue alterado.\n¿Desea registrarlo como un cliente nuevo?",
                        "Cliente no encontrado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                    if (res == DialogResult.Yes)
                    {
                        modo = EnumModoForm.Alta;
                        CargarFormulario();
                    }
                    else
                    {
                        btnGuardar.Enabled = false;
                    }
                    return; // Cortamos la ejecución aquí
                }

                // Si existe, el camino está libre para actualizar sus datos
                if (!ModificarCliente(out string mensajeModificacion))
                {
                    Mensajes.MensajeError($"No se pudo completar la modificación:\n{mensajeModificacion}");
                    return;
                }

                // Éxito en la Modificación
                Mensajes.MensajeExito(mensajeModificacion);

                // Cerramos la ventana y volvemos al menú principal
                CerrarFormulario(sender, e);
            }
            catch (Exception ex)
            {
                // Red de seguridad máxima: Si algo explota (ej: se cae el servidor a mitad de guardado), la app no se cierra
                Logger.LogError($"Error crítico en el botón Guardar de Clientes: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error técnico inesperado al intentar guardar los datos del cliente.");
            }
        }

        /// <summary>
        /// Busca un cliente por DNI y gestiona el flujo de la interfaz dependiendo del modo actual (Alta o Modificación).
        /// </summary>
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            // 1. Ejecutamos la búsqueda (BuscarCliente ya valida el DNI y maneja sus propios errores de conexión)
            bool existe = BuscarCliente();

            // =================================================================
            // ESCENARIO A: EL CLIENTE NO EXISTE (DNI Libre)
            // =================================================================
            if (!existe)
            {
                if (modo == EnumModoForm.Alta)
                {
                    Mensajes.MensajeExito("El DNI ingresado está disponible para un nuevo registro.");
                    btnGuardar.Enabled = true;
                }
                else
                {
                    // Estaba buscando para modificar, pero no existe. Le ofrecemos cambiar a Alta.
                    DialogResult res = MessageBox.Show(
                        "El DNI ingresado no corresponde a ningún cliente registrado.\n¿Desea registrar un cliente nuevo?",
                        "No encontrado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (res == DialogResult.Yes)
                    {
                        modo = EnumModoForm.Alta;
                        CargarFormulario();
                    }
                    else
                    {
                        btnGuardar.Enabled = false;
                    }
                }

                return; // Terminamos la ejecución aquí
            }

            // =================================================================
            // ESCENARIO B: EL CLIENTE SÍ EXISTE (DNI Ocupado)
            // =================================================================
            string nombreEncontrado = _cliente.NombreCompleto ?? "Cliente Encontrado";

            if (modo == EnumModoForm.Modificacion || modo == EnumModoForm.Consulta)
            {
                // Buscaba un cliente para editar y lo encontró (Corrección: habilitamos btnGuardar, no btnBuscar)
                Mensajes.MensajeExito($"Cliente encontrado:\n{nombreEncontrado}");
                btnGuardar.Enabled = true;

                // Cargamos los datos del cliente recién encontrado en la pantalla
                CargarFormulario();
            }
            else // Es modo Alta
            {
                // Quería crear uno nuevo, pero el DNI ya está en la base de datos.
                DialogResult res = MessageBox.Show(
                    $"¡Atención! El DNI ingresado ya pertenece a:\n{nombreEncontrado}\n\n¿Desea pasar al modo Modificación para editar a este cliente?",
                    "DNI Duplicado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (res == DialogResult.Yes)
                {
                    // Doble confirmación de seguridad
                    DialogResult confirmacion = MessageBox.Show(
                        "¿Está seguro que desea descartar el alta nueva y editar al cliente existente?",
                        "Confirmación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirmacion == DialogResult.Yes)
                    {
                        modo = EnumModoForm.Modificacion;
                        LimpiarCampos();
                        CargarFormulario(); // Carga al cliente encontrado en pantalla
                    }
                }
            }
        }

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

            // Actualiza el combo de localidades en cascada
            CargarLocalidades();
        }

        /// <summary>
        /// Restringe la entrada del TextBox exclusivamente a caracteres numéricos y teclas de control.
        /// </summary>
        /// <param name="sender">El TextBox del DNI.</param>
        /// <param name="e">Argumentos del evento de teclado.</param>
        private void TxtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si la tecla presionada NO es un número y NO es una tecla de control (como borrar)...
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // ... marcamos el evento como "manejado", lo que anula la pulsación y no la escribe.
                e.Handled = true;
            }
        }

    }
}
