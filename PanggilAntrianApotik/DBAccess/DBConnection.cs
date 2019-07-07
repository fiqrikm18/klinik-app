using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanggilAntrianApotik.DBAccess
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
