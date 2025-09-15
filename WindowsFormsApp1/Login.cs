using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp1.conexion;


namespace WindowsFormsApp1
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.AcceptButton = BtnIniciarSesion; // Al presionar Enter se ejecuta btnLogin_Click
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void BtnIniciarSesion_Click(object sender, EventArgs e)
        {
            //constructor para conectar con el codigo validarconexion
            LoginService loginService = new LoginService();

            if (loginService.ValidarLogin(cmbUsuario.Text, cmbPassword.Text))
            {
                this.DialogResult = DialogResult.OK; // Indica que el login fue exitoso
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cmbPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
    
}
