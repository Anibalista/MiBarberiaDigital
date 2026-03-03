using Entidades_SGBM;
using Negocio_SGBM;
using Utilidades;

namespace Front_SGBM
{
    public partial class FrmAbmClientes : Form
    {
        public EnumModoForm modo = EnumModoForm.Consulta;
        private List<Personas>? _personas = null;
        private List<Clientes>? _clientes = null;
        private List<Contactos>? _contactos = null;
        public Clientes? _cliente = null;
        private Personas? _persona = null;
        private List<Localidades>? _localidades = null;
        public List<string> opcionesBuscar = new List<string> { "Dni, Nombres", "Domicilio", "WhatsApp, Teléfono" };
        private bool cerrando = false;
        private bool cargando = false;

        //Campos
        string _campo1 = string.Empty;
        string _campo2 = string.Empty;
        Localidades? _localidadBuscada = null;
        private bool incluirAnulados = false;

        /// <summary>
        /// Inicializa el formulario ABM de Clientes.
        /// </summary>
        public FrmAbmClientes()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento de carga del formulario: inicializa controles y dispara búsqueda inicial.
        /// </summary>
        private void FrmAbmClientes_Load(object sender, EventArgs e)
        {
            CargarFormulario();
            btnBuscar_Click(sender, e);
            btnSeleccionar.Text = modo == EnumModoForm.Venta ? "Seleccionar" : "Modificar Cliente";
        }

        /// <summary>
        /// Carga las opciones de búsqueda en el combo correspondiente.
        /// </summary>
        private void CargarFormulario()
        {
            cbBusqueda.Items.Clear();
            cbBusqueda.DataSource = opcionesBuscar;
            cbBusqueda.SelectedIndex = 0;
        }

        /// <summary>
        /// Carga las localidades disponibles en el combo de localidades.
        /// </summary>
        private void BuscarLocalidades()
        {
            if (cerrando) return;

            var localidadCero = new Localidades { Localidad = "Seleccionar" };
            var localidades = new List<Localidades> { localidadCero };

            var resultado = DomiciliosNegocio.GetLocalidades();
            if (resultado.Success && resultado.Data != null)
                localidades.AddRange(resultado.Data);

            bindingLocalidades.Clear();
            bindingLocalidades.DataSource = localidades;

            try
            {
                cbLocalidad.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al seleccionar localidad: {ex.Message}");
            }
        }

        /// <summary>
        /// Cambia los nombres de los labels de búsqueda y visibilidad de localidad.
        /// </summary>
        private void CambiarNombresCampos(string lbl1, string lbl2, bool localidad)
        {
            labelCampo1.Text = lbl1;
            labelCampo2.Text = lbl2;
            cbLocalidad.Visible = localidad;
            labelLocalidad.Visible = localidad;
        }

        /// <summary>
        /// Actualiza la grilla de clientes con las personas extraídas de la lista de clientes.
        /// </summary>
        private void ActualizarGrillaClientes()
        {
            if (cerrando) return;

            bindingClientes.DataSource = null;
            ExtraerPersonas();

            var personas = _personas ?? new List<Personas>();
            bindingClientes.DataSource = personas;
            dataGridClientes.Refresh();

            if (personas.Count == 0) return;

            for (int i = 0; i < personas.Count; i++)
            {
                var persona = personas[i];
                if (i < dataGridClientes.Rows.Count)
                {
                    var row = dataGridClientes.Rows[i];
                    row.Cells["Localidad"].Value = persona.Localidad ?? "";
                    row.Cells["Domicilio"].Value = persona.Direccion ?? "";
                }
            }
            dataGridClientes.ClearSelection();
        }

        /// <summary>
        /// Actualiza la grilla de contactos asociados a la persona seleccionada.
        /// </summary>
        private void ActualizarGrillaContactos()
        {
            if (cerrando) return;

            contactosBindingSource.DataSource = null;
            _contactos = null;

            if (_persona != null)
            {
                var resultado = ContactosNegocio.GetContactosPorPersona(_persona);
                _contactos = resultado.Success ? resultado.Data : null;
            }

            var contactos = _contactos ?? new List<Contactos>();
            contactosBindingSource.DataSource = contactos;
            dataGridContactos.Refresh();
        }

