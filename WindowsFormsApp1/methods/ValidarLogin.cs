using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms; //para Application.StartupPath

namespace WindowsFormsApp1
{
    //en esta clase se valida que los datos ingresados son correctos 
    public class LoginService
    {
        private string GetConnectionString()
        {
            // Ruta dinámica donde está el .exe
            string filePath = Path.Combine(Application.StartupPath, "config.txt");
            // Alternativa más genérica:
            // string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No se encontró el archivo config.txt en: {filePath}");
            }

            var config = new Dictionary<string, string>();

            foreach (string line in File.ReadAllLines(filePath))
            {
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
                {
                    var parts = line.Split('=');
                    config[parts[0].Trim()] = parts[1].Trim();
                }
            }

            return $"Server={config["Server"]};Database={config["Database"]};User Id={config["User Id"]};Password={config["Password"]};";
        }

        public bool ValidarLogin(string usuario, string contrasena)
        {
            string connStr = GetConnectionString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Usuarios_Caast WHERE Nombre_Usuario=@usuario AND Password=@contrasena";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
