using System;
using System.Data;
using System.Data.SqlClient;

namespace dokter.DBAccess
{
    internal class DBCommand
    {
        private readonly SqlConnection conn;

        public DBCommand(SqlConnection conn)
        {
            this.conn = conn;
        }

        private void OpenConnection()
        {
            if (conn.State.Equals(ConnectionState.Closed))
                conn.Open();
        }

        private void CloseConnection()
        {
            if (conn.State.Equals(ConnectionState.Open))
                conn.Close();
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

        public bool InsertDetailResep(string kode_resep, string kode_obat, int jumlah, string keterangan)
        {
            try
            {
                OpenConnection();
                var cmd = new SqlCommand(
                    "INSERT INTO [dbo].[detail_resep]([no_resep],[kode_obat],[jumlah],[keterangan]) VALUES(@no_resep,@kode_obat,@jumlah,@keterangan)",
                    conn);
                cmd.Parameters.AddWithValue("no_resep", kode_resep);
                cmd.Parameters.AddWithValue("kode_obat", kode_obat);
                cmd.Parameters.AddWithValue("jumlah", jumlah);
                cmd.Parameters.AddWithValue("keterangan", keterangan);
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