using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Locales;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

// This data source handler is responsible for retrieving all available locales
public class SmartlingLocaleDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public SmartlingLocaleDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new SmartlingRequest("/locales-api/v2/dictionary/locales?supportedOnly=true", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<LocaleDto>>>(request);
        var targetLocales = response.Response.Data.Items
            .Where(locale => context.SearchString == null 
                             || locale.Description.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(locale => locale.LocaleId, locale => locale.Description);
        return targetLocales;
    }
}