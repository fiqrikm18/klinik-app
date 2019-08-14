using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Apotik.models
{
    internal class ModelObat : IDataErrorInfo
    {
        #region constructor

        public ModelObat(string kode_obat, string nama_obat, string harga_jual, string harga_beli, string harga_resep,
            string stok, string satuan)
        {
            this.kode_obat = kode_obat;
            this.nama_obat = nama_obat;
            this.harga_beli = harga_beli;
            this.harga_jual = harga_jual;
            this.harga_resep = harga_resep;
            this.satuan = satuan;
            this.stok = stok;
        }

        #endregion

        public string kode_obat { get; set; }
        public string nama_obat { get; set; }
        public string harga_jual { get; set; }
        public string harga_beli { get; set; }
        public string harga_resep { get; set; }
        public string stok { get; set; }
        public string satuan { get; set; }

        #region IDataErrorInfo

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                var result = "";
                if (columnName == "kode_obat")
                    if (string.IsNullOrEmpty(kode_obat))
                        result = "Kode obat harus di isi.";

                if (columnName == "nama_obat")
                    if (string.IsNullOrEmpty(nama_obat))
                        result = "Nama obat harus di isi.";

                if (columnName == "harga_beli")
                {
                    if (string.IsNullOrEmpty(harga_beli))
                        result = "Harga obat harus di isi.";

                    if (Regex.IsMatch(harga_beli, "^[A-Za-z]+$"))
                        result = "Harga harus berupa angka.";
                }

                if (columnName == "harga_jual")
                {
                    if (string.IsNullOrEmpty(harga_jual))
                        result = "Harga obat harus di isi.";

                    if (Regex.IsMatch(harga_jual, "^[A-Za-z]+$"))
                        result = "Harga harus berupa angka.";
                }

                if (columnName == "harga_resep")
                {
                    if (string.IsNullOrEmpty(harga_resep))
                        result = "Harga obat harus di isi.";

                    if (Regex.IsMatch(harga_resep, "^[A-Za-z]+$"))
                        result = "Harga harus berupa angka.";
                }

                if (columnName == "stok")
                {
                    if (string.IsNullOrEmpty(stok))
                        result = "Stok obat harus di isi.";

                    if (Regex.IsMatch(stok, "^[A-Za-z]+$"))
                        result = "Stok harus berupa angka.";
                }

                return result;
            }
        }

        #endregion
    }
}