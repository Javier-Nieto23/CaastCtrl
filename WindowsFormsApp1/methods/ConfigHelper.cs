using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class ConfigConexion
    {
        public static class ConfigHelper
        {


            public static string GetConnectionString()
            {
                string path = Path.Combine(Application.StartupPath, "config.txt");

                if (!File.Exists(path))
                    throw new FileNotFoundException("No se encontró el archivo config.txt");

                string[] lineas = File.ReadAllLines(path, Encoding.UTF8);

                string server = "";
                string database = "";
                string user = "";
                string password = "";

                foreach (string linea in lineas)
                {
                    if (linea.StartsWith("Server="))
                        server = linea.Substring("Server=".Length).Trim();
                    else if (linea.StartsWith("Database="))
                        database = linea.Substring("Database=".Length).Trim();
                    else if (linea.StartsWith("User Id="))
                        user = linea.Substring("User Id=".Length).Trim();
                    else if (linea.StartsWith("Password="))
                        password = linea.Substring("Password=".Length).Trim();
                }

                return $"Server={server};Database={database};User Id={user};Password={password};";
            }
        }
        
    }
}
