﻿using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class FileIdentifier
{
    [Display("File")]
    [DataSource(typeof(FileDataSourceHandler))]
    public string FileUri { get; set; }
}