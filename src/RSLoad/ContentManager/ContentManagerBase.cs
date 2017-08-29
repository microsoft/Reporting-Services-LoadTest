// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.SoapAccessor;
using RSAccessor.UrlAccessor;
using RSAccessor.Utilities;
using RSTest.Common.ReportServer.Information;
using RSAccessor.PortalAccessor;
using System.Text;
using System.Web.Services.Protocols;
using RSLoad.Utilities;

namespace RSLoad
{
    /// <summary>
    /// Content Manager base - common for all implentation
    /// </summary>
    internal abstract class ContentManagerBase : IContentManager
    {
        #region Properties

        private SessionManager sessionIdManager;

        /// <summary>
        /// Creates a contentManager and its session manager
        /// </summary>
        public ContentManagerBase()
        {
            sessionIdManager = new SessionManager(this);
        }

        /// <summary>
        /// Url Accesor to interact with Reporting Services Server
        /// </summary>
        public RSUrlAccessor URLAccessor
        {
            get;
            protected set;
        }

        /// <summary>
        /// SOAP Accessor to interact with Reporting Services Server
        /// </summary>
        public RSSoapAccessor SoapAccessor
        {
            get;
            protected set;
        }

        /// <summary>
        /// Portal Accessor to interact with Reporting Services Server
        /// </summary>
        public RSPortalAccessorV1 PortalAccessorV1
        {
            get;
            protected set;
        }

        /// <summary>
        /// Portal Accessor to interact with Reporting Services Server
        /// </summary>
        public RSPortalAccessorV2 PortalAccessorV2
        {
            get;
            protected set;
        }

        private string _workingFolder = string.Empty;

        /// <summary>
        /// Working folder in report server
        /// </summary>
        public string WorkingFolder
        {
            get { return _workingFolder; }
            internal set { _workingFolder = value; }
        }

        private RandomContentManager _rndContentManager = new RandomContentManager();

        /// <summary>
        /// Get random content manager - API to manage temporaary RS objects
        /// </summary>
        public RandomContentManager RndContentManager
        {
            get
            {
                return (_rndContentManager);
            }
        }

        /// <summary>
        /// Folder where actual reports are read from to upload to report server
        /// </summary>
        public string ReportFolder { get; internal set; }

        private List<string> _existingReports = new List<string>();

        /// <summary>
        /// List of existing reports
        /// </summary>
        public List<string> ExistingReports
        {
            get { return _existingReports; }
            internal set { _existingReports = value; }
        }

        private Dictionary<string, string> _reportsWithSessions = new Dictionary<string, string>();

        /// <summary>
        /// Reports with already generated session ids.
        /// Note that, this only works in sticky sessions.
        /// </summary>
        public Dictionary<string, string> SessionIDs
        {
            get
            {
                return (_reportsWithSessions);
            }
        }

        private ConcurrentDictionary<string, string> _reportSnapshots = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Keeps list of SnapshotID (HistoryIDs) for all Existing Reports
        /// </summary>
        public ConcurrentDictionary<string, string> ReportSnapshots
        {
            get
            {
                return (_reportSnapshots);
            }
        }

        /// <summary>
        /// Base working folder name (no full/relative path)
        /// </summary>
        public string TargetFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Base report folder without (not full path)
        /// </summary>
        public string SourceFolder
        {
            get;
            set;
        }

        private string _toBeDeletedFolder = @"/ToBeDeleted";

        /// <summary>
        /// Temporary content holder.  All random SOAP API tests will use this folder
        /// to create, modify, and delete items.
        /// </summary>
        public virtual string ToBeDeletedFolder
        {
            get
            {
                return (_toBeDeletedFolder);
            }

            set
            {
                _toBeDeletedFolder = value;
            }
        }

        /// <summary>
        /// Item selector, option is random or sequential.  It is decided at runtime based on provided parameter.  
        /// Default is random.
        /// </summary>
        public ICollectionSelector ItemSelector { get; set; }

        private Random m_localRand = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Local Random Variable.  We can use this if we trully want just randome.
        /// </summary>
        public Random LocalRand
        {
            get
            {
                return (m_localRand);
            }
        }

        protected abstract string RootFolder
        {
            get;
        }

        #endregion Properties

        #region Public Methods

        #region Initialization

        /// <summary>
        /// Initializes necessary information provided source and target folder
        /// </summary>
        /// <param name="sourceFolder">Source Folder where report actually exists</param>
        /// <param name="targetFolder">Target Folder where we upload reports to</param>
        public void Initialize(string sourceFolder, string targetFolder)
        {
            SourceFolder = sourceFolder;
            TargetFolder = targetFolder;
            ReportFolder = Path.Combine(Directory.GetCurrentDirectory(), SourceFolder);
            InitializeReportServerAccesors();

            WorkingFolder = CreateWorkingFolder(RootFolder, TargetFolder);
        }

