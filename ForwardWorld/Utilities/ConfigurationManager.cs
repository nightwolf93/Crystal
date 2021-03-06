﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Utilities
{
    public static class ConfigurationManager
    {
        public static Dictionary<string, string> ConfigurationElements = new Dictionary<string, string>();

        public static void LoadConfiguration()
        {
            ConfigurationElements.Clear();
            foreach (string parameter in ConfigurationSettings.AppSettings)
            {
                ConfigurationElements.Add(parameter, ConfigurationSettings.AppSettings[parameter]);
            }
            Utilities.ConsoleStyle.Infos("Configuration loaded with " + ConfigurationElements.Count + " elements !");
        }

        public static int GetIntValue(string name)
        {
            return int.Parse(ConfigurationElements[name]);
        }

        public static bool GetBoolValue(string name)
        {
            return bool.Parse(ConfigurationElements[name]);
        }

        public static string GetStringValue(string name)
        {
            return ConfigurationElements[name];
        }
    }
}
