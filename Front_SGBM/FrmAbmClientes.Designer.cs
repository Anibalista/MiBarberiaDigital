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
            groupBox1 = new GroupBox();
            checkAnulados = new CheckBox();
            btnRegistrar = new Button();
            btnImportar = new Button();
            btnBuscar = new Button();
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
            btnConsultar = new Button();
            btnSeleccionar = new Button();
            btnSalir = new Button();
            groupBox3 = new GroupBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingLocalidades).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridClientes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingClientes).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridContactos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)contactosBindingSource).BeginInit();
            groupBoxBotones.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox1.Controls.Add(checkAnulados);
            groupBox1.Controls.Add(btnRegistrar);
            groupBox1.Controls.Add(btnImportar);
            groupBox1.Controls.Add(btnBuscar);
            groupBox1.Controls.Add(btnExportar);
            groupBox1.Controls.Add(cbBusqueda);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(877, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(285, 184);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filtros y Archivos";
            // 
            // checkAnulados
            // 
            checkAnulados.AutoSize = true;
            checkAnulados.Location = new Point(81, 100);
            checkAnulados.Name = "checkAnulados";
            checkAnulados.Size = new Size(112, 19);
            checkAnulados.TabIndex = 2;
            checkAnulados.Text = "Incluir Anulados";
            checkAnulados.UseVisualStyleBackColor = true;
            // 
            // btnRegistrar
            // 
            btnRegistrar.Location = new Point(6, 22);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new Size(107, 29);
            btnRegistrar.TabIndex = 0;
            btnRegistrar.Tag = "btnNormalV";
            btnRegistrar.Text = "Nuevo";
            btnRegistrar.UseVisualStyleBackColor = true;
            btnRegistrar.Click += btnRegistrar_Click;
            // 
            // btnImportar
            // 
            btnImportar.BackColor = Color.Transparent;
            btnImportar.Location = new Point(6, 131);
            btnImportar.Name = "btnImportar";
            btnImportar.Size = new Size(109, 29);
            btnImportar.TabIndex = 5;
            btnImportar.Tag = "btnNormal";
            btnImportar.Text = "Importar Clientes";
            btnImportar.UseVisualStyleBackColor = false;
            btnImportar.Click += btnImportar_Click;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(164, 22);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(111, 29);
            btnBuscar.TabIndex = 4;
            btnBuscar.Tag = "btnNormal";
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // btnExportar
            // 
            btnExportar.Location = new Point(164, 131);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(109, 29);
            btnExportar.TabIndex = 1;
            btnExportar.Tag = "btnNormal";
            btnExportar.Text = "Exportar a Listado";
            btnExportar.UseVisualStyleBackColor = true;
            btnExportar.Click += btnExportar_Click;
            // 
            // cbBusqueda
            // 
            cbBusqueda.DropDownStyle = ComboBoxStyle.DropDownList;
            cbBusqueda.FormattingEnabled = true;
            cbBusqueda.Location = new Point(81, 71);
            cbBusqueda.Name = "cbBusqueda";
            cbBusqueda.Size = new Size(149, 23);
            cbBusqueda.TabIndex = 3;
            cbBusqueda.TextChanged += cbBusqueda_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 74);
            label2.Name = "label2";
            label2.Size = new Size(69, 15);
            label2.TabIndex = 1;
            label2.Text = "Buscar por: ";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox2.Controls.Add(cbLocalidad);
            groupBox2.Controls.Add(labelLocalidad);
            groupBox2.Controls.Add(labelCampo2);
            groupBox2.Controls.Add(txtCampo2);
            groupBox2.Controls.Add(labelCampo1);
            groupBox2.Controls.Add(txtCampo1);
            groupBox2.Location = new Point(877, 202);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(284, 139);
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
            cbLocalidad.Location = new Point(81, 96);
            cbLocalidad.Name = "cbLocalidad";
            cbLocalidad.Size = new Size(194, 23);
            cbLocalidad.TabIndex = 2;
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
            labelLocalidad.Location = new Point(6, 99);
            labelLocalidad.Name = "labelLocalidad";
            labelLocalidad.Size = new Size(58, 15);
            labelLocalidad.TabIndex = 9;
            labelLocalidad.Text = "Localidad";
            // 
            // labelCampo2
            // 
            labelCampo2.AutoSize = true;
            labelCampo2.Location = new Point(6, 62);
            labelCampo2.Name = "labelCampo2";
            labelCampo2.Size = new Size(56, 15);
            labelCampo2.TabIndex = 8;
            labelCampo2.Text = "Nombres";
            // 
            // txtCampo2
            // 
            txtCampo2.Location = new Point(81, 59);
            txtCampo2.MaxLength = 149;
            txtCampo2.Name = "txtCampo2";
            txtCampo2.Size = new Size(194, 23);
            txtCampo2.TabIndex = 1;
            // 
            // labelCampo1
            // 
            labelCampo1.AutoSize = true;
            labelCampo1.Location = new Point(37, 25);
            labelCampo1.Name = "labelCampo1";
            labelCampo1.Size = new Size(25, 15);
            labelCampo1.TabIndex = 6;
            labelCampo1.Text = "Dni";
            // 
            // txtCampo1
            // 
            txtCampo1.Location = new Point(81, 22);
            txtCampo1.MaxLength = 149;
            txtCampo1.Name = "txtCampo1";
            txtCampo1.Size = new Size(194, 23);
            txtCampo1.TabIndex = 0;
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
            dataGridClientes.Dock = DockStyle.Fill;
            dataGridClientes.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridClientes.Location = new Point(3, 19);
            dataGridClientes.MultiSelect = false;
            dataGridClientes.Name = "dataGridClientes";
            dataGridClientes.ReadOnly = true;
            dataGridClientes.RowHeadersVisible = false;
            dataGridClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridClientes.Size = new Size(860, 384);
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
            groupBox4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.Controls.Add(dataGridContactos);
            groupBox4.Location = new Point(3, 411);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(863, 139);
            groupBox4.TabIndex = 4;
            groupBox4.TabStop = false;
            groupBox4.Text = "Contactos";
            // 
            // dataGridContactos
            // 
            dataGridContactos.AllowUserToAddRows = false;
            dataGridContactos.AllowUserToDeleteRows = false;
            dataGridContactos.AllowUserToOrderColumns = true;
            dataGridContactos.AutoGenerateColumns = false;
            dataGridContactos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridContactos.Columns.AddRange(new DataGridViewColumn[] { whatsappDataGridViewTextBoxColumn, telefonoDataGridViewTextBoxColumn, instagramDataGridViewTextBoxColumn, facebookDataGridViewTextBoxColumn, emailDataGridViewTextBoxColumn });
            dataGridContactos.DataSource = contactosBindingSource;
            dataGridContactos.Dock = DockStyle.Fill;
            dataGridContactos.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridContactos.Location = new Point(3, 19);
            dataGridContactos.MultiSelect = false;
            dataGridContactos.Name = "dataGridContactos";
            dataGridContactos.ReadOnly = true;
            dataGridContactos.RowHeadersVisible = false;
            dataGridContactos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridContactos.Size = new Size(857, 117);
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
            groupBoxBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            groupBoxBotones.Controls.Add(btnConsultar);
            groupBoxBotones.Controls.Add(btnSeleccionar);
            groupBoxBotones.Controls.Add(btnSalir);
            groupBoxBotones.Location = new Point(883, 347);
            groupBoxBotones.Name = "groupBoxBotones";
            groupBoxBotones.Size = new Size(284, 203);
            groupBoxBotones.TabIndex = 5;
            groupBoxBotones.TabStop = false;
            // 
            // btnConsultar
            // 
            btnConsultar.Dock = DockStyle.Top;
            btnConsultar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConsultar.Location = new Point(3, 19);
            btnConsultar.Name = "btnConsultar";
            btnConsultar.Size = new Size(278, 42);
            btnConsultar.TabIndex = 0;
            btnConsultar.Tag = "btnPrincipal";
            btnConsultar.Text = "Ver Detalles";
            btnConsultar.UseVisualStyleBackColor = true;
            btnConsultar.Click += btnConsultar_Click;
            // 
            // btnSeleccionar
            // 
            btnSeleccionar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSeleccionar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSeleccionar.Location = new Point(3, 89);
            btnSeleccionar.Name = "btnSeleccionar";
            btnSeleccionar.Size = new Size(278, 42);
            btnSeleccionar.TabIndex = 1;
            btnSeleccionar.Tag = "btnPrincipalV";
            btnSeleccionar.Text = "Seleccionar";
            btnSeleccionar.UseVisualStyleBackColor = true;
            btnSeleccionar.Click += btnSeleccionar_Click;
            // 
            // btnSalir
            // 
            btnSalir.Dock = DockStyle.Bottom;
            btnSalir.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSalir.Location = new Point(3, 158);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(278, 42);
            btnSalir.TabIndex = 2;
            btnSalir.Tag = "btnPrincipalR";
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(dataGridClientes);
            groupBox3.Location = new Point(3, 2);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(866, 406);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Clientes";
            // 
            // FrmAbmClientes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1174, 562);
            Controls.Add(groupBoxBotones);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmAbmClientes";
            Text = "Administración de Clientes";
            Load += FrmAbmClientes_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingLocalidades).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridClientes).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingClientes).EndInit();
            groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridContactos).EndInit();
            ((System.ComponentModel.ISupportInitialize)contactosBindingSource).EndInit();
            groupBoxBotones.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox1;
        private GroupBox groupBox2;
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
        private GroupBox groupBox3;
        private Button btnConsultar;
    }
}