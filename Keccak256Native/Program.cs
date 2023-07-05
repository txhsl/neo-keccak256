using System.Text;

ulong[] KeccakRoundConstants =
{
    0x0000000000000001UL, 0x0000000000008082UL, 0x800000000000808aUL, 0x8000000080008000UL,
    0x000000000000808bUL, 0x0000000080000001UL, 0x8000000080008081UL, 0x8000000000008009UL,
    0x000000000000008aUL, 0x0000000000000088UL, 0x0000000080008009UL, 0x000000008000000aUL,
    0x000000008000808bUL, 0x800000000000008bUL, 0x8000000000008089UL, 0x8000000000008003UL,
    0x8000000000008002UL, 0x8000000000000080UL, 0x000000000000800aUL, 0x800000008000000aUL,
    0x8000000080008081UL, 0x8000000000008080UL, 0x0000000080000001UL, 0x8000000080008008UL
};

int FixedOutputLength = 32;
int Rate = 136;
byte Dsbyte = 0x01;

byte[] ComputeHash(byte[] input)
{
    return Squeeze(Absorb(input));
}

ulong[] Absorb(byte[] input)
{
    int len = input.Length;
    ulong[] state = new ulong[25];
    for (int i = 0; i < len; i += Rate)
    {
        if (i + Rate <= len)
        {
            // longer than rate
            byte[] buf = input.Skip(i).Take(Rate).ToArray();
            state = KeccakF1600(XorIn(state, buf));
        }
        else
        {
            // shorter than rate
            byte[] buf = input.Skip(i).Take(len - i).ToArray();
            state = KeccakF1600(XorIn(state, Pad(buf)));
        }
    }
    return state;
}

byte[] Squeeze(ulong[] state)
{
    byte[] output = new byte[FixedOutputLength];
    output = CopyOut(state, output);
    return output;
}

byte[] Pad(byte[] input)
{
    // Copy input to buf
    int len = input.Length;
    byte[] buf = new byte[Rate];
    input.CopyTo(buf, 0);
    // dsbyte
    buf[len] = Dsbyte;
    // Final bit
    buf[Rate - 1] ^= 0x80;
    return buf;
}

