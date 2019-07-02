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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DavesAlpacas.Backend.Data
{
    /// <summary>
    /// Factory for creating DB contexts when there is no service provider available.
    /// </summary>
    /// <remarks>Required for the design-time tools and test database setup</remarks>
    internal class DefaultDbContextFactory : IDbContextFactory, IDesignTimeDbContextFactory<DavesAlpacasDbContext>
    {
        public IConfigurationRoot Configuration { get; }
        public string ConnectionString { get; }
        public DbContextOptions<DavesAlpacasDbContext> Options { get; }

        public DefaultDbContextFactory()
        {
            this.Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

#if SQLITE
            this.ConnectionString = Configuration.GetConnectionString("SqliteConnection");
            
            var serviceProviderBuilder = new ServiceCollection()
            .AddEntityFrameworkSqlite();

            this.Options = new DbContextOptionsBuilder<DavesAlpacasDbContext>()
            .UseSqlite(ConnectionString)
            .UseInternalServiceProvider(serviceProviderBuilder.BuildServiceProvider())
                .Options;
#else
            this.ConnectionString = Configuration.GetConnectionString("PostgresConnection");

            var serviceProviderBuilder = new ServiceCollection()
                .AddEntityFrameworkNpgsql();

            this.Options = new DbContextOptionsBuilder<DavesAlpacasDbContext>()
                .UseNpgsql(ConnectionString)
                .UseInternalServiceProvider(serviceProviderBuilder.BuildServiceProvider())
                .Options;
#endif
        }

        /// <summary>
        /// Used by the EF designer tools
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DavesAlpacasDbContext CreateDbContext(string[] args)
        {
            return CreateDbContext();
        }

        public DavesAlpacasDbContext CreateDbContext()
        {
            return new DavesAlpacasDbContext(this.Options);
        }
    }
}

