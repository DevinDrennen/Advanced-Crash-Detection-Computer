using System;
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
    static class GoLED
    {
        public static bool flashing { get; set; } //If the LED is flashing, set this to false to stop it.

        static NetduinoGo.RgbLed led = new NetduinoGo.RgbLed();

        /// <summary>
        /// Flashes a green light to let the user know the system is ready.
        /// </summary>
        static void OnLight()
        {
            Thread onFlash = new Thread(new ThreadStart(OnLightWorker));
            onFlash.Start();
        }

        /// <summary>
        /// Seperate thread for the green flash.
        /// </summary>
        static void OnLightWorker()
        {
            led.SetColor(0, 255, 0); //Green
            led.SetBrightness(.75);
            Thread.Sleep(250);
            led.SetBrightness(0);
        }

        /// <summary>
        /// When started, flashing becomes true and will continue to flash until flashing is set to false.
        /// </summary>
        static void WarningLight()
        {
            Thread warningFlash = new Thread(new ThreadStart(WarningLightWorker));
            warningFlash.Start();
        }

        /// <summary>
        /// Seperate thread to flash the light.
        /// </summary>
        static void WarningLightWorker()
        {
            //International Orange (Aerospace)
            led.SetColor(255, 79, 0);

            
            flashing = true;

            //Will continue to flash until flashing is false.
            while (flashing)
            {
                led.SetBrightness(1); //Light goes on for .25 seconds
                Thread.Sleep(250);    //Then goes back off for .25 seconds
                led.SetBrightness(0);
                Thread.Sleep(250);
            }
        }
    }
}
