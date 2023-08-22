using SearchAPI;
using SearchAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProviderOne.Provider1;
using System.Net.Http.Json;

namespace ProviderOne.Provider2
{
    public class SearchProvider2 : ISearchProvider, IDisposable
    {
        HttpClient httpClient = new HttpClient();

        string uri = "";
        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            try
            {
                response = await httpClient.GetAsync(uri, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                // log httpClient error
            }
            if (response?.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            return false;
        }

        public void Dispose()
        {
            httpClient.Dispose();
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
            ProviderTwoSearchResponse? innerResponse = null;
            try
            {
                httpResponse = await httpClient.PostAsJsonAsync(uri, innerRequest, cancellationToken);
                innerResponse = await httpResponse.Content.ReadFromJsonAsync<ProviderTwoSearchResponse>();
            }
            catch (TaskCanceledException ex)
            {
                // log httpClient error
            }

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

        public SearchProvider2(string uri)
        {
            this.uri = uri;
        }
    }
}
