using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dokter.models
{
    class ModelDiagnosis
    {
        public int id { get; set; }
        public string kode { get; set; }
        public string desk { get; set; }

        public ModelDiagnosis() { }
        public ModelDiagnosis(int id, string kode, string desk)
        {
            this.id = id;
            this.kode = kode;
            this.desk = desk;
        }
    }
}
