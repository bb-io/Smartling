using Apps.Smartling.Callbacks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.Callbacks.Handlers;

public class TranslationIssueCreatedCallbackHandler(InvocationContext invocationContext) : CallbackHandler(invocationContext)
{
    protected override string Event => "translationIssue.created";
}
