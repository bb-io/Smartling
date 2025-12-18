using Apps.Smartling.Models.Dtos.Locales;

namespace Apps.Smartling.Models.Dtos.Project;

public class ProjectDtoWithTargetLocales : ProjectDto
{
    public IEnumerable<TargetLocaleDto> TargetLocales { get; set; }
}