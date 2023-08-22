using SearchAPI.Contracts;
using System;
using System.Net.Http;

namespace SearchAPI
{
    public class SearchService : ISearchService
    {
        IEnumerable<ISearchProvider> providers;
        int timeout = 1000;

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            var tasks = providers.Select(p => p.IsAvailableAsync(cancellationToken));
            var resTasks = Task.WhenAny(tasks);
            var res = await resTasks;
            return await res;
        }

        List<Route> routesCache = new List<Route>();

        public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var tasks = providers.Select(p => p.SearchAsync(request, cancellationToken));
            var aggregatedTasks = Task.WhenAll(tasks);
            var routs = await aggregatedTasks;
            var rootsMerged = routs.SelectMany(x =>  x).ToList();

            routesCache.AddRange(rootsMerged);
            var response = new SearchResponse() { Routes = rootsMerged.ToArray() };
            response.MinPrice = rootsMerged.Min(r => r.Price);
            response.MaxPrice = rootsMerged.Max(r => r.Price);
            response.MinMinutesRoute = (int)rootsMerged.Min(r => r.DestinationDateTime.Subtract(r.OriginDateTime).TotalMinutes);
            response.MaxMinutesRoute = (int)rootsMerged.Max(r => r.DestinationDateTime.Subtract(r.OriginDateTime).TotalMinutes);
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
