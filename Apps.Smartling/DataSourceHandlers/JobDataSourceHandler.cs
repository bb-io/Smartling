using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class JobDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public JobDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new SmartlingRequest($"/jobs-api/v3/projects/{ProjectId}/jobs?sortBy=createdDate&sortDirection=DESC", 
            Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<JobDto>>>(request);
        var jobs = response.Response.Data.Items
            .Where(job => context.SearchString == null 
                          || job.JobName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(job => job.TranslationJobUid, job => job.JobName);
        return jobs;
    }
}