using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class GlossaryIdentifier
{
    [Display("Glossary ID")]
    [DataSource(typeof(GlossaryDataSourceHandler))]
    public string GlossaryUid { get; set; }
}