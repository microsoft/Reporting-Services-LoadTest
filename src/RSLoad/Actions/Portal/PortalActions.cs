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
    public class PortalActions : PSSActionBase
    {
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
                var loadTestScenariosToDeployInServer = new List<string>() { "Portal" };
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

        private IContentManager _contentManager;

        public PortalActions()
        {
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
        public void ListReports()
        {
            using (Measure("ListReports"))
            {
                ContentManager.BrowseCatalogItems(ContentManager.WorkingFolder, "Report");
            }
        }

        [TestMethod]
        public void ListMobileReports()
        {
            using (Measure("ListMobileReports"))
            {
                ContentManager.BrowseCatalogItems(ContentManager.WorkingFolder, "MobileReport");
            }
        }

        [TestMethod]
        public void ListKpis()
        {
            using (Measure("ListKpis"))
            {
                ContentManager.BrowseCatalogItems(ContentManager.WorkingFolder, "Kpi");
            }
        }

        [TestMethod]
        public void ListDataSources()
        {
            using (Measure("ListDataSources"))
            {
                ContentManager.BrowseCatalogItems(ContentManager.WorkingFolder, "DataSource");
            }
        }

        [TestMethod]
        public void ListFolders()
        {
            using (Measure("ListFolders"))
            {
                ContentManager.BrowseCatalogItems(ContentManager.WorkingFolder, "Folder");
            }
        }

        [TestMethod]
        public void ListDataSets()
        {
            using (Measure("ListDataSets"))
            {
                ContentManager.BrowseCatalogItems(ContentManager.WorkingFolder, "DataSet");
            }
        }

        [TestMethod]
        public void GetReportItem()
        {
            string report = ContentManager.GetNextReport();
            using (Measure("GetReportItem"))
            {
                ContentManager.GetCatalogItem(report, expand: null);
            }
        }

        [TestMethod]
        public void GetReportItemExpandDataSources()
        {
            GetReportItemAndExpand("GetReportItemExpandDataSources", "DataSources");
        }

        [TestMethod]
        public void GetReportItemExpandSubscriptions()
        {
            GetReportItemAndExpand("GetReportItemExpandSubscriptions", "Subscriptions");
        }

        [TestMethod]
        public void GetReportItemExpandCacheRefreshPlans()
        {
            GetReportItemAndExpand("GetReportItemExpandCacheRefreshPlans", "CacheRefreshPlans");
        }

        [TestMethod]
        public void GetReportItemExpandReportHistorySnapshots()
        {
            GetReportItemAndExpand("GetReportItemExpandCacheRefreshPlans", "ReportHistorySnapshots");
        }

        [TestMethod]
        public void GetReportItemDependentItems()
        {
            string report = ContentManager.GetNextReport();
            using (Measure("GetReportItemDependentItems"))
            {
                ContentManager.GetDependentItems(report);
            }
        }

        private void GetReportItemAndExpand(string name, string propertiesToExpand)
        {
            string report = ContentManager.GetNextReport();
            using (Measure(name))
            {
                ContentManager.GetCatalogItem(report, expand: propertiesToExpand);
            }
        }
    }
}