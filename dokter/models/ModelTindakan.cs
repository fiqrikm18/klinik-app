namespace dokter.models
{
    internal class ModelTindakan
    {
        public ModelTindakan()
        {
        }

        public ModelTindakan(int id, string kode, string desk)
        {
            this.id = id;
            this.kode = kode;
            this.desk = desk;
        }

        public int id { get; set; }
        public string kode { get; set; }
        public string desk { get; set; }
    }
}