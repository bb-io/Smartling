using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Issues;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class IssueDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public IssueDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        const int limit = 20;
        ResponseWrapper<ItemsWrapper<IssueDto>> response;
        var issues = new List<IssueDto>();
        var offset = 0;

        do
        {
            var request = new SmartlingRequest($"/issues-api/v2/projects/{ProjectId}/issues/list", Method.Post);
            request.AddJsonBody(new
            {
                offset,
                limit,
                sortBy = new
                {
                    items = new[]
                    {
                        new
                        {
                            direction = "DESC",
                            fieldName = "createdDate"
                        }
                    }
                }
            });
            response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<IssueDto>>>(request);
            issues.AddRange(response.Response.Data.Items
                .Where(issue => context.SearchString == null
                                || issue.IssueText.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)));
            offset += limit;
        } while (response.Response.Data.Items.Count() == limit || issues.Count >= limit);

        return issues.ToDictionary(issue => issue.IssueUid, issue => issue.IssueText);
    }
}