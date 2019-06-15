using dokter.models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace dokter.DBAccess
{
    class DBCommand
    {
        private SqlConnection conn;

        public DBCommand(SqlConnection conn)
        {
            this.conn = conn;
        }

        ~DBCommand() { }

        public void OpenConnection()
        {
            if (conn.State.Equals(System.Data.ConnectionState.Closed))
                conn.Open();
        }

        public void CloseConnection()
        {
            if (conn.State.Equals(System.Data.ConnectionState.Open))
                conn.Close();
        }

        public string GetKodePoli()
        {
            string kodePoli = "";
            var poliklinik = Properties.Settings.Default.Role;
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
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return kodePoli;
        }

        public List<ModelAntrian> GetDataAntrian(string tgl_Berobat)
        {
            List<ModelAntrian> antrian = new List<ModelAntrian>();
            OpenConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT tb_pasien.nama as nama, tb_antrian_poli.* FROM tb_antrian_poli LEFT JOIN [tb_pasien] ON tb_antrian_poli.no_rm = tb_pasien.no_rekam_medis WHERE [tgl_berobat]= @tgl_berobat AND [poliklinik]=@poliklinik AND status='Antri' ORDER BY no_urut ASC", conn);
                cmd.Parameters.AddWithValue("tgl_berobat", tgl_Berobat);
                cmd.Parameters.AddWithValue("poliklinik", GetKodePoli());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        antrian.Add(new ModelAntrian(reader["id"].ToString(), reader["no_rm"].ToString(), reader["nama"].ToString() ,int.Parse(reader["no_urut"].ToString()), reader["poliklinik"].ToString(), reader["status"].ToString(), reader["tgl_berobat"].ToString()));
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            CloseConnection();
            return antrian;
        }

        public List<ModelPasien> GetDataPasien()
        {
            List<ModelPasien> pasien = new List<ModelPasien>();

            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select * from tb_pasien", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pasien.Add(new ModelPasien(reader["no_identitas"].ToString(), reader["no_rekam_medis"].ToString(), reader["nama"].ToString(),
                            reader["tanggal_lahir"].ToString(), reader["jenis_kelamin"].ToString(), reader["no_telp"].ToString(),
                            reader["alamat"].ToString(), reader["tgl_daftar"].ToString()));
                    }
                }

                CloseConnection();
            }

            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return pasien;
        }

        public List<ModelRekamMedis> GetDataRekamMedis()
        {
            List<ModelRekamMedis> rekam_medis = new List<ModelRekamMedis>();
            OpenConnection();

            try
            {
                //SqlCommand cmd = new SqlCommand("select tb_rekam_medis.*, tb_pasien.nama as nama_pasien, tb_dokter.nama as nama_dokter, tb_poliklinik.nama_poli as nama_poli from tb_rekam_medis left join tb_dokter on tb_rekam_medis.id_dokter = tb_dokter.id left join tb_poliklinik on tb_rekam_medis.poli = tb_poliklinik.kode_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_rekam_medis.no_rm", conn);
                SqlCommand cmd = new SqlCommand("select top 1 tb_rekam_medis.*, tb_pasien.nama as nama_pasien, tb_dokter.nama as nama_dokter, tb_poliklinik.nama_poli as nama_poli from tb_rekam_medis left join tb_dokter on tb_rekam_medis.id_dokter = tb_dokter.id left join tb_poliklinik on tb_rekam_medis.poli = tb_poliklinik.kode_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_rekam_medis.no_rm order by tgl_pemeriksaan DESC", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rekam_medis.Add(new ModelRekamMedis(int.Parse(reader["id"].ToString()), reader["no_rm"].ToString(), reader["riwayat_penyakit"].ToString(),
                            reader["alergi"].ToString(), int.Parse(reader["berat_badan"].ToString()), reader["keluhan"].ToString(),
                            reader["diagnosa"].ToString(), reader["tindakan"].ToString(), reader["id_dokter"].ToString(), reader["poli"].ToString(),
                            reader["tgl_pemeriksaan"].ToString(), reader["nama_dokter"].ToString(), reader["nama_poli"].ToString(), reader["nama_pasien"].ToString()));
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            CloseConnection();
            return rekam_medis;
        }

        public bool InsertDataRekamMedis(string no_rm, string riwayat_penyakit, string alergi, string berat_badan, string keluhan, string diagnosa, string tindakan, string id_dokter, string poli)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[tb_rekam_medis] ([no_rm] ,[riwayat_penyakit] ,[alergi], [berat_badan] ,[keluhan] ,[diagnosa] ,[tindakan] ,[id_dokter] ,[poli]) VALUES (@no_rm,@riwayat_penyakit,@alergi,@berat_badan,@keluhan,@diagnosa,@tindakan,@id_dokter,@poli", conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.Parameters.AddWithValue("riwayat_penyakit", riwayat_penyakit);
                cmd.Parameters.AddWithValue("alergi", alergi);
                cmd.Parameters.AddWithValue("berat_badan", berat_badan);
                cmd.Parameters.AddWithValue("keluhan", keluhan);
                cmd.Parameters.AddWithValue("diagnosa", diagnosa);
                cmd.Parameters.AddWithValue("tindakan", tindakan);
                cmd.Parameters.AddWithValue("id_dokter", id_dokter);
                cmd.Parameters.AddWithValue("poli", poli);
                var res = cmd.ExecuteNonQuery();

                if (res == 1) return true;
                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool InsertDataResep(string kode_resep, string no_rm, string no_resep, string id_dokter)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[tb_resep]([kode_resep],[no_rm],[no_resep],[id_dokter]) VALUES(@kode_resep,@no_rm,@no_resep,@id_dokter)", conn);
                cmd.Parameters.AddWithValue("kode_resep", kode_resep);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.Parameters.AddWithValue("no_resep", no_resep);
                cmd.Parameters.AddWithValue("id_dokter", id_dokter);
                var res = cmd.ExecuteNonQuery();

                if (res == 1)
                    return true;
                CloseConnection();
            }
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool InsertDetailResep(string kode_resep, string kode_obat, int jumlah, string keterangan)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[detail_resep]([no_resep],[kode_obat],[jumlah],[keterangan]) VALUES(@no_resep,@kode_obat,@jumlah,@keterangan)", conn);
                cmd.Parameters.AddWithValue("no_resep", kode_resep);
                cmd.Parameters.AddWithValue("kode_obat", kode_obat);
                cmd.Parameters.AddWithValue("jumlah", jumlah);
                cmd.Parameters.AddWithValue("keterangan", keterangan);
                var res = cmd.ExecuteNonQuery();

                if (res == 1)
                    return true;

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
