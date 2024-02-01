using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Smartling.Models.Requests.Glossaries;

public class ImportGlossaryRequest
{
    public FileReference Glossary { get; set; }
    
    [Display("Glossary ID", Description = "Existing glossary ID to import data into.")]
    [DataSource(typeof(GlossaryDataSourceHandler))]
    public string? GlossaryUid { get; set; }
    
    [Display("Archive existing entries", Description = "Archive entries that are not in the import file when " +
                                                       "importing to an existing glossary. Defaults to 'False'.")]
    public bool? ArchiveExistingEntries { get; set; }
}