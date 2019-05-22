using System.Configuration;
using MySql.Data.MySqlClient;

namespace pendaftaran.DBAccess
{
    internal class DBConnection
    {
        private static MySqlConnection MsqlConn;

        /// <summary>
        ///     create connection to the database
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection dbConnection()
        {
            if (MsqlConn == null)
            {
                var connectionString =
                    ConfigurationManager.ConnectionStrings["klinikDatabaseConeection"].ConnectionString;
                MsqlConn = new MySqlConnection(connectionString);
            }

            return MsqlConn;
        }
    }
}