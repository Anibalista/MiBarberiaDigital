using Entidades_SGBM;
using Front_SGBM.UXDesign;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Front_SGBM
{
    public partial class FrmContactos : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;
        public List<Contactos>? _contactos = null;
        public List<Contactos>? _contactosNuevos = null;
        public string origen = string.Empty;

        // Expresión regular compilada una sola vez para mejorar el rendimiento
        private static readonly Regex _emailRegex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$", RegexOptions.Compiled);

        private int contadorReplicas = 1;

        //Valores de Carga
        private string areaWhat = string.Empty;
        private string area = string.Empty;
        private string whatsapp = string.Empty;
        private string fijo = string.Empty;
        private string email = string.Empty;
        private string instagram = string.Empty;
        private string facebook = string.Empty;
        private bool extranjeroWhat = false;

        public FrmContactos()
        {
            InitializeComponent();
        }

        //Métodos
        /// <summary>
        /// Evento principal de carga del formulario que inicializa el estado según la acción solicitada.
        /// </summary>
        /// <remarks>
        /// <para>
        /// El comportamiento inicial depende del valor de la variable <c>modo</c>:
        /// <list type="bullet">
        /// <item><b>Modificación:</b> Delega la tarea a <see cref="CargarContactos"/> para traer los datos existentes.</item>
        /// <item><b>Alta:</b> Inicializa una nueva lista en memoria y le agrega un objeto <c>Contactos</c> en blanco, listo para ser enlazado a la interfaz.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El formulario que originó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void FrmContactos_Load(object sender, EventArgs e)
        {
            try
            {
                // Evaluamos el propósito del formulario
                if (modo == EnumModoForm.Modificacion)
                {
                    // Traemos los datos de la base de datos
                    CargarContactos(sender, e);
                }
                else if (modo == EnumModoForm.Alta)
                {
                    // Preparamos el entorno en memoria para un nuevo registro
                    // Sintaxis moderna: instanciamos la lista y agregamos el objeto en una sola operación
                    _contactos = new List<Contactos>
                    {
                        new Contactos()
                    };
                }
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error al inicializar el formulario de contactos.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Vuelca los datos de la lista de contactos en los controles visuales del formulario.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Lógica de Carga (Principal vs Dinámicos):</b>
        /// <list type="number">
        /// <item><b>Contacto Principal:</b> El primer registro (<c>_contactos[0]</c>) se asigna a los TextBoxes fijos por defecto del formulario.</item>
        /// <item><b>Contactos Adicionales:</b> A partir del segundo registro, se extraen los valores y se simula un clic en el botón "Más" (<see cref="BtnMas_Click"/>) para generar los controles dinámicos necesarios en la interfaz.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El objeto que invoca el método (típicamente el formulario durante el Load).</param>
        /// <param name="e">Argumentos del evento.</param>
        private void CargarContactos(object sender, EventArgs e)
        {
            try
            {
                // Cláusula de guarda unificada: Salimos inmediatamente si no hay nada que cargar
                if (_contactos == null || _contactos.Count == 0) return;

                // 1. Cargar el Primer Contacto (Principal)
                // Extrae los datos a las variables de nivel de clase
                ExtraerValoresContacto(_contactos[0]);

                // Asignación a los controles fijos de la UI
                txtAreaWhat.Text = areaWhat;
                txtNumWhat.Text = whatsapp;
                txtAreaFijo.Text = area;
                txtNumFijo.Text = fijo;
                txtInsta.Text = instagram;
                txtFace.Text = facebook;
                txtEmail.Text = email;
                checkExtranjeroWhat.Checked = extranjeroWhat;

                // Limpieza de las variables temporales para no arrastrar basura a la siguiente iteración
                VaciarValores();

                // Si solo había un contacto, el trabajo está terminado
                if (_contactos.Count == 1) return;

                // 2. Cargar Contactos Adicionales (Controles Dinámicos)
                for (int i = 1; i < _contactos.Count; i++)
                {
                    // Preparamos las variables con los datos del contacto actual
                    ExtraerValoresContacto(_contactos[i]);

                    // Reutilizamos la lógica del botón "Más" para crear los controles visuales 
                    // e inyectarles los valores que acabamos de extraer.
                    BtnMas_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error al intentar cargar los contactos.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Extrae, parsea y asigna los datos de la entidad contacto a las variables temporales del formulario.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Lógica de Procesamiento:</b>
        /// <list type="bullet">
        /// <item><b>Redes Sociales y Email:</b> Asignación directa usando el operador coalescente nulo (<c>?? string.Empty</c>).</item>
        /// <item><b>Teléfonos Fijos:</b> Divide la cadena esperando el formato "Area-Numero". Si el formato es inválido, deja las variables vacías.</item>
        /// <item><b>WhatsApp:</b> Aplica lógica específica para Argentina. Si el área comienza con "54" o "549", remueve el prefijo de país y marca <c>extranjeroWhat</c> como falso.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="contacto">El objeto con los datos crudos provenientes de la base de datos.</param>
        private void ExtraerValoresContacto(Contactos contacto)
        {
            try
            {
                // 1. Extracción Directa (Protección contra nulos)
                instagram = contacto.Instagram ?? string.Empty;
                facebook = contacto.Facebook ?? string.Empty;
                email = contacto.Email ?? string.Empty;

                // 2. Procesamiento de Teléfono Fijo
                // Inicializamos vacío por defecto para ahorrarnos los bloques 'else'
                area = string.Empty;
                fijo = string.Empty;

                if (!string.IsNullOrWhiteSpace(contacto.Telefono))
                {
                    string[] partesFijo = contacto.Telefono.Split('-');
                    if (partesFijo.Length == 2)
                    {
                        area = partesFijo[0];
                        fijo = partesFijo[1];
                    }
                }

                // 3. Procesamiento de WhatsApp
                areaWhat = string.Empty;
                whatsapp = string.Empty;
                extranjeroWhat = contacto.ExtranjeroWhatsapp;

                if (!string.IsNullOrWhiteSpace(contacto.Whatsapp))
                {
                    string[] partesWhat = contacto.Whatsapp.Split('-');
                    if (partesWhat.Length == 2)
                    {
                        extranjeroWhat = true; // Asumimos que es extranjero por defecto si tiene guion
                        areaWhat = partesWhat[0];
                        whatsapp = partesWhat[1];

                        // Lógica de limpieza para números nacionales (Argentina)
                        if (areaWhat.StartsWith("54"))
                        {
                            // Substring es más limpio para "saltar" los primeros N caracteres
                            areaWhat = areaWhat.StartsWith("549") ? areaWhat.Substring(3) : areaWhat.Substring(2);
                            extranjeroWhat = false; // Confirmado que es local
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error al extraer y formatear los datos del contacto.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Restablece las variables temporales de estado de los contactos a sus valores por defecto.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método actúa como un limpiador entre iteraciones o antes de procesar nuevos datos.
        /// Es fundamental para evitar que queden en memoria valores "basura" (efectos secundarios) 
        /// del contacto procesado anteriormente durante las iteraciones de carga o guardado.
        /// </para>
        /// </remarks>
        private void VaciarValores()
        {
            // Datos de WhatsApp
            areaWhat = string.Empty;
            whatsapp = string.Empty;
            extranjeroWhat = false;

            // Datos de Teléfono Fijo
            area = string.Empty;
            fijo = string.Empty;

            // Datos Digitales / Redes Sociales
            email = string.Empty;
            instagram = string.Empty;
            facebook = string.Empty;
        }

        //Botones
        /// <summary>
        /// Gestiona la cancelación de la edición, confirmando con el usuario y liberando el estado del formulario padre.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de Cancelación:</b>
        /// <list type="number">
        /// <item>Solicita confirmación al usuario para evitar la pérdida accidental de datos en progreso.</item>
        /// <item>Identifica el formulario contenedor usando la variable de estado <c>origen</c>.</item>
        /// <item>Restablece la bandera <c>editandoContactos</c> en el padre para habilitar futuras ediciones.</item>
        /// <item>Cierra la ventana actual.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón de cancelar.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Confirmación de usuario
                DialogResult result = Mensajes.Respuesta(
                    "¿Desea cancelar la edición de contactos?\nNo se guardará ninguna modificación.",
                    "Confirmar Cancelación");

                if (result == DialogResult.No) return;

                // 2. Liberación de recursos en el formulario "Padre"
                // Según quién nos haya invocado, buscamos su instancia abierta y reseteamos su estado.
                if (origen == "Clientes")
                {
                    var frmClientes = Application.OpenForms.OfType<FrmEditClientes>().FirstOrDefault();
                    if (frmClientes != null)
                    {
                        frmClientes.editandoContactos = false;
                    }
                }
                else if (origen == "Barberos")
                {
                    var frmBarbero = Application.OpenForms.OfType<FrmEditBarbero>().FirstOrDefault();
                    if (frmBarbero != null)
                    {
                        frmBarbero.editandoContactos = false;
                    }
                }

                // 3. Cierre exitoso
                this.Close();
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error al intentar cerrar el formulario.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Recopila los datos de todos los bloques de contacto, aplica reglas de formato y los envía al formulario padre.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Proceso de Guardado:</b>
        /// <list type="number">
        /// <item>Itera sobre los <see cref="GroupBox"/> dentro del contenedor dinámico.</item>
        /// <item>Extrae el valor de los controles (TextBox, CheckBox) identificándolos por su nombre y el contador de réplica.</item>
        /// <item>Aplica reglas de negocio: Si es un número local sin área, asigna por defecto el área '3446'. Si es WhatsApp local, añade el prefijo internacional '549'.</item>
        /// <item>Descarta los bloques de contacto que estén completamente vacíos.</item>
        /// <item>Envía la lista consolidada de contactos al formulario padre (Clientes o Barberos).</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón de guardar.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Confirmación del usuario
            DialogResult res = MessageBox.Show("¿Desea Guardar los Cambios?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No) return;

            try
            {
                // 2. Inicialización de colecciones
                if (_contactos == null) _contactos = new List<Contactos>();
                _contactosNuevos = new List<Contactos>();

                int indexLista = 0; // Índice real para acceder a _contactos
                string sufijoControl = ""; // Para el primer grupo es vacío, luego será "1", "2", etc.

                // 3. Extracción de datos iterando la UI
                foreach (Control contenedor in flowLayoutPanel1.Controls)
                {
                    if (contenedor is GroupBox groupBox && groupBox.Name != "groupBoxBotones")
                    {
                        VaciarValores(); // Limpiamos variables temporales de la iteración anterior
                        Contactos contactoTemp = new Contactos();
                        bool grupoVacio = true; // Bandera para descartar contactos en blanco

                        // Iteramos los controles dentro del GroupBox actual
                        foreach (Control control in groupBox.Controls)
                        {
                            switch (control)
                            {
                                case CheckBox chk when chk.Name == $"checkExtranjeroWhat{sufijoControl}":
                                    extranjeroWhat = chk.Checked;
                                    break;

                                case TextBox txt:
                                    string nombreTxt = txt.Name;
                                    string valorTxt = txt.Text.Trim();

                                    // Asignamos el valor extraído a la variable temporal correspondiente
                                    if (nombreTxt == $"txtAreaWhat{sufijoControl}") areaWhat = valorTxt;
                                    else if (nombreTxt == $"txtNumWhat{sufijoControl}") whatsapp = valorTxt;
                                    else if (nombreTxt == $"txtAreaFijo{sufijoControl}") area = valorTxt;
                                    else if (nombreTxt == $"txtNumFijo{sufijoControl}") fijo = valorTxt;
                                    else if (nombreTxt == $"txtInsta{sufijoControl}")
                                    {
                                        instagram = valorTxt;
                                        if (!string.IsNullOrEmpty(instagram)) { grupoVacio = false; contactoTemp.Instagram = instagram; }
                                    }
                                    else if (nombreTxt == $"txtFace{sufijoControl}")
                                    {
                                        facebook = valorTxt;
                                        if (!string.IsNullOrEmpty(facebook)) { grupoVacio = false; contactoTemp.Facebook = facebook; }
                                    }
                                    else if (nombreTxt == $"txtEmail{sufijoControl}")
                                    {
                                        email = valorTxt;
                                        if (!string.IsNullOrEmpty(email)) { grupoVacio = false; contactoTemp.Email = email; }
                                    }
                                    break;
                            }
                        }

                        // 4. Aplicación de Reglas de Negocio a Teléfonos

                        // WhatsApp
                        if (!string.IsNullOrWhiteSpace(whatsapp))
                        {
                            grupoVacio = false;

                            if (!extranjeroWhat) // Es número local (Argentina)
                            {
                                if (string.IsNullOrWhiteSpace(areaWhat)) areaWhat = "3446"; // Área por defecto (Gualeguaychú)
                                areaWhat = "549" + areaWhat; // Prefijo internacional de celular
                            }
                            contactoTemp.Whatsapp = $"{areaWhat}-{whatsapp}";
                        }

                        // Teléfono Fijo
                        if (!string.IsNullOrWhiteSpace(fijo))
                        {
                            grupoVacio = false;
                            if (string.IsNullOrWhiteSpace(area)) area = "3446"; // Área por defecto
                            contactoTemp.Telefono = $"{area}-{fijo}";
                        }

                        // 5. Consolidación en la Lista de Salida
                        if (!grupoVacio)
                        {
                            // Si estábamos modificando un contacto existente, conservamos su ID original
                            if (indexLista < _contactos.Count)
                            {
                                contactoTemp.IdContacto = _contactos[indexLista].IdContacto;
                            }
                            _contactosNuevos.Add(contactoTemp);
                        }

                        // Preparación de índices para la próxima iteración
                        indexLista++;
                        sufijoControl = indexLista.ToString();
                    }
                }

                // 6. Inyección de dependencias (Envío de datos al formulario padre)
                bool transferenciaExitosa = false;

                if (origen == "Clientes")
                {
                    var frmClientes = Application.OpenForms.OfType<FrmEditClientes>().FirstOrDefault();
                    if (frmClientes != null)
                    {
                        transferenciaExitosa = frmClientes.TraerContactos(_contactosNuevos);
                    }
                }
                else if (origen == "Barberos")
                {
                    var frmBarberos = Application.OpenForms.OfType<FrmEditBarbero>().FirstOrDefault();
                    if (frmBarberos != null)
                    {
                        transferenciaExitosa = frmBarberos.TraerContactos(_contactosNuevos);
                    }
                }

                // 7. Evaluación final
                if (!transferenciaExitosa)
                {
                    Mensajes.MensajeError("Hubo un problema al intentar enviar los contactos al formulario principal. Verifique que el formulario de origen siga abierto.");
                    return;
                }

                // Éxito: Cierra la ventana de contactos
                this.Close();
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error crítico durante el proceso de guardado.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Elimina el último bloque de contacto generado dinámicamente y actualiza el modelo de datos.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Flujo de Eliminación:</b>
        /// <list type="number">
        /// <item><b>Validación:</b> Verifica que haya al menos un contacto adicional para eliminar (no permite borrar el principal).</item>
        /// <item><b>Limpieza Visual:</b> Busca el último <see cref="GroupBox"/> clonado, lo remueve del contenedor y libera su memoria (<see cref="Control.Dispose"/>).</item>
        /// <item><b>Ajuste de UI:</b> Reduce la altura del formulario para mantener el diseño compacto.</item>
        /// <item><b>Sincronización de Datos:</b> Remueve el último registro de la lista <c>_contactos</c> en memoria usando <see cref="List{T}.RemoveAt"/> para máxima eficiencia.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón que disparó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnMenos_Click(object sender, EventArgs e)
        {
            // Cláusula de guarda: Si el contador es menor a 2, significa que solo está el contacto base original. No se borra.
            if (contadorReplicas < 2) return;

            try
            {
                // 1. Localizar el control visual correspondiente al último contador
                string nombreGrupo = $"groupBox{contadorReplicas}";
                var groupBoxToRemove = flowLayoutPanel1.Controls.Find(nombreGrupo, false).FirstOrDefault();

                if (groupBoxToRemove != null)
                {
                    // 2. Remover de la UI y DESTRUIR para liberar memoria no administrada (Crucial en WinForms)
                    flowLayoutPanel1.Controls.Remove(groupBoxToRemove);
                    groupBoxToRemove.Dispose();

                    // 3. Ajustar la altura del formulario contenedor
                    this.Height -= (groupBox1.Height + 10);
                }

                // 4. Sincronizar la lista de datos en memoria
                if (_contactos != null && _contactos.Count >= contadorReplicas)
                {
                    // RemoveAt borra el elemento directamente por índice. 
                    // Es mucho más eficiente en CPU y Memoria que re-instanciar la lista con LINQ (.Take).
                    _contactos.RemoveAt(_contactos.Count - 1);
                }

                // 5. Actualizar la variable de estado
                contadorReplicas--;
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error al intentar eliminar el formulario de contacto.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Genera dinámicamente un nuevo bloque de controles (GroupBox) para agregar un contacto adicional.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Funcionamiento:</b>
        /// <list type="number">
        /// <item>Verifica que no se haya excedido el límite de réplicas permitidas (máximo 5 contactos adicionales).</item>
        /// <item>Ajusta el tamaño del formulario contenedor.</item>
        /// <item>Itera sobre los controles del <c>groupBox1</c> (plantilla) y crea copias idénticas en memoria.</item>
        /// <item>Asigna a cada control clonado los valores temporales pre-cargados (si existen) y conecta sus eventos (<c>KeyPress</c>, <c>Validating</c>).</item>
        /// <item>Añade el nuevo GroupBox al <see cref="FlowLayoutPanel"/> y actualiza la lista en memoria <c>_contactos</c>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El botón que disparó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BtnMas_Click(object sender, EventArgs e)
        {
            // Límite de contactos adicionales permitidos
            if (contadorReplicas >= 5) return;

            try
            {
                // 1. Ajuste de UI y Creación del Contenedor Principal
                this.Height += groupBox1.Height + 10;

                GroupBox nuevoGroupBox = new GroupBox
                {
                    Text = $"Contacto Nro {contadorReplicas + 1}",
                    Name = $"groupBox{contadorReplicas + 1}",
                    Size = groupBox1.Size,
                    BackColor = groupBox1.BackColor
                    // Nota: No se asigna 'Location' porque flowLayoutPanel1 lo acomodará automáticamente.
                };

                // 2. Clonación de Controles (Pattern Matching)
                foreach (Control control in groupBox1.Controls)
                {
                    Control nuevoControl = null;

                    switch (control)
                    {
                        case CheckBox chkOriginal:
                            nuevoControl = new CheckBox
                            {
                                Checked = extranjeroWhat,
                                TabIndex = contadorReplicas * 7 + 4
                            };
                            break;

                        case TextBox txtOriginal:
                            TextBox nuevoTxt = new TextBox
                            {
                                Text = string.Empty // Valor por defecto
                            };

                            // Asignación específica de valores, eventos y TabIndex según el nombre del control original
                            switch (txtOriginal.Name)
                            {
                                case "txtAreaWhat":
                                    nuevoTxt.KeyPress += Numeric_KeyPress;
                                    nuevoTxt.Text = areaWhat;
                                    nuevoTxt.TabIndex = contadorReplicas * 7 + 1;
                                    break;
                                case "txtNumWhat":
                                    nuevoTxt.KeyPress += Numeric_KeyPress;
                                    nuevoTxt.Text = whatsapp;
                                    nuevoTxt.TabIndex = contadorReplicas * 7 + 2;
                                    break;
                                case "txtAreaFijo":
                                    nuevoTxt.KeyPress += Numeric_KeyPress;
                                    nuevoTxt.Text = area;
                                    nuevoTxt.TabIndex = contadorReplicas * 7 + 4;
                                    break;
                                case "txtNumFijo":
                                    nuevoTxt.KeyPress += Numeric_KeyPress;
                                    nuevoTxt.Text = fijo;
                                    nuevoTxt.TabIndex = contadorReplicas * 7 + 5;
                                    break;
                                case "txtInsta":
                                    nuevoTxt.Text = instagram;
                                    nuevoTxt.TabIndex = contadorReplicas * 7 + 6;
                                    break;
                                case "txtFace":
                                    nuevoTxt.Text = facebook;
                                    nuevoTxt.TabIndex = contadorReplicas * 7 + 7;
                                    break;
                                case "txtEmail":
                                    nuevoTxt.Validating += TxtEmail_Validating;
                                    nuevoTxt.Text = email;
                                    nuevoTxt.TabIndex = contadorReplicas * 7 + 8;
                                    break;
                            }
                            nuevoControl = nuevoTxt;
                            break;

                        case Label lblOriginal:
                            nuevoControl = new Label
                            {
                                Text = lblOriginal.Text
                            };
                            break;
                    }

                    // 3. Propiedades comunes a todos los controles clonados
                    if (nuevoControl != null)
                    {
                        nuevoControl.Location = control.Location;
                        nuevoControl.Size = control.Size;
                        nuevoControl.Font = control.Font;
                        nuevoControl.Name = control.Name + contadorReplicas.ToString();

                        // Añadimos el control al GroupBox clonado
                        nuevoGroupBox.Controls.Add(nuevoControl);
                    }
                }

                // 4. Renderizado en UI
                flowLayoutPanel1.Controls.Add(nuevoGroupBox);

                // 5. Sincronización con el modelo de datos en memoria
                if (_contactos == null)
                {
                    _contactos = new List<Contactos>();
                }

                if (_contactos.Count == contadorReplicas)
                {
                    _contactos.Add(new Contactos());
                }

                // 6. Actualización de estado y estilo
                contadorReplicas++;
                VaciarValores();
                EstiloAplicacion.AplicarEstilo(this); // Asumiendo que ahora usas AplicarTema como vimos en FrmAbmServicios
            }
            catch (Exception ex)
            {
                Mensajes.MensajeError($"Ocurrió un error al generar los campos para el nuevo contacto.\nDetalle Técnico:\n{ex.ToString()}");
            }
        }

        //Validadores de Campos
        /// <summary>
        /// Restringe la entrada de caracteres en el control, permitiendo únicamente dígitos y teclas de control.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Se utiliza en el evento <c>KeyPress</c> de cajas de texto donde solo se admiten números enteros 
        /// (ej: Códigos Postales, Cantidades, IDs).
        /// </para>
        /// <para>
        /// <b>Comportamiento:</b>
        /// <list type="bullet">
        /// <item>Permite números del 0 al 9 (<see cref="char.IsDigit"/>).</item>
        /// <item>Permite teclas de control como Backspace o Enter (<see cref="char.IsControl"/>).</item>
        /// <item>Bloquea letras, espacios, símbolos y signos de puntuación (asigna <c>e.Handled = true</c>).</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El control que recibe la entrada de teclado.</param>
        /// <param name="e">Argumentos del evento que contienen la tecla presionada.</param>
        private void Numeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Evaluamos directamente: Se "maneja" (cancela) el evento si la tecla NO es un dígito Y NO es un control.
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        /// <summary>
        /// Valida el formato del correo electrónico de forma pasiva, utilizando alertas visuales sin bloquear la navegación.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Estrategia de Validación (Soft Validation):</b>
        /// <list type="bullet">
        /// <item>Si el campo contiene texto, se evalúa contra una expresión regular compilada.</item>
        /// <item>Si el formato es inválido, se activa el <see cref="ErrorProvider"/> mediante <see cref="SetError"/>.</item>
        /// <item><b>No se utiliza <c>e.Cancel = true</c></b> para evitar "atrapar" al usuario en el control, 
        /// permitiéndole cancelar la operación o cerrar la ventana libremente.</item>
        /// <item>Si el campo se vacía o se corrige, la alerta visual se limpia automáticamente.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="sender">El <see cref="TextBox"/> del correo electrónico.</param>
        /// <param name="e">Los argumentos del evento de validación.</param>
        private void TxtEmail_Validating(object sender, CancelEventArgs e)
        {
            // Casteo seguro usando pattern matching
            if (sender is TextBox txtEmail)
            {
                string email = txtEmail.Text.Trim();

                // 1. Caso: Campo vacío
                // Si el usuario lo borra, quitamos el error de formato. 
                // (La validación de si es un campo obligatorio se hará al presionar "Guardar").
                if (string.IsNullOrWhiteSpace(email))
                {
                    SetError(txtEmail, string.Empty);
                    return;
                }

                // 2. Caso: Contiene texto, evaluamos el formato
                if (!_emailRegex.IsMatch(email))
                {
                    // Encendemos el icono rojo de error con el mensaje de ayuda, pero lo dejamos salir del control
                    SetError(txtEmail, "El E-mail ingresado no tiene un formato válido (ej: usuario@dominio.com).");
                }
                else
                {
                    // 3. Caso: Formato correcto, limpiamos cualquier error previo
                    SetError(txtEmail, string.Empty);
                }
            }
        }

        /// <summary>
        /// Establece o limpia un mensaje de error visual asociado a un control de la interfaz.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Este método encapsula el uso del <see cref="ErrorProvider"/> del formulario para centralizar las alertas.
        /// Si el parámetro <paramref name="mensaje"/> está vacío (<c>string.Empty</c>) o es nulo, el icono de error se ocultará automáticamente del control.
        /// </para>
        /// </remarks>
        /// <param name="control">El control (ej: TextBox, ComboBox) sobre el cual se mostrará la alerta.</param>
        /// <param name="mensaje">El texto descriptivo del error, o una cadena vacía para limpiarlo.</param>
        private void SetError(Control control, string mensaje) => errorProvider1.SetError(control, mensaje);
    }
}
