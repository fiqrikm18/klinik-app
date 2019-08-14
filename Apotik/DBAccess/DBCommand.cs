using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Apotik.models;
using Apotik.Properties;

namespace Apotik.DBAccess
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

        public bool InsertDataObat(string kode_obat, string nama_obat, string satuan, string stok, string harga_jual,
            string harga_beli, string harga_resep)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "INSERT INTO [dbo].[tb_obat] ([kode_obat] ,[nama_obat] ,[stok] ,[satuan] ,[harga_beli] ,[harga_jual] ,[harga_resep]) VALUES (@kode_obat ,@nama_obat ,@stok ,@satuan ,@harga_beli ,@harga_jual ,@harga_resep)",
                    conn);
                cmd.Parameters.AddWithValue("kode_obat", kode_obat);
                cmd.Parameters.AddWithValue("nama_obat", nama_obat);
                cmd.Parameters.AddWithValue("stok", stok);
                cmd.Parameters.AddWithValue("satuan", satuan);
                cmd.Parameters.AddWithValue("harga_beli", harga_beli);
                cmd.Parameters.AddWithValue("harga_jual", harga_jual);
                cmd.Parameters.AddWithValue("harga_resep", harga_resep);
                var res = cmd.ExecuteNonQuery();

                if (res == 1)
                    return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public int LastAntrian()
        {
            var res = 0;
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "select top 1 no_urut from tb_antrian where tgl_berobat=CONVERT(date, getdate(), 111) and status='Antri' and tujuan_antrian='Apotik' order by 1 asc",
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

        public int LastAntrianPrev()
        {
            var res = 0;
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "select top 1 no_urut from tb_antrian where tgl_berobat=CONVERT(date, getdate(), 111) and status='Panggil' and tujuan_antrian='Apotik' order by 1 desc",
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

        public bool UpdateAntrianPrev()
        {
            try
            {
                var cmd =
                    new SqlCommand(
                        "update tb_antrian set status='Antri' where no_urut=@no_urut and tujuan_antrian='Apotik' and tgl_berobat=convert(date, getdate(), 111)",
                        conn);
                cmd.Parameters.AddWithValue("no_urut", LastAntrianPrev());

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

        public bool UpdateAntrian()
        {
            try
            {
                var cmd =
                    new SqlCommand(
                        "update tb_antrian set status='Panggil' where no_urut=@no_urut and tujuan_antrian='Apotik' and tgl_berobat=convert(date, getdate(), 111)",
                        conn);
                cmd.Parameters.AddWithValue("no_urut", LastAntrian());

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

        public bool Login(string id, string pass)
        {
            //var res = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select count(*) from tb_apoteker where id=@id and password=@pass", conn);
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

        public bool CreateTransactionResep(string apoteker, string kode_resep, int total)
        {
            try
            {
                OpenConnection();
                var cmd =
                    new SqlCommand(
                        "insert into tb_transaksi(apoteker, kode_resep, total) values(@apoteker, @kode_resep, @total)",
                        conn);
                cmd.Parameters.AddWithValue("apoteker", apoteker);
                cmd.Parameters.AddWithValue("kode_resep", kode_resep);
                cmd.Parameters.AddWithValue("total", total);

                if (cmd.ExecuteNonQuery() == 1) return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public List<ModelObat> GetDataObat()
        {
            OpenConnection();
            var dataObat = new List<ModelObat>();

            try
            {
                var cmd = new SqlCommand("Select * from tb_obat", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        dataObat.Add(new ModelObat(reader["kode_obat"].ToString(), reader["nama_obat"].ToString(),
                            reader["harga_jual"].ToString(), reader["harga_beli"].ToString(),
                            reader["harga_resep"].ToString(), reader["stok"].ToString(), reader["satuan"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dataObat;
        }

        public List<ModelAntrianApotik> GetDataAntrianApotik()
        {
            var antrian = new List<ModelAntrianApotik>();

            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select ta.*, tp.nama from tb_antrian ta left join tb_pasien tp on ta.no_rm = tp.no_rekam_medis where ta.tgl_berobat = convert(date, getdate(), 111) and tujuan_antrian='Apotik' and  status='Antri' order by  no_urut asc",
                    conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        antrian.Add(new ModelAntrianApotik(reader["id"].ToString(), reader["no_rm"].ToString(),
                            reader["no_resep"].ToString(),
                            reader["no_urut"].ToString(), reader["status"].ToString(),
                            reader["tgl_berobat"].ToString(), reader["nama"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return antrian;
        }

        public string GetKodeResepByRm(string no_rm)
        {
            var kode_resep = "";
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select top 1 tb_antrian.no_resep from  tb_antrian where tb_antrian.no_rm=@no_rm and tb_antrian.status='Panggil' and tujuan_antrian='Apotik' order by 1 desc",
                    conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) kode_resep = reader["no_resep"].ToString();
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return kode_resep;
        }

        public string GetKodeResepByNoUrut()
        {
            var kode_resep = "";

            try
            {
                OpenConnection();
                var command =
                    new SqlCommand(
                        "SELECT TOP 1 no_resep FROM tb_antrian WHERE tgl_berobat = CONVERT(date, GETDATE(), 111) AND status='Panggil' and tujuan_antrian='Apotik' ORDER BY no_urut DESC",
                        conn);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) kode_resep = reader["no_resep"].ToString();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return kode_resep;
        }

        public bool UpdateStatusAntrianApotik(string kode_resep)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "update tb_antrian set status='Selesai' where no_resep=@no_resep and tgl_berobat=convert(date, getdate(), 111)",
                    conn);
                cmd.Parameters.AddWithValue("no_resep", kode_resep);

                if (cmd.ExecuteNonQuery() == 1) return true;
                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public List<ModelResep> GetDataResep()
        {
            var resep = new List<ModelResep>();

            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select tb_resep.*, tb_dokter.nama as nama_dokter, tb_pasien.nama as nama_pasien from tb_resep left join tb_dokter on tb_resep.id_dokter = tb_dokter.id left join tb_pasien on tb_pasien.no_rekam_medis = tb_resep.no_rm where tgl_resep = convert(date , getdate(), 111)",
                    conn);
                //cmd.Parameters.AddWithValue("kode_resep", kode_resep);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        resep.Add(new ModelResep(reader["kode_resep"].ToString(), reader["no_rm"].ToString(),
                            reader["no_resep"].ToString(), reader["id_dokter"].ToString(),
                            reader["tgl_resep"].ToString(), reader["nama_dokter"].ToString(),
                            reader["nama_pasien"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return resep;
        }

        public List<ModelDetailResep> GetDataDetailResep(string kode_resep)
        {
            var detailResep = new List<ModelDetailResep>();

            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select tb_detail_resep.*, tb_obat.nama_obat as nama_obat, tb_obat.harga_resep as harga_obat, (jumlah * tb_obat.harga_resep) as subtotal from tb_detail_resep left join tb_obat on tb_detail_resep.kode_obat = tb_obat.kode_obat where no_resep=@no_resep and tgl_buat=convert(date, getdate(), 111)",
                    conn);
                cmd.Parameters.AddWithValue("no_resep", kode_resep);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        detailResep.Add(new ModelDetailResep(reader["id"].ToString(), reader["no_resep"].ToString(),
                            reader["kode_obat"].ToString(), reader["nama_obat"].ToString(), reader["dosis"].ToString(),
                            reader["penggunaan"].ToString(), reader["ket"].ToString(), reader["jumlah"].ToString(),
                            int.Parse(reader["subtotal"].ToString()),
                            int.Parse(reader["harga_obat"].ToString()), reader["tgl_buat"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return detailResep;
        }

        public bool UpdateDataObat(string kode_obat, string nama_obat, string satuan, string stok, string harga_jual,
            string harga_beli, string harga_resep)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "UPDATE [dbo].[tb_obat] SET [nama_obat] = @nama_obat ,[harga_beli] = @harga_beli ,[harga_jual] = @harga_jual ,[harga_resep] = @harga_resep, [stok] = @stok, [satuan]=@satuan WHERE [kode_obat] = @kode_obat",
                    conn);
                cmd.Parameters.AddWithValue("nama_obat", nama_obat);
                cmd.Parameters.AddWithValue("stok", stok);
                cmd.Parameters.AddWithValue("harga_beli", harga_beli);
                cmd.Parameters.AddWithValue("harga_jual", harga_jual);
                cmd.Parameters.AddWithValue("harga_resep", harga_resep);
                cmd.Parameters.AddWithValue("kode_obat", kode_obat);
                cmd.Parameters.AddWithValue("satuan", satuan);
                var res = cmd.ExecuteNonQuery();

                if (res == 1)
                    return true;

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public List<ModelApoteker> GetDataApoteker()
        {
            var apoteker = new List<ModelApoteker>();
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("select * from tb_apoteker where  id=@id", conn);
                cmd.Parameters.AddWithValue("id", Settings.Default.KodeApoteker);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        apoteker.Add(new ModelApoteker(reader["id"].ToString(), reader["nama"].ToString(),
                            reader["telp"].ToString(),
                            reader["alamat"].ToString(), reader["jenis_kelamin"].ToString(),
                            reader["password"].ToString()));
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return apoteker;
        }

        public int CountAntrianApotik()
        {
            var res = 0;
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "select count(*) as total_antrian from tb_antrian where status='Panggil' and tgl_berobat=convert(date, getdate(), 111)",
                    conn);
                res = int.Parse(cmd.ExecuteScalar().ToString());

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public int GetCountDataObat(string kode_obat)
        {
            var res = 0;

            try
            {
                OpenConnection();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM tb_obat WHERE kode_obat=@kode_obat", conn);
                cmd.Parameters.AddWithValue("kode_obat", kode_obat);
                res = int.Parse(cmd.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public bool DeleteDataObat(string kode_obat)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand("delete from tb_obat where kode_obat=@kode_obat", conn);
                cmd.Parameters.AddWithValue("kode_obat", kode_obat);
                var res = cmd.ExecuteNonQuery();

                if (res == 1)
                    return true;

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