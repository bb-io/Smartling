using System.Text;
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
            var transformation = Transformation.Parse(fileContent, fileName);
            var xliff = transformation.Serialize();
            return new PreparedUploadFile(Encoding.UTF8.GetBytes(xliff), transformation.XliffFileName, "xliff2");
        }
        catch (Exception ex)
        {
            throw new PluginMisconfigurationException($"Failed to convert '{fileName}' from HTML to XLIFF.", ex);
        }
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