        /// <summary>
        /// Initializes necessary information provided source and target folder and deploys resources
        /// </summary>
        /// <param name="sourceFolder">Source Folder where report actually exists</param>
        /// <param name="targetFolder">Target Folder where we upload reports to</param>
        public virtual void InitializeWithResources(string sourceFolder, string targetFolder)
        {
            Initialize(sourceFolder, targetFolder);
        }

        /// <summary>
        /// Create folder under given parent folder
        /// NOTE: Should check for existence first, throw incase of error
        /// </summary>
        /// <param name="parent">Parent Folder</param>
        /// <param name="folderName">Folder To Create</param>
        /// <returns>Newly created folder path</returns>
        protected virtual string CreateWorkingFolder(string parent, string folderName)
        {
            string workingFolder = string.Empty;
            string itemPath = string.Empty;

            try
            {
                RSCatalogItem folder = SoapAccessor.Management.CreateFolder(folderName, parent, null);

                workingFolder = folder.Path;
            }
            catch (Exception ex)
            {
                // should check if folder existed.

                if (string.IsNullOrEmpty(workingFolder))
                {
                    if (parent.EndsWith("/"))
                        workingFolder = parent + folderName;
                    else
                        workingFolder = parent + "/" + folderName;
                }

                Logging.Log("Folder create failed: {0}", ex.Message);
            }

            return (workingFolder);
        }

        #endregion

        #region Data Sources
        /// <summary>
        /// Create known/default data sources for PSS
        /// </summary>
        public abstract void CreateKnownDataSources();

        /// <summary>
        /// Create data sources from given definition file using serverName.
        /// </summary>
        /// <param name="datasourceFile">File containing data source definition</param>
        /// <param name="targetFolder">RS Target Folder</param>
        /// <param name="fileExtension">File extension</param>
        public void CreateDataSources(string datasourceFile, string targetFolder, string fileExtension)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(datasourceFile);

            XmlNodeList datasourceNodes = doc.SelectNodes("//DataSource");
            string extension = string.Empty;
            string name = string.Empty;
            string connectString, server, database = string.Empty;
            bool enabled = true;
            string prompt = string.Empty;
            bool useWinCred = false;
            string userid = string.Empty;
            string password = string.Empty;
            bool impersonateUser = false;

            foreach (XmlNode node in datasourceNodes)
            {

                extension = node.Attributes["Extension"].Value;
                name = node.Attributes["Name"].Value + fileExtension;
                server = ReportServerInformation.DefaultInformation.DatasourceDatabaseServer;
                database = node.Attributes["Database"].Value;
                connectString = string.Format("Server={0};Database={1}", server, database);
                enabled = Convert.ToBoolean(node.Attributes["Enabled"].Value);
                prompt = node.Attributes["Prompt"].Value;
                useWinCred = Convert.ToBoolean(node.Attributes["UseWindowsCredential"].Value);
                userid = ReportServerInformation.DefaultInformation.DatasourceSQLUser;
                password = ReportServerInformation.DefaultInformation.DatasourceSQLPassword;
                useWinCred = Convert.ToBoolean(node.Attributes["ImpersonateUser"].Value);

                RSDataSourceDefinition dsDef = CreateDatasourceDefinition(extension, connectString, enabled, prompt, impersonateUser, useWinCred, userid, password);
                CreateDataSource(name, dsDef, targetFolder); 

            }
        }

        /// <summary>
        /// Create Data Source On RS Server (NOTE: probably return catalog item instead of path only)
        /// </summary>
        /// <param name="name">Data Source Name</param>
        /// <param name="dsDef">Data Source Definition</param>
        /// <param name="parent">Parent Folder to Crate Data Source</param>
        /// <returns>Path to newly created datasource</returns>
        public virtual string CreateDataSource(string name, RSDataSourceDefinition dsDef, string parent)
        {
            RSCatalogItem dsItem = SoapAccessor.Management.CreateDataSource(name, parent, true, dsDef, null);

            return dsItem.Path;
        }

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
        /// <param name="password">password for above</param>
        /// <returns>RS Data Source Definition</returns>
        public RSDataSourceDefinition CreateDatasourceDefinition(string extension, string connectString, bool enabled, string prompt, bool imperseronateUser, bool windowsCred, string user, string password)
        {
            RSDataSourceDefinition definition = new RSDataSourceDefinition();
            definition.CredentialRetrieval = RSCredentialRetrievalEnum.Store;
            definition.Extension = extension;
            definition.ConnectString = connectString;

            // connectString would look like below for SQL related extension for example
            // string.Format("Server={0};Database={1}", DSServer, Database);
            // if (TrustServerCertificate)
            //    definition.ConnectString = string.Format("{0};TrustServerCertificate={1}", definition.ConnectString, TrustServerCertificate);

            definition.Enabled = enabled;
            definition.EnabledSpecified = enabled;
            definition.ImpersonateUserSpecified = imperseronateUser;
            definition.Prompt = prompt;
            definition.WindowsCredentials = windowsCred;
            definition.UserName = user;
            definition.Password = password;

            return definition;
        }
        #endregion

