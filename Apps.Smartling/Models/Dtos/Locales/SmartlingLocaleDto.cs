namespace Apps.Smartling.Models.Dtos.Locales;

public class SmartlingLocaleDto
{
    public string LocaleId { get; set; }
    public string Description { get; set; }
    public LanguageInfoDto Language { get; set; }
    public CountryInfoDto? Country { get; set; }
    public bool MtSupported { get; set; }
}

public class LanguageInfoDto
{
    public string LanguageId { get; set; }
    public string Description { get; set; }
    public string Direction { get; set; }
    public string WordDelimiter { get; set; }
}

public class CountryInfoDto
{
    public string CountryId { get; set; }
    public string Description { get; set; }
}