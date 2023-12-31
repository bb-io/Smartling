﻿using Apps.Smartling.Constants;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
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
}