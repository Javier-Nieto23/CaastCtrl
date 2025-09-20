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
using WindowsFormsApp1.methods;
using static WindowsFormsApp1.methods.CargarFolios;
using static WindowsFormsApp1.methods.EmpresasDias;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //CargarRestantes();
            CargarFolios();

            FolioGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            FolioGrid.MultiSelect = false; // Opcional, para permitir solo una fila seleccionada

        }



        private void CargarFolios()
        {
            CargarFolios carga = new CargarFolios();
            DataTable dt = carga.ObtenerFolios();

            FolioGrid.Rows.Clear();// limpiar las filas anteriores

            foreach (DataRow row in dt.Rows)
            {
                int index = FolioGrid.Rows.Add();//Añadir nueva fila
                FolioGrid.Rows[index].Cells["IDFolio"].Value = row["ID_Folio"];
                FolioGrid.Rows[index].Cells["Fecha"].Value = ((DateTime)row["Fecha_Solicitud"]).ToString("dd/MM/yyyy");//Formateo opcional
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // abrir menu de empresas
            BtnBuscar mainForm = new BtnBuscar();
            mainForm.Show();
            
        }





        private void button3_Click(object sender, EventArgs e)
        {
            SolicitudServicio mainForm = new SolicitudServicio();
            mainForm.Show();

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form5 userForm= new Form5();
            userForm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnEditarFolio_Click(object sender, EventArgs e)
        {
            string idFolio = null; // Variable para almacenar el ID del folio seleccionado

            // Verifica que haya una fila seleccionada
            if (FolioGrid.SelectedRows.Count > 0)
            {
                // Obtén el ID del folio de la fila seleccionada
                 idFolio = FolioGrid.SelectedRows[0].Cells["IDFolio"].Value.ToString();
            }
            else if (FolioGrid.CurrentCell != null)
            {
                int rowIndex = FolioGrid.CurrentCell.RowIndex;
                idFolio = FolioGrid.Rows[rowIndex].Cells["IDFolio"].Value.ToString();
            }
            if (!string.IsNullOrEmpty(idFolio))
            {
                // Abre el formulario Form4 y pasa el ID del folio
                Form4 editForm = new Form4(idFolio);
                editForm.Show();
            }
            else
            {

                MessageBox.Show("Selecciona un folio para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string idFolio = null;

            // Verifica que haya una fila seleccionada
            if (FolioGrid.SelectedRows.Count > 0)
            {
                idFolio = FolioGrid.SelectedRows[0].Cells["IDFolio"].Value.ToString();
            }
            else if (FolioGrid.CurrentCell != null)
            {
                int rowIndex = FolioGrid.CurrentCell.RowIndex;
                idFolio = FolioGrid.Rows[rowIndex].Cells["IDFolio"].Value.ToString();
            }

            if (string.IsNullOrEmpty(idFolio))
            {
                MessageBox.Show("Selecciona un folio para borrar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"¿Seguro que quieres borrar el folio {idFolio} y todos sus datos asociados?",
                "Confirmar borrado",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

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
                            // Borrar datos asociados en Hojas_Servicio
                            string deleteHojas = "DELETE FROM Hojas_Servicio WHERE ID_Folio = @idFolio";
                            using (SqlCommand cmdHojas = new SqlCommand(deleteHojas, conn, transaction))
                            {
                                cmdHojas.Parameters.AddWithValue("@idFolio", idFolio);
                                cmdHojas.ExecuteNonQuery();
                            }

                            // Borrar folio en Control_Interno
                            string deleteFolio = "DELETE FROM Control_Interno WHERE ID_Folio = @idFolio";
                            using (SqlCommand cmdFolio = new SqlCommand(deleteFolio, conn, transaction))
                            {
                                cmdFolio.Parameters.AddWithValue("@idFolio", idFolio);
                                cmdFolio.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Folio y datos asociados borrados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Actualiza el grid
                            CargarFolios();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error al borrar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CargarFolios();
        }

        //metodo para poder filtrar las empresas buscadas 
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string filtro = textBox1.Text.Trim();
            
            if(string.IsNullOrEmpty(filtro))
            {
               EmpresasGrid.Rows.Clear(); // Cargar todas las empresas si el filtro está vacío
                return;
            }

            LicenciaService service = new LicenciaService();
            DataTable dt = service.ObtenerDiasRestantes(filtro);

            EmpresasGrid.Rows.Clear(); // Limpiar filas anteriores

            foreach (DataRow row in dt.Rows)
            {
                int index = EmpresasGrid.Rows.Add();
                EmpresasGrid.Rows[index].Cells["EmpresaNombre"].Value = row["Nombre_Empresa"];
                EmpresasGrid.Rows[index].Cells["DiasRestantes"].Value = row["Dias_Restantes"];
                EmpresasGrid.Rows[index].Cells["FechaInicio"].Value = ((DateTime)row["Fecha_Inicio"]).ToString("dd/MM/yyyy");
            }
        
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string filtro = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(filtro))
            {
                FolioGrid.Rows.Clear(); // Cargar todas las empresas si el filtro está vacío
                return;
            }

            CargarFolios service = new CargarFolios();
            DataTable dt = service.ObtenerFolios(filtro);

             FolioGrid.Rows.Clear(); // Limpiar filas anteriores

            foreach (DataRow row in dt.Rows)
            {
                int index = FolioGrid.Rows.Add();
                FolioGrid.Rows[index].Cells["IDFolio"].Value = row["ID_Folio"];

                FolioGrid.Rows[index].Cells["Fecha_Inicio"].Value = ((DateTime)row["Fecha_Inicio"]).ToString("dd/MM/yyyy");
            }


        }
    }
}