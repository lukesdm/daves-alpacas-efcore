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

using DavesAlpacas.Backend.Common;
using DavesAlpacas.Backend.Data;
using DavesAlpacas.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DavesAlpacas.Backend.Services
{
    /// <summary>
    /// The herd management service.
    /// </summary>
    /// <remarks>Each call should be self contained, with no leaking of the entity model.</remarks>
    internal class HerdService
    {
        IDbContextFactory _dbContextFactory;

        /// <summary>
        /// Creates an instance of the service.
        /// </summary>
        /// <param name="dbContextFactory">The DB context factory</param>
        /// <remarks>Want to give the service responsibility for managing it's own DB contexts, so just inject the options.</remarks>
        public HerdService(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Adds an alpaca to the herd.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="sex">Sex</param>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <param name="marketValue">Market value, in euros</param>
        /// <returns>The ID of the newly added alpaca</returns>
        public Guid AddAlpaca(string name, Sex sex, DateTime dateOfBirth, decimal marketValue)
        {
            Guid newAlpacaId = Guid.NewGuid();
            var alpaca = new Alpaca()
            {
                Id = newAlpacaId,
                Name = name,
                Sex = sex,
                DateOfBirth = dateOfBirth,
                MarketValue = marketValue
            };

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.Alpacas.Add(alpaca);
                dbContext.SaveChanges();
            };

            return newAlpacaId;
        }

        /// <summary>
        /// Asynchronously adds an alpaca to the herd.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="sex">Sex</param>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <param name="marketValue">Market value, in euros</param>
        /// <returns>A task that will return the ID of the newly added alpaca.</returns>
        public async Task<Guid> AddAlpacaAsync(string name, Sex sex, DateTime dateOfBirth, decimal marketValue)
        {
            Guid newAlpacaId = Guid.NewGuid();
            var alpaca = new Alpaca()
            {
                Id = newAlpacaId,
                Name = name,
                Sex = sex,
                DateOfBirth = dateOfBirth,
                MarketValue = marketValue
            };

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                await dbContext.Alpacas.AddAsync(alpaca);
                await dbContext.SaveChangesAsync();
            };

            return newAlpacaId;
        }

        /// <summary>
        /// Calculates the total herd value.
        /// </summary>
        /// <returns>The total herd value, in Euros.</returns>
        public decimal CalculateTotalValue()
        {
            decimal total;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                total = dbContext.Alpacas.Sum(a => a.MarketValue);
            }
            return total;
        }

        /// <summary>
        /// Asynchronously calculates the total herd value
        /// </summary>
        /// <returns>A task that will return the total herd value, in euros.</returns>
        public async Task<decimal> CalculateTotalValueAsync()
        {
            decimal total;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                total = await dbContext.Alpacas.SumAsync(a => a.MarketValue);
            }
            return total;
        }
    }
}

