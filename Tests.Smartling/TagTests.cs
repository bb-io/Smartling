using Apps.Smartling.Actions;
using SmartlingTests.Base;

namespace Tests.Smartling
{
    [TestClass]
    public class TagTests : TestBase
    {
        [TestMethod]
        public async Task AddTagsToStrings_WorksAsExpected()
        {
           var action = new TagActions(InvocationContext);
            var strings = new Apps.Smartling.Models.Identifiers.StringHashcodesIdentifier
            {
                Hashcodes = new List<string> { "b226f88857ecc2003e840999237fe23f" }
            };
            var input = new Apps.Smartling.Models.Requests.Tags.AddTagsRequest
            {
                Tags = new List<string> { "BB Test tag 2" }
            };
            await action.AddTagsToStrings(strings, input);

            Assert.IsTrue(true);
        }
    }
}
