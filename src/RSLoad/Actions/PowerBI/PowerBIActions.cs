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
                var loadTestScenariosToDeployInServer = new List<string>() { "PowerBI_Reports", "Paginated_NoDatasource" };
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
            string report = ContentManager.GetNextCatalogItem("PowerBIReport");

            var context = this.ContentManager.PortalAccessorV1.CreateContext();

            CredentialCache myCache = new CredentialCache();
            Uri reportServerUri = new Uri(ReportServerInformation.DefaultInformation.ReportServerUrl);
            myCache.Add(new Uri(reportServerUri.GetLeftPart(UriPartial.Authority)), "NTLM", new NetworkCredential(ReportServerInformation.DefaultInformation.ExecutionAccount, ReportServerInformation.DefaultInformation.ExecutionAccountPwd));
            var _executionCredentials = myCache;
            
            PowerBIReport pbiReport = context.CatalogItemByPath(report).GetValue() as PowerBIReport;

            PowerBIClient.SimulatePowerBIReportUsage(_executionCredentials, pbiReport);
        }
    }
}