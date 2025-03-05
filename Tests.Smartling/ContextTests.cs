using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Smartling.Actions;
using Apps.Smartling.Models.Requests.Context;
using Blackbird.Applications.Sdk.Common.Files;
using SmartlingTests.Base;

namespace Tests.Smartling
{
    [TestClass]
    public class ContextTests : TestBase
    {
        [TestMethod]
        public async Task UploadNewContext_IsSuccess()
        {
            var action = new ContextActions(InvocationContext,FileManager);

            var response = await action.UploadNewContext(new AddProjectContextRequest
            {
                ContextFile = new FileReference { Name = "test.MOV" },
                Name = "FirstFileMOV",
                ContextMatching = true
            });

            Console.WriteLine(response.Response.Code);
            Console.WriteLine(response.Response.Data.ContextUid);
            Console.WriteLine(response.Response.Data.ProcessUid);
            Assert.IsNotNull(response);
        }

        //[TestMethod]
        //public async Task LinkContextToString_IsSuccess()
        //{
        //    var action = new ContextActions(InvocationContext, FileManager);

        //    var response = await action.LinkContextToString(new AddProjectContextRequest
        //    {
        //        ContextFile = new FileReference { Name = "test.MOV" },
        //        Name = "FirstFileMOV",
        //        ContextMatching = true
        //    });

        //    Console.WriteLine(response.Response.Code);
        //    Console.WriteLine(response.Response.Data.ContextUid);
        //    Console.WriteLine(response.Response.Data.ProcessUid);
        //    Assert.IsNotNull(response);
        //}
    }
}
