using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Callbacks.Models.Payload.Jobs;

public abstract class TranslationJobWebhookPayload
{
    [Display("Event ID")]
    public string EventId { get; set; } = string.Empty;

    [Display("Event type")]
    public string EventType { get; set; } = string.Empty;

    [Display("Schema version")]
    public string SchemaVersion { get; set; } = string.Empty;

    public TranslationJobWebhookAccountDto Account { get; set; } = new();

    public TranslationJobWebhookProjectDto Project { get; set; } = new();

    [Display("Translation job")]
    public TranslationJobWebhookJobDto TranslationJob { get; set; } = new();
}

public class TranslationJobWebhookAccountDto
{
    [Display("Account ID")]
    public string AccountUid { get; set; } = string.Empty;

    [Display("Account name")]
    public string? AccountName { get; set; }
}

public class TranslationJobWebhookProjectDto
{
    [Display("Project ID")]
    public string ProjectUid { get; set; } = string.Empty;

    [Display("Project name")]
    public string? ProjectName { get; set; }

    [Display("Project type")]
    public string? ProjectTypeCode { get; set; }

    [Display("Source locale")]
    public TranslationJobWebhookLocaleDto? SourceLocale { get; set; }

    [Display("Target locales")]
    public IEnumerable<TranslationJobWebhookLocaleDto>? TargetLocales { get; set; }
}

public class TranslationJobWebhookJobDto
{
    [Display("Job ID")]
    public string JobUid { get; set; } = string.Empty;

    [Display("Job name")]
    public string? JobName { get; set; }

    [Display("Job status")]
    public string? JobStatus { get; set; }

    [Display("Job number")]
    public string? JobNumber { get; set; }

    [Display("Job description")]
    public string? JobDescription { get; set; }

    [Display("Job due date")]
    public DateTime? JobDueDate { get; set; }

    [Display("Reference number")]
    public string? ReferenceNumber { get; set; }

    [Display("Rush request")]
    public bool? RushRequest { get; set; }

    [Display("Created date")]
    public DateTime? CreatedDate { get; set; }

    [Display("Updated date")]
    public DateTime? UpdatedDate { get; set; }

    [Display("Created by user")]
    public TranslationJobWebhookUserDto? CreatedByUser { get; set; }

    [Display("Updated by user")]
    public TranslationJobWebhookUserDto? UpdatedByUser { get; set; }

    [Display("Authorized by user")]
    public TranslationJobWebhookUserDto? AuthorizedByUser { get; set; }

    [Display("Target locales")]
    public IEnumerable<TranslationJobWebhookTargetLocaleDto>? TargetLocales { get; set; }

    public IEnumerable<TranslationJobWebhookFileDto>? Files { get; set; }

    [Display("Custom fields")]
    public IEnumerable<TranslationJobWebhookCustomFieldContainerDto>? CustomFields { get; set; }
}

public class TranslationJobWebhookUserDto
{
    [Display("User ID")]
    public string? UserUid { get; set; }

    [Display("First name")]
    public string? FirstName { get; set; }

    [Display("Last name")]
    public string? LastName { get; set; }
}

public class TranslationJobWebhookLocaleDto
{
    [Display("Locale ID")]
    public string LocaleId { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class TranslationJobWebhookTargetLocaleDto : TranslationJobWebhookLocaleDto
{
    public string? Status { get; set; }
}

public class TranslationJobWebhookFileDto
{
    [Display("File ID")]
    public string? FileUid { get; set; }

    [Display("File URI")]
    public string? FileUri { get; set; }

    [Display("Locale IDs")]
    public IEnumerable<string>? LocaleIds { get; set; }
}

public class TranslationJobWebhookCustomFieldContainerDto
{
    [Display("Custom field")]
    public TranslationJobWebhookCustomFieldDto? CustomField { get; set; }
}

public class TranslationJobWebhookCustomFieldDto
{
    [Display("Custom field value")]
    public string? CustomFieldValue { get; set; }

    [Display("Custom field ID")]
    public string? CustomFieldUid { get; set; }

    [Display("Custom field type")]
    public string? CustomFieldType { get; set; }

    [Display("Custom field name")]
    public string? CustomFieldName { get; set; }

    [Display("Custom field description")]
    public string? CustomFieldDescription { get; set; }
}
