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

        string endpoint = 
            $"/files-api/v2/projects/{projectId}/files" +
            $"/list?orderBy=lastUploaded_desc&limit=30&uriMask={context.SearchString ?? ""}";        
        var request = new SmartlingRequest(endpoint, Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<UploadedFileDto>>>(request);

        var files = response.Response.Data.Items;
        return files.Select(x => new DataSourceItem(x.FileUri, x.FileUri));
    }
}