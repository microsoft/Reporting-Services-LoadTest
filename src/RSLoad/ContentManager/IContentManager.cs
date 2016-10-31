// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using RSAccessor.SoapAccessor;
using RSAccessor.UrlAccessor;
using RSAccessor.Utilities;
using RSAccessor.PortalAccessor;

namespace RSLoad
{
    /// <summary>
    /// Content Manager Interface
    ///     - Reports
    ///     - Data sources
    ///     - Shared Data sources
    ///     - Component libraries
    ///     - Snapshots
    ///     - Sessions, etc.
    /// </summary>
    public interface IContentManager
    {
        #region Properties

        /// <summary>
        /// Url Accesor to interact with Reporting Services Server
        /// </summary>
        RSUrlAccessor URLAccessor { get; }

        /// <summary>
        /// SOAP Accessor to interact with Reporting Services Server
        /// </summary>
        RSSoapAccessor SoapAccessor { get; }

        /// <summary>
        /// Portal Accessor to interact with Reporting Services Server
        /// </summary>
        RSPortalAccessorV1 PortalAccessorV1 { get; }

        /// <summary>
        /// Working folder on report server
        /// </summary>
        string WorkingFolder { get; }

        /// <summary>
        /// Folder where actual reports exist in disk
        /// </summary>
        string ReportFolder { get; }

        /// <summary>
        /// List of existing reports
        /// </summary>
        List<string> ExistingReports { get; }

        List<string> ExistingMobileReports { get; }

        List<string> ExistingKpis { get; }

        List<string> ExistingPowerBIReports { get; }

        /// <summary>
        /// Reports with already generated session ids.
        /// Note that, this only works in sticky sessions.
        /// </summary>
        Dictionary<string, string> SessionIDs { get; }

        /// <summary>
        /// Keeps list of SnapshotID (HistoryIDs) for all Existing Reports
        /// </summary>
        ConcurrentDictionary<string, string> ReportSnapshots { get; }

        /// <summary>
        /// Base Source Folder (where reports exist)
        /// </summary>
        string SourceFolder { get; set; }
        
        /// <summary>
        /// Base Target Folder (report server)
        /// </summary>
        string TargetFolder { get; set; }

        /// <summary>
        /// Temporary content holder.  All random SOAP API tests will use this folder
        /// to create, modify, and delete items.
        /// </summary>
        string ToBeDeletedFolder { get; set; }

        /// <summary>
        /// Get random content manager - API to manage temporaary RS objects
        /// </summary>
        RandomContentManager RndContentManager { get; }

        /// <summary>
        /// Item selector, option is random or sequential.  It is decided at runtime based on provided parameter.  
        /// Default is random.
        /// </summary>
        ICollectionSelector ItemSelector { get; set; }

        /// <summary>
        /// Local Random Variable.  We can use this if we trully want just randome.
        /// </summary>
        Random LocalRand { get; }
        #endregion

        #region Methods

        #region Initializer
        /// <summary>
        /// Initializes necessary information provided source and target folder
        /// </summary>
        /// <param name="sourceFolder">Source Folder where report actually exists</param>
        /// <param name="targetFolder">Target Folder where we upload reports to</param>
        void Initialize(string sourceFolder, string targetFolder);

        /// <summary>
        /// Initializes necessary information provided source and target folder and deploys resources
        /// </summary>
        /// <param name="sourceFolder">Source Folder where report actually exists</param>
        /// <param name="targetFolder">Target Folder where we upload reports to</param>
        void InitializeWithResources(string sourceFolder, string targetFolder);

        #endregion

        #region Data Source
        /// <summary>
        /// Create known/default data sources for PSS
        /// </summary>
        void CreateKnownDataSources();

        /// <summary>
        /// Create data sources from given definition file using serverName.
        /// </summary>
        /// <param name="datasourceFile">File containing data source definition</param>
        /// <param name="targetFolder">RS Target Folder</param>
        /// <param name="fileExtension">File extension</param>
        void CreateDataSources(string datasourceFile, string targetFolder, string fileExtension);

