namespace Front_SGBM
{
    partial class FrmEditVentas
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
            panelHeader = new Panel();
            cbVendedor = new ComboBox();
            bindingEmpleados = new BindingSource(components);
            lblVendedor = new Label();
            txtNombreCompleto = new TextBox();
            btnBuscarCliente = new Button();
            txtDni = new TextBox();
            lblTitulo = new Label();
            panel1 = new Panel();
            lblMensaje = new Label();
            btnCancelar = new Button();
            btnGuardar = new Button();
            lblDescuento = new Label();
            cbMedioPago = new ComboBox();
            bindingMedios = new BindingSource(components);
            lblMedioPago = new Label();
            txtTotalAbonado = new TextBox();
            lblTotal = new Label();
            lblFiltro = new Label();
            pnlVenta = new Panel();
            groupBoxCarrito = new GroupBox();
            lblPorciento = new Label();
            txtDescuento = new TextBox();
            lblDesc = new Label();
            btnQuitarDetalle = new Button();
            txtTotalEfectivo = new TextBox();
            lblTotalEfectivo = new Label();
            txtTotalACobrar = new TextBox();
            lblTotalLista = new Label();
            dataGridCarrito = new DataGridView();
            descripcionDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            cantidadDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            precioUnitarioDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            InteresDescuento = new DataGridViewTextBoxColumn();
            subTotalDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bindingCarrito = new BindingSource(components);
            groupBoxSeleccion = new GroupBox();
            checkStock = new CheckBox();
            numCantidad = new NumericUpDown();
            lblCantidad = new Label();
            btnSeleccionar = new Button();
            dataGridSeleccion = new DataGridView();
            txtFiltro = new TextBox();
            errorProvider1 = new ErrorProvider(components);
            lblNroVtaTitulo = new Label();
            lblNroVta = new Label();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingEmpleados).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingMedios).BeginInit();
            pnlVenta.SuspendLayout();
            groupBoxCarrito.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridCarrito).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingCarrito).BeginInit();
            groupBoxSeleccion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridSeleccion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.Controls.Add(lblNroVta);
            panelHeader.Controls.Add(lblNroVtaTitulo);
            panelHeader.Controls.Add(cbVendedor);
            panelHeader.Controls.Add(lblVendedor);
            panelHeader.Controls.Add(txtNombreCompleto);
            panelHeader.Controls.Add(btnBuscarCliente);
            panelHeader.Controls.Add(txtDni);
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1043, 96);
            panelHeader.TabIndex = 0;
            // 
            // cbVendedor
            // 
            cbVendedor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbVendedor.DataSource = bindingEmpleados;
            cbVendedor.DisplayMember = "NombreCompleto";
            cbVendedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVendedor.FormattingEnabled = true;
            cbVendedor.Location = new Point(829, 50);
            cbVendedor.Name = "cbVendedor";
            cbVendedor.Size = new Size(202, 23);
            cbVendedor.TabIndex = 5;
            cbVendedor.ValueMember = "IdEmpleado";
            // 
            // bindingEmpleados
            // 
            bindingEmpleados.DataSource = typeof(Entidades_SGBM.Empleados);
            // 
            // lblVendedor
            // 
            lblVendedor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblVendedor.AutoSize = true;
            lblVendedor.Location = new Point(741, 54);
            lblVendedor.Name = "lblVendedor";
            lblVendedor.Size = new Size(54, 15);
            lblVendedor.TabIndex = 4;
            lblVendedor.Text = "Barbero: ";
            // 
            // txtNombreCompleto
            // 
            txtNombreCompleto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtNombreCompleto.Location = new Point(312, 51);
            txtNombreCompleto.Name = "txtNombreCompleto";
            txtNombreCompleto.PlaceholderText = "NOMBRE Y APELLIDO";
            txtNombreCompleto.Size = new Size(373, 23);
            txtNombreCompleto.TabIndex = 3;
            // 
            // btnBuscarCliente
            // 
            btnBuscarCliente.Location = new Point(175, 50);
            btnBuscarCliente.Name = "btnBuscarCliente";
            btnBuscarCliente.Size = new Size(75, 23);
            btnBuscarCliente.TabIndex = 2;
            btnBuscarCliente.Text = "Buscar";
            btnBuscarCliente.UseVisualStyleBackColor = true;
            btnBuscarCliente.Click += BtnBuscarCliente_Click;
            // 
            // txtDni
            // 
            txtDni.Location = new Point(12, 51);
            txtDni.Name = "txtDni";
            txtDni.PlaceholderText = "DNI DEL CLIENTE";
            txtDni.Size = new Size(140, 23);
            txtDni.TabIndex = 1;
            txtDni.KeyPress += TxtNumerico_KeyPress;
            // 
            // lblTitulo
            // 
            lblTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Roboto Condensed", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.Location = new Point(408, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(193, 29);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Registro de Ventas";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblMensaje);
            panel1.Controls.Add(btnCancelar);
            panel1.Controls.Add(btnGuardar);
            panel1.Controls.Add(lblDescuento);
            panel1.Controls.Add(cbMedioPago);
            panel1.Controls.Add(lblMedioPago);
            panel1.Controls.Add(txtTotalAbonado);
            panel1.Controls.Add(lblTotal);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 481);
            panel1.Name = "panel1";
            panel1.Size = new Size(1043, 155);
            panel1.TabIndex = 1;
            // 
            // lblMensaje
            // 
            lblMensaje.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMensaje.AutoSize = true;
            lblMensaje.Location = new Point(385, 85);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Size = new Size(0, 15);
            lblMensaje.TabIndex = 12;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancelar.Location = new Point(787, 93);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(241, 50);
            btnCancelar.TabIndex = 11;
            btnCancelar.Tag = "btnPrincipalR";
            btnCancelar.Text = "CANCELAR";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += BtnCancelar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGuardar.Location = new Point(787, 15);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(241, 72);
            btnGuardar.TabIndex = 10;
            btnGuardar.Tag = "btnPrincipalV";
            btnGuardar.Text = "CONFIRMAR PAGO";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += BtnGuardar_Click;
            // 
            // lblDescuento
            // 
            lblDescuento.AutoSize = true;
            lblDescuento.Location = new Point(430, 34);
            lblDescuento.Name = "lblDescuento";
            lblDescuento.Size = new Size(0, 15);
            lblDescuento.TabIndex = 9;
            // 
            // cbMedioPago
            // 
            cbMedioPago.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbMedioPago.DataSource = bindingMedios;
            cbMedioPago.DisplayMember = "Medio";
            cbMedioPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMedioPago.FormattingEnabled = true;
            cbMedioPago.Location = new Point(165, 92);
            cbMedioPago.Name = "cbMedioPago";
            cbMedioPago.Size = new Size(140, 23);
            cbMedioPago.TabIndex = 8;
            cbMedioPago.ValueMember = "IdMedioPago";
            cbMedioPago.SelectedIndexChanged += CbMedioPago_SelectedIndexChanged;
            // 
            // bindingMedios
            // 
            bindingMedios.DataSource = typeof(Entidades_SGBM.MediosPagos);
            // 
            // lblMedioPago
            // 
            lblMedioPago.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMedioPago.AutoSize = true;
            lblMedioPago.Location = new Point(25, 95);
            lblMedioPago.Name = "lblMedioPago";
            lblMedioPago.Size = new Size(93, 15);
            lblMedioPago.TabIndex = 7;
            lblMedioPago.Text = "Medio de Pago: ";
            // 
            // txtTotalAbonado
            // 
            txtTotalAbonado.Location = new Point(165, 38);
            txtTotalAbonado.Name = "txtTotalAbonado";
            txtTotalAbonado.Size = new Size(140, 23);
            txtTotalAbonado.TabIndex = 6;
            txtTotalAbonado.KeyPress += TxtDecimal_KeyPress;
            // 
            // lblTotal
            // 
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(25, 41);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(87, 15);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total Abonado:";
            // 
            // lblFiltro
            // 
            lblFiltro.AutoSize = true;
            lblFiltro.Location = new Point(6, 29);
            lblFiltro.Name = "lblFiltro";
            lblFiltro.Size = new Size(40, 15);
            lblFiltro.TabIndex = 5;
            lblFiltro.Text = "Filtro: ";
            // 
            // pnlVenta
            // 
            pnlVenta.Controls.Add(groupBoxCarrito);
            pnlVenta.Controls.Add(groupBoxSeleccion);
            pnlVenta.Dock = DockStyle.Fill;
            pnlVenta.Location = new Point(0, 96);
            pnlVenta.Name = "pnlVenta";
            pnlVenta.Size = new Size(1043, 385);
            pnlVenta.TabIndex = 2;
            // 
            // groupBoxCarrito
            // 
            groupBoxCarrito.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxCarrito.Controls.Add(lblPorciento);
            groupBoxCarrito.Controls.Add(txtDescuento);
            groupBoxCarrito.Controls.Add(lblDesc);
            groupBoxCarrito.Controls.Add(btnQuitarDetalle);
            groupBoxCarrito.Controls.Add(txtTotalEfectivo);
            groupBoxCarrito.Controls.Add(lblTotalEfectivo);
            groupBoxCarrito.Controls.Add(txtTotalACobrar);
            groupBoxCarrito.Controls.Add(lblTotalLista);
            groupBoxCarrito.Controls.Add(dataGridCarrito);
            groupBoxCarrito.Location = new Point(569, 0);
            groupBoxCarrito.Name = "groupBoxCarrito";
            groupBoxCarrito.Size = new Size(471, 379);
            groupBoxCarrito.TabIndex = 1;
            groupBoxCarrito.TabStop = false;
            groupBoxCarrito.Text = "Carrito de compras";
            // 
            // lblPorciento
            // 
            lblPorciento.AutoSize = true;
            lblPorciento.Location = new Point(160, 30);
            lblPorciento.Name = "lblPorciento";
            lblPorciento.Size = new Size(17, 15);
            lblPorciento.TabIndex = 26;
            lblPorciento.Text = "%";
            // 
            // txtDescuento
            // 
            txtDescuento.Location = new Point(101, 27);
            txtDescuento.Name = "txtDescuento";
            txtDescuento.Size = new Size(53, 23);
            txtDescuento.TabIndex = 25;
            txtDescuento.TextChanged += TxtDescuento_TextChanged;
            txtDescuento.KeyPress += TxtNumerico_KeyPress;
            // 
            // lblDesc
            // 
            lblDesc.AutoSize = true;
            lblDesc.Location = new Point(15, 30);
            lblDesc.Name = "lblDesc";
            lblDesc.Size = new Size(69, 15);
            lblDesc.TabIndex = 24;
            lblDesc.Text = "Descuento: ";
            // 
            // btnQuitarDetalle
            // 
            btnQuitarDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnQuitarDetalle.Location = new Point(384, 26);
            btnQuitarDetalle.Name = "btnQuitarDetalle";
            btnQuitarDetalle.Size = new Size(75, 23);
            btnQuitarDetalle.TabIndex = 23;
            btnQuitarDetalle.Tag = "btnNormalR";
            btnQuitarDetalle.Text = "Quitar";
            btnQuitarDetalle.UseVisualStyleBackColor = true;
            // 
            // txtTotalEfectivo
            // 
            txtTotalEfectivo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtTotalEfectivo.Enabled = false;
            txtTotalEfectivo.Location = new Point(114, 340);
            txtTotalEfectivo.Name = "txtTotalEfectivo";
            txtTotalEfectivo.Size = new Size(117, 23);
            txtTotalEfectivo.TabIndex = 22;
            // 
            // lblTotalEfectivo
            // 
            lblTotalEfectivo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTotalEfectivo.AutoSize = true;
            lblTotalEfectivo.Location = new Point(15, 343);
            lblTotalEfectivo.Name = "lblTotalEfectivo";
            lblTotalEfectivo.Size = new Size(80, 15);
            lblTotalEfectivo.TabIndex = 21;
            lblTotalEfectivo.Text = "Total Efectivo:";
            // 
            // txtTotalACobrar
            // 
            txtTotalACobrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            txtTotalACobrar.Enabled = false;
            txtTotalACobrar.Location = new Point(312, 340);
            txtTotalACobrar.Name = "txtTotalACobrar";
            txtTotalACobrar.Size = new Size(140, 23);
            txtTotalACobrar.TabIndex = 20;
            // 
            // lblTotalLista
            // 
            lblTotalLista.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblTotalLista.AutoSize = true;
            lblTotalLista.Location = new Point(260, 343);
            lblTotalLista.Name = "lblTotalLista";
            lblTotalLista.Size = new Size(35, 15);
            lblTotalLista.TabIndex = 19;
            lblTotalLista.Text = "Total:";
            // 
            // dataGridCarrito
            // 
            dataGridCarrito.AllowUserToAddRows = false;
            dataGridCarrito.AllowUserToDeleteRows = false;
            dataGridCarrito.AllowUserToOrderColumns = true;
            dataGridCarrito.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridCarrito.AutoGenerateColumns = false;
            dataGridCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridCarrito.Columns.AddRange(new DataGridViewColumn[] { descripcionDataGridViewTextBoxColumn, cantidadDataGridViewTextBoxColumn, precioUnitarioDataGridViewTextBoxColumn, InteresDescuento, subTotalDataGridViewTextBoxColumn });
            dataGridCarrito.DataSource = bindingCarrito;
            dataGridCarrito.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridCarrito.Location = new Point(6, 56);
            dataGridCarrito.Name = "dataGridCarrito";
            dataGridCarrito.ReadOnly = true;
            dataGridCarrito.RowHeadersVisible = false;
            dataGridCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridCarrito.Size = new Size(453, 265);
            dataGridCarrito.TabIndex = 0;
            // 
            // descripcionDataGridViewTextBoxColumn
            // 
            descripcionDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            descripcionDataGridViewTextBoxColumn.DataPropertyName = "Descripcion";
            descripcionDataGridViewTextBoxColumn.FillWeight = 35F;
            descripcionDataGridViewTextBoxColumn.HeaderText = "Descripcion";
            descripcionDataGridViewTextBoxColumn.Name = "descripcionDataGridViewTextBoxColumn";
            descripcionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cantidadDataGridViewTextBoxColumn
            // 
            cantidadDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            cantidadDataGridViewTextBoxColumn.DataPropertyName = "Cantidad";
            cantidadDataGridViewTextBoxColumn.FillWeight = 10F;
            cantidadDataGridViewTextBoxColumn.HeaderText = "Cant.";
            cantidadDataGridViewTextBoxColumn.Name = "cantidadDataGridViewTextBoxColumn";
            cantidadDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // precioUnitarioDataGridViewTextBoxColumn
            // 
            precioUnitarioDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            precioUnitarioDataGridViewTextBoxColumn.DataPropertyName = "PrecioUnitario";
            precioUnitarioDataGridViewTextBoxColumn.FillWeight = 20F;
            precioUnitarioDataGridViewTextBoxColumn.HeaderText = "Precio";
            precioUnitarioDataGridViewTextBoxColumn.Name = "precioUnitarioDataGridViewTextBoxColumn";
            precioUnitarioDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // InteresDescuento
            // 
            InteresDescuento.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InteresDescuento.DataPropertyName = "InteresDescuento";
            InteresDescuento.FillWeight = 15F;
            InteresDescuento.HeaderText = "Descuento";
            InteresDescuento.Name = "InteresDescuento";
            InteresDescuento.ReadOnly = true;
            // 
            // subTotalDataGridViewTextBoxColumn
            // 
            subTotalDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            subTotalDataGridViewTextBoxColumn.DataPropertyName = "SubTotal";
            subTotalDataGridViewTextBoxColumn.FillWeight = 20F;
            subTotalDataGridViewTextBoxColumn.HeaderText = "SubTotal";
            subTotalDataGridViewTextBoxColumn.Name = "subTotalDataGridViewTextBoxColumn";
            subTotalDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bindingCarrito
            // 
            bindingCarrito.DataSource = typeof(Entidades_SGBM.DetallesVentas);
            // 
            // groupBoxSeleccion
            // 
            groupBoxSeleccion.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxSeleccion.Controls.Add(checkStock);
            groupBoxSeleccion.Controls.Add(numCantidad);
            groupBoxSeleccion.Controls.Add(lblCantidad);
            groupBoxSeleccion.Controls.Add(btnSeleccionar);
            groupBoxSeleccion.Controls.Add(dataGridSeleccion);
            groupBoxSeleccion.Controls.Add(txtFiltro);
            groupBoxSeleccion.Controls.Add(lblFiltro);
            groupBoxSeleccion.Location = new Point(3, 6);
            groupBoxSeleccion.Name = "groupBoxSeleccion";
            groupBoxSeleccion.Size = new Size(566, 373);
            groupBoxSeleccion.TabIndex = 0;
            groupBoxSeleccion.TabStop = false;
            groupBoxSeleccion.Text = "Selección de Servicios - Productos";
            // 
            // checkStock
            // 
            checkStock.AutoSize = true;
            checkStock.Location = new Point(474, 28);
            checkStock.Name = "checkStock";
            checkStock.Size = new Size(74, 19);
            checkStock.TabIndex = 16;
            checkStock.Text = "Sin Stock";
            checkStock.UseVisualStyleBackColor = true;
            checkStock.CheckedChanged += checkStock_CheckedChanged;
            // 
            // numCantidad
            // 
            numCantidad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            numCantidad.Location = new Point(91, 335);
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(58, 23);
            numCantidad.TabIndex = 15;
            numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblCantidad
            // 
            lblCantidad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblCantidad.AutoSize = true;
            lblCantidad.Location = new Point(9, 337);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(58, 15);
            lblCantidad.TabIndex = 14;
            lblCantidad.Text = "Cantidad:";
            // 
            // btnSeleccionar
            // 
            btnSeleccionar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSeleccionar.Location = new Point(162, 334);
            btnSeleccionar.Name = "btnSeleccionar";
            btnSeleccionar.Size = new Size(398, 33);
            btnSeleccionar.TabIndex = 12;
            btnSeleccionar.Tag = "btnNormalV";
            btnSeleccionar.Text = "AÑADIR AL CARRITO";
            btnSeleccionar.UseVisualStyleBackColor = true;
            btnSeleccionar.Click += BtnSeleccionar_Click;
            // 
            // dataGridSeleccion
            // 
            dataGridSeleccion.AllowUserToAddRows = false;
            dataGridSeleccion.AllowUserToDeleteRows = false;
            dataGridSeleccion.AllowUserToOrderColumns = true;
            dataGridSeleccion.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridSeleccion.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridSeleccion.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridSeleccion.Location = new Point(6, 70);
            dataGridSeleccion.MultiSelect = false;
            dataGridSeleccion.Name = "dataGridSeleccion";
            dataGridSeleccion.ReadOnly = true;
            dataGridSeleccion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridSeleccion.Size = new Size(554, 245);
            dataGridSeleccion.TabIndex = 13;
            // 
            // txtFiltro
            // 
            txtFiltro.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFiltro.Location = new Point(64, 26);
            txtFiltro.Name = "txtFiltro";
            txtFiltro.Size = new Size(373, 23);
            txtFiltro.TabIndex = 12;
            txtFiltro.TextChanged += Filtro_TextChanged;
            // 
            // errorProvider1
            // 
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider1.ContainerControl = this;
            // 
            // lblNroVtaTitulo
            // 
            lblNroVtaTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblNroVtaTitulo.AutoSize = true;
            lblNroVtaTitulo.Location = new Point(808, 20);
            lblNroVtaTitulo.Name = "lblNroVtaTitulo";
            lblNroVtaTitulo.Size = new Size(56, 15);
            lblNroVtaTitulo.TabIndex = 6;
            lblNroVtaTitulo.Text = "Venta N°:";
            // 
            // lblNroVta
            // 
            lblNroVta.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblNroVta.AutoSize = true;
            lblNroVta.Location = new Point(881, 20);
            lblNroVta.Name = "lblNroVta";
            lblNroVta.Size = new Size(0, 15);
            lblNroVta.TabIndex = 7;
            // 
            // FrmEditVentas
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1043, 636);
            Controls.Add(pnlVenta);
            Controls.Add(panel1);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(980, 588);
            Name = "FrmEditVentas";
            Load += FrmEditVentas_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingEmpleados).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingMedios).EndInit();
            pnlVenta.ResumeLayout(false);
            groupBoxCarrito.ResumeLayout(false);
            groupBoxCarrito.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridCarrito).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingCarrito).EndInit();
            groupBoxSeleccion.ResumeLayout(false);
            groupBoxSeleccion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridSeleccion).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitulo;
        private TextBox txtDni;
        private ComboBox cbVendedor;
        private Label lblVendedor;
        private TextBox txtNombreCompleto;
        private Button btnBuscarCliente;
        private Panel panel1;
        private Label lblTotal;
        private TextBox txtTotalAbonado;
        private Label lblDescuento;
        private ComboBox cbMedioPago;
        private Label lblMedioPago;
        private Button btnCancelar;
        private Button btnGuardar;
        private Label lblFiltro;
        private Panel pnlVenta;
        private GroupBox groupBoxCarrito;
        private GroupBox groupBoxSeleccion;
        private Button btnSeleccionar;
        private DataGridView dataGridSeleccion;
        private TextBox txtFiltro;
        private DataGridView dataGridCarrito;
        private NumericUpDown numCantidad;
        private Label lblCantidad;
        private TextBox txtTotalACobrar;
        private Label lblTotalLista;
        private Label lblMensaje;
        private TextBox txtTotalEfectivo;
        private Label lblTotalEfectivo;
        private BindingSource bindingEmpleados;
        private BindingSource bindingMedios;
        private DataGridViewTextBoxColumn fondosMembresiasDataGridViewTextBoxColumn;
        private CheckBox checkStock;
        private ErrorProvider errorProvider1;
        private BindingSource bindingCarrito;
        private Button btnQuitarDetalle;
        private TextBox txtDescuento;
        private Label lblDesc;
        private Label lblPorciento;
        private DataGridViewTextBoxColumn descripcionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantidadDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn precioUnitarioDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn InteresDescuento;
        private DataGridViewTextBoxColumn subTotalDataGridViewTextBoxColumn;
        private Label lblNroVta;
        private Label lblNroVtaTitulo;
    }
}