using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pendaftaran.Mifare;
using admin.Utils;

namespace CardWriter
{
    class Program
    {
        private static byte Msb = 0x00;
        private static byte BlockUsername = 1;
        private static byte BlockPasswordFrom = 2;
        private static byte BlockPasswordTo = 4;

        private static SmartCardOperation so;

        static void Main(string[] args)
        {
            so = new SmartCardOperation();
            if (so.IsReaderAvailable())
            {
                so.isoReaderInit();

                var user = Util.ToArrayByte16("ADM001");
                var pass = Util.ToArrayByte32(Encryptor.MD5Hash("ADM001"));

                if (so.WriteBlock(Msb, BlockUsername, user))
                {
                    //Console.WriteLine(user.ToString());
                    Console.WriteLine(Util.ToASCII(so.ReadBlock(Msb, BlockUsername), 0, user.Length));
                }

                if(so.WriteBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo, pass))
                {
                    Console.WriteLine(Util.ToASCII(so.ReadBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo), 0, pass.Length));
                }
            }
            else
            {
                Console.WriteLine("Reader not available");
            }

            Console.ReadLine();
        }
    }
}
