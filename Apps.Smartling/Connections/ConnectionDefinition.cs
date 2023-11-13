using Apps.Smartling.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Smartling.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
    {
        new()
        {
            Name = "API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.ProjectId) { DisplayName = "Project ID" },
                new(CredsNames.UserIdentifier) { DisplayName = "User identifier" },
                new(CredsNames.UserSecret) { DisplayName = "User secret", Sensitive = true }
            }
        }
    };
    
    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        var projectId = values.First(v => v.Key == CredsNames.ProjectId);
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            projectId.Key,
            projectId.Value
        );
        
        var userId = values.First(v => v.Key == CredsNames.UserIdentifier);
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            userId.Key,
            userId.Value
        );
        
        var userSecret = values.First(v => v.Key == CredsNames.UserSecret);
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            userSecret.Key,
            userSecret.Value
        );
    }
}