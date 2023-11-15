namespace Apps.Smartling.Models.Dtos.Locales;

public record SourceTargetLocalesPairDto(SourceLocaleIdDto SourceLocale, TargetLocaleIdDto TargetLocale);

public record SourceLocaleIdDto(string LocaleId);

public record TargetLocaleIdDto(string LocaleId);
