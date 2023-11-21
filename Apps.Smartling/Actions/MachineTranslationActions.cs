using Apps.Smartling.Api;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Apps.Smartling.Models;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Identifiers.MachineTranslation;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList]
public class MachineTranslationActions : SmartlingInvocable // TODO: finish when file MT is available 
{
    public MachineTranslationActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task TranslateFile([ActionParameter] FileWrapper file, 
        [ActionParameter] SmartlingTargetLocalesIdentifier targetLocales, 
        [ActionParameter] SmartlingSourceLocaleIdentifier sourceLocale,
        [ActionParameter] [Display("File type")] [DataSource(typeof(MTFileTypeDataSourceHandler))] string? fileType)
    {
        var getProjectRequest = new SmartlingRequest($"/projects-api/v2/projects/{ProjectId}", Method.Get);
        var getProjectResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDto>>(getProjectRequest);
        var accountUid = getProjectResponse.Response.Data.AccountUid;
        
        if (fileType == null)
        {
            var fileExtension = Path.GetExtension(file.File.Name).TrimStart('.');

            switch (fileExtension)
            {
                case "txt":
                    fileType = "PLAIN_TEXT";
                    break;
                case "md":
                    fileType = "MARKDOWN";
                    break;
                case "xliff":
                    fileType = "XLIFF2";
                    break;
                case "xltx":
                    fileType = "XLSX_TEMPLATE";
                    break;
                default:
                    fileType = fileExtension.ToUpper();
                    break;
            }
        }

        var uploadFileRequest =
            new SmartlingRequest($"/file-translations-api/v2/accounts/{accountUid}/files", Method.Post);
        uploadFileRequest.AddParameter("request", JsonConvert.SerializeObject(new { fileType }));
        uploadFileRequest.AddFile("file", file.File.Bytes, file.File.Name);
        var uploadFileResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<FileIdentifier>>(uploadFileRequest);
        var fileUid = uploadFileResponse.Response.Data.FileUid;

        var sourceLocaleId = sourceLocale.LocaleId;

        if (sourceLocaleId == null)
        {
            var detectFileLanguageRequest = new SmartlingRequest(
                $"/file-translations-api/v2/accounts/{accountUid}/files/{fileUid}/language-detection", Method.Post);
            var detectFileLanguageResponse =
                await Client.ExecuteWithErrorHandling<ResponseWrapper<LanguageDetectionIdentifier>>(
                    detectFileLanguageRequest);
            sourceLocaleId = detectFileLanguageResponse.Response.Data.LanguageDetectionUid;
        }

        var translateFileRequest =
            new SmartlingRequest($"/file-translations-api/v2/accounts/{accountUid}/files/{fileUid}/mt", Method.Post);
        translateFileRequest.AddJsonBody(new
        {
            sourceLocaleId,
            targetLocaleIds = targetLocales.LocaleIds
        });
        var translateFileResponse =
            await Client.ExecuteWithErrorHandling<ResponseWrapper<MTIdentifier>>(translateFileRequest);
        var mtUid = translateFileResponse.Response.Data.MtUid;
        
        // TODO: check translation progress
    }
}