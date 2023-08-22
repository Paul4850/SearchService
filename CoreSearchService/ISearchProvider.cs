using SearchAPI.Contracts;

namespace SearchAPI
{
    public interface ISearchProvider
    {
        Task<IEnumerable<SearchAPI.Contracts.Route>> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
    }
}
