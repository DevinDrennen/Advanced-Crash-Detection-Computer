using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Collections;
using Microsoft.SPOT.Net.NetworkInformation;
using System.Net;
using System.Threading;

namespace ACDC_Control.WebServer
{
    /// <summary>
    /// Class to display data and or statuses about the system via the web through the wifi connection
    /// </summary>
    /// <remarks>
    /// Will likely change this class a lot as the interface is developed.
    /// </remarks>
    public static class WebDisplay
    {
        static string jQScript;

        /// <summary>
        /// Listener which waits for web requests from clients
        /// </summary>
        private static Listener webListener;

        /// <summary>
        /// One of the strings to test the displaying of data
        /// </summary>
        public static string DataString { get; private set; }

        /// <summary>
        /// Begin the web display so web responses are answered with the webpage
        /// </summary>
        /// <param name="port">The port on which we will accept a connection to the web display</param>
        public static void Initialize(int port = 80, int updatePeriod = 100)
        {
            // Set up the listener to wait for requests from clients on <port>
            // Also set up the event to be triggered when a request is received.
            webListener = new Listener(port);
            webListener.ReceivedRequest += WebListener_ReceivedRequest;

            jQScript = "<script>" +
                            "setInterval(function(){ imuDataUpdate() }, " + updatePeriod + ");" +
                            "function imuDataUpdate(){" +
                                "$.get(\"imuData.html\", function(data){" +
                                    "document.getElementById(\"imuDataText\").innerHTML = data;" +
                                "});" +
                            "}" +
                        "</script>";
            DataString = "";
        }

        /// <summary>
        /// Triggered when the listender gets a request from a client.
        /// This is triggered to allow adding content to the response.
        /// </summary>
        /// <param name="request"></param>
        private static void WebListener_ReceivedRequest(Request request)
        {
            if (request.URL != "/imuData.html")
            {
                request.SendResponse(
                    "<!DOCTYPE html>" +
                    "<html>" +
                    "<head>" +
                    "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js\"></script>" +
                    "<title>ACDC</title>" +
                    "</head>" +
                    "<h2>IMU Data:</h2>" +
                    "<p id=\"imuDataText\"></p>" +
                    jQScript +
                    "</body>" +
                    "</html>"
                    );
            }
            else
                lock(DataString)
                    request.SendResponse(DataString);
        }

        public static void FormatIMUDataString(float[] data)
        {
            DataString = "";
            foreach (float val in data)
                DataString += ((int)val).ToString("D4") + "</br>";
        }
    }
}
