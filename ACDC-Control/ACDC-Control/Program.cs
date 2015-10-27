using ACDC_Control.Math.Types;
using ACDC_Control.IMU;
using ACDC_Control.WebServer;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NetduinoGo;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;

namespace ACDC_Control
{
    public class Program
    {
        static Thread mainThread;
        static RazorIMU IMU;
        static OutputPort activityLED;

        static FileStream writer;
        static string csvPath = @"\SD\data_"; // base path
        static byte[] buffer;

        public static void Main()
        {
            mainThread = Thread.CurrentThread;

            activityLED = new OutputPort(Pins.ONBOARD_LED, false);

            // Wait for internet connectivity then print the IP address of the netduino
            while (NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress == IPAddress.Any.ToString())
                Thread.Sleep(100);
            Debug.Print("NETDUINO 3 WIFI IP: " + NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);

            int i = 0;
            while (File.Exists(csvPath + i + ".csv"))
                i++;
            csvPath += i + ".csv";
            writer = new FileStream(csvPath, FileMode.Append);

            //ComplexNumTests();
            //VectorTests();
            //MatrixTests(); 
            InitializeACDC();

            mainThread.Priority = ThreadPriority.BelowNormal;
            // Loop forever to keep the code running
            // flashing the on board led to let us know code hasn't crashed
            while (true)
            {
                activityLED.Write(!activityLED.Read());
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Core system things.
        /// </summary>
        private static void InitializeACDC()
        {
            // Start the web display
            WebDisplay.Initialize();

            // Start reading data from the sensor and set up event to receive converted/pprocessed data.
            IMU = new RazorIMU(SerialPorts.COM1, 115200);
            IMU.StartReading();
            IMU.DataProcessed += Imu_DataProcessed;
        }

        private static void Imu_DataProcessed()
        {
            buffer = Encoding.UTF8.GetBytes(IMU.Data[9] + "," + IMU.Data[10] + "," + IMU.Data[11] + "\n");
            WebDisplay.UpdateString(IMU.Data);

            writer.Write(buffer, 0, buffer.Length);
            writer.Flush();
        }

        /// <summary>
        /// Test complex number type.
        /// </summary>
        /// <remarks>
        /// PASSING 10/17/2015
        /// </remarks>
        private static void ComplexNumTests()
        {
            Complex c1 = 1, c2 = 0, c3 = Complex.Unit;
            Complex c4 = new Complex(2, 2);
            Complex c5 = new Complex(2, -4);
            Complex c6 = new Complex(5, 5);
            Complex c7 = new Complex(3, 9);

            Debug.Print("Float 1 becomes: " + c1);
            Debug.Print("Float 0 becomes: " + c2);
            Debug.Print("Unit Complex is: " + c3);
            Debug.Print("The conjugate of " + c4 + " is " + c4.Conjugate());
            Debug.Print("Magnitude and Phase of " + c4 + " are " + c4.Magnitude + " and " + c4.Phase + " radians");
            Debug.Print(c4 + " + " + c5 + " is " + (c4 + c5));
            Debug.Print(c6 + " * 2 is " + (c6 * 2));
            Debug.Print(c6 + " / 5 is " + (c6 / 5));
            Debug.Print(c6 + " * " + c4 + " is " + (c6 * c4) + " with phase " + ((c6 * c4).Phase * (180 / System.Math.PI)) + " degrees");
            Debug.Print(c7 + " + 3 is " + (c7 + 3));
        }

        /// <summary>
        /// Test vector type.
        /// </summary>
        private static void VectorTests()
        {
            Vector v0 = Vector.Zero, v1 = Vector.Unit;

            /*
            Write tests!
            */
        }

        /// <summary>
        /// Test matrix type.
        /// </summary>
        /// <remarks>
        /// PASSING 10/17/2015
        /// </remarks>
        private static void MatrixTests()
        {
            Matrix A = new Matrix(new float[][]{
                new float[]{ 1, 2},
                new float[]{ 3, 4},
                new float[]{ 5, 6},
                new float[]{ 7, 8}
            }, 4, 2);

            Matrix B = new Matrix(new float[][]{
                new float[]{ 10, 11},
                new float[]{ 12, 13},
            }, 2, 2);

            Matrix B2 = new Matrix(new float[][]{
                new float[]{ 3, 4},
                new float[]{ 13, 14},
            }, 2, 2);

            Matrix C = new Matrix(new float[][]{
                new float[]{ 10, 15, 20},
                new float[]{ -30, -40, -50},
            }, 2, 3);

            Matrix D = Matrix.CreateIdentityMatrix(3);

            Debug.Print("A * 2:\n" + (A * 2));
            Debug.Print("B + B2:\n" + (B + B2));
            Debug.Print("B - B2:\n" + (B - B2));
            Debug.Print("B * C:\n" + (B * C));
            Debug.Print("A Transpose:\n" + A.GetTranspose());
            Debug.Print("B Transpose:\n" + B.GetTranspose());
            Debug.Print("C Transpose:\n" + C.GetTranspose());
            Debug.Print("D:\n" + D);
        }
    }
}
