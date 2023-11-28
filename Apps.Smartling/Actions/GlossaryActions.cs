using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Glossaries;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Glossaries;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Glossaries;
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

    [Action("Update glossary", Description = "Update an existing glossary. Specify only fields that need to be updated.")]
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

    #region Get

    [Action("Get glossary entry", Description = "Retrieve a glossary entry.")]
    public async Task<GlossaryEntryDto> GetGlossaryEntry([ActionParameter] GlossaryIdentifier glossaryIdentifier,
        [ActionParameter] GlossaryEntryIdentifier glossaryEntryIdentifier)
        => await GetGlossaryEntryAsync(glossaryIdentifier, glossaryEntryIdentifier);

    [Action("Search glossary entries", Description = "List glossary entries that match the specified filter options. " +
                                                     "If no parameters are specified, all glossary entries will be returned.")]
    public async Task<SearchGlossaryEntriesResponse> SearchGlossaryEntries(
        [ActionParameter] GlossaryIdentifier glossaryIdentifier,
        [ActionParameter] SearchGlossaryEntriesRequest input)
    {
        var locales = input.LocaleIds;

        if (locales == null)
        {
            var getGlossaryRequest =
                new SmartlingRequest($"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}",
                    Method.Get);
            var getGlossaryResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryDto>>(getGlossaryRequest);
            locales = getGlossaryResponse.Response.Data.LocaleIds;
        }

        object createdFilter;
        var after = input.GlossaryEntryCreatedAfter;
        var before = input.GlossaryEntryCreatedBefore;

        switch (after, before)
        {
            case (null, null):
                createdFilter = null;
                break;

            case var dates when dates.after != null && dates.before != null:
                createdFilter = new
                {
                    type = "date_range",
                    level = "ANY",
                    from = dates.after,
                    to = dates.before
                };
                break;

            case var dates when dates.after != null:
                createdFilter = new
                {
                    type = "after",
                    level = "ANY",
                    date = dates.after
                };
                break;

            default:
                createdFilter = new
                {
                    type = "before",
                    level = "ANY",
                    date = before
                };
                break;
        }

        var searchEntriesRequest =
            new SmartlingRequest(
                $"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}/entries/search",
                Method.Post);
        searchEntriesRequest.AddJsonBody(new
        {
            query = input.Query,
            localeIds = locales,
            entryState = input.EntryState,
            missingTranslationLocaleId = input.MissingTranslationLocaleId,
            presentTranslationLocaleId = input.PresentTranslationLocaleId,
            dntLocaleId = input.DntLocaleId,
            returnFallbackTranslations = input.ReturnFallbackTranslations,
            labels = input.LabelIds == null
                ? null
                : new
                {
                    type = "associated",
                    labelUids = input.LabelIds
                },
            dntTermSet = input.DntTermSet,
            created = createdFilter
        });

        var searchEntriesResponse =
            await Client
                .ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<GlossaryEntryDto>>>(searchEntriesRequest);
        var entries = searchEntriesResponse.Response.Data.Items;
        return new(entries);
    }

    #endregion
    
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

    #region Put

    [Action("Update glossary entry", Description = "Update an existing glossary entry. Specify only fields that need " +
                                                   "to be updated.")]
    public async Task<GlossaryEntryDto> UpdateGlossaryEntry([ActionParameter] GlossaryIdentifier glossaryIdentifier,
        [ActionParameter] GlossaryEntryIdentifier glossaryEntryIdentifier, 
        [ActionParameter] UpdateGlossaryEntryRequest input)
    {
        var entry = await GetGlossaryEntryAsync(glossaryIdentifier, glossaryEntryIdentifier);
        var request =
            new SmartlingRequest(
                $"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}/entries/{glossaryEntryIdentifier.EntryUid}",
                Method.Put);
        request.AddJsonBody(new
        {
            definition = input.Definition ?? entry.Definition,
            partOfSpeech = input.PartOfSpeech ?? entry.PartOfSpeech,
            labelUids = input.LabelUids ?? entry.LabelUids,
            translations = entry.Translations.Select(translation => new
            {
                localeId = translation.LocaleId,
                term = translation.Term,
                notes = translation.Notes,
                caseSensitive = translation.CaseSensitive,
                exactMatch = translation.ExactMatch,
                doNotTranslate = translation.DoNotTranslate,
                disabled = translation.Disabled,
                variants = translation.Variants,
                customFieldValues = translation.CustomFieldValues.Select(value => new
                {
                    fieldUid = value.FieldUid,
                    fieldValue = value.FieldValue
                })
            }),
            customFieldValues = entry.CustomFieldValues.Select(value => new
            {
                fieldUid = value.FieldUid,
                fieldValue = value.FieldValue
            })
        });

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryEntryDto>>(request);
        var updatedEntry = response.Response.Data;
        return updatedEntry;
    }
    
    [Action("Add glossary entry translation", Description = "Add a translation for a glossary entry.")]
    public async Task<GlossaryEntryDto> AddGlossaryEntryTranslation(
        [ActionParameter] GlossaryIdentifier glossaryIdentifier,
        [ActionParameter] GlossaryEntryIdentifier glossaryEntryIdentifier, 
        [ActionParameter] AddGlossaryEntryTranslationRequest input)
    {
        var entry = await GetGlossaryEntryAsync(glossaryIdentifier, glossaryEntryIdentifier);
        var request =
            new SmartlingRequest(
                $"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}/entries/{glossaryEntryIdentifier.EntryUid}",
                Method.Put);

        var translations = entry.Translations.ToList();
        var targetTranslationIndex = translations.FindIndex(translation => translation.LocaleId == input.TermLocaleId);

        if (targetTranslationIndex != -1)
        {
            translations[targetTranslationIndex].Term = input.Term;
            translations[targetTranslationIndex].CaseSensitive = input.TermCaseSensitive ?? translations[targetTranslationIndex].CaseSensitive;
            translations[targetTranslationIndex].ExactMatch = input.TermExactMatch ?? translations[targetTranslationIndex].ExactMatch;
            translations[targetTranslationIndex].DoNotTranslate = input.DoNotTranslate ?? translations[targetTranslationIndex].DoNotTranslate;
            translations[targetTranslationIndex].Notes = input.TermNotes ?? translations[targetTranslationIndex].Notes;
            translations[targetTranslationIndex].Disabled = input.Disabled ?? translations[targetTranslationIndex].Disabled;
            translations[targetTranslationIndex].Variants = input.TermVariations ?? translations[targetTranslationIndex].Variants;
        }
        else
        {
            translations.Add(new GlossaryEntryTranslationDto
            {
                LocaleId = input.TermLocaleId,
                Term = input.Term,
                Notes = input.TermNotes,
                CaseSensitive = input.TermCaseSensitive ?? false,
                ExactMatch = input.TermExactMatch ?? false,
                DoNotTranslate = input.DoNotTranslate ?? false,
                Disabled = input.Disabled ?? false,
                Variants = input.TermVariations ?? new string[] { },
                CustomFieldValues = new GlossaryEntryTranslationCustomFieldDto[] { }
            });
        }
        
        request.AddJsonBody(new
        {
            definition = entry.Definition,
            partOfSpeech = entry.PartOfSpeech,
            labelUids = entry.LabelUids,
            translations = translations.Select(translation => new
            {
                localeId = translation.LocaleId,
                term = translation.Term,
                notes = translation.Notes,
                caseSensitive = translation.CaseSensitive,
                exactMatch = translation.ExactMatch,
                doNotTranslate = translation.DoNotTranslate,
                disabled = translation.Disabled,
                variants = translation.Variants,
                customFieldValues = translation.CustomFieldValues.Select(value => new
                {
                    fieldUid = value.FieldUid,
                    fieldValue = value.FieldValue
                })
            }),
            customFieldValues = entry.CustomFieldValues.Select(value => new
            {
                fieldUid = value.FieldUid,
                fieldValue = value.FieldValue
            })
        });

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryEntryDto>>(request);
        var updatedEntry = response.Response.Data;
        return updatedEntry;
    }

    #endregion

    #region Delete

    [Action("Remove glossary entry", Description = "Remove a glossary entry.")]
    public async Task RemoveGlossaryEntry([ActionParameter] GlossaryIdentifier glossaryIdentifier,
        [ActionParameter] GlossaryEntryIdentifier glossaryEntryIdentifier)
    {
        var request =
            new SmartlingRequest(
                $"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}/entries/delete",
                Method.Post);
        request.AddJsonBody(new
        {
            filter = new
            {
                entryUids = new[] { glossaryEntryIdentifier.EntryUid }
            }
        });
        
        await Client.ExecuteWithErrorHandling(request);
    }

    #endregion
    
    private async Task<GlossaryEntryDto> GetGlossaryEntryAsync(GlossaryIdentifier glossaryIdentifier,
        GlossaryEntryIdentifier glossaryEntryIdentifier)
    {
        var request =
            new SmartlingRequest(
                $"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}/entries/{glossaryEntryIdentifier.EntryUid}",
                Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<GlossaryEntryDto>>(request);
        var entry = response.Response.Data;
        return entry;
    }

    #endregion
}