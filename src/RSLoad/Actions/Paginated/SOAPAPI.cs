// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSAccessor.SoapAccessor;


namespace RSLoad
{
    /// <summary>
    /// All SOAP API tests
    /// Mostly used in stress.
    /// Maybe used in perf.
    /// </summary>
    public partial class PaginatedActions : PSSActionBase
    {
        #region Properties
        private Dictionary<string, RndReportInfo> m_rndReportInfo = new Dictionary<string, RndReportInfo>();

        private Dictionary<string, RndReportInfo> RandomReportInfo 
        {
            get { return (m_rndReportInfo); }
        }

        private List<string> m_rndReports = new List<string>();

        private List<string> RandomReports 
        {
            get { return (m_rndReports); }
        }

        #endregion

        #region SOAP Actions
        /// <summary>
        /// Create a random report on given folder.
        /// We should keep track of newly created reports so we can use in other operations.
        /// </summary>
        [TestMethod]
        public void RndCreateReport()
        {
            string rndReport = CreateNewReportName(this.ContentManager);
            string rndReportPath = this.ContentManager.CreateGenericReport(rndReport, this.ContentManager.ToBeDeletedFolder);
            this.ContentManager.RndContentManager.AddOrReplaceReport(rndReportPath);
        }

        /// <summary>
        /// Create Report Snapshot (ReportItemHistory)
        /// </summary>
        [TestMethod]
        public void RndCreateReportSnapshots()
        {
            string rndReport = this.ContentManager.RndContentManager.GetNextReport();
            RSWarning[] warns = null;
            string snapshotId = null;

            if (!string.IsNullOrEmpty(rndReport))
            {
                snapshotId = this.ContentManager.SoapAccessor.Management.CreateItemHistorySnapshot(rndReport, out warns);
                this.ContentManager.RndContentManager.AddSnapshot(rndReport, snapshotId);
            }
        }

        /// <summary>
        /// List report snapshots for any given report
        /// </summary>
        [TestMethod]
        public void RndListReportSnapshots()
        {
            IContentManager instance = this.ContentManager;
            string rndReport = GetOrCreateNextReport();
            RSItemHistorySnapshot[] snapshots = null;
            RndReportInfo repInfo = null;
            
            if (!string.IsNullOrEmpty(rndReport))
            {
                snapshots = instance.SoapAccessor.Management.ListItemHistory(rndReport);                
                repInfo = instance.RndContentManager.GetReportInfo(rndReport);
                if (snapshots.Count() != repInfo.Snapshots.Count)
                {
                    // no good data, should we fail? Note that somebody could've deleted/added row by now.
                }
            }
        }

        /// <summary>
        /// GetReportHistory Options
        /// </summary>
        [TestMethod]
        public void RndGetReportHistoryOptions()
        {
            IContentManager instance = this.ContentManager;
            string rndReport = GetOrCreateNextReport();
            bool keepReportHistSnapshot = false;
            RSScheduleDefinitionOrReference schedDefOrRef = null;
            bool historyCollected = false;

            if (!string.IsNullOrEmpty(rndReport))
            {
                historyCollected = instance.SoapAccessor.Management.GetItemHistoryOptions(rndReport, out keepReportHistSnapshot, out schedDefOrRef);

                // should verify? Would be nice.
            }
        }

        /// <summary>
        /// Delete a random report, report must exist.
        /// </summary>
        [TestMethod]
        public void RndDeleteReport()
        {
            IContentManager instance = this.ContentManager;
            string rndReport = GetOrCreateNextReport();
            if (!string.IsNullOrEmpty(rndReport))
            {
                instance.SoapAccessor.Management.DeleteItem(rndReport);
                instance.RndContentManager.RemoveReport(rndReport);
            }
        }

        /// <summary>
        /// Simulate where user navigates through folders.
        /// </summary>
        [TestMethod]
        public void RndListChildren()
        {
            IContentManager instance = this.ContentManager;
            string folder = instance.RndContentManager.GetRndFolder();
            if (!string.IsNullOrEmpty(folder))
            {
                instance.SoapAccessor.Management.ListChildren(folder, false);
            }
        }

        /// <summary>
        /// Create a random folder
        /// </summary>
        [TestMethod]
        public void RndCreateFolder()
        {
            IContentManager instance = this.ContentManager;
            string folder = instance.GenerateRndFileName();
            if (!string.IsNullOrEmpty(folder))
            {
                RSCatalogItem item = instance.SoapAccessor.Management.CreateFolder(folder, instance.ToBeDeletedFolder, null);
                if (item != null)
                {
                    instance.RndContentManager.AddFolder(item.Path);
                }
            }
        }

