using Apps.Smartling.Api;
using Apps.Smartling.Constants;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Jobs;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests;
using Apps.Smartling.Models.Requests.Jobs;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Jobs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;
using DisplayAttribute = Blackbird.Applications.Sdk.Common.DisplayAttribute;

namespace Apps.Smartling.Actions;

[ActionList("Jobs")]
public class JobActions(InvocationContext invocationContext) : SmartlingInvocable(invocationContext)
{
    #region Get

    [Action("Get job", Description = "Get the details of a job.")]
    public async Task<JobDto> GetJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}", 
            Method.Get
        );
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobDto>>(request);
        var job = response.Response.Data;
        return job;
    }  
    
    [Action("Get job word count", Description = "Get the word count of a job.")]
    public async Task<WordCountResponse> GetJobWordCount(
        [ActionParameter] JobIdentifier jobIdentifier,
        [ActionParameter] DatesOptionalRequest datesOptionalRequest)
    {
        var startDate = datesOptionalRequest.StartDate?.ToString("yyyy-MM-dd") ?? DateTime.Parse("2024-01-01").ToString("yyyy-MM-dd");
        var endDate = datesOptionalRequest.EndDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
        
        var request = new SmartlingRequest($"/reports-api/v3/word-count?startDate={startDate}&endDate={endDate}&jobUids={jobIdentifier.TranslationJobUid}", 
            Method.Get);
        
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<WordCountDto>>>(request);
        var wordCountResponse = WordCountResponse.CreateFromDtos(response.Response.Data.Items);
        return wordCountResponse;
    }

    [Action("Get job progress", Description = "Get the progress of a job.")]
    public async Task<JobProgressResponse> GetJobProgress(
        [ActionParameter] ProjectIdentifier project,
        [ActionParameter] JobIdentifier jobIdentifier,
        [ActionParameter] TargetLocaleOptionalIdentifier targetLocaleRequest)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var queryParams = string.Empty;
        if (!string.IsNullOrEmpty(targetLocaleRequest?.TargetLocaleId))
            queryParams = $"?targetLocaleId={targetLocaleRequest.TargetLocaleId}";

        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/progress{queryParams}",
            Method.Get
        );

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobProgressDto>>(request);

        return JobProgressResponse.CreateFromDto(response.Response.Data);
    }

    [Action("List job schedule items", Description = "List all schedule items for a specific job..")]
    public async Task<ListScheduleItemsResponse> ListJobScheduleItems(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier,
        [ActionParameter] TargetLocaleOptionalIdentifier targetLocale)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var endpoint = $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/schedule";
        var request = new SmartlingRequest(endpoint, Method.Get);

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<ScheduleItemDto>>>(request);
        var scheduleItems = response.Response.Data.Items;
        if (targetLocale != null && targetLocale.TargetLocaleId != null)
        {
            if (scheduleItems.Any(x => x.TargetLocaleId == targetLocale.TargetLocaleId))
            { return new(scheduleItems.Where(x => x.TargetLocaleId == targetLocale.TargetLocaleId)); }
            return new ListScheduleItemsResponse(new List<ScheduleItemDto>());
        }
        return new(scheduleItems);
    }

    [Action("Search jobs", Description = "List jobs that match the specified filter options. If no parameters are " +
                                         "specified, all jobs will be returned.")]
    public async Task<SearchJobsResponse> SearchJobs(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] SearchJobsRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var translationJobStatus = input.TranslationJobStatus == null ? "" : string.Join(",", input.TranslationJobStatus);
        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs?translationJobStatus={translationJobStatus}&limit={input.Limit}",
            Method.Get
        );

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<JobDto>>>(request);
        var jobs = response.Response.Data.Items;

        if (input.CreatedDateBefore != null)
            jobs = jobs.Where(job => job.CreatedDate <= input.CreatedDateBefore);
        
        if (input.CreatedDateAfter != null)
            jobs = jobs.Where(job => job.CreatedDate >= input.CreatedDateAfter);
        
        if (input.DueDateBefore != null)
            jobs = jobs.Where(job => job.DueDate <= input.DueDateBefore);
        
        if (input.DueDateAfter != null)
            jobs = jobs.Where(job => job.DueDate >= input.DueDateAfter);

        return new SearchJobsResponse { Jobs = jobs};
    }

    #endregion

    #region Post
    
    [Action("Create job", Description = "Create a new job.")]
    public async Task<JobDto> CreateJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] CreateJobRequest input, 
        [ActionParameter] TargetLocalesIdentifier targetLocales)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest($"/jobs-api/v3/projects/{projectId}/jobs", Method.Post);
        request.AddJsonBody(new
        {
            jobName = input.JobName,
            description = input.Description,
            targetLocaleIds = targetLocales.TargetLocaleIds,
            dueDate = input.DueDate,
            referenceNumber = input.ReferenceNumber,
            callbackUrl = input.CallbackUrl ??
                          $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}{ApplicationConstants.SmartlingBridgePath}"
                              .SetQueryParameter("id", projectId),
            callbackMethod = input.CallbackMethod ?? "POST"
        });

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobDto>>(request);
        var job = response.Response.Data;
        return job;
    }

    [Action("Modify translation job schedule", Description = "Modifies translation job schedule")]
    public async Task<ResponseData> ModifySchedule(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier,
        [ActionParameter] ModifyScheduleRequest input, 
        [ActionParameter] TargetLocaleIdentifier targetLocale)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/schedule",
            Method.Post
        );

        var body = new
        {
            schedules = new[]
            {
                new
                {
                    workflowStepUid = input.WorkflowUid,
                    dueDate = input.DueDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    targetLocaleId = targetLocale.TargetLocaleId
                }
            }
        };

        request.AddJsonBody(body);

        var response = await Client.ExecuteWithErrorHandling<ModifyTranslationJobDto>(request);

        return response.Response.Data;
    }

    [Action("Add locale to job", Description = "Add a locale to a job.")]
    public async Task<JobIdentifier> AddLocaleToJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] TargetLocaleIdentifier targetLocale, 
        [ActionParameter] [Display("Sync content")] bool? syncContent)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/locales/{targetLocale.TargetLocaleId}",
            Method.Post
        );
        request.AddJsonBody(new
        {
            syncContent = syncContent ?? true
        });
        
        await Client.ExecuteWithErrorHandling(request);
        return jobIdentifier;
    }
    
    [Action("Authorize job", Description = "Authorize all content within a job.")]
    public async Task<JobIdentifier> AuthorizeJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier, 
        [ActionParameter] TargetLocalesIdentifier targetLocales, 
        [ActionParameter] WorkflowIdentifier workflowIdentifier)
    {
        if ((targetLocales.TargetLocaleIds != null && workflowIdentifier.WorkflowUid == null)
            || (targetLocales.TargetLocaleIds == null && workflowIdentifier.WorkflowUid != null))
            throw new PluginMisconfigurationException("Please specify both target locales and workflow or leave both unspecified.");

        string projectId = await GetProjectId(project.ProjectId);
        for (var i = 0; i < 6; i++)
        {
            var getJobRequest = 
                new SmartlingRequest($"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}", 
                    Method.Get);
            var getJobResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobDto>>(getJobRequest);
            var job = getJobResponse.Response.Data;

            if (job.JobStatus != "AWAITING_AUTHORIZATION")
                await Task.Delay(TimeSpan.FromSeconds(10));
            else
                break;
        }

        var authorizeJobRequest = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/authorize", 
            Method.Post
        );

        if (targetLocales.TargetLocaleIds == null)
            authorizeJobRequest.AddJsonBody(new { });
        else
        {
            var localeWorkflows = targetLocales.TargetLocaleIds.Select(locale => new
            {
                targetLocaleId = locale,
                workflowUid = workflowIdentifier.WorkflowUid
            });
            
            authorizeJobRequest.AddJsonBody(new { localeWorkflows });
        }
        
        await Client.ExecuteWithErrorHandling(authorizeJobRequest);
        return jobIdentifier;
    }

    [Action("Close job", Description = "Close a completed job.")]
    public async Task<JobIdentifier> CloseJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/close", 
            Method.Post
        );
        await Client.ExecuteWithErrorHandling(request);
        return jobIdentifier;
    }
    
    [Action("Cancel job", Description = "Cancel a job.")]
    public async Task<JobIdentifier> CancelJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier,
        [ActionParameter] [Display("Reason")] string? reason)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}/cancel", 
            Method.Post
        );
        request.AddJsonBody(new { reason });
        await Client.ExecuteWithErrorHandling(request);
        return jobIdentifier;
    }

    #endregion

    #region Put
    
    [Action("Update job", Description = "Update a job. Specify only fields that need to be updated.")]
    public async Task<JobDto> UpdateJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier,
        [ActionParameter] UpdateJobRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var getJobRequest = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}", 
            Method.Get
        );
        var getJobResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobDto>>(getJobRequest);
        var job = getJobResponse.Response.Data;
        
        var updateJobRequest = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}", 
            Method.Put
        );
        updateJobRequest.AddJsonBody(new
        {
            jobName = input.JobName ?? job.JobName,
            description = input.Description ?? job.Description,
            dueDate = input.DueDate ?? job.DueDate,
            referenceNumber = input.ReferenceNumber ?? job.ReferenceNumber,
            callbackUrl = input.CallbackUrl ?? job.CallbackUrl,
            callbackMethod = input.CallbackMethod ?? job.CallbackMethod
        });
        
        var updateJobResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<JobDto>>(updateJobRequest);
        var updatedJob = updateJobResponse.Response.Data;
        return updatedJob;
    }

    #endregion
    
    #region Delete

    [Action("Delete job", Description = "Delete a job. Only job that is in CANCELLED status can be deleted.")]
    public async Task DeleteJob(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] JobIdentifier jobIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest(
            $"/jobs-api/v3/projects/{projectId}/jobs/{jobIdentifier.TranslationJobUid}", 
            Method.Delete
        );
        await Client.ExecuteWithErrorHandling(request);
    }
    
    #endregion
}