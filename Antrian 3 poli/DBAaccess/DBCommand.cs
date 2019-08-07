using Antrian_3_poli.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antrian_3_poli.DBAaccess
{
    class DBCommand
    {
        private readonly SqlConnection conn;

        public DBCommand(SqlConnection connection)
        {
            conn = connection;
        }

        ~DBCommand()
        {
        }

        public void OpenConnection()
        {
            if (conn.State.Equals(ConnectionState.Closed))
            {
                conn.Open();
            }
        }

        public void CloseConnection()
        {
            if (conn.State.Equals(ConnectionState.Open))
            {
                conn.Close();
            }
        }

        public List<ModelAntrianPoli> GetAntrianPoli()
        {
            List<ModelAntrianPoli> antrianPoli = new List<ModelAntrianPoli>();
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(
                    "select tb_antrian.*, tb_pasien.nama from tb_antrian join tb_pasien on tb_antrian.no_rm = tb_pasien.no_rekam_medis where tb_antrian.tgl_berobat = CONVERT(date, getdate(), 111) and tujuan_antrian='Poliklinik' and status='Antri'",
                    conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        antrianPoli.Add(new ModelAntrianPoli(reader["id"].ToString(), reader["no_rm"].ToString(),
                            reader["nama"].ToString(),
                            int.Parse(reader["no_urut"].ToString()), reader["poliklinik"].ToString(),
                            reader["status"].ToString(),
                            reader["tgl_berobat"].ToString()));
                    }
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return antrianPoli;
        }

        public int GetNoAntriPeriksa(string kodePoli)
        {
            int no_antri = 0;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(
                    "select top 1 no_urut from tb_antrian where tujuan_antrian='Poliklinik' and poliklinik=@poliklinik and tgl_berobat = CONVERT(date, getdate(), 111) and status='Panggil' order by 1 desc",
                    conn);
                cmd.Parameters.AddWithValue("poliklinik", kodePoli);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        no_antri = reader.GetInt32(0);
                    }
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
            int total = 0;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(
                    "select count(no_urut) from tb_antrian where status='Antri' and tgl_berobat=CONVERT(date, getdate(), 111) and tujuan_antrian = 'Poliklinik'",
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
    }
}
