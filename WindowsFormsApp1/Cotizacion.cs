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

namespace CaastCtrl
{
    public partial class Cotizacion : Form
    {
        public Cotizacion()
        {
            InitializeComponent();
        }


        private void CargarCotizaciones()
        {
            try
            {
                string query = "SELECT * FROM ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las cotizaciones: " + ex.Message);
            }
        }
    }
}
