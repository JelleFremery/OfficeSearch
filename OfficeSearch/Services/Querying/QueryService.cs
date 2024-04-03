using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using OfficeSearch.Models;

namespace OfficeSearch.Services.Querying;

public class QueryService
{
    private const UseCase USECASE = UseCase.SimpleSearch;

    private readonly SearchClient _searchClient;

    public QueryService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string? searchServiceUri = configuration.GetValue<string>("SearchServiceUri");
        string? queryApiKey = configuration.GetValue<string>("SearchServiceQueryApiKey");
        string? indexName = configuration.GetValue<string>("IndexName");

        ArgumentException.ThrowIfNullOrWhiteSpace(searchServiceUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(queryApiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        // Create a service and index client.
        var uri = new Uri(searchServiceUri);
        var indexClient = new SearchIndexClient(uri, new AzureKeyCredential(queryApiKey));
        _searchClient = indexClient.GetSearchClient(indexName);
    }

    public async Task RunQueryAsync(OfficeSearchData model)
    {
        var task = USECASE switch
        {
            UseCase.SimpleSearch => RunQueryAsync1(model),
            UseCase.FilterCategory => RunQueryAsync2(model),
            UseCase.SortRating => RunQueryAsync3(model),
            _ => throw new InvalidOperationException("Invalid use case.")
        };
        await task;
    }

    // Use case 1: simple search, searchMode=Any
    public async Task RunQueryAsync1(OfficeSearchData model)
    {
        var options = new SearchOptions()
        {
            IncludeTotalCount = true
        };

        // Enter Office property names into this list so only these values will be returned.
        // If Select is empty, all values will be returned, which can be inefficient.
        options.Select.Add("OfficeName");
        options.Select.Add("Address/City");
        options.Select.Add("Address/StateProvince");
        options.Select.Add("Description");
        options.Select.Add("Description_nl");
        options.Select.Add("Description_sr");
        options.Select.Add("NrOfParkingSpots");
        options.Select.Add("Rating");
        options.Select.Add("Tags");
        options.Select.Add("Desks");

        // For efficiency, the search call should be asynchronous, so use SearchAsync rather than Search.
        model.resultList = await _searchClient.SearchAsync<Office>(model.searchText, options).ConfigureAwait(false);
    }

    // Use case 2: Filter on Category 
    // (other "filterable" fields include Address/City and Address/StateProvince)
    public async Task RunQueryAsync2(OfficeSearchData model)
    {
        var options = new SearchOptions()
        {
            IncludeTotalCount = true,
            Filter = "search.in(Category,'Budget,Suite')" //TODO
        };

        // Enter Office property names into this list so only these values will be returned.
        // If Select is empty, all values will be returned, which can be inefficient.
        options.Select.Add("OfficeName");
        options.Select.Add("Address/City");
        options.Select.Add("Address/StateProvince");
        options.Select.Add("Description");
        options.Select.Add("NrOfParkingSpots");
        options.Select.Add("Rating");
        options.Select.Add("Tags");
        options.Select.Add("Desks");

        // For efficiency, the search call should be asynchronous, so use SearchAsync rather than Search.
        model.resultList = await _searchClient.SearchAsync<Office>(model.searchText, options).ConfigureAwait(false);
    }

    // Use case 3: Sort on Rating (there are no other "sortable" fields in the index)
    public async Task RunQueryAsync3(OfficeSearchData model)
    {
        var options = new SearchOptions()
        {
            IncludeTotalCount = true,
        };

        options.OrderBy.Add("Rating desc");

        // Enter Office property names into this list so only these values will be returned.
        // If Select is empty, all values will be returned, which can be inefficient.
        options.Select.Add("OfficeName");
        options.Select.Add("Address/City");
        options.Select.Add("Address/StateProvince");
        options.Select.Add("Description");
        options.Select.Add("NrOfParkingSpots");
        options.Select.Add("Rating");
        options.Select.Add("Tags");
        options.Select.Add("Desks");

        // For efficiency, the search call should be asynchronous, so use SearchAsync rather than Search.
        model.resultList = await _searchClient.SearchAsync<Office>(model.searchText, options).ConfigureAwait(false);
    }

    private enum UseCase
    {
        SimpleSearch = 1,
        FilterCategory = 2,
        SortRating = 3
    };
}