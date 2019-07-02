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
using DavesAlpacas.Backend.Data.Entities;
using DavesAlpacas.Backend.Services;
using DavesAlpacas.Backend.Tests.Infrastructure;
using DavesAlpacas.Testing.Tools;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DavesAlpacas.Backend.Tests.Services
{
    /// <summary>
    /// Database integration tests for the <see cref="HerdService"/> class.
    /// </summary>
    [Collection(nameof(ServicesTestsCollection))]
    public class HerdServiceTests : DbIntegrationTestBase
    {
        /// <summary>
        /// Initializes an instance of the <see cref="HerdService"/> class - called each test by xUnit.
        /// </summary>
        /// <param name="testOutputHelper">xUnit test output helper - injected by xUnit</param>
        /// <param name="collectionFixture">Collection fixture - injected by xUnit</param>
        public HerdServiceTests(ITestOutputHelper testOutputHelper, ServicesTestsCollectionFixture collectionFixture)
            : base(testOutputHelper, collectionFixture)
        {
        }

        [Fact]
        public void CanInitializeService() // TODO: Ref
        {
            // Arrange
            HerdService herdService;

            // Act
            // ---------------------------------------------------------
            herdService = new HerdService(TestContext.DbContextFactory);
            // ---------------------------------------------------------

            // Assert
            Assert.NotNull(herdService);
        }

        // (sync, no collect)
        [Fact]
        public void CanAddAnAlpacaToHerd_v0() // TODO: REF
        {
            // Arrange
            var expectedAlpaca = new
            {
                Name = "Henry",
                Sex = Sex.Male,
                DateOfBirth = new DateTime(2007, 7, 8),
                MarketValue = 10000
            };
            var sut = new HerdService(TestContext.DbContextFactory);

            // Act
            // -------------------------------------------------------------------
            Guid alpacaId = sut.AddAlpaca(expectedAlpaca.Name, expectedAlpaca.Sex, 
                expectedAlpaca.DateOfBirth, expectedAlpaca.MarketValue);
            // -------------------------------------------------------------------

            // Assert
            Alpaca actualAlpaca;
            using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
            {
                actualAlpaca = dbContext.Alpacas.Find(alpacaId);
            }
            Assert.Equal(expectedAlpaca.Name, actualAlpaca.Name);
            Assert.Equal(expectedAlpaca.Sex, actualAlpaca.Sex);
            Assert.Equal(expectedAlpaca.DateOfBirth, actualAlpaca.DateOfBirth);
            Assert.Equal(expectedAlpaca.MarketValue, actualAlpaca.MarketValue);
        }

        // (sync, v0 collect, no check)
        [Fact]
        public void CanAddAnAlpacaToHerd_v1() // TODO: Ref
        {
            // Arrange
            var expectedAlpaca = new
            {
                Name = "Henry",
                Sex = Sex.Male,
                DateOfBirth = new DateTime(2007, 7, 8),
                MarketValue = 10000
            };
            var sut = new HerdService(TestContext.DbContextFactory);

            // Act
            Guid alpacaId = new Guid();
            var log = TestContext.Collect( () => 
            {
                // -------------------------------------------------------------------
                alpacaId = sut.AddAlpaca(expectedAlpaca.Name, expectedAlpaca.Sex,
                    expectedAlpaca.DateOfBirth, expectedAlpaca.MarketValue);
                // -------------------------------------------------------------------

                return LogCollectorProvider.GetCorrelationId();
            });
            TestContext.Attach(log);

            // Assert
            Alpaca actualAlpaca;
            using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
            {
                actualAlpaca = dbContext.Alpacas.Find(alpacaId);
            }
            Assert.Equal(expectedAlpaca.Name, actualAlpaca.Name);
            Assert.Equal(expectedAlpaca.Sex, actualAlpaca.Sex);
            Assert.Equal(expectedAlpaca.DateOfBirth, actualAlpaca.DateOfBirth);
            Assert.Equal(expectedAlpaca.MarketValue, actualAlpaca.MarketValue);
        }

        // (sync, v0 collect, no check)
        [Fact]
        public void CanCalculateHerdValue_v1() // TODO: REF
        {
            // Arrange
            decimal expectedTotal = 43300;
            decimal? actualTotal = null;
            HerdService sut = new HerdService(TestContext.DbContextFactory);

            // Act
            var log = TestContext.Collect(() =>
            {
                // -------------------------------------
                actualTotal = sut.CalculateTotalValue();
                // -------------------------------------
                return LogCollectorProvider.GetCorrelationId();
            });
            TestContext.Attach(log);

            // Assert
            Assert.Equal(expectedTotal, actualTotal);
        }

        // (sync, v0 collect, no check)
        [Fact]
        public void CanAddMultipleAlpacasToHerd_v1() // TODO: Ref
        {
            // Arrange
            var expectedAlpacas = new[] {
                new { Name = "Henry", Sex = Sex.Male, DateOfBirth = new DateTime(2007, 7, 8), MarketValue = 10000 }
                ,new { Name = "Henrietta", Sex = Sex.Female, DateOfBirth = new DateTime(2007, 11, 13), MarketValue = 8000 }
                ,new { Name = "Jo", Sex = Sex.Female, DateOfBirth = new DateTime(2008, 1, 8), MarketValue = 7000 }
            };
            var sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperationCount = 3;

            // Act
            Guid[] alpacaIds = new Guid[3];
            var log = TestContext.Collect(() =>
            {
                // --------------------------------------------------------------------
                alpacaIds[0] = sut.AddAlpaca(expectedAlpacas[0].Name, expectedAlpacas[0].Sex,
                    expectedAlpacas[0].DateOfBirth, expectedAlpacas[0].MarketValue);
                alpacaIds[1] = sut.AddAlpaca(expectedAlpacas[1].Name, expectedAlpacas[1].Sex,
                    expectedAlpacas[1].DateOfBirth, expectedAlpacas[1].MarketValue);
                alpacaIds[2] = sut.AddAlpaca(expectedAlpacas[2].Name, expectedAlpacas[2].Sex,
                    expectedAlpacas[2].DateOfBirth, expectedAlpacas[2].MarketValue);
                // --------------------------------------------------------------------

                return LogCollectorProvider.GetCorrelationId();
            });
            TestContext.Attach(log);

            // Assert
            for (int i = 0; i < expectedAlpacas.Length; i++)
            {
                Alpaca actualAlpaca;
                using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
                {
                    actualAlpaca = dbContext.Alpacas.Find(alpacaIds[i]);
                }
                Assert.Equal(expectedAlpacas[i].Name, actualAlpaca.Name);
                Assert.Equal(expectedAlpacas[i].Sex, actualAlpaca.Sex);
                Assert.Equal(expectedAlpacas[i].DateOfBirth, actualAlpaca.DateOfBirth);
                Assert.Equal(expectedAlpacas[i].MarketValue, actualAlpaca.MarketValue);
            }
        }

        // (sync, v0 collect, with check) TODO: Ref
        [Fact]
        public void CanAddMultipleAlpacasToHerd_v2()
        {
            // Arrange
            var expectedAlpacas = new[] {
                new { Name = "Henry", Sex = Sex.Male, DateOfBirth = new DateTime(2007, 7, 8), MarketValue = 10000 }
                ,new { Name = "Henrietta", Sex = Sex.Female, DateOfBirth = new DateTime(2007, 11, 13), MarketValue = 8000 }
                ,new { Name = "Jo", Sex = Sex.Female, DateOfBirth = new DateTime(2008, 1, 8), MarketValue = 7000 }
            };
            var sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperationCount = 3;

            // Act
            Guid[] alpacaIds = new Guid[3];
            var log = TestContext.Collect(() =>
            {
                // --------------------------------------------------------------------
                alpacaIds[0] = sut.AddAlpaca(expectedAlpacas[0].Name, expectedAlpacas[0].Sex,
                    expectedAlpacas[0].DateOfBirth, expectedAlpacas[0].MarketValue);
                alpacaIds[1] = sut.AddAlpaca(expectedAlpacas[1].Name, expectedAlpacas[1].Sex,
                    expectedAlpacas[1].DateOfBirth, expectedAlpacas[1].MarketValue);
                alpacaIds[2] = sut.AddAlpaca(expectedAlpacas[2].Name, expectedAlpacas[2].Sex,
                    expectedAlpacas[2].DateOfBirth, expectedAlpacas[2].MarketValue);
                // --------------------------------------------------------------------

                return LogCollectorProvider.GetCorrelationId();
            });
            TestContext.Attach(log);

            // Assert
            for (int i = 0; i < expectedAlpacas.Length; i++)
            {
                Alpaca actualAlpaca;
                using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
                {
                    actualAlpaca = dbContext.Alpacas.Find(alpacaIds[i]);
                }
                Assert.Equal(expectedAlpacas[i].Name, actualAlpaca.Name);
                Assert.Equal(expectedAlpacas[i].Sex, actualAlpaca.Sex);
                Assert.Equal(expectedAlpacas[i].DateOfBirth, actualAlpaca.DateOfBirth);
                Assert.Equal(expectedAlpacas[i].MarketValue, actualAlpaca.MarketValue);
            }

            Assert.Equal(expectedDbOperationCount, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }

        // (sync, collect v1, with check)
        [Fact]
        public void CanCalculateHerdValue_v2() // TODO: REF
        {
            // Arrange
            decimal expectedTotal = 43300;
            decimal actualTotal;
            HerdService sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperations = 1;

            // Act
            TestContext.StartCollect();
            // -------------------------------------
            actualTotal = sut.CalculateTotalValue();
            // -------------------------------------
            var log = TestContext.EndCollect();
            TestContext.Attach(log);

            // Assert
            Assert.Equal(expectedTotal, actualTotal);
            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }

        // (async, v0 collect, with check)
        [Fact]
        public void CanAddAnAlpacaToHerdAsync_v1()
        {
            // Arrange
            var expectedAlpaca = new
            {
                Name = "Henry",
                Sex = Sex.Male,
                DateOfBirth = new DateTime(2007, 7, 8),
                MarketValue = 10000
            };
            var sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperations = 1;

            // Act
            Guid alpacaId = new Guid();
            var log = TestContext.Collect(() =>
            {
                // -------------------------------------------------------------------
                alpacaId = sut.AddAlpacaAsync(expectedAlpaca.Name, expectedAlpaca.Sex,
                    expectedAlpaca.DateOfBirth, expectedAlpaca.MarketValue)
                    .GetAwaiter().GetResult();
                // -------------------------------------------------------------------

                return LogCollectorProvider.GetCorrelationId();
            });
            TestContext.Attach(log);

            // Assert
            Alpaca actualAlpaca;
            using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
            {
                actualAlpaca = dbContext.Alpacas.Find(alpacaId);
            }
            Assert.Equal(expectedAlpaca.Name, actualAlpaca.Name);
            Assert.Equal(expectedAlpaca.Sex, actualAlpaca.Sex);
            Assert.Equal(expectedAlpaca.DateOfBirth, actualAlpaca.DateOfBirth);
            Assert.Equal(expectedAlpaca.MarketValue, actualAlpaca.MarketValue);

            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }

        // (async, collect v0, with check, FAIL)
        //[Fact(Skip = "Expected to fail - operations not captured properly.")]
        // NOTE: Only fails with Postgres. Async interactions with SQLite connections are different.
        // Also - may not fail when run on its own.
        [Fact]
        public void CanCalculateHerdValueAsync_v1() // TODO: REF
        {
            // Arrange
            decimal expectedTotal = 43300;
            decimal? actualTotal = null;
            HerdService sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperations = 1;

            // Act
            var log = TestContext.Collect(() =>
            {
                // -----------------------------------------
                actualTotal = sut.CalculateTotalValueAsync()
                    .GetAwaiter().GetResult();
                // -----------------------------------------
                return LogCollectorProvider.GetCorrelationId();
            });
            TestContext.Attach(log);

            // Assert
            Assert.Equal(expectedTotal, actualTotal);
            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand)); // Expected to fail - operations not captured properly.
        }

        // (async, v0 collect, check, FAIL)
        //[Fact(Skip = "Expected to fail - operations not captured properly.")]
        // // NOTE: Only fails with Postgres. Async interactions with SQLite connections are different.
        [Fact]
        public void CanAddMultipleAlpacasToHerdAsync_v1() // TODO: REF
        {
            // Arrange
            var expectedAlpacas = new[] {
                new { Name = "Henry", Sex = Sex.Male, DateOfBirth = new DateTime(2007, 7, 8), MarketValue = 10000 }
                ,new { Name = "Henrietta", Sex = Sex.Female, DateOfBirth = new DateTime(2007, 11, 13), MarketValue = 8000 }
                ,new { Name = "Jo", Sex = Sex.Female, DateOfBirth = new DateTime(2008, 1, 8), MarketValue = 7000 }
            };
            var sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperationCount = 3;

            // Act
            Guid[] alpacaIds = new Guid[0];
            var log = TestContext.Collect(() =>
            {
                // --------------------------------------------------------------------
                alpacaIds = Task.WhenAll(
                    sut.AddAlpacaAsync(expectedAlpacas[0].Name, expectedAlpacas[0].Sex,
                      expectedAlpacas[0].DateOfBirth, expectedAlpacas[0].MarketValue),
                    sut.AddAlpacaAsync(expectedAlpacas[1].Name, expectedAlpacas[1].Sex,
                      expectedAlpacas[1].DateOfBirth, expectedAlpacas[1].MarketValue),
                    sut.AddAlpacaAsync(expectedAlpacas[2].Name, expectedAlpacas[2].Sex,
                      expectedAlpacas[2].DateOfBirth, expectedAlpacas[2].MarketValue)
                    ).GetAwaiter().GetResult();
                // --------------------------------------------------------------------

                return LogCollectorProvider.GetCorrelationId();
            });
            TestContext.Attach(log);

            // Assert
            for (int i = 0; i < expectedAlpacas.Length; i++)
            {
                Alpaca actualAlpaca;
                using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
                {
                    actualAlpaca = dbContext.Alpacas.Find(alpacaIds[i]);
                }
                Assert.Equal(expectedAlpacas[i].Name, actualAlpaca.Name);
                Assert.Equal(expectedAlpacas[i].Sex, actualAlpaca.Sex);
                Assert.Equal(expectedAlpacas[i].DateOfBirth, actualAlpaca.DateOfBirth);
                Assert.Equal(expectedAlpacas[i].MarketValue, actualAlpaca.MarketValue);
            }

            Assert.Equal(expectedDbOperationCount, GetLogEntryCount(log, LogCategories.EfDbCommand)); // Expected to fail - operations not captured properly.
        }

        // (async, v1 collect, with check)
        [Fact]
        public void CanAddAnAlpacaToHerdAsync_v2() // TODO: REF
        {
            // Arrange
            var expectedAlpaca = new
            {
                Name = "Henry",
                Sex = Sex.Male,
                DateOfBirth = new DateTime(2007, 7, 8),
                MarketValue = 10000
            };
            var sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperations = 1;

            // Act
            TestContext.StartCollect();
            // ------------------------------------------------------------------------
            Guid alpacaId = sut.AddAlpacaAsync(expectedAlpaca.Name, expectedAlpaca.Sex, 
                expectedAlpaca.DateOfBirth, expectedAlpaca.MarketValue)
                .GetAwaiter().GetResult();
            // ------------------------------------------------------------------------
            var log = TestContext.EndCollect();
            TestContext.Attach(log);

            // Assert
            Alpaca actualAlpaca;
            using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
            {
                actualAlpaca = dbContext.Alpacas.Find(alpacaId);
            }
            Assert.Equal(expectedAlpaca.Name, actualAlpaca.Name);
            Assert.Equal(expectedAlpaca.Sex, actualAlpaca.Sex);
            Assert.Equal(expectedAlpaca.DateOfBirth, actualAlpaca.DateOfBirth);
            Assert.Equal(expectedAlpaca.MarketValue, actualAlpaca.MarketValue);

            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }

        // (async, collect v1, with check)
        [Fact]
        public void CanCalculateHerdValueAsync_v2() // TODO: REF
        {
            // Arrange
            decimal expectedTotal = 43300;
            decimal actualTotal;
            HerdService sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperations = 1;

            // Act
            TestContext.StartCollect();
            // -----------------------------------------
            actualTotal = sut.CalculateTotalValueAsync()
                .GetAwaiter().GetResult();
            // -----------------------------------------
            var log = TestContext.EndCollect();
            TestContext.Attach(log);

            // Assert
            Assert.Equal(expectedTotal, actualTotal);
            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }

        // (async, collect v1, with check)
        [Fact]
        public void CanAddMultipleAlpacasToHerdAsync_v2() // TODO: REF
        {
            // Arrange
            var expectedAlpacas = new[] {
                new { Name = "Henry", Sex = Sex.Male, DateOfBirth = new DateTime(2007, 7, 8), MarketValue = 10000 }
                ,new { Name = "Henrietta", Sex = Sex.Female, DateOfBirth = new DateTime(2007, 11, 13), MarketValue = 8000 }
                ,new { Name = "Jo", Sex = Sex.Female, DateOfBirth = new DateTime(2008, 1, 8), MarketValue = 7000 }
            };
            var sut = new HerdService(TestContext.DbContextFactory);
            const int expectedDbOperationCount = 3;

            // Act
            TestContext.StartCollect();
            // ---------------------------------------------------------------------
            Guid[] alpacaIds = Task.WhenAll(
                sut.AddAlpacaAsync(expectedAlpacas[0].Name, expectedAlpacas[0].Sex, 
                    expectedAlpacas[0].DateOfBirth, expectedAlpacas[0].MarketValue), 
                sut.AddAlpacaAsync(expectedAlpacas[1].Name, expectedAlpacas[1].Sex, 
                    expectedAlpacas[1].DateOfBirth, expectedAlpacas[1].MarketValue), 
                sut.AddAlpacaAsync(expectedAlpacas[2].Name, expectedAlpacas[2].Sex, 
                    expectedAlpacas[2].DateOfBirth, expectedAlpacas[2].MarketValue)
                ).GetAwaiter().GetResult();
            // --------------------------------------------------------------------
            var log = TestContext.EndCollect();
            TestContext.Attach(log);

            // Assert
            for (int i = 0; i < expectedAlpacas.Length; i++)
            {
                Alpaca actualAlpaca;
                using (var dbContext = TestContext.DbContextFactory.CreateDbContext())
                {
                    actualAlpaca = dbContext.Alpacas.Find(alpacaIds[i]);
                }
                Assert.Equal(expectedAlpacas[i].Name, actualAlpaca.Name);
                Assert.Equal(expectedAlpacas[i].Sex, actualAlpaca.Sex);
                Assert.Equal(expectedAlpacas[i].DateOfBirth, actualAlpaca.DateOfBirth);
                Assert.Equal(expectedAlpacas[i].MarketValue, actualAlpaca.MarketValue);
            }
            Assert.Equal(expectedDbOperationCount, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }
    }
}

