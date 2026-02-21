namespace Front_SGBM
{
    partial class FrmEditServicios
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            groupBoxBotones = new GroupBox();
            btnSalir = new Button();
            btnGuardar = new Button();
            groupBoxCampos = new GroupBox();
            cbCategoria = new ComboBox();
            bindingSourceCategorias = new BindingSource(components);
            checkComision = new CheckBox();
            lblCategoria = new Label();
            txtMargen = new TextBox();
            lblMargen = new Label();
            btnAdminCostos = new Button();
            txtCostosServicio = new TextBox();
            lblCostosServicio = new Label();
            txtComision = new TextBox();
            lblComision = new Label();
            txtPrecio = new TextBox();
            lblPrecio = new Label();
            txtPuntaje = new TextBox();
            lblPuntaje = new Label();
            txtDuracion = new TextBox();
            lblDuracion = new Label();
            txtDescripcionServicio = new TextBox();
            label1 = new Label();
            txtServicio = new TextBox();
            lblServicio = new Label();
            lblTitulo = new Label();
            groupBoxInsumos = new GroupBox();
            btnLimpiar = new Button();
            dataGridInsumos = new DataGridView();
            txtTotalCostos = new TextBox();
            btnModificar = new Button();
            lblTotal = new Label();
            btnEliminar = new Button();
            txtUnidades = new TextBox();
            lblUnidades = new Label();
            btnNuevoInsumo = new Button();
            txtCantidad = new TextBox();
            txtMontoInsumo = new TextBox();
            lblCantidad = new Label();
            txtDescripcionInsumo = new TextBox();
            lblMontoCosto = new Label();
            lblDescripcionInsumo = new Label();
            cbProductos = new ComboBox();
            bindingSourceProductos = new BindingSource(components);
            lblSelectProducto = new Label();
            errorProvider1 = new ErrorProvider(components);
            checkActivo = new CheckBox();
            groupBoxBotones.SuspendLayout();
            groupBoxCampos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCategorias).BeginInit();
            groupBoxInsumos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridInsumos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceProductos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // groupBoxBotones
            // 
            groupBoxBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxBotones.Controls.Add(btnSalir);
            groupBoxBotones.Controls.Add(btnGuardar);
            groupBoxBotones.Location = new Point(5, 474);
            groupBoxBotones.Name = "groupBoxBotones";
            groupBoxBotones.Size = new Size(539, 100);
            groupBoxBotones.TabIndex = 0;
            groupBoxBotones.TabStop = false;
            // 
            // btnSalir
            // 
            btnSalir.Location = new Point(317, 22);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(141, 44);
            btnSalir.TabIndex = 1;
            btnSalir.Tag = "btnPrincipalR";
            btnSalir.Text = "Cancelar";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += BtnSalir_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(79, 22);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(150, 44);
            btnGuardar.TabIndex = 0;
            btnGuardar.Tag = "btnPrincipalV";
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += BtnGuardar_Click;
            // 
            // groupBoxCampos
            // 
            groupBoxCampos.Controls.Add(checkActivo);
            groupBoxCampos.Controls.Add(cbCategoria);
            groupBoxCampos.Controls.Add(checkComision);
            groupBoxCampos.Controls.Add(lblCategoria);
            groupBoxCampos.Controls.Add(txtMargen);
            groupBoxCampos.Controls.Add(lblMargen);
            groupBoxCampos.Controls.Add(btnAdminCostos);
            groupBoxCampos.Controls.Add(txtCostosServicio);
            groupBoxCampos.Controls.Add(lblCostosServicio);
            groupBoxCampos.Controls.Add(txtComision);
            groupBoxCampos.Controls.Add(lblComision);
            groupBoxCampos.Controls.Add(txtPrecio);
            groupBoxCampos.Controls.Add(lblPrecio);
            groupBoxCampos.Controls.Add(txtPuntaje);
            groupBoxCampos.Controls.Add(lblPuntaje);
            groupBoxCampos.Controls.Add(txtDuracion);
            groupBoxCampos.Controls.Add(lblDuracion);
            groupBoxCampos.Controls.Add(txtDescripcionServicio);
            groupBoxCampos.Controls.Add(label1);
            groupBoxCampos.Controls.Add(txtServicio);
            groupBoxCampos.Controls.Add(lblServicio);
            groupBoxCampos.Controls.Add(lblTitulo);
            groupBoxCampos.Location = new Point(7, 0);
            groupBoxCampos.Name = "groupBoxCampos";
            groupBoxCampos.Size = new Size(537, 468);
            groupBoxCampos.TabIndex = 1;
            groupBoxCampos.TabStop = false;
            // 
            // cbCategoria
            // 
            cbCategoria.DataSource = bindingSourceCategorias;
            cbCategoria.DisplayMember = "Descripcion";
            cbCategoria.FormattingEnabled = true;
            cbCategoria.Location = new Point(112, 311);
            cbCategoria.Name = "cbCategoria";
            cbCategoria.Size = new Size(204, 23);
            cbCategoria.TabIndex = 9;
            cbCategoria.ValueMember = "IdCategoria";
            // 
            // bindingSourceCategorias
            // 
            bindingSourceCategorias.DataSource = typeof(Entidades_SGBM.Categorias);
            // 
            // checkComision
            // 
            checkComision.AutoSize = true;
            checkComision.Checked = true;
            checkComision.CheckState = CheckState.Checked;
            checkComision.Location = new Point(315, 266);
            checkComision.Name = "checkComision";
            checkComision.Size = new Size(112, 19);
            checkComision.TabIndex = 18;
            checkComision.Text = "Restar Comisión";
            checkComision.UseVisualStyleBackColor = true;
            checkComision.CheckedChanged += CheckComision_CheckedChanged;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(34, 314);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(58, 15);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría";
            // 
            // txtMargen
            // 
            txtMargen.Enabled = false;
            txtMargen.Location = new Point(112, 264);
            txtMargen.Name = "txtMargen";
            txtMargen.Size = new Size(142, 23);
            txtMargen.TabIndex = 8;
            // 
            // lblMargen
            // 
            lblMargen.AutoSize = true;
            lblMargen.Location = new Point(44, 267);
            lblMargen.Name = "lblMargen";
            lblMargen.Size = new Size(48, 15);
            lblMargen.TabIndex = 16;
            lblMargen.Text = "Margen";
            // 
            // btnAdminCostos
            // 
            btnAdminCostos.Location = new Point(370, 222);
            btnAdminCostos.Name = "btnAdminCostos";
            btnAdminCostos.Size = new Size(134, 23);
            btnAdminCostos.TabIndex = 7;
            btnAdminCostos.Tag = "btnNormal";
            btnAdminCostos.Text = "Administrar";
            btnAdminCostos.UseVisualStyleBackColor = true;
            btnAdminCostos.Click += BtnAdminCostos_Click;
            // 
            // txtCostosServicio
            // 
            txtCostosServicio.Enabled = false;
            txtCostosServicio.Location = new Point(112, 221);
            txtCostosServicio.Name = "txtCostosServicio";
            txtCostosServicio.Size = new Size(142, 23);
            txtCostosServicio.TabIndex = 6;
            // 
            // lblCostosServicio
            // 
            lblCostosServicio.AutoSize = true;
            lblCostosServicio.Location = new Point(49, 224);
            lblCostosServicio.Name = "lblCostosServicio";
            lblCostosServicio.Size = new Size(43, 15);
            lblCostosServicio.TabIndex = 13;
            lblCostosServicio.Text = "Costos";
            // 
            // txtComision
            // 
            txtComision.Location = new Point(417, 177);
            txtComision.Name = "txtComision";
            txtComision.PlaceholderText = "máx 100.00%";
            txtComision.Size = new Size(87, 23);
            txtComision.TabIndex = 5;
            txtComision.KeyPress += ValidarSoloDecimales_KeyPress;
            txtComision.Leave += TxtComision_Leave;
            // 
            // lblComision
            // 
            lblComision.AutoSize = true;
            lblComision.Location = new Point(322, 180);
            lblComision.Name = "lblComision";
            lblComision.Size = new Size(58, 15);
            lblComision.TabIndex = 11;
            lblComision.Text = "Comisión";
            // 
            // txtPrecio
            // 
            txtPrecio.Location = new Point(112, 177);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.PlaceholderText = "$ pesos";
            txtPrecio.Size = new Size(142, 23);
            txtPrecio.TabIndex = 4;
            txtPrecio.KeyPress += ValidarSoloDecimales_KeyPress;
            txtPrecio.Leave += TxtPrecio_Leave;
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.Location = new Point(20, 180);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(72, 15);
            lblPrecio.TabIndex = 9;
            lblPrecio.Text = "Precio Venta";
            // 
            // txtPuntaje
            // 
            txtPuntaje.Location = new Point(417, 134);
            txtPuntaje.Name = "txtPuntaje";
            txtPuntaje.Size = new Size(87, 23);
            txtPuntaje.TabIndex = 3;
            txtPuntaje.KeyPress += ValidarSoloNumeros_KeyPress;
            // 
            // lblPuntaje
            // 
            lblPuntaje.AutoSize = true;
            lblPuntaje.Location = new Point(330, 137);
            lblPuntaje.Name = "lblPuntaje";
            lblPuntaje.Size = new Size(47, 15);
            lblPuntaje.TabIndex = 7;
            lblPuntaje.Text = "Puntaje";
            // 
            // txtDuracion
            // 
            txtDuracion.Location = new Point(112, 134);
            txtDuracion.Name = "txtDuracion";
            txtDuracion.PlaceholderText = "(minutos)";
            txtDuracion.Size = new Size(142, 23);
            txtDuracion.TabIndex = 2;
            txtDuracion.KeyPress += ValidarSoloNumeros_KeyPress;
            // 
            // lblDuracion
            // 
            lblDuracion.AutoSize = true;
            lblDuracion.Location = new Point(37, 137);
            lblDuracion.Name = "lblDuracion";
            lblDuracion.Size = new Size(55, 15);
            lblDuracion.TabIndex = 5;
            lblDuracion.Text = "Duración";
            // 
            // txtDescripcionServicio
            // 
            txtDescripcionServicio.Location = new Point(112, 89);
            txtDescripcionServicio.Name = "txtDescripcionServicio";
            txtDescripcionServicio.Size = new Size(392, 23);
            txtDescripcionServicio.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 92);
            label1.Name = "label1";
            label1.Size = new Size(69, 15);
            label1.TabIndex = 3;
            label1.Text = "Descripción";
            // 
            // txtServicio
            // 
            txtServicio.Location = new Point(112, 44);
            txtServicio.Name = "txtServicio";
            txtServicio.Size = new Size(393, 23);
            txtServicio.TabIndex = 0;
            // 
            // lblServicio
            // 
            lblServicio.AutoSize = true;
            lblServicio.Location = new Point(46, 47);
            lblServicio.Name = "lblServicio";
            lblServicio.Size = new Size(48, 15);
            lblServicio.TabIndex = 1;
            lblServicio.Text = "Servicio";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(250, 19);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(100, 15);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Datos del Servicio";
            // 
            // groupBoxInsumos
            // 
            groupBoxInsumos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxInsumos.Controls.Add(btnLimpiar);
            groupBoxInsumos.Controls.Add(dataGridInsumos);
            groupBoxInsumos.Controls.Add(txtTotalCostos);
            groupBoxInsumos.Controls.Add(btnModificar);
            groupBoxInsumos.Controls.Add(lblTotal);
            groupBoxInsumos.Controls.Add(btnEliminar);
            groupBoxInsumos.Controls.Add(txtUnidades);
            groupBoxInsumos.Controls.Add(lblUnidades);
            groupBoxInsumos.Controls.Add(btnNuevoInsumo);
            groupBoxInsumos.Controls.Add(txtCantidad);
            groupBoxInsumos.Controls.Add(txtMontoInsumo);
            groupBoxInsumos.Controls.Add(lblCantidad);
            groupBoxInsumos.Controls.Add(txtDescripcionInsumo);
            groupBoxInsumos.Controls.Add(lblMontoCosto);
            groupBoxInsumos.Controls.Add(lblDescripcionInsumo);
            groupBoxInsumos.Controls.Add(cbProductos);
            groupBoxInsumos.Controls.Add(lblSelectProducto);
            groupBoxInsumos.Location = new Point(550, 0);
            groupBoxInsumos.Name = "groupBoxInsumos";
            groupBoxInsumos.Size = new Size(430, 588);
            groupBoxInsumos.TabIndex = 2;
            groupBoxInsumos.TabStop = false;
            groupBoxInsumos.Text = "Insumos y Gastos";
            // 
            // btnLimpiar
            // 
            btnLimpiar.Enabled = false;
            btnLimpiar.Location = new Point(227, 86);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(133, 23);
            btnLimpiar.TabIndex = 22;
            btnLimpiar.Tag = "btnNormal";
            btnLimpiar.Text = "Limpiar Campos";
            btnLimpiar.UseVisualStyleBackColor = true;
            btnLimpiar.Click += BtnLimpiar_Click;
            // 
            // dataGridInsumos
            // 
            dataGridInsumos.AllowUserToAddRows = false;
            dataGridInsumos.AllowUserToDeleteRows = false;
            dataGridInsumos.AllowUserToOrderColumns = true;
            dataGridInsumos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridInsumos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridInsumos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridInsumos.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridInsumos.Location = new Point(6, 250);
            dataGridInsumos.Name = "dataGridInsumos";
            dataGridInsumos.ReadOnly = true;
            dataGridInsumos.RowHeadersVisible = false;
            dataGridInsumos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridInsumos.Size = new Size(411, 289);
            dataGridInsumos.TabIndex = 8;
            dataGridInsumos.SelectionChanged += DataGridInsumos_SelectionChanged;
            // 
            // txtTotalCostos
            // 
            txtTotalCostos.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            txtTotalCostos.Enabled = false;
            txtTotalCostos.Location = new Point(302, 559);
            txtTotalCostos.Name = "txtTotalCostos";
            txtTotalCostos.Size = new Size(115, 23);
            txtTotalCostos.TabIndex = 9;
            // 
            // btnModificar
            // 
            btnModificar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnModificar.Enabled = false;
            btnModificar.Location = new Point(185, 222);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(108, 23);
            btnModificar.TabIndex = 6;
            btnModificar.Tag = "btnNormal";
            btnModificar.Text = "ModificarCosto";
            btnModificar.UseVisualStyleBackColor = true;
            btnModificar.Click += BtnModificar_Click;
            // 
            // lblTotal
            // 
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(208, 562);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(71, 15);
            lblTotal.TabIndex = 19;
            lblTotal.Text = "Total Costos";
            // 
            // btnEliminar
            // 
            btnEliminar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEliminar.Enabled = false;
            btnEliminar.Location = new Point(314, 221);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(103, 23);
            btnEliminar.TabIndex = 7;
            btnEliminar.Tag = "btnNormalR";
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += BtnEliminar_Click;
            // 
            // txtUnidades
            // 
            txtUnidades.Enabled = false;
            txtUnidades.Location = new Point(284, 180);
            txtUnidades.Name = "txtUnidades";
            txtUnidades.Size = new Size(76, 23);
            txtUnidades.TabIndex = 5;
            txtUnidades.KeyPress += ValidarSoloNumeros_KeyPress;
            txtUnidades.Leave += TxtUnidades_Leave;
            // 
            // lblUnidades
            // 
            lblUnidades.AutoSize = true;
            lblUnidades.Location = new Point(208, 183);
            lblUnidades.Name = "lblUnidades";
            lblUnidades.Size = new Size(56, 15);
            lblUnidades.TabIndex = 21;
            lblUnidades.Text = "Unidades";
            // 
            // btnNuevoInsumo
            // 
            btnNuevoInsumo.Enabled = false;
            btnNuevoInsumo.Location = new Point(9, 222);
            btnNuevoInsumo.Name = "btnNuevoInsumo";
            btnNuevoInsumo.Size = new Size(116, 23);
            btnNuevoInsumo.TabIndex = 2;
            btnNuevoInsumo.Tag = "btnNormalV";
            btnNuevoInsumo.Text = "Añadir";
            btnNuevoInsumo.UseVisualStyleBackColor = true;
            btnNuevoInsumo.Click += BtnNuevoInsumo_Click;
            // 
            // txtCantidad
            // 
            txtCantidad.Enabled = false;
            txtCantidad.Location = new Point(94, 180);
            txtCantidad.Name = "txtCantidad";
            txtCantidad.Size = new Size(87, 23);
            txtCantidad.TabIndex = 4;
            txtCantidad.KeyPress += ValidarSoloNumeros_KeyPress;
            txtCantidad.Leave += TxtCantidad_Leave;
            // 
            // txtMontoInsumo
            // 
            txtMontoInsumo.Enabled = false;
            txtMontoInsumo.Location = new Point(94, 87);
            txtMontoInsumo.Name = "txtMontoInsumo";
            txtMontoInsumo.PlaceholderText = "$ pesos";
            txtMontoInsumo.Size = new Size(87, 23);
            txtMontoInsumo.TabIndex = 1;
            txtMontoInsumo.KeyPress += ValidarSoloDecimales_KeyPress;
            // 
            // lblCantidad
            // 
            lblCantidad.AutoSize = true;
            lblCantidad.Location = new Point(23, 183);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(55, 15);
            lblCantidad.TabIndex = 19;
            lblCantidad.Text = "Cantidad";
            // 
            // txtDescripcionInsumo
            // 
            txtDescripcionInsumo.Enabled = false;
            txtDescripcionInsumo.Location = new Point(94, 42);
            txtDescripcionInsumo.Name = "txtDescripcionInsumo";
            txtDescripcionInsumo.Size = new Size(266, 23);
            txtDescripcionInsumo.TabIndex = 0;
            // 
            // lblMontoCosto
            // 
            lblMontoCosto.AutoSize = true;
            lblMontoCosto.Location = new Point(35, 90);
            lblMontoCosto.Name = "lblMontoCosto";
            lblMontoCosto.Size = new Size(43, 15);
            lblMontoCosto.TabIndex = 19;
            lblMontoCosto.Text = "Monto";
            // 
            // lblDescripcionInsumo
            // 
            lblDescripcionInsumo.AutoSize = true;
            lblDescripcionInsumo.Location = new Point(9, 45);
            lblDescripcionInsumo.Name = "lblDescripcionInsumo";
            lblDescripcionInsumo.Size = new Size(69, 15);
            lblDescripcionInsumo.TabIndex = 19;
            lblDescripcionInsumo.Text = "Descripción";
            // 
            // cbProductos
            // 
            cbProductos.DataSource = bindingSourceProductos;
            cbProductos.DisplayMember = "Descripcion";
            cbProductos.DropDownStyle = ComboBoxStyle.DropDownList;
            cbProductos.Enabled = false;
            cbProductos.FormattingEnabled = true;
            cbProductos.Location = new Point(94, 134);
            cbProductos.Name = "cbProductos";
            cbProductos.Size = new Size(266, 23);
            cbProductos.TabIndex = 3;
            cbProductos.ValueMember = "IdProducto";
            cbProductos.SelectedIndexChanged += CbProductos_SelectedIndexChanged;
            // 
            // bindingSourceProductos
            // 
            bindingSourceProductos.DataSource = typeof(Entidades_SGBM.Productos);
            // 
            // lblSelectProducto
            // 
            lblSelectProducto.AutoSize = true;
            lblSelectProducto.Location = new Point(22, 137);
            lblSelectProducto.Name = "lblSelectProducto";
            lblSelectProducto.Size = new Size(56, 15);
            lblSelectProducto.TabIndex = 0;
            lblSelectProducto.Text = "Producto";
            // 
            // errorProvider1
            // 
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider1.ContainerControl = this;
            // 
            // checkActivo
            // 
            checkActivo.AutoSize = true;
            checkActivo.Checked = true;
            checkActivo.CheckState = CheckState.Checked;
            checkActivo.Location = new Point(354, 313);
            checkActivo.Name = "checkActivo";
            checkActivo.Size = new Size(104, 19);
            checkActivo.TabIndex = 19;
            checkActivo.Text = "Servicio Activo";
            checkActivo.UseVisualStyleBackColor = true;
            // 
            // FrmEditServicios
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 588);
            Controls.Add(groupBoxInsumos);
            Controls.Add(groupBoxCampos);
            Controls.Add(groupBoxBotones);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(980, 588);
            Name = "FrmEditServicios";
            ShowIcon = false;
            Load += FrmEditServicios_Load;
            groupBoxBotones.ResumeLayout(false);
            groupBoxCampos.ResumeLayout(false);
            groupBoxCampos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCategorias).EndInit();
            groupBoxInsumos.ResumeLayout(false);
            groupBoxInsumos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridInsumos).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceProductos).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxBotones;
        private Button btnGuardar;
        private Button btnSalir;
        private GroupBox groupBoxCampos;
        private GroupBox groupBoxInsumos;
        private ComboBox cbProductos;
        private Label lblSelectProducto;
        private BindingSource bindingSourceProductos;
        private Label lblTitulo;
        private TextBox txtDuracion;
        private Label lblDuracion;
        private TextBox txtDescripcionServicio;
        private Label label1;
        private TextBox txtServicio;
        private Label lblServicio;
        private TextBox txtPuntaje;
        private Label lblPuntaje;
        private TextBox txtComision;
        private Label lblComision;
        private TextBox txtPrecio;
        private Label lblPrecio;
        private TextBox txtCostosServicio;
        private Label lblCostosServicio;
        private CheckBox checkComision;
        private TextBox txtMargen;
        private Label lblMargen;
        private Button btnAdminCostos;
        private ComboBox cbCategoria;
        private BindingSource bindingSourceCategorias;
        private Label lblCategoria;
        private TextBox txtDescripcionInsumo;
        private Label lblDescripcionInsumo;
        private Button btnNuevoInsumo;
        private TextBox txtMontoInsumo;
        private Label lblMontoCosto;
        private Button btnModificar;
        private Button btnEliminar;
        private TextBox txtUnidades;
        private Label lblUnidades;
        private TextBox txtCantidad;
        private Label lblCantidad;
        private DataGridView dataGridInsumos;
        private TextBox txtTotalCostos;
        private Label lblTotal;
        private Button btnLimpiar;
        private ErrorProvider errorProvider1;
        private CheckBox checkActivo;
    }
}