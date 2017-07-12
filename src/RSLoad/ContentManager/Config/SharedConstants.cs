// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

namespace RSLoad
{
    /// <summary>
    /// Keep all constant together, at least the sharable one, move them to one file.
    /// </summary>
    public class SharedConstants
    {
        public const int StaticRandomSeed = 123456;

        // Constants with the name/identifier of the context parameters exposed and processed by the InitProReportsPlugin
        public const string InitializedResourcesKey = "InitializedResources";
        public const string ScenarioNameKey = "ScenarioName";
        public const string ScenarioListKey = "ScenarioList";
        public const string BadCombinationsFileKey = "BadCombinationsFile";
        public const string ReportsWeightFileKey = "ReportsWeightFile";
        public const string BadCombinationsKey = "BadCombinations";
        public const string ReportsWeightKey = "ReportsWeight";
        public const string SourceReportFolderKey = "SourceReportFolder";
        public const string TargetReportFolderKey = "TargetReportFolder";
        public const string RuntimeResourcesSelectorKey = "RuntimeResourcesSelector";
        public const string IsLoadTest = "IsLoadTest";
        public const string RuntimeResourcesFolder = @"RuntimeResources";

        public const string ApiV1PostFix = "api/v1.0";
        public const string ApiV2PostFix = "api/v2.0";
        public const string PbiEndpoint = "powerbi/api/explore/reports";
    }
}
