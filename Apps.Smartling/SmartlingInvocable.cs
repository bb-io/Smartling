using RestSharp;
using Apps.Smartling.Api;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Apps.Smartling.Models.Dtos.Project;

namespace Apps.Smartling;

public class SmartlingInvocable : BaseInvocable
{
    protected readonly SmartlingClient Client;
    protected readonly IEnumerable<AuthenticationCredentialsProvider> Credentials;

    protected SmartlingInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Credentials = invocationContext.AuthenticationCredentialsProviders;
        Client = new(Credentials);
    }
    
    protected async Task<string> GetAccountUid()
    {
        string connectionType = Credentials.Get(CredsNames.ConnectionType).Value;

        switch (connectionType)
        {
            case ConnectionTypes.AccountWide:
                return Credentials.Get(CredsNames.AccountUid).Value;
            case ConnectionTypes.ProjectWide:
                string projectId = await GetProjectId();
                var request = new SmartlingRequest($"/projects-api/v2/projects/{projectId}", Method.Get);
                var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDtoWithTargetLocales>>(request);
                return response.Response.Data.AccountUid;
            default:
                throw new Exception($"Unsupported connection type: {connectionType}");
        }
    }

    protected async Task<string> GetProjectId(string? inputProjectId = null)
    {
        string connectionType = Credentials.Get(CredsNames.ConnectionType).Value;

        return connectionType switch
        {
            ConnectionTypes.AccountWide =>
                inputProjectId
                ?? throw new PluginMisconfigurationException("Please specify the project ID in the input"),

            ConnectionTypes.ProjectWide =>
                inputProjectId
                ?? Credentials.Get(CredsNames.ProjectId).Value
                ?? throw new PluginMisconfigurationException("Please specify the project ID in the connection or input"),

            _ => throw new Exception($"Unsupported connection type: {connectionType}")
        };
    }
}