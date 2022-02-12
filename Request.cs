using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get
            {
                return headerLines;
            }
        }
        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
            headerLines = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] separtors = new string[] { "\r\n" };
            requestLines = requestString.Split(separtors, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3) //count
            {
                return false;
            }
            // Parse Request line
            if (!ParseRequestLine()) {
                return false;
            }
            if (!ValidateIsURI(this.relativeURI))
            {
                return false;
            }
            if (!ValidateBlankLine()) {
                return false;
            }
            if (!LoadHeaderLines()) {
                return false;
            }
            return true;
        }

        private bool ParseRequestLine()
        {
            string[] requestLine = new string[3]; //Contain Method, URI and HTTP version 
            requestLine = requestLines[0].Split(' ');

            //Check Method
            if (!requestLine[0].Equals("GET"))
            {
                return false;
            }

            this.relativeURI = requestLine[1];
            //check URI (Valid or Not)
            if (requestLine[2] == "HTTP/1.0")
                this.httpVersion = HTTPVersion.HTTP10;

            else if (requestLine[2] == "HTTP/1.1")
                this.httpVersion = HTTPVersion.HTTP11;

            else if (requestLine[2] == "HTTP/0.9" || requestLine[2] ==""|| requestLine[2] == null)
                this.httpVersion = HTTPVersion.HTTP09;
            else
                return false;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {

            string[] colon = new string[] { ":" };
            try
            {
                for (int i = 1; i < requestLines.Length - 2; i++)
                {
                    string[] headerline;
                    headerline = requestLines[i].Split(colon, StringSplitOptions.None);
                    headerLines.Add(headerline[0], headerline[1]);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            return requestLines.Contains(string.Empty);
        }
    }
}