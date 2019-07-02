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

using DavesAlpacas.Testing.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace DavesAlpacas.Backend.Tests.Infrastructure
{
    /// <summary>
    /// Common logic for database integration tests.
    /// </summary>
    public abstract class DbIntegrationTestBase
    {
        /// <summary>
        /// Per-test data
        /// </summary>
        private protected TestContext TestContext { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbIntegrationTestBase"/> class.
        /// </summary>
        /// <param name="testOutputHelper">The xUnit output helper.</param>
        /// <param name="collectionFixture">The collection fixture.</param>
        public DbIntegrationTestBase(ITestOutputHelper testOutputHelper, ServicesTestsCollectionFixture collectionFixture)
        {
            this.TestContext = new TestContext(testOutputHelper, collectionFixture);
            prepareTestData();
        }

        /// <summary>
        /// Helper method to get the number of log entries for the specified category
        /// </summary>
        /// <param name="log">The log entries</param>
        /// <param name="category">The category to filter on, or <see langword="null"/> to check all entries.</param>
        /// <returns></returns>
        /// <returns>The number of log entries for the specified category.</returns>
        /// <remarks>NOTE: For demo purposes, this will filter out SQLite 'PRAGMA' commands.</remarks>
        protected int GetLogEntryCount(IEnumerable<LogEntry> log, string category)
        {
            var filteredLog = log;

#if SQLITE
            // Because we're creating db contexts without an existing connection instance, we always get a PRAGMA command.
            // For demo purposes and to not diverge from Postgresql behaviour, ignore these.
            filteredLog = log.Where(le => !(le.Category == LogCategories.EfDbCommand
                && (le.Message.Split('\n').ElementAtOrDefault(1)?.StartsWith("PRAGMA") ?? false)));
#endif

            int count = category != null ?
                filteredLog.Count(le => le.Category == category)
                : filteredLog.Count();
            return count;
        }

        private void prepareTestData()
        {
            cleanUpOldData();

            Importer importer = new Importer(TestContext.DbContextFactory);
            importer.ImportAlpacas();
            importer.ImportShearings();
        }

        private void cleanUpOldData()
        {
            using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
            {
                dbContext.Shearings.RemoveRange(dbContext.Shearings);
                dbContext.Alpacas.RemoveRange(dbContext.Alpacas);
                dbContext.SaveChanges();
            }
        }
    }
}

