using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    internal class conexion
    {
        public class ConexionBD
        {
            public static string LeerConexion()
            {
                string ruta = "C:\\ WindowsFormsApp1\\config.txt"; // Debe estar en la misma carpeta que el .exe
                if (!File.Exists(ruta))
                {
                    throw new FileNotFoundException("El archivo de configuración no existe.");
                }

                string conexion = File.ReadAllText(ruta).Trim();
                return conexion;
            }

            public static SqlConnection ObtenerConexion()
            {
                string cadena = LeerConexion();
                SqlConnection conn = new SqlConnection(cadena);
                return conn;
            }
        }
    }
}
