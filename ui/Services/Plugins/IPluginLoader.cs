using System;
using System.Threading;
using System.Threading.Tasks;

namespace algo_vis.ui.Services.Plugins;

public interface IPluginLoader
{
    Task<Type[]> LoadAlgorithmsAsync(
        string folder,
        IProgress<PluginLoadProgress>? progress = null,
        CancellationToken cancellation = default);

    Task<Type[]> LoadVisualisersAsync(
        string folder,
        IProgress<PluginLoadProgress>? progress = null,
        CancellationToken cancellation = default);

    Task<Type[]> LoadConvertersAsync(
        string folder,
        IProgress<PluginLoadProgress>? progress = null,
        CancellationToken cancellation = default);
}