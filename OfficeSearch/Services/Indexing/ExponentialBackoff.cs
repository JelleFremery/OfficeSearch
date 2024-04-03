using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using OfficeSearch.Models;

namespace OfficeSearch.Services.Indexing;

public static class ExponentialBackoff
{
    public static async Task IndexDataAsync(SearchClient searchClient, List<Office> offices, int batchSize, int numThreads)
    {
        int numDocs = offices.Count;
        Console.WriteLine("Uploading {0} documents...\n", numDocs.ToString());

        DateTime startTime = DateTime.Now;
        Console.WriteLine("Started at: {0} \n", startTime);
        Console.WriteLine("Creating {0} threads...\n", numThreads);

        // Creating a list to hold active tasks
        List<Task<IndexDocumentsResult>> uploadTasks = new List<Task<IndexDocumentsResult>>();

        for (int i = 0; i < numDocs; i += batchSize)
        {
            List<Office> officeBatch = offices.GetRange(i, batchSize);
            var task = ExponentialBackoffAsync(searchClient, officeBatch, i);
            uploadTasks.Add(task);
            Console.WriteLine("Sending a batch of {0} docs starting with doc {1}...\n", batchSize, i);

            // Checking if we've hit the specified number of threads
            if (uploadTasks.Count >= numThreads)
            {
                Task<IndexDocumentsResult> firstTaskFinished = await Task.WhenAny(uploadTasks);
                Console.WriteLine("Finished a thread, kicking off another...");
                uploadTasks.Remove(firstTaskFinished);
            }
        }

        // waiting for remaining results to finish
        await Task.WhenAll(uploadTasks);

        DateTime endTime = DateTime.Now;

        TimeSpan runningTime = endTime - startTime;
        Console.WriteLine("\nEnded at: {0} \n", endTime);
        Console.WriteLine("Upload time total: {0}", runningTime);

        double timePerBatch = Math.Round(runningTime.TotalMilliseconds / (numDocs / batchSize), 4);
        Console.WriteLine("Upload time per batch: {0} ms", timePerBatch);

        double timePerDoc = Math.Round(runningTime.TotalMilliseconds / numDocs, 4);
        Console.WriteLine("Upload time per document: {0} ms \n", timePerDoc);
    }

    private static async Task<IndexDocumentsResult> ExponentialBackoffAsync(SearchClient searchClient, List<Office> offices, int id)
    {
        // Create batch of documents for indexing
        var batch = IndexDocumentsBatch.Upload(offices);

        // Create an object to hold the result
        IndexDocumentsResult result;

        // Define parameters for exponential backoff
        int attempts = 0;
        TimeSpan delay = delay = TimeSpan.FromSeconds(2);
        int maxRetryAttempts = 5;

        // Implement exponential backoff
        do
        {
            try
            {
                attempts++;
                result = await searchClient.IndexDocumentsAsync(batch).ConfigureAwait(false);

                var failedDocuments = result.Results.Where(r => r.Succeeded != true).ToList();

                // handle partial failure
                if (failedDocuments.Count > 0)
                {
                    if (attempts == maxRetryAttempts)
                    {
                        Console.WriteLine("[MAX RETRIES HIT] - Giving up on the batch starting at {0}", id);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("[Batch starting at doc {0} had partial failure]", id);
                        Console.WriteLine("[Attempt: {0} of {1} failed]", attempts, maxRetryAttempts);
                        Console.WriteLine("[Retrying {0} failed documents] \n", failedDocuments.Count);

                        // creating a batch of failed documents to retry
                        var failedDocumentKeys = failedDocuments.Select(doc => doc.Key).ToList();
                        offices = offices.Where(h => failedDocumentKeys.Contains(h.OfficeId)).ToList();
                        batch = IndexDocumentsBatch.Upload(offices);

                        Task.Delay(delay).Wait();
                        delay = delay * 2;
                        continue;
                    }
                }


                return result;
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("[Batch starting at doc {0} failed]", id);
                Console.WriteLine("[Attempt: {0} of {1} Failed] - Error: {2} \n", attempts, maxRetryAttempts, ex.Message);
                Console.WriteLine("[Retrying entire batch] \n");

                if (attempts == maxRetryAttempts)
                {
                    Console.WriteLine("[MAX RETRIES HIT] - Giving up on the batch starting at {0}", id);
                    break;
                }

                Task.Delay(delay).Wait();
                delay = delay * 2;
            }
        } while (true);

        return null;
    }
}
