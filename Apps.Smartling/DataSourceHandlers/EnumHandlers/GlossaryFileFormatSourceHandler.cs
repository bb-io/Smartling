using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

    public class GlossaryFileFormatSourceHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new("TBX", "TBX - recommended, interoperable with other apps"),
            new("CSV", "CSV - raw export"),
        };

    }
}