        /// <summary>
        /// Create Data Source On RS Server
        /// </summary>
        /// <param name="name">Data Source Name</param>
        /// <param name="dsDef">Data Source Definition</param>
        /// <param name="parent">Parent Folder to Crate Data Source</param>
        /// <returns>Path to newly created datasource</returns>
        string CreateDataSource(string name, RSDataSourceDefinition dsDef, string parent);

        /// <summary>
        /// Creates data source definition object with given values
        /// </summary>
        /// <param name="extension">Extension To User for Data Source</param>
        /// <param name="connectString">Connection String - different format for different extension</param>
        /// <param name="enabled">Whether data source is enabled</param>
        /// <param name="prompt">Whether to prompt for credential</param>
        /// <param name="imperseronateUser">Is using impersonation</param>
        /// <param name="windowsCred">Is using Windows Credential</param>
        /// <param name="user">user name (name or domain\name)</param>
        /// <param name="password">password for user above</param>
        /// <returns>Data Source Definition</returns>
        RSDataSourceDefinition CreateDatasourceDefinition(string extension, string connectString, bool enabled, string prompt, bool imperseronateUser, bool windowsCred, string user, string password);

        #endregion

        #region SharedDataSet
        /// <summary>
        /// Publish SharedDataSets.
        /// </summary>
        void PublishSharedDataSets();

        /// <summary>
        /// Publish SharedDataSets from srcFolder (Disk) to destFolder (RS)
        /// </summary>
        /// <param name="srcFolder">Source Folder where RSDs are</param>
        /// <param name="destFolder">Destination Folder in RS backend</param>
        void PublishSharedDataSets(string srcFolder, string destFolder);

        /// <summary>
        /// Publish given SharedDataSet to report server
        /// </summary>
        /// <param name="sharedDataSet">SharedDataSet on physical disk</param>
        /// <param name="displayName">Name of the SharedDataSet on the server</param>
        void PublishSharedDataSet(string sharedDataSet, string displayName);

        /// <summary>
        /// Helper function to create a SharedDataSet quickly.
        /// </summary>
        /// <param name="sharedDataSet">RDS File path to publish</param>
        /// <param name="displayName">Display Name of SharedDataSet</param>
        /// <param name="parentFolder">Parent Folder to create SharedDataSet in</param>
        /// <returns>Final path on server</returns>
        void PublishSharedDataSet(string sharedDataSet, string displayName, string parentFolder);


        /// <summary>
        /// Helper function to create a SharedDataSet quickly ignoring the SOAP exception
        /// Microsoft.ReportingServices.Diagnostics.Utilities.ItemAlreadyExistsException
        /// </summary>
        /// <param name="sharedDataSet">RDS File path to publish</param>
        /// <param name="displayName">Display Name of SharedDataSet</param>
        /// <param name="parentFolder">Parent Folder to create SharedDataSet in</param>
        void TryPublishSharedDataSet(string sharedDataSet, string displayName, string parentFolder);
        #endregion

        #region Report Publishing
        /// <summary>
        /// Upload reports to server
        /// A location should be provided, for now lets start with static.
        /// - /General in native mode
        /// </summary>
        void PublishKnownReports();

        /// <summary>
        /// Populate Existing Reports through ListChildren
        /// </summary>
        void PopulateReportListFromServer();

        /// <summary>
        /// For each report int the list, create session 
        /// </summary>
        void CreateSessionIDsFromKnownReports();

        /// <summary>
        /// Creates snapshots for known Reports 
        /// </summary>
        void CreateReportSnapshotsForKnownReports();

        /// <summary>
        /// Create new report snapshot and add to ReportSnapshots collection
        /// </summary>
        /// <param name="report">Full report path</param>
        /// <returns>SnapshotId aka HistoryId</returns>
        string CreateReportSnapshot(string report);

        /// <summary>
        /// Publish Reports From srcFolder (Disk) to destFolder (RS)
        /// </summary>
        /// <param name="srcFolder">Source Folder where RDLs are</param>
        /// <param name="destFolder">Destination Folder in RS backend</param>
        void PublishReports(string srcFolder, string destFolder);

        /// <summary>
        /// Publish given report to report server
        /// </summary>
        /// <param name="report">Report on physical disk</param>
        /// <param name="displayName">Name of the report on the server</param>
        /// <returns>server report path</returns>
        string PublishReport(string report, string displayName);

