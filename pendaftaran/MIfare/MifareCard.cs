using PCSC;
using PCSC.Iso7816;

namespace pendaftaran.Mifare
{
    public class MifareCard
    {
        private const byte CUSTOM_CLA = 0xFF;
        private readonly IIsoReader _isoreader;

        public MifareCard(IsoReader isoReader)
        {
            _isoreader = isoReader;
        }

        public bool LoadKey(KeyStructure keyStructure, byte keyNumber, byte[] key)
        {
            var loadKeyCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any)
            {
                CLA = CUSTOM_CLA,
                Instruction = InstructionCode.ExternalAuthenticate,
                P1 = (byte) keyStructure,
                P2 = keyNumber,
                Data = key
            };

            //Debug.WriteLine($"Load Authentication Keys: {BitConverter.ToString(loadKeyCmd.ToArray())}");
            var response = _isoreader.Transmit(loadKeyCmd);
            //Debug.WriteLine($"SW1 SW2 = {response.SW1:X2} {response.SW2:X2}");

            return IsSuccess(response);
        }

        public bool Authenticate(byte Msb, byte Lsb, KeyType keyType, byte keyNumber)
        {
            var authBlock = new GeneralAuthenticate
            {
                Msb = Msb,
                Lsb = Lsb,
                KeyType = keyType,
                KeyNumber = keyNumber
            };

            var authenticateCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any)
            {
                CLA = CUSTOM_CLA,
                Instruction = InstructionCode.InternalAuthenticate,
                P1 = 0x00,
                P2 = 0x00,
                Data = authBlock.ToArray()
            };

            //Debug.WriteLine($"General Authenticate: {BitConverter.ToString(authenticateCmd.ToArray())}");
            var response = _isoreader.Transmit(authenticateCmd);
            //Debug.WriteLine($"SW1 SW2 = {response.SW1:X2} {response.SW2:X2}");

            return IsSuccess(response);
        }

        public bool UpdateBinary(byte Msb, byte Lsb, byte[] data)
        {
            var updateBinaryCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any)
            {
                CLA = CUSTOM_CLA,
                Instruction = InstructionCode.UpdateBinary,
                P1 = Msb,
                P2 = Lsb,
                Data = data
            };

            //Debug.WriteLine($"Update Binary: {BitConverter.ToString(updateBinaryCmd.ToArray())}");
            var response = _isoreader.Transmit(updateBinaryCmd);
            //Debug.WriteLine($"Sw1 SW2 = {response.SW1:X2} {response.SW2:X2}");

            return IsSuccess(response);
        }

        public byte[] ReadBinary(byte Msb, byte Lsb, int size)
        {
            var readBinaryCmd = new CommandApdu(IsoCase.Case2Short, SCardProtocol.Any)
            {
                CLA = CUSTOM_CLA,
                Instruction = InstructionCode.ReadBinary,
                P1 = Msb,
                P2 = Lsb,
                Le = size
            };

            //Debug.WriteLine($"Read Binary: {BitConverter.ToString(readBinaryCmd.ToArray())}");
            var response = _isoreader.Transmit(readBinaryCmd);
            //Debug.WriteLine($"SW1 SW2 = {response.SW1:X2} {response.SW2:X2}\nData: {response.GetData()}");

            return IsSuccess(response)
                ? response.GetData() ?? new byte[0]
                : null;
        }

        private bool IsSuccess(Response response)
        {
            return response.SW1 == (byte) SW1Code.Normal && response.SW2 == 0x00;
        }
    }
}