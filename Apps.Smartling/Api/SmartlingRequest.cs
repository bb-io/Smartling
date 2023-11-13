using RestSharp;

namespace Apps.Smartling.Api;

public class SmartlingRequest : RestRequest
{
    public SmartlingRequest(string endpoint, Method method) : base(endpoint, method)
    {
    }
}