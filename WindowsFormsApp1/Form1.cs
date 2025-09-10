using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.methods;
using static WindowsFormsApp1.methods.EmpresasDias;
using static WindowsFormsApp1.methods.CargarFolios;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            CargarRestantes();
            CargarFolios();
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
        private void CargarRestantes()
        {
            LicenciaService service = new LicenciaService();
            DataTable dt = service.ObtenerDiasRestantes();

            EmpresasGrid.Rows.Clear(); // Limpiar filas anteriores

            foreach (DataRow row in dt.Rows)
            {
                int index = EmpresasGrid.Rows.Add(); // Añadir nueva fila
                EmpresasGrid.Rows[index].Cells["EmpresaNombre"].Value = row["Nombre_Empresa"];
                EmpresasGrid.Rows[index].Cells["DiasRestantes"].Value = row["Dias_Restantes"];
                EmpresasGrid.Rows[index].Cells["FechaInicio"].Value = ((DateTime)row["Fecha_Inicio"]).ToString("dd/MM/yyyy"); // Formateo opcional
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // abrir menu de empresas
            BtnBuscar mainForm = new BtnBuscar();
            mainForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "¿Estás seguro que quieres cerrar sesión?",
                "Confirmar Cierre de Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {

                Application.Exit();

            }
            // Si elige "No", no se hace nada

        }



        private void button3_Click(object sender, EventArgs e)
        {
            SolicitudServicio mainForm = new SolicitudServicio();
            mainForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form5 userForm= new Form5();
            userForm.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
