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
    private readonly IEnumerable<AuthenticationCredentialsProvider> _credentials;

    protected SmartlingInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        _credentials = invocationContext.AuthenticationCredentialsProviders;
        Client = new(_credentials);
    }
    
    protected async Task<string> GetAccountUid()
    {
        string connectionType = _credentials.Get(CredsNames.ConnectionType).Value;
        string accountId = string.Empty;
        
        switch (connectionType)
        {
            case ConnectionTypes.AccountWide:
                accountId = _credentials.Get(CredsNames.AccountUid).Value;
                break;
            case ConnectionTypes.ProjectWide:
                var request = new SmartlingRequest($"/projects-api/v2/projects/{GetProjectId()}", Method.Get);
                var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDtoWithTargetLocales>>(request);
                accountId = response.Response.Data.AccountUid;
                break;
            default:
                throw new Exception($"Unsupported connection type: {connectionType}");
        }

        return accountId;
    }

    protected async Task<string> GetProjectId(string? inputProjectId = null)
    {
        string connectionType = _credentials.Get(CredsNames.ConnectionType).Value;

        return connectionType switch
        {
            ConnectionTypes.AccountWide =>
                inputProjectId
                ?? throw new PluginMisconfigurationException("Please specify the project ID in the input"),

            ConnectionTypes.ProjectWide =>
                inputProjectId
                ?? _credentials.Get(CredsNames.ProjectId).Value
                ?? throw new PluginMisconfigurationException("Please specify the project ID in the connection or input"),

            _ => throw new Exception($"Unsupported connection type: {connectionType}")
        };
    }
}