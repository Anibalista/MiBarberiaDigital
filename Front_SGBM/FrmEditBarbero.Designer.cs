namespace Front_SGBM
{
    partial class FrmEditBarbero
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
            panelTitulo = new Panel();
            labelTitulo = new Label();
            panelBotones = new Panel();
            btnCancelar = new Button();
            btnGuardar = new Button();
            groupBox1 = new GroupBox();
            cbTipo = new ComboBox();
            lblIngreso = new Label();
            dateTimeIngreso = new DateTimePicker();
            lblTipo = new Label();
            cbEstados = new ComboBox();
            bindingEstados = new BindingSource(components);
            linkContactos = new LinkLabel();
            label6 = new Label();
            label5 = new Label();
            dateTimeNacimiento = new DateTimePicker();
            label4 = new Label();
            txtApellido = new TextBox();
            label3 = new Label();
            txtNombre = new TextBox();
            btnBuscar = new Button();
            txtDni = new TextBox();
            label2 = new Label();
            groupBox2 = new GroupBox();
            dataGridContactos = new DataGridView();
            whatsappDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            telefonoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            instagramDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            facebookDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            emailDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bindingContactos = new BindingSource(components);
            groupBox3 = new GroupBox();
            cbProvincia = new ComboBox();
            bindingProvincias = new BindingSource(components);
            label15 = new Label();
            cbLocalidad = new ComboBox();
            bindingLocalidades = new BindingSource(components);
            txtPiso = new TextBox();
            label14 = new Label();
            txtDepto = new TextBox();
            label16 = new Label();
            txtNro = new TextBox();
            label17 = new Label();
            label19 = new Label();
            label20 = new Label();
            txtBarrio = new TextBox();
            txtCalle = new TextBox();
            label21 = new Label();
            pnlContent = new Panel();
            errorProvider1 = new ErrorProvider(components);
            panelTitulo.SuspendLayout();
            panelBotones.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingEstados).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridContactos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingContactos).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingProvincias).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingLocalidades).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // panelTitulo
            // 
            panelTitulo.Controls.Add(labelTitulo);
            panelTitulo.Location = new Point(0, 0);
            panelTitulo.Name = "panelTitulo";
            panelTitulo.Size = new Size(717, 42);
            panelTitulo.TabIndex = 0;
            // 
            // labelTitulo
            // 
            labelTitulo.AutoSize = true;
            labelTitulo.Font = new Font("Roboto Condensed", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTitulo.Location = new Point(271, 9);
            labelTitulo.Name = "labelTitulo";
            labelTitulo.Size = new Size(280, 29);
            labelTitulo.TabIndex = 0;
            labelTitulo.Text = "Administración de Barberos";
            // 
            // panelBotones
            // 
            panelBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panelBotones.Controls.Add(btnCancelar);
            panelBotones.Controls.Add(btnGuardar);
            panelBotones.Location = new Point(0, 550);
            panelBotones.Name = "panelBotones";
            panelBotones.Size = new Size(717, 72);
            panelBotones.TabIndex = 1;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.Location = new Point(485, 18);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(146, 42);
            btnCancelar.TabIndex = 1;
            btnCancelar.Tag = "btnPrincipalR";
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Enabled = false;
            btnGuardar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuardar.Location = new Point(77, 18);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(146, 42);
            btnGuardar.TabIndex = 0;
            btnGuardar.Tag = "btnPrincipalV";
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbTipo);
            groupBox1.Controls.Add(lblIngreso);
            groupBox1.Controls.Add(dateTimeIngreso);
            groupBox1.Controls.Add(lblTipo);
            groupBox1.Controls.Add(cbEstados);
            groupBox1.Controls.Add(linkContactos);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(dateTimeNacimiento);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(txtApellido);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtNombre);
            groupBox1.Controls.Add(btnBuscar);
            groupBox1.Controls.Add(txtDni);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(0, 48);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(717, 176);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Datos del Barbero";
            // 
            // cbTipo
            // 
            cbTipo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTipo.Enabled = false;
            cbTipo.FormattingEnabled = true;
            cbTipo.Location = new Point(471, 30);
            cbTipo.Name = "cbTipo";
            cbTipo.Size = new Size(237, 23);
            cbTipo.TabIndex = 14;
            // 
            // lblIngreso
            // 
            lblIngreso.AutoSize = true;
            lblIngreso.Location = new Point(6, 143);
            lblIngreso.Name = "lblIngreso";
            lblIngreso.Size = new Size(46, 15);
            lblIngreso.TabIndex = 13;
            lblIngreso.Text = "Ingreso";
            // 
            // dateTimeIngreso
            // 
            dateTimeIngreso.Enabled = false;
            dateTimeIngreso.Location = new Point(77, 137);
            dateTimeIngreso.MaxDate = new DateTime(2199, 12, 31, 0, 0, 0, 0);
            dateTimeIngreso.MinDate = new DateTime(1910, 1, 1, 0, 0, 0, 0);
            dateTimeIngreso.Name = "dateTimeIngreso";
            dateTimeIngreso.Size = new Size(243, 23);
            dateTimeIngreso.TabIndex = 12;
            dateTimeIngreso.Value = new DateTime(2007, 1, 1, 0, 0, 0, 0);
            // 
            // lblTipo
            // 
            lblTipo.AutoSize = true;
            lblTipo.Location = new Point(349, 32);
            lblTipo.Name = "lblTipo";
            lblTipo.Size = new Size(102, 15);
            lblTipo.TabIndex = 11;
            lblTipo.Text = "Tipo de Empleado";
            // 
            // cbEstados
            // 
            cbEstados.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbEstados.DataSource = bindingEstados;
            cbEstados.DisplayMember = "Estado";
            cbEstados.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstados.Enabled = false;
            cbEstados.FormattingEnabled = true;
            cbEstados.Location = new Point(471, 65);
            cbEstados.Name = "cbEstados";
            cbEstados.Size = new Size(237, 23);
            cbEstados.TabIndex = 5;
            cbEstados.ValueMember = "IdEstado";
            // 
            // bindingEstados
            // 
            bindingEstados.DataSource = typeof(Entidades_SGBM.Estados);
            // 
            // linkContactos
            // 
            linkContactos.ActiveLinkColor = Color.Blue;
            linkContactos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            linkContactos.AutoSize = true;
            linkContactos.BackColor = Color.Transparent;
            linkContactos.DisabledLinkColor = Color.Black;
            linkContactos.Enabled = false;
            linkContactos.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkContactos.Location = new Point(572, 157);
            linkContactos.Name = "linkContactos";
            linkContactos.Size = new Size(139, 19);
            linkContactos.TabIndex = 6;
            linkContactos.TabStop = true;
            linkContactos.Text = "Modificar Contactos";
            linkContactos.VisitedLinkColor = Color.Blue;
            linkContactos.LinkClicked += LinkContactos_LinkClicked;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(407, 68);
            label6.Name = "label6";
            label6.Size = new Size(42, 15);
            label6.TabIndex = 9;
            label6.Text = "Estado";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(370, 102);
            label5.Name = "label5";
            label5.Size = new Size(81, 15);
            label5.TabIndex = 8;
            label5.Text = "F. Nacimiento";
            // 
            // dateTimeNacimiento
            // 
            dateTimeNacimiento.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dateTimeNacimiento.Enabled = false;
            dateTimeNacimiento.Format = DateTimePickerFormat.Short;
            dateTimeNacimiento.Location = new Point(471, 99);
            dateTimeNacimiento.MaxDate = new DateTime(2199, 12, 31, 0, 0, 0, 0);
            dateTimeNacimiento.MinDate = new DateTime(1910, 1, 1, 0, 0, 0, 0);
            dateTimeNacimiento.Name = "dateTimeNacimiento";
            dateTimeNacimiento.Size = new Size(237, 23);
            dateTimeNacimiento.TabIndex = 4;
            dateTimeNacimiento.Value = new DateTime(2007, 1, 1, 0, 0, 0, 0);
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 102);
            label4.Name = "label4";
            label4.Size = new Size(56, 15);
            label4.TabIndex = 6;
            label4.Text = "Apellidos";
            // 
            // txtApellido
            // 
            txtApellido.Enabled = false;
            txtApellido.Location = new Point(77, 99);
            txtApellido.MaxLength = 149;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(243, 23);
            txtApellido.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 68);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 4;
            label3.Text = "Nombres";
            // 
            // txtNombre
            // 
            txtNombre.Enabled = false;
            txtNombre.Location = new Point(77, 65);
            txtNombre.MaxLength = 149;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(243, 23);
            txtNombre.TabIndex = 2;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(206, 29);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(114, 23);
            btnBuscar.TabIndex = 1;
            btnBuscar.Tag = "btnNormal";
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // txtDni
            // 
            txtDni.Location = new Point(77, 29);
            txtDni.MaxLength = 14;
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(113, 23);
            txtDni.TabIndex = 0;
            txtDni.TextAlign = HorizontalAlignment.Center;
            txtDni.KeyPress += KeyPress_Numerico;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(38, 32);
            label2.Name = "label2";
            label2.Size = new Size(25, 15);
            label2.TabIndex = 0;
            label2.Text = "Dni";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox2.Controls.Add(dataGridContactos);
            groupBox2.Location = new Point(0, 221);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(717, 168);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Datos de Contacto";
            // 
            // dataGridContactos
            // 
            dataGridContactos.AllowUserToAddRows = false;
            dataGridContactos.AllowUserToDeleteRows = false;
            dataGridContactos.AllowUserToOrderColumns = true;
            dataGridContactos.AutoGenerateColumns = false;
            dataGridContactos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridContactos.Columns.AddRange(new DataGridViewColumn[] { whatsappDataGridViewTextBoxColumn, telefonoDataGridViewTextBoxColumn, instagramDataGridViewTextBoxColumn, facebookDataGridViewTextBoxColumn, emailDataGridViewTextBoxColumn });
            dataGridContactos.DataSource = bindingContactos;
            dataGridContactos.Dock = DockStyle.Fill;
            dataGridContactos.Location = new Point(3, 19);
            dataGridContactos.Name = "dataGridContactos";
            dataGridContactos.ReadOnly = true;
            dataGridContactos.RowHeadersVisible = false;
            dataGridContactos.Size = new Size(711, 146);
            dataGridContactos.TabIndex = 1;
            // 
            // whatsappDataGridViewTextBoxColumn
            // 
            whatsappDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            whatsappDataGridViewTextBoxColumn.DataPropertyName = "Whatsapp";
            whatsappDataGridViewTextBoxColumn.FillWeight = 13F;
            whatsappDataGridViewTextBoxColumn.HeaderText = "Whatsapp";
            whatsappDataGridViewTextBoxColumn.MinimumWidth = 80;
            whatsappDataGridViewTextBoxColumn.Name = "whatsappDataGridViewTextBoxColumn";
            whatsappDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // telefonoDataGridViewTextBoxColumn
            // 
            telefonoDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            telefonoDataGridViewTextBoxColumn.DataPropertyName = "Telefono";
            telefonoDataGridViewTextBoxColumn.FillWeight = 13F;
            telefonoDataGridViewTextBoxColumn.HeaderText = "Telefono";
            telefonoDataGridViewTextBoxColumn.MinimumWidth = 80;
            telefonoDataGridViewTextBoxColumn.Name = "telefonoDataGridViewTextBoxColumn";
            telefonoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // instagramDataGridViewTextBoxColumn
            // 
            instagramDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            instagramDataGridViewTextBoxColumn.DataPropertyName = "Instagram";
            instagramDataGridViewTextBoxColumn.FillWeight = 24F;
            instagramDataGridViewTextBoxColumn.HeaderText = "Instagram";
            instagramDataGridViewTextBoxColumn.MinimumWidth = 120;
            instagramDataGridViewTextBoxColumn.Name = "instagramDataGridViewTextBoxColumn";
            instagramDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // facebookDataGridViewTextBoxColumn
            // 
            facebookDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            facebookDataGridViewTextBoxColumn.DataPropertyName = "Facebook";
            facebookDataGridViewTextBoxColumn.FillWeight = 24F;
            facebookDataGridViewTextBoxColumn.HeaderText = "Facebook";
            facebookDataGridViewTextBoxColumn.MinimumWidth = 120;
            facebookDataGridViewTextBoxColumn.Name = "facebookDataGridViewTextBoxColumn";
            facebookDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // emailDataGridViewTextBoxColumn
            // 
            emailDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            emailDataGridViewTextBoxColumn.DataPropertyName = "Email";
            emailDataGridViewTextBoxColumn.FillWeight = 26F;
            emailDataGridViewTextBoxColumn.HeaderText = "Email";
            emailDataGridViewTextBoxColumn.MinimumWidth = 150;
            emailDataGridViewTextBoxColumn.Name = "emailDataGridViewTextBoxColumn";
            emailDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bindingContactos
            // 
            bindingContactos.DataSource = typeof(Entidades_SGBM.Contactos);
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox3.Controls.Add(cbProvincia);
            groupBox3.Controls.Add(label15);
            groupBox3.Controls.Add(cbLocalidad);
            groupBox3.Controls.Add(txtPiso);
            groupBox3.Controls.Add(label14);
            groupBox3.Controls.Add(txtDepto);
            groupBox3.Controls.Add(label16);
            groupBox3.Controls.Add(txtNro);
            groupBox3.Controls.Add(label17);
            groupBox3.Controls.Add(label19);
            groupBox3.Controls.Add(label20);
            groupBox3.Controls.Add(txtBarrio);
            groupBox3.Controls.Add(txtCalle);
            groupBox3.Controls.Add(label21);
            groupBox3.Location = new Point(0, 395);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(717, 149);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Datos de Domicilio";
            // 
            // cbProvincia
            // 
            cbProvincia.DataSource = bindingProvincias;
            cbProvincia.DisplayMember = "Provincia";
            cbProvincia.Enabled = false;
            cbProvincia.FormattingEnabled = true;
            cbProvincia.Location = new Point(77, 108);
            cbProvincia.Name = "cbProvincia";
            cbProvincia.Size = new Size(243, 23);
            cbProvincia.TabIndex = 5;
            cbProvincia.ValueMember = "IdProvincia";
            cbProvincia.TextChanged += cbProvincia_TextChanged;
            // 
            // bindingProvincias
            // 
            bindingProvincias.DataSource = typeof(Entidades_SGBM.Provincias);
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(7, 111);
            label15.Name = "label15";
            label15.Size = new Size(56, 15);
            label15.TabIndex = 16;
            label15.Text = "Provincia";
            // 
            // cbLocalidad
            // 
            cbLocalidad.DataSource = bindingLocalidades;
            cbLocalidad.DisplayMember = "Localidad";
            cbLocalidad.Enabled = false;
            cbLocalidad.FormattingEnabled = true;
            cbLocalidad.Location = new Point(471, 70);
            cbLocalidad.Name = "cbLocalidad";
            cbLocalidad.Size = new Size(237, 23);
            cbLocalidad.TabIndex = 6;
            cbLocalidad.ValueMember = "IdLocalidad";
            // 
            // bindingLocalidades
            // 
            bindingLocalidades.DataSource = typeof(Entidades_SGBM.Localidades);
            // 
            // txtPiso
            // 
            txtPiso.Enabled = false;
            txtPiso.Location = new Point(672, 27);
            txtPiso.MaxLength = 9;
            txtPiso.Name = "txtPiso";
            txtPiso.Size = new Size(36, 23);
            txtPiso.TabIndex = 3;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(637, 30);
            label14.Name = "label14";
            label14.Size = new Size(29, 15);
            label14.TabIndex = 13;
            label14.Text = "Piso";
            // 
            // txtDepto
            // 
            txtDepto.Enabled = false;
            txtDepto.Location = new Point(592, 27);
            txtDepto.MaxLength = 9;
            txtDepto.Name = "txtDepto";
            txtDepto.Size = new Size(31, 23);
            txtDepto.TabIndex = 2;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(547, 30);
            label16.Name = "label16";
            label16.Size = new Size(39, 15);
            label16.TabIndex = 11;
            label16.Text = "Depto";
            // 
            // txtNro
            // 
            txtNro.Enabled = false;
            txtNro.Location = new Point(471, 27);
            txtNro.MaxLength = 9;
            txtNro.Name = "txtNro";
            txtNro.Size = new Size(46, 23);
            txtNro.TabIndex = 1;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(424, 30);
            label17.Name = "label17";
            label17.Size = new Size(27, 15);
            label17.TabIndex = 9;
            label17.Text = "Nro";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(393, 73);
            label19.Name = "label19";
            label19.Size = new Size(58, 15);
            label19.TabIndex = 6;
            label19.Text = "Localidad";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(25, 73);
            label20.Name = "label20";
            label20.Size = new Size(38, 15);
            label20.TabIndex = 4;
            label20.Text = "Barrio";
            // 
            // txtBarrio
            // 
            txtBarrio.Enabled = false;
            txtBarrio.Location = new Point(77, 70);
            txtBarrio.MaxLength = 149;
            txtBarrio.Name = "txtBarrio";
            txtBarrio.Size = new Size(243, 23);
            txtBarrio.TabIndex = 4;
            // 
            // txtCalle
            // 
            txtCalle.Enabled = false;
            txtCalle.Location = new Point(77, 27);
            txtCalle.MaxLength = 149;
            txtCalle.Name = "txtCalle";
            txtCalle.Size = new Size(243, 23);
            txtCalle.TabIndex = 0;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(30, 30);
            label21.Name = "label21";
            label21.Size = new Size(33, 15);
            label21.TabIndex = 0;
            label21.Text = "Calle";
            // 
            // pnlContent
            // 
            pnlContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlContent.Location = new Point(723, 0);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(488, 622);
            pnlContent.TabIndex = 5;
            // 
            // errorProvider1
            // 
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider1.ContainerControl = this;
            // 
            // FrmEditBarbero
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1211, 622);
            Controls.Add(pnlContent);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(panelBotones);
            Controls.Add(panelTitulo);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmEditBarbero";
            Text = "Formulario de clientes";
            Load += FrmEditBarbero_Load;
            panelTitulo.ResumeLayout(false);
            panelTitulo.PerformLayout();
            panelBotones.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingEstados).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridContactos).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingContactos).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingProvincias).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingLocalidades).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTitulo;
        private Label labelTitulo;
        private Panel panelBotones;
        private Button btnGuardar;
        private Button btnCancelar;
        private GroupBox groupBox1;
        private Label label4;
        private TextBox txtApellido;
        private Label label3;
        private TextBox txtNombre;
        private Button btnBuscar;
        private TextBox txtDni;
        private Label label2;
        private Label label5;
        private DateTimePicker dateTimeNacimiento;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private TextBox txtPiso;
        private Label label14;
        private TextBox txtDepto;
        private Label label16;
        private TextBox txtNro;
        private Label label17;
        private Label label19;
        private Label label20;
        private TextBox txtBarrio;
        private TextBox txtCalle;
        private Label label21;
        private ComboBox cbProvincia;
        private Label label15;
        private ComboBox cbLocalidad;
        private LinkLabel linkContactos;
        private DataGridView dataGridContactos;
        private DataGridViewTextBoxColumn whatsappDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn telefonoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn instagramDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn facebookDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn emailDataGridViewTextBoxColumn;
        private BindingSource bindingContactos;
        private BindingSource bindingProvincias;
        private BindingSource bindingLocalidades;
        private ComboBox cbEstados;
        private Label label6;
        private BindingSource bindingEstados;
        private Panel pnlContent;
        private ErrorProvider errorProvider1;
        private Label lblIngreso;
        private DateTimePicker dateTimeIngreso;
        private Label lblTipo;
        private ComboBox cbTipo;
    }
}