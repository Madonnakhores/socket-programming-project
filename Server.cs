using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 1000);
            serverSocket.Bind(iep);
        }
        public void StartServer()
        {
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("server open");
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    byte[] RecievedData = new byte[1024]; //request as a byte array
                    // TODO: Receive request
                    int recievedLength = clientSocket.Receive(RecievedData);
                    // TODO: break the while loop if receivedLen==0
                    if (recievedLength == 0)
                    {
                        Console.WriteLine("Server closed");
                        break;
                    }
                    string recievedRequest = Encoding.ASCII.GetString(RecievedData); //request as a string
                    // TODO: Create a Request object using received request string
                    Request request = new Request(recievedRequest);
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    //byte[] responseInBytes = Encoding.ASCII.GetBytes(response.ToString());
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }
        Response HandleRequest(Request request)
        {
            string content;
            try
            {
                //TODO: check for bad request
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", content,null);
                }
                string relative = request.relativeURI.Substring(1);
                //TODO: check for redirect
                if(Configuration.RedirectionRules.ContainsKey(relative))
                {
                    string redirected_path = Configuration.RootPath + "/" + Configuration.RedirectionRules[relative];
                    if (File.Exists(redirected_path))
                    {
                        content = LoadDefaultPage(GetRedirectionPagePathIFExist(relative));
                        return new Response(StatusCode.Redirect, "text/html", content, Configuration.RedirectionRules[relative]);
                    }
                    else
                    {
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                        return new Response(StatusCode.NotFound, "text/html", content, null);
                    }
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string path = Configuration.RootPath + request.relativeURI;
                //TODO: check file exists
                //TODO: read the physical file
                // Create OK response
                if (File.Exists(path))
                {
                    content = LoadDefaultPage(relative);
                    return new Response(StatusCode.OK, "text/html", content, null);
                }
                else
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", content, null);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError,"text/html",content,null);
            }
        }
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if(Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RootPath + Configuration.RedirectionRules[relativePath];
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            string content;
            try
            {
            StreamReader sr = File.OpenText(filePath);
            content = sr.ReadToEnd();
            sr.Close();
            return content;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
        }

        private void LoadRedirectionRules(string filePath)//Path of Redirection rules file
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file
                string[] lines;
                lines = File.ReadAllLines(filePath);
                // then fill Configuration.RedirectionRules dictionary
                char[] comma = new char[] { ',' };
                Configuration.RedirectionRules = new Dictionary<string, string>();
                for (int i = 0; i <lines.Length;i++)
                {
                    string[] redirectionRule;
                    redirectionRule = lines[i].Split(comma, StringSplitOptions.None);
                    Configuration.RedirectionRules.Add(redirectionRule[0], redirectionRule[1]);
                }           
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}