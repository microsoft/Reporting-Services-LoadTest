// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.LoadTesting;
using RSLoad.Utilities;
using System.Collections;
using RSLoad.Utilites;

namespace RSLoad
{
    /// <summary>
    /// Plug in that run before any test to initialize pro report related runtime parameters
    /// </summary>
    public class InitContentPlugin : ILoadTestPlugin
    {
        private const int FallbackNumberOfSessions = 5;
        private const string RandomResourceSelector = "random";
        private string _sourceFolder; 
        private string _targetFolder;
        private string _itemSelector = RandomResourceSelector;
        private bool _initialized = false; // flag that tells whether we created folders and reports already.
        private List<string> _scenarios = new List<string>();
        private List<BadCombination> _badCombinations;
        private List<ReportWeight> _reportsWeight;
        private string _badCombinationsFile;
        private string _reportsWeightFile;


        /// <summary>
        /// Initialize the plugin
        /// </summary>
        /// <param name="loadTest"><see cref="LoadTest"/> context</param>
        public void Initialize(LoadTest loadTest)
        {
            string badCombinationsFile = string.Empty;
            string reportsWeightFile = string.Empty;
            LoadContextProperty(loadTest, SharedConstants.SourceReportFolderKey, ref _sourceFolder);
            LoadContextProperty(loadTest, SharedConstants.TargetReportFolderKey, ref _targetFolder);
            LoadContextProperty(loadTest, SharedConstants.RuntimeResourcesSelectorKey, ref _itemSelector);
            LoadContextProperty(loadTest, SharedConstants.BadCombinationsFileKey, ref badCombinationsFile);
            LoadContextProperty(loadTest, SharedConstants.ReportsWeightFileKey, ref reportsWeightFile);

            var loadTestScenarios = new List<string>();
            foreach (var scenario in loadTest.Scenarios)
                loadTestScenarios.Add(scenario.Name);

            Initialize(loadTestScenarios, badCombinationsFile, reportsWeightFile);

            loadTest.TestStarting += new EventHandler<TestStartingEventArgs>(TestStarting);
        }

        public void Initialize(List<string> loadTestScenarios, string badCombinationsFile, string reportsWeightFile)
        {
            _badCombinationsFile = badCombinationsFile;
            _reportsWeightFile = reportsWeightFile;
            _scenarios = loadTestScenarios;
            _initialized = InitializeResources();

            InitializeBadCombinations();
            InitializeReportWeight();
        }

        public void SetTestContextProperties(IDictionary properties, string scenarioName)
        {
            properties.AddOrReplace(SharedConstants.SourceReportFolderKey, _sourceFolder);
            properties.AddOrReplace(SharedConstants.TargetReportFolderKey, _targetFolder);
            properties.AddOrReplace(SharedConstants.RuntimeResourcesSelectorKey, _itemSelector);
            properties.AddOrReplace(SharedConstants.InitializedResourcesKey, _initialized);
            properties.AddOrReplace(SharedConstants.ScenarioListKey, _scenarios);
            properties.AddOrReplace(SharedConstants.BadCombinationsKey, _badCombinations);
            properties.AddOrReplace(SharedConstants.ReportsWeightKey, _reportsWeight);
            properties.AddOrReplace(SharedConstants.ScenarioNameKey, scenarioName);
        }

        /// <summary>
        /// Pass the values loaded in the pluging to the test context
        /// so that the test methods are able to read the properties
        /// </summary>
        private void TestStarting(object source, TestStartingEventArgs testStartingEventArgs)
        {
            SetTestContextProperties(testStartingEventArgs.TestContextProperties as IDictionary, testStartingEventArgs.ScenarioName);
            testStartingEventArgs.TestContextProperties.Add(SharedConstants.IsLoadTest, "true");
        }

        private void LoadContextProperty(LoadTest loadTest, string propertyKey, ref string varToSet)
        {
            if (loadTest.Context.ContainsKey(propertyKey))
                varToSet = loadTest.Context[propertyKey].ToString();
        }

        private bool InitializeResources()
        {
            Logging.Log("Initializing Resources in the server");
            bool result = false;
            string currentScenario = string.Empty;
            try
            {
                // The mobile reports in the Portal scenario depend on the MobileTests scenario content to be available
                if(_scenarios.Contains("Portal") && ! _scenarios.Contains("MobileTest"))
                {
                    _scenarios.Add("MobileTest");
                }
                _scenarios.Sort(); 
                foreach (string scenario in _scenarios)
                {
                    Logging.Log("Initializing Scenario {0}", scenario);
                    currentScenario = scenario;
                    IContentManager contentManager = ContentManagerFactory.GetInstance(scenario);

                    string scenarioAsFolder = scenario.Replace("_", "\\");
                    string sourceFolder = Path.Combine(SharedConstants.RuntimeResourcesFolder, scenarioAsFolder);
                    contentManager.InitializeWithResources(sourceFolder, scenario);

                    Logging.Log("Initializing Known Datasources");
                    contentManager.CreateKnownDataSources();

                    Logging.Log("Initializing Shared Datasets");
                    contentManager.PublishSharedDataSets();

                    Logging.Log("Initializing Known Reports");
                    contentManager.PublishKnownReports();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log("Initialization failed with {0} in scenario {1}", ex.Message, currentScenario);
                throw;
            }

            return (result);
        }

        private List<T> LoadSerializedList<T>(string file)
        {
            List<T> deserializedList = null;
            if (!String.IsNullOrEmpty(file))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                deserializedList = serializer.Deserialize(new StreamReader(file)) as List<T>;
            }

            return deserializedList;
        }

        private void InitializeBadCombinations()
        {
            _badCombinations = LoadSerializedList<BadCombination>(_badCombinationsFile);
        }

        private void InitializeReportWeight()
        {
            _reportsWeight = LoadSerializedList<ReportWeight>(_reportsWeightFile);
        }
    }
}