using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using WindowsFormsApp1.methods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace WindowsFormsApp1
{
    public partial class Form5 : Form
    {
        
        public Form5()
        {

            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            // Llenar comboBox1 al iniciar
            comboBox1.Items.Add("Admin");
            comboBox1.Items.Add("Guest");
            

            comboBox1.SelectedIndex = 0; // Selección por defecto
            CargarUsuarios();
            

        }

        private void CrearUsuario_Click(object sender, EventArgs e)
        {
           
        }




        private void CargarUsuarios()
        {
            // Instancio la clase usando el constructor
            CargarUsuarios service = new CargarUsuarios();
            DataTable dt = service.Cargar_Usuarios();
            UsuariosGrid.DataSource = dt; // Asigno al DataGridView del formulario
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
        
            this.Close();
    
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos.");
                    textBox1.Focus();
                    return;
                }
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string  TipoUsuario = "";
                    //verificar que el usuario es admin

                    string sqlCheck = "SELECT Tipo_Usuario FROM Usuarios_Caast WHERE ID_Usuario = @idUsuario";
                    using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@idUsuario", LoginService.IdUsuarioActual);

                        object result = cmdCheck.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            TipoUsuario = result.ToString();
                        }

                        if (TipoUsuario != "Admin")
                        {
                            MessageBox.Show("No tienes permisos para crear usuarios.");
                            return; // cortamos aquí
                        }




                        string sqlInsert = @"INSERT INTO Usuarios_Caast (Nombre_Usuario,Password,Tipo_Usuario) 
                                       VALUES (@Nombre_Usuario,@Contraseña,@Tipo_Usuario)";
                        using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre_Usuario", textBox1.Text.Trim());
                            cmd.Parameters.AddWithValue("@Contraseña", textBox2.Text.Trim());
                            cmd.Parameters.AddWithValue("@Tipo_Usuario", comboBox1.Text.Trim());

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Usuario creado exitosamente.");
                                CargarUsuarios(); // Refresca el DataGridView
                            }
                            else
                            {
                                MessageBox.Show("No se pudo crear el usuario. Intenta de nuevo.");
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear el usuario: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string NombreUsuario = null; // Variable para almacenar el ID del usuario seleccionado
            if (UsuariosGrid.SelectedRows.Count > 0)
            {
                // Obtener el ID del usuario de la fila seleccionada
                NombreUsuario = UsuariosGrid.SelectedRows[0].Cells[0].Value.ToString();
            }
            else if (UsuariosGrid.CurrentCell != null)
            {
                int rowIndex = UsuariosGrid.CurrentCell.RowIndex;
                NombreUsuario = UsuariosGrid.Rows[rowIndex].Cells[0].Value.ToString();
            }

            // Validar que se seleccionó un usuario
            if (string.IsNullOrEmpty(NombreUsuario))
            {
                MessageBox.Show("Por favor, seleccione un usuario para eliminar.");
                return;
            }
            DialogResult result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar el usuario con ID {NombreUsuario}?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "DELETE FROM Usuarios_Caast WHERE Nombre_Usuario = @Nombre_Usuario";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre_Usuario", NombreUsuario);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Usuario eliminado exitosamente.");
                            CargarUsuarios(); // Refresca el DataGridView
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el usuario. Intenta de nuevo.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el usuario: " + ex.Message);
            }
        }
    }
}          
