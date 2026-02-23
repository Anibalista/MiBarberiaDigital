
namespace Front_SGBM
{
    partial class FrmMenuPrincipal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip = new MenuStrip();
            administracionToolMenuItem = new ToolStripMenuItem();
            clientesToolStripMenuItem = new ToolStripMenuItem();
            abmClientesToolStripMenuItem = new ToolStripMenuItem();
            nuevoClienteToolStripMenuItem = new ToolStripMenuItem();
            serviciosToolStripMenuItem = new ToolStripMenuItem();
            aBMServiciosToolStripMenuItem = new ToolStripMenuItem();
            nuevoServicioToolStripMenuItem = new ToolStripMenuItem();
            productosToolStripMenuItem = new ToolStripMenuItem();
            aBMProductosToolStripMenuItem = new ToolStripMenuItem();
            colaboradoresToolStripMenuItem = new ToolStripMenuItem();
            nuevoModificarToolStripMenuItem = new ToolStripMenuItem();
            contabilidadToolStripMenuItem = new ToolStripMenuItem();
            reporteríaToolStripMenuItem = new ToolStripMenuItem();
            ventasToolStripMenuItem = new ToolStripMenuItem();
            comprasToolStripMenuItem = new ToolStripMenuItem();
            comboTemas = new ToolStripComboBox();
            statusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolTip = new ToolTip(components);
            pnlHeader = new Panel();
            flpMenu = new FlowLayoutPanel();
            btnVentas = new Button();
            btnCajas = new Button();
            btnServicios = new Button();
            btnProductos = new Button();
            btnReportes = new Button();
            btnClientes = new Button();
            btnContabilidad = new Button();
            btnSalir = new Button();
            pnlTitulo = new Panel();
            lblSubTitulo = new Label();
            lblTitulo = new Label();
            pnlContent = new Panel();
            menuStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            pnlHeader.SuspendLayout();
            flpMenu.SuspendLayout();
            pnlTitulo.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.BackColor = Color.Transparent;
            menuStrip.Items.AddRange(new ToolStripItem[] { administracionToolMenuItem, contabilidadToolStripMenuItem, reporteríaToolStripMenuItem, ventasToolStripMenuItem, comprasToolStripMenuItem, comboTemas });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(7, 2, 0, 2);
            menuStrip.Size = new Size(1165, 27);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "MenuStrip";
            // 
            // administracionToolMenuItem
            // 
            administracionToolMenuItem.DropDownItems.AddRange(new ToolStripItem[] { clientesToolStripMenuItem, serviciosToolStripMenuItem, productosToolStripMenuItem, colaboradoresToolStripMenuItem });
            administracionToolMenuItem.Name = "administracionToolMenuItem";
            administracionToolMenuItem.Size = new Size(100, 23);
            administracionToolMenuItem.Text = "Administracion";
            // 
            // clientesToolStripMenuItem
            // 
            clientesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { abmClientesToolStripMenuItem, nuevoClienteToolStripMenuItem });
            clientesToolStripMenuItem.Name = "clientesToolStripMenuItem";
            clientesToolStripMenuItem.Size = new Size(180, 22);
            clientesToolStripMenuItem.Text = "Clientes";
            // 
            // abmClientesToolStripMenuItem
            // 
            abmClientesToolStripMenuItem.Name = "abmClientesToolStripMenuItem";
            abmClientesToolStripMenuItem.Size = new Size(149, 22);
            abmClientesToolStripMenuItem.Text = "ABM Clientes";
            abmClientesToolStripMenuItem.Click += AbmClientesToolStripMenuItem_Click;
            // 
            // nuevoClienteToolStripMenuItem
            // 
            nuevoClienteToolStripMenuItem.Name = "nuevoClienteToolStripMenuItem";
            nuevoClienteToolStripMenuItem.Size = new Size(149, 22);
            nuevoClienteToolStripMenuItem.Text = "Nuevo Cliente";
            nuevoClienteToolStripMenuItem.Click += NuevoClienteToolStripMenuItem_Click;
            // 
            // serviciosToolStripMenuItem
            // 
            serviciosToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aBMServiciosToolStripMenuItem, nuevoServicioToolStripMenuItem });
            serviciosToolStripMenuItem.Name = "serviciosToolStripMenuItem";
            serviciosToolStripMenuItem.Size = new Size(180, 22);
            serviciosToolStripMenuItem.Text = "Servicios";
            // 
            // aBMServiciosToolStripMenuItem
            // 
            aBMServiciosToolStripMenuItem.Name = "aBMServiciosToolStripMenuItem";
            aBMServiciosToolStripMenuItem.Size = new Size(153, 22);
            aBMServiciosToolStripMenuItem.Text = "ABM Servicios";
            aBMServiciosToolStripMenuItem.Click += ABMServiciosToolStripMenuItem_Click;
            // 
            // nuevoServicioToolStripMenuItem
            // 
            nuevoServicioToolStripMenuItem.Name = "nuevoServicioToolStripMenuItem";
            nuevoServicioToolStripMenuItem.Size = new Size(153, 22);
            nuevoServicioToolStripMenuItem.Text = "Nuevo Servicio";
            nuevoServicioToolStripMenuItem.Click += NuevoServicioToolStripMenuItem_Click;
            // 
            // productosToolStripMenuItem
            // 
            productosToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aBMProductosToolStripMenuItem });
            productosToolStripMenuItem.Name = "productosToolStripMenuItem";
            productosToolStripMenuItem.Size = new Size(180, 22);
            productosToolStripMenuItem.Text = "Productos";
            // 
            // aBMProductosToolStripMenuItem
            // 
            aBMProductosToolStripMenuItem.Name = "aBMProductosToolStripMenuItem";
            aBMProductosToolStripMenuItem.Size = new Size(157, 22);
            aBMProductosToolStripMenuItem.Text = "ABM Productos";
            aBMProductosToolStripMenuItem.Click += ABMProductosToolStripMenuItem_Click;
            // 
            // colaboradoresToolStripMenuItem
            // 
            colaboradoresToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { nuevoModificarToolStripMenuItem });
            colaboradoresToolStripMenuItem.Name = "colaboradoresToolStripMenuItem";
            colaboradoresToolStripMenuItem.Size = new Size(180, 22);
            colaboradoresToolStripMenuItem.Text = "Colaboradores";
            // 
            // nuevoModificarToolStripMenuItem
            // 
            nuevoModificarToolStripMenuItem.Name = "nuevoModificarToolStripMenuItem";
            nuevoModificarToolStripMenuItem.Size = new Size(180, 22);
            nuevoModificarToolStripMenuItem.Text = "Nuevo-Modificar";
            nuevoModificarToolStripMenuItem.Click += nuevoModificarToolStripMenuItem_Click;
            // 
            // contabilidadToolStripMenuItem
            // 
            contabilidadToolStripMenuItem.Name = "contabilidadToolStripMenuItem";
            contabilidadToolStripMenuItem.Size = new Size(87, 23);
            contabilidadToolStripMenuItem.Text = "Contabilidad";
            // 
            // reporteríaToolStripMenuItem
            // 
            reporteríaToolStripMenuItem.Name = "reporteríaToolStripMenuItem";
            reporteríaToolStripMenuItem.Size = new Size(73, 23);
            reporteríaToolStripMenuItem.Text = "Reportería";
            // 
            // ventasToolStripMenuItem
            // 
            ventasToolStripMenuItem.Name = "ventasToolStripMenuItem";
            ventasToolStripMenuItem.Size = new Size(53, 23);
            ventasToolStripMenuItem.Text = "Ventas";
            // 
            // comprasToolStripMenuItem
            // 
            comprasToolStripMenuItem.Name = "comprasToolStripMenuItem";
            comprasToolStripMenuItem.Size = new Size(67, 23);
            comprasToolStripMenuItem.Text = "Compras";
            // 
            // comboTemas
            // 
            comboTemas.Alignment = ToolStripItemAlignment.Right;
            comboTemas.Name = "comboTemas";
            comboTemas.Size = new Size(121, 23);
            comboTemas.SelectedIndexChanged += ComboTemas_SelectedIndexChanged;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            statusStrip.Location = new Point(0, 702);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 16, 0);
            statusStrip.Size = new Size(1165, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(42, 17);
            toolStripStatusLabel.Text = "Estado";
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(flpMenu);
            pnlHeader.Controls.Add(pnlTitulo);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 27);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1165, 115);
            pnlHeader.TabIndex = 4;
            // 
            // flpMenu
            // 
            flpMenu.Controls.Add(btnVentas);
            flpMenu.Controls.Add(btnCajas);
            flpMenu.Controls.Add(btnServicios);
            flpMenu.Controls.Add(btnProductos);
            flpMenu.Controls.Add(btnReportes);
            flpMenu.Controls.Add(btnClientes);
            flpMenu.Controls.Add(btnContabilidad);
            flpMenu.Controls.Add(btnSalir);
            flpMenu.Dock = DockStyle.Bottom;
            flpMenu.Location = new Point(0, 61);
            flpMenu.Name = "flpMenu";
            flpMenu.Size = new Size(1165, 54);
            flpMenu.TabIndex = 1;
            // 
            // btnVentas
            // 
            btnVentas.Location = new Point(3, 3);
            btnVentas.Name = "btnVentas";
            btnVentas.Size = new Size(106, 34);
            btnVentas.TabIndex = 0;
            btnVentas.Text = "Ventas";
            btnVentas.UseVisualStyleBackColor = true;
            btnVentas.Click += BtnVentas_Click;
            // 
            // btnCajas
            // 
            btnCajas.Location = new Point(115, 3);
            btnCajas.Name = "btnCajas";
            btnCajas.Size = new Size(106, 34);
            btnCajas.TabIndex = 3;
            btnCajas.Text = "Cajas";
            btnCajas.UseVisualStyleBackColor = true;
            // 
            // btnServicios
            // 
            btnServicios.Location = new Point(227, 3);
            btnServicios.Name = "btnServicios";
            btnServicios.Size = new Size(106, 34);
            btnServicios.TabIndex = 4;
            btnServicios.Text = "Servicios";
            btnServicios.UseVisualStyleBackColor = true;
            btnServicios.Click += BtnServicios_Click;
            // 
            // btnProductos
            // 
            btnProductos.Location = new Point(339, 3);
            btnProductos.Name = "btnProductos";
            btnProductos.Size = new Size(106, 34);
            btnProductos.TabIndex = 5;
            btnProductos.Text = "Productos";
            btnProductos.UseVisualStyleBackColor = true;
            btnProductos.Click += BtnProductos_Click;
            // 
            // btnReportes
            // 
            btnReportes.Location = new Point(451, 3);
            btnReportes.Name = "btnReportes";
            btnReportes.Size = new Size(106, 34);
            btnReportes.TabIndex = 6;
            btnReportes.Text = "Reportes";
            btnReportes.UseVisualStyleBackColor = true;
            // 
            // btnClientes
            // 
            btnClientes.Location = new Point(563, 3);
            btnClientes.Name = "btnClientes";
            btnClientes.Size = new Size(106, 34);
            btnClientes.TabIndex = 2;
            btnClientes.Text = "Clientes";
            btnClientes.UseVisualStyleBackColor = true;
            btnClientes.Click += BtnClientes_Click;
            // 
            // btnContabilidad
            // 
            btnContabilidad.Location = new Point(675, 3);
            btnContabilidad.Name = "btnContabilidad";
            btnContabilidad.Size = new Size(106, 34);
            btnContabilidad.TabIndex = 7;
            btnContabilidad.Text = "Contabilidad";
            btnContabilidad.UseVisualStyleBackColor = true;
            // 
            // btnSalir
            // 
            btnSalir.Location = new Point(787, 3);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(106, 34);
            btnSalir.TabIndex = 8;
            btnSalir.Text = "Cerrar App";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += BtnSalir_Click;
            // 
            // pnlTitulo
            // 
            pnlTitulo.Controls.Add(lblSubTitulo);
            pnlTitulo.Controls.Add(lblTitulo);
            pnlTitulo.Dock = DockStyle.Top;
            pnlTitulo.Location = new Point(0, 0);
            pnlTitulo.Name = "pnlTitulo";
            pnlTitulo.Size = new Size(1165, 55);
            pnlTitulo.TabIndex = 0;
            // 
            // lblSubTitulo
            // 
            lblSubTitulo.Anchor = AnchorStyles.Top;
            lblSubTitulo.AutoSize = true;
            lblSubTitulo.Location = new Point(495, 31);
            lblSubTitulo.Name = "lblSubTitulo";
            lblSubTitulo.Size = new Size(98, 15);
            lblSubTitulo.TabIndex = 1;
            lblSubTitulo.Text = "Pantalla Principal";
            // 
            // lblTitulo
            // 
            lblTitulo.Anchor = AnchorStyles.Top;
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Roboto Condensed", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.Location = new Point(460, 2);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(190, 29);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Mi barbería Digital";
            // 
            // pnlContent
            // 
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 142);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(1165, 560);
            pnlContent.TabIndex = 5;
            // 
            // FrmMenuPrincipal
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1165, 724);
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MainMenuStrip = menuStrip;
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(1181, 763);
            Name = "FrmMenuPrincipal";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mi Barbería Digital";
            WindowState = FormWindowState.Maximized;
            Load += FrmMenuPrincipal_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            pnlHeader.ResumeLayout(false);
            flpMenu.ResumeLayout(false);
            pnlTitulo.ResumeLayout(false);
            pnlTitulo.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void buscarClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion


        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolTip toolTip;
        private ToolStripMenuItem administracionToolMenuItem;
        private ToolStripMenuItem clientesToolStripMenuItem;
        private ToolStripMenuItem nuevoClienteToolStripMenuItem;
        private ToolStripMenuItem abmClientesToolStripMenuItem;
        private Panel pnlHeader;
        private Panel pnlTitulo;
        private Label lblTitulo;
        private Label lblSubTitulo;
        private FlowLayoutPanel flpMenu;
        private Panel pnlContent;
        private Button btnVentas;
        private Button btnClientes;
        private Button btnCajas;
        private Button btnServicios;
        private Button btnProductos;
        private Button btnReportes;
        private Button btnContabilidad;
        private Button btnSalir;
        private ToolStripMenuItem contabilidadToolStripMenuItem;
        private ToolStripMenuItem reporteríaToolStripMenuItem;
        private ToolStripMenuItem ventasToolStripMenuItem;
        private ToolStripMenuItem comprasToolStripMenuItem;
        private ToolStripComboBox comboTemas;
        private ToolStripMenuItem serviciosToolStripMenuItem;
        private ToolStripMenuItem aBMServiciosToolStripMenuItem;
        private ToolStripMenuItem nuevoServicioToolStripMenuItem;
        private ToolStripMenuItem productosToolStripMenuItem;
        private ToolStripMenuItem aBMProductosToolStripMenuItem;
        private ToolStripMenuItem colaboradoresToolStripMenuItem;
        private ToolStripMenuItem nuevoModificarToolStripMenuItem;
    }
}



