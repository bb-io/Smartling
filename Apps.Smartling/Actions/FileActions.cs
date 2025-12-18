using System.Globalization;
using System.Net;
using System.Net.Mime;
using Apps.Smartling.Api;
using Apps.Smartling.Models;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Files;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Files;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Files;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList("Files")]
public class FileActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : SmartlingInvocable(invocationContext)
{
    #region Get

    [Action("Search files", Description = "List recently uploaded source files with filters and pagination.")]
    public async Task<SearchFilesResponse> SearchFiles(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] SearchFilesRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);

        const int pageSize = 100;
        var offset = 0;
        var total = (int?)null;

        var all = new List<SourceFileSummaryDto>();

        while (true)
        {
            var request = new SmartlingRequest($"/files-api/v2/projects/{projectId}/files/list", Method.Get);

            request.AddQueryParameter("orderBy", "lastUploaded_desc");

            request.AddQueryParameter("limit", pageSize.ToString());
            request.AddQueryParameter("offset", offset.ToString());

            if (!string.IsNullOrWhiteSpace(input.FileUriContains))
                request.AddQueryParameter("uriMask", input.FileUriContains);
            if (input.FileTypes != null)
            {
                foreach (var t in input.FileTypes.Where(s => !string.IsNullOrWhiteSpace(s)))
                    request.AddQueryParameter("fileTypes[]", t);
            }

            if (input.UploadedAfter.HasValue)
                request.AddQueryParameter("lastUploadedAfter", FormatSmartlingDate(input.UploadedAfter.Value));

            if (input.UploadedBefore.HasValue)
                request.AddQueryParameter("lastUploadedBefore", FormatSmartlingDate(input.UploadedBefore.Value));


            var resp = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsPagedWrapper<SourceFileSummaryDto>>>(request);
            var data = resp.Response.Data;

            if (total is null) total = data.TotalCount;

            var pageItems = data.Items?.ToList() ?? new List<SourceFileSummaryDto>();
            if (pageItems.Count == 0)
                break;

            all.AddRange(pageItems);
            offset += pageItems.Count;

            if (offset >= total)
                break;
        }

        return new SearchFilesResponse
        {
            Items = all,
            TotalCount = total ?? all.Count
        };
    }

    [Action("List source files within job", Description = "List all source files within a job.")]
    public async Task<ListFilesResponse> ListFilesWithinJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/files", 
            Method.Get
        );
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<SourceFileWithLocalesDto>>>(request);
        return new(response.Response.Data.Items);
    }

    [Action("Download source file", Description = "Download source file.")]
    public async Task<FileWrapper> DownloadSourceFile(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] SourceFileIdentifier fileIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var endpoint = $"/files-api/v2/projects/{projectId}/file?fileUri={fileIdentifier.FileUri}";
        var file = await DownloadFile(endpoint);
        return new() { File = file };
    }

    [Action("Download translated file", Description = "Download translated file for a single locale.")]
    public async Task<FileWrapper> DownloadTranslatedFile(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] SourceFileIdentifier fileIdentifier,
        [ActionParameter] TargetLocaleIdentifier targetLocale)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var endpoint =
            $"/files-api/v2/projects/{projectId}/locales/{targetLocale.TargetLocaleId}/file?fileUri={fileIdentifier.FileUri}";
        var file = await DownloadFile(endpoint);
        file.Name = CreateNameForTranslatedFile(file.Name, targetLocale.TargetLocaleId);
        return new() { File = file };
    }
    
    [Action("Download file translations", Description = "Download all translations for the requested file.")]
    public async Task<DownloadFilesResponse> DownloadFileTranslations(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] SourceFileIdentifier fileIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var endpoint = $"/files-api/v2/projects/{projectId}/locales/all/file/zip?fileUri={fileIdentifier.FileUri}";
        var zip = await DownloadFile(endpoint);
        var files = await (await fileManagementClient.DownloadAsync(zip)).GetFilesFromZip();
        var resultFiles = new List<FileWrapper>();

        foreach (var file in files)
        {
            var splitPath = file.Path.Split("/");
            var locale = splitPath[0];
            var filename = splitPath[^1];
            var resultFilename = CreateNameForTranslatedFile(filename, locale);

            if (!MimeTypes.TryGetMimeType(resultFilename, out var contentType))
                contentType = MediaTypeNames.Application.Octet;

            var fileReference = await fileManagementClient.UploadAsync(file.FileStream, contentType, resultFilename);
            resultFiles.Add(new() { File = fileReference });
            
            file.Dispose();
        }
        
        return new(resultFiles);
    } 

    [Action("Download file translations in ZIP archive", Description = "Download a ZIP archive with all translations " +
                                                                       "for the requested file.")]
    public async Task<FileWrapper> DownloadFileTranslationsAsZip(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] SourceFileIdentifier fileIdentifier,
        [ActionParameter] [Display("Output ZIP filename")] string? zipFilename)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var endpoint = $"/files-api/v2/projects/{projectId}/locales/all/file/zip?fileUri={fileIdentifier.FileUri}";
        var zip = await DownloadFile(endpoint);
        zip.Name = zipFilename != null ? 
            Path.HasExtension(zipFilename) ? zipFilename : zipFilename + ".zip"
            : fileIdentifier.FileUri + ".zip";
        return new() { File = zip };
    }

    #region Get utils

    private async Task<FileReference> DownloadFile(string endpoint)
    {
        var request = new SmartlingRequest(endpoint, Method.Get);
        var response = await Client.ExecuteWithErrorHandling(request);
        var filename = response.ContentHeaders.First(header => header.Name == "Content-Disposition").Value.ToString()
            .Split('"')[1];
        var contentType = response.ContentType.Split(';')[0];

        using var stream = new MemoryStream(response.RawBytes);
        var file = await fileManagementClient.UploadAsync(stream, contentType, filename);
        return file;
    }

    private static string FormatSmartlingDate(DateTime dtUtc) =>
        dtUtc.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);

    private static string CreateNameForTranslatedFile(string originalFilename, string locale) 
        => $"{Path.GetFileNameWithoutExtension(originalFilename)}_{locale}{Path.GetExtension(originalFilename)}";

    #endregion
    
    #endregion

    #region Post

    [Action("Upload source file to job", Description = "Add all non-published strings from a file to a job.")]
    public async Task<SourceFileIdentifier> AddFileToJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] FileWrapper file, 
        [ActionParameter] TargetLocalesIdentifier targetLocales,
        [ActionParameter] fileUploadRequest FileInput)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var fileUri = file.File.Name;
        var fileType = FileInput != null && !string.IsNullOrEmpty(FileInput?.Type) ? FileInput.Type : GetFileType(file.File.Name);
        var getTargetFileDataRequest =
            new SmartlingRequest($"/files-api/v2/projects/{projectId}/target-file-types", Method.Post);
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
        
        var uploadFileRequest = new SmartlingRequest($"/files-api/v2/projects/{projectId}/file", Method.Post);

        var fileBytes = fileManagementClient.DownloadAsync(file.File).Result.GetByteData().Result;
        uploadFileRequest.AddFile("file", fileBytes, file.File.Name);
        uploadFileRequest.AddParameter("fileUri", fileUri);
        uploadFileRequest.AddParameter("fileType", fileType);
        if (FileInput != null && FileInput.Directives != null && FileInput.Directives.Any()
            && FileInput.DirectiveValues != null && FileInput.DirectiveValues.Any())
        {
            var directivePairs = FileInput.Directives.Zip(FileInput.DirectiveValues, (key, value) => new { key, value });
            foreach (var pair in directivePairs)
            {
                uploadFileRequest.AddParameter(pair.key, pair.value);
            }

        }
        await Client.ExecuteWithErrorHandling(uploadFileRequest);

        fileUri = getTargetFileDataResponse.Response.Data.Items.First().TargetFiles.First().TargetFileUri;

        RestResponse getFileStatusResponse;

        do
        {
            var getFileStatusRequest = new SmartlingRequest(
                $"/files-api/v2/projects/{projectId}/file/status?fileUri={fileUri}", 
                Method.Get
            );

            getFileStatusResponse = await Client.ExecuteAsync(getFileStatusRequest);
        } while (getFileStatusResponse.StatusCode != HttpStatusCode.OK);

        var locales = targetLocales.TargetLocaleIds;

        if (locales == null)
        {
            var getJobRequest = new SmartlingRequest($"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}", 
                Method.Get);
            var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobDto>>(getJobRequest);
            locales = response.Response.Data.TargetLocaleIds;
        }
        
        var addFileToJobRequest = 
            new SmartlingRequest($"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/file/add", 
                Method.Post);
        
        addFileToJobRequest.AddJsonBody(new
        {
            fileUri,
            targetLocaleIds = locales
        });

        await Client.ExecuteWithErrorHandling(addFileToJobRequest);
        return new SourceFileIdentifier { FileUri = fileUri };
    }

    [Action("Upload file to project", Description = "Uploads original source content to project.")]
    public async Task<SourceFileIdentifier> UploadFile(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] FileWrapper file,
        [ActionParameter] fileUploadRequest FileInput)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var fileUri = file.File.Name;
        var fileType = FileInput != null && !string.IsNullOrEmpty(FileInput?.Type) ? FileInput.Type : GetFileType(file.File.Name);
        var getTargetFileDataRequest =
            new SmartlingRequest($"/files-api/v2/projects/{projectId}/target-file-types", Method.Post);
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

        var uploadFileRequest = new SmartlingRequest($"/files-api/v2/projects/{projectId}/file", Method.Post);

        var fileBytes = fileManagementClient.DownloadAsync(file.File).Result.GetByteData().Result;
        uploadFileRequest.AddFile("file", fileBytes, file.File.Name);
        uploadFileRequest.AddParameter("fileUri", fileUri);
        uploadFileRequest.AddParameter("fileType", fileType);
        if (FileInput != null && FileInput.Directives != null && FileInput.Directives.Any()
           && FileInput.DirectiveValues != null && FileInput.DirectiveValues.Any())
        {
            var directivePairs = FileInput.Directives.Zip(FileInput.DirectiveValues, (key, value) => new { key, value });
            foreach (var pair in directivePairs)
            {
                uploadFileRequest.AddParameter(pair.key, pair.value);
            }

        }
        await Client.ExecuteWithErrorHandling(uploadFileRequest);

        fileUri = getTargetFileDataResponse.Response.Data.Items.First().TargetFiles.First().TargetFileUri;

        RestResponse getFileStatusResponse;

        do
        {
            var getFileStatusRequest =
                new SmartlingRequest($"/files-api/v2/projects/{projectId}/file/status?fileUri={fileUri}", Method.Get);
            getFileStatusResponse = await Client.ExecuteAsync(getFileStatusRequest);
        } while (getFileStatusResponse.StatusCode != HttpStatusCode.OK);

        return new SourceFileIdentifier { FileUri = fileUri };
    }

    [Action("Link uploaded file to job", Description = "")]
    public async Task LinkFileToJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] SourceFileIdentifier input,
        [ActionParameter] TargetLocalesIdentifier targetLocales) 
    {
        var locales = targetLocales.TargetLocaleIds;
        string projectId = await GetProjectId(project.ProjectId);

        if (locales == null)
        {
            var getJobRequest = new SmartlingRequest($"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}",
                Method.Get);
            var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobDto>>(getJobRequest);
            locales = response.Response.Data.TargetLocaleIds;
        }

        var addFileToJobRequest =
            new SmartlingRequest($"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/file/add",
                Method.Post);

        addFileToJobRequest.AddJsonBody(new
        {
            input.FileUri,
            targetLocaleIds = locales
        });

        await Client.ExecuteWithErrorHandling(addFileToJobRequest);
    }

    [Action("Import translation", Description = "Import a translated file.")]
    public async Task<ImportTranslationResponse> ImportTranslation(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] FileWrapper file, 
        [ActionParameter] TargetLocaleIdentifier targetLocale, 
        [ActionParameter] SourceFileIdentifier fileIdentifier,
        [ActionParameter] ImportTranslationRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var fileType = GetFileType(file.File.Name);
        var request = new SmartlingRequest(
            $"/files-api/v2/projects/{projectId}/locales/{targetLocale.TargetLocaleId}/file/import", 
            Method.Post
        );

        var fileBytes = fileManagementClient.DownloadAsync(file.File).Result.GetByteData().Result;
        request.AddFile("file", fileBytes, file.File.Name);
        request.AddParameter("fileUri", fileIdentifier.FileUri);
        request.AddParameter("fileType", fileType);
        request.AddParameter("translationState", input.TranslationState);

        if (input.Overwrite != null)
            request.AddParameter("overwrite", input.Overwrite.Value);
        
        var result = await Client.ExecuteWithErrorHandling<ResponseWrapper<ImportTranslationResponse>>(request);
        return result.Response.Data;
    }

    #region Post utils

    private static string GetFileType(string filename)
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