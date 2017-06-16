// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.PortalAccessor;
using RSLoad.Utilities;
using RSAccessor.PortalAccessor.OData.Model;
using ODataV2Model = RSAccessor.PortalAccessor.OData.V2.Model;
using RSAccessor.SoapAccessor;

namespace RSLoad
{
    internal class PortalContentManager : ContentManagerBase, IContentManager
    {
        private const string PortalTestName = "PortalTest";
        private const int TestFolderNumber = 100;
        private const string TestFolderName = "TestFolder_{0}";
        private const string RootPath = "/";
        private const string OrderByCaluse = "$orderby";

        private Dictionary<string, List<string>> _badMethodReportCombinations = null;

        protected override string RootFolder
        {
            get
            {
                return RootPath;
            }
        }

        public PortalContentManager()
        {
            ExistingReports = new List<string>();
            ExistingMobileReports = new List<string>();
            ExistingKpis = new List<string>();
            ExistingPowerBIReports = new List<string>();
        }

        public override string CreateDataSource(string name, RSDataSourceDefinition dsDef, string parent)
        {
            var dataSource = new DataSource
            {
                ConnectionString = dsDef.ConnectString,
                IsConnectionStringOverridden = true,
                CredentialRetrieval = dsDef.CredentialRetrieval == RSCredentialRetrievalEnum.Integrated ? CredentialRetrievalType.Integrated :
                                      dsDef.CredentialRetrieval == RSCredentialRetrievalEnum.Prompt ? CredentialRetrievalType.Prompt :
                                      dsDef.CredentialRetrieval == RSCredentialRetrievalEnum.Store ? CredentialRetrievalType.Store :
                                      CredentialRetrievalType.None,
                CredentialsByUser = dsDef.CredentialRetrieval == RSCredentialRetrievalEnum.Prompt ? new CredentialsSuppliedByUser
                {
                    UseAsWindowsCredentials = dsDef.WindowsCredentials,
                    DisplayText = dsDef.Prompt
                } : null,
                CredentialsInServer = dsDef.CredentialRetrieval == RSCredentialRetrievalEnum.Store ? new CredentialsStoredInServer
                {
                    ImpersonateAuthenticatedUser = dsDef.ImpersonateUserSpecified,
                    UserName = dsDef.UserName,
                    Password = dsDef.Password,
                    UseAsWindowsCredentials = dsDef.WindowsCredentials
                } : null,
                DataSourceType = dsDef.Extension,
                IsOriginalConnectionStringExpressionBased = dsDef.OriginalConnectStringExpressionBased,
                IsEnabled = dsDef.Enabled,
                Path = parent,
                Name = name
            };

            try
            {
                PortalAccessorV1.AddToCatalogItems(dataSource);
            }
            catch (Exception ex)
            {
                Logging.Log("Folder create failed: {0}", ex.Message);
                throw;
            }

            return RSPortalAccessorV1.CreateFullPath(parent, name);
        }

        public override void CreateKnownDataSources()
        {
            string dsDefinitionPath = Path.Combine(Directory.GetCurrentDirectory(),SharedConstants.RuntimeResourcesFolder, "DataSources.xml");
            CreateDataSources(dsDefinitionPath, this.WorkingFolder, string.Empty);
        }

        public override string GetExtension(string ext)
        {
            return (string.Empty);
        }

        public override void InitializeWithResources(string sourceFolder, string targetFolder)
        {
            base.InitializeWithResources(sourceFolder, targetFolder);
            CreateRSFolder(RootPath, TargetFolder);
            CreateRSFolder(RootPath, ToBeDeletedFolder.TrimStart('/'));
            if (PortalTestName.Equals(sourceFolder, StringComparison.OrdinalIgnoreCase))
            {
                for (int index = TestFolderNumber; index-- > 0;)
                    CreateRSFolder(WorkingFolder, string.Format(TestFolderName, index));
            }
        }

        protected override string CreateWorkingFolder(string parent, string folderName)
        {
            return CreateRSFolder(parent, folderName);
        }

