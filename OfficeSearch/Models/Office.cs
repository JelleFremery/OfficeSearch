using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Indexes;
using System.Text.Json.Serialization;

namespace OfficeSearch.Models
{
    public partial class Office
    {
        [SimpleField(IsFilterable = true, IsKey = true)]
        public string OfficeId { get; set; }

        [SearchableField(IsSortable = true)]
        public string OfficeName { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string Description { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.NlLucene)]
        [JsonPropertyName("Description_nl")]
        public string DescriptionNl { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.SrCyrillicMicrosoft)]
        [JsonPropertyName("Description_sr")]
        public string DescriptionSr { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string Category { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string[] Tags { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public int NrOfParkingSpots { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public DateTimeOffset? LastRenovationDate { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public double? Rating { get; set; }

        public Address Address { get; set; }

        public Desk[] Desks { get; set; }
    }
}
