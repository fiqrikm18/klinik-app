using System.Security.Cryptography;
using System.Text;

namespace admin.Utils
{
    public static class Encryptor
    {
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(Encoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            var result = md5.Hash;

            var strBuilder = new StringBuilder();
            for (var i = 0; i < result.Length; i++)
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));

            return strBuilder.ToString();
        }
    }
}