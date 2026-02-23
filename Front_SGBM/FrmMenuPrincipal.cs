using Entidades_SGBM;
using Front_SGBM.UXDesign;

namespace Front_SGBM
{
    public partial class FrmMenuPrincipal : Form
    {
        private int childFormNumber = 0;
        bool cerrando = false;

        public FrmMenuPrincipal()
        {
            InitializeComponent();
        }

        private void FrmMenuPrincipal_Load(object sender, EventArgs e)
        {
            comboTemas.Items.AddRange(EstiloAplicacion.ObtenerTemas().ToArray());
            // Seleccionar el tema guardado
            string temaGuardado = Properties.Settings.Default.NombreTema;
            if (!string.IsNullOrEmpty(temaGuardado) && comboTemas.Items.Contains(temaGuardado))
                comboTemas.SelectedItem = temaGuardado;
            else
                comboTemas.SelectedIndex = 0;

            // 1. Aplicar estilo general
            EstiloAplicacion.AplicarEstilo(this);
        }

        // Evento opcional: Cerrar el menú si hacen clic en el fondo del formulario
        private void FrmMenuPrincipal_Click(object sender, EventArgs e)
        {
            EstiloAplicacion.CerrarSubMenu();
        }

        // Método genérico: abre o devuelve un formulario hijo
        public Form AbrirFrmHijo<T>(bool cerrar, Panel content = null) where T : Form, new()
        {
            try
            {
                Form frm = Application.OpenForms.OfType<T>().FirstOrDefault();

                if (cerrar)
                {
                    frm?.Close();
                }

                if (frm == null)
                {
                    frm = new T();
                    ConfigurarFormHijo(frm, content); // Configuración básica
                    ConfigurarEstiloHijo(frm);        // Configuración visual/tema

                }
                else
                {
                    frm.BringToFront();
                }

                return frm;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al abrir formulario hijo", ex);
            }
        }

        // Configuración básica para que el form se comporte como hijo dentro del pnlContent
        private void ConfigurarFormHijo(Form frm, Panel content)
        {
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;

            frm.Dock = DockStyle.Fill;

            if (content == null)
                content = pnlContent;
            content.Controls.Clear();
            content.Controls.Add(frm);
            content.Tag = frm; // (Opcional) Guardamos referencia en el Tag
        }

        // Configuración visual (tema, estilos, etc.)
        private void ConfigurarEstiloHijo(Form frm)
        {
            EstiloAplicacion.AplicarEstilo(frm);
        }

        //Inicio de Métodos propios
        //Accesos
        public void AbrirEditClientes(object sender, EventArgs e, EnumModoForm modo, Clientes? cliente = null)
        {
            FrmEditClientes frm = (FrmEditClientes)AbrirFrmHijo<FrmEditClientes>(true);
            frm._cliente = cliente;
            frm.modo = modo;
            frm.Show();
        }

        public void AbrirAbmClientes(object sender, EventArgs e, EnumModoForm modo)
        {
            FrmAbmClientes frm = (FrmAbmClientes)AbrirFrmHijo<FrmAbmClientes>(true);
            frm.modo = modo;
            frm.Show();
        }

        public void AbrirEditServicios(object sender, EventArgs e, EnumModoForm modo, Servicios? servicio = null)
        {
            FrmEditServicios frm = (FrmEditServicios)AbrirFrmHijo<FrmEditServicios>(true);
            frm._servicio = servicio;
            frm.modo = modo;
            frm.Show();
        }

        public void AbrirEditEmpleados(object sender, EventArgs e, EnumModoForm modo, Empleados? empleado = null)
        {
            FrmEditBarbero frm = (FrmEditBarbero)AbrirFrmHijo<FrmEditBarbero>(true);
            frm._barbero = empleado;
            frm.modo = modo;
            frm.Show();
        }

        public void AbrirAbmProductos(object sender, EventArgs e, EnumModoForm modo)
        {
            FrmAbmProductos frm = (FrmAbmProductos)AbrirFrmHijo<FrmAbmProductos>(true);
            frm.modo = modo;
            frm.Show();
        }

        public void AbrirAbmServicios(object sender, EventArgs e, EnumModoForm modo)
        {
            FrmAbmServicios frm = (FrmAbmServicios)AbrirFrmHijo<FrmAbmServicios>(true);
            frm.modo = modo;
            frm.Show();
        }

