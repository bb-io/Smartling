using Apps.Smartling.Api;
using Apps.Smartling.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Smartling.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
    {
        SmartlingClient client;
        
        try
        {
            client = new SmartlingClient(authProviders);
        }
        catch (Exception exception)
        {
            return new()
            {
                IsValid = false,
                Message = exception.Message
            };
        }

        var connectionType = authProviders.Get(CredsNames.ConnectionType).Value;
        string endpoint = string.Empty;

        switch (connectionType)
        {
            case ConnectionTypes.AccountWide:
                string accountUid = authProviders.Get(CredsNames.AccountUid).Value;
                endpoint = $"/accounts-api/v2/accounts/{accountUid}/projects";
                break;
            case ConnectionTypes.ProjectWide:
                string projectId = authProviders.Get(CredsNames.ProjectId).Value;
                endpoint = $"/projects-api/v2/projects/{projectId}";
                break;
        }

        var request = new SmartlingRequest(endpoint, Method.Get);

        try
        {
            await client.ExecuteWithErrorHandling(request);
            return new()
            {
                IsValid = true
            };
        }
        catch
        {
            return new()
            {
                IsValid = false,
                Message = "Invalid project ID."
            };
        }
    }
}