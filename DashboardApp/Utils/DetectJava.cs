using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashboardApp.Utils
{
    class DetectJava
    {
        /// <summary>
        /// Attempt to find the install path of whatever java version is installed
        /// This method should work for 99% of people using this software :)
        /// </summary>
        /// <returns></returns>
        static string GuessJavaVersion()
        {
            string e = FindJavaPath();
            string r = null;

            if (e.Contains("\\Java\\jre1.8")) {
                r = "JRE 8";
            } else if (e.Contains("\\Java\\jre7"))
            {
                r = "JRE 7";
            } else if (e.Contains("\\Java\\jre6"))
            {
                r = "JRE 6";
            }

            if (r == null) {
                // return error string (TODO: translation)
                return "Java installation not detected.";
            } else {
                // append arch of Java
                if (!System.Environment.Is64BitOperatingSystem) { r += " x86"; }
                    else if (e.Contains("\\Program Files (x86)\\")) { r += " x86"; }
                    else if (e.Contains("\\Program Files\\")) { r += " x64"; }
                return r;
            }
        }

        /// <summary>
        /// Find the path to the Java installation, if available.
        /// </summary>
        /// <returns>File path for Java if installed, else null.</returns>
        public static string FindJavaPath()
        {
            // http://stackoverflow.com/questions/17821960/best-way-to-find-java-path-in-c-sharp
            string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
            RegistryKey baseKey;
            string result;

            // if Windows is 64-bit, check for a x64 Java installation.
            if (System.Environment.Is64BitOperatingSystem)
            {
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaKey);
                result = ScanForJRE(baseKey);
                if (result != null) return result;
            }

            // otherwise, look for x86 Java.
            baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(javaKey);
            result = ScanForJRE(baseKey);
            return result;

        }

        private static string ScanForJRE(RegistryKey baseKey)
        {
            if (baseKey == null) return null;

            string currentVersion = baseKey.GetValue("CurrentVersion", null).ToString();
            if (currentVersion == null) {
                return null;
            }

            using (RegistryKey homeKey = baseKey.OpenSubKey(currentVersion))
            {
                return homeKey.GetValue("JavaHome", "").ToString();
            }
        }
       
    }
}
