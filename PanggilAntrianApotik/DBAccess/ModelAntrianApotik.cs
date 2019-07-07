using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanggilAntrianApotik.DBAccess
{
    public class ModelAntrianApotik
    {
        public ModelAntrianApotik()
        {
        }

        public ModelAntrianApotik(string id, string no_rm, string no_resep, string no_antrian, string status,
            string tgl_resep, string nama)
        {
            this.id = id;
            this.no_rm = no_rm;
            this.no_resep = no_resep;
            this.no_antrian = no_antrian;
            this.status = status;
            this.tgl_resep = tgl_resep;
            this.nama = nama;
        }

        public string id { get; set; }
        public string no_rm { get; set; }
        public string no_resep { get; set; }
        public string no_antrian { get; set; }
        public string status { get; set; }
        public string tgl_resep { get; set; }
        public string nama { get; set; }

        ~ModelAntrianApotik()
        {
        }
    }
}