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

namespace DavesAlpacas.Testing.Tools
{
    /// <summary>
    /// Wrapper for the logger categories of interest
    /// </summary>
    /// <remarks>
    /// These are the logger categories you may see while debugging the tests. 
    /// They are exposed as a hierarchy by the EF class <see cref="DbLoggerCategory"/>
    /// This class exposes them in a way that is slightly more convenient for our purposes.
    /// "Microsoft.EntityFrameworkCore.Update"
    /// "Microsoft.EntityFrameworkCore.Database.Command"
    /// "Microsoft.EntityFrameworkCore.Query"
    /// "Microsoft.EntityFrameworkCore.Infrastructure"
    /// "Microsoft.EntityFrameworkCore.Database.Connection"
    /// "Microsoft.EntityFrameworkCore.Database.Transaction"
    /// "Microsoft.EntityFrameworkCore.Model.Validation"
    /// "Microsoft.EntityFrameworkCore.Model"
    /// "Microsoft.EntityFrameworkCore.ChangeTracking"
    /// 
    /// Could add other categories from outside of EF here also.
    /// </remarks>
    public static class LogCategories
    {
        /// <summary>
        /// Top level Entity Framework Core category - captures all Entity Framework Core messages
        /// </summary>
        public static string Ef { get => DbLoggerCategory.Name; } // "Microsoft.EntityFrameworkCore"

        /// <summary>
        /// Captures query text and execution time
        /// </summary>
        public static string EfDbCommand { get => DbLoggerCategory.Database.Command.Name; } //"Microsoft.EntityFrameworkCore.Database.Command";
    }
}

