﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace admin.models
{
    public class Keuangan : IDataErrorInfo
    {
        public string id { get; set; }
        public string nama { get; set; }
        public string telp { get; set; }
        public string jenis_kelamin { get; set; }
        public string alamat { get; set; }
        public string password { get; set; }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string result = "";

                if (columnName == "id")
                    if (string.IsNullOrEmpty(id.ToString()))
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

                if (columnName == "password")
                    if (string.IsNullOrEmpty(password))
                        result = "Password harus di isi.";

                return result;
            }
        }

        public Keuangan() { }
        public Keuangan(string id, string nama, string telp, string jenis_kelamin, string password, string alamat)
        {
            this.id = id;
            this.nama = nama;
            this.telp = telp;
            this.jenis_kelamin = jenis_kelamin;
            this.password = password;
            this.alamat = alamat;
        }
    }
}