        //Métodos genéricos
        public void AbrirFrmContactos(EnumModoForm modo, string origen, List<Contactos>? contactos, Panel content)
        {
            FrmContactos frm = (FrmContactos)AbrirFrmHijo<FrmContactos>(true, content);
            frm.origen = origen;
            frm.modo = modo;
            frm._contactos = contactos;
            frm.Show();
        }

        //Opciones
        private void NuevoClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirEditClientes(sender, e, EnumModoForm.Alta);
        }

        private void AbmClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirAbmClientes(sender, e, EnumModoForm.Consulta);
        }

        private void NuevoServicioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirEditServicios(sender, e, EnumModoForm.Alta);
        }

        private void ABMServiciosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirAbmServicios(sender, e, EnumModoForm.Consulta);
        }
        private void ABMProductosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirAbmProductos(sender, e, EnumModoForm.Consulta);
        }

        private void nuevoModificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirEditEmpleados(sender, e, EnumModoForm.Alta);
        }

        private void BtnVentas_Click(object sender, EventArgs e)
        {

        }

        private void BtnClientes_Click(object sender, EventArgs e)
        {
            // 1. Defino las opciones del submenú
            var opcionesClientes = new List<EstiloAplicacion.OpcionMenu>
            {
                // OPCIÓN A: Nuevo Cliente -> Abre FrmEditClientes
                new EstiloAplicacion.OpcionMenu("Nuevo Cliente", (s, args) =>
                {
                    // Uso tu método genérico. 'false' porque no queremos cerrar instancias previas forzosamente
                    Form frm = AbrirFrmHijo<FrmEditClientes>(false);
                    frm.Show(); // Importante: Aseguramos que se muestre
                }),

                // OPCIÓN B: Gestión Clientes -> Abre FrmAbmClientes
                new EstiloAplicacion.OpcionMenu("Gestión Clientes", (s, args) =>
                {
                    Form frm = AbrirFrmHijo<FrmAbmClientes>(false);
                    frm.Show();
                })
            };

            // 2. Llamamos al Toggle para mostrar el menú debajo del botón
            EstiloAplicacion.ToggleSubMenu(this, (Button)sender, opcionesClientes);
        }

        private void ComboTemas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cerrando) return;

            try
            {
                // Guardar el tema elegido en Settings
                Properties.Settings.Default.NombreTema = comboTemas.SelectedItem.ToString();
                Properties.Settings.Default.Save();

                // Reaplicar estilo al formulario principal
                EstiloAplicacion.AplicarEstilo(this);

                // Reaplicar estilo a los formularios hijos
                foreach (Form child in pnlContent.Controls.OfType<Form>())
                {
                    foreach (Panel panel in child.Controls.OfType<Panel>())
                    {
                        if (panel.Name == "pnlContent")
                        {
                            Form child2 = panel.Controls.OfType<Form>().FirstOrDefault();
                            if (child2 != null)
                            {
                                EstiloAplicacion.AplicarEstilo(child2);
                                child2.Refresh();
                            }
                        }
                    }
                    EstiloAplicacion.AplicarEstilo(child);
                    child.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error aplicando tema: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show("¿Desea cerrar la aplicación?\nLos cambios no guardados se perderán", "Confirmar Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta == DialogResult.No)
                return;
            cerrando = true;
            Close();
        }

        private void BtnServicios_Click(object sender, EventArgs e)
        {
            var opcionesClientes = new List<EstiloAplicacion.OpcionMenu>
            {

                new EstiloAplicacion.OpcionMenu("Nuevo Servicio", (s, args) =>
                {
                    Form frm = AbrirFrmHijo<FrmEditServicios>(false);
                    frm.Show();
                }),

                new EstiloAplicacion.OpcionMenu("Gestión Servicios", (s, args) =>
                {
                    Form frm = AbrirFrmHijo<FrmAbmServicios>(true);
                    frm.Show();
                })
            };

            EstiloAplicacion.ToggleSubMenu(this, (Button)sender, opcionesClientes);
        }


        private void BtnProductos_Click(object sender, EventArgs e)
        {
            var opcionesClientes = new List<EstiloAplicacion.OpcionMenu>
            {
                /*
                new EstiloAplicacion.OpcionMenu("Nuevo Producto", (s, args) =>
                {
                    Form frm = AbrirFrmHijo<FrmEditServicios>(false);
                    frm.Show();
                }),*/

                new EstiloAplicacion.OpcionMenu("Gestión Productos", (s, args) =>
                {
                    Form frm = AbrirFrmHijo<FrmAbmProductos>(true);
                    frm.Show();
                })
            };

            EstiloAplicacion.ToggleSubMenu(this, (Button)sender, opcionesClientes);
        }
    }
}
