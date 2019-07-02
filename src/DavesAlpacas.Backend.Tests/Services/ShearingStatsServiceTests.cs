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

using DavesAlpacas.Backend.Services;
using DavesAlpacas.Backend.Tests.Infrastructure;
using DavesAlpacas.Testing.Tools;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DavesAlpacas.Backend.Tests.Services
{
    /// <summary>
    /// Database integration tests for the <see cref="ShearingStatsService"/> class.
    /// </summary>
    /// <remarks>Uses data from  Alpacas.txt and Shearings.txt imported during test setup - see base class initialization</remarks>
    [Collection(nameof(ServicesTestsCollection))]
    public class ShearingStatsServiceTests : DbIntegrationTestBase
    {
        /// <summary>
        /// Initializes an instance of the <see cref="ShearingStatsServiceTests"/> class - called each test by xUnit.
        /// </summary>
        /// <param name="testOutputHelper">xUnit test output helper - injected by xUnit</param>
        /// <param name="collectionFixture">Collection fixture - injected by xUnit</param>
        public ShearingStatsServiceTests(ITestOutputHelper testOutputHelper, ServicesTestsCollectionFixture collectionFixture)
            : base(testOutputHelper, collectionFixture)
        {
        }

        [Fact]
        public void CanInitializeService()
        {
            // Arrange
            ShearingStatsService service;

            // Act
            // --------------------------------------------------------------
            service = new ShearingStatsService(TestContext.DbContextFactory);
            // --------------------------------------------------------------

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void CanCalculateTop3AllTime()
        {
            // Arrange
            var expectedResults = new[] {
                new { AlpacaName = "Pablo", Date = new DateTime(2014, 7, 11), Yield = 2488 },
                new { AlpacaName = "Juanita", Date = new DateTime(2009, 7, 19), Yield = 2487 },
                new { AlpacaName = "Gertrude", Date = new DateTime(2014, 7, 13), Yield = 2486 },
            };
            const int expectedDbOperations = 1;

            var statsService = new ShearingStatsService(TestContext.DbContextFactory);
            
            // Act
            TestContext.StartCollect();
            // -----------------------------------------------
            var actualResults = statsService.GetTop3AllTime();
            // -----------------------------------------------
            var log = TestContext.EndCollect();
            TestContext.Attach(log);

            // Assert
            Assert.Equal(3, actualResults.Length);
            for (int i = 0; i < expectedResults.Length; i++)
            {
                Assert.Equal(expectedResults[i].AlpacaName, actualResults[i].AlpacaName);
                Assert.Equal(expectedResults[i].Date, actualResults[i].Date);
                Assert.Equal(expectedResults[i].Yield, actualResults[i].Yield);
            }
            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }

        [Fact]
        public void CanCalculateTop3AllTimeAsync()
        {
            // Arrange
            var expectedResults = new[] {
                new { AlpacaName = "Pablo", Date = new DateTime(2014, 7, 11), Yield = 2488 },
                new { AlpacaName = "Juanita", Date = new DateTime(2009, 7, 19), Yield = 2487 },
                new { AlpacaName = "Gertrude", Date = new DateTime(2014, 7, 13), Yield = 2486 },
            };
            const int expectedDbOperations = 1;
            var statsService = new ShearingStatsService(TestContext.DbContextFactory);

            // Act
            TestContext.StartCollect();
            // ----------------------------------------------------------------------------------------
            ShearingStatsService.TopShearingResult[] actualResults = statsService.GetTop3AllTimeAsync()
                .GetAwaiter().GetResult();
            // ----------------------------------------------------------------------------------------
            var log = TestContext.EndCollect();
            TestContext.Attach(log);

            // Assert
            Assert.Equal(3, actualResults.Length);
            for (int i = 0; i < expectedResults.Length; i++)
            {
                Assert.Equal(expectedResults[i].AlpacaName, actualResults[i].AlpacaName);
                Assert.Equal(expectedResults[i].Date, actualResults[i].Date);
                Assert.Equal(expectedResults[i].Yield, actualResults[i].Yield);
            }
            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }

        [Theory]
        [InlineData(2012, new string[] { "Cindy", "Juanita", "Leeroy" })]
        [InlineData(2001, new string[] { })] // edge case - no shearings for that year
        public void CanCalculateTop3ForYear(int year, string[] expected)
        {
            // Arrange
            ShearingStatsService sut = new ShearingStatsService(TestContext.DbContextFactory);
            const int expectedDbOperations = 1;

            // Act
            TestContext.StartCollect();
            // -------------------------------------------------------------------------------
            ShearingStatsService.TopShearingResult[] actualResults = sut.GetTop3ForYear(year);
            // -------------------------------------------------------------------------------
            var log = TestContext.EndCollect();
            TestContext.Attach(log);

            // Assert
            Assert.Equal(expected, actualResults.Select(a => a.AlpacaName).ToArray());
            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }

        [Theory]
        [InlineData(2012, new string[] { "Cindy", "Juanita", "Leeroy" })]
        [InlineData(2001, new string[] { })] // edge case - no shearings for that year
        public void CanCalculateTop3ForYearAsync(int year, string[] expected)
        {
            // Arrange
            ShearingStatsService sut = new ShearingStatsService(TestContext.DbContextFactory);
            const int expectedDbOperations = 1;

            // Act
            TestContext.StartCollect();
            // -----------------------------------------------------------------------------------
            ShearingStatsService.TopShearingResult[] actualResults = sut.GetTop3ForYearAsync(year)
                .GetAwaiter().GetResult();
            // -----------------------------------------------------------------------------------
            var log = TestContext.EndCollect();
            TestContext.Attach(log);

            // Assert
            Assert.Equal(expected, actualResults.Select(a => a.AlpacaName).ToArray());
            Assert.Equal(expectedDbOperations, GetLogEntryCount(log, LogCategories.EfDbCommand));
        }
    }
}

