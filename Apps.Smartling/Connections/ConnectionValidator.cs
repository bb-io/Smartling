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
        
        var projectId = authProviders.Get(CredsNames.ProjectId).Value;
        var request = new SmartlingRequest($"/projects-api/v2/projects/{projectId}", Method.Get);

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