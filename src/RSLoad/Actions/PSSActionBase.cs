// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace RSLoad
{
    /// <summary>
    /// Base class with common methods to the PSS actions
    /// </summary>
    public class PSSActionBase
    {
        /// <summary>
        /// Begin a measure, checking if the TestContext is available,
        /// need to be able to reuse the action methods outside of performance actions
        /// like initializers
        /// </summary>
        /// <param name="measureName">Name of the metric to measure</param>
        protected void BeginMeasure(string measureName)
        {
            if (TestContext != null)
                TestContext.BeginTimer(measureName);
        }

        /// <summary>
        /// Ends a measure, checking if the TestContext is available,
        /// need to be able to reuse the action methods outside of performance actions
        /// like initializers
        /// NOTE: every measure is represented as a Transaction in the results, the transaction is counted and measured 
        /// even if there is an exception/failure in the test
        /// </summary>
        /// <param name="measureName">Name of the metric to measure</param>
        protected void EndMeasure(string measureName)
        {
            if (TestContext != null)
                TestContext.EndTimer(measureName);
        }

        protected IDisposable Measure(string measureName)
        {
            return new MeasureContext(measureName, TestContext);
        }

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }

            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Disposable class for measuring time spent inside a "using" block
        /// </summary>
        private sealed class MeasureContext : IDisposable
        {
            private readonly string _measureName;
            private readonly TestContext _testContext;

            /// <summary>
            /// Creates a disposable instance of the class and starts measuring.
            /// It is recommended to use this class in the header of a using block.
            /// </summary>
            /// <param name="measureName">Name of the measure</param>
            /// <param name="testContext">Test context</param>
            public MeasureContext(string measureName, TestContext testContext)
            {
                _testContext = testContext;
                _measureName = measureName;

                if (_testContext != null)
                    _testContext.BeginTimer(measureName);
            }

            #region IDisposable Support
            private bool disposedValue = false;

            void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // Stop timer
                        if (_testContext != null)
                            _testContext.EndTimer(_measureName);
                    }

                    disposedValue = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            void IDisposable.Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
            }
            #endregion
        }
    }
}
