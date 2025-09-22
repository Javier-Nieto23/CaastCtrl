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
using WindowsFormsApp1.methods;

namespace CaastCtrl
{
    public partial class VentanaFolios : Form
    {
        public VentanaFolios()
        {
            InitializeComponent();
            GridFolios();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void GridFolios() 
        {
            try 
            {
                using (SqlConnection con = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    con.Open();

                    string sql = @"SELECT ID_Folio,Fecha_Solicitud FROM Control_Interno";

                    SqlDataAdapter da = new SqlDataAdapter(sql, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.Rows.Clear();

                    foreach (DataRow dr in dt.Rows)
                    {
                        //convierte la fecha al formato dd/MM/yyyy
                        DateTime fecha = Convert.ToDateTime(dr["Fecha_Solicitud"]);

                        dataGridView1.Rows.Add(
                        dr["ID_Folio"].ToString(),
                        fecha.ToString("MM/dd/yyyy")
                        );
                    }

                }
            }
            catch (Exception ex) 
            { 
                MessageBox.Show("Error al cargar los folios: " + ex.Message);
               

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            GridFolios();
        }
    }
}
