namespace Gateway.API.Extensions;

public static class OcelotExtensions
{
    public static ConfigurationManager AddOcelotConfigurations(this ConfigurationManager configuration)
    {
        configuration.AddJsonFile("Settings/ocelot.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile("Settings/ocelot.auth.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile("Settings/ocelot.sales.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile("Settings/ocelot.inventory.json", optional: false, reloadOnChange: true);
        return configuration;
    }
}
