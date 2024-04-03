using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace OfficeSearch.Models;

public partial class Desk
{
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public required string Name { get; set; }

    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public required string Location { get; set; }

    [SimpleField(IsFilterable = true, IsFacetable = true)]
    public bool HeightAdjustable { get; set; }

    [SimpleField]
    public double? Length { get; set; }

    [SimpleField]
    public double? Width { get; set; }

    [SearchableField(IsFilterable = true, IsFacetable = true)]
    public required string[] Tags { get; set; }
}
