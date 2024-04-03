// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Bug", "S2583:Conditionally executed code should be reachable", Justification = "https://github.com/SonarSource/sonar-dotnet/issues/8449", Scope = "member", Target = "~M:OfficeSearch.Services.Indexing.ExponentialBackoff.ExponentialBackoffAsync(Azure.Search.Documents.SearchClient,System.Collections.Generic.List{OfficeSearch.Models.Office},System.Int32)~System.Threading.Tasks.Task{Azure.Search.Documents.Models.IndexDocumentsResult}")]
[assembly: SuppressMessage("Major Bug", "S2583:Conditionally executed code should be reachable", Justification = "https://github.com/SonarSource/sonar-dotnet/issues/8449", Scope = "member", Target = "~M:OfficeSearch.Services.Indexing.IndexBuilder.ValidateIndexAsync(Azure.Search.Documents.Indexes.SearchIndexClient,System.String,System.Int64)~System.Threading.Tasks.Task")]
