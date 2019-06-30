using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antrian.models;

namespace Antrian.DBAccess
{
    public class DBCommand
    {
        SqlConnection conn;

        public DBCommand(SqlConnection connection)
        {
            this.conn = connection;
        }

        ~DBCommand() { }

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

        public string GetKodePoli()
        {
            string kodePoli = "";
            var poliklinik = Properties.Settings.Default.poliklinik;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 [kode_poli] FROM [tb_poliklinik] WHERE [nama_poli]=@nama_poli", conn);
                cmd.Parameters.AddWithValue("nama_poli", poliklinik);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        kodePoli = reader["kode_poli"].ToString();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return kodePoli;
        }

        public List<ModelAntrianPoli> GetAntrianPoli()
        {
            List<ModelAntrianPoli> antrianPoli = new List<ModelAntrianPoli>();
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select tb_antrian_poli.*, tb_pasien.nama from tb_antrian_poli join tb_pasien on tb_antrian_poli.no_rm = tb_pasien.no_rekam_medis where tb_antrian_poli.poliklinik = @poli and tb_antrian_poli.tgl_berobat = CONVERT(date, getdate(), 111) and status='Antri'", conn);
                cmd.Parameters.AddWithValue("poli", GetKodePoli());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        antrianPoli.Add(new ModelAntrianPoli(reader["id"].ToString(), reader["no_rm"].ToString(), reader["nama"].ToString(),
                            int.Parse(reader["no_urut"].ToString()), reader["poliklinik"].ToString(), reader["status"].ToString(),
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

        public int GetNoAntriPeriksa()
        {
            int no_antri = 0;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select top 1 no_urut from tb_antrian_poli where poliklinik=@poliklinik and tgl_berobat = CONVERT(date, getdate(), 111) and status='Periksa' order by 1 desc", conn);
                cmd.Parameters.AddWithValue("poliklinik", GetKodePoli());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        no_antri = reader.GetInt32(0);
                    }
                }

                CloseConnection();
            }
            catch(SqlException ex)
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
                SqlCommand cmd = new SqlCommand("select count(no_urut) from tb_antrian_poli where status='Antri' and tgl_berobat=CONVERT(date, getdate(), 111) and poliklinik=@poliklinik", conn);
                cmd.Parameters.AddWithValue("poliklinik", GetKodePoli());
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