        /// <summary>
        /// Delete random folder (existing one)
        /// </summary>
        [TestMethod]
        public void RndDeleteFolder()
        {
            IContentManager instance = this.ContentManager;
            string folder = instance.RndContentManager.GetRndFolder();
            if (!string.IsNullOrEmpty(folder))
            {
                instance.SoapAccessor.Management.DeleteItem(folder);
                instance.RndContentManager.RemoveFolder(folder);
            }
        }

        /// <summary>
        /// Move Report (Rename)
        /// </summary>
        [TestMethod]
        public void RndMoveReport()
        {
            IContentManager instance = this.ContentManager;
            string rndReport = GetOrCreateNextReport();
            string rndNewName = CreateNewReportName(instance);

            if (!string.IsNullOrEmpty(rndReport))
            {
                if (!rndReport.Contains(rndNewName)) // no dupe
                {
                    int index = rndReport.LastIndexOf('/');
                    string newReportPath = string.Format(@"{0}/{1}", rndReport.Substring(0, index), rndNewName);
                    
                    instance.SoapAccessor.Management.MoveItem(rndReport, newReportPath); // move
                    
                    RndReportInfo repInfo = instance.RndContentManager.GetReportInfo(rndReport);
                    repInfo.ItemPath = newReportPath;
                    instance.RndContentManager.RemoveReport(rndReport);
                    instance.RndContentManager.AddReportInfo(repInfo);
                }
            }
        }

        /// <summary>
        /// Get Report Parameters for any given report
        /// We can use normal list for this one instead of temporary item list
        /// as it is readonly operaton
        /// </summary>
        [TestMethod]
        public void RndGetReportParameters()
        {
            IContentManager instance = this.ContentManager;
            string rndReport = instance.GetNextReport();

            if (!string.IsNullOrEmpty(rndReport))
            {
                RSItemParameter[] parameters = instance.SoapAccessor.Management.GetItemParameters(rndReport, null, false, null, null); // since 
            }
        }

        /// <summary>
        /// Get Item Property Method - get itm property on any given report.
        /// </summary>
        [TestMethod]
        public void GetItemProperties()
        {
            IContentManager instance = this.ContentManager;
            string rndReport = GetOrCreateNextReport();

            if (!string.IsNullOrEmpty(rndReport))
            {
                RSProperty[] properties = instance.SoapAccessor.Management.GetProperties(rndReport, null);
            }
        }

        private string GetOrCreateNextReport()
        {
            string rndReport = this.ContentManager.RndContentManager.GetNextReport();
            if (string.IsNullOrEmpty(rndReport))
            {
                this.RndCreateReport();
                rndReport = this.ContentManager.RndContentManager.GetNextReport();
            }

            return rndReport;
        }

        /// <summary>
        /// Set execution option on random reports
        /// </summary>
        [TestMethod]
        public void RndSetExecutionOptions()
        {
            string[] options = new string[] { "Live", "Snapshot" };
            IContentManager instance = this.ContentManager;
            string rndReport = GetOrCreateNextReport();
            int option = instance.LocalRand.Next(2); // 0 or 1 to select Live or Snapshot

            RSScheduleDefinition schDef = new RSScheduleDefinition();
            schDef.StartDateTime = DateTime.Now.AddMinutes(30.0);
            RSMinuteRecurrence minRecurrence = new RSMinuteRecurrence();
            minRecurrence.MinutesInterval = 60;
            schDef.Item = minRecurrence;

            if (string.IsNullOrEmpty(rndReport)) // should measure this 
                instance.SoapAccessor.Management.SetExecutionOptions(rndReport, options[option], options[option].Equals("Live") ? schDef : null); 
        }


        #endregion

        #region Private Helpers
        /// <summary>
        /// Helper function - Gets a new report name (with extension of .rdl when needed (in SP)
        /// Note: Not complete path, just name.
        /// </summary>
        /// <param name="instance">Intance of ContentManager</param>
        /// <returns>Newly created Report Name</returns>
        private static string CreateNewReportName(IContentManager instance)
        {
            string rndReport = instance.GenerateRndFileName() + instance.GetExtension(".rdl");
            return rndReport;
        }
        
        #endregion
    }    
}
