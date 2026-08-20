using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos.OfflineTranslations;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.OnlineTranslations.Api;
using Apps.Smartling.Polling.Models;
using Apps.Smartling.Polling.Models.Requests;
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
        [PollingEventParameter] TranslationPackageIdentifier packageIdentifier,
        [PollingEventParameter] OnTranslationPackageGeneratedRequest input)
    {
        if (request.Memory is null)
        {
            return new PollingEventResponse<TranslationPackageMemory, TranslationPackageDto>
            {
                FlyBird = false,
                Memory = new TranslationPackageMemory { Notified = false },
                Result = null
            };
        }
        
        if (request.Memory.Notified)
            return DontFly(request);
        
        string projectId = await GetProjectId(project.ProjectId);
        
        string endpoint = $"/translations-api/v2/projects/{projectId}/translation-packages/{packageIdentifier.TranslationPackageUid}";
        var packageRequest = new SmartlingRequest(endpoint, Method.Get);
        var packageResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<TranslationPackageApiResponse>>(packageRequest);

        var responseData = packageResponse.Response.Data;
        
        if (string.IsNullOrWhiteSpace(responseData.Links.Content))
            return DontFly(request);    // Main XLIFF is not ready
        
        if (input.AlsoWaitForTbx is true && string.IsNullOrWhiteSpace(responseData.Links.Glossary) || 
            input.AlsoWaitForTmx is true && string.IsNullOrWhiteSpace(responseData.Links.TranslationMemory))
        {
            return DontFly(request);
        }

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