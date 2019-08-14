using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using dokter.models;
using dokter.Properties;

namespace dokter.DBAccess
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

        public bool Login(string id, string pass)
        {
            //var res = 0;
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand("select count(*) from tb_dokter where id=@id and password=@pass and tugas=@tugas",
                        conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("tugas", GetKodePoli());
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

        public List<ModelDiagnosis> GetDataDiagnosis()
        {
            var data = new List<ModelDiagnosis>();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select * from tb_diagnosis order by kode asc", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        data.Add(new ModelDiagnosis(int.Parse(reader["id"].ToString()), reader["kode"].ToString(),
                            reader["deskripsi"].ToString()));
                }

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return data;
        }

        public List<ModelTindakan> GetDataTindakan()
        {
            var data = new List<ModelTindakan>();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select * from tb_tindakan order by kode asc", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        data.Add(new ModelTindakan(int.Parse(reader["id"].ToString()), reader["kode"].ToString(),
                            reader["deskripsi"].ToString()));
                }

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return data;
        }

        public string GetKodePoli()
        {
            var kodePoli = "";
            var poliklinik = Settings.Default.Role;
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

        public List<ModelObat> GetDataObat()
        {
            var obat = new List<ModelObat>();
            OpenConnection();

            try
            {
                var cmd = new SqlCommand("select * from tb_obat", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        obat.Add(new ModelObat(reader["kode_obat"].ToString(), reader["nama_obat"].ToString(),
                            int.Parse(reader["stok"].ToString()),
                            reader["satuan"].ToString(), reader["harga_beli"].ToString(),
                            reader["harga_jual"].ToString(),
                            reader["harga_resep"].ToString(), reader["tgl_insert"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return obat;
        }

        public List<ModelAntrian> GetDataAntrian(string tgl_Berobat)
        {
            var antrian = new List<ModelAntrian>();
            OpenConnection();

            try
            {
                var cmd = new SqlCommand(
                    "SELECT tb_pasien.nama as nama, tb_antrian.* FROM tb_antrian LEFT JOIN [tb_pasien] ON tb_antrian.no_rm = tb_pasien.no_rekam_medis WHERE [tgl_berobat]= @tgl_berobat AND [poliklinik]=@poliklinik and tujuan_antrian='Poliklinik' AND Not status='Selesai' ORDER BY no_urut ASC",
                    conn);
                cmd.Parameters.AddWithValue("tgl_berobat", tgl_Berobat);
                cmd.Parameters.AddWithValue("poliklinik", GetKodePoli());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        antrian.Add(new ModelAntrian(reader["id"].ToString(), reader["no_rm"].ToString(),
                            reader["nama"].ToString(), int.Parse(reader["no_urut"].ToString()),
                            reader["poliklinik"].ToString(), reader["status"].ToString(),
                            reader["tgl_berobat"].ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            CloseConnection();
            return antrian;
        }

        public List<ModelDokter> GetDataDokter()
        {
            var dokter = new List<ModelDokter>();

            try
            {
                OpenConnection();
                var cmd = new SqlCommand("SELECT * FROM tb_dokter", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        dokter.Add(new ModelDokter(reader["id"].ToString(), reader["nama"].ToString(),
                            reader["telp"].ToString(),
                            reader["spesialisasi"].ToString(), reader["alamat"].ToString(),
                            reader["password"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dokter;
        }

        public List<ModelDokter> GetDataDokter(string id)
        {
            var dokter = new List<ModelDokter>();

            try
            {
                OpenConnection();
                var cmd = new SqlCommand("SELECT * FROM tb_dokter WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        dokter.Add(new ModelDokter(reader["id"].ToString(), reader["nama"].ToString(),
                            reader["telp"].ToString(),
                            reader["spesialisasi"].ToString(), reader["alamat"].ToString(),
                            reader["password"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dokter;
        }

        public List<ModelPasien> GetDataPasien()
        {
            var pasien = new List<ModelPasien>();

            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select * from tb_pasien", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        pasien.Add(new ModelPasien(reader["no_identitas"].ToString(),
                            reader["no_rekam_medis"].ToString(), reader["nama"].ToString(),
                            reader["tanggal_lahir"].ToString(), reader["jenis_kelamin"].ToString(),
                            reader["no_telp"].ToString(),
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

        public List<ModelRekamMedis> GetDataRekamMedis()
        {
            var rekam_medis = new List<ModelRekamMedis>();
            OpenConnection();

            try
            {
                //SqlCommand cmd = new SqlCommand("select tb_rekam_medis.*, tb_pasien.nama as nama_pasien, tb_dokter.nama as nama_dokter, tb_poliklinik.nama_poli as nama_poli from tb_rekam_medis left join tb_dokter on tb_rekam_medis.id_dokter = tb_dokter.id left join tb_poliklinik on tb_rekam_medis.poli = tb_poliklinik.kode_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_rekam_medis.no_rm", conn);
                var cmd = new SqlCommand(
                    "select top 1 tb_rekam_medis.*, tb_pasien.nama as nama_pasien, tb_dokter.nama as nama_dokter, tb_poliklinik.nama_poli as nama_poli from tb_rekam_medis left join tb_dokter on tb_rekam_medis.id_dokter = tb_dokter.id left join tb_poliklinik on tb_rekam_medis.poli = tb_poliklinik.kode_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_rekam_medis.no_rm order by tgl_pemeriksaan DESC",
                    conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        rekam_medis.Add(new ModelRekamMedis(int.Parse(reader["id"].ToString()),
                            reader["no_rm"].ToString(), reader["riwayat_penyakit"].ToString(),
                            reader["alergi"].ToString(), reader["berat_badan"].ToString(), reader["keluhan"].ToString(),
                            reader["diagnosa"].ToString(), reader["tindakan"].ToString(),
                            reader["id_dokter"].ToString(), reader["poli"].ToString(),
                            reader["tgl_pemeriksaan"].ToString(), reader["nama_dokter"].ToString(),
                            reader["nama_poli"].ToString(), reader["nama_pasien"].ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            CloseConnection();
            return rekam_medis;
        }

        public List<ModelRekamMedis> GetAllDataRekamMedisFrom(string no_rm = null)
        {
            var rekam_medis = new List<ModelRekamMedis>();
            OpenConnection();

            try
            {
                //SqlCommand cmd = new SqlCommand("select tb_rekam_medis.*, tb_pasien.nama as nama_pasien, tb_dokter.nama as nama_dokter, tb_poliklinik.nama_poli as nama_poli from tb_rekam_medis left join tb_dokter on tb_rekam_medis.id_dokter = tb_dokter.id left join tb_poliklinik on tb_rekam_medis.poli = tb_poliklinik.kode_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_rekam_medis.no_rm", conn);
                //SqlCommand cmd = new SqlCommand(
                //    "select distinct a.no_rm, a.riwayat_penyakit, a.berat_badan, a.keluhan, a.alergi, STUFF((select distinct ';' + td.deskripsi +'('+td.kode+')' from tb_rekam_medis left join tb_diagnosis td on tb_rekam_medis.diagnosa = td.kode where tgl_pemeriksaan = a.tgl_pemeriksaan FOR XML PATH ('')), 1, 1, '') as diagnosa, stuff((select distinct ';' + tt.deskripsi+'('+tt.kode+')' from tb_rekam_medis a left join tb_tindakan tt on a.tindakan = tt.kode where tgl_pemeriksaan = a.tgl_pemeriksaan FOR XML PATH ('')), 1, 1, '') as tindakan, a.id_dokter, b.nama as nama_dokter, a.poli, c.nama_poli as nama_poli, a.tgl_pemeriksaan from tb_rekam_medis a left join tb_dokter b on a.id_dokter = b.id left join tb_poliklinik c on c.kode_poli = a.poli",
                //    conn);

                var cmd = new SqlCommand("[getDataRekamMedis]", conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        rekam_medis.Add(new ModelRekamMedis(
                            reader["no_rm"].ToString(), reader["riwayat_penyakit"].ToString(),
                            reader["alergi"].ToString(), reader["berat_badan"].ToString(), reader["keluhan"].ToString(),
                            reader["diagnosa"].ToString(), reader["tindakan"].ToString(),
                            reader["id_dokter"].ToString(), reader["poli"].ToString(),
                            reader["tgl_pemeriksaan"].ToString(), reader["nama_dokter"].ToString(),
                            reader["nama_poli"].ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            CloseConnection();
            return rekam_medis;
        }

        public string GetNoRmByNoUrut()
        {
            var no_rm = "";

            try
            {
                OpenConnection();
                var command =
                    new SqlCommand(
                        "SELECT TOP 1 no_rm FROM tb_antrian WHERE tgl_berobat = CONVERT(date, GETDATE(), 111) AND poliklinik=@poliklinik AND status='Panggil' ORDER BY no_urut DESC",
                        conn);

                //var command =
                //    new SqlCommand(
                //        "SELECT TOP 1 no_rm FROM tb_antrian WHERE tgl_berobat = CONVERT(date, GETDATE(), 111) AND poliklinik=@poliklinik AND status='Antri' ORDER BY no_urut ASC",
                //        conn);
                command.Parameters.AddWithValue("poliklinik", GetKodePoli());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) no_rm = reader["no_rm"].ToString();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return no_rm;
        }

        public bool InsertDataRekamMedis(string no_rm, string riwayat_penyakit, string alergi, string berat_badan,
            string keluhan, string diagnosa, string tindakan, string id_dokter, string poli)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "INSERT INTO [dbo].[tb_rekam_medis] ([no_rm] ,[riwayat_penyakit] ,[alergi], [berat_badan] ,[keluhan] ,[diagnosa] ,[tindakan] ,[id_dokter] ,[poli]) VALUES (@no_rm,@riwayat_penyakit,@alergi,@berat_badan,@keluhan,@diagnosa,@tindakan,@id_dokter,@poli)",
                    conn);
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
                var cmd = new SqlCommand(
                    "INSERT INTO [dbo].[tb_resep]([kode_resep],[no_rm],[no_resep],[id_dokter]) VALUES(@kode_resep,@no_rm,@no_resep,@id_dokter)",
                    conn);
                cmd.Parameters.AddWithValue("kode_resep", kode_resep);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.Parameters.AddWithValue("no_resep", no_resep);
                cmd.Parameters.AddWithValue("id_dokter", id_dokter);
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

        public bool IsDataNomorResepExist(string no_rm, string kode_resep)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select count(kode_resep) from tb_resep where no_rm =@no_rm and kode_resep=@kode_resep order by 1 desc",
                    conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.Parameters.AddWithValue("kode_resep", kode_resep);
                var res = int.Parse(cmd.ExecuteScalar().ToString());

                if (res > 0) return true;

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool InsertDetailResep(string kode_resep, string kode_obat, int jumlah, string keterangan, string dosis,
            string penggunaan)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "INSERT INTO [dbo].[tb_detail_resep]([no_resep],[kode_obat],[jumlah],[ket], [penggunaan], [dosis]) VALUES(@no_resep,@kode_obat,@jumlah,@ket, @penggunaan, @dosis)",
                    conn);
                cmd.Parameters.AddWithValue("no_resep", kode_resep);
                cmd.Parameters.AddWithValue("kode_obat", kode_obat);
                cmd.Parameters.AddWithValue("jumlah", jumlah);
                cmd.Parameters.AddWithValue("ket", keterangan);
                cmd.Parameters.AddWithValue("dosis", dosis);
                cmd.Parameters.AddWithValue("penggunaan", penggunaan);
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

        public bool InsertAntrianApotik(string no_rm, string no_resep, string no_urut, string status)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "INSERT INTO tb_antrian(no_rm, no_resep, no_urut, status, tujuan_antrian) values(@no_rm, @no_resep, @no_urut, @status, 'Apotik')",
                    conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.Parameters.AddWithValue("no_resep", no_resep);
                cmd.Parameters.AddWithValue("no_urut", no_urut);
                cmd.Parameters.AddWithValue("status", status);

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public int GetLastNoUrutApotik()
        {
            var res = 0;

            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select top 1 no_urut from tb_antrian where tgl_berobat = convert(varchar(10), getdate(), 111) and tujuan_antrian='Apotik' order by 1 desc",
                    conn);

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

        public int GetLastNoResep(string no_rm)
        {
            var res = 0;

            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select top 1 no_resep from tb_resep where no_rm =@no_rm order by 1 desc",
                    conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);

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

        public int CountDataAntrian()
        {
            var res = 0;

            try
            {
                OpenConnection();
                var command =
                    new SqlCommand(
                        "select COUNT(*) from tb_antrian where poliklinik=@poliklinik and tgl_berobat=CONVERT(date, getdate(), 111) and status='Panggil'",
                        conn);
                command.Parameters.AddWithValue("poliklinik", GetKodePoli());
                res = int.Parse(command.ExecuteScalar().ToString());

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public int LastAntrian()
        {
            var res = 0;
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "select top 1 no_urut from tb_antrian where tgl_berobat=CONVERT(date, getdate(), 111) and poliklinik=@poli and status='Antri' and tujuan_antrian='Poliklinik' order by 1 asc",
                        conn);
                cmd.Parameters.AddWithValue("poli", GetKodePoli());

                if (conn.State.Equals(ConnectionState.Closed)) OpenConnection();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) res = reader.GetInt32(0);
                }

                conn.Close();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public int LastAntrianPrev()
        {
            var res = 0;
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "select top 1 no_urut from tb_antrian where tgl_berobat=CONVERT(date, getdate(), 111) and poliklinik=@poli and status='Panggil' and tujuan_antrian='Poliklinik' order by 1 desc",
                        conn);
                cmd.Parameters.AddWithValue("poli", GetKodePoli());

                if (conn.State.Equals(ConnectionState.Closed)) OpenConnection();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) res = reader.GetInt32(0);
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public bool UpdateAntrianPrev()
        {
            try
            {
                var cmd =
                    new SqlCommand(
                        "update tb_antrian set status='Antri' where no_urut=@no_urut and poliklinik=@poli and tujuan_antrian='Poliklinik' and status='Panggil' and tgl_berobat=convert(date, getdate(), 111)",
                        conn);
                cmd.Parameters.AddWithValue("no_urut", LastAntrianPrev());
                cmd.Parameters.AddWithValue("poli", GetKodePoli());

                if (conn.State.Equals(ConnectionState.Closed)) OpenConnection();

                if (cmd.ExecuteNonQuery() == 1) return true;

                conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool UpdateAntrian()
        {
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "update tb_antrian set status='Panggil' where no_urut=@no_urut and poliklinik=@poli and tujuan_antrian='Poliklinik' and tgl_berobat=convert(date, getdate(), 111)",
                        conn);
                cmd.Parameters.AddWithValue("no_urut", LastAntrian());
                cmd.Parameters.AddWithValue("poli", GetKodePoli());
                if (conn.State.Equals(ConnectionState.Closed)) OpenConnection();

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public int GetLastAntrianPoli()
        {
            var res = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select top 1 no_urut from tb_antrian where status='Panggil' and tgl_berobat=CONVERT(date, GETDATE(), 111) order by 1 desc",
                    conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) res = reader.GetInt32(0);
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public bool UpdateStatusAntrian(string no_rm)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "update tb_antrian set status='Selesai' where no_rm=@no_rm and poliklinik=@poli and no_urut=@no_urut and tujuan_antrian='Poliklinik' and tgl_berobat=convert(date, getdate(), 111)",
                    conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.Parameters.AddWithValue("poli", GetKodePoli());
                cmd.Parameters.AddWithValue("no_urut", GetLastAntrianPoli());
                CloseConnection();
                OpenConnection();

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool DeleteRekamMedis(int id)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("DELETE FROM [dbo].[tb_rekam_medis] WHERE id=@id", conn);
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

        public bool UpdateDataRekamMedis(string no_rm, string riwayat_penyakit, string alergi, string berat_badan,
            string keluhan, string diagnosa, string tindakan, string id_dokter, string poli)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "UPDATE [dbo].[tb_rekam_medis] set [riwayat_penyakit]=@riwayat_penyakit ,[alergi]=@alergi, [berat_badan]=@berat_badan ,[keluhan]=@keluhan ,[diagnosa]=@diagnosa ,[tindakan]=@tindakan ,[id_dokter]=@id_dokter ,[poli]=@poli where [no_rm]=@no_rm",
                    conn);
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
    }
}