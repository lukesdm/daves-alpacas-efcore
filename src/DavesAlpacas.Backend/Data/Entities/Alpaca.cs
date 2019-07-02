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
using System;

namespace DavesAlpacas.Backend.Data.Entities
{
    /// <summary>
    /// An alpaca, cutest of the camelids.
    /// </summary>
    internal class Alpaca
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The animal's given name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The sex (male or female)
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// The date of birth
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// The estimated market value, in euros
        /// </summary>
        public decimal MarketValue { get; set; }
    }
}

