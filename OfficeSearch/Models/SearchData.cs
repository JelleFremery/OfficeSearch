using Azure.Search.Documents.Models;

namespace OfficeSearch.Models;

public abstract class SearchData<T> where T : class
{
    // The text to search for.
    public string? SearchText { get; set; }

    public void SetSearchResults(SearchResults<T> results)
    {
        _resultList = results;
    }

    public SearchResults<T>? GetSearchResults()
    {
        return _resultList;
    }

    // The list of results.
    private SearchResults<T>? _resultList;
}
