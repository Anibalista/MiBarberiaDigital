namespace Front_SGBM
{
    partial class FrmContactos
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
            label2 = new Label();
            checkExtranjeroWhat = new CheckBox();
            label1 = new Label();
            txtEmail = new TextBox();
            label13 = new Label();
            label12 = new Label();
            txtNumFijo = new TextBox();
            label10 = new Label();
            txtAreaFijo = new TextBox();
            label11 = new Label();
            txtNumWhat = new TextBox();
            label6 = new Label();
            label7 = new Label();
            txtFace = new TextBox();
            label8 = new Label();
            txtInsta = new TextBox();
            txtAreaWhat = new TextBox();
            label9 = new Label();
            btnCancelar = new Button();
            btnGuardar = new Button();
            btnMenos = new Button();
            btnMas = new Button();
            groupBoxBotones = new GroupBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            errorProvider1 = new ErrorProvider(components);
            groupBox1.SuspendLayout();
            groupBoxBotones.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(checkExtranjeroWhat);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(txtEmail);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(txtNumFijo);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(txtAreaFijo);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(txtNumWhat);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(txtFace);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(txtInsta);
            groupBox1.Controls.Add(txtAreaWhat);
            groupBox1.Controls.Add(label9);
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(351, 223);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Contacto Nro 1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(65, 22);
            label2.Name = "label2";
            label2.Size = new Size(65, 15);
            label2.TabIndex = 19;
            label2.Text = "Extranjero?";
            // 
            // checkExtranjeroWhat
            // 
            checkExtranjeroWhat.AutoSize = true;
            checkExtranjeroWhat.BackColor = Color.Transparent;
            checkExtranjeroWhat.CheckAlign = ContentAlignment.BottomCenter;
            checkExtranjeroWhat.Location = new Point(38, 23);
            checkExtranjeroWhat.Name = "checkExtranjeroWhat";
            checkExtranjeroWhat.Size = new Size(15, 14);
            checkExtranjeroWhat.TabIndex = 0;
            checkExtranjeroWhat.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 189);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 16;
            label1.Text = "E-Mail";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(90, 186);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(251, 23);
            txtEmail.TabIndex = 7;
            txtEmail.Validating += TxtEmail_Validating;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(152, 74);
            label13.Name = "label13";
            label13.Size = new Size(189, 15);
            label13.TabIndex = 14;
            label13.Text = "-----------Teléfono Fijo------------";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(165, 22);
            label12.Name = "label12";
            label12.Size = new Size(175, 15);
            label12.TabIndex = 13;
            label12.Text = "-----------Whatsapp------------";
            // 
            // txtNumFijo
            // 
            txtNumFijo.Location = new Point(239, 95);
            txtNumFijo.Name = "txtNumFijo";
            txtNumFijo.Size = new Size(102, 23);
            txtNumFijo.TabIndex = 4;
            txtNumFijo.KeyPress += Numeric_KeyPress;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(160, 98);
            label10.Name = "label10";
            label10.Size = new Size(57, 15);
            label10.TabIndex = 11;
            label10.Text = "Número: ";
            // 
            // txtAreaFijo
            // 
            txtAreaFijo.Location = new Point(90, 95);
            txtAreaFijo.Name = "txtAreaFijo";
            txtAreaFijo.Size = new Size(50, 23);
            txtAreaFijo.TabIndex = 3;
            txtAreaFijo.KeyPress += Numeric_KeyPress;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = Color.Transparent;
            label11.Location = new Point(21, 98);
            label11.Name = "label11";
            label11.Size = new Size(51, 15);
            label11.TabIndex = 9;
            label11.Text = "C. Area: ";
            // 
            // txtNumWhat
            // 
            txtNumWhat.Location = new Point(222, 48);
            txtNumWhat.Name = "txtNumWhat";
            txtNumWhat.Size = new Size(119, 23);
            txtNumWhat.TabIndex = 2;
            txtNumWhat.KeyPress += Numeric_KeyPress;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(145, 51);
            label6.Name = "label6";
            label6.Size = new Size(69, 15);
            label6.TabIndex = 7;
            label6.Text = "Número: 15";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 158);
            label7.Name = "label7";
            label7.Size = new Size(58, 15);
            label7.TabIndex = 6;
            label7.Text = "Facebook";
            // 
            // txtFace
            // 
            txtFace.Location = new Point(90, 155);
            txtFace.Name = "txtFace";
            txtFace.Size = new Size(251, 23);
            txtFace.TabIndex = 6;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 127);
            label8.Name = "label8";
            label8.Size = new Size(60, 15);
            label8.TabIndex = 4;
            label8.Text = "Instagram";
            // 
            // txtInsta
            // 
            txtInsta.Location = new Point(90, 124);
            txtInsta.Name = "txtInsta";
            txtInsta.Size = new Size(251, 23);
            txtInsta.TabIndex = 5;
            // 
            // txtAreaWhat
            // 
            txtAreaWhat.Location = new Point(89, 48);
            txtAreaWhat.Name = "txtAreaWhat";
            txtAreaWhat.Size = new Size(50, 23);
            txtAreaWhat.TabIndex = 1;
            txtAreaWhat.KeyPress += Numeric_KeyPress;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.Location = new Point(21, 51);
            label9.Name = "label9";
            label9.Size = new Size(51, 15);
            label9.TabIndex = 0;
            label9.Text = "C. Area: ";
            // 
            // btnCancelar
            // 
            btnCancelar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold);
            btnCancelar.Location = new Point(163, 65);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(144, 35);
            btnCancelar.TabIndex = 3;
            btnCancelar.Tag = "btnSecundarioR";
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += BtnCancelar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold);
            btnGuardar.Location = new Point(163, 16);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(144, 35);
            btnGuardar.TabIndex = 2;
            btnGuardar.Tag = "btnSecundarioV";
            btnGuardar.Text = "Guardar Contacto";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += BtnGuardar_Click;
            // 
            // btnMenos
            // 
            btnMenos.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold);
            btnMenos.Location = new Point(11, 32);
            btnMenos.Name = "btnMenos";
            btnMenos.Size = new Size(45, 37);
            btnMenos.TabIndex = 1;
            btnMenos.Tag = "btnPrincipal";
            btnMenos.Text = "-";
            btnMenos.UseVisualStyleBackColor = true;
            btnMenos.Click += BtnMenos_Click;
            // 
            // btnMas
            // 
            btnMas.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold);
            btnMas.Location = new Point(68, 32);
            btnMas.Name = "btnMas";
            btnMas.Size = new Size(45, 37);
            btnMas.TabIndex = 0;
            btnMas.Tag = "btnPrincipal";
            btnMas.Text = "+";
            btnMas.UseVisualStyleBackColor = true;
            btnMas.Click += BtnMas_Click;
            // 
            // groupBoxBotones
            // 
            groupBoxBotones.Controls.Add(btnMas);
            groupBoxBotones.Controls.Add(btnMenos);
            groupBoxBotones.Controls.Add(btnCancelar);
            groupBoxBotones.Controls.Add(btnGuardar);
            groupBoxBotones.Dock = DockStyle.Bottom;
            groupBoxBotones.Location = new Point(0, 258);
            groupBoxBotones.Name = "groupBoxBotones";
            groupBoxBotones.Size = new Size(361, 113);
            groupBoxBotones.TabIndex = 5;
            groupBoxBotones.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(361, 258);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // errorProvider1
            // 
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider1.ContainerControl = this;
            // 
            // FrmContactos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(361, 371);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(groupBoxBotones);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmContactos";
            Text = "FrmContactos";
            Load += FrmContactos_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBoxBotones.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label13;
        private Label label12;
        private TextBox txtNumFijo;
        private Label label10;
        private TextBox txtAreaFijo;
        private Label label11;
        private TextBox txtNumWhat;
        private Label label6;
        private Label label7;
        private TextBox txtFace;
        private Label label8;
        private TextBox txtInsta;
        private TextBox txtAreaWhat;
        private Label label9;
        private Label label1;
        private TextBox txtEmail;
        private Button btnMenos;
        private Button btnMas;
        private Button btnCancelar;
        private Button btnGuardar;
        private GroupBox groupBoxBotones;
        private Label label2;
        private CheckBox checkExtranjeroWhat;
        private FlowLayoutPanel flowLayoutPanel1;
        private ErrorProvider errorProvider1;
    }
}