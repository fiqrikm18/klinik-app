using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apotik.mifare
{
    public enum KeyStructure : byte
    {
        VolatileMemory = 0x00,
        NonVolatileMemory = 0x21
    }
}
