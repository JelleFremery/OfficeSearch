
namespace OfficeSearch.Services.Indexing
{
    public interface IIndexBuilder
    {
        Task Build();
        Task OptimizeBatchSize();
    }
}