using Apps.Smartling.Utils;
using RestSharp;

namespace Tests.Smartling;

[TestClass]
public class UploadDirectivesTests
{
    [TestMethod]
    public void AddDirectives_WithEmptyDirectiveNames_SkipsThemAndKeepsValidOnes()
    {
        var request = new RestRequest("/files-api/v2/projects/project-1/file", Method.Post);
        string[] directives = ["smartling.placeholder_format_custom", "", null!, " ", "smartling.process_slide_notes"];
        string[] directiveValues = ["\\{\\{.+?\\}\\}", "ignored-1", "ignored-2", "ignored-3", "false"];

        SmartlingUploadDirectivesHelper.AddDirectives(request, directives, directiveValues);

        var addedParameters = request.Parameters.Select(p => (p.Name, Value: p.Value?.ToString())).ToList();
        CollectionAssert.AreEqual(
            new List<(string?, string?)>
            {
                ("smartling.placeholder_format_custom", "\\{\\{.+?\\}\\}"),
                ("smartling.process_slide_notes", "false")
            },
            addedParameters);
    }

    [TestMethod]
    public void AddDirectives_WithoutDirectiveValues_AddsNothing()
    {
        var request = new RestRequest("/files-api/v2/projects/project-1/file", Method.Post);

        SmartlingUploadDirectivesHelper.AddDirectives(request, ["smartling.process_slide_notes"], null);

        Assert.AreEqual(0, request.Parameters.Count);
    }
}
