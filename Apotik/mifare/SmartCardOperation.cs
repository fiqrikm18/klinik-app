using System;
using System.Collections.Generic;
using PCSC;
using PCSC.Iso7816;

namespace Apotik.mifare
{
    public class SmartCardOperation
    {
        private const byte Msb = 0x00;
        private readonly IContextFactory contextFactory = ContextFactory.Instance;
        private readonly byte[] key = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
        private readonly string[] readerNames;
        private MifareCard card;
        private IsoReader isoReader;
        private string nfcReader;

        public SmartCardOperation()
        {
            var ctx = contextFactory.Establish(SCardScope.System);
            readerNames = ctx.GetReaders();

            card = new MifareCard(isoReader);
        }

        #region member smart card operations

        public bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }

        public bool IsReaderAvailable()
        {
            if (NoReaderAvailable(readerNames)) return false;

            nfcReader = readerNames[0];
            if (string.IsNullOrEmpty(nfcReader)) return false;

            isoReaderInit();
            card = new MifareCard(isoReader);

            return true;
        }

        public bool WriteBlock(byte msb, byte lsb, byte[] data)
        {
            isoReaderInit();
            card = new MifareCard(isoReader);

            if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
            {
                if (card.Authenticate(msb, lsb, KeyType.KeyA, 0x00))
                    if (card.UpdateBinary(msb, lsb, data))
                        return true;

                return false;
            }

            return false;
        }

        public bool WriteBlockRange(byte msb, byte blockFrom, byte blockTo, byte[] data)
        {
            isoReaderInit();
            card = new MifareCard(isoReader);

            byte i;
            var count = 0;
            var blockData = new byte[16];

            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;

                Array.Copy(data, count * 16, blockData, 0, 16);

                if (WriteBlock(msb, i, blockData)) count++;
                else return false;
            }

            return true;
        }

        public byte[] ReadBlock(byte msb, byte lsb)
        {
            isoReaderInit();
            card = new MifareCard(isoReader);

            var readBinary = new byte[16];

            if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
                if (card.Authenticate(msb, lsb, KeyType.KeyA, 0x00))
                    readBinary = card.ReadBinary(msb, lsb, 16);

            return readBinary;
        }

        //        public byte[] ReadBlockRange(byte msb, byte blockFrom, byte blockTo)
        //        {
        //            byte i;
        //            int nBlock = 0;
        //            int count = 0;
        //
        //            for (i = blockFrom; i <= blockTo;)
        //            {
        //                if ((i + 1) % 4 == 0) continue;
        //                nBlock++;
        //            }
        //
        //            var dataOut = new byte[nBlock * 16];
        //            for (i = blockFrom; i <= blockTo; i++)
        //            {
        //                if ((i + 1) % 4 == 0) continue;
        //                Array.Copy(ReadBlock(msb, i), 0, dataOut, count * 16, 16);
        //                count++;
        //            }
        //
        //            return dataOut;
        //        }


        public void connect()
        {
            var ctx = new SCardContext();
            ctx.Establish(SCardScope.System);
            var reader = new SCardReader(ctx);
            reader.Connect(nfcReader, SCardShareMode.Shared, SCardProtocol.Any);
        }

        public void isoReaderInit()
        {
            try
            {
                var ctx = contextFactory.Establish(SCardScope.System);
                isoReader = new IsoReader(
                    ctx,
                    nfcReader,
                    SCardShareMode.Shared,
                    SCardProtocol.Any,
                    false);

                card = new MifareCard(isoReader);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public byte[] ReadBlockRange(byte msb, byte blockFrom, byte blockTo)
        {
            isoReaderInit();
            card = new MifareCard(isoReader);

            byte i;
            var nBlock = 0;
            var count = 0;

            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;
                nBlock++;
            }

            var dataOut = new byte[nBlock * 16];
            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;
                Array.Copy(ReadBlock(msb, i), 0, dataOut, count * 16, 16);
                count++;
            }

            return dataOut;
        }

        public bool ClearAllBlock()
        {
            //var res = false;

            isoReaderInit();
            card = new MifareCard(isoReader);

            var data = new byte[16];
            if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
                for (byte i = 1; i <= 63; i++)
                    if ((i + 1) % 4 == 0)
                    {
                    }
                    else
                    {
                        if (card.Authenticate(Msb, i, KeyType.KeyA, 0x00))
                        {
                            Array.Clear(data, 0, 16);
                            if (WriteBlock(Msb, i, data))
                            {
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

            return true;
        }
    }

    #endregion
}