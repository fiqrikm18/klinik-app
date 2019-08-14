namespace pendaftaran.models
{
    internal class ModelDetailPasien
    {
        public ModelDetailPasien(string id, string no_rm, string nama, string tgl_lahir, string jenis_kelamin,
            string no_telp,
            string alamat, string tgl_daftar, string golongan_darah)
        {
            this.id = id;
            this.no_rm = no_rm;
            this.nama = nama;
            this.tgl_lahir = tgl_lahir;
            this.jenis_kelamin = jenis_kelamin;
            this.no_telp = no_telp;
            this.alamat = alamat;
            this.tgl_daftar = tgl_daftar;
            this.golongan_darah = golongan_darah;
        }

        public string id { get; set; }
        public string no_rm { get; set; }
        public string nama { get; set; }
        public string tgl_lahir { get; set; }
        public string jenis_kelamin { get; set; }
        public string no_telp { get; set; }
        public string alamat { get; set; }
        public string tgl_daftar { get; set; }
        public string golongan_darah { get; set; }

        ~ModelDetailPasien()
        {
        }
    }
}