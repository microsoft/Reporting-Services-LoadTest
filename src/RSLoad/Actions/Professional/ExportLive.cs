// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RSLoad
{
    /// <summary>
    /// Profesional Report Actions - We can use for all perf/stress/scale
    /// </summary>
    public partial class PaginatedActions : PSSActionBase
    {
        #region Basic Export Tests (no session)
        /// <summary>
        /// Export to PDF format
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void UrlAccessLiveExportPDF()
        {
            ExportTo("PDF");
        }

        /// <summary>
        /// Export to PPTX format
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void UrlAccessLiveExportPPTX()
        {
            ExportTo("PPTX");
        }

        /// <summary>
        /// Export to CSV format
        /// </summary>
        [TestMethod]
        public void UrlAccessLiveExportCSV()
        {
            ExportTo("CSV");
        }

        /// <summary>
        /// Export to XML format
        /// </summary>
        [TestMethod]
        public void UrlAccessLiveExportXML()
        {
            ExportTo("XML");
        }

        /// <summary>
        /// Export to WORD format
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void UrlAccessLiveExportWORD()
        {
            ExportTo("WORD");
        }

        /// <summary>
        /// Export to WORDOPENXML format
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void UrlAccessLiveExportWORDOPENXML()
        {
            ExportTo("WORDOPENXML");
        }

        /// <summary>
        /// Export to EXCEL format
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void UrlAccessLiveExportEXCEL()
        {
            ExportTo("EXCEL");
        }

        /// <summary>
        /// Export to EXCELOPENXML format
        /// </summary>
        [TestMethod]
        public void UrlAccessLiveExportEXCELOPENXML()
        {
            ExportTo("EXCELOPENXML");
        }

        /// <summary>
        /// Export to RGDI format
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void UrlAccessLiveExportRGDI()
        {
            ExportTo("RGDI");
        }

        /// <summary>
        /// Export to EMF format
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void UrlAccessLiveExportEMF()
        {
            ExportTo("EMF");
        }

        /// <summary>
        /// Export to "HTML4.0 format
        /// </summary>
        [TestMethod]
        public void UrlAccessLiveExportHTML40()
        {
            ExportTo("HTML4.0");
        }

        /// <summary>
        /// Export to "HTML5.0 format
        /// </summary>
        [TestMethod]
        public void UrlAccessLiveExportHTML50()
        {
            ExportTo("HTML5");
        }

        /// <summary>
        /// Export to MHTML format
        /// </summary>
        [TestMethod]
        public void UrlAccessLiveExportMHTML()
        {
            ExportTo("MHTML");
        }

        /// <summary>
        /// Export to ATOM format
        /// </summary>
        [TestMethod]
        public void UrlAccessLiveExportATOM()
        {
            ExportTo("ATOM");
        }

        /// <summary>
        /// Export to TIFF format
        /// </summary>
        [TestCategory("RunInContinuousIntegration")]
        [TestMethod]
        public void UrlAccessLiveExportTIFF()
        {
            ExportTo("IMAGE", "TIFF");
        }

        /// <summary>
        /// Export to PNGormat
        /// </summary>
        [TestMethod]
        public void UrlAccessLiveExportPNG()
        {
            ExportTo("IMAGE", "PNG");
        }
        #endregion

        #region Private Methods
        private void ExportTo(string renderFormat)
        {
            ExportTo(renderFormat, string.Empty);
        }

        private void ExportTo(string renderFormat, string outputFormat)
        {
            string report = this.ContentManager.GetNextReport();
            string url = this.ContentManager.ConstructUrl(report, renderFormat, null);
            string metricName = this.ContentManager.GetReportName(report);
            string execId;
            BeginMeasure(metricName);
            try
            {
                this.ContentManager.IssueGetRequest(url, out execId);
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
        #endregion
    }
}
