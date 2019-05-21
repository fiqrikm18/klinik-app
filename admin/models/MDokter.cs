using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace admin.models
{
    class MDokter : IDataErrorInfo
    {
        public string id { get; set; }
        public string nama { get; set; }
        public string telp { get; set; }
        public string alamat { get; set; }
        public string spesialisasi { get; set; }
        public string tugas { get; set; }
        public string jenis_kelamin { get; set; }

        public MDokter(string id, string nama, string telp, string spesialisasi, string alamat)
        {
            this.id = id;
            this.nama = nama;
            this.alamat = alamat;
            this.telp = telp;
            this.spesialisasi = spesialisasi;
        }

        #region member IDataErrorInfo
        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "id")
                {
                    if (string.IsNullOrEmpty(id))
                        result = "Id/kode dokter harus diisi.";
                }

                if (columnName == "nama")
                {
                    if(string.IsNullOrEmpty(nama))
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
                {
                    if (string.IsNullOrEmpty(alamat))
                        result = "Alamat harus di isi.";
                }

                if (columnName == "spesialisasi")
                {
                    if (string.IsNullOrEmpty(spesialisasi))
                        result = "spesialisasi harus di isi.";
                }

                return result;
            }
        }

        public string Error { get; }
        #endregion
    }
}
