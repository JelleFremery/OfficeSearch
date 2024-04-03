using Azure.Search.Documents.Models;

namespace OfficeSearch.Models;

public abstract class SearchData<T> where T : class
{
    // The text to search for.
    public string searchText { get; set; }

    // The list of results.
    public SearchResults<T> resultList;
}
