using Apps.Smartling.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using Apps.Smartling.Models.Dtos.Contexts;

namespace Apps.Smartling.DataSourceHandlers
{
    public class ProjectContextDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
    {
        public ProjectContextDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            var searchProjectContextRequest = new SmartlingRequest($"/context-api/v2/projects/{ProjectId}/contexts", Method.Get);
            var searchProjectContextResponse =
                await Client.Paginate<ProjectContextDto>(searchProjectContextRequest);
            var jobs = searchProjectContextResponse
                .Where(prContext => context.SearchString == null
                              || prContext.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(prContext => prContext.ContextUid, job => job.Name);
            return jobs;
        }
    }
}
