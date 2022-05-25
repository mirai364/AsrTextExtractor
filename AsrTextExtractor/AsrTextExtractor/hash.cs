using System;
using System.Linq;
using System.Security.Cryptography;

public enum CRCbitFeed
{
    Left,
    Right,
}

public enum CRCpolynomial : uint
{
    CRC8 = 1 << 7 | 1 << 6 | 1 << 4 | 1 << 2 | 1,
    CRC8_ATM = 1 << 2 | 1 << 1 | 1,
    CRC8_CCITT = 1 << 7 | 1 << 3 | 1 << 2 | 1 << 1 | 1,
    CRC8_Dallas = 1 << 5 | 1 << 4 | 1 << 1 | 1,
    CRC8_SEA = 1 << 4 | 1 << 3 | 1 << 2 | 1,
    CRC16 = 1 << 15 | 1 << 2 | 1,
    CRC16_CCITT = 1 << 12 | 1 << 5 | 1,
    CRC16_IBM = CRC16,
    CRC32 = 1 << 26 | 1 << 23 | 1 << 22 | 1 << 16 | 1 << 12 | 1 << 11 | 1 << 10 | 1 << 8 | 1 << 7 | 1 << 5 | 1 << 4 | 1 << 2 | 1 << 1 | 1,
    CRC32_C = 1 << 28 | 1 << 27 | 1 << 26 | 1 << 25 | 1 << 23 | 1 << 22 | 1 << 20 | 1 << 19 | 1 << 18 | 1 << 14 | 1 << 13 | 1 << 11 | 1 << 10 | 1 << 9 | 1 << 8 | 1 << 6 | 1,
    CRC32_K = 1 << 30 | 1 << 29 | 1 << 28 | 1 << 26 | 1 << 20 | 1 << 19 | 1 << 17 | 1 << 16 | 1 << 15 | 1 << 11 | 1 << 10 | 1 << 7 | 1 << 6 | 1 << 4 | 1 << 2 | 1 << 1 | 1,
}

public class CRC : HashAlgorithm
{
    public CRC(CRCpolynomial poly, CRCbitFeed feed = CRCbitFeed.Right)
    {
        InitialMask = true;
        FinalMask = true;
        Polynomial = (uint)poly;
        BitFeed = feed;

        base.HashSizeValue = (int)Math.Floor(Math.Log(Polynomial) / Math.Log(2F) / 8 + 1) * 8;
        CRCmask = (uint)Math.Pow(2, base.HashSizeValue) - 1;

        CRCtable = new uint[256];

        if (BitFeed == CRCbitFeed.Left)
        {
            CRCpoly = Polynomial;
            uint msb = (uint)(1 << (base.HashSizeValue - 1));

            for (uint i = 0; i < 256; i++)
            {
                uint c = i << (base.HashSizeValue - 8);
                for (int j = 0; j < 8; j++)
                    c = (uint)((c << 1) ^ (((c & msb) != 0) ? CRCpoly : 0));
                CRCtable[i] = c & CRCmask;
            }
        }
        else
        {
            for (int i = 0; i < base.HashSizeValue; i += 4)
                CRCpoly = (CRCpoly << 4) | Rtbl[(Polynomial >> i) & 0xf];

            for (uint i = 0; i < 256; i++)
            {
                uint c = i;
                for (int j = 0; j < 8; j++)
                    c = (c & 1) != 0 ? (CRCpoly ^ (c >> 1)) : (c >> 1);
                CRCtable[i] = c;
            }
        }

        Initialize();
    }

    uint[] Rtbl = { 0x0, 0x8, 0x4, 0xc, 0x2, 0xa, 0x6, 0xe, 0x1, 0x9, 0x5, 0xd, 0x3, 0xb, 0x7, 0xf };

    bool _InitialMask = false;
    public bool InitialMask
    {
        get
        {
            return _InitialMask;
        }
        set
        {
            _InitialMask = value;
            Initialize();
        }
    }
    public bool FinalMask { get; set; }
    public uint Polynomial { get; private set; }
    public CRCbitFeed BitFeed { get; private set; }

    public int CRCsize
    {
        get { return base.HashSizeValue / 8; }
    }

    byte[] CRCcrc;
    uint LSTvalue;

    uint CRCmask;
    uint CRCvalue;
    uint CRCpoly;
    uint[] CRCtable;

    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        unchecked
        {
            while (--cbSize >= 0)
                CRCvalue = CRCtable[(CRCvalue ^ array[ibStart++]) & 0xff] ^ (CRCvalue >> 8);
        }
        LSTvalue = CRCvalue;
    }

    protected override byte[] HashFinal()
    {
        HashValue = new byte[CRCsize];

        for (uint i = (uint)HashValue.Length, temp = CRCvalue ^ (FinalMask ? CRCmask : 0); i > 0; temp >>= 8)
            HashValue[--i] = (byte)(temp & 0xff);

        return HashValue;
    }

    public override void Initialize()
    {
        CRCvalue = InitialMask ? CRCmask : 0;
    }

    public bool ConsistencyCheck(byte[] crc, byte[] buffer)
    {
        CRCcrc = crc.Reverse().ToArray();
        uint CRCuint = 0;



        CRCvalue = CRCmask;
        ComputeHash(buffer);
        CRCuint = LSTvalue;

        CRCvalue = CRCuint;
        HashCore(CRCcrc, 0, CRCcrc.Length);
        if (CRCvalue == 0)
            return true;

        CRCcrc = CRCcrc.Select(e => (byte)(e ^ 0xff)).ToArray();

        CRCvalue = CRCuint;
        HashCore(CRCcrc, 0, CRCcrc.Length);
        if (CRCvalue == 0)
            return true;



        CRCvalue = 0;
        ComputeHash(buffer);
        CRCuint = LSTvalue;

        CRCvalue = CRCuint;
        HashCore(CRCcrc, 0, CRCcrc.Length);
        if (CRCvalue == 0)
            return true;

        CRCcrc = CRCcrc.Select(e => (byte)(e ^ 0xff)).ToArray();

        CRCvalue = CRCuint;
        HashCore(CRCcrc, 0, CRCcrc.Length);
        if (CRCvalue == 0)
            return true;



        return false;
    }
}