using System.Net.Mime;
using System.Text.RegularExpressions;
using Apps.Smartling.Api;
using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Glossaries;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Glossaries;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Glossaries;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Glossaries.Utils.Converters;
using Blackbird.Applications.Sdk.Glossaries.Utils.Dtos;
using Blackbird.Applications.Sdk.Glossaries.Utils.Dtos.Enums;
using Blackbird.Applications.Sdk.Glossaries.Utils.Parsers;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList]
public class GlossaryActions : SmartlingInvocable
{
    private readonly string _accountUid;
    private readonly IFileManagementClient _fileManagementClient;

    public GlossaryActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
        : base(invocationContext)
    {
        _accountUid = GetAccountUid().Result;
        _fileManagementClient = fileManagementClient;
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

    [Action("Update glossary",
        Description = "Update an existing glossary. Specify only fields that need to be updated.")]
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
        [ActionParameter] [Display("Locale IDs")] [DataSource(typeof(SmartlingLocaleDataSourceHandler))]
        IEnumerable<string> localeIds)
    {
        var existingGlossary = await GetGlossaryAsync(glossaryIdentifier);
        var requestBody = new
        {
            glossaryName = existingGlossary.GlossaryName,
            description = existingGlossary.Description,
            localeIds = existingGlossary.LocaleIds.Union(localeIds),
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
    
    #region Import/Export

    private const string Term = "Term";
    private const string Variations = "Linguistic Variations";
    private const string Notes = "Notes";
    private const string Id = "ID";
    private const string Definition = "Definition";
    private const string PartOfSpeech = "Part Of Speech";
    private const string CaseSensitive = "Case Sensitive";
    private const string ExactMatch = "Exact Match";
    private const string LabelNames = "Label Names";
    
    [Action("Export glossary", Description = "Export a glossary.")]
    public async Task<GlossaryResponse> ExportGlossary([ActionParameter] GlossaryIdentifier glossaryIdentifier,
        [ActionParameter] ExportGlossaryRequest input)
    {
        var glossary = await GetGlossaryAsync(glossaryIdentifier);
        var locales = input.LocaleIds ?? glossary.LocaleIds;
        
        object? modifiedFilter;
        var after = input.GlossaryEntriesModifiedAfter;
        var before = input.GlossaryEntriesModifiedBefore;

        switch (after, before)
        {
            case (null, null):
                modifiedFilter = null;
                break;

            case var dates when dates.after != null && dates.before != null:
                modifiedFilter = new
                {
                    type = "date_range",
                    level = "ANY",
                    from = dates.after,
                    to = dates.before
                };
                break;

            case var dates when dates.after != null:
                modifiedFilter = new
                {
                    type = "after",
                    level = "ANY",
                    date = dates.after
                };
                break;

            default:
                modifiedFilter = new
                {
                    type = "before",
                    level = "ANY",
                    date = before
                };
                break;
        }
        
        var request =
            new SmartlingRequest(
                $"/glossary-api/v3/accounts/{_accountUid}/glossaries/{glossaryIdentifier.GlossaryUid}/entries/download",
                Method.Post).WithJsonBody(new
            {
                format = "CSV",
                localeIds = locales,
                skipEntries = false,
                filter = new
                {
                    entryState = input.EntriesState ?? "ACTIVE",
                    returnFallbackTranslations = input.ReturnFallbackTranslations ?? false,
                    labels = input.LabelIds == null
                        ? null
                        : new
                        {
                            type = "associated",
                            labelUids = input.LabelIds
                        },
                    lastModified = modifiedFilter,
                    sorting = new
                    {
                        direction = "ASC",
                        field = "term"
                    }
                }
            });

        var response = await Client.ExecuteWithErrorHandling(request);

        await using var csvMemoryStream = new MemoryStream(response.RawBytes);
        var parsedCsv = await csvMemoryStream.ParseCsvFile();
        
        var glossaryConceptEntries = new List<GlossaryConceptEntry>();

        var entriesCount = parsedCsv.First().Value.Count;
        
        for (var i = 0; i < entriesCount; i++)
        {
            string entryId = string.Empty;
            string? entryDefinition = null;
            PartOfSpeech? partOfSpeech = null;
            List<string>? entryNotes = null;
            
            var languageSections = new List<GlossaryLanguageSection>();
        
            foreach (var column in parsedCsv)
            {
                var columnName = column.Key;
                var columnValues = column.Value;
                
                switch (columnName)
                {
                    case Id:
                        entryId = string.IsNullOrWhiteSpace(columnValues[i])
                            ? Guid.NewGuid().ToString()
                            : columnValues[i].Trim();
                        
                        break;
                    
                    case Definition:
                        entryDefinition = string.IsNullOrWhiteSpace(columnValues[i]) ? null : columnValues[i].Trim();
                        break;
                    
                    case PartOfSpeech:
                        if (Enum.TryParse(columnValues[i].Replace(" ", string.Empty),
                                out PartOfSpeech partOfSpeechValue))
                            partOfSpeech = partOfSpeechValue;
                        
                        break;
                    
                    case LabelNames:
                        entryNotes = string.IsNullOrWhiteSpace(columnValues[i])
                            ? null
                            : new List<string>(new[] { $"Labels: {columnValues[i]}" });
                        
                        break;
                    
                    case var languageTerm when new Regex($@"^{Term}.*\[(\w{{2}}-\w{{2}})\]").IsMatch(languageTerm):
                        var languageCode = new Regex($@"^{Term}.*\[(\w{{2}}-\w{{2}})\]").Match(languageTerm).Groups[1]
                            .Value.ToLower();
                        
                        if (!string.IsNullOrWhiteSpace(columnValues[i]))
                            languageSections.Add(new(languageCode,
                                new List<GlossaryTermSection>(new GlossaryTermSection[]
                                    { new(columnValues[i].Trim()) { PartOfSpeech = partOfSpeech } })));
                        
                        break;
                    
                    case var termVariations when new Regex($@"^{Variations}.*\[(\w{{2}}-\w{{2}})\]").IsMatch(termVariations):
                            if (!string.IsNullOrWhiteSpace(columnValues[i]))
                            {
                                languageCode = new Regex($@"^{Variations}.*\[(\w{{2}}-\w{{2}})\]")
                                    .Match(termVariations).Groups[1].Value.ToLower();

                                var targetLanguageSectionIndex =
                                    languageSections.FindIndex(section => section.LanguageCode == languageCode);

                                var terms = columnValues[i]
                                    .Split(',')
                                    .Select(term => new GlossaryTermSection(term.Trim())
                                        { PartOfSpeech = partOfSpeech });

                                if (targetLanguageSectionIndex == -1)
                                    languageSections.Add(new(languageCode, new List<GlossaryTermSection>(terms)));
                                else
                                    languageSections[targetLanguageSectionIndex].Terms.AddRange(terms);
                            }

                            break;

                    case var termNotes when new Regex($@"^{Notes}.*\[(\w{{2}}-\w{{2}})\]").IsMatch(termNotes):
                        if (!string.IsNullOrWhiteSpace(columnValues[i]))
                        {
                            languageCode = new Regex($@"^{Notes}.*\[(\w{{2}}-\w{{2}})\]").Match(termNotes)
                                .Groups[1].Value.ToLower();

                            var targetLanguageSectionIndex =
                                languageSections.FindIndex(section => section.LanguageCode == languageCode);

                            languageSections[targetLanguageSectionIndex].Terms.First().Notes =
                                new List<string>(new[] { columnValues[i] });
                        }

                        break;
                        
                    case var termCaseSensitive when new Regex($@"^{CaseSensitive}.*\[(\w{{2}}-\w{{2}})\]").IsMatch(termCaseSensitive):
                        if (bool.TryParse(columnValues[i], out var caseSensitive))
                        {
                            languageCode = new Regex($@"^{CaseSensitive}.*\[(\w{{2}}-\w{{2}})\]")
                                .Match(termCaseSensitive).Groups[1].Value.ToLower();

                            var caseSensitivity = caseSensitive
                                ? CaseSensitivity.CaseSensitive
                                : CaseSensitivity.CaseInsensitive;

                            var targetLanguageSectionIndex =
                                languageSections.FindIndex(section => section.LanguageCode == languageCode);

                            foreach (var term in languageSections[targetLanguageSectionIndex].Terms)
                            {
                                term.CaseSensitivity = caseSensitivity;
                            }
                        }

                        break;
                    
                    case var termExactMatch when new Regex($@"^{ExactMatch}.*\[(\w{{2}}-\w{{2}})\]").IsMatch(termExactMatch):
                        if (bool.TryParse(columnValues[i], out var exactMatch))
                        {
                            languageCode = new Regex($@"^{ExactMatch}.*\[(\w{{2}}-\w{{2}})\]")
                                .Match(termExactMatch).Groups[1].Value.ToLower();
                            
                            var targetLanguageSectionIndex =
                                languageSections.FindIndex(section => section.LanguageCode == languageCode);

                            foreach (var term in languageSections[targetLanguageSectionIndex].Terms)
                            {
                                term.ExactMatch = exactMatch;
                            }
                        }

                        break;
                }
            }
        
            var entry = new GlossaryConceptEntry(entryId, languageSections)
            {
                Definition = entryDefinition,
                Notes = entryNotes,
            };
            glossaryConceptEntries.Add(entry);
        }

        var title = input.Title ?? glossary.GlossaryName;
        var description = !string.IsNullOrWhiteSpace(input.SourceDescription ?? glossary.Description)
            ? input.SourceDescription ?? glossary.Description
            : $"Glossary export from Smartling on {DateTime.Now.ToLocalTime().ToString("F")}";
        
        var exportedGlossary = new Glossary(glossaryConceptEntries)
        {
            Title = title,
            SourceDescription = description
        };

        await using var glossaryStream = exportedGlossary.ConvertToTbx();
        var glossaryFileReference = 
            await _fileManagementClient.UploadAsync(glossaryStream, MediaTypeNames.Text.Xml, $"{title}.tbx");
        return new(glossaryFileReference);
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