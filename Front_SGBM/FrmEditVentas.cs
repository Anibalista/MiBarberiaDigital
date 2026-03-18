using Entidades_SGBM;
using Front_SGBM.UXDesign;
using Negocio_SGBM;
using Utilidades;
using System.Data;

namespace Front_SGBM
{
    public partial class FrmEditVentas : Form
    {
        #region Inicialización y Carga de Datos

        #region Clases internas y variables
        // --- Clase interna para unificar Productos y Servicios en la grilla izquierda ---
        private class ItemSeleccion
        {
            public int? Id { get; set; }
            public string Codigo { get; set; } // Para productos, el código de barras; para servicios, puede ser un código interno o vacío
            public string Descripcion { get; set; }
            public decimal Precio { get; set; }
            public decimal Efectivo { get; set; }

            // Referencias originales ocultas por si las necesitamos al añadir al carrito
            public Productos? ProductoOrigen { get; set; }
            public Servicios? ServicioOrigen { get; set; }
        }

        // --- Variables de estado ---
        private List<ItemSeleccion> _catalogoItems = new List<ItemSeleccion>();
        private List<DetallesVentas> _carrito = new List<DetallesVentas>();
        private Clientes? _clienteActual = null;
        private Empleados? _vendedorActual = null;
        private Estados? _estadoActual = null;
        private decimal? descuento = null;
        private bool cerrando = false; // Para controlar el cierre del formulario
        private bool cargando = false; // Para evitar eventos durante la carga inicial

        #endregion

        #region Constructor y Load

        public FrmEditVentas()
        {
            InitializeComponent();
        }

        private void FrmEditVentas_Load(object sender, EventArgs e)
        {
            // 1. Cargar Barberos (Vendedores)
            if (!CargarVendedores())
            {
                Mensajes.MensajeError("No hay barberos/empleados registrados en el sistema. Debe registrar al menos uno para iniciar ventas.");
                btnGuardar.Enabled = false;
                btnBuscarCliente.Enabled = false;
                btnSeleccionar.Enabled = false;
                cbMedioPago.Enabled = false;
                return;
            }

            // 2. Cargar Nro de Venta
            CargarNroVenta();

            // 3. Cargar Medios de Pago
            CargarMediosPago();

            // 4. Cargar la grilla izquierda (Catálogo unificado)
            CargarCatalogoSeleccion();

            // 4. Cargar Estado
            CargarEstadoGenerico();

            // 5. Mostrar el catálogo en la grilla
            ActualizarGrillaSeleccion(_catalogoItems);

        }

        #endregion

        #region Métodos de carga y actualización de datos

        private void CargarCatalogoSeleccion()
        {
            _catalogoItems = new List<ItemSeleccion>();
            var resServicios = ServiciosNegocio.ListarActivos();
            if (resServicios.Success && resServicios.Data != null)
            {
                _catalogoItems.AddRange(resServicios.Data.Select(s => new ItemSeleccion
                {
                    Id = s.IdServicio,
                    Codigo = string.Empty,
                    Descripcion = s.NombreServicio,
                    Precio = s.PrecioLista,
                    Efectivo = s.PrecioContado ?? s.PrecioLista,
                    ServicioOrigen = s
                }));
            }

            var resProductos = ProductosNegocio.GetProductosActivos(checkStock.Checked);
            if (resProductos.Success && resProductos.Data != null)
            {
                _catalogoItems.AddRange(resProductos.Data.Select(p => new ItemSeleccion
                {
                    Id = p.IdProducto,
                    Codigo = p.CodProducto,
                    Descripcion = p.Descripcion,
                    Precio = p.PrecioVenta,
                    Efectivo = p.PrecioVenta,
                    ProductoOrigen = p
                }));
            }
        }

