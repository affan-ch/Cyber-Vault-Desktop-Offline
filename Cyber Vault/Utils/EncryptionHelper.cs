using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Cyber_Vault.Utils;

internal class EncryptionHelper
{
    // Encrypts a string using the Master Key
    public static string Encrypt(string data, string pin)
    {
        if(data == string.Empty || data == null || pin == string.Empty || pin == null)
        {
            return string.Empty;
        }
        using var aesAlg = Aes.Create();
        aesAlg.KeySize = 256; // Set the key size explicitly for AES-256
        aesAlg.BlockSize = 128; // Set the block size explicitly
        aesAlg.Key = Encoding.UTF8.GetBytes(pin.PadRight(32, '0')[..32]); // Use a 32-byte key for AES-256

        aesAlg.GenerateIV();
        var iv = aesAlg.IV;

        aesAlg.Mode = CipherMode.CFB; // Cipher Feedback Mode
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
        if(cipherText == string.Empty || pin == string.Empty || cipherText == null || pin == null)
        {
            return string.Empty; 
        }

        var iv = Convert.FromBase64String(cipherText).Take(16).ToArray();
        var cipherBytes = Convert.FromBase64String(cipherText).Skip(16).ToArray();

        using var aesAlg = Aes.Create();
        aesAlg.KeySize = 256; // Set the key size explicitly for AES-256
        aesAlg.BlockSize = 128; // Set the block size explicitly
        aesAlg.Key = Encoding.UTF8.GetBytes(pin.PadRight(32, '0')[..32]);
        aesAlg.IV = iv;
        aesAlg.Mode = CipherMode.CFB;
        aesAlg.Padding = PaddingMode.None; // No padding

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipherBytes);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }

    public static byte[]? DocumentEncrypt(byte[] data, string pin)
    {
        if ( data == null || pin == string.Empty || pin == null)
        {
            return null;
        }
        using var aesAlg = Aes.Create();
        aesAlg.KeySize = 256; // Set the key size explicitly for AES-256
        aesAlg.BlockSize = 128; // Set the block size explicitly
        aesAlg.Key = Encoding.UTF8.GetBytes(pin.PadRight(32, '0')[..32]); // Use a 32-byte key for AES-256

        aesAlg.GenerateIV();
        var iv = aesAlg.IV;

        aesAlg.Mode = CipherMode.CFB; // Cipher Feedback Mode
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
        return ivAndCiphertext;
       
    }




}
