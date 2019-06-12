using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dokter.models
{
    class ModelRekamMedis
    {
        public int id { get; set; }
        public string no_rm { get; set; }
        public string riwayat_penyakit { get; set; }
        public string alergi { get; set; }
        public int berat_badan { get; set; }
        public string keluhan { get; set; }
        public string diagnosa { get; set; }
        public string tindakan { get; set; }
        public string id_dokter { get; set; }
        public string nama_dokter { get; set; }
        public string poli { set; get; }
        public string nama_poli { get; set; }
        public string tgl_pemeriksaan { get; set; }
        public string nama_pasien { get; set; }

        public ModelRekamMedis() { }
        public ModelRekamMedis(int id, string no_rm, string riwayat_penyakit, string alergi, int berat_badan, string keluhan, string diagnosa, string tindakan, string id_dokter, string poli, string tgl_pemeriksaan, string nama_dokter, string nama_poli, string nama_pasien)
        {
            DateTime dt = DateTime.Parse(tgl_pemeriksaan);

            this.id = id;
            this.no_rm = no_rm;
            this.riwayat_penyakit = riwayat_penyakit;
            this.alergi = alergi;
            this.berat_badan = berat_badan;
            this.keluhan = keluhan;
            this.tindakan = tindakan;
            this.id_dokter = id_dokter;
            this.poli = poli;
            this.tgl_pemeriksaan = dt.ToString("dd MMM yyyy");
            this.nama_dokter = nama_dokter;
            this.nama_poli = nama_poli;
            this.nama_pasien = nama_pasien;
        }

        ~ModelRekamMedis() { }
    }
}
