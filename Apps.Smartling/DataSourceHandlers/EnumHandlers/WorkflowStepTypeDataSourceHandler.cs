using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class WorkflowStepTypeDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "Translation", "Translation" },
            { "Edit", "Edit" },
            { "Review", "Review" },
            { "Post-Edit", "Post-Edit" },
            { "Transcreation", "Transcreation" },
            { "Transcreation review", "Transcreation review" },
            { "AI Review", "AI Review" },
            { "Quality Evaluation", "Quality Evaluation" },
            { "LQA", "LQA" },
            { "Internal Review", "Internal Review" },
            { "Desktop Publishing", "Desktop Publishing" },
            { "AI Translation", "AI Translation" },
            { "Light Post-Edit", "Light Post-Edit" }
        };
    }
}
