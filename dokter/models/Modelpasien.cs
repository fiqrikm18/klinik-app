using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dokter.models
{
    public class ModelPasien
    {
        public string id { get; set; }
        public string no_rm { get; set; }
        public string nama { get; set; }
        public string tgl_lahir { get; set; }
        public string jenis_kelamin { get; set; }
        public string no_telp { get; set; }
        public string alamat { get; set; }
        public string tgl_daftar { get; set; }

        public ModelPasien(string id, string no_rm, string nama, string tgl_lahir, string jenis_kelamin, string no_telp, string alamat, string tgl_daftar)
        {
            this.id = id;
            this.no_rm = no_rm;
            this.nama = nama;
            this.tgl_lahir = tgl_lahir;
            this.jenis_kelamin = jenis_kelamin;
            this.no_telp = no_telp;
            this.alamat = alamat;
            this.tgl_daftar = tgl_daftar;
        }

        ~ModelPasien() { }
    }
}
