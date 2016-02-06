using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DashboardApp.Models
{
    /// <summary>
    /// Re-used for Welcome screen and config View
    /// </summary>
    class JarSelector
    {
        List<string> jarList;

        public void Search()
        {
            jarList = new List<string>();
            
            DirectoryInfo info = new DirectoryInfo(@System.Environment.CurrentDirectory);
            FileInfo[] dirs = info.GetFiles("*.jar", SearchOption.TopDirectoryOnly).OrderBy(p => p.CreationTime).ToArray();

            foreach (var i in dirs)
            {
               
                jarList.Add(i.Name);
            }
        }

        public List<string> GetList()
        {
            return jarList;
        }

        public bool IsJarAvailable()
        {
            return (jarList.Count > 0);
        }
    }
}
