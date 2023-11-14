using Apps.Smartling.Api;
using Apps.Smartling.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

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
}