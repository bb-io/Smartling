﻿using Apps.Smartling.DataSourceHandlers;
using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Issues;

public class CreateIssueRequest
{
    [Display("Issue text")]
    public string IssueText { get; set; }
    
    [Display("Issue type")]
    [DataSource(typeof(IssueTypeCodeDataSourceHandler))]
    public string IssueTypeCode { get; set; }
    
    [Display("Issue subtype")]
    [DataSource(typeof(IssueSubTypeCodeDataSourceHandler))]
    public string IssueSubTypeCode { get; set; }
    
    [Display("Issue severity level")]
    [DataSource(typeof(IssueSeverityLevelCodeDataSourceHandler))]
    public string? IssueSeverityLevelCode { get; set; }
}