using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Molinos.DataAgro.Entities.Helpers
{
    public class AES
    {
        public static byte[] GetPasswordBytes()
        {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("kjsahjhduywewytqaJNSBKASKDIURE25631Asadhhj"));
        }

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] salt = passwordBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.BlockSize = 128;
                    Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                    rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(checked((int)Math.Round(unchecked((double)rijndaelManaged.KeySize / 8.0))));
                    rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(checked((int)Math.Round(unchecked((double)rijndaelManaged.BlockSize / 8.0))));
                    rijndaelManaged.Mode = CipherMode.CBC;
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cryptoStream.Close();
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] salt = passwordBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.BlockSize = 128;
                    Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                    rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(checked((int)Math.Round(unchecked((double)rijndaelManaged.KeySize / 8.0))));
                    rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(checked((int)Math.Round(unchecked((double)rijndaelManaged.BlockSize / 8.0))));
                    rijndaelManaged.Mode = CipherMode.CBC;
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cryptoStream.Close();
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        public static string Encrypt(string text, byte[] passwordBytes)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] randomBytes = AES.GetRandomBytes(AES.GetSaltSize(passwordBytes));
            byte[] bytesToBeEncrypted = new byte[checked(randomBytes.Length + (bytes.Length - 1) + 1)];
            int num1 = 0;
            int num2 = checked(randomBytes.Length - 1);
            int index1 = num1;
            while (index1 <= num2)
            {
                bytesToBeEncrypted[index1] = randomBytes[index1];
                checked { ++index1; }
            }
            int num3 = 0;
            int num4 = checked(bytes.Length - 1);
            int index2 = num3;
            while (index2 <= num4)
            {
                bytesToBeEncrypted[checked(index2 + randomBytes.Length)] = bytes[index2];
                checked { ++index2; }
            }
            return Convert.ToBase64String(AES.AES_Encrypt(bytesToBeEncrypted, passwordBytes));
        }

        public static string Decrypt(string decryptedText, byte[] passwordBytes)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(decryptedText);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] numArray = AES.AES_Decrypt(bytesToBeDecrypted, passwordBytes);
            int saltSize = AES.GetSaltSize(passwordBytes);
            byte[] bytes = new byte[checked(numArray.Length - saltSize - 1 + 1)];
            int num1 = saltSize;
            int num2 = checked(numArray.Length - 1);
            int index = num1;
            while (index <= num2)
            {
                bytes[checked(index - saltSize)] = numArray[index];
                checked { ++index; }
            }
            return Encoding.UTF8.GetString(bytes);
        }

        public static int GetSaltSize(byte[] passwordBytes)
        {
            byte[] bytes = new Rfc2898DeriveBytes(passwordBytes, passwordBytes, 1000).GetBytes(2);
            StringBuilder stringBuilder = new StringBuilder();
            int num1 = 0;
            int num2 = checked(bytes.Length - 1);
            int index1 = num1;
            while (index1 <= num2)
            {
                stringBuilder.Append(Convert.ToInt32(bytes[index1]).ToString());
                checked { ++index1; }
            }
            int num3 = 0;
            string str = stringBuilder.ToString();
            int index2 = 0;
            int length = str.Length;
            while (index2 < length)
            {
                int int32 = Convert.ToInt32(str[index2].ToString());
                checked { num3 += int32; }
                checked { ++index2; }
            }
            return num3;
        }

        public static byte[] GetRandomBytes(int length)
        {
            byte[] data = new byte[checked(length - 1 + 1)];
            RandomNumberGenerator.Create().GetBytes(data);
            return data;
        }
    }
}
