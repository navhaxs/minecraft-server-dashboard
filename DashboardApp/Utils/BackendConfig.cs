using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DashboardApp.Utils
{
    public class BackendConfig
    {
        public static List<string> GetListofJarfile()
        {
            var filenames = new List<string>();
            string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.jar", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in files)
                filenames.Add(Path.GetFileName(file));

            return filenames;
        }

        public string SelectedJarfile { get; set; }
    }
}
