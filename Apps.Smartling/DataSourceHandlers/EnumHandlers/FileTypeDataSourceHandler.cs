using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class FileTypeDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "android", "android" },
            { "ios", "ios" },
            { "gettext", "gettext" },
            { "html", "html" },
            { "java_properties", "java_properties" },
            { "xliff", "xliff" },
            { "xml", "xml" },
            { "json", "json" },
            { "docx", "docx" },
            { "pptx", "pptx" },
            { "xlsx", "xlsx" },
            { "xlsxTemplate", "xlsxTemplate" },
            { "idml", "idml" },
            { "qt", "qt" },
            { "resx", "resx" },
            { "plain_text", "plain_text" },
            { "csv", "csv" },
            { "srt", "srt" },
            { "stringsdict", "stringsdict" },
            { "xls", "xls" },
            { "doc", "doc" },
            { "ppt", "ppt" },
            { "pres", "pres" },
            { "madcap", "madcap" },
            { "yaml", "yaml" },
            { "markdown", "markdown" }

        };
    }
}