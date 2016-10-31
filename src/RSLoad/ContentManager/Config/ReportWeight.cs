// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace RSLoad
{
    /// <summary>
    /// Represents a report and the weight to use in the selection of random reports
    /// </summary>
    [Serializable]
    public class ReportWeight
    {
        /// <summary>
        /// Report Name
        /// </summary>
        [XmlAttribute("Report")]
        public string Report { get; set; }

        /// <summary>
        /// Numeric Weight for the report
        /// </summary>
        [XmlAttribute("Weight")]
        public int Weight { get; set; }
    }
}
