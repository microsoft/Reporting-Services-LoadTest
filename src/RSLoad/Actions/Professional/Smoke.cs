// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSTest.Common.ReportServer.Information;
using RSLoad.Utilities;

namespace RSLoad
{
    /// <summary>
    /// All SOAP API tests
    /// Mostly used in stress.
    /// Maybe used in perf.
    /// </summary>
    public partial class PaginatedActions : PSSActionBase
    {
        #region Smoke Tests
        /// <summary>
        /// Verify all reports in the collection can be rendered.
        /// Should not run as prt of the test
        /// </summary>
        [TestMethod]
        public void SmokeVerifyReports()
        {
            foreach (string report in this.ContentManager.ExistingReports)
            {
                try
                {
                    SimulateFirstPageLoad(report);
                }
                catch (Exception ex)
                {
                    Logging.Log("Failed on report: {0} with {1}", report, ex.Message);
                }
            }
        }
        #endregion


        /// <summary>
        /// Call the portal
        /// </summary>
        [TestMethod]
        public void NewPortal()
        {
            string execid;
            this.ContentManager.IssueGetRequest(ReportServerInformation.DefaultInformation.ReportPortalUrl, out execid);
        }

    }
}
