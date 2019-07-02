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

namespace DavesAlpacas.Testing.Tools
{
    /// <summary>
    /// A Log Entry for use with the .Net Core logging APIs
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// The formatted message
        /// </summary>
        public string Message { get; }
        
        /// <summary>
        /// The log level
        /// </summary>
        public LogLevel LogLevel { get; }
        
        /// <summary>
        /// The Event ID
        /// </summary>
        public EventId EventId { get; }
        
        /// <summary>
        /// The State
        /// </summary>
        public object State { get; }
        
        /// <summary>
        /// The Exception (if any)
        /// </summary>
        public Exception Exception { get; }
        
        /// <summary>
        /// The Correlation ID (if any)
        /// </summary>
        public CorrelationId CorrelationId { get; }
        
        /// <summary>
        /// The log category
        /// </summary>
        public string Category { get; }
        
        /// <summary>
        /// The timestamp
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="correlationId"></param>
        /// <param name="category"></param>
        /// <param name="timestamp"></param>
        public LogEntry(string message, LogLevel logLevel, EventId eventId, object state, Exception exception, CorrelationId correlationId, string category, DateTime timestamp)
        {
            this.Message = message;
            this.LogLevel = logLevel;
            this.EventId = eventId;
            this.State = state;
            this.Exception = exception;
            this.CorrelationId = correlationId;
            this.Category = category;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Generates a formatted string of the form {Timestamp}: {Message},
        /// e.g. "2019-06-19T11:55:23.7231395Z: Process started".
        /// </summary>
        /// <returns>A formatted string, e.g. "2019-06-19T11:55:23.7231395Z: Process started".</returns>
        public override string ToString()
        {
            // Format like 2019-06-19T11:55:23.7231395Z
            string timestamp = Timestamp.ToString("o");

            return $"{timestamp}: {this.Message}";
        }
    }
}