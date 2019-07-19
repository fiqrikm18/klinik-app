﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace admin.models
{
    public class ModelTransaksi
    {
        public int id { get; set; }
        public string id_apoteker { get; set; }
        public string nama_apoteker { get; set; }
        public string kode_resep { get; set; }
        public string total { get; set; }
        public string tgl_transaksi { get; set; }

        public ModelTransaksi() { }
        public ModelTransaksi(int id, string id_apoteker, string nama_apoteker, string kode_resep, int total, string tgl_transaksi)
        {
            DateTime dt = DateTime.Parse(tgl_transaksi);

            this.id = id;
            this.id_apoteker = id_apoteker;
            this.nama_apoteker = nama_apoteker;
            this.kode_resep = kode_resep;
            this.total = total.ToString("C", new CultureInfo("id-ID"));
            this.tgl_transaksi = dt.ToShortDateString();
        }
    }
}
