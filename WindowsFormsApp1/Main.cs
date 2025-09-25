using CaastCtrl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.methods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class BtnBuscar : Form
    {
        public BtnBuscar()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            // Llenar comboBox1 al iniciar
            comboBox1.Items.Add("BASICO");
            comboBox1.Items.Add("EMPRESARIAL");
            comboBox1.Items.Add("COMPACTO");

            comboBox1.SelectedIndex = 0; // Selección por defecto
            CargarEmpresas();
            dataGridView1.MultiSelect = false;

        }

        private void CargarEmpresas(string filtro = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();

                    string sql = @"SELECT Nombre_Empresa,Direccion,No_Cliente,Perfil,Cantidad_Dias,Fecha_Inicio FROM Empresas 
             
                    ";//"INNER JOIN Contacto_Empresa c ON c.ID_Empresa = e.ID_Empresa";

                    if (!string.IsNullOrEmpty(filtro))
                        sql += "  WHERE Nombre_Empresa LIKE @filtro OR No_Cliente LIKE @filtro";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        if (!string.IsNullOrEmpty(filtro))
                            cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                        //muestra la informacion de la tabla Empresa
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.Rows.Clear();

                        // Agregar cada empresa solo en Column1
                        foreach (DataRow row in dt.Rows)
                        {
                            dataGridView1.Rows.Add(
                            row["Nombre_Empresa"].ToString(),
                            row["Direccion"].ToString(),
                            row["No_Cliente"].ToString(),
                            row["Perfil"].ToString(),
                            row["Cantidad_Dias"].ToString()

                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar empresas: " + ex.Message);
            }
        }
    


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            CargarEmpresas(textBox1.Text.Trim());
        }
        

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            try

            {
                // Validación: campos obligatorios
                if (string.IsNullOrWhiteSpace(textBox3.Text)) // Nombre_Empresa
                {
                    MessageBox.Show("Por favor ingrese el nombre de la empresa.");
                    textBox3.Focus();
                    return;
                }
                // Validación: campos obligatorios
                if (string.IsNullOrWhiteSpace(textBox2.Text)) // Dirección
                {
                    MessageBox.Show("Por favor ingrese la dirección de la empresa.");
                    textBox2.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox4.Text)) // No_Cliente
                {
                    MessageBox.Show("Por favor ingrese el número de cliente.");
                    textBox4.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox5.Text)) // Cantidad_Dias
                {
                    MessageBox.Show("Por favor ingrese la cantidad de días.");
                    textBox5.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(comboBox1.Text)) // Perfil
                {
                    MessageBox.Show("Por favor ingrese el perfil de la empresa.");
                    textBox3.Focus();
                    return;
                }
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    DateTime fechaseleccionada = dateTimePicker1.Value;
                    conn.Open();
                    string query = "INSERT INTO EMPRESAS (Nombre_Empresa,Direccion,No_Cliente,Perfil,Cantidad_Dias,Fecha_Inicio) VALUES (@Nombre_Empresa,@Direccion,@No_Cliente,@Perfil,@Cantidad_Dias,@Fecha_Inicio)";


                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre_Empresa", textBox3.Text);
                        cmd.Parameters.AddWithValue("@Direccion", textBox2.Text);
                        cmd.Parameters.AddWithValue("@No_Cliente", textBox4.Text);
                        cmd.Parameters.AddWithValue("@Perfil", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@Cantidad_Dias", textBox5.Text);
                        cmd.Parameters.AddWithValue("@Fecha_Inicio", fechaseleccionada);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Empresa agregada exitosamente.");
                            // Limpiar todos los campos
                            textBox3.Clear(); // Nombre_Empresa
                            textBox2.Clear(); // Dirección
                            textBox4.Clear(); // No_Cliente
                            textBox5.Clear(); // Cantidad_Dias

                            comboBox1.SelectedIndex = 0; // Reinicia al primer valor ("BASICO")
                            dateTimePicker1.Value = DateTime.Now; // Reinicia la fecha al día actual

                            CargarEmpresas();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo agregar la empresa.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el formulario: " + ex.Message);

            }
        }

        private void BtnBuscar_Load(object sender, EventArgs e)
        {
         
        }

        private void button3_Click(object sender, EventArgs e)
        {
         
            this.Close();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            string nombreEmpresa = null;

            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener el nombre de la empresa de la fila seleccionada
                nombreEmpresa = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            }
            else if (dataGridView1.CurrentCell != null)
            {
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                nombreEmpresa = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
            }
            //validar que se selecciono una empresa
            if (string.IsNullOrEmpty(nombreEmpresa))
            {
                MessageBox.Show("Por favor, seleccione una empresa para eliminar.");
                return;
            }
            DialogResult result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar la empresa '{nombreEmpresa}'?",
                "Confirmar Modificación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();

                    string deleteQuery = @"DELETE FROM Empresas 
                                    WHERE Nombre_Empresa = @Nombre_Empresa";


                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre_Empresa", nombreEmpresa);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {

                            MessageBox.Show("Empresa eliminada exitosamente.");
                            CargarEmpresas();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar la empresa.");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar la empresa: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filtro = textBox1.Text.Trim();
            CargarEmpresas(filtro);
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            
        }
    }
}
