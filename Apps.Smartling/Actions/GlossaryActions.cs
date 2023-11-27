using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos.Glossaries;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Glossaries;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList]
public class GlossaryActions : SmartlingInvocable
{
    private readonly string _accountUid;
    
    public GlossaryActions(InvocationContext invocationContext) : base(invocationContext)
    {
        _accountUid = GetAccountUid().Result;
    }
    
    #region Glossaries

    #region Get

    [Action("Get glossary", Description = "Retrieve detailed information about a single glossary.")]
    public async Task<GlossaryDto> GetGlossary([ActionParameter] GlossaryIdentifier glossaryIdentifier)
        => await GetGlossaryAsync(glossaryIdentifier);

    #endregion

    #region Post
    
    [Action("Create glossary", Description = "Create a new glossary.")]
    public async Task<GlossaryDto> CreateGlossary([ActionParameter] CreateGlossaryRequest input)
    {
        var request = new SmartlingRequest($"/glossary-api/v3/accounts/{_accountUid}/glossaries", Method.Post);
        request.AddJsonBody(new
        {
            glossaryName = input.GlossaryName,
            description = input.Description,
            localeIds = input.LocaleIds
        });
        
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryDto>>(request);
        var glossary = response.Response.Data;
        return glossary;
    }

    #endregion

    #region Put

    [Action("Update glossary", Description = "Update an existing glossary.")]
    public async Task<GlossaryDto> UpdateGlossary([ActionParameter] GlossaryIdentifier glossaryIdentifier, 
        [ActionParameter] UpdateGlossaryRequest input)
    {
        var existingGlossary = await GetGlossaryAsync(glossaryIdentifier);
        var requestBody = new
        {
            glossaryName = input.GlossaryName ?? existingGlossary.GlossaryName,
            description = input.Description ?? existingGlossary.Description,
            localeIds = input.LocaleIds ?? existingGlossary.LocaleIds,
            verificationMode = existingGlossary.VerificationMode,
            fallbackLocales = existingGlossary.FallbackLocales
        };
        var updatedGlossary = await UpdateGlossaryAsync(glossaryIdentifier, requestBody);
        return updatedGlossary;
    }

    [Action("Add locales to glossary", Description = "Update a glossary's list of locales with new locales.")]
    public async Task<GlossaryDto> AddLocalesToGlossary([ActionParameter] GlossaryIdentifier glossaryIdentifier,
        [ActionParameter] GlossaryLocalesIdentifier locales)
    {
        var existingGlossary = await GetGlossaryAsync(glossaryIdentifier);
        var requestBody = new
        {
            glossaryName = existingGlossary.GlossaryName,
            description = existingGlossary.Description,
            localeIds = existingGlossary.LocaleIds.Union(locales.LocaleIds),
            verificationMode = existingGlossary.VerificationMode,
            fallbackLocales = existingGlossary.FallbackLocales
        };
        var updatedGlossary = await UpdateGlossaryAsync(glossaryIdentifier, requestBody);
        return updatedGlossary;
    }

    [Action("Add fallback locale to glossary", Description = "Add a fallback locale to a glossary.")]
    public async Task<GlossaryDto> AddFallbackLocaleToGlossary([ActionParameter] GlossaryIdentifier glossaryIdentifier,
        [ActionParameter] AddFallbackLocaleToGlossaryRequest input)
    {
        var existingGlossary = await GetGlossaryAsync(glossaryIdentifier);
        
        var fallbackLocales = existingGlossary.FallbackLocales.ToList();
        fallbackLocales.Add(new FallbackLocaleDto
            { FallbackLocaleId = input.FallbackLocaleId, LocaleIds = input.LocaleIds });

        var requestBody = new
        {
            glossaryName = existingGlossary.GlossaryName,
            description = existingGlossary.Description,
            localeIds = existingGlossary.LocaleIds,
            verificationMode = existingGlossary.VerificationMode,
            fallbackLocales = fallbackLocales
        };
        
        var updatedGlossary = await UpdateGlossaryAsync(glossaryIdentifier, requestBody);
        return updatedGlossary;
    }

    #endregion

    private async Task<GlossaryDto> GetGlossaryAsync(GlossaryIdentifier glossaryIdentifier)
    {
        var request =
            new SmartlingRequest($"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}",
                Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryDto>>(request);
        var glossary = response.Response.Data;
        return glossary;
    }

    private async Task<GlossaryDto> UpdateGlossaryAsync(GlossaryIdentifier glossaryIdentifier, object requestBody)
    {
        var updateGlossaryRequest =
            new SmartlingRequest($"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}",
                Method.Put);
        updateGlossaryRequest.AddJsonBody(requestBody);

        var updateGlossaryResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryDto>>(updateGlossaryRequest);
        var updatedGlossary = updateGlossaryResponse.Response.Data;
        return updatedGlossary;
    }
    
    #endregion

    #region Glossary entries

    #region Post
    
    [Action("Create glossary entry", Description = "Create a new entry in a glossary.")]
    public async Task<GlossaryEntryDto> CreateGlossaryEntry([ActionParameter] GlossaryIdentifier glossaryIdentifier, 
        [ActionParameter] CreateGlossaryEntryRequest input)
    {
        var request =
            new SmartlingRequest(
                $"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}/entries",
                Method.Post);
        request.AddJsonBody(new
        {
            definition = input.Definition,
            partOfSpeech = input.PartOfSpeech,
            labelUids = input.LabelUids,
            translations = new[]
            {
                new
                {
                    localeId = input.TermLocaleId,
                    term = input.Term,
                    notes = input.TermNotes,
                    caseSensitive = input.TermCaseSensitive,
                    exactMatch = input.TermExactMatch,
                    doNotTranslate = input.DoNotTranslate,
                    disabled = input.Disabled,
                    variants = input.TermVariations
                }
            }
        });

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryEntryDto>>(request);
        var entry = response.Response.Data;
        return entry;
    }
    
    #endregion

    #endregion
}