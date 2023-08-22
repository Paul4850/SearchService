using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProviderOne.Provider1
{
    public class ProviderOneSearchResponse
    {
        // Mandatory
        // Array of routes
        [JsonPropertyName("routes")]
        public ProviderOneRoute[] Routes { get; set; }
    }
}
