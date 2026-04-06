using System.Net.Mime;
using System.Text;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff1;
using Blackbird.Filters.Xliff.Xliff2;

namespace Apps.Smartling.Utils;

public static class SmartlingFileConversionHelper
{
    private const string SmartlingMetadataCategory = "smartling";
    private const string SourceFileTypeMetadataType = "source-file-type";
    private const string SourceXliffVersionMetadataType = "source-xliff-version";

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
            transformation.MetaData.Set(CreateMetadataCategory(), SourceFileTypeMetadataType, "html");
            transformation.MetaData.Set(CreateMetadataCategory(), SourceXliffVersionMetadataType, Xliff2Version.Xliff20.Serialize());

            var xliff20 = Xliff2Serializer.Serialize(transformation, Xliff2Version.Xliff20);
            return new PreparedUploadFile(Encoding.UTF8.GetBytes(xliff20), transformation.XliffFileName, "xliff2");
        }
        catch (Exception ex)
        {
            throw new PluginMisconfigurationException($"Failed to convert '{fileName}' from HTML to XLIFF 2.0.", ex);
        }
    }

    public static DownloadedFileContent ConvertDownloadedTranslation(byte[] fileBytes, string fileName, string contentType)
    {
        var fileContent = Encoding.UTF8.GetString(fileBytes);
        var isXliff2 = Xliff2Serializer.IsXliff2(fileContent);
        var isXliff1 = Xliff1Serializer.IsXliff1(fileContent);

        if (!isXliff1 && !isXliff2)
            return new DownloadedFileContent(fileBytes, fileName, contentType);

        if (isXliff2 && (!Xliff2Serializer.TryGetXliffVersion(fileContent, out var xliffVersion) || xliffVersion != Xliff2Version.Xliff20.Serialize()))
            return new DownloadedFileContent(fileBytes, fileName, contentType);

        try
        {
            var transformation = Transformation.Parse(fileContent, fileName);
            if (!ShouldConvertBackToHtml(transformation))
                return new DownloadedFileContent(fileBytes, fileName, contentType);

            var html = transformation.Target().Serialize();
            var restoredFileName = transformation.OriginalName ?? fileName;
            return new DownloadedFileContent(Encoding.UTF8.GetBytes(html), restoredFileName, MediaTypeNames.Text.Html);
        }
        catch
        {
            return new DownloadedFileContent(fileBytes, fileName, contentType);
        }
    }

    private static bool ShouldConvertBackToHtml(Transformation transformation)
    {
        if (!string.Equals(transformation.OriginalMediaType, MediaTypeNames.Text.Html, StringComparison.OrdinalIgnoreCase))
            return false;

        var sourceFileType = transformation.MetaData.Get(CreateMetadataCategory(), SourceFileTypeMetadataType);
        return string.Equals(sourceFileType, "html", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHtmlFile(string fileName, string fileType)
    {
        if (string.Equals(fileType, "html", StringComparison.OrdinalIgnoreCase))
            return true;

        var extension = Path.GetExtension(fileName);
        return string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase);
    }

    private static List<string> CreateMetadataCategory() => [Meta.Categories.Blackbird, SmartlingMetadataCategory];
}

public record PreparedUploadFile(byte[] FileBytes, string FileUri, string FileType);

public record DownloadedFileContent(byte[] FileBytes, string FileName, string ContentType);
