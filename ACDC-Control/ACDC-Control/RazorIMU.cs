using Microsoft.SPOT;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ACDC_Control
{
    /// <summary>
    /// 
    /// </summary>
    public class RazorIMU
    {
        private SerialPort razorIMU;
        private Thread serialReadThread;
        private byte[] syncByteArray, syncOKByteArray, syncOKBuffer;

        const int BYTES_PER_FLOAT = 4;
        private float[] data;

        public bool Initilized { get; private set; }

        public float[] Data { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baudRate"></param>
        public RazorIMU(string port, int baudRate = 57600)
        {
            // Define SYNC related byte arrays, create port, and data arrays
            syncByteArray = Encoding.UTF8.GetBytes("#s12");
            syncOKByteArray = Encoding.UTF8.GetBytes("#SYNCH12\r\n");
            syncOKBuffer = new byte[syncOKByteArray.Length];

            razorIMU = new SerialPort(port, baudRate);
            data = new float[9];
        }

        /// <summary>
        ///  Reset the binary data stream to the start
        /// </summary>
        /// <returns>True if initiated correctly, false otherwise.</returns>
        public bool InitializeDataStream()
        {
            int nextByte = 0;
            int bytesRead = 0;
            Initilized = false;

            razorIMU.Open();                                        // Open port for communication
            razorIMU.Write(syncByteArray, 0, syncByteArray.Length); // Send sync signal

            while (nextByte != '#')
            {
                if (bytesRead == 37)
                    break;

                nextByte = razorIMU.ReadByte();
                bytesRead++;
            }

            for (int i = 1; i < syncOKByteArray.Length; i++)
            {
                nextByte = razorIMU.ReadByte();
                if (nextByte != syncOKByteArray[i])
                {
                    Debug.Print("Problem connecting to Razor IMU.");
                    razorIMU.Close();
                    return false;
                }
            }

            serialReadThread = new Thread(new ThreadStart(readStream));
            serialReadThread.Start();

            Initilized = true;
            return true;
        }

       
        private void readStream()
        {
            int bytesRead = 0;
            byte[] streamChunk = new byte[data.Length * BYTES_PER_FLOAT];

            while(razorIMU.IsOpen)
            {
                while (razorIMU.BytesToRead == 0) ;
                streamChunk[bytesRead] = (byte)razorIMU.ReadByte();
                bytesRead++;
            }
        }
    }
}
