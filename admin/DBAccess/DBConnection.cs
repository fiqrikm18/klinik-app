using System.Configuration;
using MySql.Data.MySqlClient;

namespace admin.DBAccess
{
    class DBConnection
    {
        static MySqlConnection MsqlConn = null;

        /// <summary>
        /// create connection to the database
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection dbConnection()
        {
            if (MsqlConn == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["klinikDatabaseConeection"].ConnectionString;
                MsqlConn = new MySqlConnection(connectionString);
            }

            return MsqlConn;
        }
    }
}
