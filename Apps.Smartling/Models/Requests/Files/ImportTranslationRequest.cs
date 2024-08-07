﻿using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Files;

public class ImportTranslationRequest
{
    [Display("Translation state")] 
    [StaticDataSource(typeof(TranslationStateDataSourceHandler))]
    public string TranslationState { get; set; }
    
    [Display("Overwrite existing translations")]
    public bool? Overwrite { get; set; }
}