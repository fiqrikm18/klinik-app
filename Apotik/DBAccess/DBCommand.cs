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

        public bool InsertDataObat(string kode_obat, string nama_obat, string satuan, string stok ,string harga_jual, string harga_beli, string harga_resep)
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
            catch(SqlException ex)
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
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return dataObat;
        }

        public bool UpdateDataObat(string kode_obat, string nama_obat, string satuan, string stok ,string harga_jual, string harga_beli, string harga_resep)
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
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
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
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return res;
        }

        public bool HapusDataObat(string kode_obat)
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
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }
    }
}
