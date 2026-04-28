using System;
using Microsoft.Extensions.DependencyInjection;
using Shoko.Abstractions.Plugin;

namespace Shoko.Plugin.ReleaseExporter;

/// <summary>
/// Plugin responsible for importing releases to and exporting releases from the
/// file system near the video files.
/// </summary>
public class Plugin : IPlugin, IPluginServiceRegistration
{
    /// <inheritdoc/>
    public Guid ID { get; private init; } = new("031ccc48-94d6-5100-a9e0-e26c49a33319");

    /// <inheritdoc/>
    public string Name { get; private set; } = "Release Importer/Exporter";

    /// <inheritdoc/>
    public string Description { get; private set; } = """
        Responsible for importing releases to and exporting releases from the
        file system near the video files.
    """;

    /// <inheritdoc />
    public static void RegisterServices(IServiceCollection serviceCollection, IApplicationPaths applicationPaths)
    {
        serviceCollection.AddSingleton<ReleaseExporter>();
        serviceCollection.AddHostedService(sp => sp.GetRequiredService<ReleaseExporter>());
    }
}
