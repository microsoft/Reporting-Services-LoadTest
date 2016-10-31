// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RSLoad.Utilities
{
    internal static class TestContextUtilities
    {
        internal static void LoadContextProperty<T>(TestContext testContext, string propertyKey, ref T varToSet)
        {
            if (testContext.Properties.Contains(propertyKey))
                varToSet = (T)testContext.Properties[propertyKey];
        }

        internal static ICollectionSelector InitializeContentManager(TestContext testContext)
        {
            string sourceFolder = string.Empty;
            string targetFolder = string.Empty;
            bool reportsCreated = false;
            List<string> scenarioList = null;
            string runtimeResourcesSelector = null;
            List<BadCombination> badCombinations = null;
            List<ReportWeight> reportsWeight = null;

            Logging.Log("Initialize Content for the test");

            // Get runtime parameters
            LoadContextProperty(testContext, SharedConstants.SourceReportFolderKey, ref sourceFolder);
            LoadContextProperty(testContext, SharedConstants.TargetReportFolderKey, ref targetFolder);
            LoadContextProperty(testContext, SharedConstants.ScenarioListKey, ref scenarioList);
            LoadContextProperty(testContext, SharedConstants.RuntimeResourcesSelectorKey, ref runtimeResourcesSelector);
            LoadContextProperty(testContext, SharedConstants.InitializedResourcesKey, ref reportsCreated);
            LoadContextProperty<List<BadCombination>>(testContext, SharedConstants.BadCombinationsKey, ref badCombinations);
            LoadContextProperty<List<ReportWeight>>(testContext, SharedConstants.ReportsWeightKey, ref reportsWeight);

            ICollectionSelector itemSelector = null;

            if (String.IsNullOrEmpty(runtimeResourcesSelector))
                itemSelector = new RandomSelector();
            else
            {
                if (runtimeResourcesSelector.Equals("random", StringComparison.InvariantCultureIgnoreCase))
                    itemSelector = new RandomSelector();
                else
                    itemSelector = new SequentialSelector();
            }

            foreach (string scenario in scenarioList)
            {
                IContentManager contentManager = ContentManagerFactory.GetInstance(scenario);
                contentManager.Initialize(scenario, scenario);
                contentManager.ItemSelector = itemSelector;
                contentManager.PopulateReportListFromServer();
                contentManager.BadReportMethodCombinations = badCombinations;
                contentManager.ReporstWeight = reportsWeight;
            }

            return itemSelector;
        }
    }
}