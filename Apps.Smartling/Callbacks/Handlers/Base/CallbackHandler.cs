using Apps.Smartling.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Webhooks.Bridge;
using Blackbird.Applications.Sdk.Utils.Webhooks.Bridge.Models.Request;

namespace Apps.Smartling.Callbacks.Handlers.Base;

public abstract class CallbackHandler : SmartlingInvocable, IWebhookEventHandler
{
    protected abstract string Event { get; }
    
    protected CallbackHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> credentials, 
        Dictionary<string, string> values)
    {
        var (input, bridgeCredentials) = GetBridgeServiceInputs(values);
        await BridgeService.Subscribe(input, bridgeCredentials);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> credentials,
        Dictionary<string, string> values)
    {
        var (input, bridgeCredentials) = GetBridgeServiceInputs(values);
        await BridgeService.Unsubscribe(input, bridgeCredentials);
    }
    
    private (BridgeRequest webhookData, BridgeCredentials bridgeCreds) GetBridgeServiceInputs(
        Dictionary<string, string> values)
    {
        var webhookData = new BridgeRequest
        {
            Event = Event,
            Id = ProjectId,
            Url = values["payloadUrl"]
        };

        var bridgeCredentials = new BridgeCredentials
        {
            ServiceUrl = $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}{ApplicationConstants.SmartlingBridgePath}",
            Token = ApplicationConstants.BlackbirdToken
        };

        return (webhookData, bridgeCredentials);
    }
}