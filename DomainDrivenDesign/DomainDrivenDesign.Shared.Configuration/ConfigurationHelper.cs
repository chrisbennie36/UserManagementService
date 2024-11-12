using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DomainDrivenDesign.Shared.Configuration;

public class ConfigurationHelper<T>
{
    private ConfigurationManager configurationManager;
    private IServiceCollection serviceCollection;
    private WebApplicationBuilder builder

    public ConfigurationHelper(ConfigurationManager configurationManager, IServiceCollection serviceCollection)
    {
        this.configurationManager = configurationManager;
        this.serviceCollection = serviceCollection;
    }

    public ConfigurationSection GetConfiguration(string key)
    {
        return this.serviceCollection.Configure()
    }

    public 
}
