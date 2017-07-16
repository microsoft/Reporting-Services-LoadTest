// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.Xml;
using System.Reflection;

namespace RSAccessor.SoapAccessor
{
    public class RSEndPointInfo
    {
        public RSEndPointInfo()
        {
            Url = "http://localhost/ReportServer"
            EndPoint = "ReportingServices2010";
        }

        public RSEndPointInfo(string url, string endpoint)
        {
            Url = url;
            EndPoint = endpoint;
        }

        public string Url { get; set; }
        public string EndPoint { get; set; }
    }

    /// <summary>
    /// Generic RSSoapAccessor class.
    /// 1) If no url and endpoint is provided, one of the default is created (RSSoapAccessor())
    ///     a) If no config file is provided, we will create 2010 with localhost
    ///     b) If config file is provided then, we will create it with provided info
    /// 2) If Url and EndPoint is provided, then appropriate accessor is created
    /// 3) If RSEndPointInfo is provided, then same as #2.
    /// 
    /// If asked endpoint does not exist, it will CreateSoapAccessor will throw.
    /// 
    /// Caller can directly instantiate these class, but best practice should be to call from central
    /// location where all call will return same endpoint.  For example, this info could sit in MTR or RSTest Common config file.
    /// So, the factory there will call RSSoapAccessor(Url, EndPoint).
    /// 
    /// If there is no central place, next best practice should be include info in this instance config file then call default constructor.
    /// 
    /// Note: EndPointInfo is left public with thought that we might need to have that info handy after accessor is created.
    /// </summary>
    public class RSSoapAccessor
    {
        public RSSoapAccessor(string url, ICredentials credentials)
        {
            CreateSoapAccessorEndPoints(url, credentials);
        }

        public IRSManagement Management { get; set; }
        public IRSExecution Execution { get; set; }

        public RSEndPointInfo EndPointInfo { get; set; }

        #region Private Methods and Properties
        
        /// <summary>
        /// If anything goes wrong, throw.
        /// Note: Probably use activator instead of case statement mappping.
        /// </summary>
        /// <param name="url">Url to reportserver (http://localhost/reportserver)</param>
        private void CreateSoapAccessorEndPoints(string url, ICredentials credentials)
        {
            Management = new ReportingServices2010(url, credentials);
            Execution = new ReportExecution2005(url, credentials);
        }

        private RSEndPointInfo GetEndPointInfo()
        {
            RSEndPointInfo instanceInfo = null;
            string configFileName = Assembly.GetExecutingAssembly().FullName.Split(',')[0] + ".dll.config";
            XmlDocument doc = new XmlDocument();
            XmlNode node = null;

            doc.Load(configFileName);
            node = doc.SelectSingleNode("//SoapAccessorInfo");

            if (node != null)
            {
                instanceInfo = new RSEndPointInfo()
                {
                    Url = node.Attributes["Url"].Value,
                    EndPoint = node.Attributes["EndPoint"].Value
                };
            }
            else
            {
                instanceInfo = new RSEndPointInfo();
            }

            return instanceInfo;
        }

        #endregion 
    }
}
