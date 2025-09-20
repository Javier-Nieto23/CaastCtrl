using iTextSharp.text;
using iTextSharp.text.pdf;
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


        public Form4(string idFolio)
        {
            InitializeComponent();
            _idFolio = idFolio;
            this.StartPosition = FormStartPosition.CenterScreen;
            CargarDatosFolio();
            groupBox1.Text = $"Datos de la solicitud - Folio: {_idFolio}";

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
                                    cmd.Parameters.AddWithValue("@TipoServicio", string.IsNullOrEmpty(tipoServicio) ? (object)DBNull.Value : tipoServicio);
                                    cmd.Parameters.AddWithValue("@TipoEquipo", string.IsNullOrEmpty(tipoEquipo) ? (object)DBNull.Value : tipoEquipo);

                                    cmd.Parameters.AddWithValue("@idFolio", _idFolio);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            if (dgvServicios.Rows.Count > 0 && !dgvServicios.Rows[0].IsNewRow)
                            {
                                var Descripcion = dgvServicios.Rows[0].Cells["Descripcion"].Value?.ToString();


                                string updateDescripcion = @"
                                    UPDATE Hojas_Servicio SET
                                        Descripcion = @Descripcion   
                                    WHERE ID_Folio = @idFolio";
                                using (SqlCommand cmd = new SqlCommand(updateDescripcion, conn, transaction))
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
                        // Insertar nuevas hojas de censo del grid dgvHoja
                        foreach (DataGridViewRow row in dgvHoja.Rows)
                        {
                            if (row.IsNewRow) continue;
                            var folioHoja = row.Cells["HojaCenso"].Value?.ToString();
                            if (string.IsNullOrWhiteSpace(folioHoja)) continue;

                            // Verificar si ya existe la hoja en la base de datos
                            string checkQuery = "SELECT COUNT(*) FROM Hojas_Servicio WHERE ID_Folio = @idFolio AND Folio_Hoja = @folioHoja AND Censo = 'Si'";
                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@idFolio", _idFolio);
                                checkCmd.Parameters.AddWithValue("@folioHoja", folioHoja);
                                int count = (int)checkCmd.ExecuteScalar();
                                if (count == 0)
                                {
                                    // Insertar la hoja de censo
                                    string insertQuery = @"
                                    INSERT INTO Hojas_Servicio (ID_Folio, Folio_Hoja, Censo)
                                    VALUES (@idFolio, @folioHoja, 'Si')";
                                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction))
                                    {
                                        insertCmd.Parameters.AddWithValue("@idFolio", _idFolio);
                                        insertCmd.Parameters.AddWithValue("@folioHoja", folioHoja);
                                        insertCmd.ExecuteNonQuery();
                                    }
                                }
                            }
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


                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("C:/imagenes/logo caast.png");
                logo.ScaleAbsolute(150, 150); // tamaño del logo
                logo.Alignment = Element.ALIGN_RIGHT;
                logo.SetAbsolutePosition(doc.PageSize.Width - doc.RightMargin - 170,
                         doc.PageSize.Height - doc.TopMargin - 100);
                doc.Add(logo);

                // Folio en la parte superior izquierda
                Paragraph folioEncabezado = new Paragraph("Folio: " + _idFolio,
                    new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD));
                folioEncabezado.Alignment = Element.ALIGN_LEFT; // 👈 alineado a la izquierda
                doc.Add(folioEncabezado);
                doc.Add(new Paragraph("\n"));


                // Título 3
                Paragraph titulo3 = new Paragraph("Control interno CAAST 2025",
                                   new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD));
                titulo3.Alignment = Element.ALIGN_CENTER;

                doc.Add(titulo3);
                doc.Add(new Paragraph("\n"));

                // Título 1
                Paragraph titulo = new Paragraph("Solicitud de Servicio",
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
                tabla.AddCell(cellCenso);
                cellCenso.FixedHeight = 30f;

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
                tabla.AddCell(cellServicio);
                cellServicio.FixedHeight = 30f;

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
                celda9.FixedHeight = 30f;


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
                PdfPTable tablaServicios = new PdfPTable(3);
                tablaServicios.WidthPercentage = 100;
                tablaServicios.AddCell("Tipo de Servicio");
                tablaServicios.AddCell("Equipo");
                tablaServicios.AddCell("Descripción");

                // Recorrer filas del DataGridView
                foreach (DataGridViewRow row in dgvServicios.Rows)
                {
                    if (row.IsNewRow) continue;

                    tablaServicios.AddCell(row.Cells["Tipo_Servicio"].Value?.ToString());
                    tablaServicios.AddCell(row.Cells["Tipo_Equipo"].Value?.ToString());
                    tablaServicios.AddCell(row.Cells["Descripcion"].Value?.ToString());
                }




                doc.Add(tablaServicios);
                doc.Add(new Paragraph("\n"));



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

            var folioHoja = filaSeleccionada.Cells[0].Value?.ToString();

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
    }
}

   
    


