using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Apotik.models;

namespace Apotik.DBAccess
{
    class DBCommand
    {
        private SqlConnection conn;

        public DBCommand(SqlConnection conn)
        {
            this.conn = conn;
        }

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

        public bool InsertDataObat(string kode_obat, string nama_obat, string satuan, string stok, string harga_jual, string harga_beli, string harga_resep)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[tb_obat] ([kode_obat] ,[nama_obat] ,[stok] ,[satuan] ,[harga_beli] ,[harga_jual] ,[harga_resep]) VALUES (@kode_obat ,@nama_obat ,@stok ,@satuan ,@harga_beli ,@harga_jual ,@harga_resep)", conn);
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

        public bool Login(string id, string pass)
        {
            //var res = 0;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select count(*) from tb_apoteker where id=@id and password=@pass", conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("pass", pass);

                if (int.Parse(cmd.ExecuteScalar().ToString()) > 0)
                {
                    return true;
                }

                CloseConnection();
            }
            catch (Exception ex)
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
                SqlCommand cmd = new SqlCommand("Select * from tb_obat", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataObat.Add(new ModelObat(reader["kode_obat"].ToString(), reader["nama_obat"].ToString(), reader["harga_jual"].ToString(), reader["harga_beli"].ToString(), reader["harga_resep"].ToString(), reader["stok"].ToString(), reader["satuan"].ToString()));
                    }
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
            List<ModelAntrianApotik> antrian = new List<ModelAntrianApotik>();

            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select ta.*, tp.nama from tb_antrian_apotik ta left join tb_pasien tp on ta.no_rm = tp.no_rekam_medis where ta.tgl_resep = convert(date, getdate(), 111) and  status='Antri' order by  no_antrian asc", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        antrian.Add(new ModelAntrianApotik(reader["id"].ToString(), reader["no_rm"].ToString(), reader["no_resep"].ToString(),
                        reader["no_antrian"].ToString(), reader["status"].ToString(), reader["tgl_resep"].ToString(), reader["nama"].ToString()));
                    }
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
            string kode_resep = "";
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select top 1 kode_resep from tb_resep where no_rm=@no_rm order by 1 asc", conn);
                cmd.Parameters.AddWithValue("no_rm", no_rm);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kode_resep = reader["kode_resep"].ToString();
                    }
                }

                CloseConnection();
            }
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return kode_resep;
        }

        public string GetKodeResepByNoUrut()
        {
            string kode_resep = "";

            try
            {
                OpenConnection();
                SqlCommand command = new SqlCommand("SELECT TOP 1 no_resep FROM tb_antrian_apotik WHERE tgl_resep = CONVERT(date, GETDATE(), 111) AND status='Antri' ORDER BY no_antrian ASC", conn);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kode_resep = reader["no_resep"].ToString();
                    }
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
                SqlCommand cmd = new SqlCommand("update tb_antrian_apotik set status='Selesai' where no_resep=@no_resep and tgl_resep=convert(date, getdate(), 111)", conn);
                cmd.Parameters.AddWithValue("no_resep", kode_resep);

                if(cmd.ExecuteNonQuery() ==1)
                {
                    return true;
                }
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
            List<ModelResep> resep = new List<ModelResep>();

            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select tb_resep.*, tb_dokter.nama as nama_dokter, tb_pasien.nama as nama_pasien from tb_resep left join tb_dokter on tb_resep.id_dokter = tb_dokter.id left join tb_pasien on tb_pasien.no_rekam_medis = tb_resep.no_rm where tgl_resep = convert(date , getdate(), 111)", conn);
                //cmd.Parameters.AddWithValue("kode_resep", kode_resep);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resep.Add(new ModelResep(reader["kode_resep"].ToString(), reader["no_rm"].ToString(), reader["no_resep"].ToString(), reader["id_dokter"].ToString(), reader["tgl_resep"].ToString(), reader["nama_dokter"].ToString(), reader["nama_pasien"].ToString()));
                    }
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
            List<ModelDetailResep> detailResep = new List<ModelDetailResep>();

            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select tb_detail_resep.*, tb_obat.nama_obat as nama_obat, tb_obat.harga_resep as harga_obat, (jumlah * tb_obat.harga_resep) as subtotal from tb_detail_resep left join tb_obat on tb_detail_resep.kode_obat = tb_obat.kode_obat where no_resep=@no_resep and tgl_buat=convert(date, getdate(), 111)", conn);
                cmd.Parameters.AddWithValue("no_resep", kode_resep);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        detailResep.Add(new ModelDetailResep(reader["id"].ToString(), reader["no_resep"].ToString(), reader["kode_obat"].ToString(),
                            reader["nama_obat"].ToString(), reader["penggunaan"].ToString(), reader["ket"].ToString(),
                            reader["jumlah"].ToString(), int.Parse(reader["subtotal"].ToString()), int.Parse(reader["harga_obat"].ToString()), reader["tgl_buat"].ToString()));
                    }
                }

                CloseConnection();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return detailResep;
        }

        public bool UpdateDataObat(string kode_obat, string nama_obat, string satuan, string stok, string harga_jual, string harga_beli, string harga_resep)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[tb_obat] SET [nama_obat] = @nama_obat ,[harga_beli] = @harga_beli ,[harga_jual] = @harga_jual ,[harga_resep] = @harga_resep, [stok] = @stok, [satuan]=@satuan WHERE [kode_obat] = @kode_obat", conn);
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
            List<ModelApoteker> apoteker = new List<ModelApoteker>();
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select * from tb_apoteker where  id=@id", conn);
                cmd.Parameters.AddWithValue("id", Properties.Settings.Default.KodeApoteker.ToString());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        apoteker.Add(new ModelApoteker(reader["id"].ToString(), reader["nama"].ToString(), reader["telp"].ToString(),
                            reader["alamat"].ToString(), reader["jenis_kelamin"].ToString(), reader["password"].ToString()));
                    }
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
            int res = 0;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("select count(*) as total_antrian from tb_antrian_apotik where status='Antri' and tgl_resep=convert(date, getdate(), 111)", conn);
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
            int res = 0;

            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tb_obat WHERE kode_obat=@kode_obat", conn);
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
                SqlCommand cmd = new SqlCommand("delete from tb_obat where kode_obat=@kode_obat", conn);
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