        /// <summary>
        /// Carga los empleados. Retorna false si la lista viene vacía o hay error.
        /// </summary>
        private bool CargarVendedores()
        {

            try
            {
                var resultado = EmpleadosNegocio.GetEmpleados(false); // Sin anulados
                if (!resultado.Success || resultado.Data == null || !resultado.Data.Any())
                    return false;

                bindingEmpleados.DataSource = resultado.Data;
                cbVendedor.SelectedIndex = -1; // Obligamos a que lo elijan

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al cargar vendedores en Ventas: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Carga medios de pago. La regla de negocio de crearlos si no existen está en la capa Negocio.
        /// </summary>
        private void CargarMediosPago()
        {
            cargando = true;
            try
            {
                // El método en Negocio se encargará de crear "Efectivo" y "Transferencia MP" si la tabla está vacía
                var resultado = FacturasNegocio.GetMediosPago();

                if (resultado.Success && resultado.Data != null)
                {
                    bindingMedios.DataSource = resultado.Data;

                    // Seleccionar "Efectivo" por defecto
                    var indexEfectivo = resultado.Data.FindIndex(m => m.Medio.Contains("Efectivo"));
                    if (indexEfectivo >= 0) cbMedioPago.SelectedIndex = indexEfectivo;
                }
                else
                {
                    Mensajes.MensajeError("No se pudieron cargar los medios de pago. Verifique la configuración del sistema.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al cargar medios de pago en Ventas: {ex.ToString()}");
            }
            finally { cargando = false; }
        }

        /// <summary>
        /// Obtiene el estado "En Curso" por defecto de una nueva venta o lo crea por defecto
        /// </summary>
        private void CargarEstadoGenerico()
        {
            _estadoActual = null;
            try
            {
                var resEstados = EstadosNegocio.GetEstado("Ventas", "En Curso");
                if (resEstados.Success && resEstados.Data != null)
                {
                    _estadoActual = resEstados.Data;
                } else
                {
                    Logger.LogError(resEstados.Mensaje);
                    _estadoActual = new Estados
                    {
                        IdEstado = 0,
                        Indole = "Ventas",
                        Estado = "En Curso"
                    };
                }
            } 
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                Mensajes.MensajeError("Error al obtener los estados de ventas");
            }
        }

        /// <summary>
        /// Obtiene el número de venta acumulativo. Si falla, asigna uno por defecto de emergencia.
        /// </summary>
        private void CargarNroVenta()
        {
            // 1. Asignamos el valor de emergencia desde el principio
            string nroVenta = $"{DateTime.Today:yyMMdd}-10000";

            try
            {
                // 2. Intentamos buscar el real
                var resNroVenta = VentasNegocio.GenerarProximoNroVenta();

                if (resNroVenta.Success && !string.IsNullOrWhiteSpace(resNroVenta.Data))
                {
                    // 3. ¡Éxito! Pisamos el de emergencia con el real
                    nroVenta = resNroVenta.Data;
                }
                else
                {
                    // 4. Falló, pero de forma controlada por nuestra capa de Negocio. 
                    // Logueamos el mensaje que nos mandó Negocio
                    Logger.LogError($"Error controlado al generar nro de venta: {resNroVenta.Mensaje}");
                }
            }
            catch (Exception ex)
            {
                // 5. Este catch queda SOLO para catástrofes de verdad (ej. se cortó la luz, se cayó SQL)
                Logger.LogError($"Excepción crítica al intentar obtener el nro de venta: {ex.ToString()}");
            }
            finally
            {
                // 6. Pase lo que pase (éxito, error controlado o catástrofe), mostramos el número
                lblNroVta.Text = nroVenta;
            }
        }


        /// <summary>
        /// Filtra el catálogo de productos y servicios según el texto ingresado en el filtro.
        /// Se llama desde el evento TextChanged del filtro.
        /// </summary>
        private void FiltrarCatalogo()
        {
            try
            {
                string filtro = txtFiltro.Text.Trim().ToLower();
                bool stock = checkStock.Checked;
                List<ItemSeleccion>? catalogoFiltrado = _catalogoItems.Where(i => i.ProductoOrigen == null ||
                                        (i.ProductoOrigen.Stock > 0 || stock)).ToList();
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    catalogoFiltrado = _catalogoItems.Where(i => i.Descripcion.ToLower().Contains(filtro)
                                            || i.Codigo.Contains(filtro)).ToList();
                }

                ActualizarGrillaSeleccion(catalogoFiltrado ?? _catalogoItems);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al filtrar catálogo en Ventas: {ex.ToString()}");
            }
        }

        #endregion

        #endregion

        #region Eventos de formulario

        #region Automáticos y Listeners

        private void TxtNumerico_KeyPress(object sender, KeyPressEventArgs e) => e.Handled = !Validaciones.EsDigitoNumerico(e.KeyChar);

        private void TxtDecimal_KeyPress(object sender, KeyPressEventArgs e) => e.Handled = !Validaciones.EsDigitoDecimal(e.KeyChar);

        private void Filtro_TextChanged(object sender, EventArgs e)
        {
            if (cerrando) return; // Evitamos que el filtro intente acceder a controles mientras se cierra el formulario
            FiltrarCatalogo();
        }

        private void TxtDescuento_TextChanged(object sender, EventArgs e)
        {
            if (cerrando) return; // Evitamos que el filtro intente acceder a controles mientras se cierra el formulario
            if (_carrito == null || !_carrito.Any()) return; // No hay items en el carrito, no hacemos nada
            try
            {
                ActualizarCarrito();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        private void CbMedioPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando) return; // Evitamos que el cambio de selección intente acceder a controles mientras se cierra el formulario
            if (_carrito == null || !_carrito.Any()) return; // No hay items en el carrito, no hacemos nada
            try
            {
                ActualizarCarrito();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        #endregion

        #region Botones y acciones principales

        private void checkStock_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando) return; // Evitamos que el cambio de filtro intente acceder a controles mientras se cierra el formulario
            FiltrarCatalogo();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            cerrando = Mensajes.ConfirmarCierre();
            if (cerrando)
            {
                this.Close();
            }
        }

        private void BtnBuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCliente())
                    return;
                if (_clienteActual?.IdCliente == null && _clienteActual?.NombreCompleto != null)
                    Mensajes.MensajeExito($"Se registrá a {_clienteActual.NombreCompleto} como cliente");
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                Mensajes.MensajeError("Error inesperado al obtener cliente. Se registrará la venta sin cliente");
            }
        }

        private Clientes? ObtenerCliente(out string mensaje)
        {
            mensaje = string.Empty;
            if (!CampoNumerico(txtDni))
            {
                mensaje = "Ingrese un Dni válido";
                return null;
            }
            try
            {
                var resultadoCliente = ClientesNegocio.GetClientePorDni(txtDni.Text.Trim());

                if (!resultadoCliente.Success)
                {
                    mensaje = resultadoCliente.Mensaje;
                    return null;
                }

                if (resultadoCliente.Data != null)
                    return resultadoCliente.Data;

                string nombres = txtNombreCompleto.Text.Trim();
                if (!Validaciones.TextoCorrecto(nombres, ref mensaje))
                    return null;

                txtNombreCompleto.Text = Validaciones.CapitalizarTexto(nombres);
                return new Clientes();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                mensaje = "Error inesperado al buscar clientes";
                return null;
            }
        }

        private void BtnSeleccionar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validamos que haya una fila seleccionada en el catálogo
                if (dataGridSeleccion.CurrentRow == null || dataGridSeleccion.CurrentRow.DataBoundItem == null)
                {
                    Mensajes.MensajeAdvertencia("Por favor, seleccione un producto o servicio de la lista.");
                    return;
                }

                // 2. Extraemos el objeto "Wrapper" que unificó Productos y Servicios
                var itemSeleccionado = (ItemSeleccion)dataGridSeleccion.CurrentRow.DataBoundItem;

                // 3. Leemos la cantidad (Asumiendo que el control se llama numCantidad)
                int cantidad = (int)numCantidad.Value;
                if (cantidad <= 0)
                {
                    Mensajes.MensajeAdvertencia("La cantidad debe ser mayor a cero.");
                    return;
                }

                // 4. Buscamos si el ítem YA EXISTE en el carrito y creamos el detalle nuevo nulo
                DetallesVentas? nuevoDetalle = null;

                DetallesVentas? detalleExistente = null;

                if (itemSeleccionado.ProductoOrigen != null)
                    detalleExistente = _carrito.FirstOrDefault(d => d.IdProducto == itemSeleccionado.Id);
                else if (itemSeleccionado.ServicioOrigen != null)
                    detalleExistente = _carrito.FirstOrDefault(d => d.IdServicio == itemSeleccionado.Id);

                // 5. Procesamos la inserción o actualización
                if (detalleExistente != null)
                {
                    // OPCIONAL: Control de Stock Inteligente para Productos
                    if (itemSeleccionado.ProductoOrigen != null &&
                        (detalleExistente.Cantidad + cantidad) > itemSeleccionado.ProductoOrigen.Stock)
                    {
                        Mensajes.MensajeAdvertencia($"Stock insuficiente. Solo quedan {itemSeleccionado.ProductoOrigen.Stock} unidades de este producto.\nVerifique su stock");
                    }

                    // Si ya existe, simplemente le sumamos la nueva cantidad
                    detalleExistente.Cantidad += cantidad;
                }
                else
                {
                    // OPCIONAL: Control de Stock para el primer ingreso
                    if (itemSeleccionado.ProductoOrigen != null && cantidad > itemSeleccionado.ProductoOrigen.Stock)
                    {
                        Mensajes.MensajeAdvertencia($"Stock insuficiente. Solo quedan {itemSeleccionado.ProductoOrigen.Stock} unidades de este producto.\nVerifique su stock");
                    }

                    // Si es nuevo, creamos la entidad DetallesVentas
                    nuevoDetalle = new DetallesVentas
                    {
                        Cantidad = cantidad,
                        Descripcion = itemSeleccionado.Descripcion
                    };

                    // Enlazamos la entidad original para que EF Core y CalcularPrecios puedan leerla
                    if (itemSeleccionado.ProductoOrigen != null)
                    {
                        nuevoDetalle.IdProducto = itemSeleccionado.Id;
                        nuevoDetalle.Productos = itemSeleccionado.ProductoOrigen;
                    }
                    else if (itemSeleccionado.ServicioOrigen != null)
                    {
                        nuevoDetalle.IdServicio = itemSeleccionado.Id;
                        nuevoDetalle.Servicios = itemSeleccionado.ServicioOrigen;
                    }

                }

                // 6. Reset de comodidad: Volvemos el contador a 1 para el próximo ítem
                numCantidad.Value = 1;

                // 7. Llamamos al método que actualiza todo y añade el detalle si es nuevo.
                ActualizarCarrito(nuevoDetalle);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al añadir ítem al carrito: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un problema al intentar agregar el ítem al carrito.");
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion

        #region Validaciones y construcción de objetos

        private bool CampoNumerico(Control campo)
        {
            string texto = campo.Text.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                ErrorCampo(campo, "Debe Ingresar");
                return false;
            }
            int numero = 0;
            if (!int.TryParse(texto, out numero))
            {
                ErrorCampo(campo, "Formato incorrecto");
                return false;
            }

            ErrorCampo(campo);
            return true;
        }

        private bool ValidarCliente()
        {
            try
            {
                _clienteActual = ObtenerCliente(out string mensaje);
                if (!string.IsNullOrWhiteSpace(mensaje))
                {
                    Mensajes.MensajeError(mensaje + "\nSe intentará registrar la venta sin cliente");
                    _clienteActual = null;
                    return false;
                }
                if (_clienteActual == null)
                {
                    Mensajes.MensajeAdvertencia("No hay cliente registrado con ese dni.\nSe registrará al cliente junto con la venta");
                    string[] nombres = txtDni.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string nombre = "";
                    string apellido = "";
                    if (nombres.Length == 1)
                        nombre = nombres[0];
                    else if (nombres.Length > 1)
                    {
                        // Si hay más de una palabra
                        apellido = nombres[nombres.Length - 1]; // La última palabra

                        // Tomamos todas las palabras excepto la última y las unimos con un espacio
                        nombre = string.Join(" ", nombres.Take(nombres.Length - 1));
                    }

                    _clienteActual = new Clientes
                    {
                        esMiembro = true,
                        IdCliente = null,
                        IdEstado = 0,
                        IdPersona = 0,
                        Personas = new Personas
                        {
                            IdPersona = null,
                            Dni = txtDni.Text,
                            Nombres = nombre,
                            Apellidos = apellido,
                            IdDomicilio = null,
                            Domicilios = null
                        }
                    };
                    return true;
                }
                txtNombreCompleto.Text = _clienteActual.NombreCompleto;
                return true;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                Mensajes.MensajeError("Error inesperado al obtener cliente. Se registrará la venta sin cliente");
                return false;
            }
        }

        private bool CampoDecimal(Control campo)
        {
            string texto = campo.Text.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                ErrorCampo(campo);
                return false;
            }
            string mensaje = "";
            bool esDecimal = Validaciones.EsNumeroDecimal(texto, ref mensaje);
            ErrorCampo(campo, mensaje);
            return esDecimal;
        }

        private Resultado<Ventas> ConstruirVenta()
        {
            if (_carrito == null || !_carrito.Any())
                return Resultado<Ventas>.Fail("Seleccione un servicio o producto a vender");
            

            try
            {
                if (cbVendedor.SelectedIndex == -1)
                {
                    return Resultado<Ventas>.Fail("Seleccione un barbero responsable antes de continuar");
                }

                Ventas? venta = new();
                _vendedorActual = (Empleados)cbVendedor.SelectedItem;
                venta.IdEmpleado = _vendedorActual?.IdEmpleado ?? 0;

                if (venta.IdEmpleado < 1)
                {
                    return Resultado<Ventas>.Fail("Error al obtener el barbero seleccionado");
                }

                string mensaje = string.Empty;
                if (!ValidarCliente())
                {
                    venta.IdCliente = -1;
                }
                else
                {
                    venta.IdCliente = _clienteActual?.IdCliente ?? 0;
                }

                venta.NroVenta = lblNroVta.Text;

                venta.FechaVenta = DateTime.Now;

                venta.IdEstado = _estadoActual?.IdEstado ?? 0;

                if (venta.IdEstado < 1)
                {
                    venta.Estados = new Estados
                    {
                        IdEstado = 0,
                        Indole = "Ventas",
                        Estado = "En Curso"
                    };
                }
                return Resultado<Ventas>.Ok(venta);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return Resultado<Ventas>.Fail("Error inesperado al construir la venta");
            }

            

        }

        #endregion

        #region Interacciones con grillas y campos

        /// <summary>
        /// Actualiza la grilla de selección (izquierda) con los productos y servicios activos.
        /// </summary>
        private void ActualizarGrillaSeleccion(List<ItemSeleccion> catalogoFiltrado)
        {
            try
            {
                dataGridSeleccion.DataSource = null;
                dataGridSeleccion.DataSource = catalogoFiltrado;
                OcultarColumnasSeleccion();
                FormatearColumnasSeleccion();
                dataGridSeleccion.Refresh();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al actualizar grilla de selección: {ex.ToString()}");
            }

        }

        /// <summary>
        /// Ocultar columnas innecesarias en la grilla de selección para el usuario final.
        /// </summary>
        private void OcultarColumnasSeleccion()
        {
            try
            {
                dataGridSeleccion.Columns["Id"].Visible = false;
                dataGridSeleccion.Columns["ProductoOrigen"].Visible = false;
                dataGridSeleccion.Columns["ServicioOrigen"].Visible = false;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al ocultar columnas en grilla de selección: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Aplica formato de moneda a las columnas de precio en la grilla de selección.
        /// </summary>
        private void FormatearColumnasSeleccion()
        {
            try
            {
                EstiloAplicacion.ApplyFormats(dataGridSeleccion, new Dictionary<string, string>
                {
                    { "Precio", "C2" },
                    { "Efectivo", "C2" }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al formatear columnas de precio en grilla de selección: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Actualiza la grilla carrito y los textos correspondientes
        /// </summary>
        private void ActualizarCarrito(DetallesVentas? detalle = null)
        {
            cargando = true;
            try
            {
                bindingCarrito.DataSource = null;

                _carrito ??= new();

                if (detalle != null)
                    _carrito.Add(detalle);

                if (CampoDecimal(txtDescuento))
                    descuento = decimal.Parse(txtDescuento.Text.Trim()) / 100;
                else
                    descuento = null;

                CalcularPreciosCarrito();
                bindingCarrito.DataSource = _carrito;
                FormatearColumnasCarrito();
                dataGridCarrito.Refresh();
            }
            catch ( Exception ex )
            {
                Logger.LogError(ex.ToString());
                Mensajes.MensajeError("Error al actualizar el carrito");
            } 
            finally { cargando = false; }
        }

        /// <summary>
        /// Calcula el total y el precio individual segun el medio de pago seleccionado
        /// y actualiza la interfaz gráfica.
        /// </summary>
        private void CalcularPreciosCarrito()
        {
            decimal totalLista = 0;
            decimal totalEfectivo = 0;
            decimal totalFinalACobrar = 0; // Este es el que rige para la UI
            decimal totalSinDescuento = 0;
            try
            {
                bool efectivo = cbMedioPago.Text.Equals("efectivo", StringComparison.OrdinalIgnoreCase);
                decimal desc = 1 - (descuento ?? 0);
                foreach (var detalle in _carrito)
                {
                    detalle.InteresDescuento = descuento;
                    
                    if (detalle.Productos != null)
                    {
                        // 1. Asignamos el precio unitario al detalle
                        detalle.PrecioUnitario = detalle.Productos.PrecioVenta;

                        // (Los productos mantienen el mismo precio en ambos casos)
                        totalEfectivo += detalle.SubTotal;
                        totalLista += detalle.SubTotal;
                    }
                    else if (detalle.Servicios != null)
                    {
                        decimal precioEfectivo = (detalle.Servicios.PrecioContado ?? detalle.Servicios.PrecioLista);
                        detalle.PrecioUnitario = efectivo ? precioEfectivo : detalle.Servicios.PrecioLista;
                        
                        // 2. Mantenemos tus acumuladores estadísticos por si quieres mostrar el "Ahorro" en pantalla
                        totalEfectivo += precioEfectivo * desc * detalle.Cantidad;
                        totalLista += detalle.Servicios.PrecioLista * desc * detalle.Cantidad;
                    }

                    // 3. El Total a cobrar se alimenta directamente de tu nueva propiedad mágica
                    totalFinalACobrar += detalle.SubTotal;
                    totalSinDescuento += detalle.PrecioUnitario * detalle.Cantidad;
                }

                // Actualizamos los TextBox visuales
                txtTotalEfectivo.Text = totalEfectivo.ToString("0.00");
                txtTotalACobrar.Text = totalLista.ToString("0.00");
                txtTotalAbonado.Text = totalFinalACobrar.ToString("0.00");

                decimal diferencia = totalSinDescuento - totalFinalACobrar;

                if (diferencia > 0)
                    lblDescuento.Text = $"Ahorro total: ${diferencia.ToString("0.00")}";
                else if (diferencia < 0)
                    lblDescuento.Text = $"Recargo total: ${diferencia.ToString("0.00")}";
                else
                    lblDescuento.Text = "";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                Mensajes.MensajeError("Error al calcular los precios del carrito.");
            }
        }

        /// <summary>
        /// Aplica formato de moneda a las columnas de precio
        /// </summary>
        private void FormatearColumnasCarrito()
        {
            try
            {
                EstiloAplicacion.ApplyFormats(dataGridCarrito, new Dictionary<string, string>
                {
                    {"TotalDetalle", "C2" },
                    {"PrecioUnitario", "C2" },
                    {"InteresDescuento", "P2" }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        private void ErrorCampo(Control campo, string mensaje = "") => errorProvider1.SetError(campo, mensaje);

        #endregion

    }
}
