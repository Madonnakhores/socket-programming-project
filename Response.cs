using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        public StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            headerLines.Add("Content-Type:" + contentType);
            headerLines.Add("Content-Length:" + content.Length);
            headerLines.Add("Date:" + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"));
            if (redirectoinPath != null)
            {
                headerLines.Add("Location:" + redirectoinPath);
            }
            responseString = GetStatusLine(code);
            responseString += "\r\n";
            for (int i = 0; i < headerLines.Count(); i++)
            {
                responseString += headerLines[i];
                responseString += "\r\n";
            }
            responseString += "\r\n";
            responseString += content;
        }
        private string GetStatusLine(StatusCode code)
        {
            string statusLine = string.Empty;
            string message = string.Empty;
            if (StatusCode.OK == code)
            {
                message = "Ok";
            }
            else if (StatusCode.InternalServerError == code)
            {
                message = "Internal Server Error";
            }
            else if (StatusCode.NotFound == code)
            {
                message = "Not Found";
            }
            else if (StatusCode.BadRequest == code)
            {
                message = "Bad Request";
            }
            else if (StatusCode.Redirect == code)
            {
                message = "Redirecting";
            }
            statusLine = Configuration.ServerHTTPVersion + " " + (int)code + " " + message;
            return statusLine;
        }
    }
}
