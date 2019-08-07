using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antrian_3_poli.Models
{
    public class ModelAntrianPoli
    {
        public ModelAntrianPoli(string id, string no_rm, string nama, int no_urut, string poliklinik, string status,
            string tgl_berobat)
        {
            this.id = id;
            this.no_rm = no_rm;
            this.nama = nama;
            this.no_urut = no_urut;
            this.poliklinik = poliklinik;
            this.status = status;
            this.tgl_berobat = tgl_berobat;
        }

        public string id { get; set; }
        public string no_rm { get; set; }
        public string nama { get; set; }
        public int no_urut { get; set; }
        public string poliklinik { get; set; }
        public string status { get; set; }
        public string tgl_berobat { get; set; }

        ~ModelAntrianPoli()
        {
        }
    }
}
