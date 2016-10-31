// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSLoad.Utilities
{
    /// <summary>
    /// Class that holds an existing session id and the last time it was requested
    /// </summary>
    [Serializable]
    public class SessionIdLifeTime
    {
        private const int DefaultTimeoutSeconds = 540;

        private String sessionId = null;
        private long lastUsed;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sessionId">valid sessionid</param>
        /// <param name="itemPath">the model that this session is using</param>
        public SessionIdLifeTime(string sessionId, string itemPath)
        {
            this.sessionId = sessionId;
            ItemPath = itemPath;
            lastUsed = DateTime.Now.ToBinary();
        }

        /// <summary>
        /// property to get the session id
        /// </summary>
        public String SessionId
        {
            get { return sessionId; }
        }

        /// <summary>
        /// returns the sessionid and update the lastused time
        /// </summary>
        /// <returns>the sessionid</returns>
        public string UpdateLastUse()
        {
            if (!TimedOut)
                Interlocked.Exchange(ref lastUsed, DateTime.Now.ToBinary());

            return sessionId;
        }

        /// <summary>
        /// indicates if the session is alive
        /// </summary>
        /// <param name="session">sessionIdLife object</param>
        /// <returns>true if it timed out</returns>
        public bool TimedOut
        {
            get
            {
                DateTime dtValue = DateTime.FromBinary(lastUsed);
                return DateTime.Now.Subtract(TimeSpan.FromSeconds(DefaultTimeoutSeconds)) > dtValue;
            }
        }

        /// <summary>
        /// A session is tied to one specific item or model
        /// </summary>
        public string ItemPath
        {
            get;
            set;
        }

        /// <summary>
        /// Indicated if this session is in use by any other request
        /// </summary>
        public bool InUse
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Collection of live session ids
    /// </summary>
    [Serializable]
    public class SessionIdCollection
    {
        private const int MaxActiveSession = 500; // 20 threads, 10 sessions per thread
        private int _numberOfsessionsAdded;

        private string _modelPath;
        private ConcurrentQueue<SessionIdLifeTime> _sessionList;
        private ConcurrentQueue<string> _expiredSessionList;

        /// <summary>
        /// List of sessions belonging to a model
        /// </summary>
        public ConcurrentQueue<SessionIdLifeTime> ModelSessions
        {
            get
            {
                return _sessionList;
            }
        }

        /// <summary>
        /// Return all the expired sessionIds and resets the list
        /// </summary>
        /// <returns>List of sessionIds</returns>
        public ConcurrentQueue<string> DequeueAllExpiredModelSessionIds()
        {
            ConcurrentQueue<string> queueToReturn = new ConcurrentQueue<string>(_expiredSessionList);
            _expiredSessionList = new ConcurrentQueue<string>();
            return queueToReturn;
        }

        /// <summary>
        /// Creates a collection of sessions for a specifi model
        /// </summary>
        /// <param name="modelPath">model path for the sessions</param>
        public SessionIdCollection(string modelPath)
        {
            _modelPath = modelPath;
            _sessionList = new ConcurrentQueue<SessionIdLifeTime>();
            _expiredSessionList = new ConcurrentQueue<string>();
        }

        /// <summary>
        /// Add a new session for this particular model
        /// </summary>
        /// <param name="sessionId">session id</param>
        /// <returns>Return true if session added</returns>
        public Boolean AddSession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                throw new Exception("A session with a null or empty string is not valid, can't be added to the session collection");

            // only add back the session if there are less session than the maximum
            if (_sessionList.Count < MaxActiveSession)
            {
                _sessionList.Enqueue(new SessionIdLifeTime(sessionId, _modelPath));
                _numberOfsessionsAdded++;

                // return true if session added to the session list
                return true;
            }
            else
            {
                // return false if the session pool is full and don't need to add to the session list
                return false;
            }
        }

        /// <summary>
        /// Returns one existing session id
        /// </summary>
        /// <param name="exclusive">identify if the session can't be used for any other request</param>
        /// <returns>Session ID</returns>
        public String GetAny(bool exclusive)
        {
            SessionIdLifeTime sessionToReturn = null;
            bool isExpiredSession = true;
            int sessionRetries = 0;
            while (isExpiredSession)
            {
                while (!_sessionList.TryDequeue(out sessionToReturn))
                {
                    Logging.Log("No session available, retry to get a new session in 100 ms");
                    sessionRetries++;
                    System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    if (sessionRetries > 10)
                        throw new RSTestException("Not able to return a session in less than 1 second, the number of sessions added to the collection is {0}, model {1}, session list count={2}", _numberOfsessionsAdded, _modelPath, _sessionList.Count);
                }

                isExpiredSession = sessionToReturn.TimedOut;

                // if session is expired, add it to the expiredSessionList
                if (isExpiredSession)
                    _expiredSessionList.Enqueue(sessionToReturn.SessionId);

                sessionToReturn.UpdateLastUse();

                // if session is not exclusive and not expired returning to the queue to optimize reuse of sessions
                if (!exclusive && !isExpiredSession)
                    _sessionList.Enqueue(sessionToReturn);
            }

            return sessionToReturn.SessionId;
        }
    }
}
