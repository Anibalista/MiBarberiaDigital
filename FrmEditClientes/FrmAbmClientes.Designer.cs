namespace Front_SGBM
{
    partial class FrmAbmClientes
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
            flowLayoutPanel1 = new FlowLayoutPanel();
            groupBoxTitulo = new GroupBox();
            labelTitulo = new Label();
            groupBox1 = new GroupBox();
            checkAnulados = new CheckBox();
            btnRegistrar = new Button();
            btnBuscar = new Button();
            btnImportar = new Button();
            btnExportar = new Button();
            cbBusqueda = new ComboBox();
            label2 = new Label();
            groupBox2 = new GroupBox();
            cbLocalidad = new ComboBox();
            bindingLocalidades = new BindingSource(components);
            labelLocalidad = new Label();
            labelCampo2 = new Label();
            txtCampo2 = new TextBox();
            labelCampo1 = new Label();
            txtCampo1 = new TextBox();
            groupBox3 = new GroupBox();
            dataGridClientes = new DataGridView();
            apellidosDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            nombresDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dniDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            fechaNacDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            Domicilio = new DataGridViewTextBoxColumn();
            Localidad = new DataGridViewTextBoxColumn();
            bindingClientes = new BindingSource(components);
            groupBox4 = new GroupBox();
            dataGridContactos = new DataGridView();
            whatsappDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            telefonoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            instagramDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            facebookDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            emailDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            contactosBindingSource = new BindingSource(components);
            groupBoxBotones = new GroupBox();
            btnSeleccionar = new Button();
            btnSalir = new Button();
            flowLayoutPanel1.SuspendLayout();
            groupBoxTitulo.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingLocalidades).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridClientes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingClientes).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridContactos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)contactosBindingSource).BeginInit();
            groupBoxBotones.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(groupBoxTitulo);
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Controls.Add(groupBox3);
            flowLayoutPanel1.Controls.Add(groupBox4);
            flowLayoutPanel1.Controls.Add(groupBoxBotones);
            flowLayoutPanel1.Location = new Point(2, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(959, 632);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBoxTitulo
            // 
            groupBoxTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxTitulo.Controls.Add(labelTitulo);
            groupBoxTitulo.Location = new Point(3, 3);
            groupBoxTitulo.Name = "groupBoxTitulo";
            groupBoxTitulo.Size = new Size(950, 55);
            groupBoxTitulo.TabIndex = 0;
            groupBoxTitulo.TabStop = false;
            // 
            // labelTitulo
            // 
            labelTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelTitulo.AutoSize = true;
            labelTitulo.Font = new Font("Roboto Condensed", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTitulo.Location = new Point(365, 19);
            labelTitulo.Name = "labelTitulo";
            labelTitulo.Size = new Size(142, 29);
            labelTitulo.TabIndex = 1;
            labelTitulo.Text = "ABM Clientes";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(checkAnulados);
            groupBox1.Controls.Add(btnRegistrar);
            groupBox1.Controls.Add(btnBuscar);
            groupBox1.Controls.Add(btnImportar);
            groupBox1.Controls.Add(btnExportar);
            groupBox1.Controls.Add(cbBusqueda);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(3, 64);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(950, 73);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filtros y Archivos";
            // 
            // checkAnulados
            // 
            checkAnulados.AutoSize = true;
            checkAnulados.Location = new Point(339, 35);
            checkAnulados.Name = "checkAnulados";
            checkAnulados.Size = new Size(112, 19);
            checkAnulados.TabIndex = 7;
            checkAnulados.Text = "Incluir Anulados";
            checkAnulados.UseVisualStyleBackColor = true;
            // 
            // btnRegistrar
            // 
            btnRegistrar.Location = new Point(512, 33);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new Size(75, 23);
            btnRegistrar.TabIndex = 6;
            btnRegistrar.Text = "Nuevo";
            btnRegistrar.UseVisualStyleBackColor = true;
            btnRegistrar.Click += btnRegistrar_Click;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(260, 33);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(60, 22);
            btnBuscar.TabIndex = 5;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // btnImportar
            // 
            btnImportar.Location = new Point(709, 33);
            btnImportar.Name = "btnImportar";
            btnImportar.Size = new Size(109, 23);
            btnImportar.TabIndex = 4;
            btnImportar.Text = "Importar Clientes";
            btnImportar.UseVisualStyleBackColor = true;
            btnImportar.Click += btnImportar_Click;
            // 
            // btnExportar
            // 
            btnExportar.Location = new Point(824, 33);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(109, 23);
            btnExportar.TabIndex = 3;
            btnExportar.Text = "Exportar a Listado";
            btnExportar.UseVisualStyleBackColor = true;
            btnExportar.Click += btnExportar_Click;
            // 
            // cbBusqueda
            // 
            cbBusqueda.DropDownStyle = ComboBoxStyle.DropDownList;
            cbBusqueda.FormattingEnabled = true;
            cbBusqueda.Location = new Point(84, 33);
            cbBusqueda.Name = "cbBusqueda";
            cbBusqueda.Size = new Size(170, 23);
            cbBusqueda.TabIndex = 2;
            cbBusqueda.TextChanged += cbBusqueda_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 36);
            label2.Name = "label2";
            label2.Size = new Size(69, 15);
            label2.TabIndex = 1;
            label2.Text = "Buscar por: ";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(cbLocalidad);
            groupBox2.Controls.Add(labelLocalidad);
            groupBox2.Controls.Add(labelCampo2);
            groupBox2.Controls.Add(txtCampo2);
            groupBox2.Controls.Add(labelCampo1);
            groupBox2.Controls.Add(txtCampo1);
            groupBox2.Location = new Point(3, 143);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(950, 55);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Campos de búsqueda";
            // 
            // cbLocalidad
            // 
            cbLocalidad.DataSource = bindingLocalidades;
            cbLocalidad.DisplayMember = "localidadCompleta";
            cbLocalidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLocalidad.FormattingEnabled = true;
            cbLocalidad.Location = new Point(728, 22);
            cbLocalidad.Name = "cbLocalidad";
            cbLocalidad.Size = new Size(205, 23);
            cbLocalidad.TabIndex = 10;
            cbLocalidad.ValueMember = "IdLocalidad";
            cbLocalidad.VisibleChanged += cbLocalidad_VisibleChanged;
            // 
            // bindingLocalidades
            // 
            bindingLocalidades.DataSource = typeof(Entidades_SGBM.Localidades);
            // 
            // labelLocalidad
            // 
            labelLocalidad.AutoSize = true;
            labelLocalidad.Location = new Point(653, 25);
            labelLocalidad.Name = "labelLocalidad";
            labelLocalidad.Size = new Size(58, 15);
            labelLocalidad.TabIndex = 9;
            labelLocalidad.Text = "Localidad";
            // 
            // labelCampo2
            // 
            labelCampo2.AutoSize = true;
            labelCampo2.Location = new Point(339, 25);
            labelCampo2.Name = "labelCampo2";
            labelCampo2.Size = new Size(56, 15);
            labelCampo2.TabIndex = 8;
            labelCampo2.Text = "Nombres";
            // 
            // txtCampo2
            // 
            txtCampo2.Location = new Point(417, 22);
            txtCampo2.MaxLength = 149;
            txtCampo2.Name = "txtCampo2";
            txtCampo2.Size = new Size(170, 23);
            txtCampo2.TabIndex = 7;
            // 
            // labelCampo1
            // 
            labelCampo1.AutoSize = true;
            labelCampo1.Location = new Point(9, 25);
            labelCampo1.Name = "labelCampo1";
            labelCampo1.Size = new Size(25, 15);
            labelCampo1.TabIndex = 6;
            labelCampo1.Text = "Dni";
            // 
            // txtCampo1
            // 
            txtCampo1.Location = new Point(84, 22);
            txtCampo1.MaxLength = 149;
            txtCampo1.Name = "txtCampo1";
            txtCampo1.Size = new Size(170, 23);
            txtCampo1.TabIndex = 5;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(dataGridClientes);
            groupBox3.Location = new Point(3, 204);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(950, 212);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Clientes";
            // 
            // dataGridClientes
            // 
            dataGridClientes.AllowUserToAddRows = false;
            dataGridClientes.AllowUserToDeleteRows = false;
            dataGridClientes.AllowUserToOrderColumns = true;
            dataGridClientes.AutoGenerateColumns = false;
            dataGridClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridClientes.Columns.AddRange(new DataGridViewColumn[] { apellidosDataGridViewTextBoxColumn, nombresDataGridViewTextBoxColumn, dniDataGridViewTextBoxColumn, fechaNacDataGridViewTextBoxColumn, Domicilio, Localidad });
            dataGridClientes.DataSource = bindingClientes;
            dataGridClientes.Location = new Point(0, 22);
            dataGridClientes.Name = "dataGridClientes";
            dataGridClientes.ReadOnly = true;
            dataGridClientes.RowHeadersVisible = false;
            dataGridClientes.Size = new Size(944, 184);
            dataGridClientes.TabIndex = 0;
            dataGridClientes.CurrentCellChanged += bindingClientes_CurrentChanged;
            // 
            // apellidosDataGridViewTextBoxColumn
            // 
            apellidosDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            apellidosDataGridViewTextBoxColumn.DataPropertyName = "Apellidos";
            apellidosDataGridViewTextBoxColumn.FillWeight = 15F;
            apellidosDataGridViewTextBoxColumn.HeaderText = "Apellidos";
            apellidosDataGridViewTextBoxColumn.MinimumWidth = 120;
            apellidosDataGridViewTextBoxColumn.Name = "apellidosDataGridViewTextBoxColumn";
            apellidosDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nombresDataGridViewTextBoxColumn
            // 
            nombresDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            nombresDataGridViewTextBoxColumn.DataPropertyName = "Nombres";
            nombresDataGridViewTextBoxColumn.FillWeight = 15F;
            nombresDataGridViewTextBoxColumn.HeaderText = "Nombres";
            nombresDataGridViewTextBoxColumn.MinimumWidth = 120;
            nombresDataGridViewTextBoxColumn.Name = "nombresDataGridViewTextBoxColumn";
            nombresDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dniDataGridViewTextBoxColumn
            // 
            dniDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dniDataGridViewTextBoxColumn.DataPropertyName = "Dni";
            dniDataGridViewTextBoxColumn.FillWeight = 9F;
            dniDataGridViewTextBoxColumn.HeaderText = "Dni";
            dniDataGridViewTextBoxColumn.MinimumWidth = 80;
            dniDataGridViewTextBoxColumn.Name = "dniDataGridViewTextBoxColumn";
            dniDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // fechaNacDataGridViewTextBoxColumn
            // 
            fechaNacDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fechaNacDataGridViewTextBoxColumn.DataPropertyName = "FechaNac";
            fechaNacDataGridViewTextBoxColumn.FillWeight = 11F;
            fechaNacDataGridViewTextBoxColumn.HeaderText = "FechaNac";
            fechaNacDataGridViewTextBoxColumn.MinimumWidth = 100;
            fechaNacDataGridViewTextBoxColumn.Name = "fechaNacDataGridViewTextBoxColumn";
            fechaNacDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Domicilio
            // 
            Domicilio.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Domicilio.FillWeight = 35F;
            Domicilio.HeaderText = "Domicilio";
            Domicilio.MinimumWidth = 200;
            Domicilio.Name = "Domicilio";
            Domicilio.ReadOnly = true;
            // 
            // Localidad
            // 
            Localidad.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Localidad.FillWeight = 15F;
            Localidad.HeaderText = "Localidad";
            Localidad.MinimumWidth = 120;
            Localidad.Name = "Localidad";
            Localidad.ReadOnly = true;
            // 
            // bindingClientes
            // 
            bindingClientes.DataSource = typeof(Entidades_SGBM.Personas);
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.Controls.Add(dataGridContactos);
            groupBox4.Location = new Point(3, 422);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(950, 108);
            groupBox4.TabIndex = 4;
            groupBox4.TabStop = false;
            groupBox4.Text = "Contactos";
            // 
            // dataGridContactos
            // 
            dataGridContactos.AllowUserToAddRows = false;
            dataGridContactos.AllowUserToDeleteRows = false;
            dataGridContactos.AllowUserToOrderColumns = true;
            dataGridContactos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dataGridContactos.AutoGenerateColumns = false;
            dataGridContactos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridContactos.Columns.AddRange(new DataGridViewColumn[] { whatsappDataGridViewTextBoxColumn, telefonoDataGridViewTextBoxColumn, instagramDataGridViewTextBoxColumn, facebookDataGridViewTextBoxColumn, emailDataGridViewTextBoxColumn });
            dataGridContactos.DataSource = contactosBindingSource;
            dataGridContactos.Location = new Point(0, 22);
            dataGridContactos.Name = "dataGridContactos";
            dataGridContactos.ReadOnly = true;
            dataGridContactos.RowHeadersVisible = false;
            dataGridContactos.Size = new Size(947, 80);
            dataGridContactos.TabIndex = 0;
            // 
            // whatsappDataGridViewTextBoxColumn
            // 
            whatsappDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            whatsappDataGridViewTextBoxColumn.DataPropertyName = "Whatsapp";
            whatsappDataGridViewTextBoxColumn.FillWeight = 16F;
            whatsappDataGridViewTextBoxColumn.HeaderText = "Whatsapp";
            whatsappDataGridViewTextBoxColumn.MinimumWidth = 100;
            whatsappDataGridViewTextBoxColumn.Name = "whatsappDataGridViewTextBoxColumn";
            whatsappDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // telefonoDataGridViewTextBoxColumn
            // 
            telefonoDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            telefonoDataGridViewTextBoxColumn.DataPropertyName = "Telefono";
            telefonoDataGridViewTextBoxColumn.FillWeight = 16F;
            telefonoDataGridViewTextBoxColumn.HeaderText = "Telefono";
            telefonoDataGridViewTextBoxColumn.MinimumWidth = 100;
            telefonoDataGridViewTextBoxColumn.Name = "telefonoDataGridViewTextBoxColumn";
            telefonoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // instagramDataGridViewTextBoxColumn
            // 
            instagramDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            instagramDataGridViewTextBoxColumn.DataPropertyName = "Instagram";
            instagramDataGridViewTextBoxColumn.FillWeight = 20F;
            instagramDataGridViewTextBoxColumn.HeaderText = "Instagram";
            instagramDataGridViewTextBoxColumn.MinimumWidth = 150;
            instagramDataGridViewTextBoxColumn.Name = "instagramDataGridViewTextBoxColumn";
            instagramDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // facebookDataGridViewTextBoxColumn
            // 
            facebookDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            facebookDataGridViewTextBoxColumn.DataPropertyName = "Facebook";
            facebookDataGridViewTextBoxColumn.FillWeight = 20F;
            facebookDataGridViewTextBoxColumn.HeaderText = "Facebook";
            facebookDataGridViewTextBoxColumn.MinimumWidth = 180;
            facebookDataGridViewTextBoxColumn.Name = "facebookDataGridViewTextBoxColumn";
            facebookDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // emailDataGridViewTextBoxColumn
            // 
            emailDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            emailDataGridViewTextBoxColumn.DataPropertyName = "Email";
            emailDataGridViewTextBoxColumn.FillWeight = 28F;
            emailDataGridViewTextBoxColumn.HeaderText = "Email";
            emailDataGridViewTextBoxColumn.MinimumWidth = 200;
            emailDataGridViewTextBoxColumn.Name = "emailDataGridViewTextBoxColumn";
            emailDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // contactosBindingSource
            // 
            contactosBindingSource.DataSource = typeof(Entidades_SGBM.Contactos);
            // 
            // groupBoxBotones
            // 
            groupBoxBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxBotones.Controls.Add(btnSeleccionar);
            groupBoxBotones.Controls.Add(btnSalir);
            groupBoxBotones.Location = new Point(3, 536);
            groupBoxBotones.Name = "groupBoxBotones";
            groupBoxBotones.Size = new Size(950, 92);
            groupBoxBotones.TabIndex = 5;
            groupBoxBotones.TabStop = false;
            // 
            // btnSeleccionar
            // 
            btnSeleccionar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSeleccionar.Location = new Point(176, 34);
            btnSeleccionar.Name = "btnSeleccionar";
            btnSeleccionar.Size = new Size(146, 42);
            btnSeleccionar.TabIndex = 3;
            btnSeleccionar.Text = "Seleccionar";
            btnSeleccionar.UseVisualStyleBackColor = true;
            btnSeleccionar.Click += btnSeleccionar_Click;
            // 
            // btnSalir
            // 
            btnSalir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalir.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSalir.Location = new Point(645, 34);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(146, 42);
            btnSalir.TabIndex = 2;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // FrmAbmClientes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(964, 632);
            Controls.Add(flowLayoutPanel1);
            Name = "FrmAbmClientes";
            Text = "Administración de Clientes";
            Load += FrmAbmClientes_Load;
            flowLayoutPanel1.ResumeLayout(false);
            groupBoxTitulo.ResumeLayout(false);
            groupBoxTitulo.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingLocalidades).EndInit();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridClientes).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingClientes).EndInit();
            groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridContactos).EndInit();
            ((System.ComponentModel.ISupportInitialize)contactosBindingSource).EndInit();
            groupBoxBotones.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private GroupBox groupBoxTitulo;
        private Label labelTitulo;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBoxBotones;
        private Button btnSeleccionar;
        private Button btnSalir;
        private ComboBox cbBusqueda;
        private Label label2;
        private Button btnImportar;
        private Button btnExportar;
        private Label labelCampo2;
        private TextBox txtCampo2;
        private Label labelCampo1;
        private TextBox txtCampo1;
        private Button btnBuscar;
        private ComboBox cbLocalidad;
        private Label labelLocalidad;
        private DataGridView dataGridContactos;
        private BindingSource bindingLocalidades;
        private Button btnRegistrar;
        private CheckBox checkAnulados;
        private DataGridView dataGridClientes;
        private BindingSource bindingClientes;
        private DataGridViewTextBoxColumn apellidosDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nombresDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dniDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fechaNacDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn Domicilio;
        private DataGridViewTextBoxColumn Localidad;
        private DataGridViewTextBoxColumn whatsappDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn telefonoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn instagramDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn facebookDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn emailDataGridViewTextBoxColumn;
        private BindingSource contactosBindingSource;
    }
}