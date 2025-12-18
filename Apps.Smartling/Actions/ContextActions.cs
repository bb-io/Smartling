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
using Blackbird.Applications.Sdk.Common.Files;
using Apps.Smartling.Models.Responses.Context;
using Apps.Smartling.Models.Identifiers;

namespace Apps.Smartling.Actions;

[ActionList("Context")]
public class ContextActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : SmartlingInvocable(invocationContext)
{
    [Action("Add project context", Description = "Add project context")]
    public async Task AddProjectContext(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] AddProjectContextRequest addProjectContext)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var uploadProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/contexts", Method.Post);
        uploadProjectContextRequest.AlwaysMultipartFormData = true;
        if (!string.IsNullOrEmpty(addProjectContext.Name))
            uploadProjectContextRequest.AddParameter("name", addProjectContext.Name);

        await using var contextFileStream = await fileManagementClient.DownloadAsync(addProjectContext.ContextFile);
        uploadProjectContextRequest.AddFile("content", await contextFileStream.GetByteData(), addProjectContext.ContextFile.Name);
        var uploadProjectContextResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectContextDto>>(uploadProjectContextRequest);
        
        if (addProjectContext.ContextMatching.HasValue && addProjectContext.ContextMatching.Value)
        {
            var runContextMatchingRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/contexts/{uploadProjectContextResponse.Response.Data.ContextUid}/match/async", Method.Post);
            runContextMatchingRequest.AddJsonBody(new {});
            await Client.ExecuteAsyncProcessWithHandling(runContextMatchingRequest, projectId);
        }
    }

    [Action("Search project context", Description = "Search project context")]
    public async Task<List<ProjectContextDto>> SearchProjectContext(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] SearchProjectContextRequest searchProjectContext)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var searchProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/contexts", Method.Get);
        if (!string.IsNullOrEmpty(searchProjectContext.Name))
            searchProjectContextRequest.AddQueryParameter("nameFilter", searchProjectContext.Name);
        if (!string.IsNullOrEmpty(searchProjectContext.Type))
            searchProjectContextRequest.AddQueryParameter("type", searchProjectContext.Type);

        var searchProjectContextResponse =
            await Client.Paginate<ProjectContextDto>(searchProjectContextRequest);
        return searchProjectContextResponse;
    }

    [Action("Download project context", Description = "Download project context")]
    public async Task<FileReference> DownloadProjectContext(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] GetProjectContextRequest getProjectContext)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var downloadProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/contexts/{getProjectContext.ContextUid}/content", Method.Get);
        
        var downloadProjectContextResponse = await Client.ExecuteAsync(downloadProjectContextRequest);
        using var contextStream = new MemoryStream(downloadProjectContextResponse.RawBytes); 
        
        var getProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/contexts/{getProjectContext.ContextUid}", Method.Get);
        var getProjectContextResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectContextDto>>(getProjectContextRequest);

        var projectContextFileReference =
           await fileManagementClient.UploadAsync(contextStream, downloadProjectContextResponse.ContentType, getProjectContextResponse.Response.Data.Name);
        return projectContextFileReference;
    }

    [Action("Delete project context", Description = "Delete project context")]
    public async Task DeleteProjectContext(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] GetProjectContextRequest getProjectContext)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var deleteProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/contexts/{getProjectContext.ContextUid}", Method.Delete);
        await Client.ExecuteWithErrorHandling(deleteProjectContextRequest);
    }

    [Action("Upload new context", Description = "Upload new context (with optional automatic matching)")]
    public async Task<ResponseWrapper<UploadContextResponseDto>> UploadNewContext(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] AddProjectContextRequest request)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var endpoint = request.ContextMatching == true
            ? $"/context-api/v2/projects/{projectId}/contexts/upload-and-match-async"
            : $"/context-api/v2/projects/{projectId}/contexts";

        var uploadRequest = new SmartlingRequest(endpoint, Method.Post)
        {
            AlwaysMultipartFormData = true
        };

        if (!string.IsNullOrEmpty(request.Name))
            uploadRequest.AddParameter("name", request.Name);

        await using var contextFileStream = await fileManagementClient.DownloadAsync(request.ContextFile);
        uploadRequest.AddFile("content",
            await contextFileStream.GetByteData(),
            request.ContextFile.Name);

        if (request.ContextMatching == true)
        {
            var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<UploadAndMatchDto>>(uploadRequest);

            return new ResponseWrapper<UploadContextResponseDto>(
             new ResponseData<UploadContextResponseDto>(
                 response.Response.Code,
                 new UploadContextResponseDto
                 {
                     ProcessUid = response.Response.Data.ProcessUid
                 }
             )
         );
        }
        else
        {
            var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectContextDto>>(uploadRequest);

            return new ResponseWrapper<UploadContextResponseDto>(
            new ResponseData<UploadContextResponseDto>(
                response.Response.Code,
                new UploadContextResponseDto
                {
                    ContextUid = response.Response.Data.ContextUid
                }
            )
        );
        }
    }

    [Action("Link context to string", Description = "Link context to string")]
    public async Task<ResponseWrapper<LinkContextResponseDto>> LinkContextToString(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] SimpleLinkContextRequest request)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var apiRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/bindings", Method.Post);

        var body = new
        {
            bindings = new[]
            {
                    new
                    {
                        contextUid = request.ContextUid,
                        stringHashcode = request.StringHashcode
                    }
                }
        };

        apiRequest.AddJsonBody(body);

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<LinkContextResponseDto>>(apiRequest);
        return response;
    }
}