        /// <summary>
        /// Limpia los valores de búsqueda y entidades cargadas.
        /// </summary>
        private void LimpiarValores()
        {
            _campo1 = string.Empty;
            _campo2 = string.Empty;
            _localidadBuscada = null;
            _clientes = null;
            _personas = null;
            _persona = null;
        }

        /// <summary>
        /// Carga los valores de búsqueda desde los controles del formulario.
        /// </summary>
        private void CargarValores()
        {
            LimpiarValores();
            _campo1 = txtCampo1.Text;
            _campo2 = txtCampo2.Text;

            if (cbBusqueda.Text == "Domicilio")
            {
                try
                {
                    _localidadBuscada = (Localidades)bindingLocalidades.Current;
                }
                catch
                {
                    _localidadBuscada = null;
                }
            }

            if (_localidadBuscada != null && _localidadBuscada.IdLocalidad == null)
                _localidadBuscada = null;
        }

        /// <summary>
        /// Ejecuta la búsqueda de clientes según los criterios seleccionados.
        /// </summary>
        private bool BuscarClientes()
        {
            CargarValores();
            var opcion = cbBusqueda.Text;
            incluirAnulados = checkAnulados.Checked;

            var resultado = ClientesNegocio.GetListadoDeClientes(opcion, _campo1, _campo2, _localidadBuscada, incluirAnulados);
            if (!resultado.Success || resultado.Data == null)
            {
                MessageBox.Show(resultado.Mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _clientes = resultado.Data;
            return true;
        }

        /// <summary>
        /// Extrae las personas asociadas a los clientes cargados.
        /// </summary>
        private void ExtraerPersonas()
        {
            _personas = null;
            if (_clientes == null) return;

            _personas = _clientes
                .Where(c => c.Personas != null)
                .Select(c => c.Personas)
                .OrderBy(p => p.Apellidos)
                .ToList();
        }

        //Botones
        /// <summary>
        /// Evento de botón Salir: marca el formulario como cerrando y lo cierra.
        /// </summary>
        private void btnSalir_Click(object sender, EventArgs e)
        {
            cerrando = true;
            this.Close();
        }

        /// <summary>
        /// Evento de botón Buscar: ejecuta la búsqueda de clientes y actualiza la grilla.
        /// </summary>
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (cerrando) return;

            var resultado = BuscarClientes();
            if (!resultado)
                MessageBox.Show("No se encontraron clientes o ocurrió un error en la búsqueda.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            ActualizarGrillaClientes();
        }

        /// <summary>
        /// Evento de botón Seleccionar: abre el formulario de edición de clientes desde el menú principal.
        /// </summary>
        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            FrmMenuPrincipal padre = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (padre == null) return;

            try
            {
                if (modo != EnumModoForm.Venta)
                {
                    padre.AbrirEditClientes(sender, e, EnumModoForm.Modificacion, _cliente);
                }
                // TODO: Enlazar con el flujo de ventas si el modo es Venta.
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sobre el menú principal: " + ex.Message, "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento de botón Registrar: abre el formulario de alta de clientes desde el menú principal.
        /// </summary>
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            FrmMenuPrincipal padre = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (padre == null) return;

            try
            {
                padre.AbrirEditClientes(sender, e, EnumModoForm.Alta);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sobre el menú principal: " + ex.Message, "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento de botón Importar: ejecuta la importación masiva de clientes desde un archivo Excel/CSV.
        /// </summary>
        private void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Abrimos el selector de archivos
                string? rutaArchivo = ArchivosOfficce.SeleccionarArchivoXLSX();

                // Si la ruta es nula o vacía, es porque el usuario le dio a "Cancelar" en la ventana
                if (string.IsNullOrWhiteSpace(rutaArchivo)) return;

                // 2. Cambiamos el cursor a "Cargando" para darle feedback visual al usuario
                Cursor.Current = Cursors.WaitCursor;

                // 3. Instanciamos el importador pasándole la ruta
                ImportarClientes importador = new ImportarClientes(rutaArchivo);

                if (!importador.hayArchivo)
                {
                    Mensajes.MensajeAdvertencia("El archivo seleccionado no es válido o no existe.");
                    return;
                }

                // 4. Ejecutamos la magia
                var resultado = importador.ImportarArchivoClientes();

                // 5. Devolvemos el feedback usando la clase centralizada
                if (!resultado.Success)
                {
                    Mensajes.MensajeError($"Se detuvo la importación por el siguiente error:\n{resultado.Mensaje}");
                }
                else
                {
                    // El resultado.Mensaje trae el resumen armado (Nuevos: X, Actualizados: Y, etc)
                    Mensajes.MensajeExito(resultado.Mensaje);

                    btnBuscar_Click(sender, e); // Refrescamos la grilla para mostrar los cambios
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error desde la UI al intentar importar clientes: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un problema inesperado al procesar el archivo.");
            }
            finally
            {
                // 6. Aseguramos que el cursor vuelva a la normalidad pase lo que pase
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Exporta los datos visibles en la grilla a un archivo de Excel.
        /// </summary>
        private void btnExportar_Click(object sender, EventArgs e)
        {
            // 1. Verificamos que haya datos cargados en memoria
            if (_clientes == null || !_clientes.Any())
            {
                Mensajes.MensajeAdvertencia("No hay clientes cargados en la lista para exportar.");
                return;
            }

            try
            {
                // 2. Exportamos SOLO lo que el usuario filtró en la grilla
                // y no toda la base de datos, puedes sacando la lista del BindingSource:
                var listaAExportar = bindingClientes.List.OfType<Personas>().Select(p => _clientes.First(c => c.IdPersona == p.IdPersona)).ToList();

                // Opcional: Para hacerla sencilla y exportar toda la lista actual (_clientes):
                // var resultado = ArchivosOfficce.ExportarClientesAExcel(_clientes);

                var resultado = ArchivosOfficce.ExportarClientesAExcel(listaAExportar);

                // 3. Feedback
                if (resultado.Success)
                {
                    Mensajes.MensajeExito(resultado.Mensaje);
                }
                else
                {
                    Mensajes.MensajeAdvertencia(resultado.Mensaje);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error desde la UI al intentar exportar: {ex.ToString()}");
                Mensajes.MensajeError("Error inesperado al exportar los datos.");
            }
        }

        /// <summary>
        /// Evento de cambio de texto en combo de búsqueda: ajusta labels y visibilidad de localidad.
        /// </summary>
        private void cbBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (cerrando) return;

            string opcion = cbBusqueda.Text;
            if (string.IsNullOrEmpty(opcion)) return;

            if (opcion == "Dni, Nombres")
                CambiarNombresCampos("Dni", "Nombres", false);
            else if (opcion == "Domicilio")
                CambiarNombresCampos("Calle", "Barrio", true);
            else
                CambiarNombresCampos("Whatsapp", "Teléfono", false);
        }

        /// <summary>
        /// Evento de cambio de visibilidad del combo de localidad: carga localidades si corresponde.
        /// </summary>
        private void cbLocalidad_VisibleChanged(object sender, EventArgs e)
        {
            if (cerrando) return;

            if (!cbLocalidad.Visible)
            {
                BuscarLocalidades();
                return;
            }
            _localidades = null;
        }

        /// <summary>
        /// Evento de cambio en el binding de clientes: actualiza cliente y persona seleccionados.
        /// </summary>
        private void bindingClientes_CurrentChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando) return;
            if (_personas == null) return;

            _persona = null;
            try
            {
                _persona = bindingClientes.Current as Personas;
                if (_persona == null) return;

                _cliente = _clientes?.FirstOrDefault(c => c.IdPersona == _persona.IdPersona);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ActualizarGrillaContactos();
        }

        /// <summary>
        /// Evento de botón Consultar: abre el formulario de consulta de clientes desde el menú principal.
        /// </summary>
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            if (_persona == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para consultar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            FrmMenuPrincipal padre = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
            if (padre != null)
            {
                padre.AbrirEditClientes(sender, e, EnumModoForm.Consulta, _cliente);
            }
        }
    }
}
