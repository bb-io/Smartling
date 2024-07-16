using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Smartling.DataSourceHandlers.EnumHandlers;

public class PartOfSpeechDataSourceHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "NOUN", "Noun" },
            { "VERB", "Verb" },
            { "ADJECTIVE", "Adjective" },
            { "ADVERB", "Adverb" },
            { "PRONOUN", "Pronoun" },
            { "PREPOSITION", "Preposition" },
            { "INTERJECTION", "Interjection" },
            { "CONJUNCTION", "Conjunction" },
            { "PROPER_NOUN", "Proper Noun" }
        };
    }
}