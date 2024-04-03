using OfficeSearch.Models;

namespace OfficeSearch.Services.Querying
{
    public interface IQueryService
    {
        Task RunQueryAsync(OfficeSearchData model);
    }
}