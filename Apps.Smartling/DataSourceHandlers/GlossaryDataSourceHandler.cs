using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Glossaries;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class GlossaryDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public GlossaryDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var accountUid = await GetAccountUid();
        var request = new SmartlingRequest($"/glossary-api/v3/accounts/{accountUid}/glossaries/search", Method.Post);
        request.AddJsonBody(new
        {
            query = context.SearchString,
            glossaryState = "BOTH"
        });
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<GlossaryDto>>>(request);

        return response.Response.Data.Items
            .ToDictionary(glossary => glossary.GlossaryUid,
                glossary => glossary.Archived ? $"{glossary.GlossaryName} (Archived)" : glossary.GlossaryName);
    }
}