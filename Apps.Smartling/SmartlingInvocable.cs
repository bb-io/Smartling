using Apps.Smartling.Api;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Smartling;

public class SmartlingInvocable : BaseInvocable
{
    protected readonly SmartlingClient Client;
    protected readonly string ProjectId;

    protected SmartlingInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        var credentials = InvocationContext.AuthenticationCredentialsProviders;
        Client = new(credentials);
        ProjectId = credentials.Get(CredsNames.ProjectId).Value;
    }
    
    protected async Task<string> GetAccountUid()
    {
        var getProjectRequest = new SmartlingRequest($"/projects-api/v2/projects/{ProjectId}", Method.Get);
        var getProjectResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDto>>(getProjectRequest);
        var accountUid = getProjectResponse.Response.Data.AccountUid;
        return accountUid;
    }
}