using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using IniParser;
using IniParser.Model;

namespace DashboardApp.Models
{
    public class ServerProperties : System.ComponentModel.INotifyPropertyChanged
    {
        FileIniDataParser parser;
        IniData data;

        static string FILE = "server.properties";

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerProperties()
        {
            parser = new FileIniDataParser();
            parser.Parser.Configuration.CommentString = "#";
            Load();
        }

        public void Load()
        {
            data = parser.ReadFile(FILE);
        }

        public void Save()
        {
            if (data != null)
                parser.WriteFile(FILE, data);
        }

        public string GetValue(string Key)
        {
            if (data != null)
            {
                return data?[""][Key];
            }
            else
            {
                return "";
            }

        }


        public string serverport { get { return m_serverport(); } } // XXX Not working
        public string m_serverport()
        {
            var x = GetValue("server-port");
            if (x != "")
            {
                return x;
            } else
            {
                return "25565";
            }
        }

        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}