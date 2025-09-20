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
    internal class EmpresasDias
    {
        public class LicenciaService
        {
            public LicenciaService()
            {
            }

            // Devuelve un DataTable con Nombre_Empresa y Dias_Restantes
            public DataTable ObtenerDiasRestantes(string filtro = "")
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre_Empresa", typeof(string));
                dt.Columns.Add("Dias_Restantes", typeof(int));
                dt.Columns.Add("Fecha_Inicio", typeof(DateTime));

                try
                {
                    using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                    {
                        conn.Open();
                        string query = "SELECT Nombre_Empresa, Fecha_Inicio, Cantidad_Dias FROM Empresas";

                        // Aplicamos filtro si no está vacío
                        if (!string.IsNullOrEmpty(filtro))
                            query += " WHERE Nombre_Empresa LIKE @filtro OR No_Cliente LIKE @filtro";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            if (!string.IsNullOrEmpty(filtro))
                                cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                string nombre = reader["Nombre_Empresa"].ToString();
                                DateTime fechaInicio = Convert.ToDateTime(reader["Fecha_Inicio"]);
                                int cantidadDias = Convert.ToInt32(reader["Cantidad_Dias"]);

                                // Calcular días restantes
                                int diasTranscurridos = (DateTime.Now - fechaInicio).Days;
                                int diasRestantes = cantidadDias - diasTranscurridos;

                                if (diasRestantes < 0) diasRestantes = 0;

                                dt.Rows.Add(nombre, diasRestantes, fechaInicio);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al calcular días restantes: " + ex.Message);
                }

                return dt;
            }

        }
    }
}
