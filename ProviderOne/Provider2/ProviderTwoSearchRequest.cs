using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderOne.Provider2
{
    public class ProviderTwoSearchRequest
    {
        // Mandatory
        // Start point of route, e.g. Moscow 
        public string Departure { get; set; }

        // Mandatory
        // End point of route, e.g. Sochi
        public string Arrival { get; set; }

        // Mandatory
        // Start date of route
        public DateTime DepartureDate { get; set; }

        // Optional
        // Minimum value of timelimit for route
        public DateTime? MinTimeLimit { get; set; }
    }
}
