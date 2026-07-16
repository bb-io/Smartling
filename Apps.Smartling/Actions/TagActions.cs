using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Tags;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Tags;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Tags;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList("Tags")]
public class TagActions(InvocationContext invocationContext) : SmartlingInvocable(invocationContext)
{
    [Action("Add tags to strings by hashcode", Description = "Add any amount of tags to any strings.")]
    public async Task AddTagsToStrings(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] StringHashcodesIdentifier strings,
        [ActionParameter] AddTagsRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);
        await ExecuteTagRequest(
            $"/tags-api/v2/projects/{projectId}/strings/tags/add",
            new
            {
                tags = input.Tags,
                stringHashcodes = strings.Hashcodes,
            });
    }

    [Action("Remove tags from strings by hashcode", Description = "Remove specified tags from strings.")]
    public async Task RemoveTagsFromStrings(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] StringHashcodesIdentifier strings,
        [ActionParameter] AddTagsRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);
        await ExecuteTagRequest(
            $"/tags-api/v2/projects/{projectId}/strings/tags/remove",
            new
            {
                tags = input.Tags,
                stringHashcodes = strings.Hashcodes,
            });
    }

    [Action("Remove all tags from strings by hashcode", Description = "Remove all tags from strings.")]
    public async Task RemoveAllTagsFromStrings(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] StringHashcodesIdentifier strings)
    {
        string projectId = await GetProjectId(project.ProjectId);
        await ExecuteTagRequest(
            $"/tags-api/v2/projects/{projectId}/strings/tags/remove/all",
            new
            {
                stringHashcodes = strings.Hashcodes,
            });
    }

    [Action("Get all tags for strings by hashcode", Description = "Retrieve all tags linked to strings.")]
    public async Task<ListStringTagsResponse> GetAllTagsForStrings(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] StringHashcodesIdentifier strings)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var response = await ExecuteTagRequest<ResponseWrapper<ItemsWrapper<StringTagsDto>>>(
            $"/tags-api/v2/projects/{projectId}/strings/tags/search",
            new
            {
                stringHashcodes = strings.Hashcodes,
            });

        return new ListStringTagsResponse(response.Response.Data.Items, response.Response.Data.TotalCount);
    }

    private async Task ExecuteTagRequest(string endpoint, object payload)
    {
        var request = new SmartlingRequest(endpoint, Method.Post);
        request.AddJsonBody(payload);
        var response = await Client.ExecuteWithErrorHandling(request);
        ThrowIfTagResponseContainsErrors(response);
    }

    private async Task<T> ExecuteTagRequest<T>(string endpoint, object payload)
    {
        var request = new SmartlingRequest(endpoint, Method.Post);
        request.AddJsonBody(payload);
        var response = await Client.ExecuteWithErrorHandling(request);
        ThrowIfTagResponseContainsErrors(response);

        return JsonConvert.DeserializeObject<T>(response.Content)
               ?? throw new PluginApplicationException($"Could not parse tag response to {typeof(T).Name}");
    }

    private static void ThrowIfTagResponseContainsErrors(RestResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.Content))
        {
            return;
        }

        var json = JObject.Parse(response.Content);
        var errors = json["response"]?["errors"]?.ToObject<List<JToken>>();

        if (errors == null || !errors.Any())
        {
            return;
        }

        var errorMessages = string.Join("; ", errors.Select(e => e.ToString()));
        throw new PluginApplicationException(errorMessages);
    }
}
