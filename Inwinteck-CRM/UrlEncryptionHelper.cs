using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Routing;
using System.Configuration;

public static class UrlEncryptionHelper
{
    private static readonly string EncryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];

    //public static string Encrypt(string plainText)
    //{
    //    if (string.IsNullOrEmpty(EncryptionKey))
    //        throw new InvalidOperationException("Encryption key not found.");

    //    using (Aes aes = Aes.Create())
    //    {
    //        aes.Key = Convert.FromBase64String(EncryptionKey);
    //        aes.GenerateIV();
    //        var iv = aes.IV;

    //        using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
    //        using (var memoryStream = new MemoryStream())
    //        {
    //            memoryStream.Write(iv, 0, iv.Length);

    //            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
    //            using (var streamWriter = new StreamWriter(cryptoStream))
    //            {
    //                streamWriter.Write(plainText);
    //            }

    //            var encrypted = memoryStream.ToArray();
    //            return Convert.ToBase64String(encrypted);
    //        }
    //    }
    //}

    //public static string Decrypt(string cipherText)
    //{
    //    if (string.IsNullOrEmpty(EncryptionKey))
    //        throw new InvalidOperationException("Encryption key not found.");

    //    var fullCipher = Convert.FromBase64String(cipherText);

    //    using (Aes aes = Aes.Create())
    //    {
    //        var iv = new byte[16];
    //        var cipher = new byte[fullCipher.Length - iv.Length];

    //        Array.Copy(fullCipher, iv, iv.Length);
    //        Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

    //        aes.Key = Convert.FromBase64String(EncryptionKey);
    //        aes.IV = iv;

    //        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
    //        using (var memoryStream = new MemoryStream(cipher))
    //        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
    //        using (var streamReader = new StreamReader(cryptoStream))
    //        {
    //            return streamReader.ReadToEnd();
    //        }
    //    }
    //}

    private static readonly byte[] Key = EnsureKeySize(Encoding.UTF8.GetBytes(EncryptionKey));

    private static byte[] EnsureKeySize(byte[] key)
    {
        if (key.Length == 16 || key.Length == 24 || key.Length == 32)
            return key;

        // Ensure key is 32 bytes long, for AES-256 (change to 16 or 24 if needed)
        byte[] resizedKey = new byte[32];
        Array.Copy(key, resizedKey, Math.Min(key.Length, resizedKey.Length));
        return resizedKey;
    }

    public static string Encrypt(int Id)
    {
        string plainText = Id.ToString();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.GenerateIV();

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                }

                return Convert.ToBase64String(ms.ToArray())
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');
            }
        }
    }

    public static int Decrypt(string encryptedParameter)
    {
        // Ensure correct padding before converting from Base64
        encryptedParameter = encryptedParameter.Replace('-', '+').Replace('_', '/');
        switch (encryptedParameter.Length % 4)
        {
            case 2: encryptedParameter += "=="; break;
            case 3: encryptedParameter += "="; break;
        }

        byte[] encryptedBytes = Convert.FromBase64String(encryptedParameter);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;

            byte[] iv = new byte[aes.BlockSize / 8];
            Array.Copy(encryptedBytes, iv, iv.Length);
            aes.IV = iv;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length);
                    cs.FlushFinalBlock();
                }

                string decryptedText = Encoding.UTF8.GetString(ms.ToArray());
                return int.Parse(decryptedText);
            }
        }
    }
}