        #region SharedDataSets

        /// <summary>
        /// Publish RSD files.
        /// </summary>
        public abstract void PublishSharedDataSets();

        /// <summary>
        /// Publish RSD files from source folder to RS.
        /// </summary>
        /// <param name="srcFolder">Source folder on disk.</param>
        /// <param name="destFolder">RS detination folder.</param>
        public abstract void PublishSharedDataSets(string srcFolder, string destFolder);

        /// <summary>
        /// Publish RSD file from source folder to RS.
        /// </summary>
        /// <param name="sharedDataSet">Source RSD file on disk.</param>
        /// <param name="displayName">Name of the SharedDataSets to publish.</param>
        public void PublishSharedDataSet(string sharedDataSet, string displayName)
        {
            PublishSharedDataSet(sharedDataSet, displayName, string.Empty);
        }

        /// <summary>
        /// Publish RSD file from source folder to RS.
        /// </summary>
        /// <param name="sharedDataSet">Source RSD file on disk.</param>
        /// <param name="displayName">Name of the SharedDataSets to publish.</param>
        /// <param name="parentFolder">RS detination folder.</param>
        public void PublishSharedDataSet(string sharedDataSet, string displayName, string parentFolder)
        {
            RSWarning[] warns = null;
            XmlDocument doc = new XmlDocument();
            doc.Load(sharedDataSet);
            byte[] rsdBytes = Encoding.UTF8.GetBytes(doc.OuterXml);

            RSCatalogItem catalogItem = SoapAccessor.Management.CreateCatalogItem("DataSet", displayName, parentFolder, true, rsdBytes, null, out warns);

        }

        /// <summary>
        /// Helper function to create a SharedDataSet quickly ignoring the SOAP exception
        /// Microsoft.ReportingServices.Diagnostics.Utilities.ItemAlreadyExistsException
        /// </summary>
        /// <param name="sharedDataSet">RDS File path to publish</param>
        /// <param name="displayName">Display Name of SharedDataSet</param>
        /// <param name="parentFolder">Parent Folder to create SharedDataSet in</param>
        public void TryPublishSharedDataSet(string sharedDataSet, string displayName, string parentFolder)
        {
            try
            {
                PublishSharedDataSet(sharedDataSet, displayName, parentFolder);
            }
            catch (SoapException e)
            {
                Logging.Log("Shared Dataset Creation failed: {0}", e.Message);
            }
        }

        #endregion SharedDataSets

        #region Report Publishing
        /// <summary>
        /// Upload reports to server in default folder
        /// </summary>
        public void PublishKnownReports()
        {
            PublishReports(ReportFolder, WorkingFolder);
        }

        /// <summary>
        /// Populate Existing Reports through ListChildren
        /// </summary>
        public virtual void PopulateReportListFromServer()
        {
            RSCatalogItem[] reports = SoapAccessor.Management.ListChildren(WorkingFolder, false);

            foreach (RSCatalogItem report in reports)
            {
                if (report.TypeName.Equals("Report"))
                {
                    ExistingReports.Add(report.Path);
                }
            }
        }

        /// <summary>
        /// For each report int the list, create session 
        /// Index is same as ExistingReports
        /// </summary>
        public void CreateSessionIDsFromKnownReports()
        {
            int retrycount = 3;
            bool retry = false;
            string url = string.Empty;
            string execid = string.Empty;

            foreach (string report in ExistingReports)
            {
                retry = false;
                retrycount = 0;

                while ((retrycount == 0) || (retry == true))
                {
                    try
                    {
                        url = ConstructUrl(report, new MHTMLRender(), null);
                        IssueGetRequest(url, out execid); // Render once so it is in memory.
                        SessionIDs.Add(report, execid);
                        retry = false;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log("Retrying CreateSessionIDsFromKnownReports: " + ex.Message);
                        retry = true;
                    }

                    retrycount++;
                    if (retrycount > 10)
                        break;
                }
            }
        }

