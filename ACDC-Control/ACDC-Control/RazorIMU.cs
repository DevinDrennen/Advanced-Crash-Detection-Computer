using Microsoft.SPOT;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ACDC_Control
{
    /// <summary>
    /// Razor IMU instance
    /// </summary>
    public class RazorIMU
    {
        private SerialPort razorIMU;
        private Thread serialReadThread;
        private byte[] syncByteArray, syncOKByteArray, syncOKBuffer;

        const int BYTES_PER_FLOAT = 4, FLOATS_PER_TRANS = 9, CHUNK_SIZE = BYTES_PER_FLOAT * FLOATS_PER_TRANS;
        private float[] data;

        /// <summary>
        /// Indicates if communication has been initilized between the sensor and netduino.
        /// </summary>
        public bool Initilized { get; private set; }

        /// <summary>
        /// Current data chunk from sensor.
        /// [accel x, accel y, accel z, magno x, magno y, magno z, gyro x, gyro y, gyro z]
        /// </summary>
        public float[] Data
        {
            get { return data; }
            private set { data = value; }
        }

        /// <summary>
        /// Razor IMU object constructor. 
        /// </summary>
        /// <param name="port">Serial port on which this sensor resides</param>
        /// <param name="baudRate">Baud rate sensor is set up to use. Default set.</param>
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
        /// Reset the binary data stream to the start.
        /// </summary>
        /// <returns>True if initiated correctly, false otherwise.</returns>
        public bool InitializeDataStream()
        {
            int nextByte = 0;
            int bytesRead = 0;
            Initilized = false;

            // If the stream reader is alive, stop it!
            if (serialReadThread != null && serialReadThread.IsAlive)
                serialReadThread.Abort();

            // If port isn't open yet, open it.
            if(!razorIMU.IsOpen)
                razorIMU.Open();
            // Send sync signal
            razorIMU.Write(syncByteArray, 0, syncByteArray.Length); 

            // Wait for # part of the sync response signal
            while (nextByte != '#')
            {
                // Lets not wait forever, 
                // after a full frame, its time to stop and report an error.
                if (bytesRead == 37) 
                    break;

                // Read in the next byte and increase how many have been read.
                nextByte = razorIMU.ReadByte();
                bytesRead++;
            }

            // Validate sync signal a byte at a time, if fails report problem.
            for (int i = 1; i < syncOKByteArray.Length; i++)
            {
                nextByte = razorIMU.ReadByte();
                if (nextByte != syncOKByteArray[i])
                {
                    // Report problem, close port, and return a fail
                    Debug.Print("Problem connecting to Razor IMU.");
                    razorIMU.Close();
                    return false;
                }
            }

            // On success initialize stream reader and set state to initialized
            serialReadThread = new Thread(new ThreadStart(readStream));
            serialReadThread.Start();
            Initilized = true;

            return true;
        }

        /// <summary>
        /// As long as the port is open, read in data.
        /// </summary>
        private void readStream()
        {
            // Variables to keep track of and store data
            int bytesRead = 0;
            byte[] streamChunk = new byte[CHUNK_SIZE];
            string dataString;

            while(razorIMU.IsOpen)
            {
                // Store the next byte read into the next spot on the array and increase how many bytes have been read.
                streamChunk[bytesRead] = (byte)razorIMU.ReadByte();
                bytesRead++;

                // If CHUNK_SIZE (36) bytes have been read
                if(bytesRead == CHUNK_SIZE)
                {
                    // Reset the counter and convert the array
                    bytesRead = 0;
                    convertData(streamChunk);

                    // This is debugging stuff to see what the array looks like
                    dataString = "";
                    for (int i = 0; i < FLOATS_PER_TRANS; i++)
                        dataString += ((int)data[i]).ToString("D4") + ", ";
                    Debug.Print(dataString);
                }
            }
        }

        /// <summary>
        /// Convert the byte array to the floats represented byte each 4 bytes.
        /// </summary>
        /// <param name="streamChunk"></param>
        private void convertData(byte[] streamChunk)
        {
            int floatIndex = 0;
            byte[] floatChunk = new byte[4];

            lock(data)
            {
                // Extract 4 bytes at a time to make a float
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
