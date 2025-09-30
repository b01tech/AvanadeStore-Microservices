namespace Gateway.API.Extensions;

public static class OcelotExtensions
{
    public static ConfigurationManager AddOcelotConfigurations(
        this ConfigurationManager configuration
    )
    {
        configuration.AddJsonFile("Settings/ocelot.json", optional: false, reloadOnChange: true);
        return configuration;
    }
}
