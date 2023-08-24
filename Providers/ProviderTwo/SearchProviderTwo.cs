using SearchAPI;
using SearchAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProviderOne.ProviderOne;
using System.Net.Http.Json;
using System.Net.Http;

namespace ProviderOne.ProviderTwo
{
    public class SearchProviderTwo : ISearchProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;

        string uri = "";
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

        public async Task<IEnumerable<Route>> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            //await Task.Delay(3000);
            //return new Route[] { };
            
            ProviderTwoSearchRequest innerRequest = new ProviderTwoSearchRequest();
            innerRequest.Departure = request.Origin;
            innerRequest.Arrival = request.Destination;
            innerRequest.DepartureDate = request.OriginDateTime;

            innerRequest.MinTimeLimit = request.Filters?.MinTimeLimit;

            HttpResponseMessage httpResponse;
            var httpClient = _httpClientFactory.CreateClient();
            //TODO: handle client errors by a retry policy
            httpResponse = await httpClient.PostAsJsonAsync(uri, innerRequest, cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
                return new List<Route>();
            var innerResponse = await httpResponse.Content.ReadFromJsonAsync<ProviderTwoSearchResponse>();

            var response = innerResponse?.Routes.Select(
                    r => new Route()
                    {
                        Origin = r.Departure.Point,
                        Destination = r.Arrival.Point,
                        OriginDateTime = r.Departure.Date,
                        DestinationDateTime = r.Arrival.Date,
                        Price = r.Price,
                        TimeLimit = r.TimeLimit
                    }
                );

            if (response == null)
                response = new List<Route>();
            return response;
        }

        public SearchProviderTwo(IHttpClientFactory httpClientFactory, string uri)
        {
            this.uri = uri;
            _httpClientFactory = httpClientFactory;
        }
    }
}
