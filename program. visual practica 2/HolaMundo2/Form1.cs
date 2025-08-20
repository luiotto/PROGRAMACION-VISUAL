using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolaMundo2
{
    public partial class Form1 : Form
    {
        private int intentosFallidos = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string usuario, clave;
            usuario = textUsuario.Text;
            clave = textClave.Text;

            if (usuario == "admin" && clave == "admin123")
            {
                MessageBox.Show
                    ("Bienvenido " + usuario);
            }
            else
            {
                intentosFallidos++;

                if (intentosFallidos >= 3)
                {
                    MessageBox.Show("numero maximo de intentos alcanzado. el programa se cerrara.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"usuario o contraseña incorrectos. Intentos restantes: {3 - intentosFallidos}", "error de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}