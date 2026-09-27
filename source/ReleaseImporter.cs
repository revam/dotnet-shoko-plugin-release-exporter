using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Plugin;
using Shoko.Abstractions.Video.Release;

namespace Shoko.Plugin.ReleaseExporter;

/// <summary>
/// Responsible for importing releases from the file system near the video files.
/// </summary>
/// <param name="logger">Logger.</param>
/// <param name="applicationPaths">Application paths.</param>
/// <param name="configurationProvider">Configuration provider.</param>
public class ReleaseImporter(ILogger<ReleaseImporter> logger, IApplicationPaths applicationPaths, ConfigurationProvider<Configuration> configurationProvider) : IReleaseInfoProvider<Configuration>
{
    /// <inheritdoc/>
    public const string Key = "Release Importer";

    /// <inheritdoc/>
    public string Name { get; private set; } = Key;

    /// <inheritdoc/>
    public string Description { get; private set; } = """
        Responsible for importing releases from the file system near the video files.
    """;

    /// <inheritdoc/>
    public Task<ReleaseInfo?> GetReleaseInfoById(string releaseId, CancellationToken cancellationToken)
        => Task.FromResult<ReleaseInfo?>(null);

    /// <inheritdoc/>
    public async Task<ReleaseInfo?> GetReleaseInfoForVideo(ReleaseInfoContext context, CancellationToken cancellationToken)
    {
        var (video, _) = context;
        logger.LogTrace("Trying to find release for video. (Video={VideoID})", video.LocalID);
        var config = configurationProvider.Load();
        var releaseLocations = video.Files.SelectMany(l => config.GetReleaseFilePaths(applicationPaths, l.ManagedFolder, video, l.RelativePath)).ToHashSet();
        foreach (var releasePath in releaseLocations)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!File.Exists(releasePath))
                continue;

            try
            {
                var textData = await File.ReadAllTextAsync(releasePath, cancellationToken);
                if (ReadReleaseFile(textData) is not { } releaseInfo)
                    continue;

                cancellationToken.ThrowIfCancellationRequested();
                if (releaseInfo.CrossReferences.Count < 1)
                {
                    logger.LogWarning("Skipping release file without any usable cross-references: {ReleasePath}", releasePath);
                    continue;
                }

                if (string.IsNullOrEmpty(releaseInfo.ProviderName))
                    releaseInfo.ProviderName = Key;
                else if (!IsProvider(releaseInfo.ProviderName))
                    releaseInfo.ProviderName += "+" + Key;
                return releaseInfo;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Encountered an error reading release file: {ReleasePath}", releasePath);
            }
        }

        return null;
    }

    /// <summary>
    /// Reads an exported release file. Read as a plain <see cref="ReleaseInfo"/>, since
    /// <see cref="ReleaseInfoWithProvider"/> has no constructor Newtonsoft.Json can use.
    /// Files exported before the cross-references moved to <c>ProviderIDs</c> keep their
    /// AniDB IDs at the top of each cross-reference; those are moved over, and any
    /// cross-reference left without an ID is dropped.
    /// </summary>
    /// <param name="textData">The file contents.</param>
    /// <returns>The release, or <c>null</c> if the file holds no JSON object.</returns>
    /// <exception cref="JsonException">The file is not valid JSON, or does not match the release shape.</exception>
    private static ReleaseInfo? ReadReleaseFile(string textData)
    {
        if (JToken.Parse(textData) is not JObject json || json.ToObject<ReleaseInfo>() is not { } releaseInfo)
            return null;

        if (json["CrossReferences"] is JArray rawCrossReferences)
        {
            for (var index = 0; index < rawCrossReferences.Count && index < releaseInfo.CrossReferences.Count; index++)
            {
                var crossReference = releaseInfo.CrossReferences[index];
                if (crossReference.ProviderIDs.Count > 0 || rawCrossReferences[index] is not JObject rawCrossReference)
                    continue;

                crossReference.AnidbEpisodeID = rawCrossReference.Value<int?>("AnidbEpisodeID") ?? 0;
                crossReference.AnidbAnimeID = rawCrossReference.Value<int?>("AnidbAnimeID");
            }
        }

        releaseInfo.CrossReferences.RemoveAll(crossReference => crossReference.ProviderIDs.Count < 1);
        return releaseInfo;
    }

    private static bool IsProvider(string providerName) =>
        providerName is Key ||
        providerName.StartsWith($"{Key}+") ||
        providerName.EndsWith($"+{Key}") ||
        providerName.Contains($"+{Key}+");
}
