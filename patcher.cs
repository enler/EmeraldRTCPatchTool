using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emeraldRTCPatcher
{
    class patcher
    {
        private static byte[] setRTCTimeSig = new byte[] { 0xF0, 0xB5, 0x05, 0x1C, 0x0F, 0x1C, 0x16, 0x1C, 0xFF, 0xF7, 0x68, 0xFE };

        private static byte[] code = new byte[] {
            0x73, 0x46, 0x10, 0xb5, 0x0c, 0x1c, 0x34, 0x49, 0x09, 0x78, 0x00, 0x29, 0x02, 0xd0, 0x31, 0x49,
            0x8b, 0x42, 0x05, 0xd1, 0x21, 0x1c, 0x00, 0xf0, 0x0e, 0xf8, 0x10, 0xbc, 0x01, 0xbc, 0x00, 0x47,
            0x2a, 0x4b, 0x19, 0x68, 0x00, 0x23, 0x23, 0x60, 0x23, 0x71, 0x27, 0x4a, 0x0e, 0x31, 0x20, 0x1c,
            0x00, 0xf0, 0x07, 0xf8, 0xf1, 0xe7, 0xf0, 0xb5, 0x05, 0x1c, 0x0f, 0x1c, 0x16, 0x1c, 0x24, 0x4b,
            0x18, 0x47, 0x26, 0x4b, 0x18, 0x47, 0xc0, 0x46, 0xf0, 0x40, 0x2d, 0xe9, 0x03, 0xc0, 0xd1, 0xe5,
            0x92, 0x0c, 0x05, 0xe0, 0x84, 0x40, 0x9f, 0xe5, 0x95, 0x64, 0xc7, 0xe0, 0xc5, 0x3f, 0xa0, 0xe1,
            0x07, 0xe0, 0x85, 0xe0, 0xce, 0xe2, 0x63, 0xe0, 0x0e, 0x32, 0x6e, 0xe0, 0x03, 0xc1, 0x45, 0xe0,
            0x04, 0xc0, 0xc0, 0xe5, 0x02, 0x30, 0xd1, 0xe5, 0x92, 0xe3, 0x25, 0xe0, 0x95, 0x64, 0xc7, 0xe0,
            0xc5, 0xcf, 0xa0, 0xe1, 0x07, 0xe0, 0x85, 0xe0, 0xce, 0xc2, 0x6c, 0xe0, 0x0c, 0xe2, 0x6c, 0xe0,
            0x0e, 0x31, 0x45, 0xe0, 0x03, 0x30, 0xc0, 0xe5, 0xb0, 0x30, 0xd1, 0xe1, 0x93, 0xc2, 0x2e, 0xe0,
            0x3c, 0x30, 0x9f, 0xe5, 0x9e, 0x43, 0xc5, 0xe0, 0xce, 0x3f, 0xa0, 0xe1, 0x45, 0x31, 0x63, 0xe0,
            0x83, 0x10, 0x83, 0xe0, 0x81, 0x21, 0x4e, 0xe0, 0xb0, 0x30, 0xc0, 0xe1, 0x02, 0x20, 0xc0, 0xe5,
            0xf0, 0x40, 0xbd, 0xe8, 0x1e, 0xff, 0x2f, 0xe1, 0x06, 0x00, 0x00, 0x00, 0xf0, 0x5a, 0x00, 0x03,
            0x71, 0xf1, 0x02, 0x08, 0x4f, 0xf2, 0x02, 0x08, 0x08, 0x0e, 0x00, 0x03, 0x48, 0x00, 0x00, 0x00,
            0x89, 0x88, 0x88, 0x88, 0xab, 0xaa, 0xaa, 0x2a, };

        private static int searchFunc(byte[] rom, byte[] funcSig)
        {
            for (int i = 0; i < rom.Length - funcSig.Length; i++)
            {
                int j;
                for (j = 0; j < funcSig.Length; j++)
                {
                    if (rom[i + j] != funcSig[j])
                        break;
                }
                if (j == funcSig.Length)
                    return i;
            }
            return -1;
        }

        private static int searchReservedRegion(byte[] rom, int length)
        {
            for (int i = rom.Length - 1; i >= length; i-=4)
            {
                int j;
                for (j = 0; j < length; j++)
                {
                    if (rom[i - j] != 0xFF)
                        break;
                }
                if (j == length)
                    return i - j + 1 + 0x8000000;
            }
            return -1;
        }

        private static int[] getSymbolAddr(byte[] rom)
        {
            int[] results = new int[4];
            int setRTCTimeAddr = searchFunc(rom, setRTCTimeSig);
            if (setRTCTimeAddr != -1)
            {
                results[0] = setRTCTimeAddr + 0x8000000;
                results[1] = results[0] + 0x9;
                results[2] = results[0] + 0xE7;
                results[3] = BitConverter.ToInt32(rom, setRTCTimeAddr + 0xAC);
            }
            return results;
        }

        private const uint ldr_r3_bx_r3 = 0x47184B00;

        private const int powerOffset = 0xC8;
        private const int gameTimeRefOffset = 0xCC;
        private const int orig_setRTCTimeOffset = 0xD0;
        private const int callAddrOffset = 0xD4;
        private const int timeMulRefOffset = 0xDC;

        public static void ApplyRTC(byte[] rom, int power)
        {
            int freeAddr = searchReservedRegion(rom, (code.Length + 255) & ~255);
            int[] results = getSymbolAddr(rom);
            Array.Copy(BitConverter.GetBytes(power), 0, code, powerOffset, sizeof(int));
            Array.Copy(BitConverter.GetBytes(results[3]), 0, code, gameTimeRefOffset, sizeof(int));
            Array.Copy(BitConverter.GetBytes(results[1]), 0, code, orig_setRTCTimeOffset, sizeof(int));
            Array.Copy(BitConverter.GetBytes(results[2]), 0, code, callAddrOffset, sizeof(int));
            Array.Copy(BitConverter.GetBytes(BitConverter.ToInt32(code, timeMulRefOffset) + freeAddr), 0, code, timeMulRefOffset, sizeof(int));
            Array.Copy(code, 0, rom, freeAddr & 0x1FFFFFF, code.Length);
            Array.Copy(BitConverter.GetBytes(ldr_r3_bx_r3), 0, rom, results[0] & 0x1FFFFFF, sizeof(uint));
            Array.Copy(BitConverter.GetBytes(freeAddr + 1), 0, rom, (results[0] + 4) & 0x1FFFFFF, sizeof(int));
        }

        private const string emeraldMagic = "pokemon emerald version";
        private const int magicOffset = 0x108;
        private const int emeraldRomMinSize = 0x1000000;
        private const int emeraldRomMaxSize = 0x2000000;

        public static bool IsRomVaild(byte[] rom)
        {
            if (rom.Length >= emeraldRomMinSize && rom.Length < emeraldRomMaxSize)
            {
                string magic = ASCIIEncoding.ASCII.GetString(rom, magicOffset, emeraldMagic.Length);
                if (magic == emeraldMagic)
                {
                    var results = getSymbolAddr(rom);
                    return results[0] != 0;
                }
            }
            return false;
        }
    }
}
