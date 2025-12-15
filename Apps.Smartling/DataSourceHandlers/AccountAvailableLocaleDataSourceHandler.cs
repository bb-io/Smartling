using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Project;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

// This data source handler is responsible for retrieving all languages across all projects in current account
public class AccountAvailableLocaleDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public AccountAvailableLocaleDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var accountUid = await GetAccountUid();
        var getProjectsRequest =
            new SmartlingRequest($"/accounts-api/v2/accounts/{accountUid}/projects", Method.Get);
        var getProjectsResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<ProjectIdentifier>>>(getProjectsRequest);
        var projects = getProjectsResponse.Response.Data.Items;

        var locales = new Dictionary<string, string>();

        foreach (var project in projects)
        {
            var getProjectRequest = new SmartlingRequest($"/projects-api/v2/projects/{project.ProjectId}", Method.Get);
            var getProjectResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDtoWithTargetLocales>>(getProjectRequest);
            var projectDto = getProjectResponse.Response.Data;
            locales[projectDto.SourceLocaleId] = projectDto.SourceLocaleDescription;

            foreach (var targetLocale in projectDto.TargetLocales)
            {
                locales[targetLocale.LocaleId] = targetLocale.Description;
            }
        }

        return locales
            .Where(locale => context.SearchString == null 
                             || locale.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(locale => locale.Key, locale => locale.Value);
    }
}