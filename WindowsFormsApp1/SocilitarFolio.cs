using MySqlX.XDevAPI;
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
using WindowsFormsApp1;

namespace CaastCtrl
{
    public partial class SocilitarFolio : Form
    {
        public SocilitarFolio()
        {
            
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += SolicitarFolio_Load;
            comboBox1.Items.Add("HDS");
            comboBox1.Items.Add("HDC");



            comboBox1.SelectedIndex = 0; // Selección por defecto


            comboBox2.DropDownStyle = ComboBoxStyle.DropDown;
            comboBox2.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;

        }


        private void SolicitarFolio_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT Nombre_Empresa FROM Empresas";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    comboBox2.Items.Clear();

                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader["Nombre_Empresa"].ToString());
                    }

                    reader.Close();
                }
                CargarContactos();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error de SQL: " + sqlEx.Message);
            }

        }

        private void CargarContactos()
        {
            try
            {
                //Abre la conexion SQL usando 
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                     
                    conn.Open();
                    string query = "SELECT Nombre_Contacto FROM Contacto_Empresa";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    comboBox3.Items.Clear();

                    while (reader.Read())
                    {
                        comboBox3.Items.Add(reader["Nombre_Contacto"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al cargar ejecutivos: " + sqlEx.Message);
            }
        }


        private void cmbProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                string empresaSeleccionada = comboBox2.SelectedItem.ToString();

                try
                {
                    using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                    {
                        conn.Open();

  
                        // Obtener contactos de la empresa
                        string queryContactos = "SELECT c.Nombre_Contacto FROM Contacto_Empresa c " +
                            "INNER JOIN Empresas e on e.ID_Empresa = c.ID_Empresa " +
                            "WHERE e.Nombre_Empresa = @empresa";
                        SqlCommand cmdContactos = new SqlCommand(queryContactos, conn);
                        cmdContactos.Parameters.AddWithValue("@empresa", empresaSeleccionada);

                        SqlDataReader reader = cmdContactos.ExecuteReader();
                        comboBox3.Items.Clear();

                        while (reader.Read())
                        {
                            comboBox3.Items.Add(reader["Nombre_Contacto"].ToString());
                        }

                        reader.Close();
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Error al obtener datos de la empresa: " + sqlEx.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GenerarFolio()
        {
            
            try
            {
                
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {

                    conn.Open();
                    string query = "INSERT INTO Solicitud_Folio (Nombre_Empresa,Fecha_Solicitud,Descripcion,Hoja,Status_Folio,Ejecutivo,Nombre_Contacto,Firma_Recibido) VALUES (@Nombre,@Fecha,@Descripcion,@Hoja,'Solicitado',@Ejecutivo,@Nombre_Contacto,'Por Firmar')";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@Nombre", comboBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@Fecha", dateTimePicker2.Value);
                        cmd.Parameters.AddWithValue("@Descripcion", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@Hoja", comboBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@Ejecutivo", LoginService.IdUsuarioActual);
                        cmd.Parameters.AddWithValue("@Nombre_Contacto", comboBox3.Text.Trim());
                        // Usar el ID del usuario actual
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Folio solicitado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close(); // Cerrar el formulario después de guardar
                        }
                        else
                        {   
                            MessageBox.Show("No se pudo solicitar el folio.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al solicitar el folio: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GenerarFolio();
        }
    }
}
