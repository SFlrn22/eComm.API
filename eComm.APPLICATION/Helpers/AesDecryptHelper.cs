using System.Security.Cryptography;
using System.Text;

namespace eComm.PERSISTENCE.Helpers
{
    public class EncryptionHelper
    {
        public static string Decrypt(string cipheredtext, byte[] key, byte[] iv)
        {
            string simpletext = String.Empty;
            byte[] buffer = Convert.FromBase64String(cipheredtext);
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            simpletext = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return simpletext;
        }
        public static string Sha256Hash(string text)
        {
            byte[] hash;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
            }
            return Convert.ToBase64String(hash);
        }
    }
}
