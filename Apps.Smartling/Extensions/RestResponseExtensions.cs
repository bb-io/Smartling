using System.Net;
using RestSharp;

namespace Apps.Smartling.Extensions;

public static class RestResponseExtensions
{
    public static bool IsTransient(this RestResponse response)
    {
        return (int)response.StatusCode >= 500
               || response.StatusCode == 0
               || response.StatusCode == HttpStatusCode.TooManyRequests
               || response.StatusCode == HttpStatusCode.RequestTimeout;
    }
}