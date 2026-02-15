namespace Front_SGBM
{
    partial class FrmAbrirCaja
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
            lblTitulo = new Label();
            lblTipo = new Label();
            lblResponsable = new Label();
            btnCancelar = new Button();
            btnAbrir = new Button();
            lblMonto = new Label();
            cbTipo = new ComboBox();
            bindingTipoCajas = new BindingSource(components);
            cbResponsable = new ComboBox();
            bindingEmpleados = new BindingSource(components);
            txtMonto = new TextBox();
            ((System.ComponentModel.ISupportInitialize)bindingTipoCajas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingEmpleados).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.Anchor = AnchorStyles.Top;
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Roboto Condensed", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.Location = new Point(51, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(211, 29);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "APERTURA DE CAJA";
            // 
            // lblTipo
            // 
            lblTipo.AutoSize = true;
            lblTipo.Location = new Point(23, 72);
            lblTipo.Name = "lblTipo";
            lblTipo.Size = new Size(75, 15);
            lblTipo.TabIndex = 1;
            lblTipo.Text = "Tipo de Caja:";
            // 
            // lblResponsable
            // 
            lblResponsable.AutoSize = true;
            lblResponsable.Location = new Point(23, 133);
            lblResponsable.Name = "lblResponsable";
            lblResponsable.Size = new Size(76, 15);
            lblResponsable.TabIndex = 2;
            lblResponsable.Text = "Responsable:";
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCancelar.Location = new Point(23, 329);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(286, 54);
            btnCancelar.TabIndex = 3;
            btnCancelar.Tag = "btnPrincipalR";
            btnCancelar.Text = "CANCELAR";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnAbrir
            // 
            btnAbrir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnAbrir.Location = new Point(23, 258);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(286, 54);
            btnAbrir.TabIndex = 4;
            btnAbrir.Tag = "btnPrincipalV";
            btnAbrir.Text = "ABRIR CAJA";
            btnAbrir.UseVisualStyleBackColor = true;
            // 
            // lblMonto
            // 
            lblMonto.AutoSize = true;
            lblMonto.Location = new Point(23, 191);
            lblMonto.Name = "lblMonto";
            lblMonto.Size = new Size(80, 15);
            lblMonto.TabIndex = 5;
            lblMonto.Text = "Monto Inicial:";
            // 
            // cbTipo
            // 
            cbTipo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbTipo.DataSource = bindingTipoCajas;
            cbTipo.DisplayMember = "Tipo";
            cbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTipo.FormattingEnabled = true;
            cbTipo.Location = new Point(123, 69);
            cbTipo.Name = "cbTipo";
            cbTipo.Size = new Size(189, 23);
            cbTipo.TabIndex = 6;
            cbTipo.ValueMember = "IdTipo";
            // 
            // bindingTipoCajas
            // 
            bindingTipoCajas.DataSource = typeof(Entidades_SGBM.TiposCajas);
            // 
            // cbResponsable
            // 
            cbResponsable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbResponsable.DataSource = bindingEmpleados;
            cbResponsable.DisplayMember = "NombreCompleto";
            cbResponsable.DropDownStyle = ComboBoxStyle.DropDownList;
            cbResponsable.FormattingEnabled = true;
            cbResponsable.Location = new Point(123, 130);
            cbResponsable.Name = "cbResponsable";
            cbResponsable.Size = new Size(189, 23);
            cbResponsable.TabIndex = 7;
            cbResponsable.ValueMember = "IdEmpleado";
            // 
            // bindingEmpleados
            // 
            bindingEmpleados.DataSource = typeof(Entidades_SGBM.Empleados);
            // 
            // txtMonto
            // 
            txtMonto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMonto.Location = new Point(123, 188);
            txtMonto.Name = "txtMonto";
            txtMonto.Size = new Size(189, 23);
            txtMonto.TabIndex = 8;
            // 
            // FrmAbrirCaja
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(322, 410);
            Controls.Add(txtMonto);
            Controls.Add(cbResponsable);
            Controls.Add(cbTipo);
            Controls.Add(lblMonto);
            Controls.Add(btnAbrir);
            Controls.Add(btnCancelar);
            Controls.Add(lblResponsable);
            Controls.Add(lblTipo);
            Controls.Add(lblTitulo);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmAbrirCaja";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FrmAbrirCaja";
            Load += FrmAbrirCaja_Load;
            ((System.ComponentModel.ISupportInitialize)bindingTipoCajas).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingEmpleados).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Label lblTipo;
        private Label lblResponsable;
        private Button btnCancelar;
        private Button btnAbrir;
        private Label lblMonto;
        private ComboBox cbTipo;
        private ComboBox cbResponsable;
        private TextBox txtMonto;
        private BindingSource bindingTipoCajas;
        private BindingSource bindingEmpleados;
    }
}