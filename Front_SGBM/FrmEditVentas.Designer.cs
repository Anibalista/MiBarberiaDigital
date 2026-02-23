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
            lblVendedor = new Label();
            txtNombreCompleto = new TextBox();
            btnBuscarCliente = new Button();
            txtDni = new TextBox();
            lblTitulo = new Label();
            panel1 = new Panel();
            lblTotal = new Label();
            txtTotalAbonado = new TextBox();
            cbMedioPago = new ComboBox();
            lblMedioPago = new Label();
            lblDescuento = new Label();
            btnGuardar = new Button();
            btnCancelar = new Button();
            lblFiltro = new Label();
            pnlVenta = new Panel();
            groupBoxSeleccion = new GroupBox();
            groupBoxCarrito = new GroupBox();
            textBox1 = new TextBox();
            dataGridSeleccion = new DataGridView();
            btnSeleccionar = new Button();
            dataGridCarrito = new DataGridView();
            lblCantidad = new Label();
            numCantidad = new NumericUpDown();
            txtTotalACobrar = new TextBox();
            lblTotalAPagar = new Label();
            txtSubTotal = new TextBox();
            lblSubTotal = new Label();
            lblMensaje = new Label();
            bindingCarrito = new BindingSource(components);
            bindingEmpleados = new BindingSource(components);
            bindingMedios = new BindingSource(components);
            descripcionDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            cantidadDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            Column1 = new DataGridViewTextBoxColumn();
            precioUnitarioDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            TotalDetalle = new DataGridViewTextBoxColumn();
            btnEliminar = new DataGridViewButtonColumn();
            idProductoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            idServicioDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            idFondoMembresiaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            productosDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            serviciosDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            fondosMembresiasDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            panelHeader.SuspendLayout();
            panel1.SuspendLayout();
            pnlVenta.SuspendLayout();
            groupBoxSeleccion.SuspendLayout();
            groupBoxCarrito.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridSeleccion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridCarrito).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCantidad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingCarrito).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingEmpleados).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingMedios).BeginInit();
            SuspendLayout();
            // 
            // panelHeader
            // 
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
            // 
            // txtDni
            // 
            txtDni.Location = new Point(12, 51);
            txtDni.Name = "txtDni";
            txtDni.PlaceholderText = "DNI DEL CLIENTE";
            txtDni.Size = new Size(140, 23);
            txtDni.TabIndex = 1;
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
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(25, 26);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(87, 15);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total Abonado:";
            // 
            // txtTotalAbonado
            // 
            txtTotalAbonado.Location = new Point(165, 23);
            txtTotalAbonado.Name = "txtTotalAbonado";
            txtTotalAbonado.Size = new Size(140, 23);
            txtTotalAbonado.TabIndex = 6;
            // 
            // cbMedioPago
            // 
            cbMedioPago.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbMedioPago.DataSource = bindingMedios;
            cbMedioPago.DisplayMember = "Medio";
            cbMedioPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMedioPago.FormattingEnabled = true;
            cbMedioPago.Location = new Point(165, 77);
            cbMedioPago.Name = "cbMedioPago";
            cbMedioPago.Size = new Size(140, 23);
            cbMedioPago.TabIndex = 8;
            cbMedioPago.ValueMember = "IdMedioPago";
            // 
            // lblMedioPago
            // 
            lblMedioPago.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblMedioPago.AutoSize = true;
            lblMedioPago.Location = new Point(25, 80);
            lblMedioPago.Name = "lblMedioPago";
            lblMedioPago.Size = new Size(93, 15);
            lblMedioPago.TabIndex = 7;
            lblMedioPago.Text = "Medio de Pago: ";
            // 
            // lblDescuento
            // 
            lblDescuento.AutoSize = true;
            lblDescuento.Location = new Point(430, 26);
            lblDescuento.Name = "lblDescuento";
            lblDescuento.Size = new Size(0, 15);
            lblDescuento.TabIndex = 9;
            // 
            // btnGuardar
            // 
            btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnGuardar.Location = new Point(787, 15);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(241, 85);
            btnGuardar.TabIndex = 10;
            btnGuardar.Tag = "btnPrincipalV";
            btnGuardar.Text = "CONFIRMAR PAGO";
            btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCancelar.Location = new Point(787, 106);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(241, 37);
            btnCancelar.TabIndex = 11;
            btnCancelar.Tag = "btnNormalR";
            btnCancelar.Text = "CANCELAR";
            btnCancelar.UseVisualStyleBackColor = true;
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
            // groupBoxSeleccion
            // 
            groupBoxSeleccion.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxSeleccion.Controls.Add(numCantidad);
            groupBoxSeleccion.Controls.Add(lblCantidad);
            groupBoxSeleccion.Controls.Add(btnSeleccionar);
            groupBoxSeleccion.Controls.Add(dataGridSeleccion);
            groupBoxSeleccion.Controls.Add(textBox1);
            groupBoxSeleccion.Controls.Add(lblFiltro);
            groupBoxSeleccion.Location = new Point(3, 6);
            groupBoxSeleccion.Name = "groupBoxSeleccion";
            groupBoxSeleccion.Size = new Size(361, 373);
            groupBoxSeleccion.TabIndex = 0;
            groupBoxSeleccion.TabStop = false;
            groupBoxSeleccion.Text = "Selección de Servicios - Productos";
            // 
            // groupBoxCarrito
            // 
            groupBoxCarrito.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxCarrito.Controls.Add(txtSubTotal);
            groupBoxCarrito.Controls.Add(lblSubTotal);
            groupBoxCarrito.Controls.Add(txtTotalACobrar);
            groupBoxCarrito.Controls.Add(lblTotalAPagar);
            groupBoxCarrito.Controls.Add(dataGridCarrito);
            groupBoxCarrito.Location = new Point(370, 0);
            groupBoxCarrito.Name = "groupBoxCarrito";
            groupBoxCarrito.Size = new Size(670, 379);
            groupBoxCarrito.TabIndex = 1;
            groupBoxCarrito.TabStop = false;
            groupBoxCarrito.Text = "Carrito de compras";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(64, 26);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(291, 23);
            textBox1.TabIndex = 12;
            // 
            // dataGridSeleccion
            // 
            dataGridSeleccion.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridSeleccion.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridSeleccion.Location = new Point(6, 70);
            dataGridSeleccion.Name = "dataGridSeleccion";
            dataGridSeleccion.Size = new Size(349, 245);
            dataGridSeleccion.TabIndex = 13;
            // 
            // btnSeleccionar
            // 
            btnSeleccionar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSeleccionar.Location = new Point(162, 321);
            btnSeleccionar.Name = "btnSeleccionar";
            btnSeleccionar.Size = new Size(193, 46);
            btnSeleccionar.TabIndex = 12;
            btnSeleccionar.Tag = "btnNormalV";
            btnSeleccionar.Text = "AÑADIR AL CARRITO";
            btnSeleccionar.UseVisualStyleBackColor = true;
            // 
            // dataGridCarrito
            // 
            dataGridCarrito.AllowUserToAddRows = false;
            dataGridCarrito.AllowUserToDeleteRows = false;
            dataGridCarrito.AllowUserToOrderColumns = true;
            dataGridCarrito.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridCarrito.AutoGenerateColumns = false;
            dataGridCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridCarrito.Columns.AddRange(new DataGridViewColumn[] { descripcionDataGridViewTextBoxColumn, cantidadDataGridViewTextBoxColumn, Column1, precioUnitarioDataGridViewTextBoxColumn, TotalDetalle, btnEliminar, idProductoDataGridViewTextBoxColumn, idServicioDataGridViewTextBoxColumn, idFondoMembresiaDataGridViewTextBoxColumn, productosDataGridViewTextBoxColumn, serviciosDataGridViewTextBoxColumn, fondosMembresiasDataGridViewTextBoxColumn });
            dataGridCarrito.DataSource = bindingCarrito;
            dataGridCarrito.Location = new Point(6, 22);
            dataGridCarrito.Name = "dataGridCarrito";
            dataGridCarrito.RowHeadersVisible = false;
            dataGridCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridCarrito.Size = new Size(652, 299);
            dataGridCarrito.TabIndex = 0;
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
            // numCantidad
            // 
            numCantidad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            numCantidad.Location = new Point(91, 335);
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(58, 23);
            numCantidad.TabIndex = 15;
            numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // txtTotalACobrar
            // 
            txtTotalACobrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            txtTotalACobrar.Location = new Point(511, 340);
            txtTotalACobrar.Name = "txtTotalACobrar";
            txtTotalACobrar.Size = new Size(140, 23);
            txtTotalACobrar.TabIndex = 20;
            // 
            // lblTotalAPagar
            // 
            lblTotalAPagar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblTotalAPagar.AutoSize = true;
            lblTotalAPagar.Location = new Point(400, 343);
            lblTotalAPagar.Name = "lblTotalAPagar";
            lblTotalAPagar.Size = new Size(85, 15);
            lblTotalAPagar.TabIndex = 19;
            lblTotalAPagar.Text = "Total A Cobrar:";
            // 
            // txtSubTotal
            // 
            txtSubTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtSubTotal.Enabled = false;
            txtSubTotal.Location = new Point(91, 340);
            txtSubTotal.Name = "txtSubTotal";
            txtSubTotal.Size = new Size(140, 23);
            txtSubTotal.TabIndex = 22;
            // 
            // lblSubTotal
            // 
            lblSubTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblSubTotal.AutoSize = true;
            lblSubTotal.Location = new Point(15, 343);
            lblSubTotal.Name = "lblSubTotal";
            lblSubTotal.Size = new Size(55, 15);
            lblSubTotal.TabIndex = 21;
            lblSubTotal.Text = "SubTotal:";
            // 
            // lblMensaje
            // 
            lblMensaje.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMensaje.AutoSize = true;
            lblMensaje.Location = new Point(385, 80);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Size = new Size(0, 15);
            lblMensaje.TabIndex = 12;
            // 
            // bindingCarrito
            // 
            bindingCarrito.DataSource = typeof(Entidades_SGBM.DetallesVentas);
            // 
            // bindingEmpleados
            // 
            bindingEmpleados.DataSource = typeof(Entidades_SGBM.Empleados);
            // 
            // bindingMedios
            // 
            bindingMedios.DataSource = typeof(Entidades_SGBM.MediosPagos);
            // 
            // descripcionDataGridViewTextBoxColumn
            // 
            descripcionDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            descripcionDataGridViewTextBoxColumn.DataPropertyName = "Descripcion";
            descripcionDataGridViewTextBoxColumn.FillWeight = 40F;
            descripcionDataGridViewTextBoxColumn.HeaderText = "Descripcion";
            descripcionDataGridViewTextBoxColumn.Name = "descripcionDataGridViewTextBoxColumn";
            // 
            // cantidadDataGridViewTextBoxColumn
            // 
            cantidadDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            cantidadDataGridViewTextBoxColumn.DataPropertyName = "Cantidad";
            cantidadDataGridViewTextBoxColumn.FillWeight = 10F;
            cantidadDataGridViewTextBoxColumn.HeaderText = "Cantidad";
            cantidadDataGridViewTextBoxColumn.Name = "cantidadDataGridViewTextBoxColumn";
            // 
            // Column1
            // 
            Column1.DataPropertyName = "IdDetalleVenta";
            Column1.HeaderText = "Id";
            Column1.Name = "Column1";
            Column1.Visible = false;
            // 
            // precioUnitarioDataGridViewTextBoxColumn
            // 
            precioUnitarioDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            precioUnitarioDataGridViewTextBoxColumn.DataPropertyName = "PrecioUnitario";
            precioUnitarioDataGridViewTextBoxColumn.FillWeight = 20F;
            precioUnitarioDataGridViewTextBoxColumn.HeaderText = "Precio Unitario";
            precioUnitarioDataGridViewTextBoxColumn.Name = "precioUnitarioDataGridViewTextBoxColumn";
            // 
            // TotalDetalle
            // 
            TotalDetalle.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TotalDetalle.FillWeight = 20F;
            TotalDetalle.HeaderText = "Totales";
            TotalDetalle.Name = "TotalDetalle";
            // 
            // btnEliminar
            // 
            btnEliminar.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            btnEliminar.FillWeight = 10F;
            btnEliminar.HeaderText = "Quitar";
            btnEliminar.MinimumWidth = 50;
            btnEliminar.Name = "btnEliminar";
            // 
            // idProductoDataGridViewTextBoxColumn
            // 
            idProductoDataGridViewTextBoxColumn.DataPropertyName = "IdProducto";
            idProductoDataGridViewTextBoxColumn.HeaderText = "IdProducto";
            idProductoDataGridViewTextBoxColumn.Name = "idProductoDataGridViewTextBoxColumn";
            idProductoDataGridViewTextBoxColumn.Visible = false;
            // 
            // idServicioDataGridViewTextBoxColumn
            // 
            idServicioDataGridViewTextBoxColumn.DataPropertyName = "IdServicio";
            idServicioDataGridViewTextBoxColumn.HeaderText = "IdServicio";
            idServicioDataGridViewTextBoxColumn.Name = "idServicioDataGridViewTextBoxColumn";
            idServicioDataGridViewTextBoxColumn.Visible = false;
            // 
            // idFondoMembresiaDataGridViewTextBoxColumn
            // 
            idFondoMembresiaDataGridViewTextBoxColumn.DataPropertyName = "IdFondoMembresia";
            idFondoMembresiaDataGridViewTextBoxColumn.HeaderText = "IdFondoMembresia";
            idFondoMembresiaDataGridViewTextBoxColumn.Name = "idFondoMembresiaDataGridViewTextBoxColumn";
            idFondoMembresiaDataGridViewTextBoxColumn.Visible = false;
            // 
            // productosDataGridViewTextBoxColumn
            // 
            productosDataGridViewTextBoxColumn.DataPropertyName = "Productos";
            productosDataGridViewTextBoxColumn.HeaderText = "Productos";
            productosDataGridViewTextBoxColumn.Name = "productosDataGridViewTextBoxColumn";
            productosDataGridViewTextBoxColumn.Visible = false;
            // 
            // serviciosDataGridViewTextBoxColumn
            // 
            serviciosDataGridViewTextBoxColumn.DataPropertyName = "Servicios";
            serviciosDataGridViewTextBoxColumn.HeaderText = "Servicios";
            serviciosDataGridViewTextBoxColumn.Name = "serviciosDataGridViewTextBoxColumn";
            serviciosDataGridViewTextBoxColumn.Visible = false;
            // 
            // fondosMembresiasDataGridViewTextBoxColumn
            // 
            fondosMembresiasDataGridViewTextBoxColumn.DataPropertyName = "FondosMembresias";
            fondosMembresiasDataGridViewTextBoxColumn.HeaderText = "FondosMembresias";
            fondosMembresiasDataGridViewTextBoxColumn.Name = "fondosMembresiasDataGridViewTextBoxColumn";
            fondosMembresiasDataGridViewTextBoxColumn.Visible = false;
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
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            pnlVenta.ResumeLayout(false);
            groupBoxSeleccion.ResumeLayout(false);
            groupBoxSeleccion.PerformLayout();
            groupBoxCarrito.ResumeLayout(false);
            groupBoxCarrito.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridSeleccion).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridCarrito).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCantidad).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingCarrito).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingEmpleados).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingMedios).EndInit();
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
        private TextBox textBox1;
        private DataGridView dataGridCarrito;
        private NumericUpDown numCantidad;
        private Label lblCantidad;
        private TextBox txtTotalACobrar;
        private Label lblTotalAPagar;
        private Label lblMensaje;
        private TextBox txtSubTotal;
        private Label lblSubTotal;
        private BindingSource bindingEmpleados;
        private BindingSource bindingCarrito;
        private BindingSource bindingMedios;
        private DataGridViewTextBoxColumn descripcionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantidadDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn precioUnitarioDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn TotalDetalle;
        private DataGridViewButtonColumn btnEliminar;
        private DataGridViewTextBoxColumn idProductoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn idServicioDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn idFondoMembresiaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn productosDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn serviciosDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fondosMembresiasDataGridViewTextBoxColumn;
    }
}