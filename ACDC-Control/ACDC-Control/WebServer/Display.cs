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
        public static string ACDC_Status { get; private set; }
        
        /// <summary>
        /// Boolean signifying if the web display is initialized
        /// </summary>
        public static bool Initialized { get; private set; }

        /// <summary>
        /// Begin the web display so web responses are answered with the webpage
        /// </summary>
        /// <param name="port">The port on which we will accept a connection to the web display</param>
        public static void Initialize(int port = 80, int updatePeriod = 200)
        {
            // Set up some needed strings before allowing web resqests!
            ACDC_Status = "";
            jQScript = "<script>" +
                            "setInterval(function(){ updateStatus() }, " + updatePeriod + ");" +
                            "function updateStatus(){" +
                                "$.get(\"acdcStatus\", function(data, status){" +
                                    "document.getElementById(\"acdcStatus\").innerHTML = data;" +
                                "});" +
                            "}" +
                        "</script>";
            
            // Set up the listener to wait for requests from clients on <port>
            // Also set up the event to be triggered when a request is received.
            webListener = new Listener(port);
            webListener.ReceivedRequest += WebListener_ReceivedRequest;
            Initialized = true;
        }

        /// <summary>
        /// Triggered when the listender gets a request from a client.
        /// This is triggered to allow adding content to the response.
        /// </summary>
        /// <param name="request"></param>
        private static void WebListener_ReceivedRequest(Request request)
        {
            switch (request.URL)
            {
                case "/":
                    request.SendResponse(
                        "<!DOCTYPE html>" +
                        "<html>" +
                        "<head>" +
                        "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js\"></script>" +
                        "<title>ACDC Status</title>" +
                        "</head>" +
                        "<div id=\"acdcStatus\" style=\"margin - bottom:0px; font - family: 'Courier New', Courier, monospace\" ></div>" +
                        jQScript +
                        "</body>" +
                        "</html>"
                        );
                    break;
                case "/acdcStatus":
                    lock (ACDC_Status)
                    {
                        request.SendResponse(ACDC_Status);
                    }
                    break;
                default:
                    break;
            }
        }

        public static void UpdateString(double[] data, bool loggingEnabled, string logFile, string status)
        {
            lock (ACDC_Status)
            {
                if(loggingEnabled)
                    ACDC_Status = "<p> <h3>Recordinng:</h3> " + logFile + "</p>";
                else
                    ACDC_Status = "<p> <h3>Saved to:</h3> " + logFile + "</p>";

                ACDC_Status += "<p> <h3>Status:</h3> </br>" + status + "</p>";

                ACDC_Status += "<p> <h3>Sensor Data:</h3> </br>";
                ACDC_Status +=  "A X: " + (int)data[0] + "</br>" +
                                "A Y: " + (int)data[1] + "</br>" +
                                "A Z: " + (int)data[2] + "</p>";
            }
        }
    }
}
