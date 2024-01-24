﻿using System.Security.Cryptography;
using System.Text;

namespace Cyber_Vault.Utils;

internal class EncyptionHelper
{
    // Encrypts a string using the Master Key
    public static string Encrypt(string data, string pin)
    {
        using var aesAlg = Aes.Create();
        aesAlg.KeySize = 128; // Set the key size explicitly
        aesAlg.BlockSize = 128; // Set the block size explicitly
        aesAlg.Key = Encoding.UTF8.GetBytes(pin.PadRight(16, '0')[..16]);

        aesAlg.GenerateIV();
        var iv = aesAlg.IV;

        aesAlg.Mode = CipherMode.CFB;
        aesAlg.Padding = PaddingMode.None; // No padding

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(data);
        }

        // Concatenate IV and ciphertext
        var ivAndCiphertext = iv.Concat(msEncrypt.ToArray()).ToArray();
        return Convert.ToBase64String(ivAndCiphertext);
    }


    // Decrypts a string using the Master Key
    public static string Decrypt(string cipherText, string pin)
    {
        var iv = Convert.FromBase64String(cipherText).Take(16).ToArray();
        var cipherBytes = Convert.FromBase64String(cipherText).Skip(16).ToArray();

        using var aesAlg = Aes.Create();
        aesAlg.KeySize = 128; // Set the key size explicitly
        aesAlg.BlockSize = 128; // Set the block size explicitly
        aesAlg.Key = Encoding.UTF8.GetBytes(pin.PadRight(16, '0').Substring(0, 16));
        aesAlg.IV = iv;
        aesAlg.Mode = CipherMode.CFB;
        aesAlg.Padding = PaddingMode.None; // No padding

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipherBytes);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }

}