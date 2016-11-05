// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSLoad.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace RSLoad
{
    [TestClass]
    public class MobileOnboard : PSSActionBase
    {
        private static List<string> _loadTestScenariosToDeployInServer = new List<string>() { "MobileTest" };
        [TestCategory("DependsOnDatasource")]
        [TestMethod]
        public void ValidateMyNewMobile()
        {
            LoadAllMobileReports();
        }

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
                var contentPlugin = new InitContentPlugin();
                var loadTestScenarioToUse = _loadTestScenariosToDeployInServer[0];

                if (classInit)
                {
                    contentPlugin.Initialize(_loadTestScenariosToDeployInServer, String.Empty, String.Empty);
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

        public void LoadAllMobileReports()
        {
            foreach (string report in this.ContentManager.ExistingMobileReports)
            {
                try
                {
                    Logging.Log("Starting Mobile Report {0}", report);
                    LoadMobileReport(report);
                    Logging.Log("Completed Mobile Report {0}", report);
                }
                catch (Exception ex)
                {
                    Logging.Log("Failed on report: {0} exception {1}", report, ex.Message);
                    throw;
                }
            }
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
