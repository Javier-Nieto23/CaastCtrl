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
        //atrubuto estatico para guardar el id del usuario actual
        public static int IdUsuarioActual { get; private set; }
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
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    
                    string query = "SELECT ID_Usuario FROM Usuarios_Caast WHERE Nombre_Usuario=@usuario AND Password=@contrasena";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);

                        
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            IdUsuarioActual = Convert.ToInt32(result);
                            return true; // Login exitoso
                        }
                        else
                        {
                                                       return false; // Login fallido
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo acceder a la base de datos, revise config.txt", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
