using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Antrian.models;
using Antrian.Properties;

namespace Antrian.DBAccess
{
    public class DBCommand
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
            if (conn.State.Equals(ConnectionState.Closed)) conn.Open();
        }

        public void CloseConnection()
        {
            if (conn.State.Equals(ConnectionState.Open)) conn.Close();
        }

        public string GetKodePoli()
        {
            var kodePoli = "";
            var poliklinik = Settings.Default.poliklinik;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "SELECT TOP 1 [kode_poli] FROM [tb_poliklinik] WHERE [nama_poli]=@nama_poli",
                    conn);
                cmd.Parameters.AddWithValue("nama_poli", poliklinik);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) kodePoli = reader["kode_poli"].ToString();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return kodePoli;
        }

        public List<ModelAntrianApotik> GetAntrianApotik()
        {
            var antrianApotik = new List<ModelAntrianApotik>();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select tb_antrian.*, tb_pasien.nama from tb_antrian join tb_pasien on tb_antrian.no_rm = tb_pasien.no_rekam_medis where tb_antrian.tgl_berobat = CONVERT(date, getdate(), 111) and tujuan_antrian='Apotik' and status='Antri'",
                    conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        antrianApotik.Add(new ModelAntrianApotik(int.Parse(reader["id"].ToString()),
                            reader["no_rm"].ToString(), reader["no_urut"].ToString(),
                            reader["tujuan_antrian"].ToString(), reader["no_resep"].ToString(),
                            reader["status"].ToString(), reader["tgl_berobat"].ToString(), reader["nama"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return antrianApotik;
        }

        public List<ModelAntrianPoli> GetAntrianPoli()
        {
            var antrianPoli = new List<ModelAntrianPoli>();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select tb_antrian.*, tb_pasien.nama from tb_antrian join tb_pasien on tb_antrian.no_rm = tb_pasien.no_rekam_medis where tb_antrian.poliklinik = @poli and tb_antrian.tgl_berobat = CONVERT(date, getdate(), 111) and tujuan_antrian='Poliklinik' and status='Antri'",
                    conn);
                cmd.Parameters.AddWithValue("poli", GetKodePoli());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        antrianPoli.Add(new ModelAntrianPoli(reader["id"].ToString(), reader["no_rm"].ToString(),
                            reader["nama"].ToString(),
                            int.Parse(reader["no_urut"].ToString()), reader["poliklinik"].ToString(),
                            reader["status"].ToString(),
                            reader["tgl_berobat"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return antrianPoli;
        }

        public int GetNoAntriApotik()
        {
            var no_antrian = 0;
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "select top 1 no_urut from tb_antrian where tujuan_antrian='Apotik' and tgl_berobat = CONVERT(date, getdate(), 111) and status='Panggil' order by 1 desc",
                        conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) no_antrian = reader.GetInt32(0);
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return no_antrian;
        }

        public int GetNoAntriPeriksa()
        {
            var no_antri = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select top 1 no_urut from tb_antrian where poliklinik=@poliklinik and tujuan_antrian='Poliklinik' and tgl_berobat = CONVERT(date, getdate(), 111) and status='Panggil' order by 1 desc",
                    conn);
                cmd.Parameters.AddWithValue("poliklinik", GetKodePoli());

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

        public int GetTotalApotik()
        {
            var total = 0;
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "select count(no_urut) from tb_antrian where status='Antri' and tgl_berobat=CONVERT(date, getdate(), 111)  and tujuan_antrian = 'Apotik'",
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

        public int GetTotalPasien()
        {
            var total = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select count(no_urut) from tb_antrian where status='Antri' and tgl_berobat=CONVERT(date, getdate(), 111) and poliklinik=@poliklinik and tujuan_antrian = 'Poliklinik'",
                    conn);
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