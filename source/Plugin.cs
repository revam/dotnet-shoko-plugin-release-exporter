using Microsoft.Extensions.DependencyInjection;
using Shoko.Abstractions.Plugin;

namespace Shoko.Plugin.ReleaseExporter;

/// <summary>
/// Plugin responsible for importing releases to and exporting releases from the
/// file system near the video files.
/// </summary>
public partial class Plugin : IPlugin<Configuration>, IPluginServiceRegistration
{
    /// <inheritdoc />
    public static void RegisterServices(IServiceCollection serviceCollection, IApplicationPaths applicationPaths)
    {
        serviceCollection.AddSingleton<ReleaseExporter>();
        serviceCollection.AddHostedService(sp => sp.GetRequiredService<ReleaseExporter>());
    }
}
