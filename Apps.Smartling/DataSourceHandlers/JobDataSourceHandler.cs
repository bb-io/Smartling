using RestSharp;
using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.DataSourceHandlers;

public class JobDataSourceHandler(
    InvocationContext invocationContext,
    [ActionParameter] ProjectIdentifier project)
    : SmartlingInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var limit = 100;
        var offset = 0;
        var jobs = new List<DataSourceItem>(); 
        var addedIds = new HashSet<string>();

        var baseUrl = $"/jobs-api/v3/projects/{projectId}/jobs?sortBy=createdDate&sortDirection=DESC";
        if (!string.IsNullOrEmpty(context.SearchString))
            baseUrl += $"&jobName={Uri.EscapeDataString(context.SearchString)}";

        while (true)
        {
            var requestUrl = $"{baseUrl}&offset={offset}&limit={limit}";
            var request = new SmartlingRequest(requestUrl, Method.Get);
            var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<JobDto>>>(request);

            var items = response.Response.Data.Items.ToList();

            foreach (var job in items)
            {
                if (addedIds.Add(job.TranslationJobUid))
                {
                    var newJob = new DataSourceItem(job.TranslationJobUid, job.JobName);
                    jobs.Add(newJob);
                }
            }

            if (items.Count < limit)
                break;

            offset += limit;
        }

        return jobs;
    }
}
