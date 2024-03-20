using System.Security.Cryptography;
using System.Text;

namespace Applivery.Desktop.Comics.Logic
{
    public static class Utils
    {
        public static string MD5Hash(string text)
        {
            // https://www.thegeekyway.com/md5-hashing-using-items/

            MD5 md5H = MD5.Create();
            //convert the input string to a byte array and compute its hash
            byte[] data = md5H.ComputeHash(Encoding.UTF8.GetBytes(text));
            // create a new stringbuilder to collect the bytes and create a string
            StringBuilder sB = new StringBuilder();
            //loop through each byte of hashed data and format each one as a hexadecimal string
            for (int i = 0; i < data.Length; i++)
            {
                sB.Append(data[i].ToString("x2"));
            }
            //return hexadecimal string
            return sB.ToString();
        }
    }
}