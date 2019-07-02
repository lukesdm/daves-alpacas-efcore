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

namespace DavesAlpacas.Testing.Tools
{
    /// <summary>
    /// Correlation ID value object
    /// </summary>
    /// <remarks>As a struct, this allows convenient comparison. 
    /// Could use a more specific type for Value depending on implementation.</remarks>
    public struct CorrelationId
    {
        private object Value { get; }
        public CorrelationId(object value)
        {
            // Useful for debugging:
            // System.Diagnostics.Debug.WriteLine("CorrelationId: {0}", value?.GetHashCode());
            this.Value = value;
        }

        public bool HasValue()
        {
            return Value != null;
        }
    }
}

