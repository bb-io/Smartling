using Apps.Smartling.Api;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Dtos.OfflineTranslations;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.OfflineTranslations.Api;
using Apps.Smartling.Polling.Models;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Smartling.Polling;

[PollingEventList("Offline translations")]
public class OfflineTranslationPollingList(InvocationContext invocationContext) : SmartlingInvocable(invocationContext)
{
    [PollingEvent("On translation package generated [Polling]")]
    public async Task<PollingEventResponse<TranslationPackageMemory, TranslationPackageDto>> OnTranslationPackageGenerated(
        PollingEventRequest<TranslationPackageMemory> request,
        [PollingEventParameter] ProjectIdentifier project,
        [PollingEventParameter] TranslationPackageIdentifier packageIdentifier)
    {
        var memory = request.Memory ?? new TranslationPackageMemory();
        
        if (memory.Notified)
            return DontFly(request);
        
        string projectId = await GetProjectId(project.ProjectId);
        
        string endpoint = $"/translations-api/v2/projects/{projectId}/translation-packages/{packageIdentifier.TranslationPackageUid}";
        var packageRequest = new SmartlingRequest(endpoint, Method.Get);
        
        TranslationPackageApiResponse? responseData;
        try
        {
            // Can fail with 500
            var packageResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<TranslationPackageApiResponse>>(packageRequest);
            responseData = packageResponse.Response.Data;
        }
        catch (PluginApplicationException)
        {
            return DontFly(request);
        }

        string? xliffLink = responseData.Links
            .FirstOrDefault(x => string.Equals(x.Rel, TranslationPackageLinkRels.Xliff, StringComparison.OrdinalIgnoreCase))?
            .Href;

        string? tmLink = responseData.Links
            .FirstOrDefault(x => string.Equals(x.Rel, TranslationPackageLinkRels.TranslationMemory, StringComparison.OrdinalIgnoreCase))?
            .Href;
        
        if (string.IsNullOrWhiteSpace(xliffLink) && string.IsNullOrWhiteSpace(tmLink))
            return DontFly(request);

        return new PollingEventResponse<TranslationPackageMemory, TranslationPackageDto>
        {
            FlyBird = true,
            Memory = new TranslationPackageMemory { Notified = true },
            Result = new(responseData)
        };
    }

    private static PollingEventResponse<TranslationPackageMemory, TranslationPackageDto> DontFly(
        PollingEventRequest<TranslationPackageMemory> request)
    {
        return new PollingEventResponse<TranslationPackageMemory, TranslationPackageDto>
        {
            FlyBird = false,
            Memory = request.Memory
        };
    }
}