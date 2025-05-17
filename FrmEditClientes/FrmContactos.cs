using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Front_SGBM
{
    public partial class FrmContactos : Form
    {
        public EnumModoForm modo = EnumModoForm.Alta;
        public List<Contactos>? _contactos = null;
        private int contadorReplicas = 1;

        //Valores de Carga
        private string areaWhat = string.Empty;
        private string area = string.Empty;
        private string whatsapp = string.Empty;
        private string fijo = string.Empty;
        private string email = string.Empty;
        private string instagram = string.Empty;
        private string facebook = string.Empty;

        public FrmContactos()
        {
            InitializeComponent();
        }

        //Métodos
        private void FrmContactos_Load(object sender, EventArgs e)
        {
            if (modo == EnumModoForm.Modificacion)
            {
                cargarContactos(sender, e);
            }
            
        }

        private void cargarContactos(object sender, EventArgs e)
        {
            if (_contactos == null)
            {
                return;
            }
            if (_contactos.Count == 0)
            {
                return;
            }
            extraerValoresContacto(_contactos[0]);
            txtAreaWhat.Text = areaWhat;
            txtNumWhat.Text = whatsapp;
            txtAreaFijo.Text = area;
            txtNumFijo.Text = fijo;
            txtInsta.Text = instagram;
            txtFace.Text = facebook;
            txtEmail.Text = email;
            vaciarValores();
            if (_contactos.Count == 1)
            {
                return;
            }
            for (int i = 1;  i < _contactos.Count; i++)
            {
                extraerValoresContacto(_contactos[i]);
                btnMas_Click(sender, e);
            }
            
        }

        private void extraerValoresContacto(Contactos contacto)
        {
            instagram = contacto.Instagram ?? "";
            facebook = contacto.Facebook ?? "";
            email = contacto.Email ?? "";
            if (String.IsNullOrWhiteSpace(contacto.Telefono))
            {
                area = "";
                fijo = "";
            } else
            {
                string[] partes = contacto.Telefono.Split('-');
                if (partes.Length == 2)
                {
                    area = partes[0];
                    fijo = partes[1];
                }
                else
                {
                    area = "";
                    fijo = "";
                }

            }
            if (String.IsNullOrWhiteSpace(contacto.Whatsapp))
            {
                areaWhat = "";
                whatsapp = "";
            } else
            {
                string[] partes = contacto.Whatsapp.Split('-');
                if (partes.Length == 2)
                {
                    areaWhat = partes[0];
                    whatsapp = partes[1];
                }
                else
                {
                    areaWhat = "";
                    whatsapp = "";
                }

            }

        }

        private void vaciarValores()
        {
            areaWhat = string.Empty;
            area = string.Empty;
            whatsapp = string.Empty;
            fijo = string.Empty;
            email = string.Empty;
            instagram = string.Empty;
            facebook = string.Empty;
        }

        //Botones
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea cancelr la carga de contactos? (no se harán modificaciones)", "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                return;
            }
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("¿Desea Guardar los Cambios?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No)
            {
                return;
            }
            _contactos = new List<Contactos>();
            int? contador = null;
            foreach(Control groupBox in flowLayoutPanel1.Controls)
            {
                vaciarValores();
                if (groupBox is GroupBox)
                {
                    if (groupBox.Name != "groupBoxBotones")
                    {
                        Contactos contacto = new();
                        bool vacio = true;
                        foreach (Control control in groupBox.Controls)
                        {
                            if (control is TextBox textBox)
                            {
                                if (textBox.Name == "txtAreaWhat"+contador)
                                {
                                    areaWhat = textBox.Text;
                                }
                                if (textBox.Name == "txtNumWhat" + contador)
                                {
                                    whatsapp = textBox.Text;
                                    
                                }
                                if (textBox.Name == "txtAreaFijo" + contador)
                                {
                                    area = textBox.Text;
                                }
                                if (textBox.Name == "txtNumFijo" + contador)
                                {
                                    fijo = textBox.Text;
                                }
                                if (textBox.Name == "txtInsta" + contador)
                                {
                                    instagram = textBox.Text;
                                    if (!String.IsNullOrWhiteSpace(instagram))
                                    {
                                        vacio = false;
                                        contacto.Instagram = instagram;
                                    }
                                }
                                if (textBox.Name == "txtFace" + contador)
                                {
                                    facebook = textBox.Text;
                                    if (!String.IsNullOrWhiteSpace(facebook))
                                    {
                                        vacio = false;
                                        contacto.Facebook = facebook;
                                    }
                                }
                                if (textBox.Name == "txtEmail" + contador)
                                {
                                    email = textBox.Text;
                                    if (!String.IsNullOrWhiteSpace(email))
                                    {
                                        vacio = false;
                                        contacto.Email = email;
                                    }
                                }
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(whatsapp))
                        {
                            vacio = false;
                            if (String.IsNullOrWhiteSpace(areaWhat))
                            {
                                areaWhat = "+543446";
                            }
                            contacto.Whatsapp = $"{areaWhat}-{whatsapp}";
                        }
                        if (!String.IsNullOrWhiteSpace(fijo))
                        {
                            vacio = false;
                            if (String.IsNullOrWhiteSpace(area))
                            {
                                area = "3446";
                            }
                            contacto.Telefono = $"{area}-{fijo}";
                        }
                        if (!vacio)
                        {
                            _contactos.Add(contacto);
                        }
                    }
                    if (contador == null)
                    {
                        contador = 0;
                    }
                    contador++;
                }
            }
            FrmEditClientes frmEditClientes = Application.OpenForms.OfType<FrmEditClientes>().FirstOrDefault();
            if (frmEditClientes != null)
            {
                frmEditClientes.traerContactos(_contactos);
            }
            this.Close();

        }

        private void btnMenos_Click(object sender, EventArgs e)
        {
            if (contadorReplicas < 2)
            {
                return;
            }
            foreach(Control groupBox in flowLayoutPanel1.Controls)
            {
                if (groupBox.Name == "groupBox" + contadorReplicas)
                {
                    flowLayoutPanel1.Controls.Remove(groupBox);
                }
            }
            this.Height -= groupBox1.Height + 10;
            contadorReplicas--;
        }

        private void btnMas_Click(object sender, EventArgs e)
        {
            if (contadorReplicas < 5)
            {
                //Ajusto la altura del formulario
                this.Height += groupBox1.Height + 10; 
                //Creo un nuevo groupBox
                GroupBox nuevoGroupBox = new GroupBox();
                nuevoGroupBox.Text = $"Contacto Nro {contadorReplicas + 1}"; //Cambio el texto de referencia
                nuevoGroupBox.Name = $"groupBox{contadorReplicas + 1}"; //Le asigno ID incremental
                nuevoGroupBox.Location = new Point(groupBox1.Location.X, groupBox1.Location.Y
                    + (groupBox1.Height + 10) * contadorReplicas); //Ajusto la posicion
                nuevoGroupBox.Size = groupBox1.Size; //Ajusto el tamaño
                nuevoGroupBox.BackColor = groupBox1.BackColor; //Ajusto el color de fondo para futuros diseños

                //Ahora clono los controles
                foreach (Control control in groupBox1.Controls)
                {
                    Control nuevo = (Control)Activator.CreateInstance(control.GetType());
                    if (control is TextBox)
                    {
                        TextBox txt = (TextBox)nuevo;
                        txt.Location = control.Location;
                        txt.Size = control.Size;
                        txt.Name = control.Name + contadorReplicas.ToString();
                        txt.Font = control.Font;
                        if (control.Name == "txtAreaWhat")
                        {
                            txt.KeyPress += numeric_KeyPress;
                            txt.Text = areaWhat;
                            txt.TabIndex = contadorReplicas * 7 + 1;
                        }
                        if (control.Name == "txtNumWhat")
                        {
                            txt.KeyPress += numeric_KeyPress;
                            txt.Text = whatsapp;
                            txt.TabIndex = contadorReplicas * 7 + 2;
                        }
                        if (control.Name == "txtAreaFijo")
                        {
                            txt.KeyPress += numeric_KeyPress;
                            txt.Text = area;
                            txt.TabIndex = contadorReplicas * 7 + 3;
                        }
                        if (control.Name == "txtNumFijo")
                        {
                            txt.KeyPress += numeric_KeyPress;
                            txt.Text = fijo;
                            txt.TabIndex = contadorReplicas * 7 + 4;
                        }
                        if (control.Name == "txtEmail")
                        {
                            txt.Validating += txtEmail_Validating;
                            txt.Text = email;
                            txt.TabIndex = contadorReplicas * 7 + 7;
                        }
                        if (control.Name == "txtFace")
                        {
                            txt.Text = facebook;
                            txt.TabIndex = contadorReplicas * 7 + 6;
                        }
                        if (control.Name == "txtInsta")
                        {
                            txt.Text = instagram;
                            txt.TabIndex = contadorReplicas * 7 + 5;
                        }
                        nuevo = txt;
                    } else if (control is Label original)
                    {
                        Label nuevoLabel = new Label();
                        //copio caracteristicas en el label nuevo
                        nuevoLabel.Text = original.Text;
                        nuevoLabel.Name = control.Name + contadorReplicas.ToString();
                        nuevoLabel.Font = original.Font;
                        nuevoLabel.Location = original.Location;
                        nuevoLabel.Size = original.Size;
                        nuevo = nuevoLabel;
                    } else
                    {
                        nuevo.Location = control.Location;
                        nuevo.Size = control.Size;
                        nuevo.Name = control.Name + contadorReplicas.ToString();
                    }
                    nuevoGroupBox.Controls.Add(nuevo);
                }
                flowLayoutPanel1.Controls.Add(nuevoGroupBox);
                contadorReplicas++;
                vaciarValores();
            }
        }

        //Validadores de Campos
        private void numeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            TextBox currenttb = (TextBox)sender;
            if (currenttb.Text.Length > 0)
            {
                Regex emailRegex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
                if (!emailRegex.IsMatch(currenttb.Text))
                {
                    MessageBox.Show("El E-mail ingresado no tiene el formato correcto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

    }
}
