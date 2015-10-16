using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Collections;
using Microsoft.SPOT.Net.NetworkInformation;
using System.Net;
using System.Threading;

namespace ACDC_Control.WebServer
{
    public static class WebDisplay
    {
        private static Listener webListener;

        public static void Initialize(int port = 80)
        {
            webListener = new Listener(port);
            webListener.ReceivedRequest += WebListener_ReceivedRequest;
        }

        private static void WebListener_ReceivedRequest(Request request)
        {
            request.SendResponse(
                "<html>" +
                "<header><title>ACDC</title></header>" +
                "<body>" +
                "Hello!" +
                "</body>" +
                "</html>"
                );
        }
    }
}
