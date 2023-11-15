using Apps.Smartling.Callbacks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.Callbacks.Handlers;

public class JobCompletedCallbackHandler : CallbackHandler
{
    protected override string Event => "job.completed";
    
    public JobCompletedCallbackHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}