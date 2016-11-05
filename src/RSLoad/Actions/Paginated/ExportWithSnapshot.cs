// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.SoapAccessor;
using RSAccessor.UrlAccessor;
using RSAccessor.Utilities;


namespace RSLoad
{
    /// <summary>
    /// All export with Snapshot
    /// Snapshots are created for each reports available and used to export
    /// If snapshot is not there yet, trying to get one causes one to be created
    /// </summary>
    public partial class PaginatedActions : PSSActionBase
    {
        #region UrlAccess Export Tests with Snapshot

        /// <summary>
        /// Export to PDF format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportPDF()
        {
            ExportWithSnapshotTo("PDF", null);
        }

        /// <summary>
        /// Export to PPTX format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportPPTX()
        {
            ExportWithSnapshotTo("PPTX", null);
        }

        /// <summary>
        /// Export to CSV from Snapshot
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportCSV()
        {
            ExportWithSnapshotTo("CSV", null);
        }

        /// <summary>
        /// Export to XML format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportXML()
        {
            ExportWithSnapshotTo("XML", null);
        }

        /// <summary>
        /// Export to WORD format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportWORD()
        {
            ExportWithSnapshotTo("WORD", null);
        }

        /// <summary>
        /// Export to WORDOPENXML format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportWORDOPENXML()
        {
            ExportWithSnapshotTo("WORDOPENXML", null);
        }

        /// <summary>
        /// Export to EXCEL format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportEXCEL()
        {
            ExportWithSnapshotTo("EXCEL", null);
        }

        /// <summary>
        /// Export to EXCELOPENXML format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportEXCELOPENXML()
        {
            ExportWithSnapshotTo("EXCELOPENXML", null);
        }

        /// <summary>
        /// Export to RGDI format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportRGDI()
        {
            ExportWithSnapshotTo("RGDI", null);
        }

        /// <summary>
        /// Export to EMF format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportEMF()
        {
            ExportWithSnapshotTo("EMF", null);
        }

        /// <summary>
        /// Export to "HTML4.0 format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportHTML40()
        {
            ExportWithSnapshotTo("HTML4.0", null);
        }

        /// <summary>
        /// Export to "HTML5.0 format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportHTML50()
        {
            ExportWithSnapshotTo("HTML5", null);
        }

        /// <summary>
        /// Export to MHTML format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportMHTML()
        {
            ExportWithSnapshotTo("MHTML", null);
        }

        /// <summary>
        /// Export to ATOM format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportATOM()
        {
            ExportWithSnapshotTo("ATOM", null);
        }

        /// <summary>
        /// Export to TIFF format
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportTIFF()
        {
            ExportWithSnapshotTo("IMAGE", "TIFF");
        }

        /// <summary>
        /// Export to PNGormat
        /// </summary>
        [TestMethod]
        public void UrlAccessSnapshotExportPNG()
        {
            ExportWithSnapshotTo("IMAGE", "PNG");
        }
        #endregion

        #region Private Methods
        
        private void ExportWithSnapshotTo(string renderFormat, string outputFormat)
        {
            string report = this.ContentManager.GetNextReport();
            string historyId = this.ContentManager.GetSnapshotIDFromCache(report);
            string url = this.ContentManager.ConstructUrl(report, renderFormat, outputFormat, null) + "&rs:Snapshot=" + historyId;
            string metricName = this.ContentManager.GetReportName(report);
            string execID;

            BeginMeasure(metricName);
            try
            {
                this.ContentManager.IssueGetRequest(url, out execID);
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
