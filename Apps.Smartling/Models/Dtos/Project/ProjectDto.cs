namespace Apps.Smartling.Models.Dtos.Project;

public class ProjectDto
{
    public string ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string AccountUid { get; set; }
    public bool Archived { get; set; }
    public string SourceLocaleId { get; set; }
    public string SourceLocaleDescription { get; set; }
}
