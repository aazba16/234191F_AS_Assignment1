using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace _234191F_AS_Assignment1.Helpers
{
    public class EncryptionHelper
    {
        // AES-256 Key should be 32 bytes (for AES-256)
        private static readonly string Key = "ThisIsASecretKeyWith32BytesLong123"; // 32-byte key for AES-256
        private static readonly string IV = "ThisIsAnInitVect16"; // 16-byte IV for AES

        // Encrypt method for credit card number or any sensitive data
        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);  // AES Key
                aes.IV = Encoding.UTF8.GetBytes(IV);    // AES IV (must be 16 bytes)
                aes.Mode = CipherMode.CBC; // CBC Mode is more secure
                aes.Padding = PaddingMode.PKCS7; // PKCS7 padding for proper byte alignment

                // Create an encryptor object using the key and IV
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(plainText); // Write the plain text (credit card number) into the stream
                    writer.Flush();
                    cs.FlushFinalBlock(); // Finish the encryption process
                    return Convert.ToBase64String(ms.ToArray()); // Return the encrypted text as Base64 string
                }
            }
        }

        // Decrypt method to decrypt the encrypted credit card number
        public static string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);  // AES Key
                aes.IV = Encoding.UTF8.GetBytes(IV);    // AES IV (must be 16 bytes)
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Create a decryptor object using the key and IV
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText))) // Convert Base64 to byte array
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd(); // Read and return the decrypted text
                }
            }
        }
    }
}
