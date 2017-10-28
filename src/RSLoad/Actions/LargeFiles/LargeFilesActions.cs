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

        [TestMethod]
        public void UploadAndDeleteLargePbix()
        {
            string largePbixFile = SharedConstants.LargePbixPath;
            var newLargePbixFileName = Guid.NewGuid().ToString();
            string newLargePbixFile = Path.Combine(Path.GetDirectoryName(largePbixFile), newLargePbixFileName + Path.GetExtension(largePbixFile));
            File.Copy(largePbixFile, newLargePbixFile);

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
            string largeExcelWorkbookFile = SharedConstants.LargeExcelWorkbookPath;
            var newLargeExcelWorkbookFileName = Guid.NewGuid().ToString();
            string newLargeExcelWorkbookFile = Path.Combine(Path.GetDirectoryName(largeExcelWorkbookFile), newLargeExcelWorkbookFileName + Path.GetExtension(largeExcelWorkbookFile));
            File.Copy(largeExcelWorkbookFile, newLargeExcelWorkbookFile);

            var targetPath = "/" + newLargeExcelWorkbookFileName + Path.GetExtension(largeExcelWorkbookFile);

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