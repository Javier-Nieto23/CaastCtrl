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

                    string sql = @"SELECT ci.ID_Folio,ci.Fecha_Solicitud,sf.Status_Folio FROM Control_Interno as ci  
                           
                            INNER JOIN Solicitud_Folio as sf on  ci.ID_Folio = sf.ID_Folio ";


                    SqlDataAdapter da = new SqlDataAdapter(sql, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Limpia el grid antes de volver a llenarlo
                    dataGridView1.Columns.Clear();
                    dataGridView1.Rows.Clear();

                    // Crea las columnas
                    dataGridView1.Columns.Add("ID_Folio", "ID Folio");
                    dataGridView1.Columns.Add("Fecha_Solicitud", "Fecha Solicitud");


                    // Crea la columna ComboBox para el Status
                    DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
                    comboCol.HeaderText = "Status Folio";
                    comboCol.Name = "Status_Folio";

                    // Agrega los valores posibles (puedes cambiarlos según tu BD)
                    comboCol.Items.Add("Abierto");
                    comboCol.Items.Add("En proceso");
                    comboCol.Items.Add("Finalizado");
                    dataGridView1.Columns.Add(comboCol);

                    // Nueva columna de botón "Firma"
                    DataGridViewButtonColumn btnFirma = new DataGridViewButtonColumn();
                    btnFirma.HeaderText = "Firma";
                    btnFirma.Name = "Firma";
                    btnFirma.Text = "Firmar";
                    btnFirma.UseColumnTextForButtonValue = true;
                    btnFirma.FlatStyle = FlatStyle.Popup; // hace que se vea más visual
                    dataGridView1.Columns.Add(btnFirma);


                    // Llenar las filas con los datos del DataTable
                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime fecha = Convert.ToDateTime(dr["Fecha_Solicitud"]);

                        int rowIndex = dataGridView1.Rows.Add(
                            dr["ID_Folio"].ToString(),
                            fecha.ToString("MM/dd/yyyy"),
                            dr["Status_Folio"].ToString()
                            
                        );

                        // Asignar el valor seleccionado del ComboBox
                        DataGridViewRow row = dataGridView1.Rows[rowIndex];
                        row.Cells["Status_Folio"].Value = dr["Status_Folio"].ToString();
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

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    con.Open();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue; // Saltar la fila nueva
                        string idFolio = row.Cells["ID_Folio"].Value.ToString();
                        string statusFolio = row.Cells["Status_Folio"].Value.ToString();
                        string sqlUpdate = "UPDATE Solicitud_Folio SET Status_Folio = @Status_Folio WHERE ID_Folio = @ID_Folio";
                        using (SqlCommand cmd = new SqlCommand(sqlUpdate, con))
                        {
                            cmd.Parameters.AddWithValue("@Status_Folio", statusFolio);
                            cmd.Parameters.AddWithValue("@ID_Folio", idFolio);
                            cmd.ExecuteNonQuery();
                        }
                        
                    }
                    MessageBox.Show("Folios actualizados correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los folios: " + ex.Message);
            }
        }

        //  Evento para capturar el clic en el botón "Firma"
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Firma")
            {
                string idFolio = dataGridView1.Rows[e.RowIndex].Cells["ID_Folio"].Value.ToString();

                try
                {
                    using (SqlConnection con = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                    {
                        con.Open();
                        string sql = "UPDATE Solicitud_Folio SET Firma_Recibido = 'Firmado' WHERE ID_Folio = @ID_Folio";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@ID_Folio", idFolio);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Folio {idFolio} firmado correctamente ", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al firmar el folio: " + ex.Message);
                }
            }
        }
    }
}
