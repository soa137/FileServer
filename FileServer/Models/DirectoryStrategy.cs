using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileService
{
    public interface IDirectoryStrategy
    {
        string DefinePath(Stream stream, string filepath);
    }


    public class DefaultDirectoryStrategy:IDirectoryStrategy
    {
        public string DefinePath(Stream stream, string filepath)
        {
            return filepath;
        }
        
    }
    public class HashDirectoryStrategy: IDirectoryStrategy
    {
        private int levels;

        public HashDirectoryStrategy(int levels)
        {
            this.levels = levels;

        }


        private string GetMD5Hash(Stream stream)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                stream.Position = 0;
                byte[] data = md5Hash.ComputeHash(stream);
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();

            }
        }

        public string DefinePath(Stream stream, string filepath)
        {
            string s = GetMD5Hash(stream);

            for (var i = 0; i < levels; i++)
            {
                // 2 5 8 11
                s = s.Insert(3 * i + 2, "/");
            }
            return s;
        }
    }
}
