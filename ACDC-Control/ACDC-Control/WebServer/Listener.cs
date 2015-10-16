using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ACDC_Control.WebServer
{

    public delegate void RequestReceived(Request request);

    public class Listener : IDisposable
    {
        const int MAX_REQUEST_SIZE = 1024;

        public event RequestReceived ReceivedRequest = delegate { };
        private Socket listeningSocket = null;

        /// <summary>
        /// 
        /// </summary>
        public bool Listening
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Stopped
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get;
            private set;
        }
        
        public Listener(int port = 80)
        {
            Port = port;

            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            listeningSocket.Listen(5);
            Listening = true;
            Stopped = false;

            new Thread(Listen).Start();
        }

        ~Listener()
        {
            Dispose();
        }

        public void Listen()
        {
            while (Listening)
            {
                using (Socket clientSocket = listeningSocket.Accept())
                {
                    int availableBytes = clientSocket.Available;
                    int bytesReceived = (availableBytes > MAX_REQUEST_SIZE ? MAX_REQUEST_SIZE : availableBytes);

                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    var x = clientSocket.RemoteEndPoint;
                    
                    if (bytesReceived > 0)
                    {
                        byte[] buffer = new byte[bytesReceived];
                        int readByteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);

                        using (Request req = new Request(clientSocket, Encoding.UTF8.GetChars(buffer)))
                            ReceivedRequest(req);
                    }
                }

                Thread.Sleep(10);
            }

            Stopped = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            Listening = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (Stopped && listeningSocket != null)
                listeningSocket.Close();
        }
    }
}
