using System.IO;
using System.Security.Cryptography;
using System.Text;

public class MathTools
{
    public static string GetMD5Code(string path)
    {
        using(FileStream file = new FileStream(path, FileMode.Open))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5Code = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < md5Code.Length; i++)
            {
                sb.Append(md5Code[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
