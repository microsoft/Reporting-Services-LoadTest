// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.SoapAccessor;
using RSAccessor.Utilities;
using RSLoad.Utilities;

namespace RSLoad
{
    /// <summary>
    /// All export with sessions
    /// Sessions are created for each reports available and used to export
    /// Idealy, this would be faster than exporting without session.
    /// However, sometime RS could be smart enough to use the session that was already loaded even 
    /// without session id.            
    /// </summary>
    public partial class PaginatedActions : PSSActionBase
    {
        #region Test cases
        /// <summary>
        /// Full Report Render, no session is used to render.  This is report viewer scenario.
        /// You can use the session ids to get more information for getting more stats, execution log data for example.
        /// Close to FirstPage case, but without loadreport and executioninfo 
        /// </summary>
        [TestMethod]
        public void FullReportRender()
        {
            string report = this.ContentManager.GetNextReport(); 
            string sessionid = string.Empty;
            MHTMLRender renderFormat = new MHTMLRender() { Section = "0" };
            string url = this.ContentManager.ConstructUrl(report, renderFormat, null);
            string metricName = this.ContentManager.GetReportName(report);

            BeginMeasure(metricName);
            try
            {
                this.ContentManager.IssueGetRequest(url, out sessionid);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("IssueGetRequest({0}) failed.", url), ex);
            }
            finally
            {
                EndMeasure(metricName);
            }

            // collectioin execution log data
        }

