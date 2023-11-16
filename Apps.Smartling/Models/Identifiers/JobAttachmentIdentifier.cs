using Apps.Smartling.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Smartling.Models.Identifiers;

public class JobAttachmentIdentifier
{
    [Display("Attachment")]
    [DataSource(typeof(JobAttachmentDataSourceHandler))]
    public string AttachmentUid { get; set; }
}