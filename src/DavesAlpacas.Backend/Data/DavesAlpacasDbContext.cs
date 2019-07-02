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

using DavesAlpacas.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DavesAlpacas.Backend.Data
{
    /// <summary>
    /// The application's DB context
    /// </summary>
    internal class DavesAlpacasDbContext : DbContext
    {
        public virtual DbSet<Alpaca> Alpacas { get; set; }

        public virtual DbSet<Shearing> Shearings { get; set; }

        public DavesAlpacasDbContext(DbContextOptions<DavesAlpacasDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // require explicit setup.
                throw new InvalidOperationException("DB Context has not been configured. Check startup code.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Alpacas
            modelBuilder.Entity<Alpaca>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Alpaca>()
                .Property(a => a.Name)
                .IsRequired();

            modelBuilder.Entity<Alpaca>()
                .HasIndex(a => a.Name)
                .IsUnique();

            // Shearings
            modelBuilder.Entity<Shearing>()
                .HasKey(s => s.Id);
        }
    }
}

