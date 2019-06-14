namespace admin.models
{
    public class ComboboxPairs
    {
        public ComboboxPairs(string KodePoli, string NamaPoli)
        {
            kode_poliklinik = KodePoli;
            nama_poliklinik = NamaPoli;
        }

        public string kode_poliklinik { get; set; }
        public string nama_poliklinik { get; set; }
    }
}