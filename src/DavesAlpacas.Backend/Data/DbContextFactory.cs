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

namespace DavesAlpacas.Backend.Data
{
    /// <summary>
    /// Factory for creating DB contexts when there is an available initialized service provider.
    /// </summary>
    internal class DbContextFactory : IDbContextFactory
    {
        private DbContextOptions<DavesAlpacasDbContext> _dbContextOptions;

        public DbContextFactory(DbContextOptions<DavesAlpacasDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public DavesAlpacasDbContext CreateDbContext()
        {
            return new DavesAlpacasDbContext(_dbContextOptions);
        }
    }
}

