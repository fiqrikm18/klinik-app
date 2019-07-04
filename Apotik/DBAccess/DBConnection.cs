using System.Configuration;
using System.Data.SqlClient;

namespace Apotik.DBAccess
{
    internal class DBConnection
    {
        private static SqlConnection conn;

        public static SqlConnection dbConnection()
        {
            if (conn == null)
            {
                var connectionString =
                    ConfigurationManager.ConnectionStrings["klinikDatabaseConeection"].ConnectionString;
                conn = new SqlConnection(connectionString);
            }

            return conn;
        }
    }
}