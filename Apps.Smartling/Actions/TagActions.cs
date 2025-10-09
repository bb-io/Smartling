using Apps.Smartling.Api;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Tags;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList("Tags")]
public class TagActions(InvocationContext invocationContext) : SmartlingInvocable(invocationContext)
{
    [Action("Add tags to strings by hashcode", Description = "Add any amount of tags to any strings.")]
    public async Task AddTagsToStrings([ActionParameter] StringHashcodesIdentifier strings, [ActionParameter] AddTagsRequest input)
    {
        var request = new SmartlingRequest($"/tags-api/v2/projects/{ProjectId}/strings/tags/add", Method.Post);
        request.AddJsonBody(new
        {
            tags = input.Tags,
            stringHashCodes = strings.Hashcodes,
        });
        await Client.ExecuteWithErrorHandling(request);
    }
}