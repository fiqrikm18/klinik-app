using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace pendaftaran.models
{
    internal class MDaftarBaru : IDataErrorInfo
    {
        public MDaftarBaru(string norm, string identitas, string namapasien, string notelp, string alamat)
        {
            NoRm = norm;
            Identitas = identitas;
            NamaPasien = namapasien;
            NoTelp = notelp;
            Alamat = alamat;
        }

        public string NoRm { get; set; }
        public string Identitas { get; set; }
        public string NamaPasien { get; set; }
        public string NoTelp { get; set; }
        public string Alamat { get; set; }
        public string TanggalLahir { get; set; }
        public string JenisKelamin { get; set; }
        public string Poliklinik { get; set; }

        #region IDataErrorInfo members

        string IDataErrorInfo.Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "NoRm")
                    if (string.IsNullOrEmpty(NoRm))
                        result = "No rekam medis harus di isi.";

                //if (Regex.IsMatch(NoRm, "^[0-9A-Za-z ]+$"))
                //    result = "No rekam medis harus berupa huruf.";

                if (columnName == "Identitas")
                {
                    if (string.IsNullOrEmpty(Identitas))
                        result = "No identitas pasien harus di isi.";

                    //if (!Identitas.All(char.IsDigit))
                    //    result = "No identitas harus berupa angka.";

                    if (Regex.IsMatch(Identitas, "^[A-Za-z]+$"))
                        result = "No identitas pasien harus berupa angka.";
                }

                if (columnName == "NamaPasien")
                {
                    if (string.IsNullOrEmpty(NamaPasien))
                        result = "Nama pasien harus di isi.";

                    if (!Regex.IsMatch(NamaPasien, "^[A-Za-z ]+$"))
                        result = "Nama harus berupa huruf.";

                    if (NamaPasien.All(char.IsDigit))
                        result = "Nama harus berupa huruf.";
                }

                if (columnName == "NoTelp")
                {
                    if (string.IsNullOrEmpty(NoTelp))
                        result = "Nomor telepon harus di isi.";

                    //if (!NoTelp.All(char.IsDigit))
                    //    result = "No telepon harus berupa angka.";

                    if (Regex.IsMatch(NoTelp, "^[A-Za-z]+$"))
                        result = "Nomor telepon harus berupa angka.";
                }

                if (columnName == "Alamat")
                    if (string.IsNullOrEmpty(Alamat))
                        result = "Alamat pasien harus di isi.";

                return result;
            }
        }

        #endregion
    }
}