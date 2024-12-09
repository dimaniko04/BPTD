using System.Security.Cryptography;

namespace Lab1.Util;

public enum KeyLenght
{
    Aes128 = 128,
    Aes192 = 192,
    Aes256 = 256
}

public static class Aes
{
    private const int Nb = 4;

    private static readonly byte[,] SBox =
    {
        { 0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76 },
        { 0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0 },
        { 0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15 },
        { 0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75 },
        { 0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84 },
        { 0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf },
        { 0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8 },
        { 0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2 },
        { 0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73 },
        { 0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb },
        { 0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79 },
        { 0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08 },
        { 0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a },
        { 0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e },
        { 0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf },
        { 0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16 }
    };

    private static readonly byte[,] InverseSBox =
    {
        { 0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb },
        { 0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb },
        { 0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e },
        { 0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25 },
        { 0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92 },
        { 0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84 },
        { 0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06 },
        { 0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b },
        { 0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73 },
        { 0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e },
        { 0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b },
        { 0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4 },
        { 0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f },
        { 0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef },
        { 0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61 },
        { 0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d }
    };

    private static readonly byte[] Rcon = [0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1B, 0x36];

    private static int GetNk(KeyLenght keyLenght) => (int)keyLenght / 32;

    private static int GetNr(KeyLenght keyLenght) => keyLenght switch
    {
        KeyLenght.Aes128 => 10,
        KeyLenght.Aes192 => 12,
        KeyLenght.Aes256 => 14,
        _ => throw new ArgumentOutOfRangeException(nameof(keyLenght), keyLenght, null)
    };

    private static void AddRoundKey(byte[,] state, byte[,] roundKey)
    {
        for (var i = 0; i < Nb; i++)
        {
            for (var j = 0; j < Nb; j++)
            {
                state[j, i] ^= roundKey[i, j];
            }
        }
    }

    private static byte[,] KeyExpansion(byte[,] key)
    {
        KeyLenght keyLenght = (KeyLenght)key.Length;
        int nr = GetNr(keyLenght);
        int nk = GetNk(keyLenght);

        int totalKeys = Nb * (nr + 1);

        var expandedKey = new byte[totalKeys, 4];

        for (var i = 0; i < nk; i++)
        {
            expandedKey[i, 0] = key[i, 0];
            expandedKey[i, 1] = key[i, 1];
            expandedKey[i, 2] = key[i, 2];
            expandedKey[i, 3] = key[i, 3];
        }

        for (var i = nk; i < totalKeys; i++)
        {
            byte[] temp = [expandedKey[i - 1, 0], expandedKey[i - 1, 1], expandedKey[i - 1, 2], expandedKey[i - 1, 3]];

            if (i % nk == 0)
            {
                temp = SubWord(RotateWord(temp));
                temp[0] ^= Rcon[i / nk - 1];
            }

            for (var j = 0; j < 4; j++)
            {
                expandedKey[i, j] = (byte)(expandedKey[i - nk, j] ^ temp[j]);
            }
        }

        return expandedKey;
    }

    private static byte[] RotateWord(byte[] word)
    {
        return [word[1], word[2], word[3], word[0]];
    }

    private static byte[] SubWord(byte[] word)
    {
        for (var i = 0; i < 4; i++)
        {
            var row = (word[i] >> 4) & 0x0F;
            var col = word[i] & 0x0F;

            word[i] = SBox[row, col];
        }

        return word;
    }

    private static byte[,] GetRoundKey(byte[,] expandedKey, int round)
    {
        var roundKey = new byte[Nb, Nb];
        for (var i = 0; i < Nb; i++)
        {
            for (var j = 0; j < Nb; j++)
            {
                roundKey[j, i] = expandedKey[round * Nb + i, j];
            }
        }

        return roundKey;
    }

    private static void SubBytes(byte[,] state)
    {
        for (var i = 0; i < Nb; i++)
        {
            for (var j = 0; j < Nb; j++)
            {
                var b = state[i, j];
                var row = (b >> 4) & 0x0F;
                var col = b & 0x0F;
                state[i, j] = SBox[row, col];
            }
        }
    }

    private static void InvSubBytes(byte[,] state)
    {
        for (var i = 0; i < Nb; i++)
        {
            for (var j = 0; j < Nb; j++)
            {
                var b = state[i, j];
                var row = (b >> 4) & 0x0F;
                var col = b & 0x0F;
                state[i, j] = InverseSBox[row, col];
            }
        }
    }

    private static void ShiftRows(byte[,] state)
    {
        byte temp;

        temp = state[1, 0];
        state[1, 0] = state[1, 1];
        state[1, 1] = state[1, 2];
        state[1, 2] = state[1, 3];
        state[1, 3] = temp;

        temp = state[2, 0];
        state[2, 0] = state[2, 2];
        state[2, 2] = temp;

        temp = state[2, 1];
        state[2, 1] = state[2, 3];
        state[2, 3] = temp;

        temp = state[3, 0];
        state[3, 0] = state[3, 3];
        state[3, 3] = state[3, 2];
        state[3, 2] = state[3, 1];
        state[3, 1] = temp;
    }

