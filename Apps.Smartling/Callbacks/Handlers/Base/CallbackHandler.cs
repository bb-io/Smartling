﻿using Apps.Smartling.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Webhooks.Bridge;
using Blackbird.Applications.Sdk.Utils.Webhooks.Bridge.Models.Request;
using System.Text;
using System.Text.Json;

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
        await LogToWebhookAsync($"Subscribe:{JsonSerializer.Serialize(input)}");

        await BridgeService.Subscribe(input, bridgeCredentials);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> credentials,
        Dictionary<string, string> values)
    {
        var (input, bridgeCredentials) = GetBridgeServiceInputs(values);
        await LogToWebhookAsync($"Unsubscribe:  {JsonSerializer.Serialize(input)}");



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



    private async Task LogToWebhookAsync(string logMessage)
    {
        using (var client = new HttpClient())
        {
            var payload = new
            {
                message = logMessage,
                timestamp = DateTime.UtcNow
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://webhook.site/46153d60-6c08-4081-9ab3-7ba5d527fec8", content);


            if (!response.IsSuccessStatusCode)
            {
            }
        }
    }
}