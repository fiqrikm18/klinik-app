using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Apotik.models
{
    public class ModelDetailResep : IDataErrorInfo
    {
        public ModelDetailResep()
        {
        }

        public ModelDetailResep(string id, string no_resep, string kode_obat, string dosis, string ket, string jumlah,
            string pemakaian,
            string tgl_buat)
        {
            this.id = id;
            this.no_resep = no_resep;
            this.kode_obat = kode_obat;
            this.dosis = dosis;
            this.ket = ket;
            this.jumlah = jumlah;
            this.tgl_buat = tgl_buat;
            this.pemakaian = pemakaian;
        }

        public ModelDetailResep(string id, string no_resep, string kode_obat, string nama_obat, string dosis,
            string pemakaian,
            string ket, string jumlah, int sub_total, int harga_obat, string tgl_buat)
        {
            this.id = id;
            this.no_resep = no_resep;
            this.kode_obat = kode_obat;
            this.dosis = dosis;
            this.ket = ket;
            this.jumlah = jumlah;
            this.nama_obat = nama_obat;
            this.sub_total = sub_total;
            this.harga_obat = harga_obat;
            this.tgl_buat = tgl_buat;
            this.pemakaian = pemakaian;
        }

        public string id { get; set; }
        public string no_resep { get; set; }
        public string nama_obat { get; set; }
        public string kode_obat { get; set; }
        public string dosis { get; set; }
        public string ket { get; set; }
        public string jumlah { get; set; }
        public string tgl_buat { get; set; }
        public int sub_total { get; set; }
        public int harga_obat { get; set; }
        public string pemakaian { get; set; }

        /*public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }*/

        public string Error { get; }

        public string this[string columnName]
        {
            get
            {
                var result = "";
                // no_resep, kode_obat, nama_obat, dosis, ket, jumlah

                if (columnName == "no_resep")
                    if (string.IsNullOrEmpty(no_resep))
                        result = "Kode resep harus diisi.";

                if (columnName == "kode_obat")
                    if (string.IsNullOrEmpty(kode_obat))
                        result = "Kode obat harus diisi.";

                if (columnName == "nama_obat")
                    if (string.IsNullOrEmpty(nama_obat))
                        result = "Nama obat harus diisi.";

                if (columnName == "dosis")
                    if (string.IsNullOrEmpty(dosis))
                        result = "Pemakaian obat harus diisi.";

                if (columnName == "jumlah")
                {
                    if (string.IsNullOrEmpty(jumlah)) result = "Jumlah obat harus diisi.";

                    if (Regex.IsMatch(jumlah, "^[A-Za-z]+$")) result = "Jumlah obat harus berupa angka.";
                }

                return result;
            }
        }

        ~ModelDetailResep()
        {
        }
    }
}