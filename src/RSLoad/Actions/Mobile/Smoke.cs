// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSLoad.Utilities;

namespace RSLoad
{
    public partial class MobileActions : PSSActionBase
    {
        [TestCategory("LongDuration")]
        [TestMethod]
        public void SmokeVerifyMobileReports()
        {
            foreach (string report in this.ContentManager.ExistingMobileReports)
            {
                try
                {
                    LoadMobileReport(report);
                }
                catch (Exception ex)
                {
                    Logging.Log("Failed on report: {0} with {1}", report, ex.Message);
                }
            }
        }
    }
}