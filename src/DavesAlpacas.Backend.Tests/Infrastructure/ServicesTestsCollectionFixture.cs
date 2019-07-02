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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DavesAlpacas.Backend.Tests.Infrastructure
{
    /// <summary>
    /// Provides shared test collection state and logic, e.g. connection strings and test database setup
    /// </summary>
    public class ServicesTestsCollectionFixture
    {
        /// <summary>
        /// The test DB connection string
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Initializes an instance of the <see cref="ServicesTestsCollectionFixture"/> class.
        /// </summary>
        public ServicesTestsCollectionFixture()
        {
            var dbContextFactory = new DefaultDbContextFactory();
            IConfigurationRoot config = dbContextFactory.Configuration;
            string connectionString = dbContextFactory.ConnectionString;

            bool recreateDbOnStart = (bool?)config.GetValue(typeof(bool?), "RecreateDbOnStart") ?? false;

            performDbSetup(dbContextFactory, recreateDbOnStart);

            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Setup the test database - if it doesn't exist, create it. If it does exist, check the config and then maybe recreate it.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="recreateDbOnStart"></param>
        private static void performDbSetup(IDbContextFactory dbContextFactory, bool recreateDbOnStart)
        {
            // [Re]create DB
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                if (recreateDbOnStart)
                {
                    dbContext.Database.EnsureDeleted();
                }

                dbContext.Database.EnsureCreated();
            }
        }
    }
}
