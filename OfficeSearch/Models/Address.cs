using Azure.Search.Documents.Indexes;

namespace OfficeSearch.Models;

public partial class Address
{
    [SearchableField]
    public required string StreetAddress { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public required string City { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public required string StateProvince { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public required string PostalCode { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public required string Country { get; set; }
}