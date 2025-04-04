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
                    Memory = request.Memory ?? new DateMemory
                    {
                        LastInteractionDate = DateTime.UtcNow,
                        KnownJobIds = new List<string>()
                    }
                };
            }

            if (request.Memory == null)
            {
                request.Memory = new DateMemory
                {
                    LastInteractionDate = DateTime.UtcNow,
                    KnownJobIds = new List<string>()
                };

                foreach (var job in jobs)
                {
                    if (job.JobStatus == "COMPLETED" || job.JobStatus == "CLOSED")
                    {
                        request.Memory.KnownJobIds.Add(job.TranslationJobUid);
                    }
                }

                return new PollingEventResponse<DateMemory, ListJobsResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

              var newCompletedJobs = jobs
                .Where(j => (j.JobStatus == "COMPLETED" || j.JobStatus == "CLOSED")
                         && !request.Memory.KnownJobIds.Contains(j.TranslationJobUid))
                .ToArray();

            if (newCompletedJobs.Any())
            {
                foreach (var job in newCompletedJobs)
                {
                    request.Memory.KnownJobIds.Add(job.TranslationJobUid);
                }

                var listJobsResponse = new ListJobsResponse
                {
                    Response = new ResponseData
                    {
                        Code = "SUCCESS",
                        Data = new DataContent
                        {
                            Items = newCompletedJobs.ToList(),
                            TotalCount = newCompletedJobs.Length
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
