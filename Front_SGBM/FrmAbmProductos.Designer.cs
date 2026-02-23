namespace Front_SGBM
{
    partial class FrmAbmProductos
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
            groupBoxProductos = new GroupBox();
            lblCatAbm = new Label();
            cbCategoríaAbm = new ComboBox();
            btnModificar = new Button();
            btnBaja = new Button();
            checkSinStock = new CheckBox();
            checkAnulados = new CheckBox();
            txtFiltro = new TextBox();
            lblFiltro = new Label();
            dataGridProductos = new DataGridView();
            codProductoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            descripcionDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            precioVentaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            stockDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            cantidadMedidaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            medidaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            costoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            Comision = new DataGridViewTextBoxColumn();
            bindingSourceProductos = new BindingSource(components);
            groupBoxEdit = new GroupBox();
            lblPorcentaje = new Label();
            btnLimpiar = new Button();
            label1 = new Label();
            cbEditCategoria = new ComboBox();
            bindingSourceCategorias = new BindingSource(components);
            txtCantMedida = new TextBox();
            lblCantMedida = new Label();
            lblUdMedida = new Label();
            txtComision = new TextBox();
            lblComision = new Label();
            txtMargen = new TextBox();
            lblMargen = new Label();
            txtCosto = new TextBox();
            lblCosto = new Label();
            cbUdMedida = new ComboBox();
            bindingSourceMedidas = new BindingSource(components);
            txtMedida = new TextBox();
            lblMedida = new Label();
            txtStockUds = new TextBox();
            lblStockUds = new Label();
            txtPrecioVenta = new TextBox();
            lblPrecio = new Label();
            checkActivo = new CheckBox();
            txtDescripcion = new TextBox();
            lblDescripcion = new Label();
            txtCodigo = new TextBox();
            lblCodigo = new Label();
            groupBoxBotones = new GroupBox();
            btnGuardar = new Button();
            btnNuevo = new Button();
            btnSalir = new Button();
            errorProvider1 = new ErrorProvider(components);
            groupBoxProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridProductos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceProductos).BeginInit();
            groupBoxEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCategorias).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceMedidas).BeginInit();
            groupBoxBotones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // groupBoxProductos
            // 
            groupBoxProductos.Controls.Add(lblCatAbm);
            groupBoxProductos.Controls.Add(cbCategoríaAbm);
            groupBoxProductos.Controls.Add(btnModificar);
            groupBoxProductos.Controls.Add(btnBaja);
            groupBoxProductos.Controls.Add(checkSinStock);
            groupBoxProductos.Controls.Add(checkAnulados);
            groupBoxProductos.Controls.Add(txtFiltro);
            groupBoxProductos.Controls.Add(lblFiltro);
            groupBoxProductos.Controls.Add(dataGridProductos);
            groupBoxProductos.Dock = DockStyle.Top;
            groupBoxProductos.Location = new Point(0, 0);
            groupBoxProductos.Name = "groupBoxProductos";
            groupBoxProductos.Size = new Size(1178, 471);
            groupBoxProductos.TabIndex = 0;
            groupBoxProductos.TabStop = false;
            groupBoxProductos.Text = "Productos";
            // 
            // lblCatAbm
            // 
            lblCatAbm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCatAbm.AutoSize = true;
            lblCatAbm.Location = new Point(623, 29);
            lblCatAbm.Name = "lblCatAbm";
            lblCatAbm.Size = new Size(64, 15);
            lblCatAbm.TabIndex = 8;
            lblCatAbm.Text = "Categoría: ";
            // 
            // cbCategoríaAbm
            // 
            cbCategoríaAbm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbCategoríaAbm.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCategoríaAbm.FormattingEnabled = true;
            cbCategoríaAbm.Location = new Point(712, 26);
            cbCategoríaAbm.Name = "cbCategoríaAbm";
            cbCategoríaAbm.Size = new Size(224, 23);
            cbCategoríaAbm.TabIndex = 3;
            cbCategoríaAbm.SelectedIndexChanged += cbCategoríaAbm_SelectedIndexChanged;
            // 
            // btnModificar
            // 
            btnModificar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnModificar.Enabled = false;
            btnModificar.Location = new Point(979, 40);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(91, 23);
            btnModificar.TabIndex = 4;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = true;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnBaja
            // 
            btnBaja.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBaja.Enabled = false;
            btnBaja.Location = new Point(1076, 40);
            btnBaja.Name = "btnBaja";
            btnBaja.Size = new Size(91, 23);
            btnBaja.TabIndex = 5;
            btnBaja.Tag = "btnNormalR";
            btnBaja.Text = "Anular";
            btnBaja.UseVisualStyleBackColor = true;
            btnBaja.Click += btnBaja_Click;
            // 
            // checkSinStock
            // 
            checkSinStock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkSinStock.AutoSize = true;
            checkSinStock.Checked = true;
            checkSinStock.CheckState = CheckState.Checked;
            checkSinStock.Location = new Point(470, 43);
            checkSinStock.Name = "checkSinStock";
            checkSinStock.Size = new Size(110, 19);
            checkSinStock.TabIndex = 2;
            checkSinStock.Text = "Incluir Sin Stock";
            checkSinStock.UseVisualStyleBackColor = true;
            checkSinStock.CheckedChanged += checkSinStock_CheckedChanged;
            // 
            // checkAnulados
            // 
            checkAnulados.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkAnulados.AutoSize = true;
            checkAnulados.Location = new Point(470, 18);
            checkAnulados.Name = "checkAnulados";
            checkAnulados.Size = new Size(112, 19);
            checkAnulados.TabIndex = 1;
            checkAnulados.Text = "Incluir Anulados";
            checkAnulados.UseVisualStyleBackColor = true;
            checkAnulados.CheckedChanged += checkAnulados_CheckedChanged;
            // 
            // txtFiltro
            // 
            txtFiltro.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFiltro.Location = new Point(67, 26);
            txtFiltro.Name = "txtFiltro";
            txtFiltro.Size = new Size(382, 23);
            txtFiltro.TabIndex = 0;
            txtFiltro.TextChanged += txtFiltro_TextChanged;
            // 
            // lblFiltro
            // 
            lblFiltro.AutoSize = true;
            lblFiltro.Location = new Point(6, 29);
            lblFiltro.Name = "lblFiltro";
            lblFiltro.Size = new Size(40, 15);
            lblFiltro.TabIndex = 1;
            lblFiltro.Text = "Filtro: ";
            // 
            // dataGridProductos
            // 
            dataGridProductos.AllowUserToAddRows = false;
            dataGridProductos.AllowUserToDeleteRows = false;
            dataGridProductos.AllowUserToOrderColumns = true;
            dataGridProductos.AutoGenerateColumns = false;
            dataGridProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridProductos.Columns.AddRange(new DataGridViewColumn[] { codProductoDataGridViewTextBoxColumn, descripcionDataGridViewTextBoxColumn, precioVentaDataGridViewTextBoxColumn, stockDataGridViewTextBoxColumn, cantidadMedidaDataGridViewTextBoxColumn, medidaDataGridViewTextBoxColumn, costoDataGridViewTextBoxColumn, Comision });
            dataGridProductos.DataSource = bindingSourceProductos;
            dataGridProductos.Dock = DockStyle.Bottom;
            dataGridProductos.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridProductos.Location = new Point(3, 69);
            dataGridProductos.MultiSelect = false;
            dataGridProductos.Name = "dataGridProductos";
            dataGridProductos.ReadOnly = true;
            dataGridProductos.RowHeadersVisible = false;
            dataGridProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridProductos.Size = new Size(1172, 399);
            dataGridProductos.TabIndex = 6;
            dataGridProductos.SelectionChanged += dataGridProductos_SelectionChanged;
            // 
            // codProductoDataGridViewTextBoxColumn
            // 
            codProductoDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            codProductoDataGridViewTextBoxColumn.DataPropertyName = "CodProducto";
            codProductoDataGridViewTextBoxColumn.FillWeight = 10F;
            codProductoDataGridViewTextBoxColumn.HeaderText = "Código";
            codProductoDataGridViewTextBoxColumn.Name = "codProductoDataGridViewTextBoxColumn";
            codProductoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // descripcionDataGridViewTextBoxColumn
            // 
            descripcionDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            descripcionDataGridViewTextBoxColumn.DataPropertyName = "Descripcion";
            descripcionDataGridViewTextBoxColumn.FillWeight = 28F;
            descripcionDataGridViewTextBoxColumn.HeaderText = "Descripcion";
            descripcionDataGridViewTextBoxColumn.Name = "descripcionDataGridViewTextBoxColumn";
            descripcionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // precioVentaDataGridViewTextBoxColumn
            // 
            precioVentaDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            precioVentaDataGridViewTextBoxColumn.DataPropertyName = "PrecioVenta";
            precioVentaDataGridViewTextBoxColumn.FillWeight = 13F;
            precioVentaDataGridViewTextBoxColumn.HeaderText = "Precio de Venta";
            precioVentaDataGridViewTextBoxColumn.Name = "precioVentaDataGridViewTextBoxColumn";
            precioVentaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // stockDataGridViewTextBoxColumn
            // 
            stockDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            stockDataGridViewTextBoxColumn.DataPropertyName = "Stock";
            stockDataGridViewTextBoxColumn.FillWeight = 8F;
            stockDataGridViewTextBoxColumn.HeaderText = "Stock";
            stockDataGridViewTextBoxColumn.Name = "stockDataGridViewTextBoxColumn";
            stockDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cantidadMedidaDataGridViewTextBoxColumn
            // 
            cantidadMedidaDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            cantidadMedidaDataGridViewTextBoxColumn.DataPropertyName = "CantidadMedida";
            cantidadMedidaDataGridViewTextBoxColumn.FillWeight = 13F;
            cantidadMedidaDataGridViewTextBoxColumn.HeaderText = "Dosificación";
            cantidadMedidaDataGridViewTextBoxColumn.Name = "cantidadMedidaDataGridViewTextBoxColumn";
            cantidadMedidaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // medidaDataGridViewTextBoxColumn
            // 
            medidaDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            medidaDataGridViewTextBoxColumn.DataPropertyName = "Medida";
            medidaDataGridViewTextBoxColumn.FillWeight = 9F;
            medidaDataGridViewTextBoxColumn.HeaderText = "Cant. Suelta";
            medidaDataGridViewTextBoxColumn.Name = "medidaDataGridViewTextBoxColumn";
            medidaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // costoDataGridViewTextBoxColumn
            // 
            costoDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            costoDataGridViewTextBoxColumn.DataPropertyName = "Costo";
            costoDataGridViewTextBoxColumn.FillWeight = 12F;
            costoDataGridViewTextBoxColumn.HeaderText = "Costo";
            costoDataGridViewTextBoxColumn.Name = "costoDataGridViewTextBoxColumn";
            costoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Comision
            // 
            Comision.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Comision.DataPropertyName = "Comision";
            Comision.FillWeight = 7F;
            Comision.HeaderText = "Comision";
            Comision.Name = "Comision";
            Comision.ReadOnly = true;
            // 
            // bindingSourceProductos
            // 
            bindingSourceProductos.DataSource = typeof(Entidades_SGBM.Productos);
            // 
            // groupBoxEdit
            // 
            groupBoxEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxEdit.Controls.Add(lblPorcentaje);
            groupBoxEdit.Controls.Add(btnLimpiar);
            groupBoxEdit.Controls.Add(label1);
            groupBoxEdit.Controls.Add(cbEditCategoria);
            groupBoxEdit.Controls.Add(txtCantMedida);
            groupBoxEdit.Controls.Add(lblCantMedida);
            groupBoxEdit.Controls.Add(lblUdMedida);
            groupBoxEdit.Controls.Add(txtComision);
            groupBoxEdit.Controls.Add(lblComision);
            groupBoxEdit.Controls.Add(txtMargen);
            groupBoxEdit.Controls.Add(lblMargen);
            groupBoxEdit.Controls.Add(txtCosto);
            groupBoxEdit.Controls.Add(lblCosto);
            groupBoxEdit.Controls.Add(cbUdMedida);
            groupBoxEdit.Controls.Add(txtMedida);
            groupBoxEdit.Controls.Add(lblMedida);
            groupBoxEdit.Controls.Add(txtStockUds);
            groupBoxEdit.Controls.Add(lblStockUds);
            groupBoxEdit.Controls.Add(txtPrecioVenta);
            groupBoxEdit.Controls.Add(lblPrecio);
            groupBoxEdit.Controls.Add(checkActivo);
            groupBoxEdit.Controls.Add(txtDescripcion);
            groupBoxEdit.Controls.Add(lblDescripcion);
            groupBoxEdit.Controls.Add(txtCodigo);
            groupBoxEdit.Controls.Add(lblCodigo);
            groupBoxEdit.Location = new Point(6, 477);
            groupBoxEdit.Name = "groupBoxEdit";
            groupBoxEdit.Size = new Size(882, 192);
            groupBoxEdit.TabIndex = 1;
            groupBoxEdit.TabStop = false;
            groupBoxEdit.Text = "Datos de Producto";
            // 
            // lblPorcentaje
            // 
            lblPorcentaje.AutoSize = true;
            lblPorcentaje.Location = new Point(385, 71);
            lblPorcentaje.Name = "lblPorcentaje";
            lblPorcentaje.Size = new Size(17, 15);
            lblPorcentaje.TabIndex = 24;
            lblPorcentaje.Text = "%";
            // 
            // btnLimpiar
            // 
            btnLimpiar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLimpiar.Location = new Point(657, 156);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(219, 23);
            btnLimpiar.TabIndex = 11;
            btnLimpiar.Text = "Limpiar Selección";
            btnLimpiar.UseVisualStyleBackColor = true;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(237, 159);
            label1.Name = "label1";
            label1.Size = new Size(64, 15);
            label1.TabIndex = 22;
            label1.Text = "Categoría: ";
            // 
            // cbEditCategoria
            // 
            cbEditCategoria.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbEditCategoria.DataSource = bindingSourceCategorias;
            cbEditCategoria.DisplayMember = "Descripcion";
            cbEditCategoria.FormattingEnabled = true;
            cbEditCategoria.Location = new Point(324, 156);
            cbEditCategoria.Name = "cbEditCategoria";
            cbEditCategoria.Size = new Size(300, 23);
            cbEditCategoria.TabIndex = 10;
            cbEditCategoria.ValueMember = "IdCategoria";
            // 
            // bindingSourceCategorias
            // 
            bindingSourceCategorias.DataSource = typeof(Entidades_SGBM.Categorias);
            // 
            // txtCantMedida
            // 
            txtCantMedida.Enabled = false;
            txtCantMedida.Location = new Point(93, 157);
            txtCantMedida.Name = "txtCantMedida";
            txtCantMedida.Size = new Size(97, 23);
            txtCantMedida.TabIndex = 9;
            txtCantMedida.KeyPress += txtNumerico_KeyPress;
            // 
            // lblCantMedida
            // 
            lblCantMedida.AutoSize = true;
            lblCantMedida.Location = new Point(5, 159);
            lblCantMedida.Name = "lblCantMedida";
            lblCantMedida.Size = new Size(73, 15);
            lblCantMedida.TabIndex = 19;
            lblCantMedida.Text = "Cant. Suelta:";
            // 
            // lblUdMedida
            // 
            lblUdMedida.AutoSize = true;
            lblUdMedida.Location = new Point(237, 114);
            lblUdMedida.Name = "lblUdMedida";
            lblUdMedida.Size = new Size(71, 15);
            lblUdMedida.TabIndex = 18;
            lblUdMedida.Text = "Ud. Medida:";
            // 
            // txtComision
            // 
            txtComision.Location = new Point(327, 68);
            txtComision.Name = "txtComision";
            txtComision.Size = new Size(52, 23);
            txtComision.TabIndex = 4;
            txtComision.TextChanged += txtComision_TextChanged;
            txtComision.KeyPress += txtDecimal_KeyPress;
            // 
            // lblComision
            // 
            lblComision.AutoSize = true;
            lblComision.Location = new Point(237, 71);
            lblComision.Name = "lblComision";
            lblComision.Size = new Size(61, 15);
            lblComision.TabIndex = 16;
            lblComision.Text = "Comisión:";
            // 
            // txtMargen
            // 
            txtMargen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtMargen.Enabled = false;
            txtMargen.Location = new Point(774, 68);
            txtMargen.Name = "txtMargen";
            txtMargen.Size = new Size(102, 23);
            txtMargen.TabIndex = 15;
            // 
            // lblMargen
            // 
            lblMargen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblMargen.AutoSize = true;
            lblMargen.Location = new Point(669, 71);
            lblMargen.Name = "lblMargen";
            lblMargen.Size = new Size(51, 15);
            lblMargen.TabIndex = 14;
            lblMargen.Text = "Margen:";
            // 
            // txtCosto
            // 
            txtCosto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCosto.Location = new Point(505, 68);
            txtCosto.Name = "txtCosto";
            txtCosto.Size = new Size(119, 23);
            txtCosto.TabIndex = 5;
            txtCosto.TextChanged += txtCosto_TextChanged;
            txtCosto.KeyPress += txtDecimal_KeyPress;
            // 
            // lblCosto
            // 
            lblCosto.AutoSize = true;
            lblCosto.Location = new Point(440, 71);
            lblCosto.Name = "lblCosto";
            lblCosto.Size = new Size(41, 15);
            lblCosto.TabIndex = 12;
            lblCosto.Text = "Costo:";
            // 
            // cbUdMedida
            // 
            cbUdMedida.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbUdMedida.DataSource = bindingSourceMedidas;
            cbUdMedida.DisplayMember = "Unidad";
            cbUdMedida.DropDownStyle = ComboBoxStyle.DropDownList;
            cbUdMedida.FormattingEnabled = true;
            cbUdMedida.Location = new Point(324, 111);
            cbUdMedida.Name = "cbUdMedida";
            cbUdMedida.Size = new Size(300, 23);
            cbUdMedida.TabIndex = 7;
            cbUdMedida.ValueMember = "IdUnidadMedida";
            cbUdMedida.SelectedIndexChanged += cbUdMedida_SelectedIndexChanged;
            // 
            // bindingSourceMedidas
            // 
            bindingSourceMedidas.DataSource = typeof(Entidades_SGBM.UnidadesMedidas);
            // 
            // txtMedida
            // 
            txtMedida.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtMedida.Enabled = false;
            txtMedida.Location = new Point(774, 111);
            txtMedida.Name = "txtMedida";
            txtMedida.Size = new Size(102, 23);
            txtMedida.TabIndex = 8;
            txtMedida.KeyPress += txtNumerico_KeyPress;
            // 
            // lblMedida
            // 
            lblMedida.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblMedida.AutoSize = true;
            lblMedida.Location = new Point(669, 114);
            lblMedida.Name = "lblMedida";
            lblMedida.Size = new Size(75, 15);
            lblMedida.TabIndex = 9;
            lblMedida.Text = "Dosificación:";
            // 
            // txtStockUds
            // 
            txtStockUds.Location = new Point(93, 111);
            txtStockUds.Name = "txtStockUds";
            txtStockUds.Size = new Size(97, 23);
            txtStockUds.TabIndex = 6;
            txtStockUds.KeyPress += txtNumerico_KeyPress;
            // 
            // lblStockUds
            // 
            lblStockUds.AutoSize = true;
            lblStockUds.Location = new Point(6, 114);
            lblStockUds.Name = "lblStockUds";
            lblStockUds.Size = new Size(62, 15);
            lblStockUds.TabIndex = 7;
            lblStockUds.Text = "Stock Uds:";
            // 
            // txtPrecioVenta
            // 
            txtPrecioVenta.Location = new Point(93, 68);
            txtPrecioVenta.Name = "txtPrecioVenta";
            txtPrecioVenta.Size = new Size(97, 23);
            txtPrecioVenta.TabIndex = 3;
            txtPrecioVenta.TextChanged += txtPrecioVenta_TextChanged;
            txtPrecioVenta.KeyPress += txtDecimal_KeyPress;
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.Location = new Point(6, 71);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(75, 15);
            lblPrecio.TabIndex = 5;
            lblPrecio.Text = "Precio Venta:";
            // 
            // checkActivo
            // 
            checkActivo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkActivo.AutoSize = true;
            checkActivo.Checked = true;
            checkActivo.CheckState = CheckState.Checked;
            checkActivo.Location = new Point(774, 28);
            checkActivo.Name = "checkActivo";
            checkActivo.Size = new Size(60, 19);
            checkActivo.TabIndex = 2;
            checkActivo.Text = "Activo";
            checkActivo.UseVisualStyleBackColor = true;
            // 
            // txtDescripcion
            // 
            txtDescripcion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescripcion.Location = new Point(327, 26);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new Size(425, 23);
            txtDescripcion.TabIndex = 1;
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(237, 29);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(72, 15);
            lblDescripcion.TabIndex = 2;
            lblDescripcion.Text = "Descripción:";
            // 
            // txtCodigo
            // 
            txtCodigo.Location = new Point(70, 26);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(120, 23);
            txtCodigo.TabIndex = 0;
            txtCodigo.KeyPress += txtNumerico_KeyPress;
            // 
            // lblCodigo
            // 
            lblCodigo.AutoSize = true;
            lblCodigo.Location = new Point(6, 29);
            lblCodigo.Name = "lblCodigo";
            lblCodigo.Size = new Size(49, 15);
            lblCodigo.TabIndex = 0;
            lblCodigo.Text = "Código:";
            // 
            // groupBoxBotones
            // 
            groupBoxBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            groupBoxBotones.Controls.Add(btnGuardar);
            groupBoxBotones.Controls.Add(btnNuevo);
            groupBoxBotones.Controls.Add(btnSalir);
            groupBoxBotones.Location = new Point(894, 477);
            groupBoxBotones.Name = "groupBoxBotones";
            groupBoxBotones.Size = new Size(278, 192);
            groupBoxBotones.TabIndex = 2;
            groupBoxBotones.TabStop = false;
            // 
            // btnGuardar
            // 
            btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnGuardar.Enabled = false;
            btnGuardar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.Location = new Point(6, 76);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(266, 45);
            btnGuardar.TabIndex = 1;
            btnGuardar.Tag = "btnPrincipal";
            btnGuardar.Text = "Guardar Cambios";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnNuevo.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNuevo.Location = new Point(6, 22);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(266, 45);
            btnNuevo.TabIndex = 0;
            btnNuevo.Tag = "btnPrincipalV";
            btnNuevo.Text = "Registrar Nuevo";
            btnNuevo.UseVisualStyleBackColor = true;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // btnSalir
            // 
            btnSalir.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSalir.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSalir.Location = new Point(6, 134);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(266, 45);
            btnSalir.TabIndex = 2;
            btnSalir.Tag = "btnPrincipalR";
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // errorProvider1
            // 
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider1.ContainerControl = this;
            // 
            // FrmAbmProductos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1178, 681);
            Controls.Add(groupBoxBotones);
            Controls.Add(groupBoxEdit);
            Controls.Add(groupBoxProductos);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmAbmProductos";
            Text = "FrmAbmProductos";
            Load += FrmAbmProductos_Load;
            groupBoxProductos.ResumeLayout(false);
            groupBoxProductos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridProductos).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceProductos).EndInit();
            groupBoxEdit.ResumeLayout(false);
            groupBoxEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCategorias).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceMedidas).EndInit();
            groupBoxBotones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxProductos;
        private DataGridView dataGridProductos;
        private BindingSource bindingSourceProductos;
        private TextBox txtFiltro;
        private Label lblFiltro;
        private CheckBox checkSinStock;
        private CheckBox checkAnulados;
        private Button btnBaja;
        private Button btnModificar;
        private Label lblCatAbm;
        private ComboBox cbCategoríaAbm;
        private GroupBox groupBoxEdit;
        private GroupBox groupBoxBotones;
        private Button btnNuevo;
        private Button btnSalir;
        private CheckBox checkActivo;
        private TextBox txtDescripcion;
        private Label lblDescripcion;
        private TextBox txtCodigo;
        private Label lblCodigo;
        private TextBox txtMedida;
        private Label lblMedida;
        private TextBox txtStockUds;
        private Label lblStockUds;
        private TextBox txtPrecioVenta;
        private Label lblPrecio;
        private ComboBox cbUdMedida;
        private TextBox txtMargen;
        private Label lblMargen;
        private TextBox txtCosto;
        private Label lblCosto;
        private TextBox txtComision;
        private Label lblComision;
        private Label lblUdMedida;
        private TextBox txtCantMedida;
        private Label lblCantMedida;
        private Label label1;
        private ComboBox cbEditCategoria;
        private Button btnLimpiar;
        private BindingSource bindingSourceMedidas;
        private BindingSource bindingSourceCategorias;
        private Button btnGuardar;
        private Label lblPorcentaje;
        private ErrorProvider errorProvider1;
        private DataGridViewTextBoxColumn codProductoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn descripcionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn precioVentaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn stockDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantidadMedidaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn medidaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn costoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn Comision;
    }
}