    private static void InvShiftRows(byte[,] state)
    {
        byte temp;

        temp = state[1, 3];
        state[1, 3] = state[1, 2];
        state[1, 2] = state[1, 1];
        state[1, 1] = state[1, 0];
        state[1, 0] = temp;

        temp = state[2, 2];
        state[2, 2] = state[2, 0];
        state[2, 0] = temp;

        temp = state[2, 3];
        state[2, 3] = state[2, 1];
        state[2, 1] = temp;

        temp = state[3, 1];
        state[3, 1] = state[3, 2];
        state[3, 2] = state[3, 3];
        state[3, 3] = state[3, 0];
        state[3, 0] = temp;
    }

    private static void MixColumns(byte[,] state)
    {
        for (var j = 0; j < Nb; j++)
        {
            var a0 = state[0, j];
            var a1 = state[1, j];
            var a2 = state[2, j];
            var a3 = state[3, j];

            state[0, j] = (byte)(GaloisMul(a0, 2) ^ GaloisMul(a1, 3) ^ a2 ^ a3);
            state[1, j] = (byte)(a0 ^ GaloisMul(a1, 2) ^ GaloisMul(a2, 3) ^ a3);
            state[2, j] = (byte)(a0 ^ a1 ^ GaloisMul(a2, 2) ^ GaloisMul(a3, 3));
            state[3, j] = (byte)(GaloisMul(a0, 3) ^ a1 ^ a2 ^ GaloisMul(a3, 2));
        }
    }

    private static void InvMixColumns(byte[,] state)
    {
        for (var j = 0; j < Nb; j++)
        {
            var a0 = state[0, j];
            var a1 = state[1, j];
            var a2 = state[2, j];
            var a3 = state[3, j];

            state[0, j] = (byte)(GaloisMul(a0, 14) ^ GaloisMul(a1, 11) ^ GaloisMul(a2, 13) ^ GaloisMul(a3, 9));
            state[1, j] = (byte)(GaloisMul(a0, 9) ^ GaloisMul(a1, 14) ^ GaloisMul(a2, 11) ^ GaloisMul(a3, 13));
            state[2, j] = (byte)(GaloisMul(a0, 13) ^ GaloisMul(a1, 9) ^ GaloisMul(a2, 14) ^ GaloisMul(a3, 11));
            state[3, j] = (byte)(GaloisMul(a0, 11) ^ GaloisMul(a1, 13) ^ GaloisMul(a2, 9) ^ GaloisMul(a3, 14));
        }
    }

    private static byte GaloisMul(byte a, byte b)
    {
        byte p = 0;
        for (var i = 0; i < 8; i++)
        {
            if ((b & 1) == 1)
            {
                p ^= a;
            }

            var highBitSet = (a & 0x80) == 0x80;
            a <<= 1;
            if (highBitSet)
            {
                a ^= 0x1b; // x^8 + x^4 + x^3 + x^1 + 1
            }

            b >>= 1;
        }

        return p;
    }

    private static byte[] PadByteArray(
        byte[] byteArray, int blockSize)
    {
        int paddingSize = blockSize - byteArray.Length % blockSize;

        byte[] paddedByteArray = new byte[byteArray.Length + paddingSize];
        Array.Copy(byteArray, 0, paddedByteArray, 0, byteArray.Length);
        
        for (int i = byteArray.Length; i < paddedByteArray.Length; i++)
        {
            paddedByteArray[i] = (byte)paddingSize;
        }

        return paddedByteArray;
    }

    private static byte[] RemoveByteArrayPadding(
        byte[] paddedByteArray, int blockSize)
    {
        byte paddingValue = paddedByteArray[^1];
        
        if (paddingValue < 1 || paddingValue > blockSize)
            return paddedByteArray;
        
        int originalLength = paddedByteArray.Length - paddingValue;
        byte[] originalByteArray = new byte[originalLength];
        
        Array.Copy(paddedByteArray, 0, originalByteArray, 0, originalLength);

        return originalByteArray;
    }

    private static byte[] NormalizeKey(byte[] key, int keyLenght)
    {
        byte[] normalizedKey = new byte[keyLenght];

        if (key.Length >= keyLenght)
        {
            Array.Copy(key, 0, normalizedKey, 0, keyLenght);
            return normalizedKey;
        }
        
        int paddingSize = keyLenght - key.Length;
        Array.Copy(key, 0, normalizedKey, 0, key.Length);
        
        for (int i = key.Length; i < keyLenght; i++)
        {
            normalizedKey[i] = (byte)paddingSize;
        }
        
        return normalizedKey;
    }
    
    private static byte[] GenerateIv(int blockSize)
    {
        var iv = new byte[blockSize];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(iv);

        return iv;
    }
    
