using Tests.Smartling.Base;
using Apps.Smartling.Actions;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Tags;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Tests.Smartling;

[TestClass]
public class TagTests : TestBaseMultipleConnections
{
    [TestMethod, ContextDataSource]
    public async Task AddTagsToStrings_IsSuccess(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var action = new TagActions(context);
        var strings = new StringHashcodesIdentifier
        {
            Hashcodes = new List<string> { "b226f88857ecc2003e840999237fe23f" }
        };
        var input = new AddTagsRequest
        {
            Tags = new List<string> { "BB Test tag 2" }
        };

        // Act
        await action.AddTagsToStrings(project, strings, input);
    }
}
