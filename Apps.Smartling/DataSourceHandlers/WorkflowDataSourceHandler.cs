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
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class WorkflowDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    private readonly TargetLocalesIdentifier _targetLocales;
    
    public WorkflowDataSourceHandler(InvocationContext invocationContext, 
        [ActionParameter] TargetLocalesIdentifier targetLocales) : base(invocationContext)
    {
        _targetLocales = targetLocales;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (_targetLocales.TargetLocaleIds == null || !_targetLocales.TargetLocaleIds.Any())
            throw new PluginMisconfigurationException("Please enter target locales first.");
        
        var getProjectRequest = new SmartlingRequest($"/projects-api/v2/projects/{ProjectId}", Method.Get);
        var getProjectResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDtoWithTargetLocales>>(getProjectRequest);
        var project = getProjectResponse.Response.Data;

        var getWorkflowsRequest = new SmartlingRequest($"/workflows-api/v3/projects/{ProjectId}/authorization/workflows", 
            Method.Post);

        var localePairs = new List<SourceTargetLocalesPairDto>();

        foreach (var locale in _targetLocales.TargetLocaleIds)
        {
            var sourceLocale = new SourceLocaleIdDto(project.SourceLocaleId);
            var targetLocale = new TargetLocaleIdDto(locale);
            localePairs.Add(new(sourceLocale, targetLocale));
        }

        getWorkflowsRequest.AddJsonBody(new { localePairs });
        var getWorkflowsResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<WorkflowDto>>>(getWorkflowsRequest);
        var workflows = getWorkflowsResponse.Response.Data.Items;
        
        return workflows
            .Where(workflow => context.SearchString == null 
                               || workflow.WorkflowName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(workflow => workflow.WorkflowUid, workflow => workflow.WorkflowName);
    }
}