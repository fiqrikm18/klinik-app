﻿using System.Configuration;
using System.Data.SqlClient;

namespace Antrian.DBAccess
{
    internal class DBConnection
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