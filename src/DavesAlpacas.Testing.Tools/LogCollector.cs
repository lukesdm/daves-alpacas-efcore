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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DavesAlpacas.Testing.Tools
{
    /// <summary>
    /// In-memory log collector.
    /// </summary>
    /// <remarks>
    /// * Not thread safe.
    /// * Scopes handling has not been implemented.
    /// </remarks>
    internal class LogCollector : ILogger
    {
        /// <summary>
        /// Whether collection is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Whether to attempt to record the correlation ID during collection.
        /// </summary>
        public bool RecordCorrelationId { get; set; }

        /// <summary>
        /// The log category
        /// </summary>
        public string Category { get; }

        private ReadOnlyCollection<LogEntry> Entries { get; }

        private readonly List<LogEntry> _entries;


        /// <summary>
        /// Gets the entries that have been generated during collection
        /// </summary>
        /// <param name="clear">Whether to clear the entries after retrieval</param>
        /// <returns>The collected log entries</returns>
        /// <remarks>Use the <paramref name="clear"/> parameter if re-using this logger instance and memory use is a concern.</remarks>
        public ReadOnlyCollection<LogEntry> GetEntries(bool clear = false)
        {
            ReadOnlyCollection<LogEntry> results;
            if (!clear)
            {
                results = this.Entries;
            }
            else
            {
                results = _entries.ToList().AsReadOnly();
                _entries.Clear();
            }

            return results;
        }

        /// <summary>
        /// Creates a new instance of <see cref="LogCollector"/>
        /// </summary>
        /// <param name="category">The logging category</param>
        public LogCollector(string category)
        {
            _entries = new List<LogEntry>();
            Entries = new ReadOnlyCollection<LogEntry>(_entries);
            Category = category;
        }

        #region ILogger Members

        public IDisposable BeginScope<TState>(TState state)
        {
            // COULD-DO: Scope handling logic
            return new ScopeHolder<TState>();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return this.Enabled;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.Enabled)
            {
                return;
            }

            CorrelationId correlationId = RecordCorrelationId ? LogCollectorProvider.GetCorrelationId() : new CorrelationId(null);

            this._entries.Add(new LogEntry(message: formatter.Invoke(state, exception),
                logLevel, eventId, state, exception, correlationId, this.Category, DateTime.UtcNow));
        }

        #endregion

        private class ScopeHolder<TState> : IDisposable
        {
            public ScopeHolder()
            {
                // COULD-DO: Scope opening logic
            }
            public void Dispose()
            {
                // COULD-DO: Scope closing logic
            }
        }
    }
}

