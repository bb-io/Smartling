using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling;

public class SmartlingApplication : IApplication
{
    public string Name
    {
        get => "Smartling";
        set { }
    }

    public T GetInstance<T>() => throw new NotImplementedException();
}