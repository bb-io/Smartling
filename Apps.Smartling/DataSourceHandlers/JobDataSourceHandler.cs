using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class JobDataSourceHandler(InvocationContext invocationContext)
    : SmartlingInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var limit = 100;
        var offset = 0;
        var jobs = new Dictionary<string, string>();

        var baseUrl = $"/jobs-api/v3/projects/{ProjectId}/jobs?sortBy=createdDate&sortDirection=DESC";
        if (!string.IsNullOrEmpty(context.SearchString))
        {
            baseUrl += $"&jobName={Uri.EscapeDataString(context.SearchString)}";
        }

        while (true)
        {
            var requestUrl = $"{baseUrl}&offset={offset}&limit={limit}";
            var request = new SmartlingRequest(requestUrl, Method.Get);
            var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<JobDto>>>(request);

            var items = response.Response.Data.Items.ToList();

            foreach (var job in items)
            {
                if (!jobs.ContainsKey(job.TranslationJobUid))
                {
                    jobs.Add(job.TranslationJobUid, job.JobName);
                }
            }

            if (items.Count < limit)
            {
                break;
            }

            offset += limit;
        }

        return jobs;
    }
}
