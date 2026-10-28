using Apps.Smartling.Actions;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.MachineTranslation;
using Apps.Smartling.Models.Requests.MachineTranslation;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests.Smartling;

[TestClass]
public class MachineTranslationActionTests
{
    private static ApplyMachineTranslationRequest ValidInput() => new()
    {
        SourceStrings = ["<p>Hello &amp; welcome</p>", "Welcome!"],
        Keys = ["key-1", "key-2"],
        SourceLocaleId = "en-US",
        TargetLocaleId = "fr-FR"
    };

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("profile-id")]
    public void Request_PairsKeysAndPreservesHtml_AndOmitsEmptyProfile(string? profile)
    {
        var input = ValidInput();
        input.ProfileUid = profile;
        var (body, keys) = MachineTranslationActions.CreateRequestBody(input);
        var json = JObject.Parse(JsonConvert.SerializeObject(body));

        CollectionAssert.AreEqual(new[] { "key-1", "key-2" }, keys);
        Assert.AreEqual("en-US", (string?)json["sourceLocaleId"]);
        Assert.AreEqual("fr-FR", (string?)json["targetLocaleId"]);
        Assert.AreEqual("key-1", (string?)json["items"]![0]!["key"]);
        Assert.AreEqual("<p>Hello &amp; welcome</p>", (string?)json["items"]![0]!["sourceText"]);
        Assert.AreEqual("Welcome!", (string?)json["items"]![1]!["sourceText"]);
        Assert.AreEqual(!string.IsNullOrWhiteSpace(profile), json.ContainsKey("profileUid"));
        if (!string.IsNullOrWhiteSpace(profile))
            Assert.AreEqual(profile, (string?)json["profileUid"]);
    }

    [TestMethod]
    [DataRow("mismatch")]
    [DataRow("empty")]
    [DataRow("too-many")]
    [DataRow("empty-key")]
    [DataRow("null-key")]
    [DataRow("duplicate")]
    [DataRow("source-locale")]
    [DataRow("target-locale")]
    [DataRow("null-sources")]
    [DataRow("null-keys")]
    public void Request_RejectsInvalidInput(string scenario)
    {
        var input = ValidInput();
        switch (scenario)
        {
            case "mismatch": input.Keys = ["key-1"]; break;
            case "empty": input.Keys = []; input.SourceStrings = []; break;
            case "too-many":
                input.Keys = Enumerable.Range(0, 1001).Select(x => x.ToString());
                input.SourceStrings = Enumerable.Repeat("Hello", 1001);
                break;
            case "empty-key": input.Keys = ["key-1", " "]; break;
            case "null-key": input.Keys = ["key-1", null!]; break;
            case "duplicate": input.Keys = ["key-1", "key-1"]; break;
            case "source-locale": input.SourceLocaleId = " "; break;
            case "target-locale": input.TargetLocaleId = null!; break;
            case "null-sources": input.SourceStrings = null!; break;
            case "null-keys": input.Keys = null!; break;
        }

        Assert.ThrowsExactly<PluginMisconfigurationException>(() => MachineTranslationActions.CreateRequestBody(input));
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(1000)]
    public void Request_AcceptsItemCountBoundaries(int count)
    {
        var input = ValidInput();
        input.SourceStrings = Enumerable.Repeat("Hello", count);
        input.Keys = Enumerable.Range(0, count).Select(x => x.ToString());
        var (body, keys) = MachineTranslationActions.CreateRequestBody(input);
        Assert.HasCount(count, keys);
        Assert.AreEqual(count, JArray.FromObject(body["items"]).Count);
    }

    [TestMethod]
    public void Response_UnwrapsAndAlignsByKey_PreservingErrorsAndMissingResults()
    {
        const string json = """
            {"response":{"code":"SUCCESS","data":{"items":[
              {"key":"key-2","error":{"code":"MT_ERROR","message":"Provider failed","details":{"retryable":true}}},
              {"key":"key-1","translationText":"<p>Bonjour &amp; bienvenue</p>","mtUid":"mt-1","provider":"AUTO_SELECT_PROVIDER"}
            ]}}}
            """;
        var wrapper = JsonConvert.DeserializeObject<ResponseWrapper<ItemsWrapper<MachineTranslationApiItemDto>>>(json)!;
        var result = MachineTranslationActions.MapResponse(["key-1", "key-2", "missing"], wrapper.Response.Data.Items);
        var items = result.Items.ToArray();

        CollectionAssert.AreEqual(new[] { "<p>Bonjour &amp; bienvenue</p>", "", "" }, result.TranslatedStrings.ToArray());
        CollectionAssert.AreEqual(new[] { "key-1", "key-2", "missing" }, items.Select(x => x.Key).ToArray());
        Assert.AreEqual("mt-1", items[0].MtUid);
        Assert.AreEqual("AUTO_SELECT_PROVIDER", items[0].Provider);
        Assert.IsNull(items[0].Error);
        var error = JObject.Parse(items[1].Error!);
        Assert.AreEqual("MT_ERROR", (string?)error["code"]);
        Assert.IsTrue((bool?)error["details"]!["retryable"]);
        Assert.IsNotNull(items[2].Error);
    }

    [TestMethod]
    public void Response_MissingItemsRetainsEveryInputKey()
    {
        var result = MachineTranslationActions.MapResponse(["key-1"], null);
        Assert.AreEqual("", result.TranslatedStrings.Single());
        Assert.IsNotNull(result.Items.Single().Error);
    }
}
