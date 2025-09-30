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
            comboBox1.Items.Add("HDS");
            comboBox1.Items.Add("HDC");

            comboBox1.SelectedIndex = 0; // Selección por defecto

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
                    string query = "INSERT INTO Solicitud_Folio (Nombre_Empresa,Fecha_Solicitud,Descripcion,Hoja,Status_Folio) VALUES (@Nombre,@Fecha,@Descripcion,@Hoja,'Solicitado')";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", comboBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@Fecha", dateTimePicker2.Value);
                        cmd.Parameters.AddWithValue("@Descripcion", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@Hoja", comboBox1.Text.Trim());
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
