using System.Security.Cryptography;

namespace eComm.PERSISTENCE.Helpers
{
    public class AesDecryptHelper
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
    }
}
