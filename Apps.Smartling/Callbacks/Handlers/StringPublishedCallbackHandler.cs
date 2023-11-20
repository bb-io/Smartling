using Apps.Smartling.Callbacks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.Callbacks.Handlers;

public class StringPublishedCallbackHandler : CallbackHandler
{
    protected override string Event => "string.localeCompleted";
    
    public StringPublishedCallbackHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}