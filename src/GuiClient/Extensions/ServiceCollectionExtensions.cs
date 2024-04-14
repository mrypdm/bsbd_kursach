using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services,
        IConfiguration configuration, bool bindNonPublicProperties = true) where TOptions : class, new()
    {
        var options = new TOptions();

        var section = configuration.GetRequiredSection(typeof(TOptions).Name);
        section.Bind(options, o => o.BindNonPublicProperties = bindNonPublicProperties);

        return services.AddSingleton(options);
    }
}