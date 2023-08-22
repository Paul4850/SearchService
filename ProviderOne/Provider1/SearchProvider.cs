using SearchAPI;
using SearchAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using ProviderOne.Provider2;

namespace ProviderOne.Provider1
{
    public class SearchProvider1 : ISearchProvider, IDisposable
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

        public async Task<IEnumerable<SearchAPI.Contracts.Route>> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            ProviderOneSearchRequest innerRequest = new ProviderOneSearchRequest();
            innerRequest.From = request.Origin;
            innerRequest.To = request.Destination;
            innerRequest.DateFrom = request.OriginDateTime;

            innerRequest.DateTo = request.Filters?.DestinationDateTime;
            innerRequest.MaxPrice = request.Filters?.MaxPrice;
            
            HttpResponseMessage httpResponse;
            ProviderOneSearchResponse? innerResponse = null;
            try
            {
                httpResponse = await httpClient.PostAsJsonAsync(uri, innerRequest, cancellationToken);
                innerResponse = await httpResponse.Content.ReadFromJsonAsync<ProviderOneSearchResponse>();
            }
            catch (TaskCanceledException ex)
            {
                // log httpClient error
            }
            
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

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public SearchProvider1(string uri)
        {
            this.uri = uri + "/search";
        }
    }
}