        public override void PopulateReportListFromServer()
        {
            GetExistingMobileReports();
            GetExistingKpis();
            GetExistingPowerBIReports();
            base.PopulateReportListFromServer();
        }

        public override string PublishReport(string report, string displayName, string parentFolder)
        {
            try
            {
                var content = GetItemCotent(report);
                switch (Path.GetExtension(report))
                {
                    case ".rdl":
                    case ".rsmobile":
                    case ".kpi":
                    case ".pbix":
                        return PublishItemToPortal(report, displayName, parentFolder, content);
                }
            }
            catch(Exception)
            {
                Logging.Log("Failed publish report {0} in parent folder {1}", report, parentFolder);
                throw;
            }

            return null;
        }

        public override void PublishReports(string srcFolder, string destFolder)
        {
            var di = new DirectoryInfo(srcFolder);
            var files = di.GetFiles("*.rdl")
                .Concat(di.GetFiles("*.rsmobile"))
                .Concat(di.GetFiles("*.kpi"))
                .Concat(di.GetFiles("*.pbix"));

            foreach (FileInfo fi in files)
            {
                var displayName = fi.Name.Substring(0, fi.Name.IndexOf(fi.Extension));
                var reportPath = PublishReport(fi.FullName, displayName, destFolder);
                switch (fi.Extension)
                {
                    case ".rdl":
                        ExistingReports.Add(reportPath);
                        break;

                    case ".rsmobile":
                        ExistingMobileReports.Add(reportPath);
                        break;

                    case ".kpi":
                        ExistingKpis.Add(reportPath);
                        break;

                    case ".pbix":
                        ExistingPowerBIReports.Add(reportPath);
                        break;
                }
            };
        }

        public override void PublishSharedDataSets()
        {
            PublishSharedDataSets(ReportFolder, WorkingFolder);
        }

        private IOrderedQueryable<CatalogItem> GetFolderContent(string folderName)
        {
            var ctx = PortalAccessorV1.CreateContext();
            var folder = ctx.CatalogItemByPath(folderName).GetValue();
            return ctx.CatalogItems
                .ByKey(folder.Id)
                .CastToFolder()
                .CatalogItems
                .OrderBy(x => x.Name);
        }

        public override void PublishSharedDataSets(string srcFolder, string destFolder)
        {
            var di = new DirectoryInfo(srcFolder);
            var files = di.GetFiles("*.rsd");

            foreach (FileInfo fi in files)
            {
                var displayName = fi.Name.Substring(0, fi.Name.IndexOf(fi.Extension));
                TryPublishSharedDataSet(fi.FullName, displayName, destFolder);
            }
        }

        private static List<string> GetTestMethodsName()
        {
            var stackTrace = new StackTrace();
            var methodsWitTestAtt = from frame in stackTrace.GetFrames()
                                    from methodAtt in frame.GetMethod().GetCustomAttributes(typeof(TestMethodAttribute), false)
                                    select frame.GetMethod().Name;
            return methodsWitTestAtt.ToList();
        }

        private string CreateRSFolder(string parent, string folderName)
        {
            try
            {
                var folder = RSPortalAccessorV1.CreateFullPath(parent, folderName);
                var path = parent ?? RootPath;
                PortalAccessorV1.AddToCatalogItems(new Folder { Name = folderName, Path = path });
                return folder;
            }
            catch (Exception e)
            {
                Logging.Log("Folder create failed: {0}", e.Message);
                throw;
            }
        }

        private void GetExistingKpis()
        {
            var kpis = GetFolderContent(this.WorkingFolder).Where(x => x.Type == CatalogItemType.Kpi);
            foreach (var kpi in kpis)
            {
                ExistingKpis.Add(kpi.Path);
            }
        }

        private void GetExistingPowerBIReports()
        {
            var reports = GetFolderContent(this.WorkingFolder).Where(x => x.Type == CatalogItemType.PowerBIReport);
            foreach (var report in reports)
            {
                ExistingPowerBIReports.Add(report.Path);
            }
        }

