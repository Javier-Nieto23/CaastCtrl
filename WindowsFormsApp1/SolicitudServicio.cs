using CaastCtrl;
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
using System.Xml.Linq;
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

            dataGridView1.MultiSelect = false;                   // Una sola fila a la vez
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;  // Selecciona toda la fila
            dataGridView1.ReadOnly = true;                      // Evita edición directa
            dataGridView1.AllowUserToAddRows = false;           // Evita fila vacía al final

            // Asignar folio al TextBox en lugar del GroupBox
            textBox1.Text = folioPreview.ToString();
            txtCotizacion.Text = "0";
            txtPedido.Text = "0";

            cmbProveedor.DropDownStyle = ComboBoxStyle.DropDown;
            cmbProveedor.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbProveedor.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbContacto.DropDownStyle = ComboBoxStyle.DropDown;
            cmbContacto.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbContacto.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbEjecutivo.DropDownStyle = ComboBoxStyle.DropDown;
            cmbEjecutivo.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbEjecutivo.AutoCompleteSource = AutoCompleteSource.ListItems;

            dgvHoja.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHoja.MultiSelect = false; // Opcional, para permitir solo una fila seleccionada
        }


        private void LimpiarCampos()
        {
            txtCotizacion.Clear();
            txtPedido.Clear();
            txtServicio.Clear();
            cmbCliente.Clear();
            cmbContacto.SelectedIndex = -1;
            cmbProveedor.SelectedIndex = -1;
            cmbEjecutivo.SelectedIndex = -1;
            dtpFecha.Value = DateTime.Now;
            dgvServicios.Rows.Clear();
            dgvHoja.Rows.Clear();
            folioSolicitudExistenteSeleccionada = null;
            textBox1.Clear();
            if (textBox1 == null)
            {
                int folioPreview = ObtenerSiguienteFolio();
                textBox1.Text = folioPreview.ToString();
            }

            ObtenerSiguienteFolio();

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
            int siguienteFolio = 3000;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigConexion.ConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(ID_Folio), 2999) + 1 FROM Control_Interno";
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
                string query = "Select sf.ID_Folio_Con,sf.Nombre_Empresa,sf.Fecha_Solicitud,sf.Descripcion,sf.Hoja,sf.Status_Folio,sf.Nombre_Contacto,uc.Nombre_Usuario from Solicitud_Folio as sf inner join Usuarios_Caast uc on sf.Ejecutivo = uc.ID_Usuario  WHERE sf.Status_Folio = 'Solicitado'";
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
                        // Puedes mostrar solo el nombre, o combinarlo con el ID o nombre corto
                        string display = $"{reader["Nombre_Empresa"]} ";
                        cmbProveedor.Items.Add(display);
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
            equipoCol.Name = "Tipo_Equipo";
            equipoCol.HeaderText = "Equipo";
            equipoCol.Items.AddRange("Escritorio", "Portatil","Servidor");
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
                Paragraph titulo = new Paragraph("Solicitud de Servicio: " + folioSolicitud,
                                   new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;

                doc.Add(titulo);
                doc.Add(new Paragraph(" "));

               
                // se crea la tabla Solicitud de servicio
                PdfPTable tabla = new PdfPTable(4);
                //ancho de la segunda tabla
                tabla.WidthPercentage = 100;

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

                tablaServicios.SetWidths(new float[] { 2f, 2f, 2f, 2f });


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
                Paragraph firmaParrafo = new Paragraph($"Firmado por: {firma}\nFecha: {DateTime.Now:MM/dd/yyyy}\nHora:{DateTime.Now: hh:mm:tt}",new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD));
                firmaParrafo.Alignment = Element.ALIGN_LEFT;
                doc.Add(firmaParrafo);
                // se debe colocar un if cuya condicion es si el usuario tiene firma personalizada en la base de datos se digita su firma, de lo contrario solo se colocar la firma del usuario actual

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
                            

                            // Insertar cabecera en Control_Interno
                            string queryControlInterno = @"
                            INSERT INTO Control_Interno
                            (ID_Folio, No_Cotizacion, No_Pedido, Razon_Social, No_Cliente, Nombre_Contacto, Fecha_Solicitud, Ejecutivo_Asignado)
                            VALUES (@IDFolio, @NoCotizacion, @NoPedido, @RazonSocial, @NoCliente, @NombreContacto, @FechaSolicitud, @Ejecutivo);
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
                                

                                idFolioCon = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            string queryGetFolio = "SELECT ID_Folio FROM Control_Interno WHERE ID_Folio_Con = @IDFolio_Con";
                            using (SqlCommand cmdFolio = new SqlCommand(queryGetFolio, conn, transaction))
                            {
                                cmdFolio.Parameters.AddWithValue("@IDFolio_Con", idFolioCon);
                                idFolioVisible = Convert.ToInt32(cmdFolio.ExecuteScalar());
                            }


                            if (folioSolicitudExistenteSeleccionada.HasValue)
                            {
                                string queryUpdate = @"
                                UPDATE Solicitud_Folio
                                SET Status_Folio = 'Abierto',
                                ID_Folio = @nuevoIDFolio
                                WHERE ID_Folio_Con = @IDFolio";
                                using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn, transaction))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@nuevoIDFolio", idFolioVisible);
                                    cmdUpdate.Parameters.AddWithValue("@IDFolio", folioSolicitudExistenteSeleccionada.Value);

                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }



                            // Obtener ID_Folio para usar en Hojas_Servicio
                            string queryGetFolioSol = "SELECT ID_Folio FROM Control_Interno WHERE ID_Folio_Con = @IDFolio_Con";
                            using (SqlCommand cmdFolio = new SqlCommand(queryGetFolioSol, conn, transaction))
                            {
                                cmdFolio.Parameters.AddWithValue("@IDFolio_Con", idFolioCon);
                                idFolioVisible = Convert.ToInt32(cmdFolio.ExecuteScalar());
                            }

                            // Guardar Servicios
                            foreach (DataGridViewRow row in dgvServicios.Rows)
                            {
                                if (row.IsNewRow) continue;
                                string tipoServicio = row.Cells["Tipo_Servicio"].Value?.ToString();
                                string tipoEquipo = row.Cells["Tipo_Equipo"].Value?.ToString();
                                string tipoSistema = row.Cells["Tipo_Sistema"].Value?.ToString();
                                string descripcion = row.Cells["Descripcion"].Value?.ToString();
                                string queryHojaServicio = @"
                                INSERT INTO Hojas_Servicio 
                                (ID_Folio, Folio_Hoja, Tipo_Servicio, Tipo_Equipo, Tipo_Sistema, Descripcion, Censo)
                                VALUES (@IDFolio, @FolioHoja, @TipoServicio, @TipoEquipo, @TipoSistema, @Descripcion, @Censo)";

                                using (SqlCommand cmdHojaServ = new SqlCommand(queryHojaServicio, conn, transaction))
                                {
                                    cmdHojaServ.Parameters.AddWithValue("@IDFolio", idFolioVisible);
                                    cmdHojaServ.Parameters.AddWithValue("@FolioHoja", txtServicio.Text);
                                    cmdHojaServ.Parameters.AddWithValue("@TipoServicio", (object)tipoServicio ?? DBNull.Value);
                                    cmdHojaServ.Parameters.AddWithValue("@TipoEquipo", (object)tipoEquipo ?? DBNull.Value);
                                    cmdHojaServ.Parameters.AddWithValue("@TipoSistema", (object)tipoSistema ?? DBNull.Value);
                                    cmdHojaServ.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                                    cmdHojaServ.Parameters.AddWithValue("@Censo", "No");
                                    cmdHojaServ.ExecuteNonQuery();
                                }
                            }

                            

                            // Guardar Censos
                            foreach (DataGridViewRow hojaRow in dgvHoja.Rows)
                            {
                                if (hojaRow.IsNewRow) continue;

                                string queryHojaCenso = @"
                                INSERT INTO Hojas_Servicio 
                                (ID_Folio, Folio_Hoja, Censo,Descripcion)
                                VALUES (@IDFolio, @FolioHoja, @Censo,@Descripcion)";
                                using (SqlCommand cmdHojaCenso = new SqlCommand(queryHojaCenso, conn, transaction))
                                {
                                    cmdHojaCenso.Parameters.AddWithValue("@IDFolio", idFolioVisible);
                                    cmdHojaCenso.Parameters.AddWithValue("@FolioHoja", hojaRow.Cells["HojaCenso"].Value?.ToString() ?? "");
                                    cmdHojaCenso.Parameters.AddWithValue("@Descripcion","Censo de equipo");
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
            string nombreContacto = filaSeleccionada.Cells["Nombre_Contacto"].Value?.ToString() ?? "";
            string nombreUsuario = filaSeleccionada.Cells["Nombre_Usuario"].Value?.ToString() ?? "";

            // Autocompletado en dgvServicios y comboboxes
            int nuevaFila = dgvServicios.Rows.Add();
            dgvServicios.Rows[nuevaFila].Cells["Descripcion"].Value = descripcion;
            dgvServicios.Rows[nuevaFila].Cells["Tipo_Sistema"].Value = "N/A";
            cmbProveedor.Text = nombreEmpresa;
            cmbContacto.Text = nombreContacto;
            cmbEjecutivo.Text = nombreUsuario;

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

        private void button5_Click(object sender, EventArgs e)
        {
            BtnBuscar empresa = new BtnBuscar();
            empresa.Show();
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
            if (dgvServicios.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvServicios.SelectedRows)
                {
                    if (!row.IsNewRow)
                        dgvServicios.Rows.Remove(row);
                }
            }
            else
            {
                 MessageBox.Show("Selecciona una fila para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            

            if (dgvHoja.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvHoja.SelectedRows)
                {
                    if (!row.IsNewRow)
                        dgvHoja.Rows.Remove(row);
                }
            }
            else
            {
                MessageBox.Show("Selecciona una fila para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Principal principal = new Principal();
            principal.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}


    
    


