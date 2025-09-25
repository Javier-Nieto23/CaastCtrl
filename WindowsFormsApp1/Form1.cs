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
using CaastCtrl;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;


            FolioGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            FolioGrid.MultiSelect = false; // Opcional, para permitir solo una fila seleccionada

        }



        private void button1_Click(object sender, EventArgs e)
        {
            // abrir menu de empresas
            BtnBuscar mainForm = new BtnBuscar();
            mainForm.Show();
            
        }





        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
            {
                conn.Open();
                string TipoUsuario = "";

                string query = "SELECT Tipo_Usuario FROM Usuarios_Caast WHERE ID_Usuario = @idUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    LoginService loginService = new LoginService();
                    cmd.Parameters.AddWithValue("@idUsuario", LoginService.IdUsuarioActual);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        TipoUsuario = reader["Tipo_Usuario"].ToString();
                    }
                }

                // Ahora sí comparas el valor obtenido
                if (TipoUsuario != "Admin")
                {
                    MessageBox.Show("No puedes ingresar si no eres admin");
                }
                else
                {
                    SolicitudServicio mainForm = new SolicitudServicio();
                    mainForm.Show();
                }
            }
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

                            //CargarFolios folios = new CargarFolios();   
                            // Actualiza el grid
                            //folios.ObtenerFolios();
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
          VentanaFolios folios = new VentanaFolios();
            folios.Show();  
        }

        //metodo para poder filtrar las empresas buscadas 
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string filtroEmpresa = textBox1.Text.Trim();
            
            if(string.IsNullOrEmpty(filtroEmpresa))
            {
               EmpresasGrid.Rows.Clear(); // Cargar todas las empresas si el filtro está vacío
                return;
            }

            LicenciaService service = new LicenciaService();
            DataTable dt = service.ObtenerDiasRestantes(filtroEmpresa);

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
            string filtroFolio = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(filtroFolio))
            {
                FolioGrid.Rows.Clear(); // Cargar todas las empresas si el filtro está vacío
                return;
            }

            CargarFolios service = new CargarFolios();
            DataTable dt = service.ObtenerFolios(filtroFolio);

             FolioGrid.Rows.Clear(); // Limpiar filas anteriores

            foreach (DataRow row in dt.Rows)
            {
                int index = FolioGrid.Rows.Add();
                FolioGrid.Rows[index].Cells["IDFolio"].Value = row["ID_Folio"];

                FolioGrid.Rows[index].Cells["Fecha"].Value = ((DateTime)row["Fecha_Solicitud"]).ToString("dd/MM/yyyy");
            }


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            SocilitarFolio solicitarform = new SocilitarFolio();
            solicitarform.Show();
        }
    }
}