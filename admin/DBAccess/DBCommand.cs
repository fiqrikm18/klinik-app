using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using admin.models;
using admin.Utils;

namespace admin.DBAccess
{
    public class DBCommand
    {
        private readonly SqlConnection conn;
        private SqlCommand cmd;

        public DBCommand(SqlConnection conn)
        {
            this.conn = conn;
        }

        public void OpenConnection()
        {
            if (conn.State.Equals(ConnectionState.Closed)) conn.Open();
        }

        public void CloseConnection()
        {
            if (conn.State.Equals(ConnectionState.Open)) conn.Close();
        }

        public DataTable DataTableTransaksi()
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDataTransaksi]", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable DataTableTransaksiByTgl(string tgl)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDataTransaksiByTgl]", conn);
                cmd.Parameters.AddWithValue("tgl", tgl);
                cmd.CommandType = CommandType.StoredProcedure;

                var adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable DataTableTransaksiByApoteker(string apoteker)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDataTransaksiByApoteker]", conn);
                cmd.Parameters.AddWithValue("apoteker", apoteker);
                cmd.CommandType = CommandType.StoredProcedure;

                var adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable DataTableTransaksiByApotekerTgl(string apoteker, string tgl)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDataTransaksiByApotekerTGL]", conn);
                cmd.Parameters.AddWithValue("apoteker", apoteker);
                cmd.Parameters.AddWithValue("tgl", tgl);
                cmd.CommandType = CommandType.StoredProcedure;

                var adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public List<ModelKeuangan> GetDataKeuangan()
        {
            var keuangan = new List<ModelKeuangan>();

            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select * from tb_keuangan", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        keuangan.Add(new ModelKeuangan(reader["id"].ToString(), reader["nama"].ToString(),
                            reader["telp"].ToString(),
                            reader["jenis_kelamin"].ToString(), reader["password"].ToString(),
                            reader["alamat"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return keuangan;
        }

        public List<ModelTransaksi> GetDataTransaksi()
        {
            var transaksi = new List<ModelTransaksi>();
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "select tb_transaksi.*, ta.nama as nama_apoteker from tb_transaksi left join tb_apoteker ta on tb_transaksi.apoteker = ta.id",
                        conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        transaksi.Add(new ModelTransaksi(int.Parse(reader["id"].ToString()),
                            reader["apoteker"].ToString(), reader["nama_apoteker"].ToString(),
                            reader["kode_resep"].ToString(), int.Parse(reader["total"].ToString()),
                            reader["tgl_transaksi"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return transaksi;
        }

        public DataTable GetDataApoteker(string id)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDataApoteker]", conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.CommandType = CommandType.StoredProcedure;

                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable GetDatakeuangan(string id)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select * from tb_keuangan where id=@id", conn);
                cmd.Parameters.AddWithValue("id", id);

                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable GetDataDokter(string id)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDataDokter]", conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.CommandType = CommandType.StoredProcedure;

                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable GetDataStaffPendaftaran(string id)
        {
            var dt = new DataTable();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("[GetDataStaffDaftar]", conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.CommandType = CommandType.StoredProcedure;

                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                CloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public bool LoginK(string id, string pass)
        {
            //var res = 0;
            try
            {
                OpenConnection();

                var cmd = new SqlCommand("select count(*) from tb_keuangan where id=@id and password=@pass", conn);
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

        public bool Login(string id, string pass)
        {
            //var res = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select count(*) from tb_admin where id=@id and password=@pass", conn);
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

        #region GetData

        public List<MPendaftaran> GetDataPendaftaran()
        {
            var pendaftaran = new List<MPendaftaran>();

            try
            {
                OpenConnection();
                cmd = new SqlCommand("SELECT * FROM tb_pendaftaran", conn);


                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        pendaftaran.Add(new MPendaftaran(reader["id"].ToString(), reader["nama"].ToString(),
                            reader["alamat"].ToString(),
                            reader["telp"].ToString(), reader["password"].ToString(),
                            reader["jenis_kelamin"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return pendaftaran;
        }

        public List<MApoteker> GetDataApoteker()
        {
            var apoteker = new List<MApoteker>();
            //apoteker.Add(new MApoteker("pilih", "Pilih", "pilih", "pilih", "pilih"));

            try
            {
                OpenConnection();
                cmd = new SqlCommand("SELECT * FROM tb_apoteker", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        apoteker.Add(new MApoteker(reader["id"].ToString(), reader["nama"].ToString(),
                            reader["telp"].ToString(),
                            reader["alamat"].ToString(), reader["password"].ToString(),
                            reader["jenis_kelamin"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return apoteker;
        }

        public List<MDokter> GetDataDokter()
        {
            var dokter = new List<MDokter>();

            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "select  tb_dokter.*, tb_poliklinik.nama_poli as nama_poli from tb_dokter left join tb_poliklinik on tb_dokter.tugas = tb_poliklinik.kode_poli",
                    conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        dokter.Add(new MDokter(reader["id"].ToString(), reader["nama"].ToString(),
                            reader["telp"].ToString(),
                            reader["spesialisasi"].ToString(), reader["alamat"].ToString(),
                            reader["password"].ToString(), reader["nama_poli"].ToString(),
                            reader["jenis_kelamin"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dokter;
        }

        public List<ComboboxPairs> GetDataPoliklinik()
        {
            var cp = new List<ComboboxPairs>
            {
                new ComboboxPairs("000", "Pilih")
            };

            try
            {
                OpenConnection();
                cmd = new SqlCommand("select * from tb_poliklinik", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        cp.Add(new ComboboxPairs(reader["kode_poli"].ToString(), reader["nama_poli"].ToString()));
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return cp;
        }

        public List<MPoliklinik> GetDataPoliKlinik()
        {
            var cp = new List<MPoliklinik>();

            try
            {
                OpenConnection();
                cmd = new SqlCommand("select * from tb_poliklinik", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        cp.Add(new MPoliklinik(reader["kode_poli"].ToString(), reader["nama_poli"].ToString()));
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return cp;
        }

        #endregion

        #region InsertData

        public bool InsertDataPoliklinik(string kode_poli, string nama_poli)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("insert into tb_poliklinik(kode_poli, nama_poli) values(@kode_poli, @nama_poli)",
                    conn);
                cmd.Parameters.AddWithValue("kode_poli", kode_poli);
                cmd.Parameters.AddWithValue("nama_poli", nama_poli);

                if (cmd.ExecuteNonQuery() == 1) return true;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool InsertDataDokter(string id, string nama, string telp, string alamat, string spesialisasi,
            string tugas, string jenis_kelamin, string password)
        {
            if (spesialisasi == "" || string.IsNullOrEmpty(spesialisasi)) spesialisasi = "-";

            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "insert into tb_dokter(id, nama, telp, alamat, spesialisasi, tugas, jenis_kelamin, password) values(@id, @nama, @telp, @alamat, @spesialisai, @tugas, @jenis_kelamin, @password)",
                    conn);
                //id, nama, telp, alamat, spesialisai, tugas, jenis_kelamin, password
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("nama", nama);
                cmd.Parameters.AddWithValue("telp", telp);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("spesialisai", spesialisasi);
                cmd.Parameters.AddWithValue("tugas", tugas);
                cmd.Parameters.AddWithValue("jenis_kelamin", jenis_kelamin);
                cmd.Parameters.AddWithValue("password", Encryptor.MD5Hash(password));

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool InsertDataKeuangan(ModelKeuangan ku)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "insert into tb_keuangan(id, nama, telp, alamat, jenis_kelamin, password) values(@id, @nama, @telp, @alamat, @jenis_kelamin, @password)",
                    conn);
                //id, nama, telp, alamat, spesialisai, tugas, jenis_kelamin, password
                cmd.Parameters.AddWithValue("id", ku.id);
                cmd.Parameters.AddWithValue("nama", ku.nama);
                cmd.Parameters.AddWithValue("telp", ku.telp);
                cmd.Parameters.AddWithValue("alamat", ku.alamat);
                cmd.Parameters.AddWithValue("jenis_kelamin", ku.jenis_kelamin);
                cmd.Parameters.AddWithValue("password", Encryptor.MD5Hash(ku.password));

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool InsertDataStaff(string id, string nama, string telp, string alamat, string jenis_kelamin,
            string password)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "insert into tb_pendaftaran(id, nama, telp, alamat, jenis_kelamin, password) values(@id, @nama, @telp, @alamat, @jenis_kelamin, @password)",
                    conn);
                // id, nama, telp, alamat, jenis_kelamin, password
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("nama", nama);
                cmd.Parameters.AddWithValue("telp", telp);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("jenis_kelamin", jenis_kelamin);
                cmd.Parameters.AddWithValue("password", Encryptor.MD5Hash(password));

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool InsertDataApoteker(string id, string nama, string telp, string alamat, string jenis_kelamin,
            string password)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "insert into tb_apoteker(id, nama, telp, alamat, jenis_kelamin, password) values(@id, @nama, @telp, @alamat, @jenis_kelamin, @password)",
                    conn);
                // id, nama, telp, alamat, jenis_kelamin, password
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("nama", nama);
                cmd.Parameters.AddWithValue("telp", telp);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("jenis_kelamin", jenis_kelamin);
                cmd.Parameters.AddWithValue("password", Encryptor.MD5Hash(password));

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        #endregion

        #region UpdateData

        public bool UpdateDataApoteker(string id, string nama, string jenis_kelamin, string telp, string alamat)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "update tb_apoteker set nama=@nama, alamat=@alamat, telp=@telp, jenis_kelamin=@jenis_kelamin where id=@id",
                    conn);
                cmd.Parameters.AddWithValue("nama", nama);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("telp", telp);
                cmd.Parameters.AddWithValue("jenis_kelamin", jenis_kelamin);
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

        public bool UpdateDataKeuangan(ModelKeuangan ku)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "update tb_keuangan set nama=@nama, alamat=@alamat, telp=@telp, jenis_kelamin=@jenis_kelamin where id=@id",
                    conn);
                cmd.Parameters.AddWithValue("nama", ku.nama);
                cmd.Parameters.AddWithValue("alamat", ku.alamat);
                cmd.Parameters.AddWithValue("telp", ku.telp);
                cmd.Parameters.AddWithValue("jenis_kelamin", ku.jenis_kelamin);
                cmd.Parameters.AddWithValue("id", ku.id);

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool UpdateDataStaff(string id, string nama, string jenis_kelamin, string telp, string alamat)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "update tb_pendaftaran set nama=@nama, alamat=@alamat, telp=@telp, jenis_kelamin=@jenis_kelamin where id=@id",
                    conn);
                cmd.Parameters.AddWithValue("nama", nama);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("telp", telp);
                cmd.Parameters.AddWithValue("jenis_kelamin", jenis_kelamin);
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

        public bool UpdateDataDokter(string id, string nama, string alamat, string telp, string spesialisasi,
            string tugas, string jenis_kelamin)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand(
                    "update tb_dokter set nama=@nama, alamat=@alamat, telp=@telp, spesialisasi=@spesialisasi, tugas=@tugas, jenis_kelamin=@jenis_kelamin where id=@id",
                    conn);
                cmd.Parameters.AddWithValue("nama", nama);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("telp", telp);
                cmd.Parameters.AddWithValue("jenis_kelamin", jenis_kelamin);
                cmd.Parameters.AddWithValue("spesialisasi", spesialisasi);
                cmd.Parameters.AddWithValue("tugas", tugas);
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

        #endregion

        #region DeleteData

        public bool DeleteDataPoliklinik(string id)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("delete from tb_poliklinik where kode_poli=@kode_poli", conn);
                cmd.Parameters.AddWithValue("kode_poli", id);

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public bool DeleteDataDokter(string id)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("delete from tb_dokter where id=@id", conn);
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

        public bool DeleteDatakeuangan(string id)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("delete from tb_keuangan where id=@id", conn);
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

        public bool DeleteDataApoteker(string id)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("delete from tb_apoteker where id=@id", conn);
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

        public bool DeleteDataStaff(string id)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("delete from tb_pendaftaran where id=@id", conn);
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

        #endregion

        #region CheckData

        public int CheckApotekerExsist(string id)
        {
            var res = 0;

            try
            {
                OpenConnection();
                cmd = new SqlCommand("select count(id) from tb_apoteker where id=@id", conn);
                cmd.Parameters.AddWithValue("id", id);

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

        public int CheckDokterExsist(string id)
        {
            var res = 0;

            try
            {
                OpenConnection();
                cmd = new SqlCommand("select count(id) from tb_dokter where id=@id", conn);
                cmd.Parameters.AddWithValue("id", id);

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

        public int CheckStaffExsist(string id)
        {
            var res = 0;

            try
            {
                OpenConnection();
                cmd = new SqlCommand("select count(id) from tb_pendaftaran where id=@id", conn);
                cmd.Parameters.AddWithValue("id", id);

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

        public int CheckPoliExsist(string id)
        {
            var res = 0;

            try
            {
                OpenConnection();
                cmd = new SqlCommand("select count(kode_poli) from tb_poliklinik where kode_poli=@id", conn);
                cmd.Parameters.AddWithValue("id", id);

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

        #endregion
    }
}