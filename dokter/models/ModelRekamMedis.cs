using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace dokter.models
{
    internal class ModelRekamMedis : IDataErrorInfo
    {
        public ModelRekamMedis()
        {
        }

        public ModelRekamMedis(string keluhan, string diagnosa, string tindakan)
        {
            this.keluhan = keluhan;
            this.tindakan = tindakan;
            this.diagnosa = diagnosa;
        }

        public ModelRekamMedis(int id, string no_rm, string riwayat_penyakit, string alergi, string berat_badan,
            string keluhan, string diagnosa, string tindakan, string id_dokter, string poli, string tgl_pemeriksaan,
            string nama_dokter, string nama_poli, string nama_pasien)
        {
            var dt = DateTime.Parse(tgl_pemeriksaan);

            this.id = id;
            this.no_rm = no_rm;
            this.riwayat_penyakit = riwayat_penyakit;
            this.alergi = alergi;
            this.berat_badan = berat_badan;
            this.keluhan = keluhan;
            this.tindakan = tindakan;
            this.diagnosa = diagnosa;
            this.id_dokter = id_dokter;
            this.poli = poli;
            this.tgl_pemeriksaan = dt.ToString("dd MMM yyyy");
            this.nama_dokter = nama_dokter;
            this.nama_poli = nama_poli;
            this.nama_pasien = nama_pasien;
        }

        public ModelRekamMedis(string no_rm, string riwayat_penyakit, string alergi, string berat_badan,
            string keluhan, string diagnosa, string tindakan, string id_dokter, string poli, string tgl_pemeriksaan,
            string nama_dokter, string nama_poli)
        {
            var dt = DateTime.Parse(tgl_pemeriksaan);

            this.no_rm = no_rm;
            this.riwayat_penyakit = riwayat_penyakit;
            this.alergi = alergi;
            this.berat_badan = berat_badan;
            this.keluhan = keluhan;
            this.tindakan = tindakan.Replace(";", "\n\n");
            this.diagnosa = diagnosa.Replace(";", "\n\n");
            this.id_dokter = id_dokter;
            this.poli = poli;
            this.tgl_pemeriksaan = dt.ToString("dd MMM yyyy");
            this.nama_dokter = nama_dokter;
            this.nama_poli = nama_poli;
        }

        public int id { get; set; }
        public string no_rm { get; set; }
        public string riwayat_penyakit { get; set; }
        public string alergi { get; set; }
        public string berat_badan { get; set; }
        public string keluhan { get; set; }
        public string diagnosa { get; set; }
        public string tindakan { get; set; }
        public string id_dokter { get; set; }
        public string nama_dokter { get; set; }
        public string poli { set; get; }
        public string nama_poli { get; set; }
        public string tgl_pemeriksaan { get; set; }
        public string nama_pasien { get; set; }

        public string Error => throw new NotImplementedException();

        #region IDataErrorInfo

        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "id")
                    if (string.IsNullOrEmpty(id.ToString()))
                        result = "Id harus diisi.";

                if (columnName == "no_rm")
                    if (string.IsNullOrEmpty(no_rm))
                        result = "Nomor rekam medis harus diisi.";

                if (columnName == "keluhan")
                    if (string.IsNullOrEmpty(keluhan))
                        result = "Keluhan harus diisi.";

                if (columnName == "diagnosa")
                    if (string.IsNullOrEmpty(diagnosa))
                        result = "Diagnosa harus diisi.";

                if (columnName == "tindakan")
                    if (string.IsNullOrEmpty(tindakan))
                        result = "Tindakan harus diisi.";

                if (columnName == "berat_badan")
                    if (Regex.IsMatch(berat_badan, "^[A-Za-z]+$"))
                        result = "Berat badan harus berupa angka.";

                if (columnName == "riwayat_penyakit")
                    if (!Regex.IsMatch(riwayat_penyakit, "^[A-Za-z ]+$"))
                        result = "Riwayat penyakit harus berupa huruf.";

                if (columnName == "alergi")
                    if (!Regex.IsMatch(alergi, "^[A-Za-z ]+$"))
                        result = "Alergi harus berupa huruf.";

                return result;
            }
        }

        #endregion

        ~ModelRekamMedis()
        {
        }
    }
}