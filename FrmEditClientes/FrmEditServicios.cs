using Entidades_SGBM;
using Negocio_SGBM;
using Front_SGBM.UXDesign;
using System.Data;
using System.Globalization;

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

        private void cerrarFormulario(object sender, EventArgs e)
        {
            cerrando = Validaciones.confirmarCierre();
            if (!cerrando)
                return;
            try
            {
                FrmMenuPrincipal padre = Application.OpenForms.OfType<FrmMenuPrincipal>().FirstOrDefault();
                if (padre != null)
                {
                    padre.abrirAbmServicios(sender, e, EnumModoForm.Consulta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sobre el menu principal" + ex.Message, "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            this.Close();
        }

        private void FrmEditServicios_Load(object sender, EventArgs e)
        {
            cargarProductos();
            cargarCategorias();
            if (modo != EnumModoForm.Alta)
                cargarServicio();
            cargarInsumos();
            if (modo == EnumModoForm.Consulta)
                activarCampos(false);
        }

        private void cargarServicio()
        {
            if (_servicio == null)
                return;
            if (_servicio.IdServicio == null)
                return;

            try
            {
                txtServicio.Text = _servicio.NombreServicio;
                txtDescripcionServicio.Text = _servicio.Descripcion ?? "";
                txtDuracion.Text = _servicio.DuracionMinutos.ToString("0.00") ?? "";
                txtPuntaje.Text = _servicio.Puntaje.ToString("0.00") ?? "";
                txtPrecio.Text = _servicio.PrecioVenta.ToString("0.00");
                txtComision.Text = _servicio.Comision.ToString("0.00");
                seleccionarCategoria();
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error: " + ex.Message);
                return;
            }
        }

        private void activarCampos(bool activos)
        {
            txtServicio.Enabled = activos;
            txtDescripcionServicio.Enabled = activos;
            txtDuracion.Enabled = activos;
            txtPrecio.Enabled = activos;
            txtPuntaje.Enabled = activos;
            txtComision.Enabled = activos;
            cbCategoria.Enabled = activos;
            btnAdminCostos.Enabled = activos;
            administrandoCostos = activos;
            activarCamposInsumos();
            btnGuardar.Enabled = activos;
        }

        private void seleccionarCategoria()
        {
            if (_categorias == null)
                return;
            if (_servicio.IdCategoria < 1)
                return;
            try
            {
                cbCategoria.SelectedValue = _servicio.IdCategoria;
                if (cbCategoria.SelectedIndex == -1)
                    Mensajes.mensajeAdvertencia("Problema al seleccionar la categoría");
                
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error: " + ex.Message);
                return;
            }
        }

        private void cargarProductos()
        {
            if (cerrando)
                return;
            string mensaje = string.Empty;
            try
            {
                bindingSourceProductos.DataSource = null;
                _productos = null;
                _productos = ProductosNegocio.ListaSimple(ref mensaje);
                if (string.IsNullOrEmpty(mensaje))
                    bindingSourceProductos.DataSource = _productos;
                else
                    MessageBox.Show("Error al traer productos de la BD", "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbProductos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al traer productos de la BD\n" + ex.Message, "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbProductos.Enabled = false;
            }
        }
        private void cargarCategorias()
        {
            if (cerrando)
                return;
            try
            {
                bindingSourceCategorias.DataSource = null;
                _categorias = null;
                string mensaje = string.Empty;
                _categorias = CategoriasNegocio.Listar(ref mensaje);
                if (!string.IsNullOrWhiteSpace(mensaje))
                    MessageBox.Show(mensaje, "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);

                bindingSourceCategorias.DataSource = _categorias;
                cbCategoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al traer categorías de la BD\n" + ex.Message, "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbCategoria.Enabled = false;
            }
        }

        private void ocultarColumnasGrilla(DataGridView dgv)
        {
            try
            {
                if (columnas == null || columnas.Length == 0 || dgv.Columns == null)
                    return;

                // Uso HashSet para búsquedas rápidas O(1)
                var columnasOcultar = new HashSet<string>(
                    columnas.Select(c => c.ToLower())
                );

                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.Visible = !columnasOcultar.Contains(column.Name.ToLower());
                }

            }
            catch
            {
                throw;
            }
        }

        private void cargarInsumos()
        {
            if (cerrando)
                return;
            try
            {
                cargando = true;
                dataGridInsumos.DataSource = null;
                _costos = null;
                string mensaje = "";
                if (_servicio != null && _servicio.IdServicio != null)
                    _costos = CostosNegocio.ObtenerInsumosPorIdServicio((int)_servicio.IdServicio, ref mensaje);
                else
                    return;

                if (_costos == null)
                    _costos = new();

                sumarCostos(ref mensaje);

                if (!string.IsNullOrWhiteSpace(mensaje))
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al traer Costos e Insumos de la BD\n" + ex.Message, "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
                refrescarGrilla();
            }
        }

        private void sumarCostos(ref string mensaje)
        {
            if (_costos == null)
                return;
            string total = "0";
            try
            {
                decimal costosTotales = 0;
                if (_costos.Count > 0)
                {
                    costosTotales = _costos.Sum(c => c.Costo);
                }
                total = costosTotales.ToString("0.00");
                txtTotalCostos.Text = total;
                txtCostosServicio.Text = total;
                calcularMargen(ref mensaje);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al sumar costos\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void refrescarGrilla()
        {
            if (cargando)
                return;
            try
            {
                if (_costos == null)
                    _costos = new();

                cargando = true;
                List<CostosServicios> lista = new List<CostosServicios>();
                dataGridInsumos.DataSource = null;
                txtTotalCostos.Clear();
                
                if (_costos.Count > 0)
                    lista = _costos.OrderBy(i => i.Descripcion).ToList();
                

                decimal totalCostos = lista.Sum(i => i.Costo);
                txtTotalCostos.Text = totalCostos.ToString("N2", CultureInfo.GetCultureInfo("es-AR"));

                dataGridInsumos.DataSource = lista;
                txtCostosServicio.Text = txtTotalCostos.Text;
                ocultarColumnasGrilla(dataGridInsumos);
                formatoGrilla();
                dataGridInsumos.Refresh();
                dataGridInsumos.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al refrescar la grilla de Costos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
            }
        }

        private void formatoGrilla()
        {
            try
            {
                // Aplica formato de moneda a la columna "Costo"
                EstiloAplicacion.ApplyCurrencyFormat(dataGridInsumos, "Costo");

                // Ajusta el FillWeight según el nombre de la columna
                foreach (DataGridViewColumn col in dataGridInsumos.Columns)
                {
                    switch (col.Name)
                    {
                        case "Costo":
                            col.FillWeight = 20;
                            col.MinimumWidth = 40;
                            break;
                        case "Descripción":
                            col.FillWeight = 50;
                            col.MinimumWidth = 120;
                            break;
                        case "Unidades":
                            col.FillWeight = 15;
                            col.MinimumWidth = 50;
                            break;
                        case "CantidadMedida":
                            col.FillWeight = 15;
                            col.MinimumWidth = 55;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Eventos de cálculos
        private bool calcularMargen(ref string mensaje)
        {
            bool correcto = true;
            try
            {
                //Solo creo el servicio para el calculo y así tampoco interrumpo la carga
                if (_servicio == null)
                    _servicio = new();

                //valido precio de venta
                _servicio.PrecioVenta = validarCampoNumerico(txtPrecio, true);
                if (_servicio.PrecioVenta <= 0)
                {
                    mensaje = "Verifique los campos señalados";
                    correcto = false;
                }

                //Sigue comisión con limites de porcentaje (0 a 100)
                _servicio.Comision = validarCampoNumerico(txtComision, false, 0, 100);
                if (_servicio.Comision < 0)
                    correcto = false;
                else
                    _servicio.Comision /= 100;

                //Ahora costo
                _servicio.Costos = validarCampoNumerico(txtCostosServicio, false);
                if (_servicio.Costos < 0)
                    correcto = false;

                if (correcto)
                {
                    decimal comision = checkComision.Checked && _servicio.Comision > 0 ? _servicio.PrecioVenta * _servicio.Comision : 0;
                    _servicio.Margen = _servicio.PrecioVenta - comision - _servicio.Costos;
                    txtMargen.Text = _servicio.Margen.ToString("0.00");
                }
                else
                {
                    txtMargen.Text = "";
                }

                return correcto;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al calcular margen" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //Eventos de botones
        private void btnSalir_Click(object sender, EventArgs e)
        {
            cerrarFormulario(sender, e);
        }

        private void btnAdminCostos_Click(object sender, EventArgs e)
        {
            administrandoCostos = !administrandoCostos;
            activarCamposInsumos();
        }

        private void btnNuevoInsumo_Click(object sender, EventArgs e)
        {
            try
            {
                nuevoCostoEnGrilla();
            } 
            catch
            {
                return;
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_costoServicio == null || _costos == null)
            {
                Mensajes.mensajeError("Seleccione un costo a modificar");
                return;
            }
            if (_costoServicio.IdCostoServicio == null)
            {
                Mensajes.mensajeError("Problemas con e ID del costo a modificar");
                return;
            }
            cargando = true;
            try
            {
                string mensaje = string.Empty;
                if (!armarCostoServicio(ref mensaje))
                {
                    Mensajes.mensajeError(mensaje);
                    return;
                }
                int indice = _costos.FindIndex(c => c.IdCostoServicio == _costoServicio.IdCostoServicio);
                if (indice < 0)
                {
                    Mensajes.mensajeError("No se encuentra el Costo a modificar");
                    return;
                }
                _costos[indice] = _costoServicio;

                cargando = false;
                refrescarGrilla();
                sumarCostos(ref mensaje);
                limpiarCamposInsumos();
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error al cargar el insumo-resultado:\n" + ex.Message);
            }
            finally
            {
                cargando = false;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_costoServicio == null || _costos == null)
            {
                Mensajes.mensajeError("Seleccione un costo a modificar");
                return;
            }
            if (_costoServicio.IdCostoServicio == null)
            {
                Mensajes.mensajeError("Problemas con e ID del costo a modificar");
                return;
            }
            cargando = true;
            try
            {
                string mensaje = string.Empty;
                
                int indice = _costos.FindIndex(c => c.IdCostoServicio == _costoServicio.IdCostoServicio);
                if (indice < 0)
                {
                    Mensajes.mensajeError("No se encuentra el Costo a eliminar");
                    return;
                }
                DialogResult respuesta = Mensajes.respuesta("¿Confirma eliminar el costo?\nEsta acción no se puede deshacer");
                if (respuesta == DialogResult.No)
                    return;
                _costos.RemoveAt(indice);

                cargando = false;
                refrescarGrilla();
                sumarCostos(ref mensaje);
                limpiarCamposInsumos();
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error al cargar el insumo-resultado:\n" + ex.Message);
            }
            finally
            {
                cargando = false;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                limpiarCamposInsumos();
            }
            catch
            {
                MessageBox.Show("Error de interfaz", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string accion = modo == EnumModoForm.Alta ? "Registrar" : "Modificar";
            DialogResult respuesta = Mensajes.respuesta($"¿Confirma que desea {accion} el servicio?");
            if (respuesta == DialogResult.No)
                return;

            string mensaje = string.Empty;
            try
            {
                //Valido que los campos son correctos
                if (!validarServicio(ref mensaje))
                {
                    Mensajes.mensajeError("Error:" + mensaje);
                    return;
                }
                if (modo == EnumModoForm.Alta)
                {
                    if (!ServiciosNegocio.Registrar(_servicio, _costos, ref mensaje))
                    {
                        Mensajes.mensajeError("Error" + mensaje);
                        return;
                    }
                }
                else
                {
                    string mensajeCostos = string.Empty;
                    if (!ServiciosNegocio.Modificar(_servicio, ref mensaje))
                    {
                        Mensajes.mensajeError("Error" + mensaje);
                        return;
                    } else if (!CostosNegocio.GestionarCostosServicios(_costos, (int)_servicio.IdServicio, ref mensajeCostos))
                    {
                        Mensajes.mensajeAdvertencia(mensaje + mensajeCostos);
                        mensaje = string.Empty;
                    }
                }
                DialogResult seguir = Mensajes.respuesta(mensaje + "\n¿Desea registrar un nuevo servicio?");
                if (seguir == DialogResult.Yes)
                {
                    limpiarCampos();
                    cargarProductos();
                    cargarCategorias();
                    cargarInsumos();
                }
                else
                {
                    cerrarFormulario(sender, e);
                }
                return;

            }
            catch (Exception ex)
            {
                Mensajes.mensajeError($"Error al {accion} el servicio:\n{ex.Message}");
                return;
            }
        }

        private bool validarServicio(ref string mensaje)
        {
            if (modo == EnumModoForm.Alta || _servicio == null)
                _servicio = new();
            bool correcto = true; //Bandera para no cerrar el método y poner error provider en los campos incorrectos
            try
            {
                errorProvider1.Clear();
                //Primero nombre servicio (obligatorio)
                if (!validarTexto(txtServicio, true, true))
                    correcto = false;
                else
                    _servicio.NombreServicio = txtServicio.Text;

                //Siguiente descripcion
                if (validarTexto(txtDescripcionServicio, false, false))
                    _servicio.Descripcion = txtDescripcionServicio.Text;

                //Ahora duración (int)
                _servicio.DuracionMinutos = (int)validarCampoNumerico(txtDuracion, false);
                //Chequeo 
                if (_servicio.DuracionMinutos < 0)
                    correcto = false;

                //Sigue puntaje
                _servicio.Puntaje = (int)validarCampoNumerico(txtPuntaje, false);
                if (_servicio.Puntaje < 0)
                    correcto = false;

                //Todos los numericos (Calculo y verifico)
                if (!calcularMargen(ref mensaje))
                    correcto = false;

                //Categoría
                Categorias? seleccionada = categoriaSeleccionada(ref mensaje);
                if (seleccionada != null)
                {
                    if (seleccionada.IdCategoria == null)
                        _servicio.Categorias = seleccionada;
                    else
                        _servicio.IdCategoria = (int)seleccionada.IdCategoria;
                }

                ////ver costos
                return correcto;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }

        }

        private void errorCampo(TextBox campo, string error)
        {
            try
            {
                errorProvider1.SetError(campo, error);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool validarTexto(TextBox campo, bool obligatorio, bool capitalizarTodo) //Validador genérico de campos de texto
        {
            string mensajeError = string.Empty;
            try
            {
                //Obtengo el texto del campo
                string texto = campo.Text.Trim();
                if (Validaciones.textoCorrecto(texto, ref mensajeError)) // compruebo si está bien
                {
                    campo.Text = Validaciones.capitalizarTexto(texto, !capitalizarTodo); //Lo capitalizo
                }
                else if (obligatorio) //Si es obligatorio lo marco con el provider
                {
                    errorCampo(campo, mensajeError); //Muestro el errror
                    return false;
                }
                else
                    campo.Text = "";

                //Limpio primero si tiene errores (Si el mensaje está vacío se se limpia)
                errorCampo(campo, mensajeError);
                return true;

            }
            catch
            {
                return false;
            }
        }

        private decimal validarCampoNumerico(TextBox campo, bool obligatorio, int minimo = 0, int maximo = int.MaxValue)
        {
            string mensajeError = string.Empty;
            try
            {
                //Obtengo el texto del campo
                string texto = campo.Text.Trim();
                //Declaro el resultado
                decimal resultado = 0;

                //Verifico si está vacío
                if (string.IsNullOrWhiteSpace(texto))
                {
                    if (obligatorio) //Si está vacío y es obligatorio devuelvo el menor valor de un entero
                    {
                        errorCampo(campo, "¡Campo obligatorio!"); //Le pongo el error al campo
                        return decimal.MinValue;
                    }
                    errorCampo(campo, mensajeError);//Si está bien mensajeError está vacío y se limpia el error
                    return 0; //Si no es obligatorio solo lo dejo en cero
                }
                resultado = obtenerDecimal(texto, ref mensajeError); //Obtengo el número si no está vacío
                if (!string.IsNullOrWhiteSpace(mensajeError)) //Si hubo error se captó en el mensaje
                {
                    errorCampo(campo, mensajeError);
                    return decimal.MinValue;
                }

                //Verifico si está en el rango
                if (resultado < minimo)
                    mensajeError = $"No puede ser menor a {minimo}";
                if (resultado > maximo)
                    mensajeError = $"No puede ser mayor a {maximo}";

                //Si no está en el rango el mensaje tiene texto
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    errorCampo(campo, mensajeError);
                    return decimal.MinValue;
                }
                errorCampo(campo, mensajeError);//Si está bien mensajeError está vacío y se limpia el error
                
                return resultado;
            }
            catch
            {
                return -1;
            }
        }


        private void cbProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            if (_costoServicio == null)
                _costoServicio = new();
            try
            {
                _productoSeleccionado = productoSeleccionado();
                if (_productoSeleccionado == null)
                    return;
                txtDescripcionInsumo.Text = _productoSeleccionado.ToString();
                string error = "";
                if (calcularCostoInsumo(ref error))
                {
                    txtMontoInsumo.Text = _costoServicio.Costo.ToString();
                    _costoServicio.Productos = _productoSeleccionado;
                    _costoServicio.IdProducto = _productoSeleccionado.IdProducto;
                } else
                {
                    Mensajes.mensajeError(error);
                    return;
                }
            } 
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error inesperado\n" + ex.Message);
                return;
            }
            
        }

        private void dataGridInsumos_SelectionChanged(object sender, EventArgs e)
        {
            if (cerrando || cargando)
                return;
            try
            {
                limpiarCamposInsumos();
                if (_costos == null)
                    return;
                _costoServicio = insumoSeleccionado();
                cargarInsumoACampos();
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Ocurrió un problema con la selección de costos\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void cargarInsumoACampos()
        {
            string mensaje = string.Empty;
            try
            {
                if (_costoServicio == null)
                    return;
                txtDescripcionInsumo.Text = _costoServicio.ToString();
                cargando = true;
                if (_costoServicio.IdProducto != null && _productos != null)
                {
                    cbProductos.SelectedValue = _costoServicio.IdProducto;

                    if (cbProductos.SelectedIndex != -1)
                    {
                        // El producto existe en el BindingSource
                        _costoServicio.Productos = cbProductos.SelectedItem as Productos;
                        _productoSeleccionado = _costoServicio.Productos;
                    }
                    else
                    {
                        // El producto ya no existe en la BD
                        mensaje = "Problemas al encontrar el producto del insumo";
                        _costoServicio.Productos = null;
                        _costoServicio.IdProducto = null;
                    }

                }
                txtCantidad.Text = _costoServicio.CantidadMedida.ToString();
                txtUnidades.Text = _costoServicio.Unidades.ToString();
                txtMontoInsumo.Text = _costoServicio.Costo.ToString("0.00");
                cargando = false;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(mensaje))
                    throw new Exception(mensaje);
            }
        }

        //Carga de selecciones
        private Productos? productoSeleccionado()
        {
            if (cbProductos.SelectedIndex < 0) return null;
            try
            {
                return (Productos)cbProductos.SelectedItem;
            }
            catch
            {
                return null;
            }
        }

        private CostosServicios? insumoSeleccionado()
        {
            try
            {
                if (dataGridInsumos == null || dataGridInsumos.CurrentRow == null)
                    return null;
                return dataGridInsumos.CurrentRow.DataBoundItem as CostosServicios;
            }
            catch
            {
                return null;
            }
        }

        private Categorias? categoriaSeleccionada(ref string mensaje)
        {
            try
            {
                if (!Validaciones.textoCorrecto(cbCategoria.Text.Trim(), ref mensaje))
                    return null;
                Categorias? seleccionada = _categorias.FirstOrDefault(c => c.Descripcion.ToLower() == cbCategoria.Text.Trim().ToLower());
                if (seleccionada == null)
                {
                    seleccionada = new Categorias();
                    seleccionada.Descripcion = Validaciones.capitalizarTexto(cbCategoria.Text.Trim());
                }
                return seleccionada;
            }
            catch
            {
                return null;
            }
        }

        //Activación y Limpieza de campos
        private void activarCamposInsumos()
        {
            try
            {
                txtDescripcionInsumo.Enabled = administrandoCostos;
                txtMontoInsumo.Enabled = administrandoCostos;
                cbProductos.Enabled = administrandoCostos;
                txtCantidad.Enabled = administrandoCostos;
                txtUnidades.Enabled = administrandoCostos;
                btnEliminar.Enabled = administrandoCostos;
                btnLimpiar.Enabled = administrandoCostos;
                btnModificar.Enabled = administrandoCostos;
                btnNuevoInsumo.Enabled = administrandoCostos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar los campos de Insumos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void limpiarCamposInsumos()
        {
            cargando = true;
            try
            {
                txtDescripcionInsumo.Clear();
                txtMontoInsumo.Clear();
                txtCantidad.Clear();
                txtUnidades.Clear();
                cbProductos.SelectedIndex = -1;
                _productoSeleccionado = null;
                _costoServicio = null;
                limpiarErroresInsumos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar los campos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
            }
        }

        private void limpiarErroresInsumos()
        {
            errorCampo(txtDescripcionInsumo, "");
            errorCampo(txtMontoInsumo, "");
            errorCampo(txtCantidad, "");
            errorCampo(txtUnidades, "");
        }

        private void limpiarCampos()
        {
            cargando = true;
            try
            {
                _servicio = null;
                _costos = null;
                txtServicio.Clear();
                txtDescripcionServicio.Clear();
                txtDuracion.Clear();
                txtPrecio.Clear();
                txtPuntaje.Clear();
                txtComision.Clear();
                txtCostosServicio.Clear();
                txtMargen.Clear();
                txtTotalCostos.Clear();
                checkComision.Checked = true;
                administrandoCostos = false;
                limpiarCamposInsumos();
                activarCamposInsumos();
                cargando = false;
                refrescarGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar los campos:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
            }
        }

        //Carga de insumos (Costos) al servicio
        private bool armarCostoServicio(ref string mensaje)
        {
            if (_costoServicio == null)
                _costoServicio = new();
            bool error = false;
            try
            {
                if (!validarTexto(txtDescripcionInsumo, true, false))
                    error = true;
                _costoServicio.Descripcion = txtDescripcionInsumo.Text.Trim();

                decimal costo = 0;

                if (calcularCostoInsumo(ref mensaje))
                    error = error ? error : false;
                else
                    error = !string.IsNullOrWhiteSpace(mensaje);
                
                mensaje = validarCampoDecimal(txtMontoInsumo, true, ref costo);

                error = error ? error : !string.IsNullOrWhiteSpace(mensaje);
                _costoServicio.Costo = error ? 0 : decimal.Parse(txtMontoInsumo.Text);

                return !error;
                
            } catch (Exception ex)
            {
                mensaje = "Error inesperado\n" + ex.Message;
                return false;
            }
            
        }

        private void nuevoCostoEnGrilla()
        {
            cargando = true;
            if (_costoServicio == null)
                _costoServicio = new();
            if (_costoServicio.IdCostoServicio != null)
            {
                Mensajes.mensajeError("No puede volver a agregar un Costo existente, presione modificar");
                return;
            }
            try
            {
                string mensaje = string.Empty;
                if (!armarCostoServicio(ref mensaje))
                {
                    Mensajes.mensajeError(mensaje);
                    return;
                }
                
                int id = -1;
                if (_costos == null)
                    _costos = new();

                int idMinimo = _costos.Count > 0 ? _costos.Min(c => (int)c.IdCostoServicio) : 0;

                id = idMinimo > 0 ? -1 : idMinimo - 1;

                _costoServicio.IdCostoServicio = id;

                _costos.Add(_costoServicio);
                cargando = false;
                refrescarGrilla();
                sumarCostos(ref mensaje);
                limpiarCamposInsumos();
            }
            catch (Exception ex)
            {
                Mensajes.mensajeError("Error al cargar el insumo-resultado:\n" + ex.Message);
            } finally
            {
                cargando = false;
            }
        }

        private bool calcularCostoInsumo(ref string mensaje)
        {
            mensaje = string.Empty;
            if (_costoServicio == null)
                return false;
            if (_productoSeleccionado == null)
                return false;
            try
            {
                string mensajeCantidad = string.Empty;

                int medida = obtenerNumero(txtCantidad.Text, ref mensajeCantidad);
                _costoServicio.CantidadMedida = medida > 0 ? medida : null;

                int unidades = obtenerNumero(txtUnidades.Text, ref mensaje);
                _costoServicio.Unidades = unidades < 0 ? 0 : unidades;
                txtUnidades.Text = _costoServicio.Unidades.ToString();
                
                if (!string.IsNullOrWhiteSpace(mensajeCantidad) && !string.IsNullOrWhiteSpace(mensaje))
                {
                    errorCampo(txtCantidad, mensajeCantidad);
                    errorCampo(txtCantidad, mensaje);
                    mensaje = "Antes de continuar verifique los campos con error";
                    return false;
                } else
                {
                    errorCampo(txtCantidad, "");
                    errorCampo(txtCantidad, "");
                }

                mensaje = string.Empty;

                _costoServicio.Costo = _productoSeleccionado.Costo * (int)_costoServicio.Unidades;
                if (_productoSeleccionado.Medida != null)
                {
                    if (_productoSeleccionado.Medida > 0)
                        _costoServicio.Costo += (_productoSeleccionado.Costo / (int)_productoSeleccionado.Medida) * medida;
                }
                _costoServicio.IdProducto = _productoSeleccionado.IdProducto;
                _costoServicio.Productos = _productoSeleccionado;

                txtMontoInsumo.Text = _costoServicio.Costo.ToString("0.00");

                return _costoServicio.Costo > 0;
            }
            catch (Exception ex)
            {
                mensaje = "Error excepcional: " + ex.Message;
                return false;
            }

        }

        private string validarCampoDecimal(TextBox campo, bool obligatorio, ref decimal resultado)
        {
            string mensaje = string.Empty;
            try
            {
                resultado = 0;
                string texto = campo.Text.Trim();
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    if (!esnumeroDecimal(texto, ref mensaje))
                    {
                        errorCampo(campo, mensaje);
                        return "Antes de continuar verifique los campos con error";
                    } else
                    {
                        resultado = decimal.Parse(texto);
                        campo.Text = resultado.ToString("0.00");
                    }
                }
                if (obligatorio && resultado == 0)
                {
                    errorCampo(campo, "¡Campo obligatorio!");
                    return "Antes de continuar verifique los campos con error";
                }
                
                errorCampo(campo, "");
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error excepcional" + ex.Message;
            }
        }

        private int obtenerNumero(string cantidadTexto, ref string mensaje)
        {
            int cantidad = 0;
            if (!int.TryParse(cantidadTexto, out cantidad))
            {
                mensaje += "Error al convertir la unidades a número entero.";
                return 0;
            }
            return cantidad;
        }

        private decimal obtenerDecimal(string cantidadTexto, ref string mensaje)
        {
            decimal cantidad = 0;
            if (!esnumeroDecimal(cantidadTexto, ref mensaje))
            {
                return 0;
            }
            if (!decimal.TryParse(cantidadTexto, out cantidad))
            {
                mensaje += "Error al convertir la unidades a número entero.";
            }
            return cantidad;
        }

        //Validaciones numéricas
        private bool esnumeroDecimal(string numero, ref string mensaje)
        {
            return Validaciones.esNumeroDecimal(numero, ref mensaje);
        }

        //Eventos de validación de campos numéricos
        private void validarSoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !Validaciones.esDigitoNumerico(e.KeyChar);
        }

        private void validarSoloDecimales_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !Validaciones.esDigitoDecimal(e.KeyChar);
        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            if (_costoServicio == null)
                _costoServicio = new();

            try
            {
                string mensaje = string.Empty;
                if (!calcularCostoInsumo(ref mensaje))
                {
                    MessageBox.Show("Error: " + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                txtMontoInsumo.Text = _costoServicio.Costo.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void txtUnidades_Leave(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            if (_costoServicio == null)
                _costoServicio = new();
            try
            {
                string mensaje = string.Empty;
                if (!calcularCostoInsumo(ref mensaje))
                {
                    MessageBox.Show("Error: " + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                txtMontoInsumo.Text = _costoServicio.Costo.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void txtPrecio_Leave(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            obtenerMargen();
        }

        private void obtenerMargen()
        {
            string mensaje = string.Empty;
            try
            {
                if (!calcularMargen(ref mensaje))
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void checkComision_CheckedChanged(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            obtenerMargen();
        }

        private void txtComision_Leave(object sender, EventArgs e)
        {
            if (cerrando)
                return;
            obtenerMargen();
        }
    }
}
