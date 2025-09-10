using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.methods
{
    internal class CargarFolios
    {
        public CargarFolios()
        {
        }

        // Método que devuelve los folios (ID y Fecha)
        public DataTable ObtenerFolios()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_Folio", typeof(int));
            dt.Columns.Add("Fecha_Solicitud", typeof(DateTime));

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT ID_Folio, Fecha_Solicitud FROM Control_Interno";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int folio = Convert.ToInt32(reader["ID_Folio"]);
                        DateTime fecha = Convert.ToDateTime(reader["Fecha_Solicitud"]);

                        dt.Rows.Add(folio, fecha);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los folios: " + ex.Message);
            }

            return dt;
        }
    }
}
