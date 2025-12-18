namespace Apps.Smartling.Constants;

public static class ConnectionTypes
{
    public const string ProjectWide = "API key";    // Legacy value. Do NOT change!
    public const string AccountWide = "Account-wide";

    public static readonly List<string> SupportedConnectionTypes = [ProjectWide, AccountWide];
}
