using RestSharp;
using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Files;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.DataSourceHandlers;

public class FileDataSourceHandler(
    InvocationContext invocationContext, 
    [ActionParameter] ProjectIdentifier project) 
    : SmartlingInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var request = new SmartlingRequest($"/files-api/v2/projects/{projectId}/files/list", Method.Get)
            .AddQueryParameter("orderBy", "lastUploaded_desc")
            .AddQueryParameter("limit", "50")
            .AddQueryParameter("uriMask", context.SearchString ?? "");
        
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<UploadedFileDto>>>(request);

        var files = response.Response.Data.Items;
        return files.Select(x => new DataSourceItem(x.FileUri, x.FileUri));
    }
}