ulong[] KeccakF1600(ulong[] a)
{
    Console.WriteLine("KeccakF1600: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16} {17} {18} {19} {20} {21} {22} {23} {24}",
        a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8], a[9], a[10], a[11], a[12], a[13], a[14], a[15], a[16], a[17], a[18], a[19], a[20], a[21], a[22], a[23], a[24]);
    ulong t, bc0, bc1, bc2, bc3, bc4, d0, d1, d2, d3, d4;

    for (int i = 0; i < 24; i += 4)
    {
        // Round 1
        bc0 = a[0] ^ a[5] ^ a[10] ^ a[15] ^ a[20];
        bc1 = a[1] ^ a[6] ^ a[11] ^ a[16] ^ a[21];
        bc2 = a[2] ^ a[7] ^ a[12] ^ a[17] ^ a[22];
        bc3 = a[3] ^ a[8] ^ a[13] ^ a[18] ^ a[23];
        bc4 = a[4] ^ a[9] ^ a[14] ^ a[19] ^ a[24];
        d0 = bc4 ^ ((bc1 << 1) | (bc1 >> 63));
        d1 = bc0 ^ ((bc2 << 1) | (bc2 >> 63));
        d2 = bc1 ^ ((bc3 << 1) | (bc3 >> 63));
        d3 = bc2 ^ ((bc4 << 1) | (bc4 >> 63));
        d4 = bc3 ^ ((bc0 << 1) | (bc0 >> 63));
        Console.WriteLine("R1 init: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", bc0, bc1, bc2, bc3, bc4, d0, d1, d2, d3, d4);

        bc0 = a[0] ^ d0;
        t = a[6] ^ d1;
        bc1 = (t << 44) | (t >> 20);
        t = a[12] ^ d2;
        bc2 = (t << 43) | (t >> 21);
        t = a[18] ^ d3;
        bc3 = (t << 21) | (t >> 43);
        t = a[24] ^ d4;
        bc4 = (t << 14) | (t >> 50);
        a[0] = bc0 ^ (bc2 & ~bc1) ^ KeccakRoundConstants[i];
        a[6] = bc1 ^ (bc3 & ~bc2);
        a[12] = bc2 ^ (bc4 & ~bc3);
        a[18] = bc3 ^ (bc0 & ~bc4);
        a[24] = bc4 ^ (bc1 & ~bc0);
        Console.WriteLine("R1 0: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}", bc0, bc1, bc2, bc3, bc4, d0, d1, d2, d3, d4, t);

        t = a[10] ^ d0;
        bc2 = (t << 3) | (t >> 61);
        t = a[16] ^ d1;
        bc3 = (t << 45) | (t >> 19);
        t = a[22] ^ d2;
        bc4 = (t << 61) | (t >> 3);
        t = a[3] ^ d3;
        bc0 = (t << 28) | (t >> 36);
        t = a[9] ^ d4;
        bc1 = (t << 20) | (t >> 44);
        a[10] = bc0 ^ (bc2 & ~bc1);
        a[16] = bc1 ^ (bc3 & ~bc2);
        a[22] = bc2 ^ (bc4 & ~bc3);
        a[3] = bc3 ^ (bc0 & ~bc4);
        a[9] = bc4 ^ (bc1 & ~bc0);
        Console.WriteLine("R1 1: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}", bc0, bc1, bc2, bc3, bc4, d0, d1, d2, d3, d4, t);

        t = a[20] ^ d0;
        bc4 = (t << 18) | (t >> 46);
        t = a[1] ^ d1;
        bc0 = (t << 1) | (t >> 63);
        t = a[7] ^ d2;
        bc1 = (t << 6) | (t >> 58);
        t = a[13] ^ d3;
        bc2 = (t << 25) | (t >> 39);
        t = a[19] ^ d4;
        bc3 = (t << 8) | (t >> 56);
        a[20] = bc0 ^ (bc2 & ~bc1);
        a[1] = bc1 ^ (bc3 & ~bc2);
        a[7] = bc2 ^ (bc4 & ~bc3);
        a[13] = bc3 ^ (bc0 & ~bc4);
        a[19] = bc4 ^ (bc1 & ~bc0);
        Console.WriteLine("R1 2: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}", bc0, bc1, bc2, bc3, bc4, d0, d1, d2, d3, d4, t);

        t = a[5] ^ d0;
        bc1 = (t << 36) | (t >> 28);
        t = a[11] ^ d1;
        bc2 = (t << 10) | (t >> 54);
        t = a[17] ^ d2;
        bc3 = (t << 15) | (t >> 49);
        t = a[23] ^ d3;
        bc4 = (t << 56) | (t >> 8);
        t = a[4] ^ d4;
        bc0 = (t << 27) | (t >> 37);
        a[5] = bc0 ^ (bc2 & ~bc1);
        a[11] = bc1 ^ (bc3 & ~bc2);
        a[17] = bc2 ^ (bc4 & ~bc3);
        a[23] = bc3 ^ (bc0 & ~bc4);
        a[4] = bc4 ^ (bc1 & ~bc0);
        Console.WriteLine("R1 3: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}", bc0, bc1, bc2, bc3, bc4, d0, d1, d2, d3, d4, t);

        t = a[15] ^ d0;
        bc3 = (t << 41) | (t >> 23);
        t = a[21] ^ d1;
        bc4 = (t << 2) | (t >> 62);
        t = a[2] ^ d2;
        bc0 = (t << 62) | (t >> 2);
        t = a[8] ^ d3;
        bc1 = (t << 55) | (t >> 9);
        t = a[14] ^ d4;
        bc2 = (t << 39) | (t >> 25);
        a[15] = bc0 ^ (bc2 & ~bc1);
        a[21] = bc1 ^ (bc3 & ~bc2);
        a[2] = bc2 ^ (bc4 & ~bc3);
        a[8] = bc3 ^ (bc0 & ~bc4);
        a[14] = bc4 ^ (bc1 & ~bc0);
        Console.WriteLine("R1 4: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}", bc0, bc1, bc2, bc3, bc4, d0, d1, d2, d3, d4, t);

        // Round 2
        bc0 = a[0] ^ a[5] ^ a[10] ^ a[15] ^ a[20];
        bc1 = a[1] ^ a[6] ^ a[11] ^ a[16] ^ a[21];
        bc2 = a[2] ^ a[7] ^ a[12] ^ a[17] ^ a[22];
        bc3 = a[3] ^ a[8] ^ a[13] ^ a[18] ^ a[23];
        bc4 = a[4] ^ a[9] ^ a[14] ^ a[19] ^ a[24];
        d0 = bc4 ^ ((bc1 << 1) | (bc1 >> 63));
        d1 = bc0 ^ ((bc2 << 1) | (bc2 >> 63));
        d2 = bc1 ^ ((bc3 << 1) | (bc3 >> 63));
        d3 = bc2 ^ ((bc4 << 1) | (bc4 >> 63));
        d4 = bc3 ^ ((bc0 << 1) | (bc0 >> 63));

        bc0 = a[0] ^ d0;
        t = a[16] ^ d1;
        bc1 = (t << 44) | (t >> 20);
        t = a[7] ^ d2;
        bc2 = (t << 43) | (t >> 21);
        t = a[23] ^ d3;
        bc3 = (t << 21) | (t >> 43);
        t = a[14] ^ d4;
        bc4 = (t << 14) | (t >> 50);
        a[0] = bc0 ^ (bc2 & ~bc1) ^ KeccakRoundConstants[i + 1];
        a[16] = bc1 ^ (bc3 & ~bc2);
        a[7] = bc2 ^ (bc4 & ~bc3);
        a[23] = bc3 ^ (bc0 & ~bc4);
        a[14] = bc4 ^ (bc1 & ~bc0);

        t = a[20] ^ d0;
        bc2 = (t << 3) | (t >> 61);
        t = a[11] ^ d1;
        bc3 = (t << 45) | (t >> 19);
        t = a[2] ^ d2;
        bc4 = (t << 61) | (t >> 3);
        t = a[18] ^ d3;
        bc0 = (t << 28) | (t >> 36);
        t = a[9] ^ d4;
        bc1 = (t << 20) | (t >> 44);
        a[20] = bc0 ^ (bc2 & ~bc1);
        a[11] = bc1 ^ (bc3 & ~bc2);
        a[2] = bc2 ^ (bc4 & ~bc3);
        a[18] = bc3 ^ (bc0 & ~bc4);
        a[9] = bc4 ^ (bc1 & ~bc0);

        t = a[15] ^ d0;
        bc4 = (t << 18) | (t >> 46);
        t = a[6] ^ d1;
        bc0 = (t << 1) | (t >> 63);
        t = a[22] ^ d2;
        bc1 = (t << 6) | (t >> 58);
        t = a[13] ^ d3;
        bc2 = (t << 25) | (t >> 39);
        t = a[4] ^ d4;
        bc3 = (t << 8) | (t >> 56);
        a[15] = bc0 ^ (bc2 & ~bc1);
        a[6] = bc1 ^ (bc3 & ~bc2);
        a[22] = bc2 ^ (bc4 & ~bc3);
        a[13] = bc3 ^ (bc0 & ~bc4);
        a[4] = bc4 ^ (bc1 & ~bc0);

        t = a[10] ^ d0;
        bc1 = (t << 36) | (t >> 28);
        t = a[1] ^ d1;
        bc2 = (t << 10) | (t >> 54);
        t = a[17] ^ d2;
        bc3 = (t << 15) | (t >> 49);
        t = a[8] ^ d3;
        bc4 = (t << 56) | (t >> 8);
        t = a[24] ^ d4;
        bc0 = (t << 27) | (t >> 37);
        a[10] = bc0 ^ (bc2 & ~bc1);
        a[1] = bc1 ^ (bc3 & ~bc2);
        a[17] = bc2 ^ (bc4 & ~bc3);
        a[8] = bc3 ^ (bc0 & ~bc4);
        a[24] = bc4 ^ (bc1 & ~bc0);

        t = a[5] ^ d0;
        bc3 = (t << 41) | (t >> 23);
        t = a[21] ^ d1;
        bc4 = (t << 2) | (t >> 62);
        t = a[12] ^ d2;
        bc0 = (t << 62) | (t >> 2);
        t = a[3] ^ d3;
        bc1 = (t << 55) | (t >> 9);
        t = a[19] ^ d4;
        bc2 = (t << 39) | (t >> 25);
        a[5] = bc0 ^ (bc2 & ~bc1);
        a[21] = bc1 ^ (bc3 & ~bc2);
        a[12] = bc2 ^ (bc4 & ~bc3);
        a[3] = bc3 ^ (bc0 & ~bc4);
        a[19] = bc4 ^ (bc1 & ~bc0);

        // Round 3
        bc0 = a[0] ^ a[5] ^ a[10] ^ a[15] ^ a[20];
        bc1 = a[1] ^ a[6] ^ a[11] ^ a[16] ^ a[21];
        bc2 = a[2] ^ a[7] ^ a[12] ^ a[17] ^ a[22];
        bc3 = a[3] ^ a[8] ^ a[13] ^ a[18] ^ a[23];
        bc4 = a[4] ^ a[9] ^ a[14] ^ a[19] ^ a[24];
        d0 = bc4 ^ ((bc1 << 1) | (bc1 >> 63));
        d1 = bc0 ^ ((bc2 << 1) | (bc2 >> 63));
        d2 = bc1 ^ ((bc3 << 1) | (bc3 >> 63));
        d3 = bc2 ^ ((bc4 << 1) | (bc4 >> 63));
        d4 = bc3 ^ ((bc0 << 1) | (bc0 >> 63));

        bc0 = a[0] ^ d0;
        t = a[11] ^ d1;
        bc1 = (t << 44) | (t >> 20);
        t = a[22] ^ d2;
        bc2 = (t << 43) | (t >> 21);
        t = a[8] ^ d3;
        bc3 = (t << 21) | (t >> 43);
        t = a[19] ^ d4;
        bc4 = (t << 14) | (t >> 50);
        a[0] = bc0 ^ (bc2 & ~bc1) ^ KeccakRoundConstants[i + 2];
        a[11] = bc1 ^ (bc3 & ~bc2);
        a[22] = bc2 ^ (bc4 & ~bc3);
        a[8] = bc3 ^ (bc0 & ~bc4);
        a[19] = bc4 ^ (bc1 & ~bc0);

        t = a[15] ^ d0;
        bc2 = (t << 3) | (t >> 61);
        t = a[1] ^ d1;
        bc3 = (t << 45) | (t >> 19);
        t = a[12] ^ d2;
        bc4 = (t << 61) | (t >> 3);
        t = a[23] ^ d3;
        bc0 = (t << 28) | (t >> 36);
        t = a[9] ^ d4;
        bc1 = (t << 20) | (t >> 44);
        a[15] = bc0 ^ (bc2 & ~bc1);
        a[1] = bc1 ^ (bc3 & ~bc2);
        a[12] = bc2 ^ (bc4 & ~bc3);
        a[23] = bc3 ^ (bc0 & ~bc4);
        a[9] = bc4 ^ (bc1 & ~bc0);

        t = a[5] ^ d0;
        bc4 = (t << 18) | (t >> 46);
        t = a[16] ^ d1;
        bc0 = (t << 1) | (t >> 63);
        t = a[2] ^ d2;
        bc1 = (t << 6) | (t >> 58);
        t = a[13] ^ d3;
        bc2 = (t << 25) | (t >> 39);
        t = a[24] ^ d4;
        bc3 = (t << 8) | (t >> 56);
        a[5] = bc0 ^ (bc2 & ~bc1);
        a[16] = bc1 ^ (bc3 & ~bc2);
        a[2] = bc2 ^ (bc4 & ~bc3);
        a[13] = bc3 ^ (bc0 & ~bc4);
        a[24] = bc4 ^ (bc1 & ~bc0);

        t = a[20] ^ d0;
        bc1 = (t << 36) | (t >> 28);
        t = a[6] ^ d1;
        bc2 = (t << 10) | (t >> 54);
        t = a[17] ^ d2;
        bc3 = (t << 15) | (t >> 49);
        t = a[3] ^ d3;
        bc4 = (t << 56) | (t >> 8);
        t = a[14] ^ d4;
        bc0 = (t << 27) | (t >> 37);
        a[20] = bc0 ^ (bc2 & ~bc1);
        a[6] = bc1 ^ (bc3 & ~bc2);
        a[17] = bc2 ^ (bc4 & ~bc3);
        a[3] = bc3 ^ (bc0 & ~bc4);
        a[14] = bc4 ^ (bc1 & ~bc0);

        t = a[10] ^ d0;
        bc3 = (t << 41) | (t >> 23);
        t = a[21] ^ d1;
        bc4 = (t << 2) | (t >> 62);
        t = a[7] ^ d2;
        bc0 = (t << 62) | (t >> 2);
        t = a[18] ^ d3;
        bc1 = (t << 55) | (t >> 9);
        t = a[4] ^ d4;
        bc2 = (t << 39) | (t >> 25);
        a[10] = bc0 ^ (bc2 & ~bc1);
        a[21] = bc1 ^ (bc3 & ~bc2);
        a[7] = bc2 ^ (bc4 & ~bc3);
        a[18] = bc3 ^ (bc0 & ~bc4);
        a[4] = bc4 ^ (bc1 & ~bc0);

        // Round 4
        bc0 = a[0] ^ a[5] ^ a[10] ^ a[15] ^ a[20];
        bc1 = a[1] ^ a[6] ^ a[11] ^ a[16] ^ a[21];
        bc2 = a[2] ^ a[7] ^ a[12] ^ a[17] ^ a[22];
        bc3 = a[3] ^ a[8] ^ a[13] ^ a[18] ^ a[23];
        bc4 = a[4] ^ a[9] ^ a[14] ^ a[19] ^ a[24];
        d0 = bc4 ^ ((bc1 << 1) | (bc1 >> 63));
        d1 = bc0 ^ ((bc2 << 1) | (bc2 >> 63));
        d2 = bc1 ^ ((bc3 << 1) | (bc3 >> 63));
        d3 = bc2 ^ ((bc4 << 1) | (bc4 >> 63));
        d4 = bc3 ^ ((bc0 << 1) | (bc0 >> 63));

        bc0 = a[0] ^ d0;
        t = a[1] ^ d1;
        bc1 = (t << 44) | (t >> 20);
        t = a[2] ^ d2;
        bc2 = (t << 43) | (t >> 21);
        t = a[3] ^ d3;
        bc3 = (t << 21) | (t >> 43);
        t = a[4] ^ d4;
        bc4 = (t << 14) | (t >> 50);
        a[0] = bc0 ^ (bc2 & ~bc1) ^ KeccakRoundConstants[i + 3];
        a[1] = bc1 ^ (bc3 & ~bc2);
        a[2] = bc2 ^ (bc4 & ~bc3);
        a[3] = bc3 ^ (bc0 & ~bc4);
        a[4] = bc4 ^ (bc1 & ~bc0);

        t = a[5] ^ d0;
        bc2 = (t << 3) | (t >> 61);
        t = a[6] ^ d1;
        bc3 = (t << 45) | (t >> 19);
        t = a[7] ^ d2;
        bc4 = (t << 61) | (t >> 3);
        t = a[8] ^ d3;
        bc0 = (t << 28) | (t >> 36);
        t = a[9] ^ d4;
        bc1 = (t << 20) | (t >> 44);
        a[5] = bc0 ^ (bc2 & ~bc1);
        a[6] = bc1 ^ (bc3 & ~bc2);
        a[7] = bc2 ^ (bc4 & ~bc3);
        a[8] = bc3 ^ (bc0 & ~bc4);
        a[9] = bc4 ^ (bc1 & ~bc0);

        t = a[10] ^ d0;
        bc4 = (t << 18) | (t >> 46);
        t = a[11] ^ d1;
        bc0 = (t << 1) | (t >> 63);
        t = a[12] ^ d2;
        bc1 = (t << 6) | (t >> 58);
        t = a[13] ^ d3;
        bc2 = (t << 25) | (t >> 39);
        t = a[14] ^ d4;
        bc3 = (t << 8) | (t >> 56);
        a[10] = bc0 ^ (bc2 & ~bc1);
        a[11] = bc1 ^ (bc3 & ~bc2);
        a[12] = bc2 ^ (bc4 & ~bc3);
        a[13] = bc3 ^ (bc0 & ~bc4);
        a[14] = bc4 ^ (bc1 & ~bc0);

        t = a[15] ^ d0;
        bc1 = (t << 36) | (t >> 28);
        t = a[16] ^ d1;
        bc2 = (t << 10) | (t >> 54);
        t = a[17] ^ d2;
        bc3 = (t << 15) | (t >> 49);
        t = a[18] ^ d3;
        bc4 = (t << 56) | (t >> 8);
        t = a[19] ^ d4;
        bc0 = (t << 27) | (t >> 37);
        a[15] = bc0 ^ (bc2 & ~bc1);
        a[16] = bc1 ^ (bc3 & ~bc2);
        a[17] = bc2 ^ (bc4 & ~bc3);
        a[18] = bc3 ^ (bc0 & ~bc4);
        a[19] = bc4 ^ (bc1 & ~bc0);

        t = a[20] ^ d0;
        bc3 = (t << 41) | (t >> 23);
        t = a[21] ^ d1;
        bc4 = (t << 2) | (t >> 62);
        t = a[22] ^ d2;
        bc0 = (t << 62) | (t >> 2);
        t = a[23] ^ d3;
        bc1 = (t << 55) | (t >> 9);
        t = a[24] ^ d4;
        bc2 = (t << 39) | (t >> 25);
        a[20] = bc0 ^ (bc2 & ~bc1);
        a[21] = bc1 ^ (bc3 & ~bc2);
        a[22] = bc2 ^ (bc4 & ~bc3);
        a[23] = bc3 ^ (bc0 & ~bc4);
        a[24] = bc4 ^ (bc1 & ~bc0);
    }

    return a;
}

