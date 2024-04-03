using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace OfficeSearch.Models;

public partial class Desk
{
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public string Name { get; set; }

    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public string Location { get; set; }

    [SimpleField(IsFilterable = true, IsFacetable = true)]
    public bool HeightAdjustable { get; set; }

    [SimpleField]
    public double? Length { get; set; }

    [SimpleField]
    public double? Width { get; set; }

    [SearchableField(IsFilterable = true, IsFacetable = true)]
    public string[] Tags { get; set; }
}
