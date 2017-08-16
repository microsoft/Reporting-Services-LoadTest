// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.PortalAccessor.OData.Model;
using RSLoad.Utilities;
using RSTest.Common.ReportServer.Information;

namespace RSLoad
{
    [TestClass]
    public partial class PowerBIActions : PSSActionBase
    {
        private IContentManager _contentManager;
        private const string _powerbiScenario = "PowerBI_Reports";
        /// <summary>
        /// This method is called before calling any tess.
        /// </summary>
        /// <param name="testContext">Context passed in</param>
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            InitializeContextForRunningAsUnitTest(testContext, classInit: true);
            TestContextUtilities.InitializeContentManager(testContext);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            InitializeContextForRunningAsUnitTest(this.TestContext, classInit: false);
        }

        private static void InitializeContextForRunningAsUnitTest(TestContext testContext, bool classInit)
        {
            if (!testContext.Properties.Contains(SharedConstants.IsLoadTest))
            {
                var loadTestScenariosToDeployInServer = new List<string>() { _powerbiScenario };
                var contentPlugin = new InitContentPlugin();
                var loadTestScenarioToUse = loadTestScenariosToDeployInServer[0];

                if (classInit)
                {
                    contentPlugin.Initialize(
                        loadTestScenariosToDeployInServer, 
                        Path.Combine(SharedConstants.RuntimeResourcesFolder, @"Paginated\BadCombinations.xml"),
                        Path.Combine(SharedConstants.RuntimeResourcesFolder, @"Paginated\ScaleReportsWeight.xml"));
                }
                contentPlugin.SetTestContextProperties(testContext.Properties, loadTestScenarioToUse);
            }
        }

        protected IContentManager ContentManager
        {
            get
            {
                if (_contentManager == null)
                {
                    var currentScenarioName = string.Empty;
                    TestContextUtilities.LoadContextProperty(this.TestContext, SharedConstants.ScenarioNameKey, ref currentScenarioName);
                    if (string.IsNullOrEmpty(currentScenarioName))
                        throw new Exception("The scenario name is not available, in the test, the method need to be executed from a load test in VSTS.");

                    _contentManager = ContentManagerFactory.GetInstance(currentScenarioName);
                }

                return _contentManager;
            }
        }

        [TestCategory("PBI")]
        [TestMethod]
        public void UsePowerBIReports()
        {
            var report = ContentManager.GetNextCatalogItem("PowerBIReport");

            var context = this.ContentManager.PortalAccessorV1.CreateContext();
            var executionCredentails = ExtractExecutionCredentails();

            var pbiReport = context.CatalogItemByPath(report).GetValue() as PowerBIReport;
            PowerBIClient.SimulatePowerBIReportUsage(executionCredentails, pbiReport, pbiReport.Name);
        }

        [TestCategory("PBI")]
        [TestMethod]
        public void UsePowerBIReportsEmbeddedReuseModel()
        {
            var report = ContentManager.GetNextCatalogItem("PowerBIReportEmbedded");

            var context = this.ContentManager.PortalAccessorV1.CreateContext();
            var executionCredentails = ExtractExecutionCredentails();

            var pbiReport = context.CatalogItemByPath(report).GetValue() as PowerBIReport;
            PowerBIClient.SimulatePowerBIReportUsage(executionCredentails, pbiReport, pbiReport.Name);
        }

        [TestCategory("PBI")]
        [TestMethod]
        public void UsePowerBIReportsEmbeddedStreamNewModel()
        {
            string originalReportName;
            var reportOnServer = PublishUniqueReportOnServer(out originalReportName);
            var context = this.ContentManager.PortalAccessorV1.CreateContext();
            var executionCredentails = ExtractExecutionCredentails();

            var pbiReport = context.CatalogItemByPath(reportOnServer).GetValue() as PowerBIReport;
            PowerBIClient.SimulatePowerBIReportUsage(executionCredentails, pbiReport, originalReportName);
        }

        private static ICredentials ExtractExecutionCredentails()
        {
            ICredentials executionCredentails = CredentialCache.DefaultNetworkCredentials;
            if (!string.IsNullOrEmpty(ReportServerInformation.DefaultInformation.ExecutionAccount))
            {
                var myCache = new CredentialCache();
                var reportServerUri = new Uri(ReportServerInformation.DefaultInformation.ReportServerUrl);
                myCache.Add(new Uri(reportServerUri.GetLeftPart(UriPartial.Authority)),
                    "NTLM",
                    new NetworkCredential(
                        ReportServerInformation.DefaultInformation.ExecutionAccount,
                        ReportServerInformation.DefaultInformation.ExecutionAccountPwd));
                executionCredentails = myCache;
            }
            return executionCredentails;
        }

        private string PublishUniqueReportOnServer(out string origFileName)
        {
            var scenarioAsFolder = _powerbiScenario.Replace("_", "\\");
            var sourceFolder = Path.Combine(SharedConstants.RuntimeResourcesFolder, scenarioAsFolder);
            var reportFolder = Path.Combine(Directory.GetCurrentDirectory(), sourceFolder);

            var di = new DirectoryInfo(reportFolder);
            var files = di.GetFiles("*Embedded.pbix");
            var reportFile = ContentManager.ItemSelector.GetItem(files);

            origFileName = reportFile.Name.Substring(0, reportFile.Name.IndexOf(reportFile.Extension, StringComparison.Ordinal));
            var displayName = origFileName + DateTime.Now.ToFileTime();
           
            var reportOnDisk = Path.Combine(reportFolder, reportFile.Name);
            var reportOnServer = ContentManager.PublishReport(reportOnDisk, displayName);
            return reportOnServer;
        }
    }
}