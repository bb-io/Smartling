using Apps.Smartling.Polling;
using Apps.Smartling.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;
using SmartlingTests.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Smartling
{
    [TestClass]
    public class PollingTests : TestBase
    {
        [TestMethod]
        public async Task OnJobCompleted()
        {
            var customDate = DateTime.ParseExact("Mar 27, 2025, 6:31 PM", "MMM dd, yyyy, h:mm tt", CultureInfo.InvariantCulture);

            var action = new PollingList(InvocationContext);
            var request = new PollingEventRequest<DateMemory>
            {
                Memory = new DateMemory { LastInteractionDate = customDate }
            };

            var response = await action.OnJobCompleted(request);
            Console.WriteLine($"Request memory: {request.Memory?.LastInteractionDate}");
            Console.WriteLine($"Response memory: {response.Memory?.LastInteractionDate}");
            Assert.IsNotNull(response);
        }
    }
}
