namespace dokter.models
{
    public class ModelObat
    {
        public ModelObat()
        {
        }

        public ModelObat(string kode_obat, string nama_obat, int stok, string satuan, string harga_beli,
            string harga_jual, string harga_resep, string tgl_insert)
        {
            this.kode_obat = kode_obat;
            this.nama_obat = nama_obat;
            this.satuan = satuan;
            this.stok = stok;
            this.harga_beli = harga_beli;
            this.harga_jual = harga_jual;
            this.harga_resep = harga_resep;
            this.tgl_insert = tgl_insert;
        }

        public string kode_obat { get; set; }
        public string nama_obat { get; set; }
        public int stok { get; set; }
        public string satuan { get; set; }
        public string harga_beli { get; set; }
        public string harga_jual { get; set; }
        public string harga_resep { get; set; }
        public string tgl_insert { get; set; }

        ~ModelObat()
        {
        }
    }
}