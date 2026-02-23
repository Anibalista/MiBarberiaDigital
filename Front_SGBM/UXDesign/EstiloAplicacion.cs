using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Front_SGBM.UXDesign
{
    public static class EstiloAplicacion
    {
        // Obtener temas disponibles
        public static List<string> ObtenerTemas()
        {
            List<string> Temas = new List<string>();
            foreach (string nombre in Enum.GetNames(typeof(Tema)))
            {
                Temas.Add(nombre);
            }
            return Temas;
        }

        // Tema actual
        public static Tema CurrentTemas
        {
            get
            {
                //Busco la propiedad en la configuración del sistema
                var name = Properties.Settings.Default.NombreTema;
                if (Enum.TryParse<Tema>(name, true, out var tema))
                    return tema;
                return Tema.Original;
            }
        }

        // ==========================================
        // PROPIEDADES DE COLOR (Mapeadas a los temas)
        // ==========================================
        public static Color ColorPrincipal => ObtenerColorPrincipal();          // Usado para acciones principales y Headers de Grillas
        public static Color ColorSecundario => ObtenerColorSecundario();             // Usado para bordes o detalles secundarios
        public static Color ColorFondoContenido => ObtenerFondoContenido();     // Fondo del contenido (pnlContent / Formularios Hijos)
        public static Color ColorFondoControles => ObtenerFondoControles();     // Fondo de controles contenedores (Grillas, Cards)
        public static Color TextoGeneral => ObtenerTextoGeneral();              // Texto general
        public static Color TextoSecundario => ObtenerTextoSecundario();        // Texto deshabilitado o secundario
        public static Color ColorEncabezado => ObtenerColorEncabezado();        // Fondo del pnlHeader
        public static Color ColorTextoEncabezado => ObtenerTextoEncabezado();   // Texto del pnlHeader
        public static Color ColorMenuOpciones => ObtenerColorMenuOpciones();    // Fondo del flpAside

        // NUEVAS PROPIEDADES NECESARIAS
        public static Color HoverColor => ObtenerColorHover();                  // Color al pasar el mouse por botones del menú
        public static Color ColorSalir => ObtenerColorSalir();                  // Color para el botón Salir/Eliminar
                                                                                // Colores temáticos
        public static Color ColorAzulado = Color.FromArgb(70, 130, 180); // SteelBlue
        public static Color ColorVerde = Color.FromArgb(34, 139, 34);  // ForestGreen

        // ==========================================
        // MÉTODOS DE OBTENCIÓN DE COLOR (Getters)
        // ==========================================

        private static Color ObtenerColorMenuOpciones()
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.FromArgb(52, 73, 94);   // Azul acero oscuro
                case Tema.Dark: return Color.FromArgb(37, 37, 38);   // Gris casi negro
                case Tema.Vintage: return Color.FromArgb(192, 192, 192); // Plata clásico
                case Tema.Suave: return Color.FromArgb(179, 157, 219); // Lavanda
                default: return SystemColors.ControlDark;
            }
        }

        private static Color ObtenerColorEncabezado()
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.FromArgb(44, 62, 80);   // Azul noche
                case Tema.Dark: return Color.FromArgb(30, 30, 30);   // Gris VS Code
                case Tema.Vintage: return Color.FromArgb(0, 0, 128);     // Azul Navy Win95
                case Tema.Suave: return Color.FromArgb(159, 168, 218); // Índigo suave
                default: return SystemColors.ControlDark;
            }
        }

        private static Color ObtenerTextoEncabezado()
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.White;
                case Tema.Dark: return Color.WhiteSmoke;
                case Tema.Vintage: return Color.White;
                case Tema.Suave: return Color.FromArgb(74, 74, 74); // Gris oscuro
                default: return Color.White;
            }
        }

        private static Color ObtenerColorPrincipal() // Color de Acento / Hover / Botones Principales
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.FromArgb(26, 188, 156);  // Turquesa
                case Tema.Dark: return Color.FromArgb(62, 62, 66);    // Gris claro (Highlight)
                case Tema.Vintage: return Color.FromArgb(0, 0, 128);     // Azul Navy (Reutilizado para coherencia)
                case Tema.Suave: return Color.FromArgb(206, 147, 216); // Magenta Pastel
                default: return SystemColors.Highlight;
            }
        }

        private static Color ObtenerColorSecundario()
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.LightGray;
                case Tema.Dark: return Color.FromArgb(45, 45, 48);
                case Tema.Vintage: return Color.Gray;
                case Tema.Suave: return Color.White;
                default: return SystemColors.ControlDark;
            }
        }

        private static Color ObtenerFondoContenido() // El fondo donde van los formularios hijos
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.FromArgb(236, 240, 241); // Gris nube
                case Tema.Dark: return Color.FromArgb(45, 45, 48);    // Gris medio oscuro
                case Tema.Vintage: return Color.White;                   // Blanco clásico
                case Tema.Suave: return Color.FromArgb(255, 253, 231); // Crema/Amarillo muy pálido
                default: return SystemColors.Window;
            }
        }

        private static Color ObtenerFondoControles() // Para tarjetas o fondos de grillas
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.White;
                case Tema.Dark: return Color.FromArgb(30, 30, 30);    // Un poco más oscuro que el fondo
                case Tema.Vintage: return Color.White;
                case Tema.Suave: return Color.White;
                default: return SystemColors.ControlLight;
            }
        }

        private static Color ObtenerTextoGeneral()
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.FromArgb(44, 62, 80);    // Mismo que header
                case Tema.Dark: return Color.FromArgb(241, 241, 241); // Blanco hueso
                case Tema.Vintage: return Color.Black;
                case Tema.Suave: return Color.FromArgb(74, 74, 74);    // Gris suave
                default: return Color.Black;
            }
        }

        private static Color ObtenerTextoSecundario()
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.Gray;
                case Tema.Dark: return Color.Gray;
                case Tema.Vintage: return Color.DimGray;
                case Tema.Suave: return Color.DarkGray;
                default: return Color.DimGray;
            }
        }

        // Nuevos métodos para Hover y Danger
        private static Color ObtenerColorHover()
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.FromArgb(26, 188, 156);  // Turquesa
                case Tema.Dark: return Color.FromArgb(62, 62, 66);    // Gris highlight
                case Tema.Vintage: return Color.FromArgb(223, 223, 223); // Gris claro 3D
                case Tema.Suave: return Color.FromArgb(206, 147, 216); // Magenta pastel
                default: return Color.LightBlue;
            }
        }

        private static Color ObtenerColorSalir()
        {
            switch (CurrentTemas)
            {
                case Tema.Original: return Color.FromArgb(231, 76, 60);   // Rojo suave
                case Tema.Dark: return Color.FromArgb(211, 47, 47);   // Rojo oscuro
                case Tema.Vintage: return Color.FromArgb(128, 0, 0);     // Marrón rojizo
                case Tema.Suave: return Color.FromArgb(239, 154, 154); // Rojo pastel
                default: return Color.Red;
            }
        }

        // ==========================================
        // CONFIGURACIÓN DE TIPOGRAFÍA
        // ==========================================
        public static string FontFamily => "Segoe UI";
        public static float FontSize => 10f;
        public static int Spacing => 8;

        // ==========================================
        // NUEVOS MÉTODOS DE ADAPTACIÓN AL LAYOUT
        // ==========================================

        /// <summary>
        /// Aplica estilo al MenuStrip superior (haciéndolo transparente/plano).
        /// </summary>
        public static void EstiloMenuStrip(MenuStrip menu)
        {
            menu.BackColor = Color.Transparent;
            menu.ForeColor = TextoGeneral;
            menu.Font = new Font(FontFamily, 9f, FontStyle.Regular);
            menu.Renderer = new MiRenderizadorMenu(); // Render personalizado

            // Items individuales
            foreach (ToolStripItem item in menu.Items)
            {
                item.ForeColor = TextoGeneral;
                if (item is ToolStripComboBox)
                {
                    item.Font = new Font(FontFamily, FontSize);
                    item.ForeColor = TextoGeneral;
                    item.BackColor = ColorFondoControles;

                }
            }
        }

        /// <summary>
        /// Aplica estilo al panel intermedio de botones (pnlTitulo en tu imagen).
        /// </summary>
        public static void EstiloTitulo(Panel panelMenu)
        {
            // El fondo de la barra donde están los botones OP1, OP2
            panelMenu.BackColor = ColorEncabezado;
            panelMenu.Padding = new Padding(5); // Un poco de aire
            foreach(Control c in panelMenu.Controls)
            {
                if (c is Label)
                {
                    if (c.Name == "lblTitulo")
                    {
                        c.Font = new Font(FontFamily, FontSize * 1.5f, FontStyle.Bold); // Ajusté un poco el tamaño
                    }
                    c.ForeColor = ColorTextoEncabezado;
                }
            }
        }

        /// <summary>
        /// Configura un botón para que despliegue un ContextMenuStrip estilizado (Simulando el Combo).
        /// </summary>
        /// <param name="botonPadre">El botón Opcion</param>
        /// <param name="menuDesplegable">El ContextMenuStrip con subopcion 1, subopcion 2...</param>
        public static void ConfigurarDropdown(Button botonPadre, ContextMenuStrip menuDesplegable)
        {
            // Estilar el menú
            menuDesplegable.Renderer = new MiRenderizadorMenu();
            menuDesplegable.Font = new Font(FontFamily, FontSize);
            menuDesplegable.ShowImageMargin = false; // Estilo plano sin margen de iconos

            // Asignar evento al botón para abrir el menú
            botonPadre.Click += (s, e) =>
            {
                // Mostrar justo debajo del botón
                menuDesplegable.Show(botonPadre, new Point(0, botonPadre.Height));
            };

            // Opcional: Agregar flechita al texto del botón
            if (!botonPadre.Text.EndsWith(" ▼"))
                botonPadre.Text += " ▼";
        }

        // Eventos hover para la barra horizontal
        private static void BtnBarra_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = HoverColor;
                // Si quieres un borde inferior estilo "Material Design":
                btn.FlatAppearance.BorderColor = ColorPrincipal;
                btn.FlatAppearance.BorderSize = 0; // O 2 para ver una linea abajo
            }
        }

        private static void BtnBarra_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.Transparent; // Vuelve a transparente/fondo
            }
        }

        // ===============================
        // CLASE RENDERIZADORA PARA MENÚS
        // ===============================
        // Esta clase se encarga de pintar los menús (MenuStrip y ContextMenu)
        // con los colores de la clase estática en lugar del gris de Windows.
        private class MiRenderizadorMenu : ToolStripProfessionalRenderer
        {
            public MiRenderizadorMenu() : base(new MisColoresMenu()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected)
                {
                    // Fondo al pasar el mouse por una opción del menú
                    Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                    e.Graphics.FillRectangle(new SolidBrush(EstiloAplicacion.HoverColor), rc);
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
        }

        // Tabla de colores para el Renderizador
        private class MisColoresMenu : ProfessionalColorTable
        {
            // Fondo del desplegable
            public override Color ToolStripDropDownBackground => EstiloAplicacion.ColorFondoControles;

            // Borde del desplegable
            public override Color MenuBorder => EstiloAplicacion.ColorPrincipal;

            // Fondo de un item seleccionado
            public override Color MenuItemSelected => EstiloAplicacion.HoverColor;

            // Borde de un item seleccionado
            public override Color MenuItemBorder => EstiloAplicacion.HoverColor;

            // Color de la barra superior (MenuStrip)
            public override Color MenuStripGradientBegin => Color.Transparent;
            public override Color MenuStripGradientEnd => Color.Transparent;

        }

        // ==========================================
        // MÉTODOS DE APLICACIÓN (APPLY)
        // ==========================================

        // Aplica tema al formulario principal
        public static void AplicarEstilo(Form form)
        {
            form.BackColor = ColorFondoContenido;
            form.Font = new Font(FontFamily, FontSize);
            ApplyTemas(form);

            // Buscar controles específicos por nombre
            var pnlHeader = form.Controls["pnlHeader"] as Panel; // Titulo y Subtitulo
            var menuStrip = form.MainMenuStrip;

            if (pnlHeader != null) EstiloEncabezado(pnlHeader);
            if (menuStrip != null) EstiloMenuStrip(menuStrip);
        }

        // Aplica tema al header
        public static void EstiloEncabezado(Panel header)
        {
            header.BackColor = ColorEncabezado;
            foreach (Control c in header.Controls)
            {
                if (c is Label lbl)
                {
                    if (lbl.Name == "lblTitulo")
                    {
                        lbl.Font = new Font(FontFamily, FontSize * 1.5f, FontStyle.Bold); // Ajusté un poco el tamaño
                    }
                    lbl.ForeColor = ColorTextoEncabezado;
                }
            }

            var pnlTitulo = header.Controls["pnlTitulo"] as Panel; // Barra de Botones
            var flpMenu = header.Controls["flpMenu"] as FlowLayoutPanel; //menu de botones
            if (pnlTitulo != null) EstiloTitulo(pnlTitulo);
            if (flpMenu != null) EstiloMenu(flpMenu);
        }

        // Aplica tema al menu (Menú en Botones)
        public static void EstiloMenu(FlowLayoutPanel menu)
        {
            menu.BackColor = ColorMenuOpciones;
            menu.Padding = new Padding(5);

            foreach (Control c in menu.Controls)
            {
                if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = new Font(FontFamily, FontSize, FontStyle.Regular);
                    btn.TextAlign = ContentAlignment.MiddleLeft; // Alinear texto para que parezca menú

                    // Configuración Base
                    btn.BackColor = ColorMenuOpciones;

                    btn.Margin = new Padding(2,2,2,0);

                    // Ajuste de color de texto según el tema
                    if (CurrentTemas == Tema.Vintage)
                        btn.ForeColor = Color.Black;
                    else
                        btn.ForeColor = Color.White; // Casi todos los temas oscuros usan texto claro en aside

                    // CASO ESPECIAL: Botón Salir en el Aside (si está ahí)
                    if (btn.Name == "btnSalir")
                    {
                        btn.BackColor = ColorSalir;
                        btn.ForeColor = Color.White;
                    }
                    else
                    {
                        // Limpiamos eventos previos para no acumularlos si se llama varias veces
                        // Nota: Esto es simplificado. Lo ideal es no reasignar eventos constantemente.
                        btn.MouseEnter -= Btn_MouseEnter;
                        btn.MouseLeave -= Btn_MouseLeave;

                        btn.MouseEnter += Btn_MouseEnter;
                        btn.MouseLeave += Btn_MouseLeave;
                    }
                }
            }
        }

        // Eventos para efecto Hover
        private static void Btn_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button btn)
                btn.BackColor = HoverColor;
        }

        private static void Btn_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button btn)
                btn.BackColor = ColorMenuOpciones; // Vuelve al color del contenedor
        }

        // Aplica tema a controles hijos recursivamente
        public static void ApplyTemas(Control root)
        {
            if (root is Form frm)
            {
                frm.BackColor = ColorFondoContenido;
                frm.Font = new Font(FontFamily, FontSize);
            }
            else
            {
                // Paneles internos transparentes para heredar color de fondo
                // A menos que sean Cards especificas
                root.BackColor = Color.Transparent;
                root.Font = new Font(FontFamily, FontSize);
            }

            foreach (Control c in root.Controls)
            {
                if (c is Label lbl)
                {
                    lbl.ForeColor = TextoGeneral;
                }
                else if (c is Button btn)
                {
                    StyleButton(btn);
                }
                else if (c is Panel pnl)
                {
                    pnl.BackColor = Color.Transparent;
                    ApplyTemas(pnl);
                }
                else if (c is FlowLayoutPanel flp)
                {
                    flp.BackColor = Color.Transparent;
                    ApplyTemas(flp);
                }
                else if (c is GroupBox gb)
                {
                    gb.ForeColor = TextoGeneral; // Color del título del GroupBox
                    gb.BackColor = Color.Transparent;
                    ApplyTemas(gb);
                }
                else if (c is DataGridView dgv)
                {
                    StyleDataGrid(dgv);
                }
                else if (c is TextBox || c is ComboBox)
                {
                    c.Font = new Font(FontFamily, FontSize);
                    c.ForeColor = TextoGeneral;
                    c.BackColor = ColorFondoControles;
                }
            }
        }

        // Estilo específico para botones generales (CRUD)
        public static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.TextAlign = ContentAlignment.MiddleCenter; // centrado

            string tag = btn.Tag?.ToString()?.ToLower() ?? "";

            switch (tag)
            {
                case "btnprincipal":
                    btn.Font = new Font(FontFamily, FontSize * 2, FontStyle.Bold);
                    btn.BackColor = ColorPrincipal;
                    btn.ForeColor = Color.White;
                    if (btn.Text != "+" && btn.Text != "-")
                        btn.MinimumSize = new Size(150, 44);
                    break;

                case "btnprincipalv":
                    btn.Font = new Font(FontFamily, FontSize * 2, FontStyle.Bold);
                    btn.BackColor = (CurrentTemas == Tema.Dark) ? ColorAzulado : ColorVerde;
                    btn.ForeColor = Color.White;
                    btn.MinimumSize = new Size(150, 44);
                    break;

                case "btnprincipalr":
                    btn.Font = new Font(FontFamily, FontSize * 2, FontStyle.Bold);
                    btn.BackColor = ColorSalir;
                    btn.ForeColor = Color.White;
                    btn.MinimumSize = new Size(150, 44);
                    break;

                case "btnsecundario":
                    btn.Font = new Font(FontFamily, (float)(FontSize * 1.5), FontStyle.Bold);
                    btn.BackColor = ColorFondoControles;
                    btn.ForeColor = TextoGeneral;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = TextoSecundario;
                    btn.MinimumSize = new Size(140, 35);
                    break;

                case "btnsecundariov":
                    btn.Font = new Font(FontFamily, (float)(FontSize * 1.5), FontStyle.Bold);
                    btn.BackColor = (CurrentTemas == Tema.Dark) ? ColorAzulado : ColorVerde;
                    btn.ForeColor = Color.White;
                    btn.MinimumSize = new Size(140, 35);
                    break;

                case "btnsecundarior":
                    btn.Font = new Font(FontFamily, (float)(FontSize * 1.5), FontStyle.Bold);
                    btn.BackColor = ColorSalir;
                    btn.ForeColor = Color.White;
                    btn.MinimumSize = new Size(140, 35);
                    break;

                case "btnnormalr":
                    btn.Font = new Font(FontFamily, FontSize, FontStyle.Regular);
                    btn.BackColor = ColorSalir;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = TextoSecundario;
                    btn.MinimumSize = new Size(100, 25);
                    break;

                case "btnnormalv":
                    btn.Font = new Font(FontFamily, FontSize, FontStyle.Regular);
                    btn.BackColor = (CurrentTemas == Tema.Dark) ? ColorAzulado : ColorVerde;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = TextoSecundario;
                    btn.MinimumSize = new Size(100, 25);
                    break;

                default: // Botones normales
                    btn.Font = new Font(FontFamily, FontSize, FontStyle.Regular);
                    btn.BackColor = ColorFondoControles;
                    btn.ForeColor = TextoGeneral;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = TextoSecundario;
                    btn.MinimumSize = new Size(100, 25);
                    break;
            }

            // Ajustar altura según fuente
            btn.Height = btn.Font.Height + 12; // padding configurable

            // Hover genérico
            btn.FlatAppearance.MouseOverBackColor = HoverColor;
        }

        // Estilo específico para DataGridView
        public static void StyleDataGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = ColorFondoControles;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = true;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Estilo de encabezados
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorPrincipal; // Header de tabla usa Primary
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = (CurrentTemas == Tema.Suave || CurrentTemas == Tema.Vintage) ? Color.Black : Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(FontFamily, FontSize, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(Spacing);

            // Estilo de filas
            dgv.DefaultCellStyle.BackColor = ColorFondoControles;
            dgv.DefaultCellStyle.ForeColor = TextoGeneral;
            dgv.DefaultCellStyle.Font = new Font(FontFamily, FontSize, FontStyle.Regular);

            // Selección
            dgv.DefaultCellStyle.SelectionBackColor = HoverColor; // Color de selección
            dgv.DefaultCellStyle.SelectionForeColor = (CurrentTemas == Tema.Suave) ? Color.Black : Color.White;

            // Filas alternadas (Zebra)
            // Hacemos el color alterno un poquito más oscuro que la superficie
            dgv.AlternatingRowsDefaultCellStyle.BackColor = ControlPaint.Dark(ColorFondoControles, 0.05f);

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

        }

        // Formatos de monedas, porcentajes, etc
        public static void ApplyFormats(DataGridView dgv, Dictionary<string, string> formats)
        {
            foreach (var kvp in formats)
            {
                if (dgv.Columns.Contains(kvp.Key))
                {
                    dgv.Columns[kvp.Key].DefaultCellStyle.Format = kvp.Value;
                    dgv.Columns[kvp.Key].DefaultCellStyle.FormatProvider = new CultureInfo("es-AR");
                }
            }
        }

        public static void ApplyCurrencyFormat(DataGridView dgv, string columnName)
        {
            if (dgv.Columns[columnName] != null)
            {
                dgv.Columns[columnName].DefaultCellStyle.Format = "C2";
                dgv.Columns[columnName].DefaultCellStyle.FormatProvider = new CultureInfo("es-AR");
            }
        }

        // ==========================================
        // GESTIÓN DE SUBMENÚS DINÁMICOS (DROPDOWNS)
        // ==========================================

        // Variable para rastrear el panel abierto actualmente
        private static FlowLayoutPanel _panelSubMenuActual;
        private static Button _botonPadreActual;

        /// <summary>
        /// Estructura simple para definir una opción del submenú
        /// </summary>
        public class OpcionMenu
        {
            public string Texto { get; set; }
            public EventHandler Accion { get; set; }

            public OpcionMenu(string texto, EventHandler accion)
            {
                Texto = texto;
                Accion = accion;
            }
        }

        /// <summary>
        /// Muestra u oculta un panel con subopciones debajo del botón presionado.
        /// </summary>
        /// <param name="formulario">El Formulario principal (this)</param>
        /// <param name="botonPadre">El botón del menú principal que se presionó</param>
        /// <param name="opciones">Lista de opciones (Texto y Evento)</param>
        public static void ToggleSubMenu(Form formulario, Button botonPadre, List<OpcionMenu> opciones)
        {
            // 1. Si ya hay un menú abierto del MISMO botón, lo cerramos (Toggle)
            if (_panelSubMenuActual != null && _botonPadreActual == botonPadre)
            {
                CerrarSubMenu();
                return;
            }

            // 2. Si hay un menú de OTRO botón, lo cerramos primero
            if (_panelSubMenuActual != null)
            {
                CerrarSubMenu();
            }

            // 3. Crear el nuevo Panel
            _botonPadreActual = botonPadre;
            _panelSubMenuActual = new FlowLayoutPanel
            {
                Name = "pnlSubMenuDinamico",
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = ColorPrincipal, // El fondo del desplegable (según tu imagen parece azul/primary)
                Padding = new Padding(0),
                Margin = new Padding(0),
                BorderStyle = BorderStyle.None,
                MaximumSize = new Size(200, 0), // Ancho máximo
                MinimumSize = new Size(botonPadre.Width, 0) // Mínimo el ancho del botón padre
            };

            // 4. Calcular Posición (Truco para que aparezca justo abajo aunque el botón esté en otro panel)
            Point coordenadasBoton = botonPadre.PointToScreen(Point.Empty);
            Point coordenadasForm = formulario.PointToClient(coordenadasBoton);

            _panelSubMenuActual.Location = new Point(coordenadasForm.X, coordenadasForm.Y + botonPadre.Height);

            // 5. Crear Botones para cada opción
            foreach (var op in opciones)
            {
                Button btnItem = new Button
                {
                    Text = op.Texto,
                    Width = _panelSubMenuActual.MinimumSize.Width, // Ocupar todo el ancho
                    Height = 35, // Altura fija cómoda
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 0, 0, 0),
                    BackColor = ColorPrincipal, // Fondo igual al panel
                    ForeColor = (CurrentTemas == Tema.Suave || CurrentTemas == Tema.Vintage) ? Color.Black : Color.White,
                    Font = new Font(FontFamily, FontSize - 1, FontStyle.Regular),
                    Cursor = Cursors.Hand
                };

                btnItem.FlatAppearance.BorderSize = 0;
                // Efecto Hover más claro/oscuro
                btnItem.FlatAppearance.MouseOverBackColor = HoverColor;

                // Asignar evento click
                btnItem.Click += (s, e) =>
                {
                    op.Accion?.Invoke(s, e); // Ejecutar la acción
                    CerrarSubMenu(); // Cerrar menú al seleccionar
                };

                _panelSubMenuActual.Controls.Add(btnItem);
            }

            // 6. Agregar al formulario y traer al frente
            formulario.Controls.Add(_panelSubMenuActual);
            _panelSubMenuActual.BringToFront();
        }

        public static void CerrarSubMenu()
        {
            if (_panelSubMenuActual != null)
            {
                _panelSubMenuActual.Dispose(); // Elimina el control de memoria
                _panelSubMenuActual = null;
                _botonPadreActual = null;
            }
        }
    }
}
