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
using WindowsFormsApp1;
using WindowsFormsApp1.methods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CaastCtrl
{
    public partial class Principal : Form
    {
        private int? empresaSeleccionadaId = null;
        private string contactoSeleccionadoNombre = null; // agregado para copiar contacto
        public Principal()
        {
            InitializeComponent();
            CargarContactos();
            this.Load += SolicitarEmpresa_Load;
            this.StartPosition = FormStartPosition.CenterScreen;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            // manejar clic derecho para seleccionar fila y mostrar menú contextual
            dataGridView1.CellMouseDown += DataGridView1_CellMouseDown;

            // crear menú contextual para copiar contacto
            var ctx = new ContextMenuStrip();
            var copyContacto = new ToolStripMenuItem("Copiar contacto");
            copyContacto.Click += CopyContacto_Click;
            ctx.Items.Add(copyContacto);
            dataGridView1.ContextMenuStrip = ctx;


            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // evita que sea el encabezado
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                // Guarda el ID de la empresa seleccionada
                empresaSeleccionadaId = Convert.ToInt32(fila.Cells["ID_Empresa"].Value);

                // Pasa los valores de la fila a los controles
                comboBox1.Text = fila.Cells["Nombre_Empresa"].Value?.ToString();

                textBox4.Text = fila.Cells["Telefono"].Value?.ToString();
                textBox3.Text = fila.Cells["Correo"].Value?.ToString();
                textBoxNombreContacto.Text = fila.Cells["Nombre_Contacto"].Value?.ToString();
                //textBox5.Text = fila.Cells["Cantidad_Dias"].Value?.ToString();

               
            }
        }

        private void DataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Selected = true;
                var fila = dataGridView1.Rows[e.RowIndex];
                contactoSeleccionadoNombre = fila.Cells["Nombre_Contacto"].Value?.ToString();
                // Establecer CurrentCell para operaciones basadas en la celda actual
                if (dataGridView1.Columns.Count > 2 && fila.Cells[2] != null)
                    dataGridView1.CurrentCell = fila.Cells[2];
            }
        }

        private void CopyContacto_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(contactoSeleccionadoNombre))
            {
                MessageBox.Show("No hay contacto seleccionado para copiar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Clipboard.SetText(contactoSeleccionadoNombre);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al copiar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarContactos(string filtro = "")
        {
            try
            {
                //Limpiar el DataGtidView y definir columnas
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();

                //Asignacion de columnas
                dataGridView1.Columns.Add("ID_Empresa", "ID");
                dataGridView1.Columns.Add("Nombre_Empresa", "Empresa");
                dataGridView1.Columns.Add("Nombre_Contacto", "Contacto");
                dataGridView1.Columns.Add("Telefono", "Telefono");
                dataGridView1.Columns.Add("Correo", "Email");

                dataGridView1.Columns[0].Visible = false; //Ocultar columna ID_Empresa

                DataTable dataTable = new DataTable();
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT e.ID_Empresa, e.Nombre_Empresa, c.Nombre_Contacto, c.Telefono, c.Correo " +
                                   "FROM Contacto_Empresa c " +
                                   "INNER JOIN Empresas e ON e.ID_Empresa = c.ID_Empresa";
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        query += " WHERE e.Nombre_Empresa LIKE @Empresa";
                    }
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(filtro))
                        {
                            cmd.Parameters.AddWithValue("@Empresa", "%" + filtro + "%");
                        }
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dataTable);
                    }
                }
                foreach (DataRow row in dataTable.Rows)
                {
                    dataGridView1.Rows.Add(row["ID_Empresa"], row["Nombre_Empresa"], row["Nombre_Contacto"], row["Telefono"], row["Correo"]);
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al obtener datos de la empresa: " + sqlEx.Message);
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CargarContactos(textBoxNombreContacto.Text.Trim());


        }

        private void SolicitarEmpresa_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT Nombre_Empresa FROM Empresas";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    comboBox1.Items.Clear();
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["Nombre_Empresa"].ToString());
                        
                    }
                    reader.Close();
                }
                CargarEmpresas();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error de SQL: " + sqlEx.Message);
            }
        }


        private void CargarEmpresas()
        {
            try
            {
                //Abre la conexion SQL usando 
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {

                    conn.Open();
                    string query = "SELECT Nombre_Empresa FROM Empresas";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    comboBox1.Items.Clear();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["Nombre_Empresa"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al cargar ejecutivos: " + sqlEx.Message);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string filtro = textBox2.Text.Trim();
            CargarContactos(filtro);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(comboBox1.Text))
                {
                    MessageBox.Show("Por favor ingrese el nombre de la empresa.");
                    comboBox1.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Por favor ingrese el telefono del contacto de la empresa.");
                    textBox4.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Por favor ingrese el correo del contacto de la empresa.");
                    textBox3.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBoxNombreContacto.Text))
                {
                    MessageBox.Show("Por favor ingrese el nombre del contacto.");
                    textBoxNombreContacto.Focus();
                    return;
                }


                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();

                    //Obtener el ID de la empresa seleccionada en el ComboBox
                    int idEmpresa; 
                    using (SqlCommand cmdGetId = new SqlCommand("SELECT ID_Empresa FROM Empresas WHERE Nombre_Empresa = @Nombre_Empresa", conn))
                    {
                        cmdGetId.Parameters.AddWithValue("@Nombre_Empresa", comboBox1.Text);
                        object result = cmdGetId.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("La empresa seleccionada no existe. Por favor seleccione una empresa válida.");
                            return;
                        }
                        idEmpresa = Convert.ToInt32(result);
                    }

                    string query;
                    if (empresaSeleccionadaId.HasValue)
                    {
                        // UPDATE si ya hay un ID
                        query = @"UPDATE Contacto_Empresa 
                                  SET 
                                  Nombre_Contacto = @Nombre_Contacto,
                                  Telefono = @Telefono,
                                  Correo = @Correo
                                  WHERE ID_Empresa = @ID_Empresa";
                    }
                    else
                    {
                        //INSERT si no hay ID
                        query = @"INSERT INTO Contacto_Empresa
                                  (ID_Empresa,Nombre_Contacto,Telefono, Correo) 
                                  VALUES (@ID_Empresa,@Nombre_Contacto, @Telefono, @Correo)";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Empresa", idEmpresa);//0
                        cmd.Parameters.AddWithValue("@Nombre_Empresa", comboBox1.Text.Trim());//1
                        cmd.Parameters.AddWithValue("@Nombre_Contacto", textBoxNombreContacto.Text.Trim());//2
                        cmd.Parameters.AddWithValue("@Telefono", textBox4.Text.Trim());//3
                        cmd.Parameters.AddWithValue("@Correo", textBox3.Text.Trim());//4

                       
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
                    // Método auxiliar para limpiar los campos y reiniciar el estado
        private void LimpiarCampos()
        {
            textBox3.Clear();
            textBoxNombreContacto.Clear();
            textBox4.Clear();
            comboBox1.Text = String.Empty;
            empresaSeleccionadaId = null; // reset para nuevo registro
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow fila = dataGridView1.SelectedRows[0];

                //  Guardar el ID de la empresa seleccionada
                empresaSeleccionadaId = Convert.ToInt32(fila.Cells[0].Value);


                textBoxNombreContacto.Text = fila.Cells[2].Value.ToString(); 

                textBox4.Text = fila.Cells[3].Value.ToString();
                textBox3.Text = fila.Cells[4].Value.ToString(); 
                comboBox1.Text = fila.Cells[1].Value.ToString();

            }
            else
            {
                MessageBox.Show("Por favor seleccione una empresa para modificar.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CargarContactos();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string idContacto = null;

            if (dataGridView1.SelectedRows.Count > 2)
            {
                // Obtener el nombre de la empresa de la fila seleccionada
                idContacto = dataGridView1.SelectedRows[2].Cells[2].Value.ToString();
            }
            else if (dataGridView1.CurrentCell != null)
            {
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                idContacto = dataGridView1.Rows[rowIndex].Cells[2].Value.ToString();
            }
            //validar que se selecciono una empresa
            if (string.IsNullOrEmpty(idContacto))
            {
                MessageBox.Show("Por favor, seleccione una empresa para eliminar.");
                return;
            }
            DialogResult result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar el contacto '{idContacto}'?",
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
                    string deleteQuery = @"DELETE FROM Contacto_Empresa WHERE Nombre_Contacto = @Nombre_Contacto;";
                    



                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre_Contacto", idContacto);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {

                            MessageBox.Show("Contacto eliminado exitosamente.");
                            CargarEmpresas();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el contacto.");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el contacto: " + ex.Message);
            }
        }
    }
    
    
}
