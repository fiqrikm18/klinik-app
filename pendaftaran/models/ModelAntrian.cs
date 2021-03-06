﻿namespace pendaftaran.models
{
    public class ModelAntrian
    {
        public ModelAntrian(string id, string no_rm, string nama, int no_urut, string poliklinik, string nama_poli,
            string status, string tgl_berobat)
        {
            this.id = id;
            this.no_rm = no_rm;
            this.nama = nama;
            this.no_urut = no_urut;
            this.poliklinik = poliklinik;
            this.nama_poli = nama_poli;
            this.status = status;
            this.tgl_berobat = tgl_berobat;
        }

        public string id { get; set; }
        public string no_rm { get; set; }
        public string nama { get; set; }
        public int no_urut { get; set; }
        public string poliklinik { get; set; }
        public string nama_poli { get; set; }
        public string status { get; set; }
        public string tgl_berobat { get; set; }

        ~ModelAntrian()
        {
        }
    }
}