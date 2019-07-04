using System;
using dokter.DBAccess;

namespace dokter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cmd = new DBCommand(DBConnection.dbConnection());

            if (cmd.InsertDataRekamMedis("4545", "Asma", "Debu", "54", "Sesak Napas", "Asma", "Obat", "DK001", "001"))
            {
                Console.WriteLine("Berhasil Insert");
                Console.ReadLine();
            }

            Console.ReadLine();
        }
    }
}