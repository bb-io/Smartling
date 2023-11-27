using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Glossaries;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class GlossaryLabelDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public GlossaryLabelDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var accountUid = await GetAccountUid();
        var request = new SmartlingRequest($"/glossary-api/v3/accounts/{accountUid}/labels", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<GlossaryLabelDto>>>(request);
        return response.Response.Data.Items
            .Where(label => context.SearchString == null 
                            || label.LabelText.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(label => label.LabelUid, label => label.LabelText);
    }
}