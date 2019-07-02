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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DavesAlpacas.Backend.Tests.Infrastructure
{
    /// <summary>
    /// A basic test data importer - imports tab separated text to the DB
    /// </summary>
    internal class Importer
    {
        const string AlpacasFile = "Alpacas.txt";
        const string ShearingsFile = "Shearings.txt";
        readonly IDbContextFactory _dbContextFactory;

        public Importer(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void ImportAlpacas()
        {
            Debug.Assert(File.Exists(AlpacasFile));

            string[] lines = File.ReadAllLines(AlpacasFile);
            Debug.Assert(lines.Length > 0);

            // Name	Sex	DateOfBirth	MarketValue
            string[] columnHeaders = lines[0].Split('\t');
            Debug.Assert(columnHeaders[0] == "Name");
            Debug.Assert(columnHeaders[1] == "Sex");
            Debug.Assert(columnHeaders[2] == "DateOfBirth");
            Debug.Assert(columnHeaders[3] == "MarketValue");

            List<Alpaca> alpacasToAdd = new List<Alpaca>();

            foreach (string line in lines.Skip(1))
            {
                string[] fields = line.Split('\t');
                alpacasToAdd.Add(new Alpaca()
                {
                    Id = Guid.NewGuid(),
                    Name = fields[0],
                    Sex = (Sex)Enum.Parse(typeof(Sex), fields[1]),
                    DateOfBirth = Convert.ToDateTime(fields[2]),
                    MarketValue = Convert.ToDecimal(fields[3])
                });
            }

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.Alpacas.AddRange(alpacasToAdd);
                dbContext.SaveChanges();
            }
        }

        public void ImportShearings()
        {
            Debug.Assert(File.Exists(ShearingsFile));

            string[] lines = File.ReadAllLines(ShearingsFile);
            Debug.Assert(lines.Length > 0);

            // Date	Alpaca	Yield
            string[] columnHeaders = lines[0].Split('\t');
            Debug.Assert(columnHeaders[0] == "Date");
            Debug.Assert(columnHeaders[1] == "Alpaca");
            Debug.Assert(columnHeaders[2] == "Yield");

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var shearingsImport =
                    from tokens in lines.Skip(1).Select(l => l.Split('\t'))
                    select new
                    {
                        Date = Convert.ToDateTime(tokens[0]),
                        AlpacaName = tokens[1],
                        Yield = Convert.ToInt32(tokens[2])
                    };

                IEnumerable<Shearing> shearingsWithLookup =
                    from shearing in shearingsImport
                    join alpaca in dbContext.Alpacas on shearing.AlpacaName equals alpaca.Name
                    select new Shearing()
                    {
                        Id = Guid.NewGuid(),
                        Alpaca = alpaca,
                        Date = shearing.Date,
                        Yield = shearing.Yield
                    };

                Shearing[] shearingsToAdd = shearingsWithLookup.ToArray();
                dbContext.AddRange(shearingsToAdd);
                dbContext.SaveChanges();
            }
        }
    }
}

