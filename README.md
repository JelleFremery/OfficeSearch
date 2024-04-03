# OfficeSearch

1. Create a new AI Search service in the Azure portal.		
1. Copy the SearchServiceUri, SearchServiceAdminApiKey, and SearchServiceQueryApiKey values from the Keys and Endpoints page of the new service in the Azure portal.
1. Look around the code, specifically the IndexBuilder and QueryService classes.
1. If you are looking to optimize upload time, take a specific look at the IndexBuilder class, specifically the OptimizeBatchSize method, which can be called from the HomeController.BuildIndex() method instead of the current Indexer.Build() method.
1. Run the app.
1. Go to the Build Index page and click the Create Index button to (re)create an index and populate it with the sample data. This may take several minutes.
1. Query the newly created index by going to the Index page, entering a search term in the search box and clicking the Search button.
1. You should now see the search results displayed in the search results box.
1. For further exploration, you can set the UseCase constant in the QueryService class to "FilterCategory" or "SortRating" to customize the search results.
1. Optionally, think about how you'd go about building a code-first index for unstructured data, like PDFs.