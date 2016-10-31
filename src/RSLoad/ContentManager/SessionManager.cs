// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RSAccessor.SoapAccessor;
using RSAccessor.UrlAccessor;
using RSAccessor.Utilities;
using RSLoad.Utilities;

namespace RSLoad
{
    /// <summary>
    /// SessionID List just to simply the structure a little.  
    /// </summary>
    public class SessionIdList
    {
        /// <summary>
        /// Constructor for SessionIdList
        /// </summary>
        /// <param name="tid">managed therad id</param>
        /// <param name="sessionIdList">SessioinIdlifeTime Object</param>
        public SessionIdList(int tid, Dictionary<string, SessionIdLifeTime> sessionIdList)
        {
            Tid = tid;
            SessionIds = sessionIdList;
        }
        
        /// <summary>
        /// Managed Thread Id
        /// </summary>
        public int Tid { get; set; }

        /// <summary>
        /// SessionIds - List of Session Ids.
        /// </summary>
        public Dictionary<string, SessionIdLifeTime> SessionIds { get; set; }
    }

    /// <summary>
    /// Pro Report Session Manager.
    /// Internally, we are managing session pool per thread as we don't want to share same sessionid between two request in parallel.
    /// For example, print scenario may have issue with two use going with same sessionid in parallel.
    /// </summary>
    public class SessionManager
    {
        private IContentManager _contentManager;

        /// <summary>
        /// Constructor of the session manager
        /// </summary>
        /// <param name="contentManager">Content Manager <see cref="IContentManager"/> which this session manager belons </param>
        public SessionManager(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        private ConcurrentDictionary<int, SessionIdList> sessionIdListByTid = new ConcurrentDictionary<int, SessionIdList>();

        private object insertlock = new object();

        /// <summary>
        /// Add given session id to current thread's session list.
        /// </summary>
        /// <param name="session">SessionLifeTime object</param>
        protected void Add(SessionIdLifeTime session)
        {
            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            sessionIdListByTid.AddOrUpdate(
                tid, 
                InitializeSessionList(tid, session),
                (tidForUpdateLambda, existingSessionListXReport) =>
                {
                    // The lambda creates a copy of the SessionListxReport as a new local variable so is not affected by other theads
                    // then the update option of the collection will have the valid sessionListxReport of the last thread that was invoked
                    SessionIdList newSessionListxReport = new SessionIdList(tidForUpdateLambda, existingSessionListXReport.SessionIds);
                    if (!newSessionListxReport.SessionIds.ContainsKey(session.ItemPath))
                        newSessionListxReport.SessionIds.Add(session.ItemPath, session);
                    return newSessionListxReport;
                });
        }

        private SessionIdList InitializeSessionList(int threaId, SessionIdLifeTime session)
        {
            Dictionary<string, SessionIdLifeTime> sessionListxReport = new Dictionary<string, SessionIdLifeTime>();
            sessionListxReport.Add(session.ItemPath, session);
            return new SessionIdList(threaId, sessionListxReport);
        }

        /// <summary>
        /// Create a new session from a given Report
        /// </summary>
        /// <param name="reportPath">Report Path</param>
        /// <returns>Newly created session id</returns>
        protected SessionIdLifeTime CreateNewSession(string reportPath)
        {
            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            string url = _contentManager.ConstructUrl(reportPath, new MHTMLRender(), null);
            string execid;
            _contentManager.IssueGetRequest(url, out execid);  // can also use loadreport mechanism
            SessionIdLifeTime newSes = new SessionIdLifeTime(execid, reportPath);
            return (newSes);
        }

        /// <summary>
        /// Get an active session id for a given report
        /// If one does not exist, we go ahead and create one and return that
        /// </summary>
        /// <param name="reportPath">Report Path</param>
        /// <returns>SessionID aka ExecutionID</returns>
        public string GetSessionID(string reportPath)
        {
            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            string sessionid = string.Empty;
            SessionIdLifeTime newSes = null;

            if (sessionIdListByTid.Keys.Contains(tid))
            {
                Dictionary<string, SessionIdLifeTime> sessions = sessionIdListByTid[tid].SessionIds;
                if (sessions.Keys.Contains(reportPath))
                {
                    newSes = sessions[reportPath];

                    if (newSes.TimedOut)
                    {
                        sessions.Remove(reportPath);
                        newSes = null;
                    }
                }
            }

            if (newSes == null) 
            {
                newSes = CreateNewSession(reportPath);
                Add(newSes);
            }

            sessionid = newSes.UpdateLastUse();
            return (sessionid);
        }
    }
}
