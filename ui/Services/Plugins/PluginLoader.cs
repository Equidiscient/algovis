using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using algo_vis.abstractions.Interfaces;

namespace algo_vis.ui.Services.Plugins
{
  public class PluginLoader : IPluginLoader
  {
    // cache one scan per folder
    private Task<Type[]>? _allTypesTask;
    private string?        _lastFolder;

    public Task<Type[]> LoadAlgorithmsAsync(
      string folder,
      IProgress<PluginLoadProgress>? progress = null,
      CancellationToken cancellation = default)
      => LoadFilteredAsync(
           folder,
           t => t.GetInterfaces()
                  .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAlgorithm<>)),
           progress,
           cancellation);

    public Task<Type[]> LoadVisualisersAsync(
      string folder,
      IProgress<PluginLoadProgress>? progress = null,
      CancellationToken cancellation = default)
      => LoadFilteredAsync(
           folder,
           t => t.GetInterfaces()
                  .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataVisualiser<>)),
           progress,
           cancellation);

    public Task<Type[]> LoadConvertersAsync(
      string folder,
      IProgress<PluginLoadProgress>? progress = null,
      CancellationToken cancellation = default)
      => LoadFilteredAsync(
           folder,
           t => t.GetInterfaces()
                  .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITypeConverter<,>)),
           progress,
           cancellation);


    private async Task<Type[]> LoadFilteredAsync(
      string folder,
      Func<Type,bool> predicate,
      IProgress<PluginLoadProgress>? progress,
      CancellationToken cancellation)
    {
      var all = await LoadAllTypesAsync(folder, progress, cancellation)
                  .ConfigureAwait(false);

      return all.Where(predicate).ToArray();
    }


    private async Task<Type[]> LoadAllTypesAsync(
      string folder,
      IProgress<PluginLoadProgress>? progress,
      CancellationToken cancellation)
    {
      // if we already scanned this folder, return the cached result
      if (_allTypesTask != null && _lastFolder == folder)
        return await _allTypesTask.ConfigureAwait(false);

      _lastFolder    = folder;
      _allTypesTask  = ScanFolderAsync(folder, progress, cancellation);
      return await _allTypesTask.ConfigureAwait(false);
    }

    private static async Task<Type[]> ScanFolderAsync(
      string folder,
      IProgress<PluginLoadProgress>? progress,
      CancellationToken cancellation)
    {
      if (!Directory.Exists(folder))
        return [];

      var dlls       = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);
      var discovered = new List<Type>();

      for (int i = 0; i < dlls.Length; i++)
      {
        cancellation.ThrowIfCancellationRequested();
        var dll = dlls[i];
        progress?.Report(new PluginLoadProgress(dll, i+1, dlls.Length));
        await Task.Yield();

        var alc = new AssemblyLoadContext(Path.GetFileNameWithoutExtension(dll), isCollectible:true);
        await using var fs = File.OpenRead(dll);
        var asm = alc.LoadFromStream(fs);

        foreach (var t in asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
          discovered.Add(t);
      }

      return discovered.ToArray();
    }
  }
}