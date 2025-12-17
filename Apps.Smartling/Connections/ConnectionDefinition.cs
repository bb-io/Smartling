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
            Name = ConnectionTypes.ProjectWide,
            DisplayName = "Project-wide connection",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.ProjectId) { DisplayName = "Project ID" },
                new(CredsNames.UserIdentifier) { DisplayName = "User identifier" },
                new(CredsNames.UserSecret) { DisplayName = "User secret", Sensitive = true }
            }
        },
        new()
        {
            Name = ConnectionTypes.AccountWide,
            DisplayName = "Account-wide connection",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.AccountUid) { DisplayName = "Account UID" },
                new(CredsNames.UserIdentifier) { DisplayName = "User identifier" },
                new(CredsNames.UserSecret) { DisplayName = "User secret", Sensitive = true }
            }
        }
    };
    
    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        var providers = values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value)).ToList();

        var connectionType = values[nameof(ConnectionPropertyGroup)] switch
        {
            var ct when ConnectionTypes.SupportedConnectionTypes.Contains(ct) => ct,
            _ => throw new Exception($"Unknown connection type: {values[nameof(ConnectionPropertyGroup)]}")
        };

        providers.Add(new AuthenticationCredentialsProvider(CredsNames.ConnectionType, connectionType));
        return providers;
    }
}