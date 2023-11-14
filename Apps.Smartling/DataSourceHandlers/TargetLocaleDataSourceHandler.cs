using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class TargetLocaleDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public TargetLocaleDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new SmartlingRequest($"/projects-api/v2/projects/{ProjectId}", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDto>>(request);
        var targetLocales = response.Response.Data.TargetLocales
            .Where(locale => context.SearchString == null || locale.Description.Contains(context.SearchString))
            .ToDictionary(locale => locale.LocaleId, locale => locale.Description);
        return targetLocales;
    }
}