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
        const string JSON_URL = "https://s3.amazonaws.com/Minecraft.Download/versions/versions.json";
        
        /// <summary>
        /// Get the latest Minecraft version from official sources online
        /// </summary>
        /// <returns></returns>
        public static string GetLatesturl()
        {
            var wc = new WebClient();
            var json = wc.DownloadString(new Uri(JSON_URL));
            var response = JsonConvert.DeserializeObject<dynamic>(json);
            return response.latest.release;
        }

    }
}
