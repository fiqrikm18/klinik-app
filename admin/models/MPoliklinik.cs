using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace admin.models
{
    public class MPoliklinik : IDataErrorInfo
    {
        public MPoliklinik(string kode, string nama)
        {
            nama_poliklinik = nama;
            kode_poliklinik = kode;
        }

        public string kode_poliklinik { get; set; }
        public string nama_poliklinik { get; set; }

        public string this[string columnName]
        {
            get
            {
                var result = "";

                if (columnName == "kode_poliklinik")
                    if (string.IsNullOrEmpty(kode_poliklinik))
                        result = "Kode poliklinik harus diisi.";

                if (columnName == "nama_poliklinik")
                {
                    if (string.IsNullOrEmpty(nama_poliklinik))
                        result = "Nama harus di isi.";

                    if (!Regex.IsMatch(nama_poliklinik, "^[A-Za-z ]+$"))
                        result = "Nama harus berupa huruf.";

                    if (nama_poliklinik.All(char.IsDigit))
                        result = "Nama harus berupa huruf.";
                }

                return result;
            }
        }

        public string Error { get; }
    }
}