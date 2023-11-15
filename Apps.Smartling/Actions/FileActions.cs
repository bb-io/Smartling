using Apps.Smartling.Api;
using Apps.Smartling.Models;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Files;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Files;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Files;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using RestSharp;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Smartling.Actions;

[ActionList]
public class FileActions : SmartlingInvocable
{
    public FileActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    #region Get

    [Display("List files within job", Description = "List all files within a job.")]
    public async Task<ListFilesResponse> ListFilesWithinJob([ActionParameter] JobIdentifier jobIdentifier)
    {
        var request = new SmartlingRequest($"/jobs-api/v3/projects/{ProjectId}/jobs/{jobIdentifier.TranslationJobUid}/files", 
            Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<SourceFileWithLocalesDto>>>(request);
        return new(response.Response.Data.Items);
    }

    [Action("Download source file", Description = "Download source file.")]
    public async Task<FileWrapper> DownloadSourceFile([ActionParameter] SourceFileIdentifier fileIdentifier)
    {
        var endpoint = $"/files-api/v2/projects/{ProjectId}/file?fileUri={fileIdentifier.FileUri}";
        var file = await DownloadFile(endpoint);
        return new() { File = file };
    }

    [Action("Download translated file", Description = "Download translated file for a single locale.")]
    public async Task<FileWrapper> DownloadTranslatedFile([ActionParameter] SourceFileIdentifier fileIdentifier,
        [ActionParameter] TargetLocaleIdentifier targetLocale)
    {
        var endpoint =
            $"/files-api/v2/projects/{ProjectId}/locales/{targetLocale.TargetLocaleId}/file?fileUri={fileIdentifier.FileUri}";
        var file = await DownloadFile(endpoint);
        file.Name = CreateNameForTranslatedFile(file.Name, targetLocale.TargetLocaleId);
        return new() { File = file };
    }
    
    [Action("Download file translations", Description = "Download all translations for the requested file.")]
    public async Task<DownloadFilesResponse> DownloadFileTranslations([ActionParameter] SourceFileIdentifier fileIdentifier)
    {
        var endpoint = $"/files-api/v2/projects/{ProjectId}/locales/all/file/zip?fileUri={fileIdentifier.FileUri}";
        var zip = await DownloadFile(endpoint);
        var files = await zip.Bytes.GetFilesFromZip();
        var resultFiles = new List<FileWrapper>();

        foreach (var file in files)
        {
            var locale = file.Path.Split("/")[0];
            var resultFilename = CreateNameForTranslatedFile(file.File.Name, locale);
            file.File.Name = resultFilename;
            resultFiles.Add(new() { File = file.File });
        }
        
        return new(resultFiles);
    } 

    [Action("Download file translations in ZIP archive", Description = "Download a ZIP archive with all translations " +
                                                                       "for the requested file.")]
    public async Task<FileWrapper> DownloadFileTranslationsAsZip([ActionParameter] SourceFileIdentifier fileIdentifier,
        [ActionParameter] [Display("Output ZIP filename")] string? zipFilename)
    {
        var endpoint = $"/files-api/v2/projects/{ProjectId}/locales/all/file/zip?fileUri={fileIdentifier.FileUri}";
        var zip = await DownloadFile(endpoint);
        zip.Name = zipFilename != null ? 
            Path.HasExtension(zipFilename) ? zipFilename : zipFilename + ".zip"
            : fileIdentifier.FileUri + ".zip";
        return new() { File = zip };
    }

    #region Get utils

    private async Task<File> DownloadFile(string endpoint)
    {
        var request = new SmartlingRequest(endpoint, Method.Get);
        var response = await Client.ExecuteWithErrorHandling(request);
        var filename = response.ContentHeaders.First(header => header.Name == "Content-Disposition").Value.ToString()
            .Split('"')[1];
        var contentType = response.ContentType.Split(';')[0];
        return new(response.RawBytes) { ContentType = contentType, Name = filename };
    }
    
    private string CreateNameForTranslatedFile(string originalFilename, string locale) 
        => $"{Path.GetFileNameWithoutExtension(originalFilename)}_{locale}{Path.GetExtension(originalFilename)}";

    #endregion
    
    #endregion

    #region Post

    [Action("Upload source file to job", Description = "Add all non-published strings from a file to a job.")]
    public async Task<JobIdentifier> AddFileToJob([ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] FileWrapper file, [ActionParameter] TargetLocalesIdentifier targetLocales)
    {
        var fileUri = file.File.Name;
        var fileType = GetFileType(file.File.Name);
        var getTargetFileDataRequest =
            new SmartlingRequest($"/files-api/v2/projects/{ProjectId}/target-file-types", Method.Post);
        getTargetFileDataRequest.AddJsonBody(new
        {
            sourceFiles = new[]
            {
                new
                {
                    sourceFileUri = fileUri,
                    sourceFileType = fileType
                }
            }
        });
        var getTargetFileDataResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<TargetFileDtoWrapper>>>(getTargetFileDataRequest);
        
        var uploadFileRequest = new SmartlingRequest($"/files-api/v2/projects/{ProjectId}/file", Method.Post);
        uploadFileRequest.AddFile("file", file.File.Bytes, file.File.Name);
        uploadFileRequest.AddParameter("fileUri", fileUri);
        uploadFileRequest.AddParameter("fileType", fileType);
        await Client.ExecuteWithErrorHandling(uploadFileRequest);
        
        fileUri = getTargetFileDataResponse.Response.Data.Items.First().TargetFiles.First().TargetFileUri;

        var addFileToJobRequest = 
            new SmartlingRequest($"/jobs-api/v3/projects/{ProjectId}/jobs/{jobIdentifier.TranslationJobUid}/file/add", 
                Method.Post);
        
        addFileToJobRequest.AddJsonBody(new
        {
            fileUri,
            targetLocaleIds = targetLocales.TargetLocaleIds
        });

        await Client.ExecuteWithErrorHandling(addFileToJobRequest);
        return jobIdentifier;
    }
    
    [Action("Import translation", Description = "Import a translated file.")]
    public async Task<ImportTranslationResponse> ImportTranslation([ActionParameter] FileWrapper file, 
        [ActionParameter] TargetLocaleIdentifier targetLocale, [ActionParameter] SourceFileIdentifier fileIdentifier,
        [ActionParameter] ImportTranslationRequest input)
    {
        var fileType = GetFileType(file.File.Name);
        var request = new SmartlingRequest($"/files-api/v2/projects/{ProjectId}/locales/{targetLocale.TargetLocaleId}/file/import", 
            Method.Post);
        request.AddFile("file", file.File.Bytes, file.File.Name);
        request.AddParameter("fileUri", fileIdentifier.FileUri);
        request.AddParameter("fileType", fileType);
        request.AddParameter("translationState", input.TranslationState);

        if (input.Overwrite != null)
            request.AddParameter("overwrite", input.Overwrite.Value);
        
        var result = await Client.ExecuteWithErrorHandling<ResponseWrapper<ImportTranslationResponse>>(request);
        return result.Response.Data;
    }

    #region Post utils

    private string GetFileType(string filename)
    {
        var extension = Path.GetExtension(filename).TrimStart('.');
        string fileType;

        switch (extension)
        {
            case "po":
                fileType = "gettext";
                break;
            case "pot":
                fileType = "gettext";
                break;
            case "strings":
                fileType = "ios";
                break;
            case "properties":
                fileType = "javaProperties";
                break;
            case "md":
                fileType = "markdown";
                break;
            case "txt":
                fileType = "plainText";
                break;
            case "resw":
                fileType = "resx";
                break;
            case "xlf":
                fileType = "xliff";
                break;
            case "ts":
                fileType = "qt";
                break;
            default:
                fileType = extension;
                break;
        }

        return fileType;
    }

    #endregion

    #endregion
}