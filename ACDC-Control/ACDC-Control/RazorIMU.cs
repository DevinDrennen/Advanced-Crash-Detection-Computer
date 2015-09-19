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
        private byte[] syncByteArray, syncOKByteArray, syncOKBuffer;
        private int valueIndex = 0;
        private float[] _data;

        public bool Initiated { get; private set; }

        public float[] Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baudRate"></param>
        public RazorIMU(string port, int baudRate)
        {
            // Define SYNC related byte arrays, create port, and data array
            syncByteArray = Encoding.UTF8.GetBytes("#s12");
            syncOKByteArray = Encoding.UTF8.GetBytes("#SYNCH12\r\n");
            syncOKBuffer = new byte[syncOKByteArray.Length];
            razorIMU = new SerialPort(port, baudRate);
            _data = new float[9];
        }

        /// <summary>
        /// Reset the binary data stream to the start
        /// </summary>
        public void InitiateDataStream()
        {
            razorIMU.Open();                                        // Open port for communication
            razorIMU.Write(syncByteArray, 0, syncByteArray.Length); // Send sync signal

            while (razorIMU.BytesToRead < syncOKBuffer.Length)
                Thread.Sleep(2);

            razorIMU.Read(syncOKBuffer, 0, syncOKBuffer.Length);

            if (syncOKBuffer == syncOKByteArray)
            {
                razorIMU.DataReceived += RazorIMU_DataReceived;
                Initiated = true;
            }
            else
                Initiated = false;
        }

        /// <summary>
        /// <para>Data received from the Razor IMU.</para>
        /// <para>Data format: [accel x, accel y, accel z, mag x, mag y, mag z, gyro x, gyro y, gyro z] </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RazorIMU_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (int stored = razorIMU.BytesToRead; valueIndex < stored; valueIndex++)
            {
                if (valueIndex == 9)
                {
                    stored -= 9;
                    valueIndex = 0;
                }

                _data[valueIndex] = (byte)razorIMU.ReadByte();
            }
        }
    }
}
