using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using WindowsFormsApp1.methods;


namespace WindowsFormsApp1
{
    public partial class Form5 : Form
    {
        
        public Form5()
        {

            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            CargarUsuarios();
        }
        private void CargarUsuarios()
        {
            // Instancio la clase usando el constructor
            CargarUsuarios service = new CargarUsuarios();
            DataTable dt = service.Cargar_Usuarios();
            UsuariosGrid.DataSource = dt; // Asigno al DataGridView del formulario
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            Form1 mainForm = new Form1();
            mainForm.Show();
            this.Hide();
        }
    }
}

  



           
           
      
            
