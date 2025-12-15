using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos.Project;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

// This data source handler is responsible for retrieving target locales configured for a project
public class TargetLocaleDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public TargetLocaleDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new SmartlingRequest($"/projects-api/v2/projects/{ProjectId}", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDtoWithTargetLocales>>(request);
        var targetLocales = response.Response.Data.TargetLocales
            .Where(locale => context.SearchString == null 
                             || locale.Description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(locale => locale.LocaleId, locale => locale.Description);
        return targetLocales;
    }
}