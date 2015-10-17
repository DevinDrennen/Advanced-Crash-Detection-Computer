using ACDC_Control.DSP.Types;
using ACDC_Control.IMU;
using ACDC_Control.WebServer;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NetduinoGo;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Net;
using System.Threading;

namespace ACDC_Control
{
    public class Program
    {
        //static FileStream writer = new FileStream(csvPath, FileMode.Append);
        static string csvPath = @"\SD\data.csv";

        public static void Main()
        {
            //ComplexNumTests();
            //VectorTests();
            MatrixTests(); 

            //MainTests();
        }

        /// <summary>
        /// Core set of tests.
        /// </summary>
        private static void MainTests()
        {
            RazorIMU imu = new RazorIMU(SerialPorts.COM1);
            OutputPort activityLED = new OutputPort(Pins.ONBOARD_LED, false);
            
            // Start reading data from the sensor and set up event to receive converted/pprocessed data.
            imu.StartReading();
            imu.DataProcessed += Imu_DataProcessed;

            // Start the web data display
            WebDisplay.Initialize();

            // Loop forever to keep the code running
            while (true)
            {
                // Just plash the on board led to let us know code hasn't crashed
                activityLED.Write(!activityLED.Read());
                Thread.Sleep(1000);
            }
        }

        private static void Imu_DataProcessed(float[] data)
        {
            // Here we can show how the even is being fired and the
            // the data prints to the debug window when calculated.
            string dataString = "";
            for (int i = 0; i < 9; i++)
                dataString += ((int)data[i]).ToString("D4") + ", ";

            // Update the sensor data for the web display to show.
            /* This has been tested and shown to work on a custom subnet while
             * we wait to properly set up a connect in the school's network. 
            */
            WebDisplay.DataString = dataString;

            // Print the datastring to the debug output
            Debug.Print(dataString);
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
