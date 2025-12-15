using Apps.Smartling.Api;
using Apps.Smartling.Models;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Reports;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Reports;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Reports;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Smartling.Actions;

public class ReportActions : SmartlingInvocable
{
    private readonly string _accountUid;
    private readonly IFileManagementClient _fileManagementClient;

    public ReportActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
    {
        _accountUid = GetAccountUid().Result;
        _fileManagementClient = fileManagementClient;
    }

    [Action("Get word count report", Description = "Retrieve a word count report for specified parameters.")]
    public async Task<GetWordCountReportResponse> GetWordCountReport(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] GetWordCountReportRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        var startDate = TimeZoneInfo.ConvertTime(input.StartDate, easternTimeZone).ToString("yyyy-MM-dd");
        var endDate = TimeZoneInfo.ConvertTime(input.EndDate ?? DateTime.Now, easternTimeZone).ToString("yyyy-MM-dd");
        var endpoint =
            $"/reports-api/v3/word-count?startDate={startDate}&endDate={endDate}&accountUid={_accountUid}&projectIds={projectId}";
        var request = new SmartlingRequest(endpoint, Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<WordCountReportDto>>>(request);
        var reports = response.Response.Data.Items;
        return new(reports);
    }
    
    [Action("Get word count report in CSV format", Description = "Retrieve a word count report in CSV format for " +
                                                                 "specified parameters.")]
    public async Task<FileWrapper> GetWordCountReportInCsvFormat(
        [ActionParameter] ProjectIdentifier project, 
        [ActionParameter] GetWordCountReportRequest input)
    {
        string projectId = await GetProjectId(project.ProjectId);
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        var startDate = TimeZoneInfo.ConvertTime(input.StartDate, easternTimeZone).ToString("yyyy-MM-dd");
        var endDate = TimeZoneInfo.ConvertTime(input.EndDate ?? DateTime.Now, easternTimeZone).ToString("yyyy-MM-dd");
        var endpoint =
            $"/reports-api/v3/word-count/csv?startDate={startDate}&endDate={endDate}&accountUid={_accountUid}&projectIds={projectId}";
        var request = new SmartlingRequest(endpoint, Method.Get);
        var response = await Client.ExecuteWithErrorHandling(request);

        using var stream = new MemoryStream(response.RawBytes);
        var file = await _fileManagementClient.UploadAsync(stream, response.ContentType, response.ContentHeaders.First(h => h.Name == "Content-Disposition").Value.ToString().Split('"')[1]);
        return new()
        {
            File = file
        };
    }
}