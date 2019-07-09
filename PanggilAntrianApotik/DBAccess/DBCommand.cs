using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanggilAntrianApotik.DBAccess
{
    internal class DBCommand
    {
        private readonly SqlConnection conn;

        public DBCommand(SqlConnection conn)
        {
            this.conn = conn;
        }

        public void OpenConnection()
        {
            if (conn.State.Equals(ConnectionState.Closed))
                conn.Open();
        }

        public void CloseConnection()
        {
            if (conn.State.Equals(ConnectionState.Open))
                conn.Close();
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
                SqlCommand cmd = new SqlCommand("select tb_antrian_apotik.*, tb_pasien.nama from tb_antrian_apotik left join tb_pasien on tb_antrian_apotik.no_rm = tb_pasien.no_rekam_medis where tgl_resep=CONVERT(date, getdate(), 111) and status='Antri' order by no_antrian asc", conn);

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

        public int GetLastNoUrut()
        {
            var res = 0;

            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select top 1 no_antrian from tb_antrian_apotik where status='Antri' and tgl_resep = convert(varchar(10), getdate(), 111) order by 1 ASC",
                    conn);

                CloseConnection();
                OpenConnection();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) res = reader.GetInt32(0);
                }

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public bool UpdateStatusAntrian(int no_urut)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("update tb_antrian_apotik set status='Panggil' where no_antrian=@no_urut and tgl_resep=CONVERT(date, getdate(), 111)", conn);
                cmd.Parameters.AddWithValue("no_urut", no_urut);

                CloseConnection();
                OpenConnection();
                if(cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }

                CloseConnection();
            }
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }
    }
}