using CaastCtrl;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Mysqlx;
using Org.BouncyCastle.Asn1.Cmp;
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


        //variable que lee el id del folio
        private string _idFolio;
        private string connStr;
        private string folioServicio;

        public Form4(string idFolio)
        {
            InitializeComponent();
            _idFolio = idFolio;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += SolicitudServicio_Load;

            //hace que no se pueda editar el textbox del Hojas de servicio
           // textBox8.ReadOnly = true;

            textBox2.ReadOnly = true;



            CargarDatosFolio();


            cmbRazon_Social2.DropDownStyle = ComboBoxStyle.DropDown;
            cmbRazon_Social2.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbRazon_Social2.AutoCompleteSource = AutoCompleteSource.ListItems;


            dgvServicios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvServicios.MultiSelect = false; // Opcional, para permitir solo una fila seleccionada

            ObtenerEstadoFirma();
        }

        private void ObtenerEstadoFirma()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    textBox3.ReadOnly = true;

                    // Consulta solo el registro del folio que te interesa
                    string query = "SELECT Firma_Recibido FROM Solicitud_Folio WHERE ID_Folio = @Folio";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Asegúrate de que textBox1 tiene el ID_Folio que buscas
                        cmd.Parameters.AddWithValue("@Folio", textBox2.Text);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            //Asigna el valor del campo Firma_Recibido directamente al textBox3
                            textBox3.Text = reader["Firma_Recibido"] == DBNull.Value
                                ? "No firmado"
                                : reader["Firma_Recibido"].ToString();
                        }

                        reader.Close();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al cargar estado de firma: " + sqlEx.Message);
            }
        }



        private string ObtenerFirmaTecnicoActual()
        {
            using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
            {
                conn.Open();
                string query = @"
                        SELECT 
                            COALESCE(Firma_Tecnico, CONCAT(Nombre, ' ', Apellido)) AS Firma
                        FROM Usuarios_Caast
                        WHERE ID_Usuario = @idUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idUsuario", LoginService.IdUsuarioActual);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Firma desconocida";
                }
            }
        }

        //metodo para cargar a los usuarios
        private void CargarEjecutivos()
        {
            try
            {
                //Abre la conexion SQL usando 
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    //Consulta para validar la existencia de usuarios de la BD 
                    conn.Open();
                    string query = "SELECT Nombre_Usuario FROM Usuarios_Caast";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbEjecutivo.Items.Clear();

                    while (reader.Read())
                    {
                        cmbEjecutivo.Items.Add(reader["Nombre_Usuario"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error al cargar ejecutivos: " + sqlEx.Message);
            }
        }
        //metodo para generar la solicitud de servicio 
        private void SolicitudServicio_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT Nombre_Empresa FROM Empresas";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbRazon_Social2.Items.Clear();

                    while (reader.Read())
                    {
                        cmbRazon_Social2.Items.Add(reader["Nombre_Empresa"].ToString());
                    }

                    reader.Close();
                }
                CargarEjecutivos();
                
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error de SQL: " + sqlEx.Message);
            }

        }

        private void cmbProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRazon_Social2.SelectedItem != null)
            {
                string empresaSeleccionada = cmbRazon_Social2.SelectedItem.ToString();

                try
                {
                    using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                    {
                        conn.Open();

                        // Obtener número de cliente
                        string queryCliente = "SELECT No_Cliente FROM Empresas WHERE Nombre_Empresa = @empresa";
                        SqlCommand cmdCliente = new SqlCommand(queryCliente, conn);
                        cmdCliente.Parameters.AddWithValue("@empresa", empresaSeleccionada);
                        object result = cmdCliente.ExecuteScalar();
                        textBox7.Text = result != null ? result.ToString() : string.Empty;

                        // Obtener contactos de la empresa
                        string queryContactos = "SELECT c.Nombre_Contacto FROM Contacto_Empresa c " +
                            "INNER JOIN Empresas e on e.ID_Empresa = c.ID_Empresa " +
                            "WHERE e.Nombre_Empresa = @empresa";
                        SqlCommand cmdContactos = new SqlCommand(queryContactos, conn);
                        cmdContactos.Parameters.AddWithValue("@empresa", empresaSeleccionada);

                        SqlDataReader reader = cmdContactos.ExecuteReader();
                        cmbNombre_Contacto.Items.Clear();

                        while (reader.Read())
                        {
                            cmbNombre_Contacto.Items.Add(reader["Nombre_Contacto"].ToString());
                        }

                        reader.Close();
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Error al obtener datos de la empresa: " + sqlEx.Message);
                }
            }
        }




        private void CargarDatosFolio()
        {

            dgvServicios.DataError += (s, e) => { e.ThrowException = false; };


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
            connStr = $"Server={config["Server"]};Database={config["Database"]};User Id={config["User Id"]};Password={config["Password"]};";

            // --- Configurar dgvServicios ---
            dgvServicios.Columns.Clear();

            // Columna oculta ID_Hoja (clave primaria de Hojas_Servicio)
            DataGridViewTextBoxColumn idHojaCol = new DataGridViewTextBoxColumn();
            idHojaCol.Name = "ID_Hoja";
            idHojaCol.HeaderText = "ID Hoja";
            dgvServicios.Columns.Add(idHojaCol);


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

            // Columna Tipo de Servicio (ComboBox)
            DataGridViewComboBoxColumn tipoSistemaCol = new DataGridViewComboBoxColumn();
            tipoSistemaCol.Name = "Tipo_Sistema";
            tipoSistemaCol.HeaderText = "Sistema";          
            dgvServicios.Columns.Add(tipoSistemaCol);

            // Columna Descripcion (texto)
            DataGridViewTextBoxColumn descripcionCol = new DataGridViewTextBoxColumn();
            descripcionCol.Name = "Descripcion";
            descripcionCol.HeaderText = "Descripción";
            descripcionCol.Width = 337;
            dgvServicios.Columns.Add(descripcionCol);




            // --- Extraer opciones para los ComboBox desde la base de datos ---
            var equipos = new HashSet<string>();
            var servicios = new HashSet<string>();
            var sistema = new HashSet<string>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = @"
                    SELECT hs.ID_Hoja, hs.Tipo_Servicio, hs.Tipo_Equipo, hs.Tipo_Sistema, hs.Descripcion
                    FROM Hojas_Servicio as hs
                    INNER JOIN Control_Interno as ci ON ci.ID_Folio = hs.ID_Folio
                    WHERE ci.ID_Folio = @idFolio
                      AND hs.Censo = 'No'
                      AND hs.Tipo_Servicio IS NOT NULL AND hs.Tipo_Servicio <> ''
                      AND hs.Tipo_Equipo IS NOT NULL AND hs.Tipo_Equipo <> ''
                      AND hs.Tipo_Sistema IS NOT NULL AND hs.Tipo_Sistema <> ''
                      AND hs.Descripcion IS NOT NULL AND hs.Descripcion <> ''";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? idHoja = reader["ID_Hoja"]!= DBNull.Value ? (int?)Convert.ToInt32(reader["ID_Hoja"]): null;
                            string tipoServicio = reader["Tipo_Servicio"]?.ToString() ?? "";
                            string tipoEquipo = reader["Tipo_Equipo"]?.ToString() ?? "";
                            string tipoSistema = reader["Tipo_Sistema"]?.ToString() ?? "";



                            if (!tipoServicioCol.Items.Contains(tipoServicio) && tipoServicio != "")
                                tipoServicioCol.Items.Add(tipoServicio);

                            if (!tipoEquipoCol.Items.Contains(tipoEquipo) && tipoEquipo != "")
                                tipoEquipoCol.Items.Add(tipoEquipo);

                            if (!tipoSistemaCol.Items.Contains(tipoSistema) && tipoSistema != "")
                                tipoSistemaCol.Items.Add(tipoSistema);

                            // Ahora agrega la fila al grid
                            dgvServicios.Rows.Add(idHoja,tipoServicio, tipoEquipo, tipoSistema, reader["Descripcion"]?.ToString() ?? "");

                        }
                    }
                }
            }

            // Puedes agregar opciones adicionales manualmente si lo deseas
            tipoEquipoCol.Items.AddRange(equipos.ToArray());
            tipoEquipoCol.Items.Add("Escritorio");
            tipoEquipoCol.Items.Add("Portatil");
            tipoEquipoCol.Items.Add("Servidor");
            

            tipoServicioCol.Items.AddRange(servicios.ToArray());
            tipoServicioCol.Items.AddRange("Activacion");
            tipoServicioCol.Items.AddRange("Actualizacion");
            tipoServicioCol.Items.AddRange("Configuracion");
            tipoServicioCol.Items.AddRange("Respaldo");
            tipoServicioCol.Items.AddRange("Restauracion");
            tipoServicioCol.Items.AddRange("Revision");
            tipoServicioCol.Items.AddRange("Servicio");
            tipoServicioCol.Items.AddRange("Instalación");


            tipoSistemaCol.Items.AddRange(sistema.ToArray());
            tipoSistemaCol.Items.Add("N/A");
            tipoSistemaCol.Items.Add("MSQL");
            tipoSistemaCol.Items.Add("SEER Trafico");
            tipoSistemaCol.Items.Add("Office");
            tipoSistemaCol.Items.Add("Antivirus");
            tipoSistemaCol.Items.Add("Otros");
            tipoSistemaCol.Items.Add("MSQL BD");

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
                string query = "SELECT Folio_Hoja FROM Hojas_Servicio WHERE ID_Folio = @idFolio AND Censo = 'No'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            folioServicio = textBox8.Text = reader["Folio_Hoja"].ToString();
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
                            textBox2.Text = reader["ID_Folio"].ToString();
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


        //boton de guardar 
        private void button3_Click(object sender, EventArgs e)
        {

            string idFolioNuevo = textBox2.Text.Trim();  // ID_Folio (Folio de solicitud)
            string folioHoja = textBox8.Text.Trim();     // Folio_Hoja (Hoja de servicio)

            if (string.IsNullOrWhiteSpace(idFolioNuevo))
            {
                MessageBox.Show("Ingrese un folio válido en TextBox2 (ID_Folio).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // --- 1️ Actualizar Control_Interno ---
                            string updateCI = @"
                            UPDATE Control_Interno SET
                                ID_Folio = @folioNuevo,
                                No_Cotizacion = @NoCotizacion,
                                No_Pedido = @NoPedido,
                                Razon_Social = @RazonSocial,
                                No_Cliente = @NoCliente,
                                Nombre_Contacto = @NombreContacto,
                                Fecha_Solicitud = @FechaSolicitud,
                                Ejecutivo_Asignado = @Ejecutivo
                            WHERE ID_Folio = @folioAntiguo";

                            using (SqlCommand cmd = new SqlCommand(updateCI, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@folioNuevo", idFolioNuevo);
                                cmd.Parameters.AddWithValue("@folioAntiguo", _idFolio);
                                cmd.Parameters.AddWithValue("@NoCotizacion", txtCotizacion2.Text);
                                cmd.Parameters.AddWithValue("@NoPedido", txtPedido.Text);
                                cmd.Parameters.AddWithValue("@RazonSocial", cmbRazon_Social2.Text);
                                cmd.Parameters.AddWithValue("@NoCliente", textBox7.Text);
                                cmd.Parameters.AddWithValue("@NombreContacto", cmbNombre_Contacto.Text);
                                cmd.Parameters.AddWithValue("@FechaSolicitud", dateTimePicker1.Value);
                                cmd.Parameters.AddWithValue("@Ejecutivo", cmbEjecutivo.Text);
                                cmd.ExecuteNonQuery();
                            }

                            // --- 2️ Actualizar Solicitud_Folio ---
                            string updateSF = @"UPDATE Solicitud_Folio SET ID_Folio = @folioNuevo WHERE ID_Folio = @folioAntiguo";
                            using (SqlCommand cmd = new SqlCommand(updateSF, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@folioNuevo", idFolioNuevo);
                                cmd.Parameters.AddWithValue("@folioAntiguo", _idFolio);
                                cmd.ExecuteNonQuery();
                            }

                            

                            // --- 4️ Insertar censos (Censo = 'SI') ---
                            foreach (DataGridViewRow row in dgvHoja.Rows)
                            {
                                if (row.IsNewRow) continue;
                                string hojaCenso = row.Cells["HojaCenso"].Value?.ToString();
                                if (!string.IsNullOrWhiteSpace(hojaCenso))
                                {
                                    string insertCenso = @"
                                    IF NOT EXISTS (SELECT 1 FROM Hojas_Servicio WHERE Folio_Hoja = @folioHoja AND ID_Folio = @folioNuevo)
                                    BEGIN
                                        INSERT INTO Hojas_Servicio (ID_Folio, Folio_Hoja, Censo)
                                        VALUES (@folioNuevo, @folioHoja, 'Si')
                                    END";

                                    using (SqlCommand cmd = new SqlCommand(insertCenso, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@folioNuevo", idFolioNuevo);
                                        cmd.Parameters.AddWithValue("@folioHoja", hojaCenso);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Obtener el nuevo número de hoja de servicio ingresado
                            string nuevoFolioHoja = textBox8.Text.Trim();

                            foreach (DataGridViewRow row in dgvServicios.Rows)
                            {
                                if (row.IsNewRow) continue;
                                string nuevoTipoServicio = row.Cells["Tipo_Servicio"].Value?.ToString();
                                string nuevoTipoEquipo = row.Cells["Tipo_Equipo"].Value?.ToString();
                                string nuevoTipoSistema = row.Cells["Tipo_Sistema"].Value?.ToString();
                                string nuevaDescripcion = row.Cells["Descripcion"].Value?.ToString();

                                // Solo continuar si al menos uno de los campos tiene valor
                                if (string.IsNullOrWhiteSpace(nuevoTipoServicio) &&
                                    string.IsNullOrWhiteSpace(nuevoTipoEquipo) &&
                                    string.IsNullOrWhiteSpace(nuevoTipoSistema) &&
                                    string.IsNullOrWhiteSpace(nuevaDescripcion))
                                {
                                    continue; // No agregar ni actualizar si todos son nulos o vacíos
                                }

                                string updateQuery = @"
                                    UPDATE Hojas_Servicio
                                    SET Folio_Hoja = @FolioHoja,
                                        Tipo_Servicio = @TipoServicio,
                                        Tipo_Equipo = @TipoEquipo,
                                        Tipo_Sistema = @TipoSistema,
                                        Descripcion = @Descripcion
                                    WHERE ID_Folio = @IDFolio
                                      AND Censo = 'No'";

                                using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@FolioHoja", nuevoFolioHoja);
                                    cmd.Parameters.AddWithValue("@TipoServicio", nuevoTipoServicio);
                                    cmd.Parameters.AddWithValue("@TipoEquipo", nuevoTipoEquipo);
                                    cmd.Parameters.AddWithValue("@TipoSistema", nuevoTipoSistema);
                                    cmd.Parameters.AddWithValue("@Descripcion", nuevaDescripcion);
                                    cmd.Parameters.AddWithValue("@IDFolio", _idFolio);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // --- 5️ Actualizar o insertar servicios ---
                            foreach (DataGridViewRow row in dgvServicios.Rows)
                            {
                                if (row.IsNewRow) continue;
                                string tipoServicio = row.Cells["Tipo_Servicio"].Value?.ToString();
                                string tipoEquipo = row.Cells["Tipo_Equipo"].Value?.ToString();
                                string tipoSistema = row.Cells["Tipo_Sistema"].Value?.ToString();
                                string descripcion = row.Cells["Descripcion"].Value?.ToString();

                                // Solo continuar si al menos uno de los campos tiene valor
                                if (string.IsNullOrWhiteSpace(tipoServicio) &&
                                    string.IsNullOrWhiteSpace(tipoEquipo) &&
                                    string.IsNullOrWhiteSpace(tipoSistema) &&
                                    string.IsNullOrWhiteSpace(descripcion))
                                {
                                    continue; // No agregar ni actualizar si todos son nulos o vacíos
                                }

                                string hojaServicio = textBox8.Text.Trim(); // Folio_Hoja del TextBox

                                object idHojaCell = row.Cells["ID_Hoja"].Value;
                                int idHojaGrid = 0;
                                bool tieneIdHoja = idHojaCell != null && int.TryParse(idHojaCell.ToString(), out idHojaGrid) && idHojaGrid > 0;

                                if (tieneIdHoja)
                                {
                                    // Actualizar si ya existe (por ID_Folio y ID_Hoja)
                                    string updateServicio = @"
                                    UPDATE Hojas_Servicio SET
                                        Tipo_Servicio = @TipoServicio,
                                        Tipo_Equipo = @TipoEquipo,
                                        Tipo_Sistema = @TipoSistema,
                                        Descripcion = @Descripcion
                                    WHERE ID_Folio = @IDFolio AND ID_Hoja = @IDHoja AND Censo = 'No'";

                                    using (SqlCommand cmdUpdate = new SqlCommand(updateServicio, conn, transaction))
                                    {
                                        cmdUpdate.Parameters.AddWithValue("@TipoServicio", (object)tipoServicio ?? DBNull.Value);
                                        cmdUpdate.Parameters.AddWithValue("@TipoEquipo", (object)tipoEquipo ?? DBNull.Value);
                                        cmdUpdate.Parameters.AddWithValue("@TipoSistema", (object)tipoSistema ?? DBNull.Value);
                                        cmdUpdate.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                                        cmdUpdate.Parameters.AddWithValue("@IDFolio", idFolioNuevo);
                                        cmdUpdate.Parameters.AddWithValue("@IDHoja", idHojaGrid);
                                        cmdUpdate.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Insertar si no existe (no hay ID_Hoja en la fila)
                                    string insertServicio = @"
                                    INSERT INTO Hojas_Servicio
                                        (ID_Folio, Folio_Hoja, Tipo_Servicio, Tipo_Equipo, Tipo_Sistema, Descripcion, Censo)
                                    VALUES
                                        (@folioNuevo, @FolioHoja, @TipoServicio, @TipoEquipo, @TipoSistema, @Descripcion, 'NO');
                                    SELECT SCOPE_IDENTITY();";

                                    using (SqlCommand cmdInsert = new SqlCommand(insertServicio, conn, transaction))
                                    {
                                        cmdInsert.Parameters.AddWithValue("@folioNuevo", idFolioNuevo);
                                        cmdInsert.Parameters.AddWithValue("@FolioHoja", hojaServicio);
                                        cmdInsert.Parameters.AddWithValue("@TipoServicio", (object)tipoServicio ?? DBNull.Value);
                                        cmdInsert.Parameters.AddWithValue("@TipoEquipo", (object)tipoEquipo ?? DBNull.Value);
                                        cmdInsert.Parameters.AddWithValue("@TipoSistema", (object)tipoSistema ?? DBNull.Value);
                                        cmdInsert.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);

                                        var nuevoID = cmdInsert.ExecuteScalar();
                                        if (nuevoID != null)
                                            row.Cells["ID_Hoja"].Value = Convert.ToInt32(nuevoID);
                                    }
                                }
                            }




                            // --- 6️ Confirmar transacción ---
                            transaction.Commit();
                            _idFolio = idFolioNuevo;

                            MessageBox.Show("Folio y datos actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatosFolio();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error al actualizar folio: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string folio = textBox1.Text.Trim();
            if (!string.IsNullOrWhiteSpace(folio))
            {
                // Verificar duplicados antes de agregar
                if (dgvHoja.Rows.Cast<DataGridViewRow>().Any(r => r.Cells["HojaCenso"].Value?.ToString() == folio))
                {
                    MessageBox.Show("Este folio ya existe.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Crear una nueva fila
                int index = dgvHoja.Rows.Add();
                dgvHoja.Rows[index].Cells["HojaCenso"].Value = folio;

                // Limpiar el TextBox
                textBox1.Clear();
                textBox1.Focus();
            }
            else
            {
                MessageBox.Show("Ingrese un número de hoja de servicio.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validación de campos obligatorios
                if (string.IsNullOrWhiteSpace(txtCotizacion2.Text) ||
                    string.IsNullOrWhiteSpace(txtPedido.Text) ||
                    //string.IsNullOrWhiteSpace(txtHojaServicio.Text) ||
                    string.IsNullOrWhiteSpace(textBox7.Text) ||
                    string.IsNullOrWhiteSpace(cmbNombre_Contacto.Text) ||
                    string.IsNullOrWhiteSpace(cmbRazon_Social2.Text) ||
                    string.IsNullOrWhiteSpace(cmbEjecutivo.Text))

                {
                    MessageBox.Show(" Debes llenar todos los campos antes de generar el PDF.",
                                    "Campos incompletos",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return; //No sigue con la creación del PDF
                }


            }
            catch (Exception)
            {

            }

            //Genera el archivo PDF
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Archivo PDF|*.pdf";
            saveFile.Title = "Guardar Solicitud de Servicio";
            saveFile.FileName = "SolicitudServicio.pdf";

            //Formato del archivo pdf
            if (saveFile.ShowDialog() == DialogResult.OK)
            {

                Document doc = new Document(PageSize.A4.Rotate(), 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, new FileStream(saveFile.FileName, FileMode.Create));
                doc.Open();


                string basePath = Application.StartupPath;

                // Combina la ruta de la carpeta "imagenes" con el archivo
                string logoPath = Path.Combine(basePath, "imagenes", "logo caast.png");

                // Carga la imagen
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(150, 150); // tamaño del logo
                logo.Alignment = Element.ALIGN_LEFT;
                logo.SetAbsolutePosition(doc.PageSize.Width - doc.RightMargin - 790,
                                         doc.PageSize.Height - doc.TopMargin - 98);

                // Agregar al documento
                doc.Add(logo);

   


                // Título 3
                Paragraph titulo3 = new Paragraph("Control interno CAAST 2025",
                                   new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD));
                titulo3.Alignment = Element.ALIGN_CENTER;

                doc.Add(titulo3);
                doc.Add(new Paragraph("\n"));

                // Título 1
                Paragraph titulo = new Paragraph("Solicitud de Servicio: " + _idFolio,
                                   new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;

                doc.Add(titulo);
                doc.Add(new Paragraph("\n"));

                // se crea la tabla Solicitud de servicio
                PdfPTable tabla = new PdfPTable(4);
                //ancho de la segunda tabla
                tabla.WidthPercentage = 100;
                //tabla.SetWidths(new float[] { 50f, 50f,50f,50f });//proporcion de las columnas

                iTextSharp.text.Font fuenteNegrita =
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fuenteNormal =
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);



                // Encabezados censo
                PdfPCell cellCenso = new PdfPCell(new Phrase("Hoja de Censo", fuenteNegrita));
                cellCenso.HorizontalAlignment = Element.ALIGN_CENTER;
                cellCenso.BackgroundColor = new BaseColor(220, 220, 220);
                cellCenso.FixedHeight = 29f;
                tabla.AddCell(cellCenso);
                

                // Columna 2: concatenar todas las hojas de censo
                string hojasCenso = string.Join(", ", dgvHoja.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => !r.IsNewRow)
                    .Select(r => r.Cells["HojaCenso"].Value?.ToString() ?? "")
                    .ToList());

                tabla.AddCell(new PdfPCell(new Phrase(hojasCenso, fuenteNormal)));

                // Encabezados servicio
                PdfPCell cellServicio = new PdfPCell(new Phrase("Hoja de Servicio", fuenteNegrita));
                cellServicio.HorizontalAlignment = Element.ALIGN_CENTER;
                cellServicio.BackgroundColor = new BaseColor(220, 220, 220);
                cellServicio.FixedHeight = 29f;
                tabla.AddCell(cellServicio);
                

                // --- Agregar fila ---
                // Columna 1: folio del servicio
                tabla.AddCell(new PdfPCell(new Phrase(textBox8.Text.Trim(), fuenteNormal)));






                PdfPCell celda1 = new PdfPCell(new Phrase("Razon social"));
                celda1.FixedHeight = 30f;
                celda1.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda2 = new PdfPCell(new Phrase(cmbRazon_Social2.Text));
                celda2.FixedHeight = 30f;


                PdfPCell celda3 = new PdfPCell(new Phrase("# Cliente"));
                celda3.FixedHeight = 30f;
                celda3.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda4 = new PdfPCell(new Phrase(textBox7.Text));
                celda4.FixedHeight = 30f;


                PdfPCell celda5 = new PdfPCell(new Phrase("Nombre del contacto"));
                celda5.FixedHeight = 30f;
                celda5.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda6 = new PdfPCell(new Phrase(cmbNombre_Contacto.Text));
                celda6.FixedHeight = 30f;


                PdfPCell celda7 = new PdfPCell(new Phrase("Fecha de solicitud"));
                celda7.FixedHeight = 30f;
                celda7.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda8 = new PdfPCell(new Phrase(dateTimePicker1.Value.ToShortDateString()));
                celda8.FixedHeight = 30f;


                PdfPCell celda9 = new PdfPCell(new Phrase("Proveedor asignado "));
                celda9.FixedHeight = 30f;
                celda9.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda10 = new PdfPCell(new Phrase("CAAST "));
                celda10.FixedHeight = 30f;


                PdfPCell celda11 = new PdfPCell(new Phrase("Ejecutivo asignado "));
                celda11.FixedHeight = 30f;
                celda11.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda12 = new PdfPCell(new Phrase(cmbEjecutivo.Text));
                celda12.FixedHeight = 30f;


                PdfPCell celda13 = new PdfPCell(new Phrase("#Cotizacion "));
                celda13.FixedHeight = 30f;
                celda13.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda14 = new PdfPCell(new Phrase(txtCotizacion2.Text));
                celda14.FixedHeight = 30f;


                PdfPCell celda15 = new PdfPCell(new Phrase("#Pedido "));
                celda15.FixedHeight = 30f;
                celda15.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda16 = new PdfPCell(new Phrase(txtPedido.Text));
                celda16.FixedHeight = 30f;



                tabla.AddCell(celda13);
                tabla.AddCell(celda14);
                tabla.AddCell(celda15);
                tabla.AddCell(celda16);
                tabla.AddCell(celda1);
                tabla.AddCell(celda2);
                tabla.AddCell(celda3);
                tabla.AddCell(celda4);
                tabla.AddCell(celda5);
                tabla.AddCell(celda6);
                tabla.AddCell(celda7);
                tabla.AddCell(celda8);
                tabla.AddCell(celda9);
                tabla.AddCell(celda10);
                tabla.AddCell(celda11);
                tabla.AddCell(celda12);




                doc.Add(tabla);


                // Título de servicios
                Paragraph tituloServicios = new Paragraph("Servicios solicitados",
                    new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD));
                tituloServicios.Alignment = Element.ALIGN_CENTER;
                doc.Add(tituloServicios);
                doc.Add(new Paragraph("\n"));

                // Tabla de servicios
                PdfPTable tablaServicios = new PdfPTable(4);
                tablaServicios.WidthPercentage = 100;
                tablaServicios.AddCell("Tipo de Servicio");
                tablaServicios.AddCell("Equipo");
                tablaServicios.AddCell("Tipo Sistema");
                tablaServicios.AddCell("Descripción");
                // Establecer anchos relativos (ajusta los valores según convenga)
                tablaServicios.SetWidths(new float[] { 2f,2f,2f,2f });
                // Esto hace que la columna "Descripción" sea más ancha que las otras

                // Encabezados con negrita
                iTextSharp.text.Font fuenteEncabezado =
                    new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);


                float altoFilaServicios = 35f;


                // Recorrer filas del DataGridView
                foreach (DataGridViewRow row in dgvServicios.Rows)
                {
                    if (row.IsNewRow) continue;


                    tablaServicios.AddCell(new PdfPCell(new Phrase(row.Cells["Tipo_Servicio"].Value?.ToString())) { FixedHeight = altoFilaServicios });
                    tablaServicios.AddCell(new PdfPCell(new Phrase(row.Cells["Tipo_Equipo"].Value?.ToString())) { FixedHeight = altoFilaServicios });
                    tablaServicios.AddCell(new PdfPCell(new Phrase(row.Cells["Tipo_Sistema"].Value?.ToString())) { FixedHeight = altoFilaServicios });
                    tablaServicios.AddCell(new PdfPCell(new Phrase(row.Cells["Descripcion"].Value?.ToString())) { FixedHeight = altoFilaServicios });



                }



                doc.Add(tablaServicios);
                doc.Add(new Paragraph("\n"));


                string firma = ObtenerFirmaTecnicoActual();
                Paragraph firmaParrafo = new Paragraph($"Firmado por: {firma}\nFecha: {DateTime.Now:MM/dd/yyyy}\nHora:{DateTime.Now: hh:mm:tt}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD));
                firmaParrafo.Alignment = Element.ALIGN_LEFT;
                doc.Add(firmaParrafo);


                // Pie
                doc.Add(new Paragraph("Generado automáticamente por el sistema de CAAST",
                            new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.ITALIC)));




                doc.Close();
                //mensaje de archivo generado
                MessageBox.Show("PDF generado correctamente ", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void button5_Click_1(object sender, EventArgs e)
        {
            // Usar CurrentRow en vez de SelectedRows
            var filaSeleccionada = dgvHoja.CurrentRow;
            if (filaSeleccionada == null || filaSeleccionada.IsNewRow)
            {
                MessageBox.Show("Seleccione la hoja de censo que desea eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var folioHoja = filaSeleccionada.Cells["HojaCenso"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(folioHoja))
            {
                MessageBox.Show("No se pudo obtener el folio de la hoja seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show($"¿Está seguro que desea eliminar el censo con folio '{folioHoja}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            // Eliminar de la base de datos
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
                    string deleteQuery = "DELETE FROM Hojas_Servicio WHERE ID_Folio = @idFolio AND Folio_Hoja = @folioHoja AND Censo = 'Si'";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                        cmd.Parameters.AddWithValue("@folioHoja", folioHoja);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            dgvHoja.Rows.Remove(filaSeleccionada);
                            MessageBox.Show("Censo eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el censo en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el censo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Agrega una nueva fila vacía al DataGridView de servicios
            dgvServicios.Rows.Add(null, "", "", "", "");
            // Opcional: selecciona la nueva fila para edición inmediata
            int lastRow = dgvServicios.Rows.Count - 1;
            dgvServicios.CurrentCell = dgvServicios.Rows[lastRow].Cells["Tipo_Servicio"];
            dgvServicios.BeginEdit(true);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Usar CurrentRow en vez de SelectedRows
            var filaSeleccionada = dgvServicios.CurrentRow;
            if (filaSeleccionada == null || filaSeleccionada.IsNewRow)
            {
                MessageBox.Show("Seleccione el servicio que desea eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var idHojaObj = filaSeleccionada.Cells["ID_Hoja"].Value;
            if (idHojaObj == null || idHojaObj == DBNull.Value || string.IsNullOrWhiteSpace(idHojaObj.ToString()))
            {
                // Si no hay ID_Hoja, solo elimina la fila del grid (no está en BD)
                dgvServicios.Rows.Remove(filaSeleccionada);
                MessageBox.Show("Fila eliminada localmente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idHoja;
            if (!int.TryParse(idHojaObj.ToString(), out idHoja))
            {
                MessageBox.Show("ID_Hoja inválido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show($"¿Está seguro que desea eliminar el servicio con ID_Hoja '{idHoja}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            // Eliminar de la base de datos
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
                    string deleteQuery = "DELETE FROM Hojas_Servicio WHERE ID_Hoja = @idHoja";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@idHoja", idHoja);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // eliminar la fila del DataGridView de servicios
                            dgvServicios.Rows.Remove(filaSeleccionada);
                            MessageBox.Show("Servicio eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Error al eliminar servicio en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el servicio: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            BtnBuscar empresa = new BtnBuscar();
            empresa.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Principal principal = new Principal();
            principal.Show();
        }
    }
}
