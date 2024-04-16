namespace Core.Constants;

internal static class ConfigConstants
{
    public const string FileName = "config.json";

    public const int PollingInterval_Min = 100;
    public const int PollingInterval_Max = 30_000;
    public const int PollingInterval_Default = 1000;
    
    public const int DeleteDelay_Min = 100;
    public const int DeleteDelay_Max = 30_000;
    public const int DeleteDelay_Default = 3000;
}
