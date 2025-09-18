using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Glossaries;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class FallbackLocaleLocaleDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    private readonly AddFallbackLocaleToGlossaryRequest _addFallbackLocaleToGlossaryRequest;

    public FallbackLocaleLocaleDataSourceHandler(InvocationContext invocationContext, 
        [ActionParameter] AddFallbackLocaleToGlossaryRequest addFallbackLocaleToGlossaryRequest) 
        : base(invocationContext)
    {
        _addFallbackLocaleToGlossaryRequest = addFallbackLocaleToGlossaryRequest;
    }
    
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (_addFallbackLocaleToGlossaryRequest.FallbackLocaleId == null)
            throw new PluginMisconfigurationException("Please enter fallback locale first.");
        
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
            var getProjectResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDto>>(getProjectRequest);
            var projectDto = getProjectResponse.Response.Data;
            locales[projectDto.SourceLocaleId] = projectDto.SourceLocaleDescription;

            foreach (var targetLocale in projectDto.TargetLocales)
            {
                locales[targetLocale.LocaleId] = targetLocale.Description;
            }
        }

        return locales
            .Where(locale => locale.Key != _addFallbackLocaleToGlossaryRequest.FallbackLocaleId)
            .Where(locale => context.SearchString == null 
                             || locale.Value.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(locale => locale.Key, locale => locale.Value);
    }
}