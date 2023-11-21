using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class MTFileTypeDataSourceHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "DOCX", ".docx" },
        { "DOCM", ".docm" },
        { "RTF", ".rtf" },
        { "PPTX", ".pptx" },
        { "XLSX", ".xlsx" },
        { "IDML", ".idml" },
        { "RESX", ".resx" },
        { "PLAIN_TEXT", ".txt" },
        { "XML", ".xml" },
        { "HTML", ".html" },
        { "PRES", ".pres" },
        { "SRT", ".srt" },
        { "MARKDOWN", ".md/.markdown" },
        { "DITA", ".dita" },
        { "VTT", ".vtt" },
        { "FLARE", ".flare" },
        { "SVG", ".svg" },
        { "XLIFF2", ".xliff" },
        { "CSV", ".csv" },
        { "XLSX_TEMPLATE", ".xltx" }
    };
}