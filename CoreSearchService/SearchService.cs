using SearchAPI.Contracts;
using System;
using System.Net.Http;

namespace SearchAPI
{
    public class SearchService : ISearchService
    {
        IEnumerable<ISearchProvider> providers;

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            var tasks = providers.Select(p => p.IsAvailableAsync(cancellationToken));
            var resTasks = Task.WhenAny(tasks);
            var res = await resTasks;
            return await res;
        }

        public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var tasks = providers.Select(p => p.SearchAsync(request, cancellationToken));
            var aggregatedTasks = Task.WhenAll(tasks);
            var routs = await aggregatedTasks;
            var rootsMerged = routs.SelectMany(x =>  x).ToList();

            var response = new SearchResponse() { Routes = rootsMerged.ToArray() };
            if (rootsMerged.Any())
            {
                response.MinPrice = rootsMerged.Min(r => r.Price);
                response.MaxPrice = rootsMerged.Max(r => r.Price);
                response.MinMinutesRoute = (int)rootsMerged.Min(r => r.DestinationDateTime.Subtract(r.OriginDateTime).TotalMinutes);
                response.MaxMinutesRoute = (int)rootsMerged.Max(r => r.DestinationDateTime.Subtract(r.OriginDateTime).TotalMinutes);
            }
            return response;
        }

        public SearchService(IEnumerable<ISearchProvider> providers)
        {
            if (providers == null)
                throw new ArgumentException(nameof(providers));
            this.providers = providers;
        }
    }

}
