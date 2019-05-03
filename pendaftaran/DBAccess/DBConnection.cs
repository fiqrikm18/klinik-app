using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pendaftaran.DBAccess
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
