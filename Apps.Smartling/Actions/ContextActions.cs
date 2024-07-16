using Apps.Smartling.Api;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using Apps.Smartling.Models.Requests.Context;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Apps.Smartling.Models.Dtos.Contexts;

namespace Apps.Smartling.Actions;

[ActionList]
public class ContextActions : SmartlingInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public ContextActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Add project context", Description = "Add project context")]
    public async Task AddProjectContext([ActionParameter] AddProjectContextRequest addProjectContext)
    {
        var uploadProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{ProjectId}/contexts", Method.Post);
        uploadProjectContextRequest.AlwaysMultipartFormData = true;
        if(!string.IsNullOrEmpty(addProjectContext.Name))
            uploadProjectContextRequest.AddParameter("name", addProjectContext.Name);

        await using var contextFileStream = await _fileManagementClient.DownloadAsync(addProjectContext.ContextFile);
        uploadProjectContextRequest.AddFile("content", await contextFileStream.GetByteData(), addProjectContext.ContextFile.Name);
        var uploadProjectContextResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectContextDto>>(uploadProjectContextRequest);
        
        if(addProjectContext.ContextMatching.HasValue && addProjectContext.ContextMatching.Value)
        {
            var runContextMatchingRequest = new SmartlingRequest($"/context-api/v2/projects/{ProjectId}/contexts/{uploadProjectContextResponse.Response.Data.ContextUid}/match/async", Method.Post);
            runContextMatchingRequest.AddJsonBody(new {});
            await Client.ExecuteAsyncProcessWithHandling(runContextMatchingRequest, ProjectId);
        }
    }

    [Action("Search project context", Description = "Search project context")]
    public async Task<List<ProjectContextDto>> SearchProjectContext([ActionParameter] SearchProjectContextRequest searchProjectContext)
    {
        var searchProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{ProjectId}/contexts", Method.Get);
        if (!string.IsNullOrEmpty(searchProjectContext.Name))
            searchProjectContextRequest.AddQueryParameter("nameFilter", searchProjectContext.Name);
        if (!string.IsNullOrEmpty(searchProjectContext.Type))
            searchProjectContextRequest.AddQueryParameter("type", searchProjectContext.Type);

        var searchProjectContextResponse =
            await Client.Paginate<ProjectContextDto>(searchProjectContextRequest);
        return searchProjectContextResponse;
    }
}
