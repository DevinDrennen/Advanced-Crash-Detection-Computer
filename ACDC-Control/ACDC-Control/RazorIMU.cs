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

        const int BYTES_PER_FLOAT = 4, FLOATS_PER_TRANS = 9, CHUNK_SIZE = BYTES_PER_FLOAT * FLOATS_PER_TRANS;
        private float[] data;

        public bool Initilized { get; private set; }

        public float[] Data
        {
            get { return data; }
            private set { data = value; }
        }

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
            data = new float[FLOATS_PER_TRANS];
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

            if(!razorIMU.IsOpen)
                razorIMU.Open();                                    // Open port for communication
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

        /// <summary>
        /// 
        /// </summary>
        private void readStream()
        {
            int bytesRead = 0;
            byte[] streamChunk = new byte[CHUNK_SIZE];
            string dataString;

            while(razorIMU.IsOpen)
            {
                streamChunk[bytesRead] = (byte)razorIMU.ReadByte();
                bytesRead++;

                if(bytesRead == CHUNK_SIZE)
                {
                    bytesRead = 0;
                    convertData(streamChunk);

                    dataString = "";
                    for (int i = 0; i < FLOATS_PER_TRANS; i++)
                        dataString += ((int)data[i]).ToString("D4") + ", ";
                    Debug.Print(dataString);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamChunk"></param>
        private void convertData(byte[] streamChunk)
        {
            int floatIndex = 0;
            byte[] floatChunk = new byte[4];

            lock(data)
            {
                for(int i = 0; i < CHUNK_SIZE; i += BYTES_PER_FLOAT)
                {
                    floatChunk[0] = streamChunk[i];
                    floatChunk[1] = streamChunk[i + 1];
                    floatChunk[2] = streamChunk[i + 2];
                    floatChunk[3] = streamChunk[i + 3];
                    data[floatIndex] = BitConverter.ToSingle(floatChunk);
                    floatIndex++;
                }
            }
        }
    }
}
