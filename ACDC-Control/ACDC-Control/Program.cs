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
        public static void Main()
        {
            NetworkInterface wifi = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            // Lets keep the button for reseting for now... 
            //InputPort buttonInput = new InputPort(Pins.ONBOARD_BTN, true, Port.ResistorMode.PullDown);
            RazorIMU imu = new RazorIMU(SerialPorts.COM1, 57600);
            OutputPort activityLED = new OutputPort(Pins.ONBOARD_LED, false);
            RgbLed lightOutput = new RgbLed();

            lightOutput.SetColor(255, 0, 0);
            lightOutput.SetBrightness(0.1);

            imu.InitiateDataStream();        
            
            while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
            {
                Debug.Print("Waiting for IP...");
                Debug.Print("Razor IMU Initiated: " + imu.Initiated);
                activityLED.Write(!activityLED.Read());
                Thread.Sleep(1000);
            }

            Debug.Print("Got IP: " + wifi.IPAddress);
            lightOutput.SetColor(0, 255, 0);
        }
    }
}
