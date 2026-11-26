using Apps.Smartling.Constants;
using Apps.Smartling.Extensions;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;

namespace Apps.Smartling.Api;

public sealed class SmartlingClient : BlackBirdRestClient
{
    private static readonly ResiliencePipeline<RestResponse> RetryPipeline = new ResiliencePipelineBuilder<RestResponse>()
        .AddRetry(new RetryStrategyOptions<RestResponse>()
        {
            ShouldHandle = new PredicateBuilder<RestResponse>().HandleResult(x => x.IsTransient()),
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(2),
            UseJitter = true,
            BackoffType = DelayBackoffType.Exponential
        })
        .Build();
    
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
        string statusCodePart = $"Status {(int)response.StatusCode}.";
        
        if (string.IsNullOrWhiteSpace(response.Content))
            throw new PluginApplicationException($"{statusCodePart} No content returned by Smartling. {response.ErrorMessage}".Trim());

        switch (response.ContentType)
        {
            case "application/json":
            {
                var errors = JsonConvert.DeserializeObject<ErrorResponseWrapper>(response.Content) ??
                             throw new PluginApplicationException($"{statusCodePart} Cannot deserialize the error response. {response.Content}");

                string messages = string.Join("; ", errors.Response.Errors.Select(error => error.Message));
                throw new PluginApplicationException($"{errors.Response.Code}: {messages}");
            }
            case "text/html":
            {
                string htmlSnippet = response.Content.Length > 300
                    ? response.Content[..300]
                    : response.Content;
                throw new PluginApplicationException($"{statusCodePart} HTML error (truncated): {htmlSnippet}");
            }
            default:
                throw new PluginApplicationException($"{statusCodePart} Raw response: {response.Content}");
        }
    }

    private static string GetAccessToken(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var client = new RestClient(new Uri(Urls.ApiUrl));
        var request = new RestRequest("/auth-api/v2/authenticate", Method.Post);
        request.AddJsonBody(new
        {
            userIdentifier = authenticationCredentialsProviders.Get(CredsNames.UserIdentifier).Value,
            userSecret = authenticationCredentialsProviders.Get(CredsNames.UserSecret).Value
        });
        
        var response = RetryPipeline.Execute(() => client.Execute(request));
        if (!response.IsSuccessful)
            throw new PluginApplicationException($"Failed to authorize ({(int)response.StatusCode}): {response.Content ?? response.ErrorMessage}");

        var accessToken = JsonConvert.DeserializeObject<ResponseWrapper<AccessTokenDto>>(response.Content).Response.Data
            .AccessToken;
        return accessToken;
    }

    public async Task<AsyncProcessResult> ExecuteAsyncProcessWithHandling(RestRequest request, string projectId)
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

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        string content = (await ExecuteWithErrorHandling(request)).Content;
        T val = JsonConvert.DeserializeObject<T>(content, JsonSettings);
        if (val == null)
        {
            throw new Exception($"Could not parse {content} to {typeof(T)}");
        }

        return val;
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        RestResponse restResponse = await RetryPipeline.ExecuteAsync(ct => new ValueTask<RestResponse>(ExecuteAsync(request, ct)));
        if (!restResponse.IsSuccessStatusCode)
        {
            throw ConfigureErrorException(restResponse);
        }

        return restResponse;
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
    
    public async Task<List<T>> PaginateByOffset<T>(RestRequest request)
    {
        var result = new List<T>();
        int totalCount;

        do
        {
            request.AddOrUpdateParameter(new QueryParameter("offset", result.Count.ToString()));
            var response = await ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<T>>>(request);
            var items = response.Response.Data.Items?.ToList() ?? [];

            if (items.Count == 0)
                break;

            result.AddRange(items);
            totalCount = response.Response.Data.TotalCount ?? 0;
        } while (result.Count < totalCount);

        return result;
    }
}