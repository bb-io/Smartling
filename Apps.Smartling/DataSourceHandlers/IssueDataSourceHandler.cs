using RestSharp;
using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Issues;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.DataSourceHandlers;

public class IssueDataSourceHandler(
    InvocationContext invocationContext,
    [ActionParameter] ProjectIdentifier project) 
    : SmartlingInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        string projectId = await GetProjectId(project.ProjectId);
        const int limit = 20;
        ResponseWrapper<ItemsWrapper<IssueDto>> response;
        var issues = new List<IssueDto>();
        var offset = 0;

        do
        {
            var request = new SmartlingRequest($"/issues-api/v2/projects/{projectId}/issues/list", Method.Post);
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

        return issues.Select(x => new DataSourceItem(x.IssueUid, x.IssueText));
    }
}