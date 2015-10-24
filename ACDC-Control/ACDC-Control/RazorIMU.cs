using System;
using System.IO.Ports;
using System.Threading;

namespace ACDC_Control.IMU
{
    public delegate void DataConverted();

    /// <summary>
    /// Razor IMU instance. Reads data from sensor on serial COM port.
    /// </summary>
    public class RazorIMU
    {
        const int BYTES_PER_FLOAT = 4, FLOATS_PER_TRANS = 9;

        public event DataConverted DataProcessed = delegate { };
        private SerialPort razorIMU;
        private Thread serialReadThread;

        private byte[][] dataBytes;
        private byte[] syncBytes;
        private float[] data, ypr;

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

        public float[] YPR
        {
            get { return ypr; }
            private set { ypr = value; }
        }

        /// <summary>
        /// Razor IMU object constructor. 
        /// </summary>
        /// <param name="port">Serial port on which this sensor resides</param>
        /// <param name="baudRate">Baud rate sensor is set up to use. Default set.</param>
        public RazorIMU(string port, int baudRate = 57600)
        {
            razorIMU = new SerialPort(port, baudRate);
            syncBytes = new byte[] { 127, 128, 0, 0 };
            dataBytes = new byte[FLOATS_PER_TRANS][];
            data = new float[FLOATS_PER_TRANS];
            ypr = new float[3];

            for (int i = 0; i < FLOATS_PER_TRANS; i++)
                dataBytes[i] = new byte[BYTES_PER_FLOAT];
        }

        /// <summary>
        /// Begin reading serial data from the sensor.
        /// </summary>
        /// <returns>True if initialized correctly, false otherwise.</returns>
        public bool StartReading()
        {
            // If the stream reader is alive, stop it!
            if (serialReadThread != null && serialReadThread.IsAlive)
                serialReadThread.Abort();

            // If port isn't open yet, open it.
            if(!razorIMU.IsOpen)
                razorIMU.Open();

            // Initialize stream reader and set state to initialized
            serialReadThread = new Thread(readStream);
            serialReadThread.Start();
            Initilized = true;

            return true;
        }

        /// <summary>
        /// As long as the port is open, read in data.
        /// </summary>
        private void readStream()
        {
            // Variables to keep track of data and data location in the byte array
            int  nextByte = -1, byteIndex = 0, floatIndex = 0;
            bool synced = false;

            // As long as the serial port is open
            while (razorIMU.IsOpen)
            {
                // Reset the indecies
                byteIndex = 0;
                floatIndex = 0;
                synced = false;

                // We wait until we're synced with the next set of values before proceeding
                while (!synced)
                {
                    // Wait for the first value of the sync signal (128)
                    while (nextByte != syncBytes[0])
                        nextByte = razorIMU.ReadByte();

                    // Check the following values to ensure it is the sync signal
                    for (int i = 1; i < syncBytes.Length; i++)
                    {
                        nextByte = razorIMU.ReadByte();
                        // If any of the values isn't right, fail sync and keep trying.
                        if (nextByte != syncBytes[i])
                        {
                            synced = false;
                            break;
                        }

                        synced = true;
                    }
                }

                /* This algorithm fills a 2d array of bytes by filling 9 rows of 4 bytes
                 * In other words, it is grouped such that each row contains the 4 bytes for a given float.
                */

                // While there are more rows to fill...
                while(floatIndex < FLOATS_PER_TRANS)
                {
                    // While there are more cells within this row to fill...
                    while (byteIndex < BYTES_PER_FLOAT)
                    {
                        // Read the nect byte
                        nextByte = razorIMU.ReadByte();

                        // If read was successful
                        if (nextByte != -1)
                        {
                            // Store the byte in this cell, and increase to the next cell in this row.
                            dataBytes[floatIndex][byteIndex] = (byte)nextByte;
                            byteIndex++;
                        }
                    }

                    // Row filled, increase floatIndex to next row and reset the cell to 0 again
                    byteIndex = 0;
                    floatIndex++;
                }

                // Grid filled, convert bytes to floats!
                convertData();
            }
        }

        /// <summary>
        /// Convert the byte array to the floats represented byte each 4 bytes.
        /// </summary>
        private void convertData()
        {
            /*
             * This method extracts each row from the grid of bytes
             * and uses the pre-grouped bytes to get the floats.
             * It also converts the sensor values to euler angles.
            */
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

                // Calculate yaw, pitch, roll (ypr). ypr is in the form [yaw, pitch, roll]
                ypr[0] = 0;
                ypr[1] = 0;
                ypr[2] = 0;

                // Fire the event, data convertion finished!
                DataProcessed();
            }
        }
    }
}
