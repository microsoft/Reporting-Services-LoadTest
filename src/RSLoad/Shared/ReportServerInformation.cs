// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using RSLoad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace RSTest.Common.ReportServer.Information
{
    /// <summary>
    /// Contains the Report Server information
    /// </summary>
    public class ReportServerInformation
    {
        private static readonly string m_configFileName = "RSTest.Common.ReportServer.dll.config";
        private static readonly string m_configFilePath = Path.Combine(Environment.CurrentDirectory, m_configFileName);
        private static readonly XmlSerializer m_serializer = new XmlSerializer(typeof(ReportServerInformation));
        private static ReportServerInformation m_reportServerInformationSingleton;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ReportServerInformation()
        {
        }

        /// <summary>
        /// Returns the singleton instance of the ReportServerInformation object.
        /// </summary>
        public static ReportServerInformation DefaultInformation
        {
            get
            {
                if (m_reportServerInformationSingleton == null)
                {
                    // Deserialize configuration xml file
                    string configXml = FixConfigXmlRootName();
                    StringReader strReader = new StringReader(configXml);
                    m_reportServerInformationSingleton = m_serializer.Deserialize(strReader) as ReportServerInformation;
                    m_reportServerInformationSingleton.MergeDefaultValue();
                    if (m_reportServerInformationSingleton == null)
                        throw new Exception(string.Format("Could not deserialize {0} to a ReportServerInformation object.", m_configFilePath));
                }

                return m_reportServerInformationSingleton;
            }
        }

        /// <summary>
        /// Fix configuration xml file root name if it's not same as the ReportServerInformation class name
        /// </summary>
        /// <returns>The configuration xml string</returns>
        private static string FixConfigXmlRootName()
        {
            // Check the root node name of configuration xml file
            XmlDocument configDoc = new XmlDocument();
            configDoc.Load(m_configFilePath);
            string configXml = configDoc.OuterXml;
            XmlElement rootNode = configDoc.DocumentElement;
            string typeName = typeof(ReportServerInformation).Name;

            if (rootNode.Name != typeName)
            {
                XmlDocument newConfigDoc = new XmlDocument();
                XmlElement newRoot = newConfigDoc.CreateElement(typeName);
                newRoot.InnerXml = rootNode.InnerXml;
                newConfigDoc.AppendChild(newRoot);
                configXml = newConfigDoc.OuterXml;
            }

            return configXml;
        }

        private void MergeDefaultValue()
        {
            SetPropertyValue<string>("DatasourceDatabaseServer", "ssrs-datasource");
        }

        /// <summary>
        /// Set the value of property using the default condition
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="propertyName">Name of property</param>
        /// <param name="value">Value to set for property</param>
        private void SetPropertyValue<T>(string propertyName, T value)
        {
            SetPropertyValue<T>(propertyName, value, null);
        }

        /// <summary>
        /// Set the value of property if the condition is met
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="propertyName">Name of property</param>
        /// <param name="value">Value to set for property</param>
        /// <param name="condition">Condition to check</param>
        private void SetPropertyValue<T>(string propertyName, T value, Func<T, bool> condition)
        {
            PropertyInfo property = this.GetType().GetProperty(propertyName);

            if (condition == null)
            {
                if (typeof(T) == typeof(string))
                {
                    condition = x => x == null || string.IsNullOrWhiteSpace(x.ToString());
                }
                else
                {
                    condition = x => x == null;
                }
            }

            T propertyValue = (T)property.GetValue(this, null);

            if (condition(propertyValue))
            {
                property.SetValue(this, value, null);
            }
        }

        /// <summary>
        /// Gets the port from a given address
        /// </summary>
        /// <param name="address">The address to parse</param>
        /// <returns>The port of a given address</returns>
        private int GetPortFromURI(string address)
        {
            Uri tempURI = new Uri(address);
            int port = tempURI.Port;

            return port;
        }

        /// <summary>
        /// Gets the port from a given address
        /// </summary>
        /// <param name="address">The address to parse</param>
        /// <returns>The port of a given address</returns>
        private string GetURLProtocolFromURI(string address)
        {
            Uri tempURI = new Uri(address);
            return tempURI.Scheme;
        }

        /// <summary>
        /// Takes an address and returns the Port portion of it.
        /// </summary>
        /// <param name="address">The URI address to parse</param>
        /// <returns>The associated with the URI or -1 if none is found</returns>
        private int ParsePortFromAddress(string address)
        {
            // Initialize error code
            int port = -1;

            // Find the index of where the port is located in the string, http://VSQLPOD000-123:654321/
            Match match = Regex.Match(address, "[a-zA-Z0-9]:[0-9]");
            if (match.Success)
            {
                int startPos = match.Index + 2; // The 2 skips the index of the first char and the ":" itself

                // If we found a pattern matching a port, get the value
                if (startPos > -1)
                {
                    // Scrub the located port for any unwated characters, ie. anything other than numeric chars
                    port = int.Parse(RemoveFromString(address.Substring(startPos), @"[^0-9]"));
                }
            }

            return port;
        }

        /// <summary>
        /// Scrubs the basestring and removes anything specified in the pattern
        /// </summary>
        /// <param name="baseString">The string to scrub</param>
        /// <param name="pattern">The pattern to scrub for</param>
        /// <returns>The scrubbed string</returns>
        private string RemoveFromString(string baseString, string pattern)
        {
            string scrubbedString = Regex.Replace(baseString, pattern, String.Empty);
            return scrubbedString;
        }

        /// <summary>
        /// Gets URL of Portal
        /// </summary>
        public string ReportPortalUrl { get; set; }

        /// <summary>
        /// Gets the prefix of RM URL
        /// </summary>
        public string ReportManagerUrlPrefix { get; set; }

        /// <summary>
        /// Gets the prefix of PowerBI API Url
        /// </summary>
        public string PowerBIUrl { get; set; }

        #region Report Server
        public string DatasourceDatabaseServer { get; set; }

        /// <summary>
        /// Gets Report Server Url.
        /// </summary>
        public string ReportServerUrl { get; set; }

        /// <summary>
        /// Gets Prefix of ReportServer Url
        /// </summary>
        public string ReportServerUrlPrefix { get; set; }

        /// <summary>
        /// Gets string value of port number of Report Server. Only used for de/serialization.
        /// </summary>
        [XmlElement("ReportServerPort")]
        public string ReportServerPortString
        {
            get { return ReportServerPort.ToString(); }
            set
            {
                int port;
                if (!int.TryParse(value, out port))
                {
                    port = 80;
                }

                ReportServerPort = port;
            }
        }

        /// <summary>
        /// Gets Port number of Report Server
        /// </summary>
        [XmlIgnore]
        public int ReportServerPort { get; set; }

        private string _restApiV1Url;

        /// <summary>
        /// Gets Report Server Portal V1 Url.
        /// </summary>
        public string RestApiV1Url
        {
            get
            {
                if (String.IsNullOrEmpty(_restApiV1Url))
                {
                    _restApiV1Url = string.Format("{0}/{1}", ReportPortalUrl, SharedConstants.ApiV1PostFix);
                }
                return _restApiV1Url;
            }
            set
            {
                _restApiV1Url = value;
            }
        }

        private string _restApiV2Url;

        /// <summary>
        /// Gets Report Server Portal V1 Url.
        /// </summary>
        public string RestApiV2Url
        {
            get
            {
                if (String.IsNullOrEmpty(_restApiV2Url))
                {
                    _restApiV2Url = string.Format("{0}/{1}", ReportPortalUrl, SharedConstants.ApiV2PostFix);
                }
                return _restApiV2Url;
            }
            set
            {
                _restApiV2Url = value;
            }
        }
        #endregion

        #region Execution Account

        /// <summary>
        /// Gets string value of IsExecutionAccountSpecified. Only used for de/serialization.
        /// </summary>
        [XmlElement("IsExecutionAccountSpecified")]
        public string IsExecutionAccountSpecifiedString
        {
            get { return IsExecutionAccountSpecified.ToString(); }
            set
            {
                bool flag;
                if (!bool.TryParse(value, out flag))
                {
                    flag = false;
                }

                IsExecutionAccountSpecified = flag;
            }
        }

        /// <summary>
        /// Is Execution Account setup? If we ever need it
        /// </summary>
        [XmlIgnore]
        public bool IsExecutionAccountSpecified { get; set; }

        /// <summary>
        /// Execution Account 
        /// </summary>
        public string ExecutionAccount { get; set; }

        /// <summary>
        /// Associated password for above account
        /// </summary>
        public string ExecutionAccountPwd { get; set; }

        public string DatasourceSQLUser { get; set; }

        public string DatasourceSQLPassword { get; set; }

        #endregion
    }
}