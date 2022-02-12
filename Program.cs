using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateRedirectionRulesFile();
            Server server = new Server(1000, @"C:\inetpub\redirectionRules.txt");
            server.StartServer();
        }
        // TODO: Create file named redirectionRules.txt
        // each line in the file specify a redirection rule
        // example: "aboutus.html,aboutus2.html"
        // means that when making request to aboustus.html,, it redirects me to aboutus2
        static void CreateRedirectionRulesFile()
        {
            string path = @"C:\inetpub\redirectionRules.txt";
            string []Rules = new string[] { "aboutus.html,aboutus2.html" };
            File.WriteAllLines(path, Rules);
        }
    }
}
