using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Jobs;
using Apps.Smartling.Polling.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.Sdk.Common.Webhooks;
using RestSharp;

namespace Apps.Smartling.Polling
{
    [PollingEventList]
    public class PollingList : SmartlingInvocable
    {
        public PollingList(InvocationContext invocationContext) : base(invocationContext)
        {

        }

        [PollingEvent("On job authorized [Polling]")]
        public async Task<PollingEventResponse<DateMemory, SearchJobsPollingResponse>> OnJobAuthorized(PollingEventRequest<DateMemory> request)
        {
            var endpoint = $"/jobs-api/v2/projects/{ProjectId}/jobs";

            var jobs = (await Client.Paginate<Apps.Smartling.Polling.Models.JobItem>(
                new SmartlingRequest(endpoint, Method.Get)
            )).ToArray();

            if (jobs.Length == 0)
            {
                return new PollingEventResponse<DateMemory, SearchJobsPollingResponse>
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
                    if (job.JobStatus == "IN_PROGRESS")
                    {
                        request.Memory.KnownJobIds.Add(job.TranslationJobUid);
                    }
                }

                return new PollingEventResponse<DateMemory, SearchJobsPollingResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

            var newInProgressJobs = jobs
                .Where(j => j.JobStatus == "IN_PROGRESS"
                         && !request.Memory.KnownJobIds.Contains(j.TranslationJobUid))
                .ToArray();

            if (newInProgressJobs.Any())
            {
                foreach (var job in newInProgressJobs)
                {
                    request.Memory.KnownJobIds.Add(job.TranslationJobUid);
                }

                var listJobsResponse = new SearchJobsPollingResponse
                {
                    Jobs = newInProgressJobs
                };

                return new PollingEventResponse<DateMemory, SearchJobsPollingResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = listJobsResponse
                };
            }
            else
            {
                return new PollingEventResponse<DateMemory, SearchJobsPollingResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }
        }


        [PollingEvent("On jobs completed [Polling]")]
        public async Task<PollingEventResponse<DateMemory, SearchJobsPollingResponse>> OnJobsCompleted(PollingEventRequest<DateMemory> request)
        {
            var endpoint = $"/jobs-api/v2/projects/{ProjectId}/jobs";

            var jobs = (await Client.Paginate<Apps.Smartling.Polling.Models.JobItem>(
                new SmartlingRequest(endpoint, Method.Get)
            )).ToArray();

            if (jobs.Length == 0)
            {
                return new PollingEventResponse<DateMemory, SearchJobsPollingResponse>
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

                return new PollingEventResponse<DateMemory, SearchJobsPollingResponse>
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

                var listJobsResponse = new SearchJobsPollingResponse
                {
                    Jobs = newCompletedJobs
                };

                return new PollingEventResponse<DateMemory, SearchJobsPollingResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = listJobsResponse
                };
            }
            else
            {
                return new PollingEventResponse<DateMemory, SearchJobsPollingResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }
        }

        [PollingEvent("On specific job completed [Polling]")]
        public async Task<PollingEventResponse<jobStatusMemory, JobDto>> OnJobCompleted(PollingEventRequest<jobStatusMemory> request, [PollingEventParameter] JobIdentifier jobIdentifier)
        {
            var jobRequest = new SmartlingRequest($"/jobs-api/v3/projects/{ProjectId}/jobs/{jobIdentifier.TranslationJobUid}",
            Method.Get);
            var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobDto>>(jobRequest);
            var job = response.Response.Data;

            var newMemory = new jobStatusMemory
            {
                LastInteractionDate = DateTime.UtcNow,
                LastJobStatus = job.JobStatus
            };

            if (request.Memory == null)
            {
                return new PollingEventResponse<jobStatusMemory, JobDto>
                {
                    FlyBird = job.JobStatus == "COMPLETED",
                    Memory = newMemory,
                    Result = job
                };
            }

            if (job.JobStatus == "COMPLETED" && job.JobStatus != request.Memory.LastJobStatus)
            {
                return new PollingEventResponse<jobStatusMemory, JobDto>
                {
                    FlyBird = true,
                    Memory = newMemory,
                    Result = job
                };
            } else
            {
                return new PollingEventResponse<jobStatusMemory, JobDto>
                {
                    FlyBird = false,
                    Memory = newMemory,
                };
            }
        }
    }
}
