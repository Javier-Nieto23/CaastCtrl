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


            // Verificar existencia de config.txt
            string configPath = System.IO.Path.Combine(Application.StartupPath, "config.txt");
            if (!System.IO.File.Exists(configPath))
            {
                CaastCtrl.CrearConexion crearConexion = new CaastCtrl.CrearConexion();
                crearConexion.ShowDialog();

                // Verificar nuevamente si se creó el archivo
                if (!System.IO.File.Exists(configPath))
                {
                    MessageBox.Show("No se pudo crear el archivo de configuración. La aplicación no puede continuar.",
                                    "Error de configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
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
