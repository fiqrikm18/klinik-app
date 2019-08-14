namespace Antrian.models
{
    public class ModelAntrianApotik
    {
        public ModelAntrianApotik(int id, string no_rm, string no_urut, string tujuan_antrian, string no_resep,
            string status, string tgl_berobat, string nama)
        {
            this.id = id;
            this.no_rm = no_rm;
            this.no_urut = no_urut;
            this.tujuan_antrian = tujuan_antrian;
            this.no_resep = no_resep;
            //this.poliklinik = poliklinik;
            this.status = status;
            this.tgl_berobat = tgl_berobat;
            this.nama = nama.ToUpperInvariant();
        }

        public int id { get; set; }
        public string no_rm { get; set; }
        public string no_urut { get; set; }
        public string tujuan_antrian { get; set; }
        public string poliklinik { get; set; }
        public string no_resep { get; set; }
        public string status { get; set; }
        public string tgl_berobat { get; set; }
        public string nama { get; set; }
    }
}