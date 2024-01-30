﻿using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class GlossaryLocalesIdentifier
{
    [Display("Locale IDs")]
    [DataSource(typeof(SmartlingLocaleDataSourceHandler))]
    public IEnumerable<string> LocaleIds { get; set; }
}