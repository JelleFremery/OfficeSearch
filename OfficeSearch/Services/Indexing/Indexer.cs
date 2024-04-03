﻿using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using OfficeSearch.Models;
using System.Text;
using System.Text.Json;

namespace OfficeSearch.Services.Indexing;

/// <summary>
/// https://learn.microsoft.com/en-us/azure/search/tutorial-optimize-indexing-push-api
/// </summary>
public class Indexer
{
    private readonly SearchClient _searchClient;
    private readonly SearchIndexClient _indexClient;
    private readonly string _indexName;

    public Indexer(IConfiguration _configuration)
    {
        _indexName = _configuration["IndexName"] ?? "";

        string searchServiceUri = _configuration["SearchServiceUri"] ?? "";
        string adminApiKey = _configuration["SearchServiceAdminApiKey"] ?? "";

        // Create a service and index client.
        _indexClient = new SearchIndexClient(new Uri(searchServiceUri), new AzureKeyCredential(adminApiKey));
        _searchClient = _indexClient.GetSearchClient(_indexName);
    }

    public async Task OptimizeBatchSize()
    {
        Console.WriteLine("{0}", "Deleting index...\n");
        await DeleteIndexIfExistsAsync(_indexName, _indexClient);

        Console.WriteLine("{0}", "Creating index...\n");
        await CreateIndexAsync(_indexName, _indexClient);

        Console.WriteLine("{0}", "Finding optimal batch size...\n");
        await TestBatchSizesAsync(_searchClient, numTries: 3);
    }

    public async Task Build()
    {
        Console.WriteLine("{0}", "Deleting index...\n");
        await DeleteIndexIfExistsAsync(_indexName, _indexClient);

        Console.WriteLine("{0}", "Creating index...\n");
        await CreateIndexAsync(_indexName, _indexClient);

        long numDocuments = 1000;
        DataGenerator dg = new DataGenerator();
        List<Office> offices = dg.GetOffices(numDocuments);

        Console.WriteLine("{0}", "Uploading using exponential backoff...\n");
        await ExponentialBackoff.IndexDataAsync(_searchClient, offices, 100, 8);

        Console.WriteLine("{0}", "Validating all data was indexed...\n");
        await ValidateIndexAsync(_indexClient, _indexName, numDocuments);

        Console.WriteLine("{0}", "\nComplete.\n");
    }

    // Delete an existing index to reuse its name
    private static async Task DeleteIndexIfExistsAsync(string indexName, SearchIndexClient indexClient)
    {
        try
        {
            await indexClient.GetIndexAsync(indexName);
            await indexClient.DeleteIndexAsync(indexName);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            //if the specified index not exist, 404 will be thrown.
        }
    }

    private static async Task CreateIndexAsync(string indexName, SearchIndexClient indexClient)
    {
        // Create a new search index structure that matches the properties of the Office class.
        // The Address class is referenced from the Office class. The FieldBuilder
        // will enumerate these to create a complex data structure for the index.
        FieldBuilder builder = new FieldBuilder();
        var definition = new SearchIndex(indexName, builder.Build(typeof(Office)));

        await indexClient.CreateIndexAsync(definition);
    }

    public static async Task UploadDocumentsAsync(SearchClient searchClient, List<Office> offices)
    {
        var batch = IndexDocumentsBatch.Upload(offices);
        try
        {
            await searchClient.IndexDocumentsAsync(batch).ConfigureAwait(false);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine("Failed to index the documents: \n{0}", ex.Message);
        }
    }

    public static async Task TestBatchSizesAsync(SearchClient searchClient, int min = 100, int max = 1000, int step = 100, int numTries = 3)
    {
        DataGenerator dg = new DataGenerator();

        Console.WriteLine("Batch Size \t Size in MB \t MB / Doc \t Time (ms) \t MB / Second");
        for (int numDocs = min; numDocs <= max; numDocs += step)
        {
            List<TimeSpan> durations = new List<TimeSpan>();
            double sizeInMb = 0.0;
            for (int x = 0; x < numTries; x++)
            {
                List<Office> offices = dg.GetOffices(numDocs);

                DateTime startTime = DateTime.Now;
                await UploadDocumentsAsync(searchClient, offices).ConfigureAwait(false);
                DateTime endTime = DateTime.Now;
                durations.Add(endTime - startTime);

                sizeInMb = EstimateObjectSize(offices);
            }

            var avgDuration = durations.Average(timeSpan => timeSpan.TotalMilliseconds);
            var avgDurationInSeconds = avgDuration / 1000;
            var mbPerSecond = sizeInMb / avgDurationInSeconds;

            Console.WriteLine("{0} \t\t {1} \t\t {2} \t\t {3} \t {4}", numDocs, Math.Round(sizeInMb, 3), Math.Round(sizeInMb / numDocs, 3), Math.Round(avgDuration, 3), Math.Round(mbPerSecond, 3));

            // Pausing 2 seconds to let the search service catch its breath
            Thread.Sleep(2000);
        }

        Console.WriteLine();
    }

    //Returns size of object in MB
    public static double EstimateObjectSize(object data)
    {
        var json = JsonSerializer.Serialize(data);
        var sizeInMb = Encoding.Unicode.GetByteCount(json) / 1000000;
        return sizeInMb;
    }

    public static async Task ValidateIndexAsync(SearchIndexClient indexClient, string indexName, long numDocsIndexed)
    {
        SearchClient searchClient = indexClient.GetSearchClient(indexName);

        long indexDocCount = await searchClient.GetDocumentCountAsync();
        while (indexDocCount != numDocsIndexed)
        {
            Console.WriteLine("Waiting for document count to update...\n");
            Thread.Sleep(2000);
            indexDocCount = await searchClient.GetDocumentCountAsync();
        }
        Console.WriteLine("Document Count is {0}\n", indexDocCount);


        var indexStats = await indexClient.GetIndexStatisticsAsync(indexName);
        while (indexStats.Value.DocumentCount != numDocsIndexed)
        {
            Console.WriteLine("Waiting for service statistics to update...\n");
            Thread.Sleep(10000);
            indexStats = await indexClient.GetIndexStatisticsAsync(indexName);
        }
        Console.WriteLine("Index Statistics: Document Count is {0}", indexStats.Value.DocumentCount);
        Console.WriteLine("Index Statistics: Storage Size is {0}\n", indexStats.Value.StorageSize);
    }
}
