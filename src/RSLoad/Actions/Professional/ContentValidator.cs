using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.SoapAccessor;
using RSAccessor.Utilities;
using RSLoad.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RSLoad.Utilites;

namespace RSLoad
{
    [TestClass]
    public class ContentValidator
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
            ServicePointManager.DefaultConnectionLimit = 50;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            InitializeContextForRunningAsUnitTest(this.TestContext, classInit: false);
        }

        private static List<string> _loadTestScenariosToDeployInServer;
        private static bool _isUnitTest = false;
        private static void InitializeContextForRunningAsUnitTest(TestContext testContext, bool classInit)
        {
            if (!testContext.Properties.Contains(SharedConstants.IsLoadTest))
            {
                _isUnitTest = true;
                _loadTestScenariosToDeployInServer = new List<string>() { "Paginated_Small", "Paginated_Large", "Paginated_General", "Paginated_GeneralMassive", "Paginated_NewReports", "Paginated_NoDatasource", "Paginated_Performance", "Paginated_PGQL", "Paginated_SubReports" };
                var contentPlugin = new InitContentPlugin();
                var loadTestScenarioToUse = _loadTestScenariosToDeployInServer[0];

                if (classInit)
                {
                    contentPlugin.Initialize(
                        _loadTestScenariosToDeployInServer,
                        Path.Combine(SharedConstants.RuntimeResourcesFolder, @"Paginated\BadCombinations.xml"),
                        Path.Combine(SharedConstants.RuntimeResourcesFolder, @"Paginated\ScaleReportsWeight.xml"));
                }
                contentPlugin.SetTestContextProperties(testContext.Properties, loadTestScenarioToUse);
            }
        }

        [TestCategory("DependsOnDatasource")]
        [TestMethod]
        [Timeout(7200000)]
        public void ValidatePaginated_General()
        {
            //ValidateAllReportsRenderHTML5_UnitTest("Paginated_General");
        }

        [TestCategory("LongDuration"), TestCategory("DependsOnDatasource")]
        [TestMethod]
        [Timeout(7200000)]
        public void ValidatePaginated_GeneralMassive()
        {
            //ValidateAllReportsRenderHTML5_UnitTest("Paginated_GeneralMassive");
        }

        [TestCategory("LongDuration"), TestCategory("DependsOnDatasource")]
        [TestMethod]
        [Timeout(7200000)]
        public void ValidatePaginated_Large()
        {
            ValidateAllReportsRenderHTML5_UnitTest("Paginated_Large");
        }

        [TestCategory("DependsOnDatasource")]
        [TestMethod]
        public void ValidatePaginated_NewReports()
        {
            ValidateAllReportsRenderHTML5_UnitTest("Paginated_NewReports");
        }

        [TestCategory("DependsOnDatasource")]
        [TestMethod]
        public void ValidatePaginated_NoDatasource()
        {
            ValidateAllReportsRenderHTML5_UnitTest("Paginated_NoDatasource");
        }

        [TestCategory("MinutesDuration"), TestCategory("DependsOnDatasource")]
        [TestMethod]
        [Timeout(7200000)]
        public void ValidatePaginated_Performance()
        {
            ValidateAllReportsRenderHTML5_UnitTest("Paginated_Performance");
        }

        [TestCategory("DependsOnDatasource")]
        [TestCategory("MinutesDuration")]
        [TestMethod]
        public void ValidatePaginated_PGQL()
        {
            ValidateAllReportsRenderHTML5_UnitTest("Paginated_PGQL");
        }

        [TestCategory("DependsOnDatasource")]
        [TestMethod]
        public void ValidatePaginated_Small()
        {
            ValidateAllReportsRenderHTML5_UnitTest("Paginated_Small");
        }

        [TestCategory("DependsOnDatasource")]
        [TestMethod]
        public void ValidatePaginated_SubReports()
        {
            ValidateAllReportsRenderHTML5_UnitTest("Paginated_SubReports");
        }

        private void ValidateAllReportsRenderHTML5_UnitTest(string scenario)
        {
            if (!_isUnitTest)
                Assert.Fail("This test is not valid to run as a LoadTest");

            HTML50Render renderFormat = new HTML50Render();
            string url;
            bool allReportsRendered = true;
            var failedReports = new List<string>();
            var contentManager = ContentManagerFactory.GetInstance(scenario);
            Parallel.ForEach(contentManager.ExistingReports, (report) =>
            {
                try
                {
                    Logging.Log("Starting Rendering Scenario {0} , Report {1}", scenario, report);
                    RSExecutionInfo info = contentManager.SoapAccessor.Execution.LoadReport(report, null);
                    RSExecutionInfo execInfo = contentManager.SoapAccessor.Execution.GetExecutionInfo();
                    url = contentManager.ConstructUrl(report, renderFormat, null, 1, 1, execInfo.ExecutionID, null);
                    string execid;
                    contentManager.IssueGetRequest(url, out execid);
                    Logging.Log("Completed Rendering Scenario {0} , Report {1}", scenario, report);
                }
                catch (Exception ex)
                {
                    Logging.Log("Report failed to render, scenario {0}, report {1} exception {2}", scenario, report, ex);
                    failedReports.Add(report);
                    allReportsRendered = false;
                }
            });

            Assert.IsTrue(allReportsRendered, "Reports failed to render {0}", failedReports.ToNewLineSeparatedString());
        }

        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }

            set
            {
                _testContextInstance = value;
            }
        }
        private TestContext _testContextInstance;
    }
}
