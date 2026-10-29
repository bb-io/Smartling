using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.MachineTranslation;
using Apps.Smartling.Models.Requests.MachineTranslation;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.MachineTranslation;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList("Machine translation")]
public class MachineTranslationActions : SmartlingInvocable
{
    public MachineTranslationActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Apply machine translation to strings", Description = "Translate up to 1000 strings using Smartling's synchronous MT API.")]
    public async Task<ApplyMachineTranslationResponse> ApplyMachineTranslationToStrings(
        [ActionParameter] ApplyMachineTranslationRequest input)
    {
        var (body, keys) = CreateRequestBody(input);
        var accountUid = await GetAccountUid();
        if (string.IsNullOrWhiteSpace(accountUid))
            throw new PluginMisconfigurationException(
                "Could not resolve the Account UID. Use an account-wide connection with an Account UID, or a project-wide connection with access to project details.");

        var request = new SmartlingRequest($"/mt-router-api/v2/accounts/{accountUid}/smartling-mt", Method.Post);
        request.AddJsonBody(body);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<MachineTranslationApiItemDto>>>(request);
        return MapResponse(keys, response.Response.Data.Items);
    }

    internal static (Dictionary<string, object> Body, string[] Keys) CreateRequestBody(ApplyMachineTranslationRequest input)
    {
        var sources = input.SourceStrings?.ToArray() ?? [];
        var keys = input.Keys?.ToArray() ?? [];
        if (sources.Length != keys.Length)
            throw new PluginMisconfigurationException("Source strings and keys must have the same number of elements. Provide one key per source string.");
        if (sources.Length == 0 || sources.Length > 1000)
            throw new PluginMisconfigurationException("Provide between 1 and 1000 source strings and matching keys per request.");
        if (keys.Any(string.IsNullOrWhiteSpace))
            throw new PluginMisconfigurationException("Keys must not be empty or whitespace. Provide a non-empty key for every source string.");
        if (keys.Distinct(StringComparer.Ordinal).Count() != keys.Length)
            throw new PluginMisconfigurationException("Keys must be unique. Remove duplicate keys before translating.");
        if (string.IsNullOrWhiteSpace(input.SourceLocaleId) || string.IsNullOrWhiteSpace(input.TargetLocaleId))
            throw new PluginMisconfigurationException("Provide both source locale ID and target locale ID, for example en-US and fr-FR.");

        var body = new Dictionary<string, object>
        {
            ["sourceLocaleId"] = input.SourceLocaleId,
            ["targetLocaleId"] = input.TargetLocaleId,
            ["items"] = keys.Select((key, index) => new { key, sourceText = sources[index] }).ToArray()
        };
        if (!string.IsNullOrWhiteSpace(input.ProfileUid))
            body["profileUid"] = input.ProfileUid;
        return (body, keys);
    }

    internal static ApplyMachineTranslationResponse MapResponse(string[] keys, IEnumerable<MachineTranslationApiItemDto>? apiItems)
    {
        var byKey = (apiItems ?? []).ToDictionary(item => item.Key, StringComparer.Ordinal);
        var items = keys.Select(key =>
        {
            if (!byKey.TryGetValue(key, out var item))
                return new MachineTranslationItemDto
                {
                    Key = key,
                    Error = "Smartling did not return a result for this key. Retry this item."
                };

            return new MachineTranslationItemDto
            {
                Key = key,
                TranslationText = item.TranslationText,
                MtUid = item.MtUid,
                Provider = item.Provider,
                Error = item.Error?.ToString(Formatting.None)
            };
        }).ToArray();
        return new ApplyMachineTranslationResponse
        {
            Items = items,
            TranslatedStrings = items.Select(item => item.TranslationText ?? string.Empty).ToArray()
        };
    }
}
