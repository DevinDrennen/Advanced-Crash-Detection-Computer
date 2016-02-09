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
    static class Buzzer
    {
        static PWM _pin = new PWM(PWMChannels.PWM_PIN_D10, 440, .5, false);
        public static bool buzzing { get; set; }

        /// <summary>
        /// Generates an alert buzz that alterantes between two tones.
        /// </summary>
        public static void AlertBuzz()
        {
            //Makes a new thread and starts it so we don't clog the main thread.
            Thread doBuzz = new Thread(new ThreadStart(AlertBuzzWorker));
            doBuzz.Start();
        }

        /// <summary>
        /// Creates a seperate thread to buzz until boolean Buzzing is set to false.
        /// </summary>
        private static void AlertBuzzWorker()
        {
            //Starts the buzzing.
            _pin.Start();

            //Sets Buzzing to be true.
            buzzing = true;

            //While the public boolean buzzing is true, it will continue
            //to alternate between two tones (A4 and A5).
            while (buzzing)
            {
                _pin.Frequency = 440;
                Thread.Sleep(500);
                _pin.Frequency = 880;
                Thread.Sleep(500);
            }

            //Stops the buzzing.
            _pin.Stop();
        }

        /// <summary>
        /// Generates a brief buzz to tell the user the device is ready.
        /// </summary>
        public static void ReadyBuzz()
        {
            //Makes a new thread and starts it so we don't clog the main thread.
            Thread doBuzz = new Thread(new ThreadStart(ReadyBuzzWorker));
            doBuzz.Start();
        }

        /// <summary>
        /// Creates a seperate thread to buzz for a quarter second.
        /// </summary>
        private static void ReadyBuzzWorker()
        {
            //Sets the pin's frequency to D5.
            _pin.Frequency = 587.33;

            //Starts the buzz, sleeps for a quarter second, and stops the buzz.
            _pin.Start();
            Thread.Sleep(250);
            _pin.Stop();
        }
    }
}

