using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NetduinoGo;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Threading;

namespace ACDC_Control
{
    public class Program
    {
        static SerialPort razorAHRS = new SerialPort(SerialPorts.COM1, 57600);
        static byte[] syncByteArray = Encoding.UTF8.GetBytes("#s12");
        static byte[] syncOKByteArray = Encoding.UTF8.GetBytes("#SYNCH12\r\n");
        static int imuValIndex = 0;

        static float[] sensorData = new float[9];

        public static void Main()
        {
            NetworkInterface wifi = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            InputPort buttonInput = new InputPort(Pins.ONBOARD_BTN, true, Port.ResistorMode.PullDown);
            RgbLed lightOutput = new RgbLed();

            lightOutput.SetColor(255, 0, 0);
            lightOutput.SetBrightness(0.2);

            razorAHRS.Write(syncByteArray, 0, syncByteArray.Length);
            razorAHRS.DataReceived += RazorAHRS_DataReceived;

            while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
            {
                Debug.Print("Waiting for IP...");
                Thread.Sleep(3000);
            }

            Debug.Print("Got IP: " + wifi.IPAddress);
            lightOutput.SetColor(0, 255, 0);
        }

        private static void RazorAHRS_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (razorAHRS.CanRead)
            {
                
            }
        }
    }
}