        private void GetExistingMobileReports()
        {
            var reports = GetFolderContent(this.WorkingFolder).Where(x => x.Type == CatalogItemType.MobileReport);
            foreach (var report in reports)
            {
                ExistingMobileReports.Add(report.Path);
            }
        }

        private bool IsBlockedCombination(string reportFullName)
        {
            var isBlockedCombination = false;
            var testMethods = GetTestMethodsName();
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

        private string PublishItemToPortal(string report, string displayName, string parentFolder, byte[] content)
        {
            switch (Path.GetExtension(report))
            {
                case ".rdl":
                    string path = RSPortalAccessorV1.CreateFullPath(parentFolder, displayName);
                    var item = new Report
                    {
                        Name = displayName,
                        Path = path,
                        Content = content
                    };
                    PortalAccessorV1.AddToCatalogItems(item);
                    return path;

                case ".rsmobile":
                    return PortalAccessorV1.AddToCatalogItems<MobileReport>(displayName, parentFolder, content);

                case ".kpi":
                    string json = Encoding.UTF8.GetString(content);
                    return PortalAccessorV1.AddToCatalogItems<Kpi>(displayName, parentFolder, json);

                case ".pbix":
                    return PortalAccessorV2.AddToCatalogItems<ODataV2Model.PowerBIReport>(displayName, parentFolder, content);

                default:
                    return null;
            }
        }

        public void BrowseCatalogItems(string path, string type)
        {
            GetFolderContent(path, (CatalogItemType)Enum.Parse(typeof(CatalogItemType), type));
        }

        public void GetCatalogItem(string path, string expand)
        {
            var ctx = PortalAccessorV1.CreateContext();
            var item = ctx.CatalogItemByPath(path);
            if (expand == null)
                item.GetValue();
            else
                item.Expand(expand).GetValue();
        }

        public void GetDependentItems(string path)
        {
            var ctx = PortalAccessorV1.CreateContext();
            var item = ctx.CatalogItemByPath(path).GetValue();
            ctx.CatalogItems
                .ByKey(item.Id)
                .GetDependentItems()
                .Execute();
        }

        private IEnumerable<CatalogItem> GetFolderContent(string path, CatalogItemType type)
        {
            var ctx = PortalAccessorV1.CreateContext();
            var folder = ctx.CatalogItemByPath(path).CastToFolder().GetValue();

            switch (type)
            {
                case CatalogItemType.Folder:
                    return GetCatalogItems<Folder>(ctx, folder.Id).Cast<CatalogItem>();

                case CatalogItemType.Report:
                    return GetCatalogItems<Report>(ctx, folder.Id).Cast<CatalogItem>();

                case CatalogItemType.LinkedReport:
                    return GetCatalogItems<LinkedReport>(ctx, folder.Id).Cast<CatalogItem>();

                case CatalogItemType.Kpi:
                    return GetCatalogItems<Kpi>(ctx, folder.Id).Cast<CatalogItem>();

                case CatalogItemType.MobileReport:
                    return GetCatalogItems<MobileReport>(ctx, folder.Id).Cast<CatalogItem>();

                case CatalogItemType.DataSet:
                    return GetCatalogItems<DataSet>(ctx, folder.Id).Cast<CatalogItem>();

                case CatalogItemType.DataSource:
                    return GetCatalogItems<DataSource>(ctx, folder.Id).Cast<CatalogItem>();

                case CatalogItemType.Resource:
                    return GetCatalogItems<Resource>(ctx, folder.Id).Cast<CatalogItem>();

                default:
                    throw new NotSupportedException();
            }
        }

        private IEnumerable<T> GetCatalogItems<T>(Container container, Guid folderId) where T : CatalogItem
        {
            return container.CatalogItems.ByKey(folderId)
                .CastToFolder()
                .CatalogItems
                .AddQueryOption(OrderByCaluse, "name ASC")
                .OfType<T>();
        }
    }
}