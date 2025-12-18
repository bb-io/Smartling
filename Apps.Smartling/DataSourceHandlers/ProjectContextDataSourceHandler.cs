using RestSharp;
using Apps.Smartling.Api;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Dtos.Contexts;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.DataSourceHandlers;

public class ProjectContextDataSourceHandler(
    InvocationContext invocationContext,
    [ActionParameter] ProjectIdentifier project) 
    : SmartlingInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var searchProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/contexts", Method.Get);
        var searchProjectContextResponse = await Client.Paginate<ProjectContextDto>(searchProjectContextRequest);

        var jobs = searchProjectContextResponse
            .Where(
                prContext => context.SearchString == null || 
                prContext.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)
            );
        return jobs.Select(x => new DataSourceItem(x.ContextUid, $"{x.Name} ({x.Created})"));
    }
}
