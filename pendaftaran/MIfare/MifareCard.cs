using PCSC.Iso7816;
using PCSC;
using System;
using System.Diagnostics;

namespace pendaftaran.MIfare
{
    class MifareCard
    {
        private const byte CUSTOM_CLA = 0xFF;
        private IIsoReader _isoreader;

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
                P1 = (byte)keyStructure,
                P2 = keyNumber,
                Data = key
            };

            Debug.WriteLine($"Load Authentication Keys: {BitConverter.ToString(loadKeyCmd.ToArray())}");
            var response = _isoreader.Transmit(loadKeyCmd);
            Debug.WriteLine($"SW1 SW2 = {response.SW1:X2} {response.SW2:X2}");

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

            Debug.WriteLine($"General Authenticate: {BitConverter.ToString(authenticateCmd.ToArray())}");
            var response = _isoreader.Transmit(authenticateCmd);
            Debug.WriteLine($"SW1 SW2 = {response.SW1:X2} {response.SW2:X2}");
            
            return IsSuccess(response);
        }

        private bool IsSuccess(Response response) 
            => (response.SW1 == (byte)SW1Code.Normal) && (response.SW2 == 0x00);
    }
}
