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

namespace WindowsFormsApp1
{
    public partial class BtnBuscar : Form
    {
        public BtnBuscar()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            CargarEmpresas();
        }

        private void CargarEmpresas(string filtro = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();

                    string sql = @"SELECT e.Nombre_Empresa,e.No_Cliente,c.Nombre_Contacto FROM Empresas e 
             
                    INNER JOIN Contacto_Empresa c ON c.ID_Empresa = e.ID_Empresa";

                    if (!string.IsNullOrEmpty(filtro))
                        sql += "  WHERE e.Nombre_Empresa LIKE @filtro OR e.No_Cliente LIKE @filtro";

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
                            row["No_Cliente"].ToString(),
                            row["Nombre_Contacto"].ToString()
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
            SolicitudServicio mainForm = new SolicitudServicio();
            mainForm.Show();
            this.Close();
        }

        private void BtnBuscar_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // abrir menu de empresas
            Form1 mainForm = new Form1();
            mainForm.Show();
            this.Hide();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filtro = textBox1.Text.Trim();
            CargarEmpresas(filtro);
        }
    }
}
