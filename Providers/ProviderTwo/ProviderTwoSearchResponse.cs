using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderOne.ProviderTwo
{
#pragma warning disable CS8618
    public class ProviderTwoSearchResponse
    {
        // Mandatory
        // Array of routes
        public ProviderTwoRoute[] Routes { get; set; }
    }
}
