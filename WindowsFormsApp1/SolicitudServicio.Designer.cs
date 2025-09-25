namespace WindowsFormsApp1
{
    partial class SolicitudServicio
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SolicitudServicio));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtServicio = new System.Windows.Forms.TextBox();
            this.cmbContacto = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvHoja = new System.Windows.Forms.DataGridView();
            this.HojaCenso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtNo_Hoja = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbEjecutivo = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbCliente = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbProveedor = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPedido = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCotizacion = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvServicios = new System.Windows.Forms.DataGridView();
            this.dataSet1 = new System.Data.DataSet();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.btnGenerarPDF = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoja)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtServicio);
            this.groupBox1.Controls.Add(this.cmbContacto);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dgvHoja);
            this.groupBox1.Controls.Add(this.txtNo_Hoja);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cmbEjecutivo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.dtpFecha);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cmbCliente);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbProveedor);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPedido);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtCotizacion);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(670, 205);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Solicitud de servicio";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(213, 163);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "#Hoja de Servicio";
            // 
            // txtServicio
            // 
            this.txtServicio.Location = new System.Drawing.Point(214, 179);
            this.txtServicio.Name = "txtServicio";
            this.txtServicio.Size = new System.Drawing.Size(175, 20);
            this.txtServicio.TabIndex = 22;
            // 
            // cmbContacto
            // 
            this.cmbContacto.FormattingEnabled = true;
            this.cmbContacto.Location = new System.Drawing.Point(15, 140);
            this.cmbContacto.Name = "cmbContacto";
            this.cmbContacto.Size = new System.Drawing.Size(175, 21);
            this.cmbContacto.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(420, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "#Hoja de Censo";
            // 
            // dgvHoja
            // 
            this.dgvHoja.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHoja.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.HojaCenso});
            this.dgvHoja.Location = new System.Drawing.Point(420, 66);
            this.dgvHoja.Name = "dgvHoja";
            this.dgvHoja.Size = new System.Drawing.Size(240, 124);
            this.dgvHoja.TabIndex = 18;
            // 
            // HojaCenso
            // 
            this.HojaCenso.HeaderText = "HojaCenso";
            this.HojaCenso.Name = "HojaCenso";
            this.HojaCenso.Width = 200;
            // 
            // txtNo_Hoja
            // 
            this.txtNo_Hoja.Location = new System.Drawing.Point(420, 40);
            this.txtNo_Hoja.Name = "txtNo_Hoja";
            this.txtNo_Hoja.Size = new System.Drawing.Size(142, 20);
            this.txtNo_Hoja.TabIndex = 17;
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(559, 29);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(37, 40);
            this.button2.TabIndex = 16;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 164);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Ejecutivo Asignado";
            // 
            // cmbEjecutivo
            // 
            this.cmbEjecutivo.FormattingEnabled = true;
            this.cmbEjecutivo.Location = new System.Drawing.Point(15, 178);
            this.cmbEjecutivo.Name = "cmbEjecutivo";
            this.cmbEjecutivo.Size = new System.Drawing.Size(175, 21);
            this.cmbEjecutivo.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(211, 123);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Fecha de solicitud";
            // 
            // dtpFecha
            // 
            this.dtpFecha.Location = new System.Drawing.Point(214, 139);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(175, 20);
            this.dtpFecha.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Nombre del contacto";
            // 
            // cmbCliente
            // 
            this.cmbCliente.Location = new System.Drawing.Point(214, 94);
            this.cmbCliente.Multiline = true;
            this.cmbCliente.Name = "cmbCliente";
            this.cmbCliente.Size = new System.Drawing.Size(175, 21);
            this.cmbCliente.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(211, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "# Cliente";
            // 
            // cmbProveedor
            // 
            this.cmbProveedor.FormattingEnabled = true;
            this.cmbProveedor.Location = new System.Drawing.Point(15, 94);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(175, 21);
            this.cmbProveedor.TabIndex = 7;
            this.cmbProveedor.SelectedIndexChanged += new System.EventHandler(this.cmbProveedor_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Razon social";
            // 
            // txtPedido
            // 
            this.txtPedido.Location = new System.Drawing.Point(214, 40);
            this.txtPedido.Multiline = true;
            this.txtPedido.Name = "txtPedido";
            this.txtPedido.Size = new System.Drawing.Size(175, 30);
            this.txtPedido.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "#Pedido";
            // 
            // txtCotizacion
            // 
            this.txtCotizacion.Location = new System.Drawing.Point(15, 40);
            this.txtCotizacion.Multiline = true;
            this.txtCotizacion.Name = "txtCotizacion";
            this.txtCotizacion.Size = new System.Drawing.Size(175, 30);
            this.txtCotizacion.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "#Cotizacion";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvServicios);
            this.groupBox2.Location = new System.Drawing.Point(13, 225);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(670, 149);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Descripcion de Servicio";
            // 
            // dgvServicios
            // 
            this.dgvServicios.AllowUserToDeleteRows = false;
            this.dgvServicios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServicios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvServicios.Location = new System.Drawing.Point(3, 16);
            this.dgvServicios.Name = "dgvServicios";
            this.dgvServicios.Size = new System.Drawing.Size(664, 130);
            this.dgvServicios.TabIndex = 0;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(19, 380);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(46, 44);
            this.button1.TabIndex = 3;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btnGenerarPDF
            // 
            this.btnGenerarPDF.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerarPDF.Image")));
            this.btnGenerarPDF.Location = new System.Drawing.Point(637, 380);
            this.btnGenerarPDF.Name = "btnGenerarPDF";
            this.btnGenerarPDF.Size = new System.Drawing.Size(46, 44);
            this.btnGenerarPDF.TabIndex = 1;
            this.btnGenerarPDF.UseVisualStyleBackColor = true;
            this.btnGenerarPDF.Click += new System.EventHandler(this.button1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            // 
            // button3
            // 
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.Location = new System.Drawing.Point(585, 380);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(46, 44);
            this.button3.TabIndex = 4;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(715, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(355, 205);
            this.dataGridView1.TabIndex = 5;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(715, 241);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(121, 60);
            this.button4.TabIndex = 6;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // SolicitudServicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1082, 436);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnGenerarPDF);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SolicitudServicio";
            this.Text = "CaastCtrl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoja)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPedido;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCotizacion;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbEjecutivo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox cmbCliente;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGenerarPDF;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Data.DataSet dataSet1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.DataGridView dgvServicios;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvHoja;
        private System.Windows.Forms.TextBox txtNo_Hoja;
        private System.Windows.Forms.ComboBox cmbContacto;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtServicio;
        private System.Windows.Forms.DataGridViewTextBoxColumn HojaCenso;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button4;
    }
}

