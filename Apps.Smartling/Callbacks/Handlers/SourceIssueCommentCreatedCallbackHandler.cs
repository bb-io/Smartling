using Apps.Smartling.Callbacks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Smartling.Callbacks.Handlers;

public class SourceIssueCommentCreatedCallbackHandler(InvocationContext invocationContext) : CallbackHandler(invocationContext)
{
    protected override string Event => "sourceIssue.comment.created";
}