        /// <summary>
        /// Creates snapshots for known Reports 
        /// </summary>
        public void CreateReportSnapshotsForKnownReports()
        {
            int retrycount = 3;
            bool retry = false;
            RSWarning[] warns = null;
            string historyId = string.Empty;

            foreach (string report in ExistingReports)
            {
                retry = false;
                retrycount = 0;

                // Either first time comming through or retry effort

                while ((retrycount == 0) || (retry == true))
                {
                    try
                    {
                        warns = null;
                        historyId = string.Empty;
                        historyId = SoapAccessor.Management.CreateItemHistorySnapshot(report, out warns);

                        // Add or update the history id to the report key
                        ReportSnapshots.AddOrUpdate(report, historyId, (key, oldValue) => historyId);
                        retry = false;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log("Retrying CreateReportSnapshotsForKnownReports : " + ex.Message);
                        retry = true;  // cause to loop back - should use retry with function
                    }

                    retrycount++;
                    if (retrycount > 10)
                        retry = false; // breaks the loop
                }
            }
        }

        /// <summary>
        /// Check whether report contains snapshot.  
        /// If report does not have, caller should remove from collection, too.
        /// </summary>
        /// <param name="report">Full path of report</param>
        /// <returns>Has snapshot or not</returns>
        public bool HasReportSnapshot(string report)
        {
            bool hasSnapshot = false;
            RSItemHistorySnapshot[] snapshots = null;

            snapshots = SoapAccessor.Management.ListItemHistory(report);
            if (snapshots != null)
                hasSnapshot = (snapshots.Length > 0);

            return (hasSnapshot);
        }

        /// <summary>
        /// Publish Reports From srcFolder (Disk) to destFolder (RS)
        /// </summary>
        /// <param name="srcFolder">Source Folder where RDLs are</param>
        /// <param name="destFolder">Destination Folder in RS backend</param>
        public abstract void PublishReports(string srcFolder, string destFolder);

        /// <summary>
        /// Publish given report to report server
        /// </summary>
        /// <param name="report">report to publish</param>
        /// <param name="displayName">report name on server</param>
        /// <returns>server report path</returns>
        public string PublishReport(string report, string displayName)
        {
            string folderName = PublishReport(report, displayName, WorkingFolder);
            return (folderName);
        }

        /// <summary>
        /// Helper function to create a report quickly.  
        /// </summary>
        /// <param name="report">RDL File path to publish</param>
        /// <param name="displayName">Display Name of Report</param>
        /// <param name="parentFolder">Parent Folder to create report in</param>
        /// <returns>Final path on server</returns>
        public abstract string PublishReport(string report, string displayName, string parentFolder);

        #endregion

        #region Exporting

        /// <summary>
        /// Construct Url to make URL access request.
        /// </summary>
        /// <param name="reportPath">Report Path</param>
        /// <param name="renderFormat">Render Format and settings</param>
        /// <param name="parameterValues">Parameter for report</param>
        /// <returns>Constructed Url</returns>
        public string ConstructUrl(string reportPath, RSRenderStruct renderFormat, NameValueCollection parameterValues)
        {
            return (URLAccessor.GetUrl(reportPath, renderFormat, parameterValues));
        }

        /// <summary>
        /// Construct Url based on parameer provided
        /// </summary>
        /// <param name="reportPath">Report Path</param>
        /// <param name="exportFormat">Export Format (PDF, CSV, ...)</param>
        /// <param name="parameterValues">Report Parameters</param>
        /// <returns>Constructed Url</returns>
        public string ConstructUrl(string reportPath, string exportFormat, NameValueCollection parameterValues)
        {
            return (ConstructUrl(reportPath, exportFormat, string.Empty, parameterValues));
        }

        /// <summary>
        /// Construct Url based on parameer provided which also included outputFormat (image type)
        /// </summary>
        /// <param name="reportPath">>Report Path</param>
        /// <param name="exportFormat">Export Format (PDF, CSV, ...)</param>
        /// <param name="outputFormat">Output Format (TIFF, PNG, ...)</param>
        /// <param name="parameterValues">Report Parameters</param>
        /// <returns>Constructed Url</returns>
        public string ConstructUrl(string reportPath, string exportFormat, string outputFormat, NameValueCollection parameterValues)
        {
            RSRenderStruct renderStruct = ConstructRenderStruct(exportFormat, outputFormat);

            RSRenderStruct a = renderStruct;

            return (ConstructUrl(reportPath, renderStruct, parameterValues));
        }

