namespace HolaMundo2
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.usuario = new System.Windows.Forms.Label();
            this.textUsuario = new System.Windows.Forms.TextBox();
            this.contraseña = new System.Windows.Forms.Label();
            this.textClave = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // usuario
            // 
            this.usuario.AutoSize = true;
            this.usuario.Location = new System.Drawing.Point(108, 105);
            this.usuario.Name = "usuario";
            this.usuario.Size = new System.Drawing.Size(41, 13);
            this.usuario.TabIndex = 0;
            this.usuario.Text = "usuario";
            this.usuario.Click += new System.EventHandler(this.label1_Click);
            // 
            // textUsuario
            // 
            this.textUsuario.Location = new System.Drawing.Point(210, 105);
            this.textUsuario.Name = "textUsuario";
            this.textUsuario.Size = new System.Drawing.Size(100, 20);
            this.textUsuario.TabIndex = 1;
            this.textUsuario.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // contraseña
            // 
            this.contraseña.AutoSize = true;
            this.contraseña.Location = new System.Drawing.Point(89, 164);
            this.contraseña.Name = "contraseña";
            this.contraseña.Size = new System.Drawing.Size(60, 13);
            this.contraseña.TabIndex = 2;
            this.contraseña.Text = "contraseña";
            // 
            // textClave
            // 
            this.textClave.Location = new System.Drawing.Point(210, 157);
            this.textClave.Name = "textClave";
            this.textClave.PasswordChar = '*';
            this.textClave.Size = new System.Drawing.Size(100, 20);
            this.textClave.TabIndex = 3;
            this.textClave.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(397, 105);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(277, 72);
            this.button1.TabIndex = 4;
            this.button1.Text = "INICIAR SESION";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textClave);
            this.Controls.Add(this.contraseña);
            this.Controls.Add(this.textUsuario);
            this.Controls.Add(this.usuario);
            this.Name = "Form1";
            this.Text = "   ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label usuario;
        private System.Windows.Forms.TextBox textUsuario;
        private System.Windows.Forms.Label contraseña;
        private System.Windows.Forms.TextBox textClave;
        private System.Windows.Forms.Button button1;
    }
}

