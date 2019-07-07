using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antrian_Apotik.DBAccess
{
    internal class DBCommand
    {
        private readonly SqlConnection conn;

        public DBCommand(SqlConnection conn)
        {
            this.conn = conn;
        }

        ~DBCommand()
        {
        }

        public bool OpenConnection()
        {
            try
            {
                if (conn.State.Equals(ConnectionState.Closed))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (SqlException)
            {
                return false;
            }

            return false;
        }

        public bool CloseConnection()
        {
            try
            {
                if (conn.State.Equals(ConnectionState.Open))
                {
                    conn.Close();
                    return true;
                }
            }
            catch (SqlException)
            {
                return false;
            }

            return false;
        }

        public int GetNoAntriPeriksa()
        {
            var no_antri = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select top 1 no_antrian from tb_antrian_apotik where tgl_resep = CONVERT(date, getdate(), 111) and status='Periksa' or status='Panggil' order by 1 desc",
                    conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) no_antri = reader.GetInt32(0);
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return no_antri;
        }

        public int GetTotalPasien()
        {
            var total = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select count(no_antrian) from tb_antrian_apotik where status='Antri' and tgl_resep=CONVERT(date, getdate(), 111)",
                    conn);
                total = int.Parse(cmd.ExecuteScalar().ToString());

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return total;
        }

        public List<ModelAntrianApotik> GetDataAntrian()
        {
            var listAntrina = new List<ModelAntrianApotik>();
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select tb_antrian_apotik.*, tb_pasien.nama from tb_antrian_apotik left join tb_pasien on tb_antrian_apotik.no_rm = tb_pasien.no_rekam_medis where tgl_resep=CONVERT(date, getdate(), 111) and status='Antri' order by no_antrian ASC", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        listAntrina.Add(new ModelAntrianApotik(reader["id"].ToString(), reader["no_rm"].ToString(),
                            reader["no_resep"].ToString(), reader["no_antrian"].ToString(), reader["status"].ToString(),
                            reader["tgl_resep"].ToString(), reader["nama"].ToString()));
                    }
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return listAntrina;
        }
    }
}