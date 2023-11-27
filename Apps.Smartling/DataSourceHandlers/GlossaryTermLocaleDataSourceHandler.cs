using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Glossaries;
using Apps.Smartling.Models.Dtos.Locales;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

// This data source handler is responsible for retrieving glossary's locales that are applicable to glossary terms
public class GlossaryTermLocaleDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    private readonly GlossaryIdentifier _glossaryIdentifier;
    
    public GlossaryTermLocaleDataSourceHandler(InvocationContext invocationContext, 
        [ActionParameter] GlossaryIdentifier glossaryIdentifier) : base(invocationContext)
    {
        _glossaryIdentifier = glossaryIdentifier;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, 
        CancellationToken cancellationToken)
    {
        if (_glossaryIdentifier.GlossaryUid == null)
            throw new Exception("Please enter glossary first.");

        var accountUid = await GetAccountUid();
        var getGlossaryRequest =
            new SmartlingRequest($"/glossary-api/v3/accounts/{accountUid}/glossaries/{_glossaryIdentifier.GlossaryUid}",
                Method.Get);
        var getGlossaryResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryDto>>(getGlossaryRequest);
        var localeIds = getGlossaryResponse.Response.Data.LocaleIds;

        var getLocalesRequest =
            new SmartlingRequest($"/locales-api/v2/dictionary/locales?localeIds={string.Join(",", localeIds)}",
                Method.Get);
        var getLocalesResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<LocaleDto>>>(getLocalesRequest);
        
        var glossaryTermLocales = getLocalesResponse.Response.Data.Items
            .Where(locale => context.SearchString == null || locale.Description.Contains(context.SearchString))
            .ToDictionary(locale => locale.LocaleId, locale => locale.Description);
        return glossaryTermLocales;
    }
}