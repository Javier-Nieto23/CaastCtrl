namespace WindowsFormsApp1
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.BtnIniciarSesion = new System.Windows.Forms.Button();
            this.BtnCancelar = new System.Windows.Forms.Button();
            this.cmbUsuario = new System.Windows.Forms.TextBox();
            this.cmbPassword = new System.Windows.Forms.TextBox();
            this.BtnUsuario = new System.Windows.Forms.Label();
            this.BtnPassword = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnIniciarSesion
            // 
            this.BtnIniciarSesion.Location = new System.Drawing.Point(22, 135);
            this.BtnIniciarSesion.Name = "BtnIniciarSesion";
            this.BtnIniciarSesion.Size = new System.Drawing.Size(61, 23);
            this.BtnIniciarSesion.TabIndex = 0;
            this.BtnIniciarSesion.Text = "Iniciar ";
            this.BtnIniciarSesion.UseVisualStyleBackColor = true;
            this.BtnIniciarSesion.Click += new System.EventHandler(this.BtnIniciarSesion_Click);
            // 
            // BtnCancelar
            // 
            this.BtnCancelar.Location = new System.Drawing.Point(87, 135);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(61, 23);
            this.BtnCancelar.TabIndex = 1;
            this.BtnCancelar.Text = "Cancelar";
            this.BtnCancelar.UseVisualStyleBackColor = true;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // cmbUsuario
            // 
            this.cmbUsuario.Location = new System.Drawing.Point(22, 37);
            this.cmbUsuario.Name = "cmbUsuario";
            this.cmbUsuario.Size = new System.Drawing.Size(126, 20);
            this.cmbUsuario.TabIndex = 2;
            // 
            // cmbPassword
            // 
            this.cmbPassword.Location = new System.Drawing.Point(22, 99);
            this.cmbPassword.Name = "cmbPassword";
            this.cmbPassword.PasswordChar = '●';
            this.cmbPassword.Size = new System.Drawing.Size(126, 20);
            this.cmbPassword.TabIndex = 3;
            this.cmbPassword.TextChanged += new System.EventHandler(this.cmbPassword_TextChanged);
            // 
            // BtnUsuario
            // 
            this.BtnUsuario.AutoSize = true;
            this.BtnUsuario.Location = new System.Drawing.Point(19, 12);
            this.BtnUsuario.Name = "BtnUsuario";
            this.BtnUsuario.Size = new System.Drawing.Size(43, 13);
            this.BtnUsuario.TabIndex = 4;
            this.BtnUsuario.Text = "Usuario";
            // 
            // BtnPassword
            // 
            this.BtnPassword.AutoSize = true;
            this.BtnPassword.Location = new System.Drawing.Point(19, 71);
            this.BtnPassword.Name = "BtnPassword";
            this.BtnPassword.Size = new System.Drawing.Size(61, 13);
            this.BtnPassword.TabIndex = 5;
            this.BtnPassword.Text = "Contraseña";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(171, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(223, 189);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Sistema desarrollado por el equipo de CAAST";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(318, 217);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Version 0.0.1A";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 239);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.BtnPassword);
            this.Controls.Add(this.BtnUsuario);
            this.Controls.Add(this.cmbPassword);
            this.Controls.Add(this.cmbUsuario);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.BtnIniciarSesion);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login";
            this.Text = "CaastCtrl";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnIniciarSesion;
        private System.Windows.Forms.Button BtnCancelar;
        private System.Windows.Forms.TextBox cmbUsuario;
        private System.Windows.Forms.TextBox cmbPassword;
        private System.Windows.Forms.Label BtnUsuario;
        private System.Windows.Forms.Label BtnPassword;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}