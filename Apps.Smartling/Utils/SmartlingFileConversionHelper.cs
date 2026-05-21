using System.Text;
using System.Text.RegularExpressions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Filters.Transformations;

namespace Apps.Smartling.Utils;

public static class SmartlingFileConversionHelper
{
    public static PreparedUploadFile PrepareUploadFile(byte[] fileBytes, string fileName, string fileType, bool? uploadAsXliff)
    {
        if (uploadAsXliff != true)
            return new PreparedUploadFile(fileBytes, fileName, fileType);

        if (!IsHtmlFile(fileName, fileType))
            throw new PluginMisconfigurationException("'Upload as XLIFF' is only supported for HTML files.");

        try
        {
            var fileContent = Encoding.UTF8.GetString(fileBytes);
            var keyToFieldId = ExtractKeyToFieldIdMap(fileContent);
            var transformation = Transformation.Parse(fileContent, fileName);
            var xliff = transformation.Serialize(unit =>
                unit.Key != null && keyToFieldId.TryGetValue(unit.Key, out var fieldId)
                    ? fieldId
                    : null);
            return new PreparedUploadFile(Encoding.UTF8.GetBytes(xliff), transformation.XliffFileName, "xliff2");
        }
        catch (Exception ex)
        {
            throw new PluginMisconfigurationException($"Failed to convert '{fileName}' from HTML to XLIFF.", ex);
        }
    }

    private static Dictionary<string, string> ExtractKeyToFieldIdMap(string htmlContent)
    {
        var map = new Dictionary<string, string>();
        var tagPattern = new Regex(@"<[a-zA-Z][^>]*>");
        var keyAttrPattern = new Regex(@"data-blackbird-key=""([^""]+)""");
        var fieldIdAttrPattern = new Regex(@"[a-zA-Z][\w-]*field-id=""([^""]+)""");

        foreach (Match tagMatch in tagPattern.Matches(htmlContent))
        {
            var tag = tagMatch.Value;
            var keyMatch = keyAttrPattern.Match(tag);
            var fieldIdMatch = fieldIdAttrPattern.Match(tag);
            if (keyMatch.Success && fieldIdMatch.Success)
            {
                map[keyMatch.Groups[1].Value] = fieldIdMatch.Groups[1].Value;
            }
        }

        return map;
    }

    private static bool IsHtmlFile(string fileName, string fileType)
    {
        if (string.Equals(fileType, "html", StringComparison.OrdinalIgnoreCase))
            return true;

        var extension = Path.GetExtension(fileName);
        return string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase);
    }
}

public record PreparedUploadFile(byte[] FileBytes, string FileUri, string FileType);

public record DownloadedFileContent(byte[] FileBytes, string FileName, string ContentType);
