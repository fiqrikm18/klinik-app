using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Apotik.DBAccess
{
    class DBConnection
    {
        static SqlConnection conn;

        public static SqlConnection dbConnection()
        {
            if(conn == null)
            {
                var connectionString = ConfigurationManager.ConnectionStrings["klinikDatabaseConeection"].ConnectionString;
                conn = new SqlConnection(connectionString);
            }

            return conn;
        }
    }
}