        /// <summary>
        /// Construct Render Struct from give parameters
        /// </summary>
        /// <param name="exportFormat">Render Format</param>
        /// <param name="outputFormat">Output format if needed</param>
        /// <returns>RS Render Struct</returns>
        public RSRenderStruct ConstructRenderStruct(string exportFormat, string outputFormat)
        {
            RSRenderStruct renderStruct = null;
            switch (exportFormat.ToUpperInvariant())
            {
                case "CSV":
                    renderStruct = new CSVRender();
                    break;
                case "XML":
                    renderStruct = new XMLRender();
                    break;
                case "PDF":
                    renderStruct = new PDFRender();
                    break;
                case "IMAGE":
                    renderStruct = new IMAGERender();
                    ((IMAGERender)renderStruct).OutputFormat = outputFormat;
                    break;
                case "WORD":
                    renderStruct = new WORDRender();
                    break;
                case "WORDOPENXML":
                    renderStruct = new WORDOPENXMLRender();
                    break;
                case "EXCEL":
                    renderStruct = new EXCELRender();
                    break;
                case "EXCELOPENXML":
                    renderStruct = new EXCELOPENXMLRender();
                    break;
                case "RGDI":
                    renderStruct = new RGDIRender();
                    break;
                case "EMF":
                    renderStruct = new EMFRender();
                    break;
                case "HTML4.0":
                    renderStruct = new HTML40Render();
                    break;
                case "HTML5":
                    renderStruct = new HTML50Render();
                    break;
                case "MHTML":
                    renderStruct = new MHTMLRender();
                    break;
                case "ATOM":
                    renderStruct = new ATOMRender();
                    break;
                case "PPTX":
                    renderStruct = new PPTXRender();
                    break;
                default:
                    break;
            }

            return renderStruct;
        }

        /// <summary>
        /// Construct Url based on parameer provided which also included outputFormat (image type)
        /// </summary>
        /// <param name="reportPath">>Report Path</param>
        /// <param name="renderFormat">Render Format and settings</param>
        /// <param name="snapshotID">If available snapshot id</param>
        /// <param name="startPage">starting page</param>
        /// <param name="endPage">ending page</param>
        /// <param name="sessiondId">session id</param>
        /// <param name="parameterValues">Paremeter values</param>
        /// <returns>Fully constructred URL based on parameters provided</returns>
        public string ConstructUrl(string reportPath, RSRenderStruct renderFormat, string snapshotID, int startPage, int endPage, string sessiondId, NameValueCollection parameterValues)
        {
            string url = URLAccessor.GetUrl(reportPath, renderFormat, parameterValues);

            if (!string.IsNullOrEmpty(snapshotID))
                url = "&rs:Snapshot=" + snapshotID;

            if (startPage != 0)
            {
                if (String.Compare(renderFormat.RenderFormat, "HTML4.0", true) == 0)
                    url += "&rc:Section=" + startPage;
                else

                    url += "&rc:StartPage=" + startPage;
            }

            if ((endPage != 0) && (string.Compare(renderFormat.RenderFormat, "HTML4.0", true) != 0))
                url += "&rs:EndPage=" + endPage;

            if (!string.IsNullOrEmpty(sessiondId))
                url += "&rs:SessionID=" + sessiondId;

            return (url);
        }

        /// <summary>
        /// Add more parameters to existing Url
        /// </summary>
        /// <param name="url">Consturced Url</param>
        /// <param name="snapshotid">Snapshot History Id if existed</param>
        /// <param name="startpage">Starting Page</param>
        /// <param name="endpage">Ending Page</param>
        /// <param name="paginationMode">Pagination Mode</param>
        /// <returns>Newly Constructed Url</returns>
        public virtual string AddMoreparamsToUrl(string url, string snapshotid, int startpage, int endpage, string paginationMode)
        {
            string newUrl = url;
            if (string.IsNullOrEmpty(url))
                throw new Exception("Url is null");
            if (!string.IsNullOrEmpty(snapshotid))
                newUrl += "&rs:Snapshot=" + snapshotid;
            if (!string.IsNullOrEmpty(paginationMode))
                newUrl += "&rs:PaginationMode=" + paginationMode;

            bool isHtml40 = url.Contains("HTML4.0");

            if (startpage > 0)
            {
                newUrl += string.Format("{0}={1}", (isHtml40 ? "&rc:Section" : "&rc:StartPage="), startpage);
            }

            if ((endpage > 0) && !isHtml40)
            {
                newUrl += "&rs:EndPage=" + endpage;
            }

            return newUrl;
        }

        #endregion

        /// <summary>
        /// Read Content from given item
        /// </summary>
        /// <param name="itemPath">Physical path to item to read</param>
        /// <returns>Byte array from read content</returns>
        public byte[] GetItemCotent(string itemPath)
        {
            byte[] content = null;
            content = File.ReadAllBytes(itemPath);
            return (content);
        }

        /// <summary>
        /// Get Report Page Count (in normal view mode)
        /// </summary>
        /// <param name="report">Report Path</param>
        /// <returns>Page Count of the given report</returns>
        public int GetPageCount(string report)
        {
            string ext;
            string mimeType;
            string encoding;
            RSWarning[] warns;
            string[] streamIDs;

            this.SoapAccessor.Execution.LoadReport(report, null);
            this.SoapAccessor.Execution.Render("HTML4.0", "<DeviceInfo><Section>1</Section></DeviceInfo>", out ext, out mimeType, out encoding, out warns, out streamIDs);
            RSExecutionInfo execInfo = this.SoapAccessor.Execution.GetExecutionInfo();

            return execInfo.NumPages;
        }

