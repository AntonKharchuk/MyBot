using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyBot
{
    public static class Constants
    {
        public static string MyBotDirectory { get; private set; }
        public static string LocalhostAddres { get; private set; }

        static Constants()
        {
            MyBotDirectory = findMyDir(new FileInfo(Assembly.GetExecutingAssembly().Location).FullName);
            //LocalhostAddres = @"https://localhost:44311/";
            LocalhostAddres = @"https://localhost:5001/";
        }
        private static string findMyDir(string path)
        {
            if (path.EndsWith("MyBot\\"))
            {
                return path;
            }
            else
                return findMyDir(path.Remove(path.Length - 1));
        }
    }
}
