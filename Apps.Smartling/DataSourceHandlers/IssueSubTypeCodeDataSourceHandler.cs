using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Issues;
using Apps.Smartling.Models.Requests.Issues;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class IssueSubTypeCodeDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    private readonly CreateIssueRequest _createIssueRequest;

    public IssueSubTypeCodeDataSourceHandler(InvocationContext invocationContext, 
        [ActionParameter] CreateIssueRequest createIssueRequest) : base(invocationContext)
    {
        _createIssueRequest = createIssueRequest;
    }
    
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (_createIssueRequest.IssueTypeCode == null)
            throw new PluginMisconfigurationException("Please enter issue type first.");
        
        var accountUid = await GetAccountUid();
        var request = new SmartlingRequest($"/issues-api/v2/accounts/{accountUid}/issue-types", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<IssueTypeDto>>>(request);
        var issueTypes = response.Response.Data.Items;
        
        return issueTypes
            .First(type => type.IssueTypeCode == _createIssueRequest.IssueTypeCode).SubTypes
            .Where(subtype => context.SearchString == null 
                              || subtype.Description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(subtype => subtype.IssueSubTypeCode, subtype => subtype.Description);
    }
}