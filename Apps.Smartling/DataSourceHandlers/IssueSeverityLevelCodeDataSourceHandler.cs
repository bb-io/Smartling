using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Issues;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class IssueSeverityLevelCodeDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public IssueSeverityLevelCodeDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new SmartlingRequest("/issues-api/v2/dictionary/issue-severity-levels", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<IssueSeverityLevelDto>>>(request);
        var severityLevels = response.Response.Data.Items
            .Where(level => context.SearchString == null || level.Description.Contains(context.SearchString))
            .ToDictionary(level => level.IssueSeverityLevelCode, level => level.Description);
        return severityLevels;
    }
}