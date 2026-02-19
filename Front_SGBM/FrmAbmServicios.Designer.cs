namespace Front_SGBM
{
    partial class FrmAbmServicios
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
            groupBoxFiltro = new GroupBox();
            checkAnulados = new CheckBox();
            btnLimpiar = new Button();
            btnBuscar = new Button();
            cbCategorias = new ComboBox();
            bindingSourceCategorias = new BindingSource(components);
            label3 = new Label();
            labelCampo1 = new Label();
            txtBusqueda = new TextBox();
            cbCriterios = new ComboBox();
            label1 = new Label();
            cbCampos = new ComboBox();
            txtFiltroRapido = new TextBox();
            label2 = new Label();
            groupBoxFooter = new GroupBox();
            dataGridCostos = new DataGridView();
            descripcionDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            costoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            unidadesDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            cantidadMedidaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bindingSourceCostos = new BindingSource(components);
            dataGridServicios = new DataGridView();
            nombreServicioDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            descripcionDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            precioVenta = new DataGridViewTextBoxColumn();
            costos = new DataGridViewTextBoxColumn();
            margen = new DataGridViewTextBoxColumn();
            comision = new DataGridViewTextBoxColumn();
            duracionMinutosDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            puntajeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            categoriasDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bindingSourceServicios = new BindingSource(components);
            groupBoxServicios = new GroupBox();
            groupBoxBotones = new GroupBox();
            btnConsultar = new Button();
            btnSeleccionar = new Button();
            btnSalir = new Button();
            btnRegistrar = new Button();
            groupBoxBtnNuevo = new GroupBox();
            errorProvider1 = new ErrorProvider(components);
            groupBoxFiltro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCategorias).BeginInit();
            groupBoxFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridCostos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCostos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridServicios).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceServicios).BeginInit();
            groupBoxServicios.SuspendLayout();
            groupBoxBotones.SuspendLayout();
            groupBoxBtnNuevo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // groupBoxFiltro
            // 
            groupBoxFiltro.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            groupBoxFiltro.Controls.Add(checkAnulados);
            groupBoxFiltro.Controls.Add(btnLimpiar);
            groupBoxFiltro.Controls.Add(btnBuscar);
            groupBoxFiltro.Controls.Add(cbCategorias);
            groupBoxFiltro.Controls.Add(label3);
            groupBoxFiltro.Controls.Add(labelCampo1);
            groupBoxFiltro.Controls.Add(txtBusqueda);
            groupBoxFiltro.Controls.Add(cbCriterios);
            groupBoxFiltro.Controls.Add(label1);
            groupBoxFiltro.Controls.Add(cbCampos);
            groupBoxFiltro.Controls.Add(txtFiltroRapido);
            groupBoxFiltro.Controls.Add(label2);
            groupBoxFiltro.Location = new Point(768, 79);
            groupBoxFiltro.Name = "groupBoxFiltro";
            groupBoxFiltro.Size = new Size(284, 347);
            groupBoxFiltro.TabIndex = 0;
            groupBoxFiltro.TabStop = false;
            groupBoxFiltro.Text = "Filtros";
            // 
            // checkAnulados
            // 
            checkAnulados.AutoSize = true;
            checkAnulados.Location = new Point(9, 47);
            checkAnulados.Name = "checkAnulados";
            checkAnulados.Size = new Size(112, 19);
            checkAnulados.TabIndex = 14;
            checkAnulados.Text = "Incluir Anulados";
            checkAnulados.UseVisualStyleBackColor = true;
            checkAnulados.CheckedChanged += CheckAnulados_CheckedChanged;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Location = new Point(7, 295);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(264, 39);
            btnLimpiar.TabIndex = 13;
            btnLimpiar.Text = "Limpiar Búsqueda";
            btnLimpiar.UseVisualStyleBackColor = true;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(9, 243);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(264, 39);
            btnBuscar.TabIndex = 12;
            btnBuscar.Text = "Búsqueda Avanzada";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += BtnBuscar_Click;
            // 
            // cbCategorias
            // 
            cbCategorias.DataSource = bindingSourceCategorias;
            cbCategorias.DisplayMember = "Descripcion";
            cbCategorias.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCategorias.FormattingEnabled = true;
            cbCategorias.Location = new Point(92, 197);
            cbCategorias.Name = "cbCategorias";
            cbCategorias.Size = new Size(181, 23);
            cbCategorias.TabIndex = 11;
            cbCategorias.ValueMember = "IdCategoria";
            cbCategorias.SelectedIndexChanged += CbCategorias_SelectedIndexChanged;
            // 
            // bindingSourceCategorias
            // 
            bindingSourceCategorias.DataSource = typeof(Entidades_SGBM.Categorias);
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 200);
            label3.Name = "label3";
            label3.Size = new Size(64, 15);
            label3.TabIndex = 10;
            label3.Text = "Categoría: ";
            // 
            // labelCampo1
            // 
            labelCampo1.AutoSize = true;
            labelCampo1.Location = new Point(6, 162);
            labelCampo1.Name = "labelCampo1";
            labelCampo1.Size = new Size(36, 15);
            labelCampo1.TabIndex = 9;
            labelCampo1.Text = "Valor:";
            // 
            // txtBusqueda
            // 
            txtBusqueda.Location = new Point(92, 159);
            txtBusqueda.MaxLength = 149;
            txtBusqueda.Name = "txtBusqueda";
            txtBusqueda.Size = new Size(181, 23);
            txtBusqueda.TabIndex = 8;
            txtBusqueda.KeyPress += TxtBusqueda_KeyPress;
            // 
            // cbCriterios
            // 
            cbCriterios.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCriterios.FormattingEnabled = true;
            cbCriterios.Location = new Point(92, 118);
            cbCriterios.Name = "cbCriterios";
            cbCriterios.Size = new Size(181, 23);
            cbCriterios.TabIndex = 7;
            cbCriterios.SelectedIndexChanged += CbCriterios_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 121);
            label1.Name = "label1";
            label1.Size = new Size(52, 15);
            label1.TabIndex = 6;
            label1.Text = "Criterio: ";
            // 
            // cbCampos
            // 
            cbCampos.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCampos.FormattingEnabled = true;
            cbCampos.Location = new Point(92, 77);
            cbCampos.Name = "cbCampos";
            cbCampos.Size = new Size(181, 23);
            cbCampos.TabIndex = 5;
            cbCampos.SelectedIndexChanged += CbCampos_SelectedIndexChanged;
            // 
            // txtFiltroRapido
            // 
            txtFiltroRapido.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFiltroRapido.Location = new Point(6, 19);
            txtFiltroRapido.Name = "txtFiltroRapido";
            txtFiltroRapido.PlaceholderText = "Filtro Rápido Por Nombre";
            txtFiltroRapido.Size = new Size(269, 23);
            txtFiltroRapido.TabIndex = 0;
            txtFiltroRapido.TextChanged += TxtFiltroRapido_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 80);
            label2.Name = "label2";
            label2.Size = new Size(69, 15);
            label2.TabIndex = 4;
            label2.Text = "Buscar por: ";
            // 
            // groupBoxFooter
            // 
            groupBoxFooter.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxFooter.Controls.Add(dataGridCostos);
            groupBoxFooter.Location = new Point(1, 422);
            groupBoxFooter.Name = "groupBoxFooter";
            groupBoxFooter.Size = new Size(758, 222);
            groupBoxFooter.TabIndex = 1;
            groupBoxFooter.TabStop = false;
            groupBoxFooter.Text = "Costos-Insumos del servicio";
            // 
            // dataGridCostos
            // 
            dataGridCostos.AllowUserToAddRows = false;
            dataGridCostos.AllowUserToDeleteRows = false;
            dataGridCostos.AllowUserToOrderColumns = true;
            dataGridCostos.AutoGenerateColumns = false;
            dataGridCostos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridCostos.Columns.AddRange(new DataGridViewColumn[] { descripcionDataGridViewTextBoxColumn1, costoDataGridViewTextBoxColumn, unidadesDataGridViewTextBoxColumn, cantidadMedidaDataGridViewTextBoxColumn });
            dataGridCostos.DataSource = bindingSourceCostos;
            dataGridCostos.Dock = DockStyle.Fill;
            dataGridCostos.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridCostos.Location = new Point(3, 19);
            dataGridCostos.MultiSelect = false;
            dataGridCostos.Name = "dataGridCostos";
            dataGridCostos.ReadOnly = true;
            dataGridCostos.RowHeadersVisible = false;
            dataGridCostos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridCostos.Size = new Size(752, 200);
            dataGridCostos.TabIndex = 0;
            // 
            // descripcionDataGridViewTextBoxColumn1
            // 
            descripcionDataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            descripcionDataGridViewTextBoxColumn1.DataPropertyName = "Descripcion";
            descripcionDataGridViewTextBoxColumn1.FillWeight = 40F;
            descripcionDataGridViewTextBoxColumn1.HeaderText = "Descripcion";
            descripcionDataGridViewTextBoxColumn1.Name = "descripcionDataGridViewTextBoxColumn1";
            descripcionDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // costoDataGridViewTextBoxColumn
            // 
            costoDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            costoDataGridViewTextBoxColumn.DataPropertyName = "Costo";
            costoDataGridViewTextBoxColumn.FillWeight = 30F;
            costoDataGridViewTextBoxColumn.HeaderText = "Costo";
            costoDataGridViewTextBoxColumn.Name = "costoDataGridViewTextBoxColumn";
            costoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // unidadesDataGridViewTextBoxColumn
            // 
            unidadesDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            unidadesDataGridViewTextBoxColumn.DataPropertyName = "Unidades";
            unidadesDataGridViewTextBoxColumn.FillWeight = 15F;
            unidadesDataGridViewTextBoxColumn.HeaderText = "Unidades";
            unidadesDataGridViewTextBoxColumn.Name = "unidadesDataGridViewTextBoxColumn";
            unidadesDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cantidadMedidaDataGridViewTextBoxColumn
            // 
            cantidadMedidaDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            cantidadMedidaDataGridViewTextBoxColumn.DataPropertyName = "CantidadMedida";
            cantidadMedidaDataGridViewTextBoxColumn.FillWeight = 15F;
            cantidadMedidaDataGridViewTextBoxColumn.HeaderText = "Medida";
            cantidadMedidaDataGridViewTextBoxColumn.Name = "cantidadMedidaDataGridViewTextBoxColumn";
            cantidadMedidaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bindingSourceCostos
            // 
            bindingSourceCostos.DataSource = typeof(Entidades_SGBM.CostosServicios);
            // 
            // dataGridServicios
            // 
            dataGridServicios.AllowUserToAddRows = false;
            dataGridServicios.AllowUserToDeleteRows = false;
            dataGridServicios.AllowUserToOrderColumns = true;
            dataGridServicios.AutoGenerateColumns = false;
            dataGridServicios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridServicios.Columns.AddRange(new DataGridViewColumn[] { nombreServicioDataGridViewTextBoxColumn, descripcionDataGridViewTextBoxColumn, precioVenta, costos, margen, comision, duracionMinutosDataGridViewTextBoxColumn, puntajeDataGridViewTextBoxColumn, categoriasDataGridViewTextBoxColumn });
            dataGridServicios.DataSource = bindingSourceServicios;
            dataGridServicios.Dock = DockStyle.Fill;
            dataGridServicios.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridServicios.Location = new Point(3, 19);
            dataGridServicios.MultiSelect = false;
            dataGridServicios.Name = "dataGridServicios";
            dataGridServicios.ReadOnly = true;
            dataGridServicios.RowHeadersVisible = false;
            dataGridServicios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridServicios.Size = new Size(752, 387);
            dataGridServicios.TabIndex = 2;
            dataGridServicios.SelectionChanged += DataGridServicios_SelectionChanged;
            // 
            // nombreServicioDataGridViewTextBoxColumn
            // 
            nombreServicioDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            nombreServicioDataGridViewTextBoxColumn.DataPropertyName = "NombreServicio";
            nombreServicioDataGridViewTextBoxColumn.FillWeight = 15F;
            nombreServicioDataGridViewTextBoxColumn.HeaderText = "Servicio";
            nombreServicioDataGridViewTextBoxColumn.Name = "nombreServicioDataGridViewTextBoxColumn";
            nombreServicioDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // descripcionDataGridViewTextBoxColumn
            // 
            descripcionDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            descripcionDataGridViewTextBoxColumn.DataPropertyName = "Descripcion";
            descripcionDataGridViewTextBoxColumn.FillWeight = 25F;
            descripcionDataGridViewTextBoxColumn.HeaderText = "Descripcion";
            descripcionDataGridViewTextBoxColumn.Name = "descripcionDataGridViewTextBoxColumn";
            descripcionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // precioVenta
            // 
            precioVenta.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            precioVenta.DataPropertyName = "PrecioVenta";
            precioVenta.FillWeight = 10F;
            precioVenta.HeaderText = "PVP";
            precioVenta.Name = "precioVenta";
            precioVenta.ReadOnly = true;
            // 
            // costos
            // 
            costos.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            costos.DataPropertyName = "Costos";
            costos.FillWeight = 9F;
            costos.HeaderText = "Costos";
            costos.Name = "costos";
            costos.ReadOnly = true;
            // 
            // margen
            // 
            margen.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            margen.DataPropertyName = "Margen";
            margen.FillWeight = 9F;
            margen.HeaderText = "Margen";
            margen.Name = "margen";
            margen.ReadOnly = true;
            // 
            // comision
            // 
            comision.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            comision.DataPropertyName = "Comision";
            comision.FillWeight = 8F;
            comision.HeaderText = "Comision";
            comision.Name = "comision";
            comision.ReadOnly = true;
            // 
            // duracionMinutosDataGridViewTextBoxColumn
            // 
            duracionMinutosDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            duracionMinutosDataGridViewTextBoxColumn.DataPropertyName = "DuracionMinutos";
            duracionMinutosDataGridViewTextBoxColumn.FillWeight = 8F;
            duracionMinutosDataGridViewTextBoxColumn.HeaderText = "Duracion Min.";
            duracionMinutosDataGridViewTextBoxColumn.Name = "duracionMinutosDataGridViewTextBoxColumn";
            duracionMinutosDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // puntajeDataGridViewTextBoxColumn
            // 
            puntajeDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            puntajeDataGridViewTextBoxColumn.DataPropertyName = "Puntaje";
            puntajeDataGridViewTextBoxColumn.FillWeight = 7F;
            puntajeDataGridViewTextBoxColumn.HeaderText = "Puntaje";
            puntajeDataGridViewTextBoxColumn.Name = "puntajeDataGridViewTextBoxColumn";
            puntajeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // categoriasDataGridViewTextBoxColumn
            // 
            categoriasDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            categoriasDataGridViewTextBoxColumn.DataPropertyName = "Categorias";
            categoriasDataGridViewTextBoxColumn.FillWeight = 9F;
            categoriasDataGridViewTextBoxColumn.HeaderText = "Categoria";
            categoriasDataGridViewTextBoxColumn.Name = "categoriasDataGridViewTextBoxColumn";
            categoriasDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bindingSourceServicios
            // 
            bindingSourceServicios.DataSource = typeof(Entidades_SGBM.Servicios);
            // 
            // groupBoxServicios
            // 
            groupBoxServicios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxServicios.Controls.Add(dataGridServicios);
            groupBoxServicios.Location = new Point(4, 7);
            groupBoxServicios.Name = "groupBoxServicios";
            groupBoxServicios.Size = new Size(758, 409);
            groupBoxServicios.TabIndex = 3;
            groupBoxServicios.TabStop = false;
            groupBoxServicios.Text = "Servicios";
            // 
            // groupBoxBotones
            // 
            groupBoxBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            groupBoxBotones.Controls.Add(btnConsultar);
            groupBoxBotones.Controls.Add(btnSeleccionar);
            groupBoxBotones.Controls.Add(btnSalir);
            groupBoxBotones.Location = new Point(768, 441);
            groupBoxBotones.Name = "groupBoxBotones";
            groupBoxBotones.Size = new Size(284, 203);
            groupBoxBotones.TabIndex = 6;
            groupBoxBotones.TabStop = false;
            // 
            // btnConsultar
            // 
            btnConsultar.Dock = DockStyle.Top;
            btnConsultar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConsultar.Location = new Point(3, 19);
            btnConsultar.Name = "btnConsultar";
            btnConsultar.Size = new Size(278, 47);
            btnConsultar.TabIndex = 0;
            btnConsultar.Tag = "btnPrincipal";
            btnConsultar.Text = "Ver Detalles";
            btnConsultar.UseVisualStyleBackColor = true;
            btnConsultar.Click += BtnConsultar_Click;
            // 
            // btnSeleccionar
            // 
            btnSeleccionar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSeleccionar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSeleccionar.Location = new Point(3, 85);
            btnSeleccionar.Name = "btnSeleccionar";
            btnSeleccionar.Size = new Size(278, 45);
            btnSeleccionar.TabIndex = 1;
            btnSeleccionar.Tag = "btnPrincipalV";
            btnSeleccionar.Text = "Seleccionar";
            btnSeleccionar.UseVisualStyleBackColor = true;
            btnSeleccionar.Click += BtnSeleccionar_Click;
            // 
            // btnSalir
            // 
            btnSalir.Dock = DockStyle.Bottom;
            btnSalir.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSalir.Location = new Point(3, 155);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(278, 45);
            btnSalir.TabIndex = 2;
            btnSalir.Tag = "btnPrincipalR";
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += BtnSalir_Click;
            // 
            // btnRegistrar
            // 
            btnRegistrar.Dock = DockStyle.Fill;
            btnRegistrar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRegistrar.Location = new Point(3, 19);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new Size(275, 39);
            btnRegistrar.TabIndex = 7;
            btnRegistrar.Tag = "btnPrincipalV";
            btnRegistrar.Text = "Nuevo Servicio";
            btnRegistrar.UseVisualStyleBackColor = true;
            btnRegistrar.Click += BtnRegistrar_Click;
            // 
            // groupBoxBtnNuevo
            // 
            groupBoxBtnNuevo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBoxBtnNuevo.Controls.Add(btnRegistrar);
            groupBoxBtnNuevo.Location = new Point(768, 12);
            groupBoxBtnNuevo.Name = "groupBoxBtnNuevo";
            groupBoxBtnNuevo.Size = new Size(281, 61);
            groupBoxBtnNuevo.TabIndex = 7;
            groupBoxBtnNuevo.TabStop = false;
            // 
            // errorProvider1
            // 
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider1.ContainerControl = this;
            // 
            // FrmAbmServicios
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1056, 649);
            Controls.Add(groupBoxBtnNuevo);
            Controls.Add(groupBoxBotones);
            Controls.Add(groupBoxServicios);
            Controls.Add(groupBoxFooter);
            Controls.Add(groupBoxFiltro);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmAbmServicios";
            Text = "FrmAbmServicios";
            Load += FrmAbmServicios_Load;
            groupBoxFiltro.ResumeLayout(false);
            groupBoxFiltro.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCategorias).EndInit();
            groupBoxFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridCostos).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCostos).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridServicios).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceServicios).EndInit();
            groupBoxServicios.ResumeLayout(false);
            groupBoxBotones.ResumeLayout(false);
            groupBoxBtnNuevo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxFiltro;
        private GroupBox groupBoxFooter;
        private DataGridView dataGridCostos;
        private DataGridView dataGridServicios;
        private GroupBox groupBoxServicios;
        private TextBox txtFiltroRapido;
        private ComboBox cbCampos;
        private Label label2;
        private ComboBox cbCriterios;
        private Label label1;
        private ComboBox cbCategorias;
        private Label label3;
        private Label labelCampo1;
        private TextBox txtBusqueda;
        private Button btnLimpiar;
        private Button btnBuscar;
        private GroupBox groupBoxBotones;
        private Button btnConsultar;
        private Button btnSeleccionar;
        private Button btnSalir;
        private Button btnRegistrar;
        private BindingSource bindingSourceServicios;
        private DataGridViewTextBoxColumn descripcionDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn costoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn unidadesDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantidadMedidaDataGridViewTextBoxColumn;
        private BindingSource bindingSourceCostos;
        private BindingSource bindingSourceCategorias;
        private GroupBox groupBoxBtnNuevo;
        private DataGridViewTextBoxColumn nombreServicioDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn descripcionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn precioVenta;
        private DataGridViewTextBoxColumn costos;
        private DataGridViewTextBoxColumn margen;
        private DataGridViewTextBoxColumn comision;
        private DataGridViewTextBoxColumn duracionMinutosDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn puntajeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn categoriasDataGridViewTextBoxColumn;
        private ErrorProvider errorProvider1;
        private CheckBox checkAnulados;
    }
}