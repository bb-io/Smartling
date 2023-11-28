using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Attachments;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class JobAttachmentDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    private readonly JobIdentifier _jobIdentifier;
    
    public JobAttachmentDataSourceHandler(InvocationContext invocationContext, 
        [ActionParameter] JobIdentifier jobIdentifier) : base(invocationContext)
    {
        _jobIdentifier = jobIdentifier;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (_jobIdentifier.TranslationJobUid == null)
            throw new Exception("Please enter job first.");
        
        var accountUid = await GetAccountUid();
        
        var request = 
            new SmartlingRequest($"/attachments-api/v2/accounts/{accountUid}/jobs/{_jobIdentifier.TranslationJobUid}", 
                Method.Get);
        
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<AttachmentDto>>>(request);
        var attachments = response.Response.Data.Items;
        
        return attachments
            .Where(attachment => context.SearchString == null 
                                 || attachment.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(attachment => attachment.AttachmentUid, attachment => attachment.Name);
    }
}