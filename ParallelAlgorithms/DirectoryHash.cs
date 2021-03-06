﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ParallelAlgorithms
{
    internal class DirectoryHash
    {
        public string JustFunc(string x)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] hash = Encoding.ASCII.GetBytes(x);
                byte[] hashenc = md5.ComputeHash(hash);
                string result = "";
                foreach (byte b in hashenc)
                {
                    result += b.ToString("x2");
                }
                return result;
            }
        }

        public string GetHash(string root)
        {
            StringBuilder sb = new StringBuilder();
            string[] dirs, files;
            try
            {
                dirs = Directory.GetDirectories(root, "*", SearchOption.TopDirectoryOnly);
                files = Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly);
                List<string> hashes = new List<string>(new string[dirs.Length + files.Length]);
                Parallel.ForEach(Enumerable.Range(0, dirs.Length).ToList(),
                    index => hashes[index] = GetHash(dirs[index]));

                Parallel.ForEach(Enumerable.Range(0, files.Length).ToList(),
                    index => hashes[index + dirs.Length] = JustFunc(files[index]));
                hashes.ForEach(h => sb.Append(h));
            }
            catch
            {
            }
            return JustFunc(sb + JustFunc(root));
        }

        public string GetHashSimple(string root)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (string directory in Directory.GetDirectories(root, "*", SearchOption.TopDirectoryOnly))
                {
                    sb.Append(GetHashSimple(directory));
                }
                foreach (string file in Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly))
                {
                    sb.Append(JustFunc(file));
                }
            }
            catch
            {
            }
            return JustFunc(sb + JustFunc(root));
        }

        public void Test(string root = "C:\\Program Files")
        {
            Program.CountTime(() => GetHash(root), "HashParallel");
            Program.CountTime(() => GetHashSimple(root), "HashSimple");
        }
    }
}