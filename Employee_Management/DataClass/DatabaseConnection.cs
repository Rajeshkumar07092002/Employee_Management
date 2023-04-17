using Microsoft.AspNetCore.Components;
using System.Data.SqlClient;

namespace Employee_Management.DataClass
{
    public interface IDatabaseConnection
    {
        public SqlConnection GetConnection();
        public string ConnectionString { get; set; }
    }
    public class DatabaseConnection : IDatabaseConnection
    {
        private string? _connectionString; 
        public string ConnectionString
        {
            get
            {
                return _connectionString ?? string.Empty;
            }
            set
            {
                _connectionString = value;
            }
        }
        public DatabaseConnection(string? connectionString)
        {
            if (connectionString != null)
            {
                _connectionString = connectionString;
            }
        }
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }

}
