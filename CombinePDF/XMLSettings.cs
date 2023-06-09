﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace CombinePDF
{
    public static class XMLSettings
    {
        private static string CommandName = "CombinePDF"; // Replace COMMAND_NAME
        public static string AppSettingsDir =  Path.Combine(@"C:\Users\", Environment.UserName, @"AppData\Local\" + CommandName);
        public static string AppSettingsFile = Path.Combine(@"C:\Users\", Environment.UserName, @"AppData\Local\" + CommandName + "\\Settings.xml");

        public static bool SettingsFileExists()
        {
            bool flag = false;

            if (File.Exists(AppSettingsFile))
                flag = true;
            else
                flag = false;

            return flag;
        }

        public static string GetSettingsValue(string _Field)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(AppSettingsFile);

            XmlNode node = null;
            node = doc.SelectSingleNode(_Field);

            string value = string.Empty;

            if (node == null)
                value = string.Empty;
            else
                value = node.InnerText;

            return value;
        }

        public static void SetSettingsValue(string _Field, string _Value)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(AppSettingsFile);

            if (doc.SelectSingleNode(_Field) == null)
            {
                _Field = _Field.Replace("//Settings/", "");
                XmlNode field = doc.CreateElement(_Field);
                field.InnerText = _Value;
                doc.DocumentElement.AppendChild(field);
                doc.Save(AppSettingsFile);
            }
            else
            {
                XmlNode node = null;
                node = doc.SelectSingleNode(_Field);
                node.InnerText = _Value;
                doc.Save(AppSettingsFile);
            }
        }

        public static void CreateAppSettings_SetDefaults()
        {
            StringCollection appSettings = new StringCollection();

            if (!Directory.Exists(AppSettingsDir))
                Directory.CreateDirectory(AppSettingsDir);

            // Add settings here
            appSettings.Add("DefaultDirectory," + "");
            appSettings.Add("AlwaysOverwrite," + "false");

            if (!SettingsFileExists())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter XmlWrt = XmlWriter.Create(AppSettingsFile, settings);

                {
                    var withBlock = XmlWrt;
                    withBlock.WriteStartDocument();

                    withBlock.WriteComment("Application Settings");
                    withBlock.WriteStartElement("Settings");

                    string[] arr;

                    foreach (string setting in appSettings)
                    {
                        arr = setting.Split(',');

                        string settingName = arr[0];
                        string defaultValue = arr[1];

                        withBlock.WriteStartElement(settingName);
                        withBlock.WriteString(defaultValue);
                        withBlock.WriteEndElement();
                    }

                    withBlock.WriteEndDocument();
                    withBlock.Close();
                }

                XmlWrt = null;
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(AppSettingsFile);
                XmlElement elm = xmlDoc.DocumentElement;
                XmlNodeList lstSettings = elm.ChildNodes;
                string[] arr;
                StringCollection nodeNames = new StringCollection();

                foreach (XmlNode node in lstSettings)
                {
                    nodeNames.Add(node.Name);
                }

                foreach (string setting in appSettings)
                {
                    arr = setting.Split(',');

                    string settingName = arr[0];
                    string defaultValue = arr[1];

                    if (!nodeNames.Contains(settingName))
                    {
                        XmlNode newSetting = xmlDoc.CreateElement(settingName);
                        newSetting.InnerText = defaultValue;
                        xmlDoc.DocumentElement.AppendChild(newSetting);
                        xmlDoc.Save(AppSettingsFile);
                    }
                }
            }
        }

        public sealed class ApplicationSettings
        {
            // Add settings here
            public const string DefaultDirectory = "//Settings/DefaultDirectory";
            public const string AlwaysOverwrite = "//Settings/AlwaysOverwrite";
        }
    }
}
