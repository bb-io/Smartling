using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Glossaries;

public class GlossaryEntryDto
{
    [Display("Entry")]
    public string EntryUid { get; set; }
    
    [Display("Glossary")]
    public string GlossaryUid { get; set; }
    
    public string Definition { get; set; }
    
    [Display("Part of speech")]
    public string? PartOfSpeech { get; set; }
    
    [Display("Labels")]
    public IEnumerable<string> LabelUids { get; set; }
    
    public IEnumerable<GlossaryEntryTranslationDto> Translations { get; set; }
    
    [Display("Custom field values")]
    public IEnumerable<GlossaryEntryCustomFieldDto> CustomFieldValues { get; set; }
    
    public bool Archived { get; set; }
    
    [Display("Created by user")]
    public string CreatedByUserUid { get; set; }
    
    [Display("Modified by user")]
    public string ModifiedByUserUid { get; set; }
    
    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }
    
    [Display("Date of last modification")]
    public DateTime ModifiedDate { get; set; }
}

public class GlossaryEntryTranslationDto
{
    [Display("Locale")]
    public string LocaleId { get; set; }
    
    [Display("Fallback locale")]
    public string? FallbackLocaleId { get; set; }
    
    public string Term { get; set; }
    
    public string? Notes { get; set; }
    
    [Display("Case sensitive")]
    public bool CaseSensitive { get; set; }
    
    [Display("Exact term matching")]
    public bool ExactMatch { get; set; }
    
    [Display("Do not translate")]
    public bool DoNotTranslate { get; set; }
    
    [Display("Translation disabled")]
    public bool Disabled { get; set; }
    
    [Display("Term variations")]
    public IEnumerable<string> Variants { get; set; }
    
    [Display("Custom field values")]
    public IEnumerable<GlossaryEntryTranslationCustomFieldDto> CustomFieldValues { get; set; }
    
    [Display("Created by user")]
    public string CreatedByUserUid { get; set; }
    
    [Display("Modified by user")]
    public string ModifiedByUserUid { get; set; }
    
    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }
    
    [Display("Date of last modification")]
    public DateTime ModifiedDate { get; set; }
}

public class GlossaryEntryTranslationCustomFieldDto
{
    [Display("Field")]
    public string FieldUid { get; set; }
    
    [Display("Field name")]
    public string FieldName { get; set; }
    
    [Display("Field value")]
    public string FieldValue { get; set; }
    
    [Display("Locale")]
    public string LocaleId { get; set; }
}

public class GlossaryEntryCustomFieldDto
{
    [Display("Field")]
    public string FieldUid { get; set; }
    
    [Display("Field name")]
    public string FieldName { get; set; }
    
    [Display("Field value")]
    public string FieldValue { get; set; }
}
