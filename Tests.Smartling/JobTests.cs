using Apps.Smartling.Actions;
using Apps.Smartling.Models.Identifiers;
using Apps.Smartling.Models.Requests.Jobs;
using SmartlingTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Smartling
{
    [TestClass]
    public class JobTests:TestBase
    {
        [TestMethod]
        public async Task CreateJob_IsSuccess()
        {
            var action = new JobActions(InvocationContext);
            var job = new CreateJobRequest { JobName="TestA" };
            var locale = new TargetLocalesIdentifier { TargetLocaleIds = ["fr-CA" ]};
            var response = action.CreateJob(job, locale);

            foreach (var jobInfo in response.Result.TargetLocaleIds)
            {
                Console.WriteLine($"{jobInfo}");
            }
           
            Assert.IsNotNull(response);
        }
    }
}
