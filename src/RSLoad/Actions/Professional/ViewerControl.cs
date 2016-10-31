// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.SoapAccessor;
using RSAccessor.Utilities;

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
        #region Test cases simulating viewer control

        /// <summary>
        /// Simulation of Report Viewer Control Use (load first page)
        /// </summary>
        [TestMethod]
        public void ViewerControlFirstPage()
        {
            string report = this.ContentManager.GetNextReport();
            string metricName = this.ContentManager.GetReportName(report);

            BeginMeasure(metricName);
            try
            {
                SimulateFirstPageLoad(report);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("First Retrival failed for report {0}", report), ex);
            }
            finally
            {
                EndMeasure(metricName);
            }
        }

        /// <summary>
        /// Simulation of Export from ViewerControl
        /// 1. Set ExecutionID on to header
        /// GetEecutionInfo and pass that to contructiong url.
        /// </summary>
        [TestMethod]
        public void ViewerControlExportCSV()
        {
            ViewerControlExport("CSV");
        }

        /// <summary>
        /// Export to PDF format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportPDF()
        {
            ViewerControlExport("PDF");
        }

        /// <summary>
        /// Export to PDF format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportPPTX()
        {
            ViewerControlExport("PPTX");
        }

        /// <summary>
        /// Export to XML format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportXML()
        {
            ViewerControlExport("XML");
        }

        /// <summary>
        /// Export to WORD format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportWORD()
        {
            ViewerControlExport("WORD");
        }

        /// <summary>
        /// Export to WORDOPENXML format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportWORDOPENXML()
        {
            ViewerControlExport("WORDOPENXML");
        }

        /// <summary>
        /// Export to EXCEL format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportEXCEL()
        {
            ViewerControlExport("EXCEL");
        }

        /// <summary>
        /// Export to EXCELOPENXML format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportEXCELOPENXML()
        {
            ViewerControlExport("EXCELOPENXML");
        }

        /// <summary>
        /// Export to RGDI format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportRGDI()
        {
            ViewerControlExport("RGDI");
        }

        /// <summary>
        /// Export to EMF format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportEMF()
        {
            ViewerControlExport("EMF");
        }

        /// <summary>
        /// Export to "HTML4.0 format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportHTML40()
        {
            ViewerControlExport("HTML4.0");
        }

        /// <summary>
        /// Export to "HTML5.0 format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportHTML50()
        {
            ViewerControlExport("HTML5");
        }

        /// <summary>
        /// Export to MHTML format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportMHTML()
        {
            ViewerControlExport("MHTML");
        }

        /// <summary>
        /// Export to ATOM format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportATOM()
        {
            ViewerControlExport("ATOM");
        }

        /// <summary>
        /// Export to TIFF format
        /// </summary>
        [TestMethod]
        public void ViewerControlExportTIFF()
        {
            ViewerControlExport("IMAGE", "TIFF");
        }

        /// <summary>
        /// Export to PNGormat
        /// </summary>
        [TestMethod]
        public void ViewerControlExportPNG()
        {
            ViewerControlExport("IMAGE", "PNG");
        }
        #endregion

        #region Private Methods

        private void ViewerControlExport(string format)
        {
            ViewerControlExport(format, string.Empty);
        }

        private void ViewerControlExport(string format, string outputformat)
        {
            string report = this.ContentManager.GetNextReport();
            string metricName = this.ContentManager.GetReportName(report);
            string sessionid = this.ContentManager.GetSessionID(report);

            BeginMeasure(metricName);
            try
            {
                string newsessionid = SimulateViewerControlExport(report, sessionid, format, outputformat);
            }
            catch (Exception ex)
            {
                throw new Exception("Simulated Export Failed.", ex);
            }
            finally
            {
                EndMeasure(metricName);
            }
        }

        private string SimulateViewerControlExport(string report, string sessionid, string format, string outputformat)
        {
            string execId = string.Empty;
            this.ContentManager.SoapAccessor.Execution.LoadReport(report, null);
            this.ContentManager.SoapAccessor.Execution.ExecutionHeaderValue.ExecutionID = sessionid;
            RSExecutionInfo execInfo = this.ContentManager.SoapAccessor.Execution.GetExecutionInfo();
            RSRenderStruct renderFormat = this.ContentManager.ConstructRenderStruct(format, outputformat);
            string url = this.ContentManager.ConstructUrl(report, renderFormat, null, 0, 0, execInfo.ExecutionID, null);
            this.ContentManager.IssueGetRequest(url, out execId);

            return (execId);
        }

        private static object _lockFirstPage = new object();

        private void SimulateFirstPageLoad(string report)
        {
            RSExecutionInfo info = this.ContentManager.SoapAccessor.Execution.LoadReport(report, null);
            RSExecutionInfo execInfo = this.ContentManager.SoapAccessor.Execution.GetExecutionInfo();
            HTML50Render renderFormat = new HTML50Render();
            string url = this.ContentManager.ConstructUrl(report, renderFormat, null, 0, 0, execInfo.ExecutionID, null);
            string execid;
            this.ContentManager.IssueGetRequest(url, out execid);
            execInfo = this.ContentManager.SoapAccessor.Execution.GetExecutionInfo();
        }
        #endregion
    }
}
