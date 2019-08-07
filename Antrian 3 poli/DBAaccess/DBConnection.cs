using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antrian_3_poli.DBAaccess
{
    class DBConnection
    {
        private static SqlConnection MsqlConn;

        /// <summary>
        ///     create connection to the database
        /// </summary>
        /// <returns></returns>
        private DBConnection()
        {
        }

        ~DBConnection()
        {
        }

        public static SqlConnection dbConnection()
        {
            if (MsqlConn == null)
            {
                var connectionString =
                    ConfigurationManager.ConnectionStrings["klinikDatabaseConeection"].ConnectionString;
                MsqlConn = new SqlConnection(connectionString);
            }

            return MsqlConn;
        }
    }
}
