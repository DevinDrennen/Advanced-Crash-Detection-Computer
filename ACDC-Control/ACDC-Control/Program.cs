using ACDC_Control.IMU;
using ACDC_Control.Math.Types;
using ACDC_Control.WebServer;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ACDC_Control
{
    public class Program
    {
        static Thread mainThread;

        static RazorIMU IMU;
        static OutputPort activityLED;
        static InterruptPort usrButton = new InterruptPort(Pins.GPIO_PIN_D8, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);

        static FileStream writer;
        static string csvPath = @"\SD\data_"; // base path
        static byte[] buffer;
        static bool enableLogging = true;

        public static void Main()
        {
            mainThread = Thread.CurrentThread;

            activityLED = new OutputPort(Pins.ONBOARD_LED, false);
            
            if (enableLogging)
            {
                int i = 0;
                while (File.Exists(csvPath + i + ".csv"))
                    i++;

                csvPath += i + ".csv";
                writer = new FileStream(csvPath, FileMode.Append);

                buffer = Encoding.UTF8.GetBytes("A X,A Y,A Z,M X,M Y,M Z,G X,G Y,G Z,,Yaw,Pitch,Roll \n");
                writer.Write(buffer, 0, buffer.Length);
            }
                     
            //ComplexNumTests();
            //VectorTests();
            //MatrixTests(); 
            InitializeACDC();

            mainThread.Priority = ThreadPriority.BelowNormal;
            // Loop forever to keep the code running
            // flashing the on board led to let us know code hasn't crashed
            while (true)
            {
                activityLED.Write(!activityLED.Read() && enableLogging);
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Core system things.
        /// </summary>
        private static void InitializeACDC()
        {
            // Start reading data from the sensor and set up event to receive converted/pprocessed data.
            IMU = new RazorIMU(SerialPorts.COM1, 115200);
            IMU.StartReading();
            IMU.DataProcessed += Imu_DataProcessed;

            // Wait for internet connectivity then print the IP address of the netduino
            while (NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress == IPAddress.Any.ToString())
                Thread.Sleep(100);
            Debug.Print("NETDUINO 3 WIFI IP: " + NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);

            // Start the web display
            WebDisplay.Initialize();

            Buzzer.ReadyBuzz();
            Thread.Sleep(1000);
            Buzzer.AlertBuzz();
        }
                
        /// <summary>
        /// 
        /// </summary>
        private static void Imu_DataProcessed()
        {
            if (WebDisplay.Initialized)
                WebDisplay.UpdateString(IMU.Data, enableLogging, csvPath);

            if (enableLogging)
            {
                buffer = Encoding.UTF8.GetBytes(
                    IMU.Data[0] + "," +
                    IMU.Data[1] + "," +
                    IMU.Data[2] + "," +
                    IMU.Data[3] + "," +
                    IMU.Data[4] + "," +
                    IMU.Data[5] + "," +
                    IMU.Data[6] + "," +
                    IMU.Data[7] + "," +
                    IMU.Data[8] + ",," +
                    IMU.Data[9] + "," +
                    IMU.Data[10] + "," +
                    IMU.Data[11] + "\n");

                writer.Write(buffer, 0, buffer.Length);
                writer.Flush();

                if (!usrButton.Read())
                {
                    writer.Close();
                    writer.Dispose();
                    enableLogging = false;
                }
            }
        }
        
        #region Type Tests

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
            Matrix A = new Matrix(new double[][]{
                new double[]{ 1, 2},
                new double[]{ 3, 4},
                new double[]{ 5, 6},
                new double[]{ 7, 8}
            }, 4, 2);

            Matrix B = new Matrix(new double[][]{
                new double[]{ 10, 11},
                new double[]{ 12, 13},
            }, 2, 2);

            Matrix B2 = new Matrix(new double[][]{
                new double[]{ 3, 4},
                new double[]{ 13, 14},
            }, 2, 2);

            Matrix C = new Matrix(new double[][]{
                new double[]{ 10, 15, 20},
                new double[]{ -30, -40, -50},
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

        #endregion
    }
}
