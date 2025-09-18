using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Dtos.Locales;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class WorkflowSingleLanguageHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    private readonly TargetLocaleIdentifier _targetLocale;
    
    public WorkflowSingleLanguageHandler(InvocationContext invocationContext,
        [ActionParameter] TargetLocaleIdentifier targetLocale) : base(invocationContext)
    {
        _targetLocale = targetLocale;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (_targetLocale.TargetLocaleId == null )
            throw new PluginMisconfigurationException("Please enter target locale first.");
        
        var getProjectRequest = new SmartlingRequest($"/projects-api/v2/projects/{ProjectId}", Method.Get);
        var getProjectResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDto>>(getProjectRequest);
        var project = getProjectResponse.Response.Data;

        var getWorkflowsRequest = new SmartlingRequest($"/workflows-api/v3/projects/{ProjectId}/authorization/workflows", 
            Method.Post);

        var localePairs = new List<SourceTargetLocalesPairDto>();
        var sourceLocale = new SourceLocaleIdDto(project.SourceLocaleId);
        var targetLocale = new TargetLocaleIdDto(_targetLocale.TargetLocaleId);
        localePairs.Add(new(sourceLocale, targetLocale));

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