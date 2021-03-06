﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
namespace RemoteDesktopClient.Model
{
    static class ConfigEditor
    {
        public static List<Config> config { get; private set; } = new List<Config>();

        private static readonly string pathWithEnv = @"%USERPROFILE%\AppData\Local\RemoteServer\client.conf";
        public static void ReadConfig()
        {
            StreamReader sr = new StreamReader(Environment.ExpandEnvironmentVariables(pathWithEnv));
            string[] text = sr.ReadToEnd().Split('\n');
            sr.Close();
            List<Config> newConfig = new List<Config>();
            int i = 0;
            int lasti = 0;
            string remoteHost = "";
            int remotePort = 0;
            string remoteUser = "";
            string remotePass = "";
            string remoteName = "";
            List<int> alternativePorts = new List<int>();
            for (int line = 0; line < text.Length; line++)
            {
                var txt = text[line].Split('=');
                try
                {
                    if (txt[0][0] != '#')
                    {
                        i = int.Parse(txt[0].Split('.')[1]);
                    }
                } catch { MessageBox.Show("??"); }
               
                if (i != lasti) {
                    newConfig.Add(new Config(remoteHost, remotePort, remoteUser, remotePass, remoteName, alternativePorts));
                    lasti = i;
                    remoteHost = "";
                    remotePort = 0;
                    remoteUser = "";
                    remotePass = "";
                    remoteName = "";
                    alternativePorts = new List<int>();
                }
                try
                {
                    switch (txt[0].Split('.')[0])
                    {
                        case "remoteHost":
                            remoteHost = txt[1].Replace("\r", "");
                            break;
                        case "remotePort":
                            remotePort = int.Parse(txt[1]);
                            break;
                        case "remoteUser":
                            remoteUser = txt[1];
                            break;
                        case "remotePass":
                            remotePass = txt[1];
                            break;
                        case "remoteName":
                            remoteName = txt[1];
                            break;
                        case "AlternativePort":
                            alternativePorts.Add(int.Parse(txt[1]));
                            break;
                        case "#EndConfig":
                            newConfig.Add(new Config(remoteHost, remotePort, remoteUser, remotePass, remoteName, alternativePorts));
                            break;
                    }
                } catch {  }
            }
            EditConfig(newConfig);
        }
        public static void AddItem(string remoteHost, int remotePort, string remoteUser, string remotePass, string remoteName, List<int> alternativePorts)
        {
            var conf = new Config(remoteHost, remotePort, remoteUser, remotePass, remoteName, alternativePorts);
            //config.Add(conf);
        }
        public static void CreateDefaultConfig()
        {
            config = new List<Config>() {
                new Config("185.28.100.185", 30010, "admin", "admin", "PC1", new List<int> { 30011, 30012, 30013 }),
                new Config("185.28.100.185", 30020, "admin", "admin", "PC2", new List<int> { 30021, 30022, 30023 })
            };
        }
        public static List<Config> GetFullConfig()
        {
            return config;
        }
        public static void EditConfig(List<Config> newConfig)
        {
            config = newConfig;
        }
        public static void WriteConfigToFile(List<Config> newConfig)
        {
            StreamWriter sw = new StreamWriter(Environment.ExpandEnvironmentVariables(pathWithEnv));
            sw.Write("#Remote Desktop Client config\n");
            int i = 0;
            foreach (Config cfg in newConfig)
            {
                sw.Write($"remoteName.{i}={cfg.remoteName}\n");
                sw.Write($"remoteHost.{i}={cfg.remoteHost}\n");
                sw.Write($"remotePort.{i}={cfg.remotePort}\n");
                sw.Write($"remoteUser.{i}={cfg.remoteUser}\n");
                sw.Write($"remotePass.{i}={cfg.remotePass}\n");
                foreach (int j in cfg.alternativePorts)
                {
                    sw.Write($"AlternativePort.{i}={j}\n");
                }
                i++;
            }
            sw.Write("#EndConfig");
            sw.Close();
        }
        public static bool RemoveByName(string name)
        {
            for(int i = 0; i < config.Count; i++)
            {
                if(config[i].remoteName == name)
                {
                    config.RemoveAt(i);
                    return true;
                }
            }
            return false;
        } 
    }
}
