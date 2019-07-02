/*
Project: Daves Alpacas, demo code for the article 'Entity Framework Core - Taming the Beast'
Creator: Luke McQuade, 2019
Copyright: Luke McQuade, 2019
License: This Source Code Form is subject to the terms of the Mozilla Public License, 
         v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one
         at https://mozilla.org/MPL/2.0/.
         This Source Code Form is "Incompatible With Secondary Licenses", 
          as defined by the Mozilla Public License, v. 2.0.
*/

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DavesAlpacas.Testing.Tools
{
    /// <summary>
    /// In-memory log collector provider
    /// </summary>
    public class LogCollectorProvider : ILoggerProvider
    {
        private readonly ConcurrentBag<LogCollector> _loggers = new ConcurrentBag<LogCollector>();

        /// <summary>
        /// A reference to the 'Collect' method, used to work out the correlation ID from, in the stack.
        /// </summary>
        private static MethodBase CorrelationParent { get; }

        /// <summary>
        /// Whether any loggers have been created.
        /// </summary>
        public bool HasLoggers { get => _loggers.Any(); }

        /// <summary>
        /// Type initializer for the <see cref="LogCollectorProvider"/> class.
        /// </summary>
        static LogCollectorProvider()
        {
            CorrelationParent = typeof(LogCollectorProvider).GetMethod(nameof(Collect));
        }

        /// <summary>
        /// Collects logs for the specified work
        /// </summary>
        /// <param name="work">The work to perform to capture logging for. This should to be an anonymous method that finishes with a call to <see cref="GetCorrelationId" /></param>
        /// <returns>The results, as a collection of log entries, in chronological order.</returns>
        /// <remarks>
        /// Current implementation is fragile and doesn't work very well with async, due to <see cref="GetCorrelationId"/>.
        /// (Referred to as 'Collect V0'.)
        /// </remarks>
        public LogEntry[] Collect(Func<CorrelationId> work)
        {
            foreach (LogCollector logger in _loggers)
            {
                logger.RecordCorrelationId = true;
                logger.Enabled = true;
            }

            CorrelationId correlationId = work.Invoke();

            var logEntries = _loggers
                .SelectMany(l => l.GetEntries())
                .Where(le => le.CorrelationId.Equals(correlationId));

            foreach (LogCollector logger in _loggers)
            {
                logger.RecordCorrelationId = false;
                logger.Enabled = false;
            }

            return logEntries.OrderBy(le => le.Timestamp)
                .ToArray();
        }

        /// <summary>
        /// Creates an instance of the <see cref="LogCollector"/> class.
        /// </summary>
        /// <param name="categoryName">The log category.</param>
        /// <returns>The newly created logger.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            LogCollector logger = new LogCollector(categoryName);
            _loggers.Add(logger);
            return logger;
        }

        /// <summary>
        /// Starts collecting log entries, until <see cref="EndCollect" /> is called.
        /// </summary>
        /// <remarks(Referred to as 'Collect V1'.)</remarks>
        public void StartCollect()
        {
            foreach (LogCollector logger in _loggers)
            {
                logger.Enabled = true;
            }
        }

        /// <summary>
        /// Stops log collection and returns results.
        /// </summary>
        /// <returns>The results, as a collection of log entries in chronological order.</returns>
        /// <remarks>(Referred to as 'Collect V1'.)</remarks>
        public LogEntry[] EndCollect()
        {
            IEnumerable<LogEntry> results = Enumerable.Empty<LogEntry>();
            foreach (LogCollector logger in _loggers)
            {
                logger.Enabled = false;
                results = results.Concat(logger.GetEntries());
            };

            return results.OrderBy(le=> le.Timestamp)
                .ToArray();
        }

        /// <summary>
        /// Performs clean-up.
        /// </summary>
        public void Dispose()
        {
            // Nothing to do.
        }

        /// <summary>
        /// Calculates a correlation ID, based on the current stack.
        /// </summary>
        /// <returns>A Correlation Id</returns>
        /// <remarks>
        /// The way it works is that it walks up the stack until a 'Collect' call is identified, 
        ///  and then grabs a reference to the call below that, which should be the user's anonymous method.
        /// As it's an anonymous method, it should act as a unique identifier for that particular usage.
        /// It's quite fragile, in that it won't work if the user abstracts it to a regular method, 
        ///  or calls async code, or probably under other compilation conditions too.
        /// Finally, it won't be able to uniquely handle concurrent runs of the same method,
        ///  which might have implications for tests running in parallel (workaround - use a mutex)
        /// </remarks>
        public static CorrelationId GetCorrelationId() // COULD-DO: Extract from here and allow user-defined behaviour.
        {
            var stackFrames = new StackTrace().GetFrames();
            MethodBase current = null, previous;

            // Want to get a reference to the lambda executing directly below the Collect method - this will be unique to each place it's called*, 
            // so can be used to correlate the log entries.
            // * Unless the consumer abstracts it - need to clearly document that.
            for (int i = 0; i < stackFrames.Length; i++)
            {
                previous = current;

                if ((current = stackFrames[i].GetMethod()) == CorrelationParent)
                {
                    Debug.Assert(previous != null); // Could add other sensible assertions e.g. is in test class

                    return new CorrelationId(previous);
                }
            }

            // If we got here, it indicates that we've not been able to find the Collect method in the stack,
            //  probably due to an async call
            return new CorrelationId(null);
        }
    }
}