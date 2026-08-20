using System.Net;
using Apps.Smartling.Api;
using Apps.Smartling.Models;
using Apps.Smartling.Models.Dtos.OfflineTranslations;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.OfflineTranslations;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.OnlineTranslations;
using Apps.Smartling.Models.Responses.OnlineTranslations.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Blackbird.Filters.Constants;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList("Offline translations")]
public class OfflineTranslationActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : SmartlingInvocable(invocationContext)
{
    [Action("Create translation package", Description = "Create a new translation package for a given project and locale")]
    public async Task<TranslationPackageDto> CreateTranslationPackage(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] TargetLocaleIdentifier localeIdentifier,
        [ActionParameter] WorkflowStepIdentifier workflowStepIdentifier,
        [ActionParameter] JobOptionalIdentifier jobIdentifier,
        [ActionParameter] CreateTranslationPackageRequest createInput)
    {
        string endpoint = 
            $"translations-api/v2/projects/{projectIdentifier.ProjectId}/locales/{localeIdentifier.TargetLocaleId}/translation-packages";

        var body = new Dictionary<string, object?>
        {
            { "workflowStepUid", workflowStepIdentifier.WorkflowStepUid },
            { "translationJobUid", jobIdentifier.TranslationJobUid },
            { "generateTmx", createInput.GenerateTmx ?? false },
            { "generateTbx", createInput.GenerateTbx ?? false }
        }.AllIsNotNull();

        var request = new SmartlingRequest(endpoint, Method.Post).WithJsonBody(body);
        var response = await Client.ExecuteWithErrorHandling<TranslationPackageApiResponse>(request);
        return new(response);
    }

    [Action("Download XLIFF from translation package", Description = "Download an XLIFF file from a translation package")]
    public async Task<FileWrapper> DownloadXliffFromPackage(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] TranslationPackageIdentifier packageIdentifier)
    {
        string projectId = await GetProjectId(projectIdentifier.ProjectId);
        
        var responseBytes = await DownloadContentBytes(projectId, packageIdentifier.TranslationPackageUid, "content");
        using var fileStream = new MemoryStream(responseBytes);
        string fileName = $"{packageIdentifier.TranslationPackageUid}.xliff";
            
        var file = await fileManagementClient.UploadAsync(fileStream, MediaTypes.Xliff, fileName);
        return new FileWrapper { File = file };
    }

    [Action("Download translation memory from translation package", Description = "Download a TMX file from a translation package")]
    public async Task<FileWrapper> DownloadTmxFromPackage(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] TranslationPackageIdentifier packageIdentifier)
    {
        string projectId = await GetProjectId(projectIdentifier.ProjectId);
        
        var responseBytes = await DownloadContentBytes(projectId, packageIdentifier.TranslationPackageUid, "content");
        using var fileStream = new MemoryStream(responseBytes);
        string fileName = $"{packageIdentifier.TranslationPackageUid}.tmx";
        
        var file = await fileManagementClient.UploadAsync(fileStream, "application/x-tmx+xml", fileName);
        return new FileWrapper { File = file };
    }

    [Action("Download glossary from translation package", Description = "Download a TBX file from a translation package")]
    public async Task<FileWrapper> DownloadTbxFromPackage(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] TranslationPackageIdentifier packageIdentifier)
    {
        string projectId = await GetProjectId(projectIdentifier.ProjectId);

        string endpoint = $"/translations-api/v2/projects/{projectId}/translation-packages/{packageIdentifier.TranslationPackageUid}";
        var packageRequest = new SmartlingRequest(endpoint, Method.Get);
        var packageResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<TranslationPackageApiResponse>>(packageRequest);

        string? tbxDownloadLink = packageResponse.Response.Data.Links.Glossary;
        if (string.IsNullOrWhiteSpace(tbxDownloadLink))
        {
            throw new PluginMisconfigurationException(
                "The glossary is not available for this package. Make sure it was created with " +
                "'Include glossary (TBX)' enabled and that generation has finished");
        }

        var downloadRequest = new RestRequest(tbxDownloadLink);
        var downloadResponse = await new RestClient().ExecuteAsync(downloadRequest);
        if (downloadResponse.RawBytes is null)
            throw new PluginApplicationException("The downloaded file is empty");
        
        using var fileStream = new MemoryStream(downloadResponse.RawBytes);
        string fileName = $"{packageIdentifier.TranslationPackageUid}.tbx";
        var file = await fileManagementClient.UploadAsync(fileStream, "application/x-tbx+xml", fileName);
        return new FileWrapper { File = file };
    }

    [Action("Upload translated content", Description = "Import translated content")]
    public async Task<UploadTranslatedContentResponse> UploadTranslatedContent(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] TargetLocaleIdentifier localeIdentifier,
        [ActionParameter] UploadTranslatedContentRequest uploadInput)
    {
        await using var file = await fileManagementClient.DownloadAsync(uploadInput.File);
        var fileBytes = await file.GetByteData();
        
        string projectId = await GetProjectId(projectIdentifier.ProjectId);
        
        string endpoint = $"/translations-api/v2/projects/{projectId}/locales/{localeIdentifier.TargetLocaleId}/content";
        var request = new SmartlingRequest(endpoint, Method.Post) { AlwaysMultipartFormData = true }
            .AddFile(uploadInput.File.Name, fileBytes, uploadInput.File.Name)
            .AddParameter("submitContent", uploadInput.SubmitContent ?? false);
            
        if (!string.IsNullOrEmpty(uploadInput.WorkflowStepUid))
            request.AddParameter("workflowStepUid", uploadInput.WorkflowStepUid);

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ImportTranslationContentApiResponse>>(request);

        var importErrors = response.Response.Data.ImportErrors;
        if (importErrors is null || importErrors.Count == 0) 
            return new(response.Response.Data);
        
        List<string> errors = [];
        foreach (var importError in importErrors)
        {
            string unitId = importError.HashCode;
            string unitErrors = string.Join(", ", importError.Messages ?? []);
            errors.Add($"Unit {unitId}: {unitErrors}");
        }

        if (errors.Count == 0) 
            return new(response.Response.Data);
        
        string errorMessages = string.Join("; ", errors);
        InvocationContext.Logger?.LogError($"Some errors occured during import. {errorMessages}", []);

        return new(response.Response.Data);
    }
    
    private async Task<byte[]> DownloadContentBytes(string projectId, string packageId, string contentType)
    {
        string endpoint = $"/translations-api/v2/projects/{projectId}/translation-packages/{packageId}/{contentType}";
        
        var request = new SmartlingRequest(endpoint, Method.Get);
        var response = await Client.ExecuteWithErrorHandling(request);
        
        if (response.StatusCode == HttpStatusCode.Accepted)
            throw new PluginMisconfigurationException("The requested package hasn't generated yet. Please try again later");
        
        return response.RawBytes ?? throw new PluginApplicationException("The downloaded file is empty");
    }
}