using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DashboardApp.Utils
{
    class ServerJarfile
    {
        public static string GetLatesturl()
        {
            var wc = new WebClient();
            const string url = "https://s3.amazonaws.com/Minecraft.Download/versions/versions.json";
            var json = wc.DownloadString(new Uri(url));

            //
            var response = JsonConvert.DeserializeObject<dynamic>(json);
            return response.latest.release;
        }

    }
}
