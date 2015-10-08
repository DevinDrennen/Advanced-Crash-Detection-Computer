using ACDC_Control.DSP.Types;
using ACDC_Control.IMU;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NetduinoGo;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ACDC_Control
{
    public class Program
    {
        static string csvPath = @"\SD\data.csv";
        static FileStream writer = new FileStream(csvPath, FileMode.Append);

        public static void Main()
        {
            //ComplexNumTests();
            MainTests();
        }

        /// <summary>
        /// Core set of tests.
        /// </summary>
        private static void MainTests()
        {
            NetworkInterface wifi = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            // Lets keep the button for reseting for now... 
            //InputPort buttonInput = new InputPort(Pins.ONBOARD_BTN, true, Port.ResistorMode.PullDown);
            RazorIMU imu = new RazorIMU(SerialPorts.COM1);
            OutputPort activityLED = new OutputPort(Pins.ONBOARD_LED, false);
            RgbLed lightOutput = new RgbLed();

            lightOutput.SetColor(255, 0, 0);
            lightOutput.SetBrightness(0.1);

            imu.InitializeDataStream();
            imu.DataProcessed += Imu_DataProcessed;
            while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
            {
                //Debug.Print("Waiting for IP...");
                activityLED.Write(!activityLED.Read());
                Thread.Sleep(1000);
            }

            Debug.Print("Got IP: " + wifi.IPAddress);
            lightOutput.SetColor(0, 255, 0);
        }

        private static void Imu_DataProcessed(float[] data)
        {
            string dataLine = data[0] + ", " + data[1] + ", " + data[2] + ", " + data[3] + ", "
                            + data[4] + ", " + data[5] + ", " + data[6] + ", " + data[7] + "\n";
            byte[] buffer = Encoding.UTF8.GetBytes(dataLine);
            writer.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Test complex number type.
        /// </summary>
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

            /*
            Print out confirms proper functioning:

            Float 1 becomes: (1 + 0i)
            Float 0 becomes: (0 + 0i)
            Unit Complex is: (1 + 1i)
            The conjugate of (2 + 2i) is (2 + -2i)
            Magnitude and Phase of (2 + 2i) are 2.82842708 and 0.785398185 radians
            (2 + 2i) + (2 + -4i) is (4 + -2i)
            (5 + 5i) * 2 is (10 + 10i)
            (5 + 5i) / 5 is (1 + 1i)
            (5 + 5i) * (2 + 2i) is (0 + 20i) with phase 90.000002504478161 degrees
            (3 + 9i) + 3 is (6 + 9i)
            */
        }
    }
}
