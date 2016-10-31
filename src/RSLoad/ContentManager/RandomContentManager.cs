// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSLoad
{
    /// <summary>
    /// Random content manager.
    /// This is just for management, accounting.  This will not do any of SOAP calls for example.
    /// There are a lot of critical section in this code, but OK as test using this will be used in stress only with
    /// low priority weight.
    /// Issue: There is still issue in management when running with multiple agents.
    /// </summary>
    public class RandomContentManager
    {
        private List<string> reports = new List<string>();

        private Dictionary<string, RndReportInfo> infoReports = new Dictionary<string, RndReportInfo>();

        private List<string> Reports 
        { 
            get
            { 
                return (reports); 
            } 
        }

        private Dictionary<string, RndReportInfo> ReportsInfo 
        { 
            get 
            { 
                return infoReports; 
            } 
        }

        private List<string> folders = new List<string>();

        private List<string> Folders
        {
            get
            {
                return (folders);
            }
        }

        private Random rnd = new Random(DateTime.Now.Millisecond);

        private object objectlock = new object();

        /// <summary>
        /// Get Random Report from temporary collection of existing reports
        /// </summary>
        /// <returns>An Report Path</returns>
        public string GetNextReport()
        {
            string reportPath = string.Empty;

            int count = Reports.Count;
            if (count > 0)
                reportPath = Reports[rnd.Next(count)];

            return (reportPath);
        }

        /// <summary>
        /// Remove report from the list
        /// </summary>
        /// <param name="reportPath">report to remove</param>
        public void RemoveReport(string reportPath)
        {
            lock (objectlock)
            {
                if (Reports.Contains(reportPath))
                    Reports.Remove(reportPath);
                if (ReportsInfo.ContainsKey(reportPath))
                    ReportsInfo.Remove(reportPath);
            }
        }

        /// <summary>
        /// Get Report Info for given report
        /// </summary>
        /// <param name="reportPath">Report Item to get info</param>
        /// <returns>RndReportInfo object or null</returns>
        public RndReportInfo GetReportInfo(string reportPath)
        {
            RndReportInfo reportInfo = null;

            if (ReportsInfo.ContainsKey(reportPath))
                reportInfo = ReportsInfo[reportPath];

            return (reportInfo);
        }

        /// <summary>
        /// Update report information.
        /// If aleady exists, just replace, else add
        /// </summary>
        /// <param name="repInfo">Report Information</param>
        public void UpdateReport(RndReportInfo repInfo)
        {
            lock (objectlock)
            {
                if (!Reports.Contains(repInfo.ItemPath))
                {
                    Reports.Add(repInfo.ItemPath);
                    ReportsInfo.Add(repInfo.ItemPath, repInfo);
                }
                else
                {
                    if (ReportsInfo.ContainsKey(repInfo.ItemPath))
                        ReportsInfo[repInfo.ItemPath] = repInfo;
                }
            }
        }

        /// <summary>
        /// Add or replace Report Info
        /// If exists, replace existing item with fresh object.
        /// If new, create one and add to the list.
        /// Replace should not come to play, but if neeeded here it is
        /// </summary>
        /// <param name="reportPath">Report to add to list</param>
        public void AddOrReplaceReport(string reportPath)
        {
            RndReportInfo repInfo = new RndReportInfo(reportPath, false, false);

            lock (objectlock)
            {
                if (Reports.Contains(reportPath))
                {
                    if (ReportsInfo.ContainsKey(reportPath))
                        ReportsInfo[repInfo.ItemPath] = repInfo;
                }
                else
                {
                    Reports.Add(reportPath);
                    if (ReportsInfo.ContainsKey(reportPath))
                        ReportsInfo[repInfo.ItemPath] = repInfo;
                    else
                        ReportsInfo.Add(reportPath, repInfo);
                }
            }
        }

        /// <summary>
        /// Add report info to the list as well as to Reports List
        /// </summary>
        /// <param name="info">Report Information Object</param>
        public void AddReportInfo(RndReportInfo info)
        {
            string reportPath = info.ItemPath;

            lock (objectlock)
            {
                if (Reports.Contains(reportPath))
                {
                    if (ReportsInfo.ContainsKey(reportPath))
                        ReportsInfo[reportPath] = info;
                }
                else
                {
                    if (ReportsInfo.ContainsKey(reportPath))
                        ReportsInfo[reportPath] = info;
                    else
                        ReportsInfo.Add(reportPath, info);
                }
            }
        }

        /// <summary>
        /// Add a new snapshot to snapshot list
        /// </summary>
        /// <param name="report">Report associated with snapshot</param>
        /// <param name="snapshotId">Snapshot to add</param>
        public void AddSnapshot(string report, string snapshotId)
        {
            lock (objectlock)
            {
                RndReportInfo repInfo = ReportsInfo[report];
                if (!repInfo.Snapshots.Contains(snapshotId))
                    repInfo.Snapshots.Add(snapshotId);
                else
                {
                    // already exists.
                }
            }
        }

        /// <summary>
        /// Remove a given snapshot from snapshot list.
        /// </summary>
        /// <param name="report">Report associated with snapshot</param>
        /// <param name="snapshotId">snapshot id to remove</param>
        public void RemoveSnapshot(string report, string snapshotId)
        {
            lock (objectlock)
            {
                RndReportInfo repInfo = ReportsInfo[report];
                if (repInfo.Snapshots.Contains(snapshotId))
                    repInfo.Snapshots.Remove(snapshotId);
            }
        }

        /// <summary>
        /// Add given folder path to collection
        /// </summary>
        /// <param name="folder">folder name</param>
        public void AddFolder(string folder)
        {
            if (!Folders.Contains(folder))
                Folders.Add(folder);
        }

        /// <summary>
        /// Get Random folder from the collection
        /// </summary>
        /// <returns>Folder name</returns>
        public string GetRndFolder()
        {
            string folder = string.Empty;

            int count = Folders.Count;
            if (count > 0)
                folder = Folders[rnd.Next(count)];

            return (folder);
        }

        /// <summary>
        /// Remove given folder from the collectiion
        /// </summary>
        /// <param name="folder">folder to remove</param>
        public void RemoveFolder(string folder)
        {
            lock (objectlock)
            {
                if (Folders.Contains(folder))
                    Folders.Remove(folder);
            }
        }
    }

    /// <summary>
    /// Random Report Information
    /// To keep track of randomly created reports during stress run
    /// Typically, this is used only for stress run.
    /// NOTE: this is just skeleton, will continiue to change
    /// </summary>
    public class RndReportInfo
    {
        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="item">Item/Report name</param>
        /// <param name="hasSnapshot">has snapshot</param>
        /// <param name="hasSubscription">has subscription</param>
        public RndReportInfo(string item, bool hasSnapshot, bool hasSubscription)
        {
            ItemPath = item;
            HasSnapshot = hasSnapshot;
            HasSubscription = hasSubscription;
            Snapshots = new List<string>();
        }

        /// <summary>
        /// Item Path (Report Path)
        /// </summary>
        public string ItemPath { get; set; }

        /// <summary>
        /// Has Snapshot
        /// </summary>
        public bool HasSnapshot { get; set; }

        /// <summary>
        /// Has Subscription on this report
        /// </summary>
        public bool HasSubscription { get; set; }

        /// <summary>
        /// Snapshot List
        /// </summary>
        public List<string> Snapshots { get; set; }
    }
}
