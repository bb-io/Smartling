using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Files;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class FileDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public FileDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = 
            new SmartlingRequest(
                $"/files-api/v2/projects/{ProjectId}/files/list?orderBy=lastUploaded_desc&limit=30&uriMask={context.SearchString ?? ""}", 
                Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<UploadedFileDto>>>(request);
        var files = response.Response.Data.Items;
        return files.ToDictionary(file => file.FileUri, file => file.FileUri);
    }
}