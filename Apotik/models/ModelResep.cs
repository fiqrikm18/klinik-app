﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apotik.models
{
    public class ModelResep : IDataErrorInfo
    {
        public string kode_resep { get; set; }
        public string no_rm { get; set; }
        public string no_resep { get; set; }
        public string id_dokter { get; set; }
        public string nama_dokter { get; set; }
        public string tgl_resep { get; set; }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = "";

                if (columnName == "kode_resep")
                {
                    if (string.IsNullOrEmpty(kode_resep))
                    {
                        result = "Kode resep tidak boleh kosong.";
                    }
                }

                return result;
            }
        }

        public ModelResep() { }

        public ModelResep(string kode_resep, string no_rm, string no_resep, string id_dokter, string tgl_resep, string nama_dokter)
        {
            this.kode_resep = kode_resep;
            this.no_rm = no_rm;
            this.no_resep = no_resep;
            this.id_dokter = id_dokter;
            this.tgl_resep = tgl_resep;
            this.nama_dokter = nama_dokter;
        }

        ~ModelResep() { }
    }
}
