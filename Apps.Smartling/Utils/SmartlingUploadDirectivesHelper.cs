using RestSharp;

namespace Apps.Smartling.Utils;

public static class SmartlingUploadDirectivesHelper
{
    public static void AddDirectives(RestRequest request, IEnumerable<string>? directives, IEnumerable<string>? directiveValues)
    {
        if (directives == null || directiveValues == null)
            return;

        var directivePairs = directives
            .Zip(directiveValues, (key, value) => new { key, value })
            .Where(pair => !string.IsNullOrWhiteSpace(pair.key));

        foreach (var pair in directivePairs)
            request.AddParameter(pair.key, pair.value);
    }
}
