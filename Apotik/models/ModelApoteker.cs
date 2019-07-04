namespace Apotik.models
{
    public class ModelApoteker
    {
        public ModelApoteker(string id, string nama, string telp, string alamat, string jenisKelamin, string password)
        {
            this.id = id;
            this.nama = nama;
            this.telp = telp;
            this.alamat = alamat;
            this.jenisKelamin = jenisKelamin;
            this.password = password;
        }

        public string id { get; set; }
        public string nama { get; set; }
        public string telp { get; set; }
        public string alamat { get; set; }
        public string jenisKelamin { get; set; }
        public string password { get; set; }

        ~ModelApoteker()
        {
        }
    }
}