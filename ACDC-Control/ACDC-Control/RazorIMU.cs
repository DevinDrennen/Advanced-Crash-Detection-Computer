using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ACDC_Control.IMU
{
    public delegate void DataConverted(float[] data);

    /// <summary>
    /// Razor IMU instance
    /// </summary>
    public class RazorIMU
    {
        public event DataConverted DataProcessed = delegate { };
        private SerialPort razorIMU;
        private Thread serialReadThread;

        private byte[] syncByteArray, syncOKByteArray, syncOKBuffer;
        private byte[][] dataBytes;

        const int BYTES_PER_FLOAT = 4, FLOATS_PER_TRANS = 9, TRANS_SIZE = BYTES_PER_FLOAT * FLOATS_PER_TRANS;
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

            dataBytes = new byte[FLOATS_PER_TRANS][];

            for (int i = 0; i < FLOATS_PER_TRANS; i++)
                dataBytes[i] = new byte[BYTES_PER_FLOAT];
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
                if (bytesRead == 72) 
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
                    // Close port, and return a fail
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
            // Variables to keep track of place in array to store data.
            int floatIndex = 0, byteIndex = 0, nextByte;

            // As long as the serial port is open
            while(razorIMU.IsOpen)
            {
                // Read the next byte
                nextByte = razorIMU.ReadByte();

                // If its not -1 (timeout, no byte to read)
                if (nextByte != -1)
                {
                    // Store it in the next place of the 2D jagged array,
                    dataBytes[floatIndex][byteIndex] = (byte)nextByte;
                    byteIndex++; // then increase the 2nd dimension (bytes) index.

                    // If the (4) bytes have been put in place, for the current float, 
                    
                    if (byteIndex == BYTES_PER_FLOAT)
                    {
                        // then reset the 2nd dimension (byte) index and increase the 1st dimension (float) index
                        byteIndex = 0;
                        floatIndex++;

                        // If all the float's bytes have been filled in,
                        if (floatIndex == FLOATS_PER_TRANS)
                        {
                            // then reset the 1st dimension (float) index and convert the data.
                            floatIndex = 0;
                            convertData();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert the byte array to the floats represented byte each 4 bytes.
        /// </summary>
        private void convertData()
        {
            lock(data)
            {
                    // Acceleration
                    data[0] = BitConverter.ToSingle(dataBytes[0]);
                    data[1] = BitConverter.ToSingle(dataBytes[1]);
                    data[2] = BitConverter.ToSingle(dataBytes[2]);

                    // Magnometer
                    data[3] = BitConverter.ToSingle(dataBytes[3]);
                    data[4] = BitConverter.ToSingle(dataBytes[4]);
                    data[5] = BitConverter.ToSingle(dataBytes[5]);

                    // Gyroscope
                    data[6] = BitConverter.ToSingle(dataBytes[6]);
                    data[7] = BitConverter.ToSingle(dataBytes[7]);
                    data[8] = BitConverter.ToSingle(dataBytes[8]);

                DataProcessed(data);
            }
        }
    }
}
