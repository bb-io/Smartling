using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Issues;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Issues;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Issues;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.Actions;

[ActionList("Issues")]
public class IssueActions(InvocationContext invocationContext) : SmartlingInvocable(invocationContext)
{
    #region Get

    [Action("Get issue", Description = "Retrieve detailed information about a single issue.")]
    public async Task<IssueDto> GetIssue(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] IssueIdentifier issueIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);

        var request = new SmartlingRequest(
            $"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}", 
            Method.Get
        );
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<IssueDto>>(request);
        var issue = response.Response.Data;
        return issue;
    }

    [Action("Search issues", Description = "List issues that match the specified filter options. If no parameters are " +
                                           "specified, all issues will be returned.")]
    public async Task<SearchIssuesResponse> SearchIssues(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] SearchIssuesRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest($"/issues-api/v2/projects/{projectId}/issues/list", Method.Post);
        request.AddJsonBody(new
        {
            createdDateBefore = input.CreatedDateBefore,
            createdDateAfter = input.CreatedDateAfter,
            resolvedDateBefore = input.ResolvedDateBefore,
            resolvedDateAfter = input.ResolvedDateAfter,
            answered = input.Answered,
            reopened = input.Reopened,
            issueSeverityLevelCodes = input.IssueSeverityLevelCodes,
            issueStateCodes = input.IssueStateCode == null ? null : new[] { input.IssueStateCode },
            issueTypeCodes = input.IssueTypeCode == null ? null : new[] { input.IssueTypeCode },
            assigneeUserUid = input.AssigneeUserUid == "-1" ? null : input.AssigneeUserUid,
            hasComments = input.HasComments,
            jobFilter = input.JobIds == null ? null : new { jobUids = input.JobIds },
            stringFilter = input.StringHashcodes == null ? null : new { hashcodes = input.StringHashcodes },
            limit = input.Limit
        });

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<IssueDto>>>(request);
        var issues = response.Response.Data.Items;

        if (input.AssigneeUserUid == "-1")
            issues = issues.Where(issue => issue.AssigneeUserUid == null);
        
        return new(issues);
    }

    #endregion

    #region Post

    [Action("Create issue", Description = "Create a new issue for a string.")]
    public async Task<IssueDto> CreateIssue(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] StringIdentifier stringIdentifier, 
        [ActionParameter] TargetLocaleOptionalIdentifier targetLocale, 
        [ActionParameter] CreateIssueRequest input, 
        [ActionParameter] AssigneeIdentifier assigneeIdentifier)
    {
        if (input.IssueTypeCode == "TRANSLATION" && targetLocale.TargetLocaleId == null)
            throw new PluginMisconfigurationException("Target locale is required for translation issue.");

        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest($"/issues-api/v2/projects/{projectId}/issues", Method.Post);
        request.AddJsonBody(new
        {
            issueText = input.IssueText,
            issueTypeCode = input.IssueTypeCode,
            issueSubTypeCode = input.IssueSubTypeCode,
            String = new
            {
                hashcode = stringIdentifier.Hashcode,
                localeId = input.IssueTypeCode == "SOURCE" ? null : targetLocale.TargetLocaleId
            },
            assigneeUserUid = assigneeIdentifier.AssigneeUserUid == "-1" ? null : assigneeIdentifier.AssigneeUserUid,
            issueSeverityLevelCode = input.IssueSeverityLevelCode
        });

        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<IssueDto>>(request);
        var issue = response.Response.Data;
        return issue;
    }

    #endregion

    #region Put

    [Action("Edit issue", Description = "Edit the issue. Specify only fields that need to be updated.")]
    public async Task<IssueIdentifier> EditIssue(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] IssueIdentifier issueIdentifier, 
        [ActionParameter] EditIssueRequest input, 
        [ActionParameter] TargetLocaleOptionalIdentifier targetLocale,
        [ActionParameter] AssigneeIdentifier assigneeIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);
        if (input.IssueTypeCode != null)
        {
            if (input.IssueSubTypeCode == null)
                throw new PluginMisconfigurationException("When updating the issue type, the issue subtype should be specified.");
            
            if (input.IssueTypeCode == "TRANSLATION" && targetLocale.TargetLocaleId == null)
                throw new PluginMisconfigurationException("Target locale is required for translation issue.");

            var request =
                new SmartlingRequest(
                    $"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}/change-type", Method.Put);

            if (targetLocale.TargetLocaleId != null)
                request.AddJsonBody(new
                {
                    issueTypeCode = input.IssueTypeCode,
                    issueSubTypeCode = input.IssueSubTypeCode,
                    localeId = input.IssueTypeCode == "SOURCE" ? null : targetLocale.TargetLocaleId
                });
            else
                request.AddJsonBody(new
                {
                    issueTypeCode = input.IssueTypeCode,
                    issueSubTypeCode = input.IssueSubTypeCode
                });
            
            await Client.ExecuteWithErrorHandling(request);
        }
        
        if (input.IssueText != null)
        {
            var request =
                new SmartlingRequest($"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}/issueText",
                    Method.Put);
            request.AddJsonBody(new { issueText = input.IssueText });
            await Client.ExecuteWithErrorHandling(request);
        }

        if (input.IssueSeverityLevelCode != null)
        {
            var request =
                new SmartlingRequest(
                    $"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}/severity-level",
                    Method.Put);
            request.AddJsonBody(new { issueSeverityLevelCode = input.IssueSeverityLevelCode });
            await Client.ExecuteWithErrorHandling(request);
        }

        if (input.Answered != null)
        {
            var request =
                new SmartlingRequest($"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}/answered",
                    Method.Put);
            request.AddJsonBody(new { answered = input.Answered });
            await Client.ExecuteWithErrorHandling(request);
        }

        if (assigneeIdentifier.AssigneeUserUid != null)
        {
            if (assigneeIdentifier.AssigneeUserUid == "-1")
            {
                var request =
                    new SmartlingRequest($"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}/assignee",
                        Method.Delete);
                await Client.ExecuteWithErrorHandling(request);
            }
            else
            {
                var request =
                    new SmartlingRequest($"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}/assignee",
                        Method.Put);
                request.AddJsonBody(new
                {
                    assigneeUserUid = assigneeIdentifier.AssigneeUserUid
                });
                await Client.ExecuteWithErrorHandling(request);
            }
        }
        
        return issueIdentifier;
    }

    [Action("Open issue", Description = "Set the state of an issue to 'opened'.")]
    public async Task<IssueIdentifier> OpenIssue(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] IssueIdentifier issueIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest(
            $"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}/state",
            Method.Put
        );
        request.AddJsonBody(new { issueStateCode = "OPENED" });
        await Client.ExecuteWithErrorHandling(request);
        return issueIdentifier;
    }
    
    [Action("Close issue", Description = "Set the state of an issue to 'resolved'.")]
    public async Task<IssueIdentifier> CloseIssue(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] IssueIdentifier issueIdentifier)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var request = new SmartlingRequest(
            $"/issues-api/v2/projects/{projectId}/issues/{issueIdentifier.IssueUid}/state",
            Method.Put
        );
        request.AddJsonBody(new { issueStateCode = "RESOLVED" });
        await Client.ExecuteWithErrorHandling(request);
        return issueIdentifier;
    }

    #endregion
}