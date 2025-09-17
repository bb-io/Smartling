using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Glossaries;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class GlossaryEntryDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    private readonly GlossaryIdentifier _glossaryIdentifier;
    
    public GlossaryEntryDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] GlossaryIdentifier glossaryIdentifier) : base(invocationContext)
    {
        _glossaryIdentifier = glossaryIdentifier;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (_glossaryIdentifier.GlossaryUid == null)
            throw new PluginMisconfigurationException("Please enter glossary first.");

        var accountUid = await GetAccountUid();
        var getGlossaryRequest =
            new SmartlingRequest($"/glossary-api/v3/accounts/{accountUid}/glossaries/{_glossaryIdentifier.GlossaryUid}",
                Method.Get);
        var getGlossaryResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryDto>>(getGlossaryRequest);
        var locales = getGlossaryResponse.Response.Data.LocaleIds;

        var searchEntriesRequest =
            new SmartlingRequest(
                $"/glossary-api/v3/accounts/{accountUid}/glossaries/{_glossaryIdentifier.GlossaryUid}/entries/search",
                Method.Post);
        searchEntriesRequest.AddJsonBody(new
        {
            query = context.SearchString ?? "",
            localeIds = locales
        });

        var searchEntriesResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<GlossaryEntryDto>>>(searchEntriesRequest);
        
        return searchEntriesResponse.Response.Data.Items
            .ToDictionary(entry => entry.EntryUid,
                entry => entry.Translations.MinBy(translation => translation.CreatedDate).Term);
    }
}