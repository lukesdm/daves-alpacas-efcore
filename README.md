# Introduction 
This project accompanies the article '[TBD](https://TBD)' (currently WIP). It is used to demonstrate some techniques for working with Entity Framework Core (v2.1\*), namely using the .Net Core Logging API within integration tests, to get a handle on what EF Core is doing under the hood. 

# Dependencies
The main one:  

- .Net Core SDK 2.1\*  

And these nuget packages (plus a few others):  

- Microsoft.EntityFrameworkCore  
- Npgsql.EntityFramework.PostgreSQL\*\*  
- Microsoft.EntityFramework.Sqlite\*\*  
- xUnit  
- Microsoft.Logging.Abstractions  

The project was developed on Windows. It should work across the other OSs .Net Core supports, but hasn't been tested on them.  
  
\*It should work fine on .Net Core SDK 2.2 also, after just updating the project files.  
  
\*\*Only one of these is used at a time - choose a DB provider by editing the db.props file, in the solution folder.  
  
# Code Overview
There are three projects in the Solution.

### DavesAlpacas.Backend  
The System Under Test (SUT) - a basic set of EF Core entity models, and some services that interact with them.

### DavesAlpacas.Testing.Tools  
The custom logging tools used by the integration tests.

### DavesAlpacas.Backend.Tests  
Database integration tests.  

- *Infrastructure* namespace - useful common logic.  
  - *DbIntegrationTestBase*: Base class for database integration tests. Contains setup and cleanup code.
  - *TestContext*: Per-test state and operations, such as logging control and attaching output to test results.  
- *HerdServiceTests.cs*: A set of tests, organised to show a progression of problems encountered, and how to use the tools to solve them.  
- *ShearingStatsServiceTests.cs*: Some extra example tests.

# Build and Test
It should be possible to build and run the tests as usual from within Visual Studio, or on the command line with 'dotnet build' or 'dotnet test'. See also  DavesAlpacas.Backend.Tests\RunTests.ps1.

# Contribute
Corrections and suggestions are welcome - please create an Issue.

# License
The source code is licensed under the Mozilla Public License 2.0 (see LICENSE). In short, you can use the code for any purpose, but the source of any modified versions that are part of a publicly distributed work must be released under the same terms.
