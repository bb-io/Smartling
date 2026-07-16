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

    [TestMethod, ContextDataSource]
    public async Task RemoveTagsFromStrings_IsSuccess(InvocationContext context)
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
        await action.RemoveTagsFromStrings(project, strings, input);
    }

    [TestMethod, ContextDataSource]
    public async Task RemoveAllTagsFromStrings_IsSuccess(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var action = new TagActions(context);
        var strings = new StringHashcodesIdentifier
        {
            Hashcodes = new List<string> { "b226f88857ecc2003e840999237fe23f" }
        };

        // Act
        await action.RemoveAllTagsFromStrings(project, strings);
    }

    [TestMethod, ContextDataSource]
    public async Task GetAllTagsForStrings_IsSuccess(InvocationContext context)
    {
        // Arrange
        var project = new ProjectIdentifier { ProjectId = "2dbb9dabf" };
        var action = new TagActions(context);
        var strings = new StringHashcodesIdentifier
        {
            Hashcodes = new List<string> { "b226f88857ecc2003e840999237fe23f" }
        };

        // Act
        var response = await action.GetAllTagsForStrings(project, strings);

        // Assert
        PrintResult(response);
        Assert.IsNotNull(response);
    }
}
