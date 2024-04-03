using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Indexes;
using System.Text.Json.Serialization;

namespace OfficeSearch.Models
{
    public partial class Office
    {
        [SimpleField(IsFilterable = true, IsKey = true)]
        public required string OfficeId { get; set; }

        [SearchableField(IsSortable = true)]
        public required string OfficeName { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public required string Description { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.NlLucene)]
        [JsonPropertyName("Description_nl")]
        public required string DescriptionNl { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.SrCyrillicMicrosoft)]
        [JsonPropertyName("Description_sr")]
        public required string DescriptionSr { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public required string Category { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public required string[] Tags { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public int NrOfParkingSpots { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public DateTimeOffset? LastRenovationDate { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public double? Rating { get; set; }

        public required Address Address { get; set; }

        public required Desk[] Desks { get; set; }
    }

}
