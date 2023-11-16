using Apps.Smartling.Api;
using Apps.Smartling.Models;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Attachments;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Attachments;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList]
public class AttachmentActions : SmartlingInvocable
{
    public AttachmentActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("List files attached to job", Description = "Retrieve a list of files attached to a job.")]
    public async Task<ListAttachmentsResponse> ListFilesAttachedToJob([ActionParameter] JobIdentifier jobIdentifier)
    {
        var accountUid = await GetAccountUid();
        var getAttachmentsRequest = 
            new SmartlingRequest($"/attachments-api/v2/accounts/{accountUid}/jobs/{jobIdentifier.TranslationJobUid}", 
                Method.Get);
        var getAttachmentsResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<AttachmentDto>>>(getAttachmentsRequest);
        var attachments = getAttachmentsResponse.Response.Data.Items;
        return new ListAttachmentsResponse(attachments);
    }

    [Action("Upload attachment to job", Description = "Upload attachment file to a job.")]
    public async Task<AttachmentDto> AddAttachmentToJob([ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] FileWrapper file, [ActionParameter] [Display("Attachment description")] string? description)
    {
        var accountUid = await GetAccountUid();
        var request = new SmartlingRequest($"/attachments-api/v2/accounts/{accountUid}/jobs/attachments", Method.Post);
        request.AddFile("file", file.File.Bytes, file.File.Name);
        request.AddParameter("name", file.File.Name);
        request.AddParameter("entityUids", jobIdentifier.TranslationJobUid);

        if (description != null)
            request.AddParameter("description", description);

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<AttachmentDto>>(request);
        var attachment = response.Response.Data;
        return attachment;
    }

    [Action("Download file attached to job", Description = "Download file attached to a job.")]
    public async Task<FileWrapper> DownloadJobAttachment([ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] JobAttachmentIdentifier jobAttachmentIdentifier)
    {
        var accountUid = await GetAccountUid();
        var request =
            new SmartlingRequest(
                $"/attachments-api/v2/accounts/{accountUid}/jobs/attachments/{jobAttachmentIdentifier.AttachmentUid}",
                Method.Get);
        var response = await Client.ExecuteWithErrorHandling(request);
        
        var filename = response.ContentHeaders.First(header => header.Name == "Content-Disposition").Value.ToString()
            .Split('\'')[^1];
        var contentType = response.ContentType.Split(';')[0];
        return new FileWrapper { File = new(response.RawBytes) { ContentType = contentType, Name = filename } };
    }

    private async Task<string> GetAccountUid()
    {
        var getProjectRequest = new SmartlingRequest($"/projects-api/v2/projects/{ProjectId}", Method.Get);
        var getProjectResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDto>>(getProjectRequest);
        var accountUid = getProjectResponse.Response.Data.AccountUid;
        return accountUid;
    }
}