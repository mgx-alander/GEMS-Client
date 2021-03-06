﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

namespace Gems.UIWPF
{
    //Reference:
    //http://stackoverflow.com/questions/966323/how-to-programatically-modify-wcf-app-config-endpoint-address-setting
    public class ConfigHelper
    {
        private static string NodePath = "//system.serviceModel//client//endpoint";
        private ConfigHelper() { }

        public static string GetEndpointAddress()
        {
            return ConfigHelper.loadConfigDocument().SelectSingleNode(NodePath).Attributes["address"].Value;
        }

        public static void SaveEndpointAddress(string endpointAddress)
        {
            // load config document for current assembly
            XmlDocument doc = loadConfigDocument();

            // retrieve appSettings node
            XmlNode node = doc.SelectSingleNode(NodePath);

            if (node == null)
                throw new InvalidOperationException("Error. Could not find endpoint node in config file.");

            try
            {
                // select the 'add' element that contains the key
                //XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", key));
                node.Attributes["address"].Value = endpointAddress;

                doc.Save(getConfigFilePath());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static XmlDocument loadConfigDocument()
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(getConfigFilePath());
                return doc;
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }
        }

        private static string getConfigFilePath()
        {
            return Assembly.GetExecutingAssembly().Location + ".config";
        }
    }
}






