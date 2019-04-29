using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pendaftaran.models
{
    class ComboboxPairs
    {
        public string kode_poliklinik { get; set; }
        public string nama_poliklinik { get; set; }

        public ComboboxPairs(string KodePoli, string NamaPoli)
        {
            kode_poliklinik = KodePoli;
            nama_poliklinik = NamaPoli;
        }
    }
}
