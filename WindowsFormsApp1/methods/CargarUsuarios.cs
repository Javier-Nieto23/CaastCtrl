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
    internal class CargarUsuarios
    {
        public DataTable Cargar_Usuarios()
        {
            DataTable dt = new DataTable();
            try
            {

                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {

                    conn.Open();
                    string query = "SELECT Nombre_Usuario, Tipo_Usuario FROM Usuarios_CAAST";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.Fill(dt);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los usuarios: " + ex.Message);
            }
            return dt;
        }
    }
}
