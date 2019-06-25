using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dokter.models
{
    public class ModelDetailResep : IDataErrorInfo
    {
        public string id { get; set; }
        public string no_resep { get; set; }
        public string nama_obat { get; set; }
        public string kode_obat { get; set; }
        public string dosis { get; set; }
        public string ket { get; set; }
        public string jumlah { get; set; }
        public string tgl_buat { get; set; }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = "";
                // no_resep, kode_obat, nama_obat, dosis, ket, jumlah

                if (columnName == "no_resep")
                {
                    if (string.IsNullOrEmpty(no_resep))
                    {
                        result = "Kode resep harus diisi.";
                    }
                }

                if(columnName == "kode_obat")
                {
                    if (string.IsNullOrEmpty(kode_obat))
                    {
                        result = "Kode obat harus diisi.";
                    }
                }

                if(columnName == "nama_obat")
                {
                    if (string.IsNullOrEmpty(nama_obat))
                    {
                        result = "Nama obat harus diisi.";
                    }
                }

                if(columnName == "dosis")
                {
                    if (string.IsNullOrEmpty(dosis))
                    {
                        result = "Pemakaian obat harus diisi.";
                    }
                }

                if(columnName == "jumlah")
                {
                    if (string.IsNullOrEmpty(jumlah))
                    {
                        result = "Jumlah obat harus diisi.";
                    }

                    if(Regex.IsMatch(jumlah, "^[A-Za-z]+$"))
                    {
                        result = "Jumlah obat harus berupa angka.";
                    }
                }

                return result;
            }
        }

        public ModelDetailResep() { }

        public ModelDetailResep(string id, string no_resep, string kode_obat, string dosis, string ket, string jumlah, string tgl_buat)
        {
            this.id = id;
            this.no_resep = no_resep;
            this.kode_obat = kode_obat;
            this.dosis = dosis;
            this.ket = ket;
            this.jumlah = jumlah;
            this.tgl_buat = tgl_buat;
        }

        public ModelDetailResep(string no_resep, string kode_obat, string nama_obat, string dosis, string ket, string jumlah)
        {
            this.no_resep = no_resep;
            this.kode_obat = kode_obat;
            this.dosis = dosis;
            this.ket = ket;
            this.jumlah = jumlah;
            this.nama_obat = nama_obat;
        }

        ~ModelDetailResep() { }
    }
}
