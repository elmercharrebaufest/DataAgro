using Molinos.DataAgro.Entities.Helpers;
using System.Text;
/*
using System.DirectoryServices.AccountManagement;
*/
namespace WebDataAgro.Core
{
    public static class Util
    {
        //------------------------------------------------------------------------------------------------
        //  Metodos Publicos
        //------------------------------------------------------------------------------------------------
        private static byte[] GetPasswordBytes()
        {
            var key = "sadhgj6123hhdajdkqjnzqfjlka7Z23";

            var ba = Encoding.UTF8.GetBytes(key);

            return System.Security.Cryptography.SHA256.Create().ComputeHash(ba);
        }

        public static string EncryptData(string text)
        {
            return AES.Encrypt(text, GetPasswordBytes());
        }

        public static string DecryptString(string text)
        {
            return AES.Decrypt(text, GetPasswordBytes());
        }

        public static string GetDownloadKey(string key)
        {
            var enc = EncryptData(key);

            var res = System.Web.HttpUtility.UrlEncode(enc);

            return res;
        }        
    }
}