        /// <summary>
        /// Helper function to create a report quickly.
        /// </summary>
        /// <param name="report">RDL File path to publish</param>
        /// <param name="displayName">Display Name of Report</param>
        /// <param name="parentFolder">Parent Folder to create report in</param>
        /// <returns>Final path on server</returns>
        string PublishReport(string report, string displayName, string parentFolder);

        #endregion

        #region Exporting
        /// <summary>
        /// Construct Url to make URL access request.
        /// </summary>
        /// <param name="reportPath">Report Path</param>
        /// <param name="renderFormat">Render Format and settings</param>
        /// <param name="parameterValues">Parameter for report</param>
        /// <returns>Constructed Url</returns>
        string ConstructUrl(string reportPath, RSRenderStruct renderFormat, NameValueCollection parameterValues);

        /// <summary>
        /// Construct Url based on parameer provided
        /// </summary>
        /// <param name="reportPath">Report Path</param>
        /// <param name="exportFormat">Export Format (PDF, CSV, ...)</param>
        /// <param name="parameterValues">Report Parameters</param>
        /// <returns>Constructed Url</returns>
        string ConstructUrl(string reportPath, string exportFormat, NameValueCollection parameterValues);

        /// <summary>
        /// Construct Url based on parameer provided which also included outputFormat (image type)
        /// </summary>
        /// <param name="reportPath">>Report Path</param>
        /// <param name="exportFormat">Export Format (PDF, CSV, ...)</param>
        /// <param name="outputFormat">Output Format (TIFF, PNG, ...)</param>
        /// <param name="parameterValues">Report Parameters</param>
        /// <returns>Constructed Url</returns>
        string ConstructUrl(string reportPath, string exportFormat, string outputFormat, NameValueCollection parameterValues);

        /// <summary>
        /// Construct Url based on parameer provided which also included outputFormat (image type)
        /// </summary>
        /// <param name="reportPath">>Report Path</param>
        /// <param name="renderFormat">Render Format and settings</param>
        /// <param name="snapshotID">If available snapshot id</param>
        /// <param name="startPage">starting page</param>
        /// <param name="endPage">ending page</param>
        /// <param name="sessiondId">session id</param>
        /// <param name="parameterValues">Parameter values</param>
        /// <returns>Fully constructed Url</returns>
        string ConstructUrl(string reportPath, RSRenderStruct renderFormat, string snapshotID, int startPage, int endPage, string sessiondId, NameValueCollection parameterValues);

        /// <summary>
        /// Construct Render Struct from give parameters
        /// </summary>
        /// <param name="exportFormat">Render Format</param>
        /// <param name="outputFormat">Output format if needed</param>
        /// <returns>RS Render Struct</returns>
        RSRenderStruct ConstructRenderStruct(string exportFormat, string outputFormat);

        /// <summary>
        /// Add more parameters to existing Url
        /// </summary>
        /// <param name="url">Consturced Url</param>
        /// <param name="snapshotid">Snapshot History Id if existed</param>
        /// <param name="startpage">Starting Page</param>
        /// <param name="endpage">Ending Page</param>
        /// <param name="paginationMode">Pagination Mode</param>
        /// <returns>Newly constructed Url</returns>
        string AddMoreparamsToUrl(string url, string snapshotid, int startpage, int endpage, string paginationMode);

        /// <summary>
        /// Main export work is done here.  Few things still need to be parameterized
        /// customAuthCookies and other credentials, TimeOut, and any other stuff.
        /// </summary>
        /// <param name="url">Fully constructed URL for report server</param>
        /// <param name="executionID">ExecutionID of the request</param>
        void IssueGetRequest(string url, out string executionID);

        /// <summary>
        /// Given a url, create HttpWebRequest object
        /// Centralized the creation so all have same parameters.
        /// </summary>
        /// <param name="url">Url for creating object</param>
        /// <returns>Created HttpWebRequest object</returns>
        HttpWebRequest CreateHttpWebRequest(string url);

        #endregion

        #region Information
        /// <summary>
        /// Read Content from given item
        /// </summary>
        /// <param name="itemPath">Physical path to item to read</param>
        /// <returns>Byte array from read content</returns>
        byte[] GetItemCotent(string itemPath);

