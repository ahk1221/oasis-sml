using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OasisModLoader.Logging
{
    public class Logger
    {
        private static string path;

        public static void Initialize(string _path)
        {
            path = _path;
            File.Delete(path);
        }

        public static void Log(string line)
        {
            var sr = File.AppendText(path);
            sr.WriteLine("[{0}]: " + line, DateTime.UtcNow);

            sr.Flush();
            sr.Close();
        }
    }
}
