// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using RSAccessor.Utilities;

namespace RSAccessor.UrlAccessor
{
    public class RSUrlAccessor
    {
        public RSUrlAccessor(String reportServerUrl)
        {
            RSBaseUrl = reportServerUrl;
            Timeout = -1; // infinite by default
        }

        public string RSBaseUrl { get; set; }
        public ICredentials ExecuteCredentials { get; set; }
        public int Timeout { get; set; }

        /// <summary>
        /// URLRequest (i.e. http://machine/reportserver/?)
        /// </summary>
        public string requestUrl { get; set; }

        #region Properties
        private string m_lastExecutionID = string.Empty;

        /// <summary>
        /// Often we need this to pass to next request.  Looks like old code does not have it.  Only has 
        /// private variable.  So, adding public property to retrieve the field rather than removing.
        /// </summary>
        public string LastExectuionId
        {
            get
            {
                return (m_lastExecutionID);
            }
        }

        #endregion

        #region Protected Methods

        protected WebResponse GetResponse(string requestUrl)
        {
            return GetResponse(requestUrl, true);
        }

        /// <summary>
        /// Take the URL created and make a WebRequest to the ReportServer to get the rendered output.
        /// </summary>
        /// <param name="wrapException">Pass false is you want to receive the actual WebException
        /// that is thrown in case of an error.</param>
        /// <returns>Returns a WebResponse object</returns>
        protected WebResponse GetResponse(String requestUrl, bool wrapException)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(requestUrl);
                webRequest.Timeout = this.Timeout;
                webRequest.Credentials = this.ExecuteCredentials;
                WebResponse wr = webRequest.GetResponse();

                // try to read out last execution ID
                HttpWebResponse httpResponse = (HttpWebResponse)wr;
                CookieCollection cookies = httpResponse.Cookies;

                // if cookies are disabled, we need to manually parse them
                if (cookies == null || cookies.Count == 0)
                {
                    String[] rawCookies = httpResponse.Headers.GetValues("Set-Cookie");
                    if (rawCookies != null)
                    {
                        CookieContainer container = new CookieContainer();
                        Uri responseUri = wr.ResponseUri;
                        foreach (String cookieString in rawCookies)
                        {
                            container.SetCookies(responseUri, cookieString);
                        }

                        cookies = container.GetCookies(responseUri);
                    }
                }

                if (cookies != null)
                {
                    foreach (Cookie cookie in cookies)
                    {
                        if (cookie.Name != null && cookie.Name.StartsWith("RSExecutionSession"))
                        {
                            m_lastExecutionID = cookie.Value;
                            break;
                        }
                    }
                }

                return wr;
            }
            catch (System.Net.WebException we)
            {
                if (wrapException)
                {
                    if (we.Response != null)
                    {
                        using (StreamReader _sr = new StreamReader(we.Response.GetResponseStream()))
                        {
                            string _requesturi = we.Response.ResponseUri.AbsoluteUri;
                            StringBuilder _sb = new StringBuilder();
                            _sb.AppendLine("Web Request to the following URL Failed:");
                            _sb.AppendLine(_requesturi);
                            _sb.AppendLine("Response Stream:");
                            _sb.Append(_sr.ReadToEnd());
                            throw new Exception(_sb.ToString(), we);
                        }
                    }
                    else
                        throw new Exception("No response received", we);
                }
                throw;
            }
        }

        /// <summary>
        /// Gets rendered output from the server
        /// </summary>
        /// <returns>Returns WebResponse Stream.</returns>
        protected Stream GetResponseStream(string requestUrl)
        {
            WebResponse wr = GetResponse(requestUrl);
            Stream wrStream = wr.GetResponseStream();

            MemoryStream memStream = new MemoryStream();
            int readLength = 5000;
            int bytesRead = 0;
            byte[] temp = new byte[readLength];

            //Copy Web Response Stream to seekable stream
            while ((bytesRead = wrStream.Read(temp, 0, readLength)) > 0)
            {
                memStream.Write(temp, 0, bytesRead);
            }

            //Close response stream
            wrStream.Flush();
            wrStream.Close();

            //Reset positions
            memStream.Position = 0;
            return memStream;

        }

        /// <summary>
        /// Take all information collected and formulate it into a URL request url.
        /// </summary>
        protected string BuildRequestUrl(String path, NameValueCollection parameters)
        {
            string builtURL = null;

            builtURL = String.Format("{0}?{1}", this.RSBaseUrl, path);

            //Append Parameters
            foreach (String key in parameters.Keys)
            {
                string[] values = parameters.GetValues(key);
                if (values == null) 
                    builtURL += string.Format("&{0}:isnull=true", HttpUtility.UrlEncode(key));
                else
                {
                    //Multi Value Params May Have Multiple Values
                    foreach (String value in values)
                    {
                        builtURL += string.Format("&{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
                    }
                }
            }

            return builtURL;
        }

        /// <summary>
        /// Helper method to prefix parameters
        /// </summary>
        /// <param name="parameters">Parameters not prefixed</param>
        /// <param name="prefix">Prefix to assign</param>
        protected NameValueCollection PrefixParameters(NameValueCollection parameters, String prefix)
        {
            List<String> changeList = new List<string>();

            foreach (String key in parameters.Keys)
            {
                //If key is not prefixed
                if (!key.StartsWith(prefix))
                {
                    //Add to change list
                    changeList.Add(key);
                }
            }

            //Copy over all values to prefixed key
            foreach (String key in changeList)
            {
                //Add Revised Key and Values
                foreach (String value in parameters.GetValues(key))
                {
                    parameters.Add(prefix + key, value);
                }

                //Remove old key
                parameters.Remove(key);
            }

            return parameters;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Implements Render Method using URL Access
        /// </summary>
        /// <param name="reportPath">Catalog Path to report</param>
        /// <param name="renderStruct">Render Structure</param>
        /// <param name="reportParams">Report Parameters</param>
        /// <returns>Rendered output as Stream</returns>
        public Stream Render(String reportPath, RSRenderStruct renderStruct, NameValueCollection reportParams)
        {
            //Return Stream
            return GetResponseStream(GetUrl(reportPath, renderStruct, reportParams));
        }

        public string GetUrl(String reportPath, RSRenderStruct renderStruct, NameValueCollection reportParams)
        {
            NameValueCollection parameters;

            //Assert that report path is specified
            if (reportPath == null)
            {
                throw new Exception("Must specify path to report in order to render.");
            }

            //Build Device Info Parameters
            parameters = renderStruct.BuildParamCollection();
            parameters = PrefixParameters(parameters, "rc:");

            //Append Report Parameters
            if (reportParams != null)
            {
                reportParams = PrefixParameters(reportParams, "");
                parameters.Add(reportParams);
            }

            // Build Command and Format Parameters, but don't add
            // duplicate commands.
            if (parameters.GetValues("rs:Command") == null)
                parameters.Add("rs:Command", "Render");
            parameters.Add("rs:Format", renderStruct.RenderFormat);

            //Build the request Url
            requestUrl = BuildRequestUrl(reportPath, parameters);

            return requestUrl;
        }

        #endregion
    }


}