ulong[] XorIn(ulong[] a, byte[] buf)
{
    for (int i = 0; i * 8 < buf.Length; i++)
    {
        a[i] ^= (ulong)buf[i * 8]
            | (ulong)buf[i * 8 + 1] << 8
            | (ulong)buf[i * 8 + 2] << 16
            | (ulong)buf[i * 8 + 3] << 24
            | (ulong)buf[i * 8 + 4] << 32
            | (ulong)buf[i * 8 + 5] << 40
            | (ulong)buf[i * 8 + 6] << 48
            | (ulong)buf[i * 8 + 7] << 56;

        Console.WriteLine((ulong)buf[i * 8]);
        Console.WriteLine((ulong)buf[i * 8 + 1]);
        Console.WriteLine((ulong)buf[i * 8 + 2]);
        Console.WriteLine((ulong)buf[i * 8 + 3]);
        Console.WriteLine((ulong)buf[i * 8 + 4]);
        Console.WriteLine((ulong)buf[i * 8 + 5]);
        Console.WriteLine((ulong)buf[i * 8 + 6]);
        Console.WriteLine((ulong)buf[i * 8 + 7]);

        Console.WriteLine((ulong)buf[i * 8]);
        Console.WriteLine((ulong)buf[i * 8 + 1] << 8);
        Console.WriteLine((ulong)buf[i * 8 + 2] << 16);
        Console.WriteLine((ulong)buf[i * 8 + 3] << 24);
        Console.WriteLine((ulong)buf[i * 8 + 4] << 32);
        Console.WriteLine((ulong)buf[i * 8 + 5] << 40);
        Console.WriteLine((ulong)buf[i * 8 + 6] << 48);
        Console.WriteLine((ulong)buf[i * 8 + 7] << 56);
    }
    return a;
}

byte[] CopyOut(ulong[] a, byte[] buf)
{
    for (int i = 0; i * 8 < buf.Length; i++)
    {
        buf[i * 8] = (byte)a[i];
        buf[i * 8 + 1] = (byte)(a[i] >> 8);
        buf[i * 8 + 2] = (byte)(a[i] >> 16);
        buf[i * 8 + 3] = (byte)(a[i] >> 24);
        buf[i * 8 + 4] = (byte)(a[i] >> 32);
        buf[i * 8 + 5] = (byte)(a[i] >> 40);
        buf[i * 8 + 6] = (byte)(a[i] >> 48);
        buf[i * 8 + 7] = (byte)(a[i] >> 56);
    }
    return buf;
}

string ToHexString(byte[] value)
{
    StringBuilder sb = new();
    foreach (byte b in value)
        sb.AppendFormat("{0:x2}", b);
    return sb.ToString();
}

byte[] result = ComputeHash(Encoding.Default.GetBytes("Hello World."));

Console.WriteLine(ToHexString(result));
Console.WriteLine("6ac466601079053c254ab4f5750b05b5e881997738ed1d6dd3db2f8917ab8563");