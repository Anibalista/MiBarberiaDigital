using Entidades_SGBM;
using Negocio_SGBM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

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
        private List<CostosServicios>? _costosNuevos;
        private string[] columnas = { "IdCostoServicio", "IdProducto", "IdServicio", "Productos", "Servicios" };
        public FrmEditServicios()
        {
            InitializeComponent();
        }

        private void cerrarFormulario()
        {
            cerrando = Validaciones.confirmarCierre();
            if (cerrando)
                Close();
        }

        private void FrmEditServicios_Load(object sender, EventArgs e)
        {
            cargarProductos();
            cargarCategorias();
            cargarInsumos();
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
                    _costos = ServiciosNegocio.ObtenerInsumosPorIdServicio((int)_servicio.IdServicio, ref mensaje);
                else
                    return;

                if (_costos == null)
                    _costos = new();

                if (_costosNuevos != null)
                    _costos.AddRange(_costosNuevos);

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

                //Ahora costo
                _servicio.Costos = validarCampoNumerico(txtCostosServicio, false);
                if (_servicio.Costos < 0)
                    correcto = false;

                if (correcto)
                {
                    decimal comision = checkComision.Checked && _servicio.Comision > 0 ? _servicio.PrecioVenta * (_servicio.Comision / 100) : 0;
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
            cerrarFormulario();
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
                cargarInsumoAGrilla();
            } 
            catch
            {
                return;
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

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
            DialogResult respuesta = MessageBox.Show($"¿Confirma que desea {accion} el servicio?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta == DialogResult.No)
                return;

            string mensaje = string.Empty;
            try
            {
                //Valido que los campos son correctos
                if (!validarServicio(ref mensaje))
                {
                    MessageBox.Show("Error:" + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (modo == EnumModoForm.Alta)
                {
                    if (!ServiciosNegocio.Registrar(_servicio, ref mensaje))
                    {
                        MessageBox.Show("Error" + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                }
                else
                {

                }
                DialogResult seguir = MessageBox.Show(mensaje + "\n¿Desea registrar un seguir servicio?", "Finalizado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (seguir == DialogResult.Yes)
                {
                    limpiarCampos();
                    cargarProductos();
                    cargarCategorias();
                    cargarInsumos();
                }
                else
                {
                    cerrarFormulario();
                }
                return;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al {accion} el servicio:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Verifique los siguientes errores\n" + error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                error = validarCamposInsumos(txtDescripcionInsumo.Text, txtMontoInsumo.Text);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            } 
            catch
            {
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
                _costosNuevos = null;
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
        private void cargarInsumoAGrilla()
        {
            cargando = true;
            string mensaje = string.Empty;
            string descripcion = txtDescripcionInsumo.Text.Trim();
            string montoTexto = txtMontoInsumo.Text.Trim();
            string cantidad = txtCantidad.Text.Trim();
            string unidades = txtUnidades.Text.Trim();
            if (_costoServicio == null)
                _costoServicio = new();
            
            try
            {
                mensaje = validarCamposInsumos(descripcion, montoTexto);
                if (!string.IsNullOrEmpty(mensaje))
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _costoServicio.Descripcion = txtDescripcionInsumo.Text;
                if (!calcularCostoInsumo(ref mensaje))
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int id = -1;
                if (_costos == null)
                    _costos = new();

                if (_costosNuevos == null)
                    _costosNuevos = new();
                else
                    id = _costosNuevos.Count > 0 ? _costosNuevos.Min(c => (int)c.IdCostoServicio) - 1 : -1;
                
                _costoServicio.IdCostoServicio = id;

                _costosNuevos.Add(_costoServicio);
                _costos.Add(_costoServicio);
                cargando = false;
                refrescarGrilla();
                sumarCostos(ref mensaje);
                limpiarCamposInsumos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el insumo-resultado:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally
            {
                cargando = false;
            }
        }

        private bool calcularCostoInsumo(ref string mensaje)
        {
            if (_costoServicio == null)
                return false;
            try
            {
                
                int medida = obtenerNumero(txtCantidad.Text, ref mensaje);
                _costoServicio.CantidadMedida = medida > 0 ? medida : null;
                errorCampo(txtCantidad, mensaje);
                mensaje = string.Empty;

                int unidades = obtenerNumero(txtUnidades.Text, ref mensaje);
                _costoServicio.Unidades = unidades < 0 ? 0 : unidades;
                txtUnidades.Text = _costoServicio.Unidades.ToString();
                errorCampo(txtCantidad, mensaje);
                mensaje = string.Empty;

                decimal costo = obtenerDecimal(txtMontoInsumo.Text, ref mensaje);
                _costoServicio.Costo = costo;

                if (_productoSeleccionado != null)
                {
                    _costoServicio.Costo = _productoSeleccionado.Costo > 0 ? _productoSeleccionado.Costo * (int)_costoServicio.Unidades : costo;
                    if (_productoSeleccionado.Medida != null)
                    {
                        if (_productoSeleccionado.Medida > 0)
                            _costoServicio.Costo += (_productoSeleccionado.Costo / (int)_productoSeleccionado.Medida) * medida;
                    }
                    _costoServicio.IdProducto = _productoSeleccionado.IdProducto;
                }

                txtMontoInsumo.Text = _costoServicio.Costo.ToString();

                return _costoServicio.Costo > 0;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }

        }

        private string validarCamposInsumos(string descripcion, string monto)
        {
            string mensaje = string.Empty;
            try
            {
                if (!validarTexto(txtDescripcionInsumo, true, false))
                {
                    return "Antes de continuar verifique los campos con error";
                }
                if (string.IsNullOrWhiteSpace(monto))
                {
                    errorCampo(txtMontoInsumo, "¡Campo obligatorio!");
                    return "Antes de continuar verifique los campos con error";
                }
                if (!esnumeroDecimal(monto, ref mensaje))
                {
                    errorCampo(txtMontoInsumo, mensaje);
                    return "Antes de continuar verifique los campos con error";
                }
                errorCampo(txtMontoInsumo, "");
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al cargar el insumo-resultado:\n" + ex.Message;
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
        private bool esCaracterNumerico(char c)
        {
            return char.IsNumber(c);
        }

        private bool esCaracterDecimal(char c)
        {
            return Validaciones.esDigitoDecimal(c);
        }

        private bool esnumeroDecimal(string numero, ref string mensaje)
        {
            return Validaciones.esNumeroDecimal(numero, ref mensaje);
        }

        //Eventos de validación de campos numéricos
        private void validarSoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !esCaracterNumerico(e.KeyChar);
        }

        private void validarSoloDecimales_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !esCaracterDecimal(e.KeyChar);
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
