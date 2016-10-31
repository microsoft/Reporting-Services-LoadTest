// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System.Collections.Generic;

namespace RSLoad
{
    /// <summary>
    /// Factory to create Content Manager Instance for use
    /// </summary>
    public class ContentManagerFactory
    {
        private static object syncRoot = new object();
        private static Dictionary<string, IContentManager> _scenarioContentInstance = new Dictionary<string, IContentManager>();

        /// <summary>
        /// Instance of the content manager
        /// </summary>
        /// <param name="scenario">Name of the scenario for the content manager instance, if not specified it uses a single scenario</param>
        /// <returns>The independent content manager associated with the scenario </returns>
        public static IContentManager GetInstance(string scenario)
        {
            IContentManager contentInstance;

            // Muti-thread singleton http://msdn.microsoft.com/en-us/library/ff650316.aspx 
            lock (syncRoot)
            {
                if (!_scenarioContentInstance.TryGetValue(scenario, out contentInstance))
                {
                    if (contentInstance == null)
                    {
                        contentInstance = new PortalContentManager();
                    }
                    _scenarioContentInstance.Add(scenario, contentInstance);
                }
            }

            return contentInstance;
        }
    }
}
