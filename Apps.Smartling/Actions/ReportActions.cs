using Apps.Smartling.Api;
using Apps.Smartling.Models;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Reports;
using Apps.Smartling.Models.Requests.Reports;
using Apps.Smartling.Models.Responses;
using Apps.Smartling.Models.Responses.Reports;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.Actions;

public class ReportActions : SmartlingInvocable
{
    private readonly string _accountUid;
    
    public ReportActions(InvocationContext invocationContext) : base(invocationContext)
    {
        _accountUid = GetAccountUid().Result;
    }

    [Action("Get word count report", Description = "Retrieve a word count report for specified parameters.")]
    public async Task<GetWordCountReportResponse> GetWordCountReport([ActionParameter] GetWordCountReportRequest input)
    {
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        var startDate = TimeZoneInfo.ConvertTime(input.StartDate, easternTimeZone).ToString("yyyy-MM-dd");
        var endDate = TimeZoneInfo.ConvertTime(input.EndDate ?? DateTime.Now, easternTimeZone).ToString("yyyy-MM-dd");
        var endpoint =
            $"/reports-api/v3/word-count?startDate={startDate}&endDate={endDate}&accountUid={_accountUid}&projectIds={ProjectId}";
        var request = new SmartlingRequest(endpoint, Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<WordCountReportDto>>>(request);
        var reports = response.Response.Data.Items;
        return new(reports);
    }
    
    [Action("Get word count report in CSV format", Description = "Retrieve a word count report in CSV format for " +
                                                                 "specified parameters.")]
    public async Task<FileWrapper> GetWordCountReportInCsvFormat([ActionParameter] GetWordCountReportRequest input)
    {
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        var startDate = TimeZoneInfo.ConvertTime(input.StartDate, easternTimeZone).ToString("yyyy-MM-dd");
        var endDate = TimeZoneInfo.ConvertTime(input.EndDate ?? DateTime.Now, easternTimeZone).ToString("yyyy-MM-dd");
        var endpoint =
            $"/reports-api/v3/word-count/csv?startDate={startDate}&endDate={endDate}&accountUid={_accountUid}&projectIds={ProjectId}";
        var request = new SmartlingRequest(endpoint, Method.Get);
        var response = await Client.ExecuteWithErrorHandling(request);
        
        return new()
        {
            File = new(response.RawBytes)
            {
                ContentType = response.ContentType,
                Name = response.ContentHeaders.First(h => h.Name == "Content-Disposition").Value.ToString().Split('"')[1]
            }
        };
    }
}