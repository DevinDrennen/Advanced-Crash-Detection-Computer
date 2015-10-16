using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ACDC_Control.WebServer
{

    public delegate void RequestReceived(Request request);

    /// <summary>
    /// Listener instance. Listens to requests on a socket.
    /// </summary>
    public class Listener : IDisposable
    {
        const int MAX_REQUEST_SIZE = 1024;

        public event RequestReceived ReceivedRequest = delegate { };
        // Socket used for listening
        private Socket listeningSocket = null;

        /// <summary>
        /// Are we currently listening for requests?
        /// </summary>
        public bool Listening
        {
            get;
            private set;
        }

        /// <summary>
        /// Has the listener stopped listening?
        /// </summary>
        public bool Stopped
        {
            get;
            private set;
        }

        /// <summary>
        /// Port on which listener is listening for a request
        /// </summary>
        public int Port
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Listener object constructor.
        /// </summary>
        /// <param name="port">Port on which we want to listen.</param>
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

        /// <summary>
        /// Listener object deconstructor.
        /// </summary>
        ~Listener()
        {
            Dispose();
        }

        /// <summary>
        /// Start continous listening.
        /// Waits, as long as we don't stop it, for requests.
        /// </summary>
        public void Listen()
        {
            while (Listening)
            {
                // Create a client socket to connect to a client which sends a request to the listening socket
                using (Socket clientSocket = listeningSocket.Accept())
                {
                    int availableBytes = clientSocket.Available;
                    int bytesReceived = (availableBytes > MAX_REQUEST_SIZE ? MAX_REQUEST_SIZE : availableBytes);

                    // Get the IP of the client and the remote endpoint
                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    var x = clientSocket.RemoteEndPoint;
                    
                    if (bytesReceived > 0)
                    {
                        byte[] buffer = new byte[bytesReceived];
                        int readByteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);

                        // Create a request object to pass on the triggered event that a request has been received
                        using (Request req = new Request(clientSocket, Encoding.UTF8.GetChars(buffer)))
                            ReceivedRequest(req);
                    }
                }

                // Wait a tad to prevent lock up
                Thread.Sleep(10);
            }

            // If we stopped listening, indicate so
            Stopped = true;
        }

        /// <summary>
        /// Stop the continious listening
        /// </summary>
        public void Stop()
        {
            // Signal that we should not listen
            Listening = false;
        }

        /// <summary>
        /// Method to properly dispose of listener
        /// </summary>
        public void Dispose()
        {
            if (Stopped && listeningSocket != null)
                listeningSocket.Close();
        }
    }
}
