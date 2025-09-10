using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.methods
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    namespace WindowsFormsApp1.methods
    {
        public class SolicitudService
        {
            private string _connectionString;

            public SolicitudService()
            {
                _connectionString = ConfigConexion.ConfigHelper.GetConnectionString();
            }

            // Obtener información principal de Control_Interno
            public DataRow ObtenerSolicitud(int idFolio)
            {
                DataTable dt = new DataTable();
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = @"SELECT ID_Folio, No_Cotizacion, No_Pedido, Razon_Social, 
                                            No_Cliente, Nombre_Contacto, Fecha_Solicitud, 
                                            Ejecutivo_Asignado, Tipo_Servicio, Tipo_Equipo
                                     FROM Control_Interno
                                     WHERE ID_Folio = @idFolio";
                        SqlDataAdapter da = new SqlDataAdapter(query, conn);
                        da.SelectCommand.Parameters.AddWithValue("@idFolio", idFolio);
                        da.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener solicitud: " + ex.Message);
                }

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }

            // Obtener detalles de Hoja_Servicio
            public DataTable ObtenerHojaServicio(int idFolio)
            {
                DataTable dt = new DataTable();
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = @"SELECT ID_Folio, Folio_Hoja, Descripcion, Censo
                                     FROM Hoja_Servicio
                                     WHERE ID_Folio = @idFolio";
                        SqlDataAdapter da = new SqlDataAdapter(query, conn);
                        da.SelectCommand.Parameters.AddWithValue("@idFolio", idFolio);
                        da.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener hoja de servicio: " + ex.Message);
                }

                return dt;
            }
        }
    }

}
