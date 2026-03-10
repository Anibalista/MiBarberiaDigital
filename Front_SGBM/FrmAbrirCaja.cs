using Entidades_SGBM;
using Front_SGBM.UXDesign;
using Negocio_SGBM;
using Utilidades;

namespace Front_SGBM
{
    public partial class FrmAbrirCaja : Form
    {
        private List<TiposCajas>? _tiposCajas;
        private List<Empleados>? _empleados;
        private bool cerrando = false; // Para evitar múltiples triggers de cierre

        public FrmAbrirCaja()
        {
            InitializeComponent();
        }

        private void FrmAbrirCaja_Load(object sender, EventArgs e)
        {
            EstiloAplicacion.AplicarEstilo(this);

            CargarTiposCajas();
            CargarEmpleados();

            txtMonto.Text = "0";
            txtMonto.Focus();
            txtMonto.SelectAll();
        }

        private void CargarTiposCajas()
        {
            try
            {
                // Usamos el nuevo método que filtra las que ya están abiertas
                var resultado = CajasNegocios.GetTiposCajasDisponibles();

                if (!resultado.Success || resultado.Data == null || resultado.Data.Count == 0)
                {
                    Mensajes.MensajeAdvertencia("Todas las cajas posibles ya se encuentran abiertas en este momento.");
                    _tiposCajas = new List<TiposCajas>();
                    btnAbrir.Enabled = false; // Bloqueamos la apertura para evitar errores
                }
                else
                {
                    _tiposCajas = resultado.Data;
                    btnAbrir.Enabled = true;
                }

                // El DisplayMember y ValueMember ya están en tu Designer
                bindingTipoCajas.DataSource = _tiposCajas;
                cbTipo.DataSource = bindingTipoCajas;

                if (_tiposCajas.Count > 0)
                    cbTipo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al cargar tipos de caja en FrmAbrirCaja: {ex.ToString()}");
            }
        }

        private void CargarEmpleados()
        {
            try
            {
                // False para NO incluir los anulados
                var resultado = EmpleadosNegocio.GetEmpleados(false);

                _empleados = resultado.Success && resultado.Data != null
                             ? resultado.Data
                             : new List<Empleados>();

                // El DisplayMember y ValueMember ya están en el Designer
                bindingEmpleados.DataSource = _empleados;
                cbResponsable.DataSource = bindingEmpleados;

                // Puede quedar sin responsable por ahora
                cbResponsable.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al cargar empleados en FrmAbrirCaja: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Evento KeyPress para asegurar que solo se ingresen números y un separador decimal.
        /// </summary>
        private void TxtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Usamos el método EsDigitoDecimal que ya tenías en tu clase Validaciones.cs
            if (!Validaciones.EsDigitoDecimal(e.KeyChar))
            {
                e.Handled = true;
            }

            // Evitamos que pongan más de un punto o coma
            if ((e.KeyChar == ',' || e.KeyChar == '.') &&
                (txtMonto.Text.Contains(",") || txtMonto.Text.Contains(".")))
            {
                e.Handled = true;
            }
        }

        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarYArmarCaja(out Cajas nuevaCaja)) return;

                // Ahora la capa de negocio nos devuelve el Resultado<Cajas>
                var resultado = CajasNegocios.RegistrarApertura(nuevaCaja);

                if (resultado.Success && resultado.Data != null)
                {
                    // Tienes acceso al objeto Cajas completo (resultado.Data), 
                    // aunque no hace falta mandarlo atrás, está disponible si lo necesitas.
                    Mensajes.MensajeExito("La caja se abrió correctamente. Ya puede comenzar a operar.");

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    Mensajes.MensajeError(resultado.Mensaje);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al intentar abrir la caja: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado al procesar la apertura de caja.");
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            if (!Mensajes.ConfirmarCierre())
                return;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidarYArmarCaja(out Cajas caja)
        {
            caja = new Cajas();
            string msjError = string.Empty;

            try
            {
                if (!Validaciones.EsNumeroDecimal(txtMonto.Text, ref msjError))
                {
                    Mensajes.MensajeAdvertencia("Ingrese un monto inicial válido.\n" + msjError);
                    txtMonto.Focus();
                    return false;
                }

                if (cbTipo.SelectedItem == null || !(cbTipo.SelectedItem is TiposCajas tipoSeleccionado))
                {
                    Mensajes.MensajeAdvertencia("Debe seleccionar un tipo de caja válido.");
                    cbTipo.Focus();
                    return false;
                }

                caja.TotalEfectivo = decimal.Parse(txtMonto.Text);
                caja.TotalMP = 0;
                caja.Abierta = true;
                caja.Fecha = DateTime.Now;
                caja.IdTipo = tipoSeleccionado.IdTipo;

                if (cbResponsable.SelectedIndex != -1 && cbResponsable.SelectedItem is Empleados empleadoSel)
                {
                    caja.IdEmpleado = empleadoSel.IdEmpleado;
                }
                else
                {
                    caja.IdEmpleado = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al validar y armar la caja en FrmAbrirCaja: {ex.ToString()}");
                Mensajes.MensajeError("Ocurrió un error inesperado al validar los datos de la caja.");
                return false;
            }
        }

        private void CbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando) return; // Evitamos que se ejecute al cerrar el formulario
            if (cbTipo.SelectedIndex < 0 || cbResponsable.SelectedIndex < 0)
            {
                btnAbrir.Enabled = false;
                txtMonto.Enabled = false;
            }
            else
            {
                btnAbrir.Enabled = true;
                txtMonto.Enabled = true;
            }
        }

    }
}
