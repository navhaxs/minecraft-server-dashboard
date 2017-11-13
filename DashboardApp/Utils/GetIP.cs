using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace DashboardApp.Utils
{
    public static class GetIP
    {
        /// <summary>
        /// Create callback using lightmvvm message
        /// </summary>
        static string ipAddr = "";
        public static string GetExternalIP()
        {
            if (ipAddr != "") return ipAddr;

            var thread = new Thread(
                () =>
                {
                    lock (ipAddr)
                    {
                        ipAddr = FetchIP(); // Publish the return value
                    }
            });
            thread.Start();
            thread.Join();
        
            /*
            Dim myNetwork As New Microsoft.VisualBasic.Devices.Network
            Return myNetwork.IsAvailable
            */
            return ipAddr;    
        }


        private static string FetchIP()
        {
            try
            {

                var request = HttpWebRequest.Create("http://checkip.dyndns.org/");
                using (var response = request.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        ipAddr = stream.ReadToEnd();
                    }
                }

                var first = ipAddr.IndexOf("Address: ") + "Address: ".Length;
                var last = ipAddr.LastIndexOf("</body>");
                ipAddr = ipAddr.Substring(first, last - first);
            }
            catch (Exception)
            {
                throw new UnknownIPAddressException();
            }
            return ipAddr;
        }

        public class UnknownIPAddressException : Exception
        {
            public UnknownIPAddressException()
            {
            }

            public UnknownIPAddressException(string message)
                : base(message)
            {
            }

            public UnknownIPAddressException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}
