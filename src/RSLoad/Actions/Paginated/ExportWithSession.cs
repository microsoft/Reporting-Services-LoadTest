// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace RSLoad
{
    /// <summary>
    /// All export with sessions
    /// Sessions are created for each reports available and used to export
    /// Idealy, this would be faster than exporting without session.
    /// However, sometime RS could be smart enough to use the session that was already loaded even 
    /// without session id.            
    /// NOTE: As default, session are kept for 10 minutes, so we probably need session lifetime manager if we don't make the round by the time we need to use the session.
    /// </summary>
    public partial class PaginatedActions : PSSActionBase
    {
        #region UrlAccess Export Tests with Session
        /// <summary>
        /// Export to CSV format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportCSV()
        {
            ExportWithSessionIDTo("CSV");
        }

        /// <summary>
        /// Export to PDF format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportPDF()
        {
            ExportWithSessionIDTo("PDF");
        }

        /// <summary>
        /// Export to PPTX format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportPPTX()
        {
            ExportWithSessionIDTo("PPTX");
        }

        /// <summary>
        /// Export to XML format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportXML()
        {
            ExportWithSessionIDTo("XML");
        }

        /// <summary>
        /// Export to WORD format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportWORD()
        {
            ExportWithSessionIDTo("WORD");
        }

        /// <summary>
        /// Export to WORDOPENXML format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportWORDOPENXML()
        {
            ExportWithSessionIDTo("WORDOPENXML");
        }

        /// <summary>
        /// Export to EXCEL format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportEXCEL()
        {
            ExportWithSessionIDTo("EXCEL");
        }

        /// <summary>
        /// Export to EXCELOPENXML format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportEXCELOPENXML()
        {
            ExportWithSessionIDTo("EXCELOPENXML");
        }

        /// <summary>
        /// Export to RGDI format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportRGDI()
        {
            ExportWithSessionIDTo("RGDI");
        }

        /// <summary>
        /// Export to EMF format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportEMF()
        {
            ExportWithSessionIDTo("EMF");
        }

        /// <summary>
        /// Export to "HTML4.0 format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportHTML40()
        {
            ExportWithSessionIDTo("HTML4.0");
        }

        /// <summary>
        /// Export to "HTML5.0 format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportHTML50()
        {
            ExportWithSessionIDTo("HTML5");
        }

        /// <summary>
        /// Export to MHTML format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportMHTML()
        {
            ExportWithSessionIDTo("MHTML");
        }

        /// <summary>
        /// Export to ATOM format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportATOM()
        {
            ExportWithSessionIDTo("ATOM");
        }

        /// <summary>
        /// Export to TIFF format
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportTIFF()
        {
            ExportWithSessionIDTo("IMAGE", "TIFF");
        }

        /// <summary>
        /// Export to PNGormat
        /// </summary>
        [TestMethod]
        public void UrlAccessSessionExportPNG()
        {
            ExportWithSessionIDTo("IMAGE", "PNG");
        }
#endregion
        
        #region Private Methods
        private void ExportWithSessionIDTo(string renderFormat)
        {
            ExportWithSessionIDTo(renderFormat, null);
        }

        /// <summary>
        /// Export from Session, this is reportviewer controler scenario
        /// </summary>
        /// <param name="renderFormat">Format for Rendering</param>
        /// <param name="outputFormat">If applicable, output format</param>
        private void ExportWithSessionIDTo(string renderFormat, string outputFormat)
        {
            string report = this.ContentManager.GetNextReport();
            string sessionid = this.ContentManager.GetSessionID(report);
            string url = this.ContentManager.ConstructUrl(report, renderFormat, outputFormat, null) + "&rs:SessionID=" + sessionid;
            string metricName = this.ContentManager.GetReportName(report);
            string execId;
            BeginMeasure(metricName);
            try
            {
                this.ContentManager.IssueGetRequest(url, out execId);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("IssueGetRequest({0}) failed", url), ex);
            }
            finally
            {
                EndMeasure(metricName);
            }
        }
        #endregion
    }
}
