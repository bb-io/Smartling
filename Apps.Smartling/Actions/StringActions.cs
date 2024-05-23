using Apps.Smartling.Api;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Dtos.Strings;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Strings;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Strings;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList]
public class StringActions : SmartlingInvocable
{
    public StringActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    #region Get

    [Action("List all source strings for file", Description = "List all source strings for a specified source file.")]
    public async Task<ListSourceStringsResponse> ListSourceStringsForFile(
        [ActionParameter] SourceFileIdentifier fileIdentifier)
    {
        const int limitPerRequest = 500;
        var offset = 0;
        var strings = new List<SourceStringDto>();
        ResponseWrapper<ItemsWrapper<SourceStringDto>> response;

        do
        {
            var request =
                new SmartlingRequest(
                    $"/strings-api/v2/projects/{ProjectId}/source-strings?fileUri={fileIdentifier.FileUri}&offset={offset}",
                    Method.Get);
            response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<SourceStringDto>>>(request);
            strings.AddRange(response.Response.Data.Items);
            offset += limitPerRequest;
        } while (response.Response.Data.TotalCount == limitPerRequest);

        return new(strings);
    }

    [Action("Get source string by hashcode", Description = "Retrieve a single source string with a specified hashcode.")]
    public async Task<SourceStringDto> GetSourceStringByHashcode([ActionParameter] StringIdentifier stringIdentifier)
    {
        var request =
            new SmartlingRequest(
                $"/strings-api/v2/projects/{ProjectId}/source-strings?hashcodes={stringIdentifier.Hashcode}", 
                Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<SourceStringDto>>>(request);
        var sourceString = response.Response.Data.Items.FirstOrDefault();
        return sourceString;
    }

    [Action("List translations for strings in file", Description = "List translated strings for a specified file.")]
    public async Task<ListStringsTranslationsResponse> ListTranslationsForStringsInFile(
        [ActionParameter] SourceFileIdentifier fileIdentifier, 
        [ActionParameter] TargetLocaleIdentifier targetLocale)
    {
        const int limitPerRequest = 500;
        var offset = 0;
        var stringTranslations = new List<StringTranslationsDto>();
        ResponseWrapper<ItemsWrapper<StringTranslationsDto>> response;

        do
        {
            var request =
                new SmartlingRequest(
                    $"/strings-api/v2/projects/{ProjectId}/translations?targetLocaleId={targetLocale.TargetLocaleId}&fileUri={fileIdentifier.FileUri}&offset={offset}",
                    Method.Get);
            response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<StringTranslationsDto>>>(request);
            stringTranslations.AddRange(response.Response.Data.Items);
            offset += limitPerRequest;
        } while (response.Response.Data.TotalCount == limitPerRequest);

        return new(stringTranslations);
    }
    
    [Action("List translations for string by hashcode", Description = "List translations for a string with specified hashcode.")]
    public async Task<TranslationsResponse> GetStringTranslationsByHashcode(
        [ActionParameter] StringIdentifier stringIdentifier,
        [ActionParameter] TargetLocaleIdentifier targetLocale)
    {
        var request = new SmartlingRequest(
            $"/strings-api/v2/projects/{ProjectId}/translations?targetLocaleId={targetLocale.TargetLocaleId}&hashcodes={stringIdentifier.Hashcode}",
            Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<StringTranslationsDto>>>(request);
        var translations = response.Response.Data.Items.FirstOrDefault();
        return new TranslationsResponse { Translations = translations?.Translations != null ? translations.Translations.Select(x => x.Translation) : new List<string>()};
    }

    #endregion

    #region Post

    [Action("Add string to project", Description = "Uploads a string to a project.")]
    public async Task<StringDto> AddString([ActionParameter] AddStringRequest input)
    {
        var request = new SmartlingRequest($"/strings-api/v2/projects/{ProjectId}", Method.Post);
        request.AddJsonBody(new
        {
            strings = new[]
            {
                new
                {
                    stringText = input.StringText,
                    variant = input.VariantMetadata,
                    callbackUrl = input.CallbackUrl ??
                                  $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}{ApplicationConstants.SmartlingBridgePath}"
                                      .SetQueryParameter("id", ProjectId),
                    callbackMethod = input.CallbackMethod ?? "POST",
                    instruction = input.Instruction,
                    maxLength = input.MaxLength,
                    format = input.Format
                }
            },
            placeholderFormat = input.PlaceholderFormat,
            placeholderFormatCustom = input.PlaceholderFormatCustom,
            input.Namespace
        });

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<StringDto>>>(request);
        var stringDto = response.Response.Data.Items.FirstOrDefault();
        return stringDto;
    }

    [Action("Add string to job", Description = "Add a string to a job.")]
    public async Task<StringIdentifier> AddStringToJob([ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] StringIdentifier stringIdentifier, [ActionParameter] TargetLocalesIdentifier targetLocales,
        [Display("Move enabled")] bool? moveEnabled)
    {
        var request =
            new SmartlingRequest(
                $"/jobs-api/v3/projects/{ProjectId}/jobs/{jobIdentifier.TranslationJobUid}/strings/add", Method.Post);
        request.AddJsonBody(new
        {
            hashcodes = new[] { stringIdentifier.Hashcode },
            targetLocaleIds = targetLocales.TargetLocaleIds,
            moveEnabled = moveEnabled ?? false
        });

        await Client.ExecuteWithErrorHandling(request);
        return stringIdentifier;
    }

    [Action("Remove string from job", Description = "Remove a string from a job.")]
    public async Task<StringIdentifier> RemoveStringFromJob([ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] StringIdentifier stringIdentifier, [ActionParameter] TargetLocalesIdentifier targetLocales)
    {
        var request =
            new SmartlingRequest(
                $"/jobs-api/v3/projects/{ProjectId}/jobs/{jobIdentifier.TranslationJobUid}/strings/remove", 
                Method.Post);
        request.AddJsonBody(new
        {
            hashcodes = new[] { stringIdentifier.Hashcode },
            localeIds = targetLocales.TargetLocaleIds
        });
        
        await Client.ExecuteWithErrorHandling(request);
        return stringIdentifier;
    }

    [Action("List strings in job", Description = "Gets all the translation strings in a job.")]
    public async Task<ListStringsInJobResponse> ListStringsInJob([ActionParameter] JobIdentifier jobIdentifier)
    {
        const int limitPerRequest = 500;
        var offset = 0;
        var stringTranslations = new List<StringHashcodeLocaleDto>();
        ResponseWrapper<ItemsWrapper<StringHashcodeLocaleDto>> response;

        do
        {
            var request = new SmartlingRequest($"/jobs-api/v3/projects/{ProjectId}/jobs/{jobIdentifier.TranslationJobUid}/strings?offset={offset}",
            Method.Get);
            response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<StringHashcodeLocaleDto>>>(request);
            stringTranslations.AddRange(response.Response.Data.Items);
            offset += limitPerRequest;
        } while (response.Response.Data.TotalCount == limitPerRequest);

        return new ListStringsInJobResponse { Translations = stringTranslations, TotalCount = stringTranslations.Count};

    }

    #endregion
}