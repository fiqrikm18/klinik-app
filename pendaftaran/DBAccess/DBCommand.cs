using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using pendaftaran.models;

namespace pendaftaran.DBAccess
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

        public DataTable GetDataPasien(string no_rm)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDataPasien]", conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.CommandType = CommandType.StoredProcedure;

                var adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable GetDataRekamMedis(string no_rm)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[getDataRekamMedis]", conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.CommandType = CommandType.StoredProcedure;

                var adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable GetDetailPasien(string no_rm)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDetailPasien]", conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.CommandType = CommandType.StoredProcedure;

                var adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public bool Login(string id, string pass)
        {
            //var res = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select count(*) from tb_pendaftaran where id=@id and password=@pass",
                    conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("pass", pass);

                if (int.Parse(cmd.ExecuteScalar().ToString()) > 0) return true;

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public DataTable GetReportDataPasien(string no_rm)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("GetDataPasien", conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.CommandType = CommandType.StoredProcedure;

                var adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public int CountIdPasienExists(string identitas)
        {
            var res = 0;

            try
            {
                OpenConnection();

                var cmd = new SqlCommand(
                    "select count(no_identitas) from tb_pasien where no_identitas=@no_identitas",
                    conn);
                cmd.Parameters.AddWithValue("no_identitas", identitas);
                res = int.Parse(cmd.ExecuteScalar().ToString());

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public int CountRmPasienExists(string no_rm)
        {
            var res = 0;

            try
            {
                OpenConnection();

                var cmd = new SqlCommand(
                    "select count(no_rekam_medis) from tb_pasien where no_rekam_medis=@no_rekam_medis", conn);
                cmd.Parameters.AddWithValue("no_rekam_medis", no_rm);
                res = int.Parse(cmd.ExecuteScalar().ToString());

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public bool InsertDataPasien(string no_id, string no_rm, string nama, string tgl_lahir, string jenis_kelamin,
            string no_telp, string alamat, string golongan_darah, string jenis_kartu)
        {
            try
            {
                OpenConnection();

                var cmd = new SqlCommand(
                    "insert into tb_pasien (no_identitas, jenis_identitas,  no_rekam_medis, nama, tanggal_lahir, jenis_kelamin, no_telp, alamat, golongan_darah) values(@no_identitas, @jenis_identitas, @no_rekam_medis, @nama, @tanggal_lahir, @jenis_kelamin, @no_telp, @alamat, @golongan_darah)",
                    conn);
                //@no_identitas, @no_rekam_medis, @nama, @tanggal_lahir, @jenis_kelamin, @no_telp, @alamat
                cmd.Parameters.AddWithValue("no_identitas", no_id);
                cmd.Parameters.AddWithValue("jenis_identitas", jenis_kartu);
                cmd.Parameters.AddWithValue("no_rekam_medis", no_rm);
                cmd.Parameters.AddWithValue("nama", nama);
                cmd.Parameters.AddWithValue("tanggal_lahir", tgl_lahir);
                cmd.Parameters.AddWithValue("jenis_kelamin", jenis_kelamin);
                cmd.Parameters.AddWithValue("no_telp", no_telp);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("golongan_darah", golongan_darah);
                var res = cmd.ExecuteNonQuery();

                CloseConnection();
                if (res == 1) return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool InsertAntrian(string no_rm, int no_urut, string poliklinik, string status = "Antri")
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "insert into tb_antrian(no_rm, no_urut, poliklinik, status, tujuan_antrian) values(@no_rm, @no_urut, @poliklinik, @status, @tujuan_antrian)",
                    conn);
                //@no_rm, @no_urut, @poliklinik, @status
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.Parameters.AddWithValue("no_urut", no_urut);
                cmd.Parameters.AddWithValue("poliklinik", poliklinik);
                cmd.Parameters.AddWithValue("status", status);
                cmd.Parameters.AddWithValue("tujuan_antrian", "Poliklinik");
                var res = cmd.ExecuteNonQuery();

                CloseConnection();
                if (res == 1) return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public int GetLastNoUrut(string policode)
        {
            var res = 0;

            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select top 1 no_urut from tb_antrian where poliklinik = @policode and tgl_berobat = convert(varchar(10), getdate(), 111) order by 1 desc",
                    conn);
                cmd.Parameters.AddWithValue("policode", policode);

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

        public List<ModelAntrian> GetDataAntrian(string tgl_berobat)
        {
            var antrian = new List<ModelAntrian>();
            OpenConnection();

            try
            {
                OpenConnection();

                if (string.IsNullOrEmpty(tgl_berobat)) tgl_berobat = DateTime.Now.ToString("yyyy-MM-dd");

                var cmd = new SqlCommand(
                    "SELECT tb_pasien.nama as nama, tb_poliklinik.nama_poli as poli, tb_antrian.* FROM tb_antrian LEFT JOIN [tb_pasien] ON tb_antrian.no_rm = tb_pasien.no_rekam_medis left join tb_poliklinik on tb_poliklinik.kode_poli = tb_antrian.poliklinik where tgl_berobat=@tgl_berobat and tb_antrian.tujuan_antrian='Poliklinik' order by poliklinik",
                    conn);
                cmd.Parameters.AddWithValue("tgl_berobat", tgl_berobat);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        antrian.Add(new ModelAntrian(reader["id"].ToString(), reader["no_rm"].ToString(),
                            reader["nama"].ToString(),
                            int.Parse(reader["no_urut"].ToString()), reader["poliklinik"].ToString(),
                            reader["poli"].ToString(),
                            reader["status"].ToString(), reader["tgl_berobat"].ToString()));
                }

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return antrian;
        }

        public List<ComboboxPairs> GetPoliklinik()
        {
            var cbp = new List<ComboboxPairs>
            {
                new ComboboxPairs("Pilih", "Pilih")
            };

            try
            {
                OpenConnection();

                var cmd = new SqlCommand("select * from tb_poliklinik", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        cbp.Add(new ComboboxPairs(reader["nama_poli"].ToString(), reader["kode_poli"].ToString()));
                }

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return cbp;
        }

        public List<ModelPasien> GetDataPasien()
        {
            var pasien = new List<ModelPasien>();

            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select * from tb_pasien order by no_rekam_medis asc", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        pasien.Add(new ModelPasien(reader["no_identitas"].ToString(),
                            reader["no_rekam_medis"].ToString(), reader["nama"].ToString(),
                            DateTime.Parse(reader["tanggal_lahir"].ToString()).ToString("dd MMM yyyy"),
                            reader["jenis_kelamin"].ToString(), reader["no_telp"].ToString(),
                            reader["alamat"].ToString(), reader["tgl_daftar"].ToString(),
                            reader["golongan_darah"].ToString(), reader["jenis_identitas"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return pasien;
        }


        public bool UpdateDataPasien(string nama, string no_telp, string jenis_kelamin, string alamat, string id,
            string gol_darah)
        {
            try
            {
                OpenConnection();

                var cmd = new SqlCommand(
                    "update tb_pasien set nama=@namaPasien, jenis_kelamin=@jenisKelamin, no_telp=@noTelp, alamat=@alamat, golongan_darah=@golongan_darah where no_identitas=@identitas",
                    conn);
                cmd.Parameters.AddWithValue("golongan_darah", gol_darah);
                cmd.Parameters.AddWithValue("namaPasien", nama);
                cmd.Parameters.AddWithValue("jenisKelamin", jenis_kelamin);
                cmd.Parameters.AddWithValue("noTelp", no_telp);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("identitas", id);

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool DeleteDataPasien(string no_rm)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("delete from tb_pasien where no_rekam_medis=@no_rekam_medis", conn);
                cmd.Parameters.AddWithValue("no_rekam_medis", no_rm);

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool DetleDataAntrian(int id)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("delete from tb_antrian where id=@id", conn);
                cmd.Parameters.AddWithValue("id", id);

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }
    }
}