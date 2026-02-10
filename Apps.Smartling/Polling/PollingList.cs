using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos.Glossaries;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Polling.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Smartling.Polling;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : SmartlingInvocable(invocationContext)
{
    [PollingEvent("On jobs authorized [Polling]")]
    public async Task<PollingEventResponse<DateMemory, SearchJobsPollingResponse>> OnJobAuthorized(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] ProjectIdentifier project)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var endpoint = $"/jobs-api/v2/projects/{projectId}/jobs";

        var jobs = (await Client.Paginate<JobItem>(
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

    [PollingEvent("On specific job authorized [Polling]")]
    public async Task<PollingEventResponse<jobStatusMemory, JobDto>> OnJobAuthorized(
    PollingEventRequest<jobStatusMemory> request,
    [PollingEventParameter] JobIdentifier jobIdentifier,
    [PollingEventParameter] ProjectIdentifier project)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var jobRequest = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}",
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
                FlyBird = job.JobStatus == "IN_PROGRESS",
                Memory = newMemory,
                Result = job.JobStatus == "IN_PROGRESS" ? job : null
            };
        }

        if (job.JobStatus == "IN_PROGRESS" &&
            job.JobStatus != request.Memory.LastJobStatus)
        {
            return new PollingEventResponse<jobStatusMemory, JobDto>
            {
                FlyBird = true,
                Memory = newMemory,
                Result = job
            };
        }

        return new PollingEventResponse<jobStatusMemory, JobDto>
        {
            FlyBird = false,
            Memory = newMemory
        };
    }

    [PollingEvent("On jobs completed [Polling]")]
    public async Task<PollingEventResponse<DateMemory, SearchJobsPollingResponse>> OnJobsCompleted(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] ProjectIdentifier project)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var endpoint = $"/jobs-api/v2/projects/{projectId}/jobs";

        var jobs = (await Client.Paginate<JobItem>(
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
    public async Task<PollingEventResponse<jobStatusMemory, JobDto>> OnJobCompleted(
        PollingEventRequest<jobStatusMemory> request,
        [PollingEventParameter] JobIdentifier jobIdentifier,
        [PollingEventParameter] ProjectIdentifier project)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var jobRequest = new SmartlingRequest($"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}",
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
        }
        else
        {
            return new PollingEventResponse<jobStatusMemory, JobDto>
            {
                FlyBird = false,
                Memory = newMemory,
            };
        }
    }

    [PollingEvent("On glossary entries added [Polling]")]
    public async Task<PollingEventResponse<GlossaryEntriesMemory, NewGlossaryEntriesResponse>> OnGlossaryEntriesAdded(
    PollingEventRequest<GlossaryEntriesMemory> request,
    [PollingEventParameter] GlossaryIdentifier glossary)
    {
        var memory = request.Memory ?? new GlossaryEntriesMemory
        {
            LastCreatedDate = DateTime.UtcNow
        };

        var endpoint = $"/glossary-api/v2/glossaries/{glossary.GlossaryUid}/entries";

        var smartlingRequest = new SmartlingRequest(endpoint, Method.Get);

        smartlingRequest.AddQueryParameter("createdFrom", memory.LastCreatedDate.ToString("o"));

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<List<GlossaryEntryDto>>>(smartlingRequest);
        var entries = response.Response.Data ?? new List<GlossaryEntryDto>();

        if (request.Memory == null)
        {
            return new PollingEventResponse<GlossaryEntriesMemory, NewGlossaryEntriesResponse>
            {
                FlyBird = false,
                Memory = memory
            };
        }

        memory.LastCreatedDate = DateTime.UtcNow;

        if (entries.Any())
        {
            memory.LastCreatedDate = entries.Max(e => e.CreatedDate);
            return new PollingEventResponse<GlossaryEntriesMemory, NewGlossaryEntriesResponse>
            {
                FlyBird = true,
                Memory = memory,
                Result = new NewGlossaryEntriesResponse {Entries = entries }
            };
        }

        return new PollingEventResponse<GlossaryEntriesMemory, NewGlossaryEntriesResponse>
        {
            FlyBird = false,
            Memory = memory
        };
    }

}
