using RestSharp;
using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Smartling.Models.Dtos.Project;

namespace Apps.Smartling.DataSourceHandlers;

public class ProjectDataSourceHandler(InvocationContext context) : SmartlingInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new SmartlingRequest($"/accounts-api/v2/accounts/{GetAccountUid()}/projects", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<ProjectDto>>>(request);
        return response.Response.Data.Items.Select(x => new DataSourceItem(x.ProjectId, x.ProjectName));
    }
}
