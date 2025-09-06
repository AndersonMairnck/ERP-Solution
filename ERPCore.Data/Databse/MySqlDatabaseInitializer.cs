using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace ERPCore.Data.Database
{
    public class MySqlDatabaseInitializer
    {
        private readonly string _connectionString;

        public MySqlDatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            // Extrair server e database da connection string
            var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.Database;
            builder.Database = ""; // Remover database para conectar ao server

            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                connection.Open();

                // Criar database se não existir
                using (var command = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {databaseName} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci", connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Agora conectar ao database específico e executar scripts
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Executar scripts SQL
                ExecuteScript(connection, "01_Create_Database.sql");
                ExecuteScript(connection, "02_Create_Tables.sql");
                ExecuteScript(connection, "03_Insert_Initial_Data.sql");
            }
        }

        private void ExecuteScript(MySqlConnection connection, string scriptName)
        {
            var scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Database", "Scripts");
            var scriptPath = Path.Combine(scriptsPath, scriptName);

            if (File.Exists(scriptPath))
            {
                var scriptContent = File.ReadAllText(scriptPath);

                // Dividir o script em comandos individuais
                var commands = scriptContent.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var commandText in commands)
                {
                    if (!string.IsNullOrWhiteSpace(commandText))
                    {
                        using (var command = new MySqlCommand(commandText, connection))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erro executando comando: {ex.Message}");
                                // Continuar mesmo com erro em comandos individuais
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Script não encontrado: {scriptPath}");
            }
        }
    }
}