﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using pendaftaran.models;

namespace pendaftaran.DBAccess
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

        public int CountIdPasienExists(string identitas)
        {
            int res = 0;

            try
            {
                OpenConnection();

                SqlCommand cmd = new SqlCommand("select count(no_identitas) from tb_pasien where no_identitas=@no_identitas", conn);
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
            int res = 0;

            try
            {
                OpenConnection();

                SqlCommand cmd = new SqlCommand("select count(no_rekam_medis) from tb_pasien where no_rekam_medis=@no_rekam_medis", conn);
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

        public bool InsertDataPasien(string no_id, string no_rm, string nama, string tgl_lahir, string jenis_kelamin, string no_telp, string alamat, string golongan_darah)
        {
            try
            {
                OpenConnection();

                SqlCommand cmd = new SqlCommand("insert into tb_pasien (no_identitas, no_rekam_medis, nama, tanggal_lahir, jenis_kelamin, no_telp, alamat, golongan_darah) values(@no_identitas, @no_rekam_medis, @nama, @tanggal_lahir, @jenis_kelamin, @no_telp, @alamat, @golongan_darah)", conn);
                //@no_identitas, @no_rekam_medis, @nama, @tanggal_lahir, @jenis_kelamin, @no_telp, @alamat
                cmd.Parameters.AddWithValue("no_identitas", no_id);
                cmd.Parameters.AddWithValue("no_rekam_medis", no_rm);
                cmd.Parameters.AddWithValue("nama", nama);
                cmd.Parameters.AddWithValue("tanggal_lahir", tgl_lahir);
                cmd.Parameters.AddWithValue("jenis_kelamin", jenis_kelamin);
                cmd.Parameters.AddWithValue("no_telp", no_telp);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("golongan_darah", golongan_darah);
                var res = cmd.ExecuteNonQuery();

                CloseConnection();
                if (res == 1)
                {
                    return true;
                }
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
                SqlCommand cmd = new SqlCommand("insert into tb_antrian_poli(no_rm, no_urut, poliklinik, status) values(@no_rm, @no_urut, @poliklinik, @status)", conn);
                //@no_rm, @no_urut, @poliklinik, @status
                cmd.Parameters.AddWithValue("no_rm", no_rm);
                cmd.Parameters.AddWithValue("no_urut", no_urut);
                cmd.Parameters.AddWithValue("poliklinik", poliklinik);
                cmd.Parameters.AddWithValue("status", status);
                var res = cmd.ExecuteNonQuery();

                CloseConnection();
                if(res == 1)
                {
                    return true;
                }
            }
            catch(Exception ex)
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
                SqlCommand cmd = new SqlCommand("select top 1 no_urut from tb_antrian_poli where poliklinik = @policode and tgl_berobat = convert(varchar(10), getdate(), 111) order by 1 desc", conn);
                cmd.Parameters.AddWithValue("policode", policode);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res = reader.GetInt32(0);
                    }
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
            List<ModelAntrian> antrian = new List<ModelAntrian>();
            OpenConnection();

            try
            {
                OpenConnection();

                if (string.IsNullOrEmpty(tgl_berobat))
                    tgl_berobat = DateTime.Now.ToString("yyyy-MM-dd");

                SqlCommand cmd = new SqlCommand("SELECT tb_pasien.nama as nama, tb_poliklinik.nama_poli as poli, tb_antrian_poli.* FROM tb_antrian_poli LEFT JOIN [tb_pasien] ON tb_antrian_poli.no_rm = tb_pasien.no_rekam_medis left join tb_poliklinik on tb_poliklinik.kode_poli = tb_antrian_poli.poliklinik where tgl_berobat=@tgl_berobat order by poliklinik", conn);
                cmd.Parameters.AddWithValue("tgl_berobat", tgl_berobat);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        antrian.Add(new ModelAntrian(reader["id"].ToString(), reader["no_rm"].ToString(), reader["nama"].ToString(), 
                            int.Parse(reader["no_urut"].ToString()), reader["poliklinik"].ToString(), reader["poli"].ToString(), 
                            reader["status"].ToString(), reader["tgl_berobat"].ToString()));
                    }
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
            List<ComboboxPairs> cbp = new List<ComboboxPairs>();
            cbp.Add(new ComboboxPairs("Pilih", "Pilih"));

            try
            {
                OpenConnection();

                SqlCommand cmd = new SqlCommand("select * from tb_poliklinik", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cbp.Add(new ComboboxPairs(reader["nama_poli"].ToString(), reader["kode_poli"].ToString()));
                    }
                }
                CloseConnection();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return cbp;
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
                        pasien.Add(new ModelPasien(reader["no_identitas"].ToString(), reader["no_rekam_medis"].ToString(), reader["nama"].ToString(), DateTime.Parse(reader["tanggal_lahir"].ToString()).ToString("dd MMM yyyy"), reader["jenis_kelamin"].ToString(), reader["no_telp"].ToString(), reader["alamat"].ToString(), reader["tgl_daftar"].ToString(), reader["golongan_darah"].ToString()));
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

        public bool UpdateDataPasien(string nama, string no_telp, string jenis_kelamin, string alamat, string id, string gol_darah)
        {
            try
            {
                OpenConnection();

                SqlCommand cmd = new SqlCommand("update tb_pasien set nama=@namaPasien, jenis_kelamin=@jenisKelamin, no_telp=@noTelp, alamat=@alamat, golongan_darah=@golongan_darah where no_identitas=@identitas", conn);
                cmd.Parameters.AddWithValue("golongan_darah", gol_darah);
                cmd.Parameters.AddWithValue("namaPasien", nama);
                cmd.Parameters.AddWithValue("jenisKelamin", jenis_kelamin);
                cmd.Parameters.AddWithValue("noTelp", no_telp);
                cmd.Parameters.AddWithValue("alamat", alamat);
                cmd.Parameters.AddWithValue("identitas", id);

                if (cmd.ExecuteNonQuery() == 1)
                    return true;

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
                SqlCommand cmd = new SqlCommand("delete from tb_pasien where no_rekam_medis=@no_rekam_medis", conn);
                cmd.Parameters.AddWithValue("no_rekam_medis", no_rm);

                if(cmd.ExecuteNonQuery() == 1)
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

        /// <summary>
        /// TODO: Add function delete antrian with spesific parameters
        /// </summary>
    }
}