    public static (byte[] ciphertext, byte[] iv) Encrypt(
        byte[] plaintext, 
        byte[] key,
        KeyLenght keyLenght)
    {
        const int blockSize = Nb * Nb;
        var iv = GenerateIv(blockSize);
        var paddedPlaintText = PadByteArray(plaintext, blockSize);
        var key2D = ConvertTo2DArray(NormalizeKey(key, (int)keyLenght));
        
        var ciphertext = new byte[paddedPlaintText.Length];
        var prevBlock = iv;
        
        for (var i = 0; i < paddedPlaintText.Length; i += blockSize)
        {
            var block = new byte[blockSize];
            Array.Copy(paddedPlaintText, i, block, 0, blockSize);

            for (var j = 0; j < blockSize; j++)
            {
                block[j] ^= prevBlock[j];
            }

            var encryptedBlock = EncryptBlock(ConvertTo2DArray(block), key2D);
            prevBlock = ConvertTo1DArray(encryptedBlock);

            Array.Copy(prevBlock, 0, ciphertext, i, blockSize);
        }

        return (ciphertext, iv);
    }

    public static byte[] Decrypt(
        byte[] ciphertext, 
        byte[] key, 
        KeyLenght keyLenght,
        byte[] iv)
    {
        const int blockSize = Nb * Nb;
        var prevBlock = PadByteArray(iv, blockSize);
        var paddedPlaintText = new byte[ciphertext.Length];
        var key2D = ConvertTo2DArray(NormalizeKey(key, (int)keyLenght));

        for (var i = 0; i < ciphertext.Length; i += blockSize)
        {
            var block = new byte[blockSize];
            Array.Copy(ciphertext, i, block, 0, blockSize);

            var decryptedBlock = DecryptBlock(ConvertTo2DArray(block), key2D);
            var decryptedBytes = ConvertTo1DArray(decryptedBlock);

            for (var j = 0; j < blockSize; j++)
                paddedPlaintText[i + j] = (byte)(decryptedBytes[j] ^ prevBlock[j]);

            prevBlock = block;
        }
        var plaintext = RemoveByteArrayPadding(paddedPlaintText, blockSize);

        return plaintext;
    }

    private static byte[,] ConvertTo2DArray(byte[] input)
    {
        var rows = input.Length / 4; 
        var output = new byte[rows, 4];
        for (var i = 0; i < input.Length; i++)
        {
            output[i / 4, i % 4] = input[i];
        }
        return output;
    }

    private static byte[] ConvertTo1DArray(byte[,] input)
    {
        var output = new byte[16];
        for (var i = 0; i < 16; i++)
        {
            output[i] = input[i / 4, i % 4];
        }
        return output;
    }

    private static byte[,] EncryptBlock(byte[,] plaintext, byte[,] key)
    {
        var nr = GetNr((KeyLenght)key.Length);
        var state = new byte[Nb, Nb];
        Array.Copy(plaintext, state, plaintext.Length);

        var roundKeys = KeyExpansion(key);

        AddRoundKey(state, GetRoundKey(roundKeys, 0));
        Console.WriteLine($"\nBit entropy after round 0: {CalculateBitEntropy(state)}");

        for (var round = 1; round < nr; round++)
        {
            SubBytes(state);
            ShiftRows(state);
            MixColumns(state);
            AddRoundKey(state, GetRoundKey(roundKeys, round));
            Console.WriteLine($"Bit entropy after round {round}: {CalculateBitEntropy(state)}");
        }

        SubBytes(state);
        ShiftRows(state);
        AddRoundKey(state, GetRoundKey(roundKeys, nr));
        Console.WriteLine($"Bit entropy after final round: {CalculateBitEntropy(state)}");

        return state;
    }

    private static byte[,] DecryptBlock(byte[,] ciphertext, byte[,] key)
    {
        var nr = GetNr((KeyLenght)key.Length);
        var state = new byte[Nb, Nb];
        Array.Copy(ciphertext, state, ciphertext.Length);

        var roundKeys = KeyExpansion(key);

        AddRoundKey(state, GetRoundKey(roundKeys, nr));
        Console.WriteLine($"\nBit entropy after round 0: {CalculateBitEntropy(state)}");

        for (var round = nr - 1; round >= 1; round--)
        {
            InvShiftRows(state);
            InvSubBytes(state);
            AddRoundKey(state, GetRoundKey(roundKeys, round));
            InvMixColumns(state);
            Console.WriteLine($"Bit entropy after round {round}: {CalculateBitEntropy(state)}");
        }

        InvShiftRows(state);
        InvSubBytes(state);
        AddRoundKey(state, GetRoundKey(roundKeys, 0));
        Console.WriteLine($"Bit entropy after final round: {CalculateBitEntropy(state)}");

        return state;
    }

    private static double CalculateBitEntropy(byte[,] data)
    {
        int zeroCount = 0;
        int oneCount = 0;
        int totalBits = data.GetLength(0) * data.GetLength(1) * 8;

        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                byte value = data[i, j];
                for (int k = 0; k < 8; k++)
                {
                    if ((value & (1 << k)) == 0)
                        zeroCount++;
                    else
                        oneCount++;
                }
            }
        }

        double pZero = (double)zeroCount / totalBits;
        double pOne = (double)oneCount / totalBits;

        double entropy = 0.0;
        if (pZero > 0)
            entropy -= pZero * Math.Log2(pZero);
        if (pOne > 0)
            entropy -= pOne * Math.Log2(pOne);

        return entropy;
    }
}