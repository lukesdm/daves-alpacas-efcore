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

using System;

namespace DavesAlpacas.Backend.Data.Entities
{
    /// <summary>
    /// An instance of an alpaca being sheared
    /// </summary>
    internal class Shearing
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The animal
        /// </summary>
        public Alpaca Alpaca { get; set; }

        /// <summary>
        /// The date the animal was sheared.
        /// </summary>
        /// <remarks>Shearing takes part through the month of July, each year.</remarks>
        public DateTime Date { get; set; }

        /// <summary>
        /// The amount of fleece yielded, in grams
        /// </summary>
        public int Yield { get; set; }
    }
}

