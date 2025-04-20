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
            groupBox2 = new GroupBox();
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
            flowLayoutPanel1 = new FlowLayoutPanel();
            panelBotones = new Panel();
            btnCancelar = new Button();
            btnGuardar = new Button();
            btnMenos = new Button();
            btnMas = new Button();
            groupBox2.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            panelBotones.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(txtEmail);
            groupBox2.Controls.Add(label13);
            groupBox2.Controls.Add(label12);
            groupBox2.Controls.Add(txtNumFijo);
            groupBox2.Controls.Add(label10);
            groupBox2.Controls.Add(txtAreaFijo);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(txtNumWhat);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(txtFace);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(txtInsta);
            groupBox2.Controls.Add(txtAreaWhat);
            groupBox2.Controls.Add(label9);
            groupBox2.Location = new Point(3, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(703, 132);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Contacto Nro 1";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(392, 99);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 16;
            label1.Text = "E-Mail";
            // 
            // txtEmail
            // 
            txtEmail.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtEmail.Location = new Point(439, 96);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(251, 23);
            txtEmail.TabIndex = 15;
            txtEmail.Validating += txtEmail_Validating;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(106, 78);
            label13.Name = "label13";
            label13.Size = new Size(189, 15);
            label13.TabIndex = 14;
            label13.Text = "-----------Teléfono Fijo------------";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(106, 11);
            label12.Name = "label12";
            label12.Size = new Size(175, 15);
            label12.TabIndex = 13;
            label12.Text = "-----------Whatsapp------------";
            // 
            // txtNumFijo
            // 
            txtNumFijo.Location = new Point(222, 96);
            txtNumFijo.Name = "txtNumFijo";
            txtNumFijo.Size = new Size(108, 23);
            txtNumFijo.TabIndex = 12;
            txtNumFijo.KeyPress += numeric_KeyPress;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(159, 99);
            label10.Name = "label10";
            label10.Size = new Size(57, 15);
            label10.TabIndex = 11;
            label10.Text = "Número: ";
            // 
            // txtAreaFijo
            // 
            txtAreaFijo.Location = new Point(87, 96);
            txtAreaFijo.Name = "txtAreaFijo";
            txtAreaFijo.Size = new Size(50, 23);
            txtAreaFijo.TabIndex = 10;
            txtAreaFijo.KeyPress += numeric_KeyPress;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(15, 99);
            label11.Name = "label11";
            label11.Size = new Size(66, 15);
            label11.TabIndex = 9;
            label11.Text = "C. Area:    0";
            // 
            // txtNumWhat
            // 
            txtNumWhat.Location = new Point(222, 34);
            txtNumWhat.Name = "txtNumWhat";
            txtNumWhat.Size = new Size(108, 23);
            txtNumWhat.TabIndex = 8;
            txtNumWhat.KeyPress += numeric_KeyPress;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(152, 37);
            label6.Name = "label6";
            label6.Size = new Size(69, 15);
            label6.TabIndex = 7;
            label6.Text = "Número: 15";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(377, 68);
            label7.Name = "label7";
            label7.Size = new Size(58, 15);
            label7.TabIndex = 6;
            label7.Text = "Facebook";
            // 
            // txtFace
            // 
            txtFace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtFace.Location = new Point(439, 65);
            txtFace.Name = "txtFace";
            txtFace.Size = new Size(251, 23);
            txtFace.TabIndex = 5;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(377, 37);
            label8.Name = "label8";
            label8.Size = new Size(60, 15);
            label8.TabIndex = 4;
            label8.Text = "Instagram";
            // 
            // txtInsta
            // 
            txtInsta.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtInsta.Location = new Point(441, 34);
            txtInsta.Name = "txtInsta";
            txtInsta.Size = new Size(249, 23);
            txtInsta.TabIndex = 3;
            // 
            // txtAreaWhat
            // 
            txtAreaWhat.Location = new Point(87, 34);
            txtAreaWhat.Name = "txtAreaWhat";
            txtAreaWhat.Size = new Size(50, 23);
            txtAreaWhat.TabIndex = 1;
            txtAreaWhat.KeyPress += numeric_KeyPress;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(10, 37);
            label9.Name = "label9";
            label9.Size = new Size(71, 15);
            label9.TabIndex = 0;
            label9.Text = "C. Area: +54";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Location = new Point(1, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(714, 140);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // panelBotones
            // 
            panelBotones.Controls.Add(btnCancelar);
            panelBotones.Controls.Add(btnGuardar);
            panelBotones.Controls.Add(btnMenos);
            panelBotones.Controls.Add(btnMas);
            panelBotones.Dock = DockStyle.Bottom;
            panelBotones.Location = new Point(0, 144);
            panelBotones.Name = "panelBotones";
            panelBotones.Size = new Size(717, 54);
            panelBotones.TabIndex = 6;
            // 
            // btnCancelar
            // 
            btnCancelar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold);
            btnCancelar.Location = new Point(499, 15);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(82, 27);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold);
            btnGuardar.Location = new Point(203, 15);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(82, 27);
            btnGuardar.TabIndex = 2;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnMenos
            // 
            btnMenos.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold);
            btnMenos.Location = new Point(72, 15);
            btnMenos.Name = "btnMenos";
            btnMenos.Size = new Size(30, 27);
            btnMenos.TabIndex = 1;
            btnMenos.Text = "-";
            btnMenos.UseVisualStyleBackColor = true;
            btnMenos.Click += btnMenos_Click;
            // 
            // btnMas
            // 
            btnMas.Font = new Font("Roboto Condensed", 12F, FontStyle.Bold);
            btnMas.Location = new Point(20, 15);
            btnMas.Name = "btnMas";
            btnMas.Size = new Size(30, 27);
            btnMas.TabIndex = 0;
            btnMas.Text = "+";
            btnMas.UseVisualStyleBackColor = true;
            btnMas.Click += btnMas_Click;
            // 
            // FrmContactos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(717, 198);
            Controls.Add(panelBotones);
            Controls.Add(flowLayoutPanel1);
            Name = "FrmContactos";
            Text = "FrmContactos";
            Load += FrmContactos_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            panelBotones.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox2;
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
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel panelBotones;
        private Button btnMenos;
        private Button btnMas;
        private Button btnCancelar;
        private Button btnGuardar;
    }
}