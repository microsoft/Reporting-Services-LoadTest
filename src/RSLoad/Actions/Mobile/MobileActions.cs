// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using RSLoad.Utilities;

namespace RSLoad
{
    [TestClass]
    public partial class MobileActions : PSSActionBase
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
                var loadTestScenariosToDeployInServer = new List<string>() { "MobileTest", "Paginated_NoDatasource" };
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

        [TestMethod]
        public void LoadMobileReports()
        {
            string report = ContentManager.GetNextCatalogItem("MobileReport");
            LoadMobileReport(report);
        }

        internal void LoadMobileReport(string report)
        {
            string measureName = string.Format("LoadMobileReport: {0}", report);
            using (Measure(measureName))
            {
                MobileClient.LoadMobileReport(this.ContentManager.PortalAccessorV1, report);
            }
        }
    }
}