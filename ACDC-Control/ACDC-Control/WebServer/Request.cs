using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.IO;

namespace ACDC_Control.WebServer
{
    /// <summary>
    /// Request instance. Used to process and respond to client request.
    /// </summary>
    public class Request : IDisposable
    {
        /// <summary>
        /// Socket for client which sent a request
        /// </summary>
        private Socket client;

        const int FILE_BUFFER_SIZE = 256;

        /// <summary>
        /// Gets client IP address
        /// </summary>
        public IPAddress Client
        {
            get
            {
                if (client != null)
                {
                    IPEndPoint ip = client.RemoteEndPoint as IPEndPoint;

                    if (ip != null)
                        return ip.Address;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets request method (GET or POST)
        /// </summary>
        public string Method
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets URL of request
        /// </summary>
        public string URL
        {
            get;
            private set;
        }

        /// <summary>
        /// Request object constructor.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Data"></param>
        internal Request(Socket Client, char[] Data)
        {
            client = Client;
            ProcessRequest(Data);
        }

        /// <summary>
        /// Send a response back to the client
        /// </summary>
        /// <param name="response"></param>
        public void SendResponse(string response, string type = "text/html")
        {
            // If the client isn't null
            if (client != null)
            {
                // create HTTP response header
                string header = "HTTP/1.0 200 OK\r\nContent-Type: " + type + "; charset=utf-8\r\nContent-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";

                // Send header and other response string passed
                client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                client.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
            }
        }

        /// <summary>
        /// Processes a request from the client
        /// </summary>
        /// <param name="data"></param>
        private void ProcessRequest(char[] data)
        {
            string content = new string(data);
            string firstLine = content.Substring(0, content.IndexOf('\n'));

            // Parse the first line of the request: "GET /path/ HTTP/1.1"
            string[] words = firstLine.Split(' ');
            Method = words[0];
            URL = words[1];
        }

        /// <summary>
        /// Send a 404 "not found" response
        /// </summary>
        /// <remarks>
        /// Not used yet, but useful and might need it later.
        /// </remarks>
        public void Send404()
        {
            string header = "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";

            if (client != null)
                client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
        }

        /// <summary>
        /// Method to properly dispose of request object
        /// </summary>
        public void Dispose()
        {
            // If the client isn't null then close it and null it.
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
    }
}
