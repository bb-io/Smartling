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
            var job = new CreateJobRequest { JobName="TestЕуіе", CallbackUrl= "https://webhook.site/#!/view/46153d60-6c08-4081-9ab3-7ba5d527fec8/53e0cd33-1ad0-41dc-8000-3edd768d994b/1" };
            var locale = new TargetLocalesIdentifier { /*TargetLocaleIds = ["fr-CA" ]*/};
            var response = action.CreateJob(job, locale);

            foreach (var jobInfo in response.Result.TargetLocaleIds)
            {
                Console.WriteLine($"{jobInfo}");
            }
            Console.WriteLine($"{response.Result.ReferenceNumber}");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task AddStringJob_IsSuccess()
        {
            var action = new StringActions(InvocationContext);
            var job = new JobIdentifier { TranslationJobUid= "zapjjrjaddrc" };
            var stringIdentifier = new StringIdentifier { Hashcode = "d2a79dda4a4ssfba3b62729003f7cccf" };
            var locale = new TargetLocalesIdentifier {  };
            var response = await action.AddStringToJob(job, stringIdentifier, locale, false);

            Assert.IsNotNull(response);
        }
    }
}
