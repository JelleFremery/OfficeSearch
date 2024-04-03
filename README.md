# OfficeSearch

## Introduction
This project is a proof of concept for a service that allows searching on custom, programmatic data.
Its intended use is for the Betabit 2024 Q2 Deepdive AI session.

## Getting started
Create a new AI Search service in the Azure portal. Free tier will suffice.
Copy the SearchServiceUri, SearchServiceAdminApiKey, and SearchServiceQueryApiKey values from the Keys and Endpoints page of the new service in the Azure portal.
Familiarize yourself with the code, specifically the IndexBuilder and QueryService classes.

## Building the index
1. If you are looking to optimize upload time, take a specific look at the IndexBuilder class, specifically the OptimizeBatchSize method. This method will be called from the HomeController.BuildIndex() method instead of the current Indexer.Build() method if the flag _optimizeBatchSize is set to true.	
1. If you are fine with unoptimized upload time, simply run the app as-is.
1. Once the app is running, go to the Build Index page and click the Create Index button.
1. This will (delete and re-)create an index and populate it with generated sample data. This may take several minutes.

## Querying the index
1. Query the newly created index by going to the Index page, entering a search term in the search box and clicking the Search button.
1. You should now see the search results displayed in the search results box.
1. For further exploration, you can set the UseCase constant in the QueryService class to "FilterCategory" or "SortRating" to customize the search results.

## Thought exercise
1. Optionally, think about how you'd go about building a code-first index for unstructured data, like PDFs.