        /// <summary>
        /// Get Report Page Count (in normal view mode)
        /// </summary>
        /// <param name="report">Report Path</param>
        /// <returns>Page Count of the given report</returns>
        int GetPageCount(string report);

        /// <summary>
        /// Get SessionID for given report, if not existence or expired, create one.
        /// </summary>
        /// <param name="report">exiting report path</param>
        /// <returns>SessionID aka ExecutionID</returns>
        string GetSessionID(string report);

        /// <summary>
        /// Get SnaphshotID (aka HistoryID) for a given report.  If it does not exist, create one.
        /// NOTE: sometime this may have expiration time so will need management of that.
        /// </summary>
        /// <param name="report">Existing Report Path</param>
        /// <returns>Snapshot ID (History ID)</returns>
        string GetSnapshotIDFromCache(string report);

        /// <summary>
        /// Get Report Name (short) from give full path of report
        /// </summary>
        /// <param name="reportPath">Full Path Of report</param>
        /// <returns>Report Name without path</returns>
        string GetReportName(string reportPath);

        /// <summary>
        /// Generate Random name (should be valid item name)
        /// </summary>
        /// <returns>Item name with proper extesnion</returns>
        string GenerateRndFileName();

        /// <summary>
        /// Given extesnion, returns empty or actual extension if applicable.
        /// </summary>
        /// <param name="ext">extesnion name</param>
        /// <returns>String.Empty or actual extension</returns>
        string GetExtension(string ext);

        /// <summary>
        /// Get next report item (random/sequetial)
        /// It gets from existing reports collection.
        /// </summary>
        /// <returns>Report Path That Exists</returns>
        string GetNextReport();

        /// <summary>
        /// Get next catalog item (random/sequetial) specified by parameter 
        /// It gets from existing catalog item collection.
        /// </summary>
        /// <param name="type">Name of the type</param>
        /// <returns>Catalog item path that exists</returns>
        string GetNextCatalogItem(string type);

        /// <summary>
        /// Check whether report contains snapshot.  
        /// If report does not have, caller should remove from collection, too.
        /// </summary>
        /// <param name="report">Full path of report</param>
        /// <returns>Has snapshot or not</returns>
        bool HasReportSnapshot(string report);

         /// <summary>
        /// GetReportSnapshot from Report (not from cache)
        /// </summary>
        /// <param name="report">Report Path</param>
        /// <returns>Snapshot Id</returns>
        string GetReportSnapshotId(string report);

        /// <summary>
        /// Define the set of reports and a list of methods that should not be executed
        /// </summary>
        List<BadCombination> BadReportMethodCombinations { set; } 

        /// <summary>
        /// Set a distribution for the reports based on weight
        /// if this value is not used every report have the same chance to be returned in GetNextReport()
        /// the string is the name of the report and the int is the weight that will be assigned during the 
        /// selection of the report in GetNextReport()
        /// </summary>
        List<ReportWeight> ReporstWeight { set; }
        #endregion

        #region SOAP APIS
        /// <summary>
        /// Creates a generic report given the name and parent folder.
        /// Name alerady supposed to have extesion
        /// </summary>
        /// <param name="name">Item name to be used on the server</param>
        /// <param name="parent">Parent folder</param>
        /// <returns>Full Path of newly created item</returns>
        string CreateGenericReport(string name, string parent);
        #endregion

        #region Browsing

        /// <summary>
        /// Simulates a browse operation on the catalog.
        /// </summary>
        /// <param name="path">Path of the folder to browse in.</param>
        /// <param name="type">Name of the catalog item type (e.g. Report, DataSource).</param>
        void BrowseCatalogItems(string path, string type);

        /// <summary>
        /// Simulates a query agains a catalog item.
        /// </summary>
        /// <param name="path">Path of the catalog item on the server.</param>
        /// <param name="expand">Name of the section to expand (e.g. DataSources, ReportHistorySnapshots)</param>
        void GetCatalogItem(string path, string expand);

        /// <summary>
        /// Simulates a query agains a catalog item's dependent items.
        /// </summary>
        /// <param name="path">Path of the catalog item on the server.</param>
        void GetDependentItems(string path);

        #endregion

        #endregion
    }
}
