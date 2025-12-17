using RestSharp;
using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Dtos.Locales;
using Apps.Smartling.Models.Dtos.Project;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.DataSourceHandlers;

public class WorkflowDataSourceHandler(
    InvocationContext context, 
    [ActionParameter] TargetLocalesIdentifier targetLocales,
    [ActionParameter] ProjectIdentifier projectIdentifier) 
    : SmartlingInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        if (targetLocales.TargetLocaleIds == null || !targetLocales.TargetLocaleIds.Any())
            throw new PluginMisconfigurationException("Please enter target locales first.");

        string projectId = await GetProjectId(projectIdentifier.ProjectId);
        var getProjectRequest = new SmartlingRequest($"/projects-api/v2/projects/{projectId}", Method.Get);
        var getProjectResponse = 
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDtoWithTargetLocales>>(getProjectRequest);
        var project = getProjectResponse.Response.Data;

        var getWorkflowsRequest = new SmartlingRequest(
            $"/workflows-api/v3/projects/{projectId}/authorization/workflows",
            Method.Post
        );

        var localePairs = new List<SourceTargetLocalesPairDto>();
        foreach (var locale in targetLocales.TargetLocaleIds)
        {
            var sourceLocale = new SourceLocaleIdDto(project.SourceLocaleId);
            var targetLocale = new TargetLocaleIdDto(locale);
            localePairs.Add(new(sourceLocale, targetLocale));
        }

        getWorkflowsRequest.AddJsonBody(new { localePairs });
        var getWorkflowsResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<WorkflowDto>>>(getWorkflowsRequest);
        var workflows = getWorkflowsResponse.Response.Data.Items
            .Where(
                workflow => context.SearchString == null || 
                workflow.WorkflowName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)
            );

        return workflows.Select(x => new DataSourceItem(x.WorkflowUid, x.WorkflowName));
    }
}