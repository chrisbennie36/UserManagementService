namespace UserManagementService.Api.WebApplication.Extensions;

public static class ConfigurationManagerExtensions
{
    public static string GetStringValue (this ConfigurationManager configurationManager, string key)
    {
        string? value = configurationManager[key];

        ArgumentNullException.ThrowIfNull(value);

        return value;
    }

    public static bool GetBoolValue(this ConfigurationManager configurationManager, string key)
    {
        string stringConfigValue = GetStringValue(configurationManager, key);

        bool boolConfigValue = false;

        if(!Boolean.TryParse(stringConfigValue, out boolConfigValue))
        {
            throw new ArgumentException($"Config value: {key} could not be parsed to a boolean");
        }

        return boolConfigValue;
    }
}
