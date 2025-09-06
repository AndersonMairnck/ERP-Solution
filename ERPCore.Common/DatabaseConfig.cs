using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Common
{
    public static class DatabaseConfig
    {
        public static string MySqlVersion => "8.0.21"; // Altere para sua versão do MySQL
        public static string Charset => "utf8mb4";
        public static string Collation => "utf8mb4_unicode_ci";

        public static string GetConnectionString(string server, string database, string userId, string password)
        {
            return $"Server={server};Database={database};Uid={userId};Pwd={password};";
        }
    }
}
