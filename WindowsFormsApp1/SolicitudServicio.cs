using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using WindowsFormsApp1.methods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WindowsFormsApp1.conexion;

namespace WindowsFormsApp1
{
    public partial class SolicitudServicio : Form
    {
        //variable para generar el folio de la solicitud
        private string folioSolicitud;



        public SolicitudServicio()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += SolicitudServicio_Load;
            ConfigurarTablaServicios();
            int folioPreview = ObtenerSiguienteFolio();

            // Asignar folio al TextBox en lugar del GroupBox
            textBox1.Text = folioPreview.ToString();



        }

        //metodo para validar que el folio no exista en la base de datos
        private bool FolioExiste(int folio)
        {
            bool existe = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Control_Interno WHERE ID_Folio = @folio";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@folio", folio);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        existe = count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar folio: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                existe = true; // Para prevenir inserciones erróneas si hay fallo
            }
            return existe;
        }

        //asigna el ultimo folio +1 de la base de datos para mostrarlo en el  groupbox
        private int ObtenerSiguienteFolio()
        {
            int siguienteFolio = 5000;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(ID_Folio), 4999) + 1 FROM Control_Interno";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        siguienteFolio = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                // Si hay error, se mantiene el valor por defecto
            }
            return siguienteFolio;
        }

        private void Cargar_Solicitud_Folio()
        {
            try
            {
                string query = "Select ID_Folio_Con,Nombre_Empresa,Fecha_Solicitud,Descripcion,Hoja,Status_Folio from Solicitud_Folio";
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error de SQL: " + sqlEx.Message);
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

                    cmbProveedor.Items.Clear();

                    while (reader.Read())
                    {
                        cmbProveedor.Items.Add(reader["Nombre_Empresa"].ToString());
                    }

                    reader.Close();
                }
                CargarEjecutivos();
                Cargar_Solicitud_Folio();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Error de SQL: " + sqlEx.Message);
            }

        }


        private void ConfigurarTablaServicios()
        {


            // Limpiar columnas
            dgvServicios.Columns.Clear();

            // Columna Tipo de Servicio (ComboBox)
            DataGridViewComboBoxColumn tipoServicioCol = new DataGridViewComboBoxColumn();
            tipoServicioCol.Name = "Tipo_Servicio";
            tipoServicioCol.HeaderText = "Tipo de Servicio";
            tipoServicioCol.Items.AddRange("Activacion", "Actualizacion", "Configuracion", "Respaldo", "Restauracion", "Revision", "Servicio", "Instalación");
            dgvServicios.Columns.Add(tipoServicioCol);

            // Columna Equipo (ComboBox)
            DataGridViewComboBoxColumn equipoCol = new DataGridViewComboBoxColumn();
            equipoCol.Name = "Equipo";
            equipoCol.HeaderText = "Equipo";
            equipoCol.Items.AddRange("Escritorio", "Portatil");
            dgvServicios.Columns.Add(equipoCol);

            // Columna Tipo de Servicio (ComboBox)
            DataGridViewComboBoxColumn tipoSistemaCol = new DataGridViewComboBoxColumn();
            tipoSistemaCol.Name = "Tipo_Sistema";
            tipoSistemaCol.HeaderText = "Sistema";
            tipoSistemaCol.Items.AddRange("N/A", "MSQL", "SEER Trafico", "Office", "Antivirus", "Otros", "MSQL BD");
            dgvServicios.Columns.Add(tipoSistemaCol);

            // Columna Descripción (Texto normal)
            DataGridViewTextBoxColumn descripcionCol = new DataGridViewTextBoxColumn();
            descripcionCol.Name = "Descripcion";
            descripcionCol.HeaderText = "Descripción";
            descripcionCol.Width = 300;

            dgvServicios.Columns.Add(descripcionCol);
        }


        private void cmbProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProveedor.SelectedItem != null)
            {
                string empresaSeleccionada = cmbProveedor.SelectedItem.ToString();

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
                        cmbCliente.Text = result != null ? result.ToString() : string.Empty;

                        // Obtener contactos de la empresa
                        string queryContactos = "SELECT c.Nombre_Contacto FROM Contacto_Empresa c " +
                            "INNER JOIN Empresas e on e.ID_Empresa = c.ID_Empresa " +
                            "WHERE e.Nombre_Empresa = @empresa";
                        SqlCommand cmdContactos = new SqlCommand(queryContactos, conn);
                        cmdContactos.Parameters.AddWithValue("@empresa", empresaSeleccionada);

                        SqlDataReader reader = cmdContactos.ExecuteReader();
                        cmbContacto.Items.Clear();

                        while (reader.Read())
                        {
                            cmbContacto.Items.Add(reader["Nombre_Contacto"].ToString());
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

        //boton imprimir
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validación de campos obligatorios
                if (string.IsNullOrWhiteSpace(txtCotizacion.Text) ||
                    string.IsNullOrWhiteSpace(txtPedido.Text) ||
                    //string.IsNullOrWhiteSpace(txtHojaServicio.Text) ||
                    string.IsNullOrWhiteSpace(cmbCliente.Text) ||
                    string.IsNullOrWhiteSpace(cmbContacto.Text) ||
                    string.IsNullOrWhiteSpace(cmbProveedor.Text) ||
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
                logo.Alignment = Element.ALIGN_RIGHT;
                logo.SetAbsolutePosition(doc.PageSize.Width - doc.RightMargin - 170,
                                         doc.PageSize.Height - doc.TopMargin - 100);

                // Agregar al documento
                doc.Add(logo);

                // Folio en la parte superior izquierda
                Paragraph folioEncabezado = new Paragraph("Folio: " + folioSolicitud,
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
                tabla.AddCell(new PdfPCell(new Phrase(txtServicio.Text.Trim(), fuenteNormal)));






                PdfPCell celda1 = new PdfPCell(new Phrase("Razon social"));
                celda1.FixedHeight = 30f;
                celda1.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda2 = new PdfPCell(new Phrase(cmbProveedor.Text));
                celda2.FixedHeight = 30f;


                PdfPCell celda3 = new PdfPCell(new Phrase("# Cliente"));
                celda3.FixedHeight = 30f;
                celda3.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda4 = new PdfPCell(new Phrase(cmbCliente.Text));
                celda4.FixedHeight = 30f;


                PdfPCell celda5 = new PdfPCell(new Phrase("Nombre del contacto"));
                celda5.FixedHeight = 30f;
                celda5.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda6 = new PdfPCell(new Phrase(cmbContacto.Text));
                celda6.FixedHeight = 30f;


                PdfPCell celda7 = new PdfPCell(new Phrase("Fecha de solicitud"));
                celda7.FixedHeight = 30f;
                celda7.BackgroundColor = new BaseColor(220, 220, 220);

                PdfPCell celda8 = new PdfPCell(new Phrase(dtpFecha.Value.ToShortDateString()));
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

                PdfPCell celda14 = new PdfPCell(new Phrase(txtCotizacion.Text));
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

                // Recorrer filas del DataGridView
                foreach (DataGridViewRow row in dgvServicios.Rows)
                {
                    if (row.IsNewRow) continue;

                    tablaServicios.AddCell(row.Cells["Tipo_Servicio"].Value?.ToString());
                    tablaServicios.AddCell(row.Cells["Equipo"].Value?.ToString());
                    tablaServicios.AddCell(row.Cells["Tipo_Sistema"].Value?.ToString());
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



        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Verificar que el TextBox no esté vacío
            if (!string.IsNullOrWhiteSpace(txtNo_Hoja.Text))
            {
                // Crear una nueva fila
                int index = dgvHoja.Rows.Add();

                // Asignar el valor del TextBox a la primera columna (por ejemplo, Folio)
                dgvHoja.Rows[index].Cells["HojaCenso"].Value = txtNo_Hoja.Text;

                // Limpiar el TextBox
                txtNo_Hoja.Clear();
                txtNo_Hoja.Focus();
            }
            else
            {
                MessageBox.Show("Ingrese un número de hoja de servicio.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private int? folioSolicitudExistenteSeleccionada = null; // nullable, se asigna al seleccionar fila

        private void button3_Click(object sender, EventArgs e)
        {

            if (!int.TryParse(textBox1.Text.Trim(), out int folioIngresado))
            {
                MessageBox.Show("El folio ingresado no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (FolioExiste(folioIngresado))
            {
                MessageBox.Show($"El folio {folioIngresado} ya existe. Ingresa un folio diferente.", "Folio duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Valores de la primera fila del dgvServicios
                            string tipoServicioCab = null;
                            string tipoSistemaCab = null;
                            string tipoEquipoCab = null;
                            string descripcionCab = null;

                            if (dgvServicios.Rows.Count > 0 && !dgvServicios.Rows[0].IsNewRow)
                            {
                                tipoServicioCab = dgvServicios.Rows[0].Cells["Tipo_Servicio"].Value?.ToString();
                                tipoSistemaCab = dgvServicios.Rows[0].Cells["Tipo_Sistema"].Value?.ToString();
                                tipoEquipoCab = dgvServicios.Rows[0].Cells["Equipo"].Value?.ToString();
                                descripcionCab = dgvServicios.Rows[0].Cells["Descripcion"].Value?.ToString();
                            }

                            // Insertar cabecera en Control_Interno
                            string queryControlInterno = @"
                        INSERT INTO Control_Interno
                        (ID_Folio, No_Cotizacion, No_Pedido, Razon_Social, No_Cliente, Nombre_Contacto, Fecha_Solicitud, Ejecutivo_Asignado, Tipo_Servicio, Tipo_Equipo,Tipo_Sistema)
                        VALUES (@IDFolio, @NoCotizacion, @NoPedido, @RazonSocial, @NoCliente, @NombreContacto, @FechaSolicitud, @Ejecutivo, @TipoServicio, @TipoEquipo,@TipoSistema);
                        SELECT SCOPE_IDENTITY();";

                            int idFolioCon;
                            int idFolioVisible;

                            using (SqlCommand cmd = new SqlCommand(queryControlInterno, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@IDFolio", folioIngresado);
                                cmd.Parameters.AddWithValue("@NoCotizacion", txtCotizacion.Text);
                                cmd.Parameters.AddWithValue("@NoPedido", txtPedido.Text);
                                cmd.Parameters.AddWithValue("@RazonSocial", cmbProveedor.Text);
                                cmd.Parameters.AddWithValue("@NoCliente", cmbCliente.Text);
                                cmd.Parameters.AddWithValue("@NombreContacto", cmbContacto.Text);
                                cmd.Parameters.AddWithValue("@FechaSolicitud", dtpFecha.Value);
                                cmd.Parameters.AddWithValue("@Ejecutivo", cmbEjecutivo.Text);
                                cmd.Parameters.AddWithValue("@TipoServicio", (object)tipoServicioCab ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@TipoSistema", (object)tipoSistemaCab ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@TipoEquipo", (object)tipoEquipoCab ?? DBNull.Value);

                                idFolioCon = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // --- Actualizar la fila seleccionada de Solicitud_Folio si aplica ---
                            if (folioSolicitudExistenteSeleccionada.HasValue)
                            {
                                string queryUpdate = @"
                            UPDATE Solicitud_Folio
                            SET Status_Folio = 'Abierto'
                            WHERE ID_Folio_Con = @IDFolio";
                                using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn, transaction))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@IDFolio", folioSolicitudExistenteSeleccionada.Value);
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }

                            // Obtener ID_Folio para usar en Hojas_Servicio
                            string queryGetFolio = "SELECT ID_Folio FROM Control_Interno WHERE ID_Folio_Con = @IDFolio_Con";
                            using (SqlCommand cmdFolio = new SqlCommand(queryGetFolio, conn, transaction))
                            {
                                cmdFolio.Parameters.AddWithValue("@IDFolio_Con", idFolioCon);
                                idFolioVisible = Convert.ToInt32(cmdFolio.ExecuteScalar());
                            }

                            // Guardar Servicios
                            foreach (DataGridViewRow row in dgvServicios.Rows)
                            {
                                if (row.IsNewRow) continue;

                                string queryHojaServicio = @"
                            INSERT INTO Hojas_Servicio 
                            (ID_Folio, Folio_Hoja, Descripcion,Censo)
                            VALUES (@IDFolio, @FolioHoja,@Descripcion, @Censo)";
                                using (SqlCommand cmdHojaServ = new SqlCommand(queryHojaServicio, conn, transaction))
                                {
                                    cmdHojaServ.Parameters.AddWithValue("@IDFolio", idFolioVisible);
                                    cmdHojaServ.Parameters.AddWithValue("@FolioHoja", txtServicio.Text.Trim());
                                    cmdHojaServ.Parameters.AddWithValue("@Censo", "No");
                                    cmdHojaServ.Parameters.AddWithValue("@Descripcion", (object)descripcionCab ?? DBNull.Value);
                                    cmdHojaServ.ExecuteNonQuery();
                                }
                            }

                            // Guardar Censos
                            foreach (DataGridViewRow hojaRow in dgvHoja.Rows)
                            {
                                if (hojaRow.IsNewRow) continue;

                                string queryHojaCenso = @"
                            INSERT INTO Hojas_Servicio 
                            (ID_Folio, Folio_Hoja, Censo)
                            VALUES (@IDFolio, @FolioHoja, @Censo)";
                                using (SqlCommand cmdHojaCenso = new SqlCommand(queryHojaCenso, conn, transaction))
                                {
                                    cmdHojaCenso.Parameters.AddWithValue("@IDFolio", idFolioVisible);
                                    cmdHojaCenso.Parameters.AddWithValue("@FolioHoja", hojaRow.Cells["HojaCenso"].Value?.ToString() ?? "");
                                    cmdHojaCenso.Parameters.AddWithValue("@Descripcion", (object)descripcionCab ?? DBNull.Value);
                                    cmdHojaCenso.Parameters.AddWithValue("@Censo", "Si");
                                    cmdHojaCenso.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            folioSolicitud = idFolioVisible.ToString();
                            MessageBox.Show($"Solicitud guardada correctamente con folio: {folioSolicitud}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor selecciona una fila en la tabla para agregarla a los servicios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener la fila seleccionada
            DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];

            // Extraer el ID_Folio_Con
            folioSolicitudExistenteSeleccionada = filaSeleccionada.Cells["ID_Folio_Con"].Value != null
                ? Convert.ToInt32(filaSeleccionada.Cells["ID_Folio_Con"].Value)
                : (int?)null;

            // Resto de tu código para autocompletar información
            string descripcion = filaSeleccionada.Cells["Descripcion"].Value?.ToString() ?? "";
            string nombreEmpresa = filaSeleccionada.Cells["Nombre_Empresa"].Value?.ToString() ?? "";

            // Autocompletado en dgvServicios y comboboxes
            int nuevaFila = dgvServicios.Rows.Add();
            dgvServicios.Rows[nuevaFila].Cells["Descripcion"].Value = descripcion;
            dgvServicios.Rows[nuevaFila].Cells["Tipo_Sistema"].Value = "N/A";
            cmbProveedor.Text = nombreEmpresa;

            // Fecha
            if (filaSeleccionada.Cells["Fecha_Solicitud"].Value != null &&
                DateTime.TryParse(filaSeleccionada.Cells["Fecha_Solicitud"].Value.ToString(), out DateTime fechaSolicitud))
            {
                dtpFecha.Value = fechaSolicitud;
            }
            else
            {
                dtpFecha.Value = DateTime.Now;
            }

            // Obtener No_Cliente de la BD
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string empresaquery = "SELECT No_Cliente FROM Empresas WHERE Nombre_Empresa = @empresa";
                    using (SqlCommand cmd = new SqlCommand(empresaquery, conn))
                    {
                        cmd.Parameters.AddWithValue("@empresa", nombreEmpresa);
                        object result = cmd.ExecuteScalar();
                        cmbCliente.Text = result != null ? result.ToString() : string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el número de cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MessageBox.Show($"Servicio de '{nombreEmpresa}' agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}


    
    


