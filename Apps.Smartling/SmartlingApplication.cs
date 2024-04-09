using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Smartling;

public class SmartlingApplication : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.CatAndTms];
        set { }
    }
    
    public string Name
    {
        get => "Smartling";
        set { }
    }

    public T GetInstance<T>() => throw new NotImplementedException();
}