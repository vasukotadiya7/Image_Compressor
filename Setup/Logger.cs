using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setup
{
    public static class Logger
    {
        public static void Log(string msg)
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ImageCompressor");
            Directory.CreateDirectory(dir);

            File.AppendAllText(Path.Combine(dir, "installer.log"), $"[{DateTime.Now}] {msg}\n");
        }
    }
}
