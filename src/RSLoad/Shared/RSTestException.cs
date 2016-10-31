// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSLoad
{
    /// <summary>
    /// Convenience exception class. Provides a useful constructor override
    /// that will automatically format the exception message using string.Format().
    /// Please note that a format string compatible message is not required to use
    /// the params[] constructor form. It works just fine for a regular unformatted
    /// exception message as well.
    /// <example>
    /// throw new FormattedException("I am an unformatted message.");
    /// throw new FormattedException("I am a {0} message.", "formatted");
    /// </example>
    /// </summary>
    public abstract class FormattedException : Exception
    {
        /// <summary>
        /// Constructs a new exception, with no message.
        /// </summary>
        public FormattedException()
            : base()
        {
        }

        /// <summary>
        /// Constructs a new exception with a message, which may be a compatible
        /// .NET format string, and optional parameters to format into the message.
        /// </summary>
        /// <param name="message">Exception message. May be a .NET compatible format string.</param>
        /// <param name="parameters">Parameters used to format the given format string.</param>
        public FormattedException(string message, params object[] parameters)
            : base(string.Format(message, parameters))
        {
        }
    }

    /// <summary>
    /// Base exception class for Rosetta Test code. Used where the other,
    /// more specific exception types are not applicable.
    /// </summary>
    public class RSTestException : FormattedException
    {
        /// <summary>
        /// Constructs a new exception, with no message.
        /// </summary>
        public RSTestException()
            : base()
        {
        }

        /// <summary>
        /// Constructs a new exception with a message, which may be a compatible
        /// .NET format string, and optional parameters to format into the message.
        /// </summary>
        /// <param name="message">Exception message. May be a .NET compatible format string.</param>
        /// <param name="parameters">Parameters used to format the given format string.</param>
        public RSTestException(string message, params object[] parameters)
            : base(message, parameters)
        {
        }
    }
}
