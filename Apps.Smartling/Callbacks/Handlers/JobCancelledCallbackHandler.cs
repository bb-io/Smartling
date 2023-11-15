using Apps.Smartling.Callbacks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.Callbacks.Handlers;

public class JobCancelledCallbackHandler : CallbackHandler
{
    protected override string Event => "job.cancelled";
    
    public JobCancelledCallbackHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}