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
                    ConfigurarFormHijo(frm, content);       // Configuración básica
                    ConfigurarEstiloHijo(frm); // Configuración visual/tema

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
        private void abrirEditClientes(object sender, EventArgs e, EnumModoForm modo)
        {
            FrmEditClientes? abierto = Application.OpenForms.OfType<FrmEditClientes>().FirstOrDefault();

            if (abierto == null)
            {
                abierto = new FrmEditClientes();
                abierto.modo = modo;
                abierto.Show();
            }
            else
            {
                abierto.Focus();
            }

        }

        private void abrirAbmClientes(object sender, EventArgs e, EnumModoForm modo)
        {
            FrmAbmClientes? abierto = Application.OpenForms.OfType<FrmAbmClientes>().FirstOrDefault();

            if (abierto == null)
            {
                abierto = new FrmAbmClientes();
                abierto.MdiParent = this;
                //abierto.modo = modo;
                abierto.Show();
            }
            else
            {
                abierto.Focus();
            }
        }

        //Métodos genéricos
        public void abrirFrmContactos(EnumModoForm modo, string origen, List<Contactos>? contactos, Panel content)
        {
            FrmContactos frm = (FrmContactos)AbrirFrmHijo<FrmContactos>(true, content);
            frm.origen = origen;
            frm.modo = modo;
            frm._contactos = contactos;
            frm.Show();
        }

        //Opciones
        private void nuevoClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirEditClientes(sender, e, EnumModoForm.Alta);
        }

        private void abmClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirAbmClientes(sender, e, EnumModoForm.Consulta);
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {

        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            // 1. Definimos las opciones del submenú
            var opcionesClientes = new List<EstiloAplicacion.OpcionMenu>
            {
                // OPCIÓN A: Nuevo Cliente -> Abre FrmEditClientes
                new EstiloAplicacion.OpcionMenu("Nuevo Cliente", (s, args) =>
                {
                    // Usamos tu método genérico. 'false' porque no queremos cerrar instancias previas forzosamente
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

        private void comboTemas_SelectedIndexChanged(object sender, EventArgs e)
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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show("¿Desea cerrar la aplicación?\nLos cambios no guardados se perderán", "Confirmar Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta == DialogResult.No)
                return;
            cerrando = true;
            Close();
        }
    }
}
