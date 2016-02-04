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
        static PWM _pin = new PWM(PWMChannels.PWM_PIN_D10, 0, .5, false);
        /// <summary>
        /// The buzzer will buzz at frequency f for time t.
        /// </summary>
        /// <param name="f">Frequency, in hertz, the buzzer will buzz at.</param>
        /// <param name="t">How long, in seconds, the buzzer will buzz.</param>
        static void buzz(int f, int t)
        {
            _pin.Frequency = (uint)f;

            Thread doBuzz = new Thread(new ThreadStart(_pin.Start));

            doBuzz.Start();
            Thread.Sleep(t * 1000);
            _pin.Stop();
            doBuzz.Join();
            
        }

    }
}

