using SmartlingTests.Base;
using Apps.Smartling.Connections;
using Blackbird.Applications.Sdk.Common.Authentication;
using Tests.Smartling.Base;

namespace Tests.Smartling;

[TestClass]
public class Validator : TestBase
{
    [TestMethod, ContextDataSource]
    public async Task ValidatesCorrectConnection(Blackbird.Applications.Sdk.Common.Invocation.InvocationContext context)
    {
        // Arrange
        var validator = new ConnectionValidator();

        // Act
        var result = await validator.ValidateConnection(context.AuthenticationCredentialsProviders, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task DoesNotValidateIncorrectConnection()
    {
        // Arrange
        var validator = new ConnectionValidator();

        var newCreds = CredsGroups.First().Select(x => new AuthenticationCredentialsProvider(x.KeyName, x.Value + "_incorrect"));
        var result = await validator.ValidateConnection(newCreds, CancellationToken.None);
        Assert.IsFalse(result.IsValid);
    }
}
