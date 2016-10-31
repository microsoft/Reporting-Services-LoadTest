// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace RSLoad
{
    /// <summary>
    /// Represents a not allowed combination of Report and TestMethods
    /// </summary>
    [Serializable]
    public class BadCombination
    {
        /// <summary>
        /// Method which have some reports that are not valid in the test
        /// </summary>
        [XmlAttribute("Method")]
        public string Method { get; set; }

        /// <summary>
        /// Invalid reports for the method
        /// </summary>
        [XmlElement("InvalidReport")]
        public List<string> InvalidReports { get; set; }
    }
}
