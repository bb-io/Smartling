using RestSharp;
using Apps.Smartling.Api;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Dtos.Project;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.DataSourceHandlers;

// This data source handler is responsible for retrieving target locales configured for a project
public class TargetLocaleDataSourceHandler(
    InvocationContext invocationContext,
    [ActionParameter] ProjectIdentifier project) 
    : SmartlingInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest($"/projects-api/v2/projects/{projectId}", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDtoWithTargetLocales>>(request);

        var targetLocales = response.Response.Data.TargetLocales
            .Where(
                locale => context.SearchString == null ||
                locale.Description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)
            );
        return targetLocales.Select(x => new DataSourceItem(x.LocaleId, x.Description));
    }
}