using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dokter.DBAccess;
using System.Data.SqlClient;

namespace dokter
{
    class Program
    {
        static void Main(string[] args)
        {
            DBCommand cmd = new DBCommand(DBConnection.dbConnection());
            
            if(cmd.InsertDataRekamMedis("4545", "Asma", "Debu", "54", "Sesak Napas", "Asma", "Obat", "DK001", "001"))
            {
                Console.WriteLine("Berhasil Insert");
                Console.ReadLine();
            }

            Console.ReadLine();
        }
    }
}
