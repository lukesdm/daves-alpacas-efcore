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

using DavesAlpacas.Backend.Data;
using DavesAlpacas.Testing.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace DavesAlpacas.Backend.Tests.Infrastructure
{
    /// <summary>
    /// Per-test data and related operations for a database integration test.
    /// </summary>
    internal class TestContext
    {
        /// <summary>
        /// The DbContext factory - provided to the SUT
        /// </summary>
        public DbContextFactory DbContextFactory { get; }

        private readonly LogCollectorProvider _logCollectorProvider = new LogCollectorProvider();
        private readonly ITestOutputHelper _xunitOutput;
        private readonly ServicesTestsCollectionFixture _collectionFixture;

        public TestContext(ITestOutputHelper testOutputHelper, ServicesTestsCollectionFixture collectionFixture)
        {
            _xunitOutput = testOutputHelper;
            _collectionFixture = collectionFixture;

            var serviceProviderBuilder = new ServiceCollection()
#if SQLITE
                .AddEntityFrameworkSqlite()
#else
                .AddEntityFrameworkNpgsql()
#endif
                .AddLogging(c =>
                    c.AddDebug()
                    .AddProvider(_logCollectorProvider)
                    .AddFilter<LogCollectorProvider>(LogCategories.Ef, LogLevel.Information)
                    );

            var options = new DbContextOptionsBuilder<DavesAlpacasDbContext>()
#if SQLITE
                .UseSqlite(_collectionFixture.ConnectionString)
#else
                .UseNpgsql(_collectionFixture.ConnectionString)
#endif
                .UseInternalServiceProvider(serviceProviderBuilder.BuildServiceProvider())
                .EnableSensitiveDataLogging()
                .Options;

            this.DbContextFactory = new DbContextFactory(options);

            initLoggers();
        }

        /// <summary>
        /// Starts collecting log entries, until <see cref="EndCollect" /> is called
        /// </summary>
        /// <remarks>The V1 collection mechanism</remarks>
        public void StartCollect()
        {
            _logCollectorProvider.StartCollect();
        }

        /// <summary>
        /// Stops log collection and returns results
        /// </summary>
        /// <returns>The results, as a collection of log entries in chronological order.</returns>
        /// <remarks>The V1 collection mechanism</remarks>
        public LogEntry[] EndCollect()
        {
            return _logCollectorProvider.EndCollect();
        }

        /// <summary>
        /// Collects logs for the specified work
        /// </summary>
        /// <param name="work">The work to perform to capture logging for. This should to be an anonymous method that finishes with a call to <see cref="GetCorrelationId" /></param>
        /// <returns>The results, as a collection of log entries</returns>
        /// <remarks>The V0 collection mechanism</remarks>
        public LogEntry[] Collect(Func<CorrelationId> work)
        {
            return _logCollectorProvider.Collect(work);
        }

        /// <summary>
        /// Attaches the specified items to the test result report
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="items">Items to attach</param>
        public void Attach<T>(IEnumerable<T> items)
        {
            foreach (object item in items)
            {
                _xunitOutput.WriteLine(item.ToString());
            }
        }

        private void initLoggers()
        {
            // If StartCollect is called before any database calls have been 
            // made, the loggers won't have been initialised. So, make a 
            // throwaway call here to trigger their creation 

            if (!_logCollectorProvider.HasLoggers)
            {
                var dbContext = this.DbContextFactory.CreateDbContext();
                var _ = dbContext.Alpacas.Find(Guid.Empty);
            }
        }
    }
}

