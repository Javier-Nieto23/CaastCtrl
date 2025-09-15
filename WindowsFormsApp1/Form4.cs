using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form4 : Form
    {
        private string _idFolio;

        public Form4(string idFolio)
        {
            InitializeComponent();
            _idFolio = idFolio;
            this.StartPosition = FormStartPosition.CenterScreen;
            CargarDatosFolio();
        }

        private void CargarDatosFolio()
        {
            // Leer la cadena de conexión desde config.txt
            string filePath = Path.Combine(Application.StartupPath, "config.txt");
            var config = new System.Collections.Generic.Dictionary<string, string>();
            foreach (string line in File.ReadAllLines(filePath))
            {
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
                {
                    var parts = line.Split('=');
                    config[parts[0].Trim()] = parts[1].Trim();
                }
            }
            string connStr = $"Server={config["Server"]};Database={config["Database"]};User Id={config["User Id"]};Password={config["Password"]};";

            // --- Configurar dgvServicios ---
            dgvServicios.Columns.Clear();


            // Columna Tipo_Servicio (ComboBox)
            DataGridViewComboBoxColumn tipoServicioCol = new DataGridViewComboBoxColumn();
            tipoServicioCol.Name = "Tipo_Servicio";
            tipoServicioCol.HeaderText = "Tipo de Servicio";
            dgvServicios.Columns.Add(tipoServicioCol);

            // Columna Tipo_Equipo (ComboBox)
            DataGridViewComboBoxColumn tipoEquipoCol = new DataGridViewComboBoxColumn();
            tipoEquipoCol.Name = "Tipo_Equipo";
            tipoEquipoCol.HeaderText = "Tipo de Equipo";
            dgvServicios.Columns.Add(tipoEquipoCol);

            // Columna Descripcion (texto)
            DataGridViewTextBoxColumn descripcionCol = new DataGridViewTextBoxColumn();
            descripcionCol.Name = "Descripcion";
            descripcionCol.HeaderText = "Descripción";
            dgvServicios.Columns.Add(descripcionCol);




            // --- Extraer opciones para los ComboBox desde la base de datos ---
            var equipos = new HashSet<string>();
            var servicios = new HashSet<string>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = @"
SELECT TOP 1 hs.Descripcion, ci.Tipo_Equipo, ci.Tipo_Servicio
FROM Control_Interno as ci
INNER JOIN Hojas_Servicio as hs ON ci.ID_Folio = hs.ID_Folio
WHERE ci.ID_Folio = @idFolio";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Agrega opciones únicas a los ComboBox
                            if (reader["Tipo_Equipo"] != DBNull.Value)
                                equipos.Add(reader["Tipo_Equipo"].ToString());
                            if (reader["Tipo_Servicio"] != DBNull.Value)
                                servicios.Add(reader["Tipo_Servicio"].ToString());

                            // Agrega la fila al grid
                            dgvServicios.Rows.Add(

                                reader["Tipo_Servicio"]?.ToString() ?? "",
                                reader["Tipo_Equipo"]?.ToString() ?? "",
                                reader["Descripcion"]?.ToString() ?? ""
                                
                            );
                        }
                    }
                }
            }

            // Puedes agregar opciones adicionales manualmente si lo deseas
            tipoEquipoCol.Items.AddRange(equipos.ToArray());
            tipoEquipoCol.Items.Add("Escritorio");
            tipoEquipoCol.Items.Add("Portatil");
            tipoEquipoCol.Items.Add("Servidor");
            // ... agrega más si lo necesitas

            tipoServicioCol.Items.AddRange(servicios.ToArray());
            tipoServicioCol.Items.Add("Reparación");
            tipoServicioCol.Items.Add("Mantenimiento");
            tipoServicioCol.Items.Add("Instalación");
            // ... agrega más si lo necesitas

            // --- Mostrar solo censos marcados como 'Si' en dgvHoja ---
            dgvHoja.Rows.Clear();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT Folio_Hoja FROM Hojas_Servicio WHERE ID_Folio = @idFolio AND Censo = 'Si'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dgvHoja.Rows.Add(reader["Folio_Hoja"].ToString());
                        }


                    }

                }
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT Folio_Hoja FROM Hojas_Servicio WHERE ID_Folio = @idFolio AND Censo = 'NO'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBox8.Text = reader["Folio_Hoja"].ToString();

                        }
                    }
                }
            }
        


            

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT * FROM Control_Interno WHERE ID_Folio = @idFolio";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtCotizacion2.Text = reader["No_Cotizacion"].ToString();
                            txtPedido.Text = reader["No_Pedido"].ToString();
                            cmbRazon_Social2.Text = reader["Razon_Social"].ToString();
                            textBox7.Text = reader["No_Cliente"].ToString();
                            cmbNombre_Contacto.Text = reader["Nombre_Contacto"].ToString();
                            cmbEjecutivo.Text = reader["Ejecutivo_Asignado"].ToString();
                            dateTimePicker1.Value = reader["Fecha_Solicitud"] != DBNull.Value ? (DateTime)reader["Fecha_Solicitud"] : DateTime.Now;

                        }
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Leer la cadena de conexión desde config.txt
            string filePath = Path.Combine(Application.StartupPath, "config.txt");
            var config = new System.Collections.Generic.Dictionary<string, string>();
            foreach (string line in File.ReadAllLines(filePath))
            {
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
                {
                    var parts = line.Split('=');
                    config[parts[0].Trim()] = parts[1].Trim();
                }
            }
            string connStr = $"Server={config["Server"]};Database={config["Database"]};User Id={config["User Id"]};Password={config["Password"]};";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Actualizar datos principales del folio en Control_Interno
                            string updateFolio = @"
                                UPDATE Control_Interno SET
                                    No_Cotizacion = @NoCotizacion,
                                    No_Pedido = @NoPedido,
                                    Razon_Social = @RazonSocial,
                                    No_Cliente = @NoCliente,
                                    Nombre_Contacto = @NombreContacto,
                                    Fecha_Solicitud = @FechaSolicitud,
                                    Ejecutivo_Asignado = @Ejecutivo
                                WHERE ID_Folio = @idFolio";
                            using (SqlCommand cmd = new SqlCommand(updateFolio, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@NoCotizacion", txtCotizacion2.Text);
                                cmd.Parameters.AddWithValue("@NoPedido", txtPedido.Text);
                                cmd.Parameters.AddWithValue("@RazonSocial", cmbRazon_Social2.Text);
                                cmd.Parameters.AddWithValue("@NoCliente", textBox7.Text);
                                cmd.Parameters.AddWithValue("@NombreContacto", cmbNombre_Contacto.Text);
                                cmd.Parameters.AddWithValue("@FechaSolicitud", dateTimePicker1.Value);
                                
                                cmd.Parameters.AddWithValue("@Ejecutivo", cmbEjecutivo.Text);
                                cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                                cmd.ExecuteNonQuery();
                            }

                            string UpdateFolioHoja = @"
                                UPDATE Hojas_Servicio SET
                                    Folio_Hoja = @FolioHoja
                                WHERE ID_Folio = @idFolio AND Censo = 'NO'";
                            using (SqlCommand cmd = new SqlCommand(UpdateFolioHoja, conn, transaction))

                            {
                                cmd.Parameters.AddWithValue("@FolioHoja", string.IsNullOrEmpty(textBox8.Text) ? (object)DBNull.Value : textBox8.Text);
                                cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                                cmd.ExecuteNonQuery();
                            }


                                // Actualizar datos de servicios (solo la primera fila, puedes adaptar para varias)
                                if (dgvServicios.Rows.Count > 0 && !dgvServicios.Rows[0].IsNewRow)
                            {
                                var tipoServicio = dgvServicios.Rows[0].Cells["Tipo_Servicio"].Value?.ToString();
                                var tipoEquipo = dgvServicios.Rows[0].Cells["Tipo_Equipo"].Value?.ToString();
                                


                                string updateServicio = @"
                                    UPDATE Control_Interno SET
                                        Tipo_Servicio = @TipoServicio,
                                        Tipo_Equipo = @TipoEquipo
                                        
                                    WHERE ID_Folio = @idFolio";
                                using (SqlCommand cmd = new SqlCommand(updateServicio, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@TipoServicio", string.IsNullOrEmpty(tipoServicio) ? (object)DBNull.Value: tipoServicio);
                                    cmd.Parameters.AddWithValue("@TipoEquipo", string.IsNullOrEmpty(tipoEquipo) ? (object)DBNull.Value:tipoEquipo);
                                   
                                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            if (dgvServicios.Rows.Count > 0 && !dgvServicios.Rows[0].IsNewRow)
                            {
                                var Descripcion = dgvServicios.Rows[0].Cells["Descripcion"].Value?.ToString();
                                

                                string updateCenso = @"
                                    UPDATE Hojas_Servicio SET
                                        Descripcion = @Descripcion   
                                    WHERE ID_Folio = @idFolio";
                                using (SqlCommand cmd = new SqlCommand(updateCenso, conn, transaction))
                                {
                                    
                                    cmd.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(Descripcion) ? (object)DBNull.Value : Descripcion);
                                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("Datos actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("No Puede Existir una solicitud de servicio sin Hoja de Servicio o Censo ", "Error", MessageBoxButtons.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            // Verificar que el TextBox no esté vacío
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                // Crear una nueva fila
                int index = dgvHoja.Rows.Add();

                // Asignar el valor del TextBox a la primera columna (por ejemplo, Folio)
                dgvHoja.Rows[index].Cells["HojaCenso"].Value = textBox1.Text;

                // Limpiar el TextBox
                textBox1.Clear();
                textBox1.Focus();
            }
            else
            {
                MessageBox.Show("Ingrese un número de hoja de servicio.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
