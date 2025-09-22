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
       

        // Método que devuelve los folios (ID y Fecha)
        public DataTable ObtenerFolios(string filtroFolio = "")
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

                    if(!string.IsNullOrEmpty(filtroFolio))
                        query += " WHERE ID_Folio LIKE @filtro ";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(filtroFolio))
                            cmd.Parameters.AddWithValue("@filtro", "%" + filtroFolio + "%");
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int folio = Convert.ToInt32(reader["ID_Folio"]);
                            DateTime fecha = Convert.ToDateTime(reader["Fecha_Solicitud"]);
                            dt.Rows.Add(folio, fecha);
                        }
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
