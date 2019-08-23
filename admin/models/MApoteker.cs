using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace admin.models
{
    public class MApoteker : IDataErrorInfo
    {
        public MApoteker(string id, string nama, string no_telp, string alamat, string password)
        {
            id_apoteker = id;
            nama_apoteker = nama;
            this.alamat = alamat;
            this.no_telp = no_telp;
            this.password = password;
        }

        public MApoteker(string id, string nama, string no_telp, string alamat, string password, string jenis_kelamin)
        {
            id_apoteker = id;
            nama_apoteker = new CultureInfo("en-US", false).TextInfo.ToTitleCase(nama);
            this.alamat = alamat;
            this.no_telp = no_telp;
            this.password = password;
            this.jenis_kelamin = jenis_kelamin;
        }

        public string id_apoteker { get; set; }
        public string nama_apoteker { get; set; }
        public string no_telp { get; set; }
        public string alamat { get; set; }
        public string password { get; set; }
        public string jenis_kelamin { get; set; }


        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "id_apoteker")
                    if (string.IsNullOrEmpty(id_apoteker))
                        result = "Id/kode harus diisi.";

                if (columnName == "nama_apoteker")
                {
                    if (string.IsNullOrEmpty(nama_apoteker))
                        result = "Nama harus di isi.";

                    if (!Regex.IsMatch(nama_apoteker, "^[A-Za-z ]+$"))
                        result = "Nama harus berupa huruf.";

                    if (nama_apoteker.All(char.IsDigit))
                        result = "Nama harus berupa huruf.";
                }

                if (columnName == "no_telp")
                {
                    if (string.IsNullOrEmpty(no_telp))
                        result = "Nomor telepon harus di isi.";

                    if (Regex.IsMatch(no_telp, "^[A-Za-z]+$"))
                        result = "Nomor telepon harus berupa angka.";
                }

                if (columnName == "alamat")
                    if (string.IsNullOrEmpty(alamat))
                        result = "Alamat harus di isi.";

                if (columnName == "password")
                    if (string.IsNullOrEmpty(password))
                        result = "Password harus di isi.";

                return result;
            }
        }

        public string Error { get; }
    }
}