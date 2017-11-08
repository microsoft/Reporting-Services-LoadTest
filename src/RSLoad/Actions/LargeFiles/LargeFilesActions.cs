// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.PortalAccessor;
using RSTest.Common.ReportServer.Information;

namespace RSLoad
{
    [TestClass]
    public class LargeFilesActions : PSSActionBase
    {
        private static readonly Dictionary<Uri, string> PowerBITestResources;
        private static readonly Dictionary<Uri, string> ExcelTestResources;

        static LargeFilesActions()
        {
            PowerBITestResources = new Dictionary<Uri, string>()
            {
                { new Uri(SharedConstants.RsLoadBlobUrl + "largefiles/1.pbix"), "1.pbix" },
                { new Uri(SharedConstants.RsLoadBlobUrl + "largefiles/large-5mil.pbix"), "large-5mil.pbix" },
                { new Uri(SharedConstants.RsLoadBlobUrl + "largefiles/large-15mil.pbix"), "large-15mil.pbix" },
                { new Uri(SharedConstants.RsLoadBlobUrl + "largefiles/large-25mil.pbix"), "large-25mil.pbix" }
            };

            ExcelTestResources = new Dictionary<Uri, string>()
            {
                { new Uri(SharedConstants.RsLoadBlobUrl + "largefiles/1.xlsx"), "1.xlsx" },
                { new Uri(SharedConstants.RsLoadBlobUrl + "largefiles/output-5mil.csv"), "output-5mil.csv" },
                { new Uri(SharedConstants.RsLoadBlobUrl + "largefiles/output-10mil.csv"), "output-10mil.csv" },
                { new Uri(SharedConstants.RsLoadBlobUrl + "largefiles/output-15mil.csv"), "output-15mil.csv" }
            };
        }

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            var downloadTasks = new List<Task>();

            var testResources = PowerBITestResources.Union(ExcelTestResources);
            foreach (var resource in testResources)
            {
                var webClient = new WebClient();
                downloadTasks.Add(webClient.DownloadFileTaskAsync(resource.Key, resource.Value));
            }

            Task.WaitAll(downloadTasks.ToArray());
        }

        [TestCategory("LongDuration")]
        [TestMethod]
        public void UploadAndDeleteLargePbix()
        {
            var newLargePbixFileName = Guid.NewGuid().ToString();
            var pbixPath = GetRandomMember(PowerBITestResources).Value;
            string newLargePbixFile = Path.Combine(Path.GetDirectoryName(pbixPath), newLargePbixFileName + Path.GetExtension(pbixPath));
            File.Copy(pbixPath, newLargePbixFile);

            var targetPath = "/" + newLargePbixFileName;

            try
            {
                PortalAccessor pa =
                    new PortalAccessor(ReportServerInformation.DefaultInformation.ReportPortalUrl)
                    {
                        ExecuteCredentials = GetExecutionCredentails()
                    };

                HttpWebResponse response = pa.UploadLargeFile(newLargePbixFile, "PowerBIReport", targetPath);

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                response = pa.DeleteFile(targetPath);

                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            }
            finally
            {
                File.Delete(newLargePbixFile);
            }
        }

        [TestCategory("LongDuration")]
        [TestMethod]
        public void UploadAndDeleteLargeExcelWorkbook()
        {
            var newLargeExcelWorkbookFileName = Guid.NewGuid().ToString();
            var excelWorkbookPath = GetRandomMember(ExcelTestResources).Value;
            string newLargeExcelWorkbookFile = Path.Combine(Path.GetDirectoryName(excelWorkbookPath), newLargeExcelWorkbookFileName + Path.GetExtension(excelWorkbookPath));
            File.Copy(excelWorkbookPath, newLargeExcelWorkbookFile);

            var targetPath = "/" + newLargeExcelWorkbookFileName + Path.GetExtension(excelWorkbookPath);

            try
            {
                PortalAccessor pa =
                    new PortalAccessor(ReportServerInformation.DefaultInformation.ReportPortalUrl)
                    {
                        ExecuteCredentials = GetExecutionCredentails()
                    };

                HttpWebResponse response = pa.UploadLargeFile(newLargeExcelWorkbookFile, "ExcelWorkbook", targetPath);

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                response = pa.DeleteFile(targetPath);

                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            }
            finally
            {
                File.Delete(newLargeExcelWorkbookFile);
            }
        }

        private ICredentials GetExecutionCredentails()
        {
            ICredentials executionCredentails = CredentialCache.DefaultNetworkCredentials;
            if (!string.IsNullOrEmpty(ReportServerInformation.DefaultInformation.ExecutionAccount))
            {
                var myCache = new CredentialCache();
                var reportServerUri = new Uri(ReportServerInformation.DefaultInformation.ReportServerUrl);
                myCache.Add(new Uri(reportServerUri.GetLeftPart(UriPartial.Authority)),
                    "NTLM",
                    new NetworkCredential(
                        ReportServerInformation.DefaultInformation.ExecutionAccount,
                        ReportServerInformation.DefaultInformation.ExecutionAccountPwd));
                executionCredentails = myCache;
            }
            return executionCredentails;
        }

        private KeyValuePair<Uri, string> GetRandomMember(Dictionary<Uri, string> dictionary)
        {
            var rand = new Random();
            return dictionary.ElementAt(rand.Next(0, dictionary.Count - 1));
        }
    }
}