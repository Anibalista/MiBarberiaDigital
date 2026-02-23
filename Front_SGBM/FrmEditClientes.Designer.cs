namespace Front_SGBM
{
    partial class FrmEditClientes
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
            BtnCancelar = new Button();
            BtnGuardar = new Button();
            groupBox1 = new GroupBox();
            CbEstados = new ComboBox();
            bindingEstados = new BindingSource(components);
            LinkContactos = new LinkLabel();
            label6 = new Label();
            label5 = new Label();
            dateTimePicker1 = new DateTimePicker();
            label4 = new Label();
            txtApellido = new TextBox();
            label3 = new Label();
            txtNombre = new TextBox();
            BtnBuscar = new Button();
            TxtDni = new TextBox();
            label2 = new Label();
            groupBox2 = new GroupBox();
            DataGridContactos = new DataGridView();
            whatsappDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            telefonoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            instagramDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            facebookDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            emailDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bindingContactos = new BindingSource(components);
            groupBox3 = new GroupBox();
            CbProvincia = new ComboBox();
            bindingProvincias = new BindingSource(components);
            label15 = new Label();
            CbLocalidad = new ComboBox();
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
            ((System.ComponentModel.ISupportInitialize)DataGridContactos).BeginInit();
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
            panelTitulo.Size = new Size(779, 42);
            panelTitulo.TabIndex = 0;
            // 
            // labelTitulo
            // 
            labelTitulo.AutoSize = true;
            labelTitulo.Font = new Font("Roboto Condensed", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTitulo.Location = new Point(271, 9);
            labelTitulo.Name = "labelTitulo";
            labelTitulo.Size = new Size(206, 29);
            labelTitulo.TabIndex = 0;
            labelTitulo.Text = "Registro de Clientes";
            // 
            // panelBotones
            // 
            panelBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panelBotones.Controls.Add(BtnCancelar);
            panelBotones.Controls.Add(BtnGuardar);
            panelBotones.Location = new Point(0, 550);
            panelBotones.Name = "panelBotones";
            panelBotones.Size = new Size(779, 72);
            panelBotones.TabIndex = 1;
            // 
            // BtnCancelar
            // 
            BtnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnCancelar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BtnCancelar.Location = new Point(547, 18);
            BtnCancelar.Name = "BtnCancelar";
            BtnCancelar.Size = new Size(146, 42);
            BtnCancelar.TabIndex = 1;
            BtnCancelar.Tag = "btnPrincipalR";
            BtnCancelar.Text = "Cancelar";
            BtnCancelar.UseVisualStyleBackColor = true;
            BtnCancelar.Click += BtnCancelar_Click;
            // 
            // BtnGuardar
            // 
            BtnGuardar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BtnGuardar.Location = new Point(77, 18);
            BtnGuardar.Name = "BtnGuardar";
            BtnGuardar.Size = new Size(146, 42);
            BtnGuardar.TabIndex = 0;
            BtnGuardar.Tag = "btnPrincipalV";
            BtnGuardar.Text = "Guardar";
            BtnGuardar.UseVisualStyleBackColor = true;
            BtnGuardar.Click += BtnGuardar_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(CbEstados);
            groupBox1.Controls.Add(LinkContactos);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(txtApellido);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtNombre);
            groupBox1.Controls.Add(BtnBuscar);
            groupBox1.Controls.Add(TxtDni);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(0, 48);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(779, 153);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Datos del Cliente";
            // 
            // CbEstados
            // 
            CbEstados.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CbEstados.DataSource = bindingEstados;
            CbEstados.DisplayMember = "Estado";
            CbEstados.DropDownStyle = ComboBoxStyle.DropDownList;
            CbEstados.FormattingEnabled = true;
            CbEstados.Location = new Point(533, 70);
            CbEstados.Name = "CbEstados";
            CbEstados.Size = new Size(237, 23);
            CbEstados.TabIndex = 5;
            CbEstados.ValueMember = "IdEstado";
            // 
            // bindingEstados
            // 
            bindingEstados.DataSource = typeof(Entidades_SGBM.Estados);
            // 
            // LinkContactos
            // 
            LinkContactos.ActiveLinkColor = Color.Blue;
            LinkContactos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LinkContactos.AutoSize = true;
            LinkContactos.BackColor = Color.Transparent;
            LinkContactos.DisabledLinkColor = Color.Black;
            LinkContactos.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LinkContactos.Location = new Point(634, 131);
            LinkContactos.Name = "LinkContactos";
            LinkContactos.Size = new Size(139, 19);
            LinkContactos.TabIndex = 6;
            LinkContactos.TabStop = true;
            LinkContactos.Text = "Modificar Contactos";
            LinkContactos.VisitedLinkColor = Color.Blue;
            LinkContactos.LinkClicked += LinkContactos_LinkClicked;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(409, 73);
            label6.Name = "label6";
            label6.Size = new Size(42, 15);
            label6.TabIndex = 9;
            label6.Text = "Estado";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(409, 34);
            label5.Name = "label5";
            label5.Size = new Size(81, 15);
            label5.TabIndex = 8;
            label5.Text = "F. Nacimiento";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(533, 28);
            dateTimePicker1.MaxDate = new DateTime(2199, 12, 31, 0, 0, 0, 0);
            dateTimePicker1.MinDate = new DateTime(1910, 1, 1, 0, 0, 0, 0);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(236, 23);
            dateTimePicker1.TabIndex = 4;
            dateTimePicker1.Value = new DateTime(2007, 1, 1, 0, 0, 0, 0);
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 115);
            label4.Name = "label4";
            label4.Size = new Size(56, 15);
            label4.TabIndex = 6;
            label4.Text = "Apellidos";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(77, 112);
            txtApellido.MaxLength = 149;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(243, 23);
            txtApellido.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 73);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 4;
            label3.Text = "Nombres";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(77, 70);
            txtNombre.MaxLength = 149;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(243, 23);
            txtNombre.TabIndex = 2;
            // 
            // BtnBuscar
            // 
            BtnBuscar.Location = new Point(245, 28);
            BtnBuscar.Name = "BtnBuscar";
            BtnBuscar.Size = new Size(75, 23);
            BtnBuscar.TabIndex = 1;
            BtnBuscar.Tag = "btnNormal";
            BtnBuscar.Text = "Buscar";
            BtnBuscar.UseVisualStyleBackColor = true;
            BtnBuscar.Click += BtnBuscar_Click;
            // 
            // TxtDni
            // 
            TxtDni.Location = new Point(77, 29);
            TxtDni.MaxLength = 14;
            TxtDni.Name = "TxtDni";
            TxtDni.Size = new Size(116, 23);
            TxtDni.TabIndex = 0;
            TxtDni.TextAlign = HorizontalAlignment.Center;
            TxtDni.KeyPress += Numeric_KeyPress;
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
            groupBox2.Controls.Add(DataGridContactos);
            groupBox2.Location = new Point(0, 207);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(779, 182);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Datos de Contacto";
            // 
            // DataGridContactos
            // 
            DataGridContactos.AllowUserToAddRows = false;
            DataGridContactos.AllowUserToDeleteRows = false;
            DataGridContactos.AllowUserToOrderColumns = true;
            DataGridContactos.AutoGenerateColumns = false;
            DataGridContactos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridContactos.Columns.AddRange(new DataGridViewColumn[] { whatsappDataGridViewTextBoxColumn, telefonoDataGridViewTextBoxColumn, instagramDataGridViewTextBoxColumn, facebookDataGridViewTextBoxColumn, emailDataGridViewTextBoxColumn });
            DataGridContactos.DataSource = bindingContactos;
            DataGridContactos.Dock = DockStyle.Fill;
            DataGridContactos.Location = new Point(3, 19);
            DataGridContactos.Name = "DataGridContactos";
            DataGridContactos.ReadOnly = true;
            DataGridContactos.RowHeadersVisible = false;
            DataGridContactos.Size = new Size(773, 160);
            DataGridContactos.TabIndex = 1;
            // 
            // whatsappDataGridViewTextBoxColumn
            // 
            whatsappDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            whatsappDataGridViewTextBoxColumn.DataPropertyName = "Whatsapp";
            whatsappDataGridViewTextBoxColumn.FillWeight = 19F;
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
            emailDataGridViewTextBoxColumn.FillWeight = 20F;
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
            groupBox3.Controls.Add(CbProvincia);
            groupBox3.Controls.Add(label15);
            groupBox3.Controls.Add(CbLocalidad);
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
            groupBox3.Size = new Size(779, 149);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Datos de Domicilio";
            // 
            // CbProvincia
            // 
            CbProvincia.DataSource = bindingProvincias;
            CbProvincia.DisplayMember = "Provincia";
            CbProvincia.FormattingEnabled = true;
            CbProvincia.Location = new Point(77, 108);
            CbProvincia.Name = "CbProvincia";
            CbProvincia.Size = new Size(243, 23);
            CbProvincia.TabIndex = 5;
            CbProvincia.ValueMember = "IdProvincia";
            CbProvincia.SelectedIndexChanged += CbProvincia_SelectedIndexChanged;
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
            // CbLocalidad
            // 
            CbLocalidad.DataSource = bindingLocalidades;
            CbLocalidad.DisplayMember = "Localidad";
            CbLocalidad.FormattingEnabled = true;
            CbLocalidad.Location = new Point(471, 70);
            CbLocalidad.Name = "CbLocalidad";
            CbLocalidad.Size = new Size(237, 23);
            CbLocalidad.TabIndex = 6;
            CbLocalidad.ValueMember = "IdLocalidad";
            // 
            // bindingLocalidades
            // 
            bindingLocalidades.DataSource = typeof(Entidades_SGBM.Localidades);
            // 
            // txtPiso
            // 
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
            txtBarrio.Location = new Point(77, 70);
            txtBarrio.MaxLength = 149;
            txtBarrio.Name = "txtBarrio";
            txtBarrio.Size = new Size(243, 23);
            txtBarrio.TabIndex = 4;
            // 
            // txtCalle
            // 
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
            pnlContent.Location = new Point(785, 0);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(426, 622);
            pnlContent.TabIndex = 5;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // FrmEditClientes
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
            Name = "FrmEditClientes";
            Text = "Formulario de clientes";
            Load += FrmEditClientes_Load;
            panelTitulo.ResumeLayout(false);
            panelTitulo.PerformLayout();
            panelBotones.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingEstados).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DataGridContactos).EndInit();
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
        private Button BtnGuardar;
        private Button BtnCancelar;
        private GroupBox groupBox1;
        private Label label4;
        private TextBox txtApellido;
        private Label label3;
        private TextBox txtNombre;
        private Button BtnBuscar;
        private TextBox TxtDni;
        private Label label2;
        private Label label5;
        private DateTimePicker dateTimePicker1;
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
        private ComboBox CbProvincia;
        private Label label15;
        private ComboBox CbLocalidad;
        private LinkLabel LinkContactos;
        private DataGridView DataGridContactos;
        private BindingSource bindingContactos;
        private BindingSource bindingProvincias;
        private BindingSource bindingLocalidades;
        private ComboBox CbEstados;
        private Label label6;
        private BindingSource bindingEstados;
        private Panel pnlContent;
        private ErrorProvider errorProvider1;
        private DataGridViewTextBoxColumn whatsappDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn telefonoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn instagramDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn facebookDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn emailDataGridViewTextBoxColumn;
    }
}