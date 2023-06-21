using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NotebookShop.Classes
{
    public class Encryption
    {
        public static string Encrypt(string text/*, string keyString*/)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            string key = "U3s9Jjst4V0SRnTUPjR3SDF";

            try
            {
                var buffer = Encoding.UTF8.GetBytes(text);
                var hash = new SHA512CryptoServiceProvider();
                var aesKey = new byte[24];
                Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

                using (var aes = Aes.Create())
                {
                    if (aes == null)
                        return null;

                    aes.Key = aesKey;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var resultStream = new MemoryStream())
                    {
                        using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                        using (var plainStream = new MemoryStream(buffer))
                        {
                            plainStream.CopyTo(aesStream);
                        }

                        var result = resultStream.ToArray();
                        var combined = new byte[aes.IV.Length + result.Length];
                        Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
                        Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);

                        return Convert.ToBase64String(combined);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Decrypt(string encryptedText/*, string keyString*/)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return null;

            string key = "U3s9Jjst4V0SRnTUPjR3SDF";

            try
            {
                var combined = Convert.FromBase64String(encryptedText);
                var buffer = new byte[combined.Length];
                var hash = new SHA512CryptoServiceProvider();
                var aesKey = new byte[24];
                Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

                using (var aes = Aes.Create())
                {
                    if (aes == null)
                        return null;

                    aes.Key = aesKey;

                    var iv = new byte[aes.IV.Length];
                    var ciphertext = new byte[buffer.Length - iv.Length];

                    Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
                    Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);

                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var resultStream = new MemoryStream())
                    {
                        using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                        using (var plainStream = new MemoryStream(ciphertext))
                        {
                            plainStream.CopyTo(aesStream);
                        }

                        return Encoding.UTF8.GetString(resultStream.ToArray());
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string CalculateMD5Hash(string text)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(text);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                //To make the hex string use lower-case letters instead of upper-case, replace the single line inside the for loop with this line
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}