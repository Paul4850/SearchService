using SearchAPI.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Schema;
using SearchAPI.Controllers;

namespace SearchAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IConfiguration _configuration;
        private IMemoryCache _cache;

        private readonly ISearchService searchService;
        public SearchController(ILogger<SearchController> logger, 
                        ISearchService searchService,
                        IMemoryCache cache,
                        IConfiguration configuration)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        [HttpGet]
        [Route("ping")]
        public async Task<ActionResult> GetPing()
        {
            int pingTimeout = _configuration.GetValue<int>("Search:PingTimeout");
            _configuration.GetSection("").Get(typeof(int));
            var isAvailable = false;
            try
            {
                var s_cts = new CancellationTokenSource();
                s_cts.CancelAfter(pingTimeout);
                isAvailable = await searchService.IsAvailableAsync(s_cts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.Log( LogLevel.Information, "\nTasks cancelled: timed out.\n");
            }
            if (isAvailable)
                return await Task.FromResult(Ok());
            else
                return await Task.FromResult(StatusCode(500));
        }
        Guid GenerateGuid(string origin, string destination, DateTime originDateTime, SearchFilters? filters
            )
        {
            GuidUnion guidUnion = new GuidUnion();  
            guidUnion.Hash1 = origin.GetHashCode();
            guidUnion.Hash2 = originDateTime.GetHashCode();
            guidUnion.Hash3 = destination.GetHashCode();
            guidUnion.Hash4 = (filters?.DestinationDateTime, filters?.MinTimeLimit, filters?.MaxPrice).GetHashCode();
            return guidUnion.Guid;
        }

        [HttpPost]
        [Route("search")]
        public async Task<SearchResponse> PostRouts(SearchRequest request)
        {
            int cacheExpiration = 5;
            SearchResponse res = new SearchResponse();
            Guid requestKey = GenerateGuid(request.Origin, request.Destination, request.OriginDateTime, request.Filters);

            if (_cache.TryGetValue(requestKey, out IEnumerable<SearchAPI.Contracts.Route> routes))
            {
                var filteredRoutes = routes.Where(
                r =>
                r.Price <= request.Filters?.MaxPrice
                && r.DestinationDateTime == request.Filters.DestinationDateTime
                && r.TimeLimit >= request.Filters.MinTimeLimit
                );
                res.Routes = routes.ToArray();
                return res;
            }
            else
            {
                if (request?.Filters?.OnlyCached == true)
                {
                    return res;
                }
                else
                {
                    int pingTimeout = _configuration.GetValue<int>("Search:PingTimeout");

                    try
                    {
                        var s_cts = new CancellationTokenSource();
                        s_cts.CancelAfter(pingTimeout);
                        res = await searchService.SearchAsync(request, s_cts.Token);
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                       .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheExpiration));
                        res.Routes.ToList().ForEach(r => r.Id = requestKey);
                        _cache.Set(requestKey, res.Routes, options: cacheEntryOptions);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.Log(LogLevel.Information, "\nTasks cancelled: timed out.\n");
                    }
                }
            }

            return res;
        }
    }
}