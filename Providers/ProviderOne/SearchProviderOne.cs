using SearchAPI;
using SearchAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using ProviderOne.ProviderTwo;

namespace ProviderOne.ProviderOne
{
    public class SearchProviderOne : ISearchProvider
    {
        string uri = "";
        private readonly IHttpClientFactory _httpClientFactory;

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage? response = null;
            //TODO: handle client errors by a retry policy
            var httpClient = _httpClientFactory.CreateClient();
            response = await httpClient.GetAsync(uri, cancellationToken);
            
            if (response?.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            return false;
        }

        public async Task<IEnumerable<SearchAPI.Contracts.Route>> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            ProviderOneSearchRequest innerRequest = new ProviderOneSearchRequest();
            innerRequest.From = request.Origin;
            innerRequest.To = request.Destination;
            innerRequest.DateFrom = request.OriginDateTime;

            innerRequest.DateTo = request.Filters?.DestinationDateTime;
            innerRequest.MaxPrice = request.Filters?.MaxPrice;
            
            HttpResponseMessage httpResponse;
            
            var httpClient = _httpClientFactory.CreateClient();
            
            //TODO: handle client errors by a retry policy
            httpResponse = await httpClient.PostAsJsonAsync(uri, innerRequest, cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
                return new List<Route>();
            var innerResponse = await httpResponse.Content.ReadFromJsonAsync<ProviderOneSearchResponse>();
            
            var response = innerResponse?.Routes.Select(
                    r => new Route()
                    { 
                          Origin = r.From,
                          Destination = r.To,
                          OriginDateTime = r.DateFrom,
                          DestinationDateTime = r.DateTo,
                            Price = r.Price,
                            TimeLimit = r.TimeLimit
                    }
                );

            if(response ==  null)
                response = new List<Route>();
            return response;
        }

        public SearchProviderOne(IHttpClientFactory httpClientFactory, string uri)
        {
            _httpClientFactory = httpClientFactory;
            this.uri = uri + "/search";
        }
    }
}
