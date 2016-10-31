// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using RSLoad.Utilities;

namespace RSLoad
{
    /// <summary>
    /// Profesional Report Actions
    /// In this file, we only initialize. 
    /// Test methods are implemented in different files with partial class
    /// Groupings:
    ///     Export With Session
    ///     Export Live
    ///     Export with Snapshot
    ///     SOAP APIs (stress only)
    ///     ViewerControl Simulation
    ///     Print and other render (full and page by page)
    /// </summary>
    [TestClass]
    public partial class PaginatedActions : PSSActionBase
    {
        /// <summary>
        /// Constructor of the test action class
        /// </summary>
        public PaginatedActions()
        {
        }

        private IContentManager _contentManager = null;

        /// <summary>
        /// Content manager for the current scenario
        /// </summary>
        protected IContentManager ContentManager
        {
            get
            {
                if (_contentManager == null)
                {
                    string currentScenarioName = string.Empty;
                    TestContextUtilities.LoadContextProperty(this.TestContext, SharedConstants.ScenarioNameKey, ref currentScenarioName);
                    if (String.IsNullOrEmpty(currentScenarioName))
                        throw new Exception("The scenario name is not available, in the test, the mehod need to be executed from a load test in vsts");

                    _contentManager = ContentManagerFactory.GetInstance(currentScenarioName);
                }

                return _contentManager;
            }
        }

        #region Initializer

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
                var loadTestScenariosToDeployInServer = new List<string>() { "Paginated_NoDatasource" };
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

        #endregion
    }
}
