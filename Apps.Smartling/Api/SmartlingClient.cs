using Apps.Smartling.Constants;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Smartling.Api;

public class SmartlingClient : BlackBirdRestClient
{
    protected override JsonSerializerSettings JsonSettings =>
        new() { MissingMemberHandling = MissingMemberHandling.Ignore };

    public SmartlingClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        : base(new RestClientOptions
            { ThrowOnAnyError = false, BaseUrl = new Uri(Urls.ApiUrl) })
    {
        var accessToken = GetAccessToken(authenticationCredentialsProviders);
        this.AddDefaultHeader("Authorization", $"Bearer {accessToken}");
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var errors = JsonConvert.DeserializeObject<ErrorResponseWrapper>(response.Content);
        return new($"{errors.Response.Code}: {string.Join("; ", errors.Response.Errors.Select(error => error.Message))}");
    }

    private string GetAccessToken(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var client = new RestClient(new Uri(Urls.ApiUrl));
        var request = new RestRequest("/auth-api/v2/authenticate", Method.Post);
        request.AddJsonBody(new
        {
            userIdentifier = authenticationCredentialsProviders.Get(CredsNames.UserIdentifier).Value,
            userSecret = authenticationCredentialsProviders.Get(CredsNames.UserSecret).Value
        });
        
        var response = client.Execute(request);

        if (!response.IsSuccessful)
            throw new("Failed to authorize. Please check the validity of your user identifier and secret.");

        var accessToken = JsonConvert.DeserializeObject<ResponseWrapper<AccessTokenDto>>(response.Content).Response.Data
            .AccessToken;
        return accessToken;
    }

    public virtual async Task<AsyncProcessResult> ExecuteAsyncProcessWithHandling(RestRequest request, string projectId)
    {
        var content = await ExecuteWithErrorHandling<ResponseWrapper<AsyncProcessDto>>(request);
        var checkAsyncResultRequest = new SmartlingRequest($"/context-api/v2/projects/{projectId}/processes/{content.Response.Data.ProcessUid}", Method.Get);

        while (true)
        {
            var asyncProcessResult = await this.ExecuteWithErrorHandling<ResponseWrapper<AsyncProcessResult>>(checkAsyncResultRequest);
            if (asyncProcessResult.Response.Data.ProcessState != "IN_PROGRESS")
                return asyncProcessResult.Response.Data;
            await Task.Delay(1000);
        }
    }

    public async Task<List<T>> Paginate<T>(RestRequest request)
    {
        var result = new List<T>();
        ResponseWrapper<ItemsWrapper<T>> response = null;
        do
        {
            if(response != null && response.Response.Data.Offset != null)
            {
                request.AddQueryParameter("offset", response.Response.Data.Offset.ToString());
            }
            response = await ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<T>>>(request);
            result.AddRange(response.Response.Data.Items ?? Enumerable.Empty<T>());
        } while (response.Response?.Data?.Offset != null);

        return result;
    }
}