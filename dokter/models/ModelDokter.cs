using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace dokter.models
{
    public class ModelDokter : IDataErrorInfo
    {
        public ModelDokter(string id, string nama, string telp, string spesialisasi, string alamat, string password)
        {
            this.id = id;
            this.nama = nama;
            this.alamat = alamat;
            this.telp = telp;
            this.spesialisasi = spesialisasi;
            this.password = password;
        }

        public ModelDokter(string id, string nama, string telp, string spesialisasi, string alamat, string password,
            string poliklinik, string jenis_kelamin)
        {
            this.id = id;
            this.nama = nama;
            this.alamat = alamat;
            this.telp = telp;
            this.spesialisasi = spesialisasi;
            this.jenis_kelamin = jenis_kelamin;
            this.password = password;
            this.poliklinik = poliklinik;
        }

        public string id { get; set; }
        public string nama { get; set; }
        public string telp { get; set; }
        public string alamat { get; set; }
        public string spesialisasi { get; set; }
        public string tugas { get; set; }
        public string jenis_kelamin { get; set; }
        public string password { get; set; }
        public string poliklinik { get; set; }

        ~ModelDokter()
        {
        }

        #region member IDataErrorInfo

        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "id")
                    if (string.IsNullOrEmpty(id))
                        result = "Id/kode dokter harus diisi.";

                if (columnName == "nama")
                {
                    if (string.IsNullOrEmpty(nama))
                        result = "Nama dokter harus di isi.";

                    if (!Regex.IsMatch(nama, "^[A-Za-z ]+$"))
                        result = "Nama harus berupa huruf.";

                    if (nama.All(char.IsDigit))
                        result = "Nama harus berupa huruf.";
                }

                if (columnName == "telp")
                {
                    if (string.IsNullOrEmpty(telp))
                        result = "Nomor telepon harus di isi.";

                    if (Regex.IsMatch(telp, "^[A-Za-z]+$"))
                        result = "Nomor telepon harus berupa angka.";
                }

                if (columnName == "alamat")
                    if (string.IsNullOrEmpty(alamat))
                        result = "Alamat harus di isi.";

                if (columnName == "spesialisasi")
                    if (string.IsNullOrEmpty(spesialisasi))
                        result = "spesialisasi harus di isi.";

                if (columnName == "password")
                    if (string.IsNullOrEmpty(password))
                        result = "Password harus di isi.";

                return result;
            }
        }

        public string Error { get; }

        #endregion
    }
}