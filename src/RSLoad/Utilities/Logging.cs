// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Diagnostics;

namespace RSLoad.Utilities
{
    public class Logging
    {
        /// <summary>
        /// Log wrapper
        /// </summary>
        /// <param name="format">string.format for the logging</param>
        /// <param name="arg">string.format arguments </param>
        public static void Log(string format, params Object[] arg)
        {
            string timestamp = String.Format("RSLOAD UTC {0}:\t", DateTime.Now.ToUniversalTime());
            Trace.WriteLine(String.Format(timestamp + format, arg));
            Debug.WriteLine(String.Format(timestamp + format, arg));
        }
    }
}