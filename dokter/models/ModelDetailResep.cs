using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dokter.models
{
    public class ModelDetailResep
    {
        public string id { get; set; }
        public string no_resep { get; set; }
        public string nama_obat { get; set; }
        public string kode_obat { get; set; }
        public string dosis { get; set; }
        public string ket { get; set; }
        public string jumlah { get; set; }
        public string tgl_buat { get; set; }

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
