using Apps.Smartling.Api;
using Apps.Smartling.Polling.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Smartling.Polling
{
    [PollingEventList]
    public class PollingList : SmartlingInvocable
    {
        public PollingList(InvocationContext invocationContext) : base(invocationContext)
        {

        }

        [PollingEvent("On job completed [Polling]")]
        public async Task<PollingEventResponse<DateMemory, ListJobsResponse>> OnJobCompleted(PollingEventRequest<DateMemory> request)
        {
            var endpoint = $"/jobs-api/v2/projects/{ProjectId}/jobs";

            var jobs = (await Client.Paginate<Apps.Smartling.Polling.Models.JobItem>(
                new SmartlingRequest(endpoint, Method.Get)
            )).ToArray();

            if (jobs.Length == 0)
            {
                return new PollingEventResponse<DateMemory, ListJobsResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory ?? new DateMemory { LastInteractionDate = DateTime.UtcNow }
                };
            }

            if (request.Memory == null)
            {
                var maxCreatedAt = jobs.Max(j => j.CreatedDate);
                var memory = new DateMemory { LastInteractionDate = maxCreatedAt };
                return new PollingEventResponse<DateMemory, ListJobsResponse>
                {
                    FlyBird = false,
                    Memory = memory
                };
            }

            var completedJobs = jobs.Where(j => j.JobStatus == "COMPLETED" && j.CreatedDate > request.Memory.LastInteractionDate)
                                    .ToArray();

            if (completedJobs.Any())
            {
                var maxCreatedAt = completedJobs.Max(j => j.CreatedDate);
                request.Memory.LastInteractionDate = maxCreatedAt;

                var listJobsResponse = new Apps.Smartling.Polling.Models.ListJobsResponse
                {
                    Response = new Apps.Smartling.Polling.Models.ResponseData
                    {
                        Code = "SUCCESS",
                        Data = new Apps.Smartling.Polling.Models.DataContent
                        {
                            Items = completedJobs.ToList(),
                            TotalCount = completedJobs.Length
                        }
                    }
                };

                return new PollingEventResponse<DateMemory, ListJobsResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = listJobsResponse
                };
            }
            else
            {
                return new PollingEventResponse<DateMemory, ListJobsResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }
        }
    }
}
