namespace pendaftaran.models
{
    public class ModelPasien
    {
        public ModelPasien(string no_identitas, string no_rekam_medis, string nama, string tanggal_lahir,
            string jenis_kelamin, string no_telp, string alamat, string tgl_daftar, string golongan_darah,
            string jenis_kartu)
        {
            this.no_identitas = no_identitas;
            jenis_id = jenis_kartu;
            this.no_rekam_medis = no_rekam_medis;
            this.nama = nama;
            this.tanggal_lahir = tanggal_lahir;
            this.jenis_kelamin = jenis_kelamin;
            this.no_telp = no_telp;
            this.alamat = alamat;
            this.tgl_daftar = tgl_daftar;
            this.golongan_darah = golongan_darah;
        }

        public string no_identitas { get; set; }
        public string jenis_id { get; set; }
        public string no_rekam_medis { get; set; }
        public string nama { get; set; }
        public string tanggal_lahir { get; set; }
        public string jenis_kelamin { get; set; }
        public string no_telp { get; set; }
        public string alamat { get; set; }
        public string tgl_daftar { get; set; }
        public string golongan_darah { get; set; }
    }
}