        /// <summary>
        /// Full Report Render no session is used to render.  This is report viewer scenario.
        /// You can use the session ids to get more information for getting more stats, execution log data for example.
        /// Close to FirstPage case, but without loadreport and executioninfo 
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void FullReportRenderHTML()
        {
            string report = this.ContentManager.GetNextReport();
            string sessionid = string.Empty;
            HTML40Render renderFormat = new HTML40Render() { Section = "0" };
            string url = this.ContentManager.ConstructUrl(report, renderFormat, null);
            string metricName = this.ContentManager.GetReportName(report);

            BeginMeasure(metricName);
            try
            {
                this.ContentManager.IssueGetRequest(url, out sessionid);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("IssueGetRequest({0}) failed.", url), ex);
            }
            finally
            {
                EndMeasure(metricName);
            }
        }

        /// <summary>
        /// Full Report Render in HTML5.0, no session is used to render.  This is report viewer scenario.
        /// You can use the session ids to get more information for getting more stats, execution log data for example.
        /// Close to FirstPage case, but without loadreport and executioninfo 
        /// </summary>
        [TestMethod]
        public void FullReportRenderHTML5()
        {
            string report = this.ContentManager.GetNextReport();
            string sessionid = string.Empty;
            HTML50Render renderFormat = new HTML50Render() { Section = "0" };
            string url = this.ContentManager.ConstructUrl(report, renderFormat, null);
            string metricName = this.ContentManager.GetReportName(report);

            BeginMeasure(metricName);
            try
            {
                this.ContentManager.IssueGetRequest(url, out sessionid);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("IssueGetRequest({0}) failed.", url), ex);
            }
            finally
            {
                EndMeasure(metricName);
            }
        }

        /// <summary>
        /// Retrieve all pages, page by page.
        /// If number total number of pages is 20 or less, read all pages.
        /// If number of pages is greater than 20, then read first 20, then
        /// read few pages in between including last page. (this was original design)
        /// Should read all?
        /// Originally, we wrote the page to file.  I am changing it to just read to memory only.
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void PageByPageRenderAll()
        {
            string report = this.ContentManager.GetNextReport(); 
            string metricName = this.ContentManager.GetReportName(report);
            string url = PrepareUrl(new HTML40Render(), report);
            int pageCount = this.ContentManager.GetPageCount(report);

            BeginMeasure(metricName);
            RenderReportPageByPageAll(url, pageCount);
            EndMeasure(metricName);
        }

        /// <summary>
        /// Retrieve all pages, page by page.
        /// If number total number of pages is 20 or less, read all pages.
        /// If number of pages is greater than 20, then read first 20, then
        /// read few pages in between including last page. (this was original design)
        /// Should read all?
        /// Originally, we wrote the page to file.  I am changing it to just read to memory only.
        /// </summary>
        [TestMethod]
        public void PageByPageRenderAllHtml5()
        {
            string report = this.ContentManager.GetNextReport();
            string metricName = this.ContentManager.GetReportName(report);
            string url = PrepareUrl(new HTML50Render(), report);
            int pageCount = this.ContentManager.GetPageCount(report);

            BeginMeasure(metricName);
            RenderReportPageByPageAll(url, pageCount);
            EndMeasure(metricName);
        }

        /// <summary>
        /// Print a given report
        /// This is simulated print method from RS.
        /// 1. We get SessionID, so wedon't keep processing report again and again
        /// 2. Send first request with rs:PersistStreams=true
        /// 3. Send subsequent report with rs:GetNextStream=true
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void PrintReport()
        {
            string report = this.ContentManager.GetNextReport(); 
            string metricName = this.ContentManager.GetReportName(report);
            string url = PrepareUrlForPrint(report);

            BeginMeasure(metricName);
            PrintReportUrl(url);
            EndMeasure(metricName);
        }
        #endregion

        #region Private Methods
        private void PrintReportUrl(string url)
        {
            string persistStreams = "&rs:PersistStreams=true";
            string getNextStream = "&rs:GetNextStream=true";
            int fileSize = 0;
            int streams = 0;
            string executionID = string.Empty;
            HttpWebRequest request = this.ContentManager.CreateHttpWebRequest(url + persistStreams);
            try
            {
                while (true)
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        foreach (Cookie c in request.CookieContainer.GetCookies(new Uri(this.ContentManager.SoapAccessor.Management.Url)))
                        {
                            if (c.Name.StartsWith("RSExecutionSession", StringComparison.InvariantCultureIgnoreCase))
                                executionID = c.Value;
                        }

                        Logging.Log("Strimg: {2} SessionID: {0}, Url: {1}", executionID, url, streams++);

                        using (Stream stream = response.GetResponseStream())
                        {
                            Int32 bytesRead = 0;
                            Byte[] buffer = new Byte[1024 * 1024]; // 1 MB check
                            int streamSize = 0;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                fileSize += bytesRead; // Just doing some work
                                streamSize += bytesRead;
                            }

                            if (streamSize == 0)
                                break;
                        }

                        request = this.ContentManager.CreateHttpWebRequest(url + getNextStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log("Message: {0}\nin Url {1}", ex.Message, url);
                throw ex;
            }
        }

        /// <summary>
        /// Prepares Url for Printing with SessionID
        /// </summary>
        /// <param name="report">Report for printing</param>
        /// <returns>Completed Url</returns>
        private string PrepareUrlForPrint(string report)
        {
            IMAGERender renderFormat = new IMAGERender();
            renderFormat.OutputFormat = "EMF";
            string url = this.ContentManager.URLAccessor.GetUrl(report, renderFormat, null);
            RSExecutionInfo execInfo = this.ContentManager.SoapAccessor.Execution.LoadReport(report, null);
            url += "&rs:SessionID=" + execInfo.ExecutionID;
            return url;
        }

        private string PrepareUrl(HTMLRender renderFormat, string report)
        {
            string url = this.ContentManager.URLAccessor.GetUrl(report, renderFormat, null);

            if (url.Contains(renderFormat.getRenderFormat()) || url.Contains("RPL"))
                url = url + "&rc:Section={0}";
            else
                url = url + "&rc:StartPage={0}&rc:EndPage={0}"; // start and end is same to get just one page
            return url;
        }

        private void RenderReportPageByPageAll(string url, int pageCount)
        {
            string execid = string.Empty;
            List<int> validPages = new List<int>(new int[] { 40, 60, 80, 100 });
            validPages.Add(pageCount);
            validPages.Sort();

            for (int i = 1; i <= pageCount && i <= 20; i++)
            {
                // should we write TO file?  I am thinking to just do in-memory read.
                 this.ContentManager.IssueGetRequest(string.Format(url, i.ToString()), out execid);
            }

            if (pageCount > 20)
            {
                foreach (int i in validPages)
                {
                    if (i > pageCount) break;
                    this.ContentManager.IssueGetRequest(string.Format(url, i.ToString()), out execid);
                }
            }
        }

        #endregion
    }
}
