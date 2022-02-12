using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        //static StreamWriter sr = new StreamWriter("log.txt");  //In Template

        static string path = @"C:\inetpub\log.txt";

        public static void LogException(Exception ex)
        {
            string[] content = new string[] { "DateTime: " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy") + "\n" + "Message: " + ex.Message };
            File.WriteAllLines(path, content);
        }
    }
}
