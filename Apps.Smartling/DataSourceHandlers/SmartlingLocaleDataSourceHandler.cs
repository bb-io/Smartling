using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Locales;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class SmartlingLocaleDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public SmartlingLocaleDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new SmartlingRequest("/locales-api/v2/dictionary/locales", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<SmartlingLocaleDto>>>(request);
        var locales = response.Response.Data.Items
            .Where(locale => locale.MtSupported)
            .Where(locale => context.SearchString == null || locale.Description.Contains(context.SearchString))
            .ToDictionary(locale => locale.LocaleId, locale => locale.Description);
        return locales;
    }
}