        /// <summary>
        /// Main export work is done here.  Few things still need to be parameterized
        /// customAuthCookies and other credentials, TimeOut, and any other stuff.
        /// </summary>
        /// <param name="url">Fully constructed URL for report server</param>
        /// <param name="executionID">ExecutionID of the request</param>
        public void IssueGetRequest(string url, out string executionID)
        {
            Int32 bytesReceived = 0;
            executionID = string.Empty;
            try
            {
                HttpWebRequest request = CreateHttpWebRequest(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception("Status code of " + response.StatusCode + " (" + response.StatusDescription + ") received for url '" + url + "'");

                    foreach (Cookie c in request.CookieContainer.GetCookies(new Uri(SoapAccessor.Management.Url)))
                    {
                        if (c.Name.StartsWith("RSExecutionSession", StringComparison.InvariantCultureIgnoreCase))
                            executionID = c.Value;
                    }

                    using (Stream stream = response.GetResponseStream())
                    {
                        Byte[] buffer = new Byte[1024 * 1024];
                        Int32 bytesRead = 0;
                        bytesReceived = bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bytesReceived += bytesRead;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Log("Error retrieving url:\n\t{0},\n\texpection: {1}", url, e.Message);
                bytesReceived = 0;
                throw (e);
            }
        }

        /// <summary>
        /// Given a url, create HttpWebRequest object
        /// Centralized the creation so all have same parameters.
        /// </summary>
        /// <param name="url">Url for creating object</param>
        /// <returns>Created HttpWebRequest object</returns>
        public HttpWebRequest CreateHttpWebRequest(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            CookieContainer cookies = new CookieContainer();
            request.Proxy = null;
            request.PreAuthenticate = true;
            request.Timeout = -1; // should be externally controlled.
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Accept = "Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-shockwave-flash, */*";
            request.Headers.Add("Accept-Language", "en-us");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.KeepAlive = true;
            request.CookieContainer = cookies;
            request.Credentials = _executionCredentials;

            return (request);
        }

        /// <summary>
        /// Get SessionID for given report, if not existence or expired, create one.
        /// </summary>
        /// <param name="report">exiting report path</param>
        /// <returns>SessionID aka ExecutionID</returns>
        public string GetSessionID(string report)
        {
            // string sessionid = ContentManagerFactory.Instance.SessionIDs[report]; // do management as they expire

            string sessionid = sessionIdManager.GetSessionID(report);
            return (sessionid);
        }

        /// <summary>
        /// Get SnaphshotID (aka HistoryID) for a given report.  If it does not exist, create one, then add to list
        /// NOTE: sometime this may have expiration time so will need management of that.
        /// </summary>
        /// <param name="report">Existing Report Path</param>
        /// <returns>Snapshot ID (History ID)</returns>
        public string GetSnapshotIDFromCache(string report)
        {
            string historyIdForReport = null;

            historyIdForReport =
                ReportSnapshots.GetOrAdd(
                    report,
                    (historyID) =>
                    {
                        if (HasReportSnapshot(report))
                            return GetReportSnapshotId(report);
                        else
                            return CreateReportSnapshot(report);
                    });

            return historyIdForReport;
        }

        /// <summary>
        /// Create new report snapshot 
        /// </summary>
        /// <param name="report">Full report path</param>
        /// <returns>SnapshotId aka HistoryId</returns>
        public string CreateReportSnapshot(string report)
        {
            int retrycount = 0;
            bool retry = false;
            RSWarning[] warns = null;
            string historyId = string.Empty;

            while ((retrycount == 0) || (retry == true))
            {
                try
                {
                    warns = null;
                    historyId = string.Empty;
                    historyId = SoapAccessor.Management.CreateItemHistorySnapshot(report, out warns);
                    retry = false;
                }
                catch (Exception ex)
                {
                    Logging.Log("Retrying CreateReportSnapshot: " + ex.Message);
                    retry = true;  // cause to loop back - should use retry with function
                }

                retrycount++;
                if (retrycount > 10)
                    retry = false; // breaks the loop
            }

            return (historyId);
        }

        /// <summary>
        /// GetReportSnapshot from Report (not from cache)
        /// </summary>
        /// <param name="report">Report Path</param>
        /// <returns>Snapshot Id</returns>
        public string GetReportSnapshotId(string report)
        {
            string historyId = null;
            RSItemHistorySnapshot[] history = SoapAccessor.Management.ListItemHistory(report);
            if (history != null)
            {
                if (history.Length > 0)
                    historyId = history[0].HistoryID;
            }

            return (historyId);
        }

        /// <summary>
        /// Get Report Name (short) from give full path of report
        /// </summary>
        /// <param name="reportPath">Full Path Of report</param>
        /// <returns>Report Name without path</returns>
        public string GetReportName(string reportPath)
        {
            string name = string.Empty;
            if (reportPath.StartsWith(this.WorkingFolder, StringComparison.InvariantCultureIgnoreCase))
                name = reportPath.Substring(this.WorkingFolder.Length + 1);
            else
                name = reportPath; // should not happen anyway.

            return (name);
        }

        /// <summary>
        /// Generate Random name (should be valid item name)
        /// </summary>
        /// <returns>Item name with proper extension</returns>
        public virtual string GenerateRndFileName()
        {
            string newname = Path.GetRandomFileName();
            return (newname.Substring(0, newname.Length - 4));
        }
        #endregion 

        #region SOAP APIS
        /// <summary>
        /// Craete a generic report given the name and parent folder.
        /// Name alerady supposed to have extesion
        /// </summary>
        /// <param name="name">Item name to be used on the server</param>
        /// <param name="parent">Parent folder</param>
        /// <returns>Full Path of newly created item</returns>
        public string CreateGenericReport(string name, string parent)
        {
            RSWarning[] warns = null;
            string sourceReport = SharedConstants.RuntimeResourcesFolder + @"\Paginated\NoDatasource\ImageOnly.rdl";  // better if we could control this from outside
            byte[] content = this.GetItemCotent(sourceReport); // use cheese report
            string reportPath = string.Empty;
            RSCatalogItem item = null;

            item = this.SoapAccessor.Management.CreateCatalogItem("Report", name, parent, true, content, null, out warns);
            if (item != null) reportPath = item.Path;

            return (reportPath);
        }

        #endregion

        #region Browsing

        void IContentManager.BrowseCatalogItems(string path, string type)
        {
            throw new NotImplementedException();
        }
        void IContentManager.GetCatalogItem(string path, string expand)
        {
            throw new NotImplementedException();
        }

        void IContentManager.GetDependentItems(string path)
        {
            throw new NotImplementedException();
        }

        #endregion

        private ICredentials _executionCredentials;
        #region PrivateMethods
        /// <summary>
        /// Initialize the SOAP and the URL Report Server Accessors
        /// </summary>
        private void InitializeReportServerAccesors()
        {

            Logging.Log("Initialize Report Server Accessors for Native Environment");
            _executionCredentials = CredentialCache.DefaultNetworkCredentials;
            if (!String.IsNullOrEmpty(ReportServerInformation.DefaultInformation.ExecutionAccount))
            {
                CredentialCache myCache = new CredentialCache();
                Uri reportServerUri = new Uri(ReportServerInformation.DefaultInformation.ReportServerUrl);
                myCache.Add(new Uri(reportServerUri.GetLeftPart(UriPartial.Authority)), "NTLM", new NetworkCredential(ReportServerInformation.DefaultInformation.ExecutionAccount, ReportServerInformation.DefaultInformation.ExecutionAccountPwd));
                _executionCredentials = myCache;
            }

            RSSoapAccessor soapAcessor = new RSSoapAccessor(ReportServerInformation.DefaultInformation.ReportServerUrl, _executionCredentials);
            Logging.Log("InitializeReportServerAccesors SoapAcessor is null={0}", soapAcessor == null);
            this.SoapAccessor = soapAcessor;

            RSUrlAccessor urlAcessor = new RSUrlAccessor(ReportServerInformation.DefaultInformation.ReportServerUrl);
            Logging.Log("InitializeReportServerAccesors UrlAcessor is null={0}", urlAcessor == null);
            urlAcessor.ExecuteCredentials = _executionCredentials;
            this.URLAccessor = urlAcessor;

            RSPortalAccessorV1 portalAcessor = new RSPortalAccessorV1(ReportServerInformation.DefaultInformation.RestApiV1Url);
            Logging.Log("InitializeReportServerAccesors PortalAccessor is null={0}", portalAcessor == null);
            portalAcessor.ExecuteCredentials = _executionCredentials;
            this.PortalAccessorV1 = portalAcessor;

            RSPortalAccessorV2 portalAcessorV2 = new RSPortalAccessorV2(ReportServerInformation.DefaultInformation.RestApiV2Url);
            Logging.Log("InitializeReportServerAccesors PortalAccessor is null={0}", portalAcessor == null);
            portalAcessorV2.ExecuteCredentials = _executionCredentials;
            this.PortalAccessorV2 = portalAcessorV2;
        }

        /// <summary>
        /// Given extesnion, returns empty or actual extension if applicable.
        /// </summary>
        /// <param name="ext">extesnion name</param>
        /// <returns>String.Empty or actual extension</returns>
        public abstract string GetExtension(string ext);

        /// <summary>
        /// Get next report item (random/sequetial)
        /// It gets from existing reports collection.
        /// </summary>
        /// <returns>Report Path That Exists</returns>
        public string GetNextReport()
        {
            string report;
            if (ItemSelector == null)
                throw new Exception("The item selector need to be intialized for ContenManager before the GetNextReport() method is called");

            if (isReportsWeightSet)
                report = ItemSelector.GetItem(this.WeightedReports);
            else
                report = ItemSelector.GetItem(this.ExistingReports);

            if (_badMethodReportCombinations != null && IsBlockedCombination(report))
                report = GetNextReport();

            return report;
        }

        public string GetNextCatalogItem(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "report":
                    return GetNextReport();

                case "mobilereport":
                    return ItemSelector.GetItem(this.ExistingMobileReports);

                case "kpi":
                    return ItemSelector.GetItem(this.ExistingKpis);

                case "powerbireport":
                    return ItemSelector.GetItem(this.ExistingPowerBIReports);

                case "powerbireportembedded":
                    return ItemSelector.GetItem(this.ExistingEmbeddedPowerBIReports);

                default:
                    throw new Exception("Type {0} is not supported.");
            }
        }

        /// <summary>
        /// Verifies if the report is in the list of bad combinations and if the test method
        /// using the report is blocked
        /// </summary>
        /// <param name="reportFullName">name of the report</param>
        /// <returns>True if the combination is not usable, false if the combination is valid for use</returns>
        private bool IsBlockedCombination(string reportFullName)
        {
            bool isBlockedCombination = false;
            List<string> testMethods = GetTestMethodsName();
            if (testMethods.Count() > 0)
            {
                foreach (string testMethod in testMethods)
                {
                    List<string> badReports;
                    if (_badMethodReportCombinations.TryGetValue(testMethod, out badReports))
                    {
                        var badReportsMatchingCurrentReport = from badReport in badReports
                                                              where (reportFullName.Contains(badReport))
                                                              select badReport;

                        if (badReportsMatchingCurrentReport.Count() > 0)
                            isBlockedCombination = true;
                    }
                }
            }

            return isBlockedCombination;
        }

        /// <summary>
        /// Return the list of test methods from load test that are in the stack calling this function
        /// </summary>
        /// <returns>list of test methods from load test in the stack</returns>
        private List<string> GetTestMethodsName()
        {
            StackTrace stackTrace = new StackTrace();
            IEnumerable<string> methodsWitTestAtt = from frame in stackTrace.GetFrames()
                                                    from methodAtt in frame.GetMethod().GetCustomAttributes(typeof(TestMethodAttribute), false)
                                                    select frame.GetMethod().Name;
            return methodsWitTestAtt.ToList();
        }



        private List<string> _weightedReports;

        private List<string> WeightedReports
        {
            get
            {
                if (_weightedReports == null)
                {
                    _weightedReports = new List<string>();
                    foreach (string reportFullName in ExistingReports)
                    {
                        var weightsForReport = from reportWeight in _reportsWeight
                                               where (reportFullName.Contains(reportWeight.Report))
                                               select reportWeight.Weight;

                        int weight = 1;
                        if (weightsForReport.Count() > 0)
                            weight = weightsForReport.First();

                        // add a copy of the report full name per every point in weight
                        for (int i = 0; i < weight; i++)
                            _weightedReports.Add(reportFullName);
                    }
                }

                return _weightedReports;
            }
        }

        private List<ReportWeight> _reportsWeight = null;
        private bool isReportsWeightSet = false;

        /// <summary>
        /// Set a distribution for the reports based on weight
        /// if this value is not used every report have the same chance to be returned in GetNextReport()
        /// the string is the name of the report and the int is the weight that will be assigned during the 
        /// selection of the report in GetNextReport()
        /// </summary>
        public List<ReportWeight> ReporstWeight
        {
            set
            {
                _weightedReports = null;
                _reportsWeight = value;
                if (value != null)
                    isReportsWeightSet = true;
            }
        }

        /// <summary>
        /// The private dictionary is to make the lookup per key efficient
        /// The key is the methodName and the value is the list of invalid reports for that method
        /// </summary>
        private Dictionary<string, List<string>> _badMethodReportCombinations = null;

        /// <summary>
        /// Define the set of reports and a list of methods that should not be executed
        /// </summary>
        public List<BadCombination> BadReportMethodCombinations
        {
            set
            {
                if (value != null)
                {
                    _badMethodReportCombinations = new Dictionary<string, List<string>>();
                    foreach (BadCombination combination in value)
                        _badMethodReportCombinations.Add(combination.Method, combination.InvalidReports);
                }
            }
        }

        public List<string> ExistingMobileReports { get; internal set; }

        public List<string> ExistingKpis { get; internal set; }

        public List<string> ExistingPowerBIReports { get; internal set; }

        public List<string> ExistingEmbeddedPowerBIReports { get; internal set; }
        #endregion
    }
}
