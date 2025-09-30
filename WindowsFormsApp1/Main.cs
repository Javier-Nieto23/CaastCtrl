using CaastCtrl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.methods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class BtnBuscar : Form
    {
        private int? empresaSeleccionadaId = null;
        public BtnBuscar()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            // Llenar comboBox1 al iniciar
            comboBox1.Items.Add("BASICO");
            comboBox1.Items.Add("EMPRESARIAL");
            comboBox1.Items.Add("COMPACTO");
            //Opciones del Grid

            dataGridView1.MultiSelect = false;                   // Una sola fila a la vez
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;  // Selecciona toda la fila
            dataGridView1.ReadOnly = true;                      // Evita edición directa
            dataGridView1.AllowUserToAddRows = false;           // Evita fila vacía al final
            dataGridView1.CellClick += dataGridView1_CellClick;

            comboBox1.SelectedIndex = 0; // Selección por defecto
            CargarEmpresas();




        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // validar que no sea header
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
                empresaSeleccionadaId = Convert.ToInt32(fila.Cells[0].Value);
            }
        }

        private void CargarEmpresas(string filtro = "")
        {
            try
            {
                // Limpiar DataGridView y definir columnas
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();

                dataGridView1.Columns.Add("ID_Empresa", "ID");
                dataGridView1.Columns.Add("Nombre_Empresa", "Empresa");
                dataGridView1.Columns.Add("Nombre_Corto", "Nombre Corto");
                dataGridView1.Columns.Add("Direccion", "Dirección");
                dataGridView1.Columns.Add("No_Cliente", "No Cliente");
                dataGridView1.Columns.Add("Perfil", "Perfil");
                dataGridView1.Columns.Add("Fecha_Inicio", "Fecha Inicio");
                dataGridView1.Columns.Add("Cantidad_Dias", "Cantidad Días");
                dataGridView1.Columns.Add("Dias_Restantes", "Días Restantes");

                dataGridView1.Columns[0].Visible = false; // Ocultar ID

                // Obtener información básica de Empresas
                DataTable dtEmpresas = new DataTable();
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string sql = @"SELECT ID_Empresa, Nombre_Empresa, Nombre_Corto, Direccion, No_Cliente, Perfil, Cantidad_Dias, Fecha_Inicio FROM Empresas";

                    if (!string.IsNullOrEmpty(filtro))
                    {
                        sql += " WHERE Nombre_Empresa LIKE @filtro OR No_Cliente LIKE @filtro OR Nombre_Corto LIKE @filtro";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        if (!string.IsNullOrEmpty(filtro))
                            cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dtEmpresas);
                    }
                }

                // Obtener días restantes usando EmpresasDias
                EmpresasDias.LicenciaService servicioDias = new EmpresasDias.LicenciaService();
                DataTable dtDias = servicioDias.ObtenerDiasRestantes(filtro);

                // Combinar la información básica con los días restantes
                foreach (DataRow empresa in dtEmpresas.Rows)
                {
                    DataRow[] diasFila = dtDias.Select($"Nombre_Empresa = '{empresa["Nombre_Empresa"]}'");
                    int diasRestantes = diasFila.Length > 0 ? Convert.ToInt32(diasFila[0]["Dias_Restantes"]) : 0;

                    dataGridView1.Rows.Add(
                        empresa["Id_Empresa"],        // 0
                        empresa["Nombre_Empresa"],    // 1
                        empresa["Nombre_Corto"],     // 2
                        empresa["Direccion"],         // 3
                        empresa["No_Cliente"],        // 4
                        empresa["Perfil"],            // 5
                        empresa["Fecha_Inicio"],      // 6
                        empresa["Cantidad_Dias"],     // 7
                        diasRestantes                 // 8
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar empresas: " + ex.Message);
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            CargarEmpresas(textBox1.Text.Trim());
        }
        

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Por favor ingrese el nombre de la empresa.");
                    textBox3.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Por favor ingrese la dirección de la empresa.");
                    textBox2.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Por favor ingrese el número de cliente.");
                    textBox4.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox5.Text))
                {
                    MessageBox.Show("Por favor ingrese la cantidad de días.");
                    textBox5.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(comboBox1.Text))
                {
                    MessageBox.Show("Por favor seleccione un perfil.");
                    comboBox1.Focus();
                    return;
                }

                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    DateTime fechaseleccionada = dateTimePicker1.Value;
                    conn.Open();

                    string query;
                    if (empresaSeleccionadaId.HasValue)
                    {
                        // UPDATE si ya hay un ID
                        query = @"UPDATE Empresas 
                                  SET Nombre_Empresa = @Nombre_Empresa,
                                      Direccion = @Direccion,
                                      No_Cliente = @No_Cliente,
                                      Perfil = @Perfil,
                                      Cantidad_Dias = @Cantidad_Dias,
                                      Fecha_Inicio = @Fecha_Inicio,
                                      Nombre_Corto=@Nombre_Corto
                                  WHERE ID_Empresa = @ID_Empresa";
                    }
                    else
                    {
                        // ➕ INSERT si no hay ID
                        query = @"INSERT INTO Empresas
                                  (Nombre_Empresa, Direccion, No_Cliente, Perfil, Cantidad_Dias, Fecha_Inicio,Nombre_Corto) 
                                  VALUES (@Nombre_Empresa, @Direccion, @No_Cliente, @Perfil, @Cantidad_Dias, @Fecha_Inicio,@Nombre_Corto)";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre_Empresa", textBox3.Text);
                        cmd.Parameters.AddWithValue("@Direccion", textBox2.Text);
                        cmd.Parameters.AddWithValue("@No_Cliente", textBox4.Text);
                        cmd.Parameters.AddWithValue("@Perfil", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@Cantidad_Dias", textBox5.Text);
                        cmd.Parameters.AddWithValue("@Nombre_Corto", textBox6.Text);
                        cmd.Parameters.AddWithValue("@Fecha_Inicio", fechaseleccionada);

                        if (empresaSeleccionadaId.HasValue)
                            cmd.Parameters.AddWithValue("@ID_Empresa", empresaSeleccionadaId.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show(empresaSeleccionadaId.HasValue
                                ? "Empresa modificada exitosamente."
                                : "Empresa agregada exitosamente.");

                            
                            CargarEmpresas();
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo guardar la empresa.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la empresa: " + ex.Message);
            }
        }

        private void BtnBuscar_Load(object sender, EventArgs e)
        {
         
        }

        private void button3_Click(object sender, EventArgs e)
        {
         
            this.Close();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            string nombreEmpresa = null;

            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener el nombre de la empresa de la fila seleccionada
                nombreEmpresa = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            }
            else if (dataGridView1.CurrentCell != null)
            {
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                nombreEmpresa = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
            }
            //validar que se selecciono una empresa
            if (string.IsNullOrEmpty(nombreEmpresa))
            {
                MessageBox.Show("Por favor, seleccione una empresa para eliminar.");
                return;
            }
            DialogResult result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar la empresa '{nombreEmpresa}'?",
                "Confirmar Modificación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();

                    string deleteQuery = @"DELETE FROM Empresas 
                                    WHERE ID_Empresa = @ID_Empresa";


                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Empresa", nombreEmpresa);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {

                            MessageBox.Show("Empresa eliminada exitosamente.");
                            CargarEmpresas();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar la empresa.");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar la empresa: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filtro = textBox1.Text.Trim();
            CargarEmpresas(filtro);
        }
        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow fila = dataGridView1.SelectedRows[0];

                //  Guardar el ID de la empresa seleccionada
                empresaSeleccionadaId = Convert.ToInt32(fila.Cells[0].Value);


                textBox3.Text = fila.Cells[1].Value.ToString(); // Nombre
                textBox6.Text = fila.Cells[2].Value.ToString(); // Nombre_Corto
                textBox2.Text = fila.Cells[3].Value.ToString(); // Dirección
                textBox4.Text = fila.Cells[4].Value.ToString(); // No_Cliente
                comboBox1.Text = fila.Cells[5].Value.ToString(); // Perfil
                textBox5.Text = fila.Cells[7].Value.ToString(); // Cantidad_Dias

                object valorFecha = fila.Cells[6].Value;

                if (valorFecha != null && DateTime.TryParse(valorFecha.ToString(), out DateTime fecha))
                {
                    dateTimePicker1.Value = fecha;
                }
                else
                {
                    dateTimePicker1.Value = DateTime.Today;
                    MessageBox.Show("La fecha de inicio no es válida o está vacía.");
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una empresa para modificar.");
            }
        }

        // Método auxiliar para limpiar los campos y reiniciar el estado
        private void LimpiarCampos()
        {
            textBox3.Clear();
            textBox2.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Now;
            empresaSeleccionadaId = null; // reset para nuevo registro
        }
    }
    
}
