using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class GlossaryEntryIdentifier
{
    [Display("Entry")]
    [DataSource(typeof(GlossaryEntryDataSourceHandler))]
    public string EntryUid { get; set; }
}