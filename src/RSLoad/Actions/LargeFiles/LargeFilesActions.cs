// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.PortalAccessor;
using RSTest.Common.ReportServer.Information;

namespace RSLoad
{
    [TestClass]
    public class LargeFilesActions : PSSActionBase
    {
        private static readonly Uri LargePbixUrl = new Uri("https://rsload.blob.core.windows.net/load/largefiles/1.pbix");
        private static readonly Uri LargeExcelWorkbookUrl = new Uri("https://rsload.blob.core.windows.net/load/largefiles/1.xlsx");
        private const string LargePbixPath = "1.pbix";
        private const string LargeExcelWorkbookPath = "1.xlsx";

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(LargePbixUrl, LargePbixPath);
                webClient.DownloadFile(LargeExcelWorkbookUrl, LargeExcelWorkbookPath);
            }
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (File.Exists(LargePbixPath))
            {
                File.Delete(LargePbixPath);
            }

            if (File.Exists(LargeExcelWorkbookPath))
            {
                File.Delete(LargeExcelWorkbookPath);
            }
        }

        [TestMethod]
        public void UploadAndDeleteLargePbix()
        {
            var newLargePbixFileName = Guid.NewGuid().ToString();
            string newLargePbixFile = Path.Combine(Path.GetDirectoryName(LargePbixPath), newLargePbixFileName + Path.GetExtension(LargePbixPath));
            File.Copy(LargePbixPath, newLargePbixFile);

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

        [TestMethod]
        public void UploadAndDeleteLargeExcelWorkbook()
        {
            var newLargeExcelWorkbookFileName = Guid.NewGuid().ToString();
            string newLargeExcelWorkbookFile = Path.Combine(Path.GetDirectoryName(LargeExcelWorkbookPath), newLargeExcelWorkbookFileName + Path.GetExtension(LargeExcelWorkbookPath));
            File.Copy(LargeExcelWorkbookPath, newLargeExcelWorkbookFile);

            var targetPath = "/" + newLargeExcelWorkbookFileName + Path.GetExtension(LargeExcelWorkbookPath);

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
    }
}