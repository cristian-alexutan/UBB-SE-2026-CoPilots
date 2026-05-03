using Microsoft.Data.SqlClient;

namespace Content.Repository
{
    public class DatabaseConnectionFactory
    {
        private readonly string connectionString;

        public DatabaseConnectionFactory()
        {
            connectionString = @"Server=.\SQLEXPRESS;Initial Catalog=DutyFreeShops;Integrated Security=true; TrustServerCertificate=True";
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}