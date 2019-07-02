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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DavesAlpacas.Backend.Services
{
    /// <summary>
    /// Service for calculating shearing stats
    /// </summary>
    internal class ShearingStatsService
    {
        private readonly IDbContextFactory _dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShearingStatsService" />
        /// </summary>
        /// <param name="dbContextFactory">The DbContext factory.</param>
        public ShearingStatsService(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Get the all-time top 3 highest-yield shearings.
        /// </summary>
        /// <returns>The top 3 shearing results (or fewer, if there have been fewer than 3 total shearings).</returns>
        public TopShearingResult[] GetTop3AllTime()
        {
            TopShearingResult[] results = null;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                IQueryable<TopShearingResult> query = (
                    from shearing in dbContext.Shearings
                    join alpaca in dbContext.Alpacas on shearing.Alpaca equals alpaca
                    orderby shearing.Yield descending
                    select new TopShearingResult(alpaca.Id, alpaca.Name, shearing.Date, shearing.Yield)
                ).Take(3);

                results = query.ToArray();
            }

            Debug.Assert(results != null);

            return results;
        }

        /// <summary>
        /// Asynchrously gets the all-time top 3 highest-yield shearings.
        /// </summary>
        /// <returns>A task that will return the top 3 shearing results.</returns>
        async public Task<TopShearingResult[]> GetTop3AllTimeAsync()
        {
            TopShearingResult[] results = null;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                IQueryable<TopShearingResult> query = (
                    from shearing in dbContext.Shearings
                    join alpaca in dbContext.Alpacas on shearing.Alpaca equals alpaca
                    orderby shearing.Yield descending
                    select new TopShearingResult(alpaca.Id, alpaca.Name, shearing.Date, shearing.Yield)
                ).Take(3);

                results = await query.ToArrayAsync();
            }

            Debug.Assert(results != null);

            return results;
        }

        /// <summary>
        /// Gets the top 3 shearings by yield for the specified year.
        /// </summary>
        /// <param name="year">The year, in 'YYYY' format.</param>
        /// <returns>The top 3 shearing results (or fewer if there were fewer than 3 total shearings in the year).</returns>
        public TopShearingResult[] GetTop3ForYear(int year)
        {
            TopShearingResult[] results = null;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                IQueryable<TopShearingResult> query = (
                    from shearing in dbContext.Shearings
                    where shearing.Date.Year == year
                    orderby shearing.Yield descending
                    join alpaca in dbContext.Alpacas on shearing.Alpaca equals alpaca
                    select new TopShearingResult(alpaca.Id, alpaca.Name, shearing.Date, shearing.Yield)
                ).Take(3);

                results = query.ToArray();
            }

            Debug.Assert(results != null);

            return results;
        }

        /// <summary>
        /// Asynchronously gets the top 3 shearings by yield for the specified year.
        /// </summary>
        /// <param name="year">The year, in 'YYYY' format.</param>
        /// <returns>A task that will return the top 3 shearing results.</returns>
        public async Task<TopShearingResult[]> GetTop3ForYearAsync(int year)
        {
            TopShearingResult[] results = null;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                IQueryable<TopShearingResult> query = (
                    from shearing in dbContext.Shearings
                    where shearing.Date.Year == year
                    orderby shearing.Yield descending
                    join alpaca in dbContext.Alpacas on shearing.Alpaca equals alpaca
                    select new TopShearingResult(alpaca.Id, alpaca.Name, shearing.Date, shearing.Yield)
                ).Take(3);

                results = await query.ToArrayAsync();
            }

            Debug.Assert(results != null);

            return results;
        }

        /// <summary>
        /// Result of a Top Shearing calculation.
        /// </summary>
        /// <remarks>
        /// COULD-DO: Extract to separate file, depending on how the service would be used, 
        ///           e.g. a View Model or a Response Model.
        /// </remarks>
        public class TopShearingResult
        {
            /// <summary>
            /// The alpaca ID
            /// </summary>
            public Guid AlpacaId { get; }

            /// <summary>
            /// The alpaca name
            /// </summary>
            public string AlpacaName { get; }

            /// <summary>
            /// The shearing date
            /// </summary>
            public DateTime Date { get; }

            /// <summary>
            /// The fleece yield, in grams
            /// </summary>
            public int Yield { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="TopShearingResult"/> class.
            /// </summary>
            /// <param name="alpacaId">The alpaca ID</param>
            /// <param name="alpacaName">The alpaca name</param>
            /// <param name="date">The shearing date</param>
            /// <param name="yield">The fleece yield, in grams</param>
            public TopShearingResult(Guid alpacaId, string alpacaName, DateTime date, int yield)
            {
                AlpacaId = alpacaId;
                AlpacaName = alpacaName;
                Date = date;
                Yield = yield;
            }
        }
    }
}

