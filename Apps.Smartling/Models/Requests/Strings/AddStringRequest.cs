using Apps.Smartling.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Requests.Strings;

public class AddStringRequest
{
    [Display("String text")]
    public string StringText { get; set; }
    
    [Display("Instruction for translators")]
    public string? Instruction { get; set; }
    
    [Display("Variant metadata")]
    public string? VariantMetadata { get; set; } 
    
    [Display("Maximum character length")]
    public int? MaxLength { get; set; }
    
    [StaticDataSource(typeof(StringFormatDataSourceHandler))]
    public string? Format { get; set; }
    
    [Display("Callback URL")]
    public string? CallbackUrl { get; set; }
    
    [Display("Callback method")]
    [StaticDataSource(typeof(CallbackMethodDataSourceHandler))]
    public string? CallbackMethod { get; set; }
    
    [Display("Placeholder format")]
    [StaticDataSource(typeof(PlaceholderFormatDataSourceHandler))]
    public string? PlaceholderFormat { get; set; }
    
    [Display("Custom placeholder format (Java Regular Expression)")]
    public string? PlaceholderFormatCustom { get; set; }
    
    public string? Namespace { get; set; }
}