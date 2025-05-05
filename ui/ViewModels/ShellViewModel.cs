using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using algo_vis.builtins.algorithms;
using algo_vis.core.Attributes;
using algo_vis.core.Interfaces;
using algo_vis.ui.Models;
using algo_vis.ui.Services.Plugins;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace algo_vis.ui.ViewModels;

public partial class ShellViewModel : ViewModelBase
{
    public ObservableCollection<AlgorithmMenuItem> AllAlgorithms { get; }
    public DataGridCollectionView                 AlgorithmsView { get; }

    private readonly IPluginLoader  _pluginLoader;
    private readonly string         _pluginFolder;

    [ObservableProperty] private string              _discoveryStatus = "Idle";
    [ObservableProperty] private string              _searchText      = "";
    [ObservableProperty] private AlgorithmMenuItem? _selectedAlgorithm;
    [ObservableProperty] private bool                _isFlyoutOpen;
    [ObservableProperty] private object              _currentPage;
    [ObservableProperty] private int    _progressCurrent;
    [ObservableProperty] private int    _progressTotal;


    public ShellViewModel()
    {
        IsFlyoutOpen = true;
        _pluginLoader = new PluginLoader();
        _pluginFolder = Path.Combine(AppContext.BaseDirectory, "Plugins");

        CurrentPage   = MarkdownRendererViewModel.Instance;
        AllAlgorithms = [];
        AlgorithmsView = new DataGridCollectionView(AllAlgorithms)
        {
            GroupDescriptions =
            {
                new DataGridPathGroupDescription(nameof(AlgorithmMenuItem.GroupKey))
            },
            Filter = o =>
            {
                if (o is not AlgorithmMenuItem it) return false;
                if (string.IsNullOrWhiteSpace(SearchText)) return true;
                var s = SearchText.Trim();
                return it.Name.Contains((string)s, StringComparison.InvariantCultureIgnoreCase)
                       || it.Tags.Any(t => t.Contains((string)s, StringComparison.InvariantCultureIgnoreCase));
            }
        };
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SearchText))
                AlgorithmsView.Refresh();
        };

        _ = LoadAlgorithmsAsync();
    }

    private async Task LoadAlgorithmsAsync()
    {
        DiscoveryStatus = "Discovering…";

        // Get built-ins via PluginLoader
        var builtInAlgos = _pluginLoader.LoadBuiltInAlgorithms();
        var builtInVis   = _pluginLoader.LoadBuiltInVisualisers();

        Type[] pluginAlgos = [], pluginVis = [];
        try { pluginVis   = await _pluginLoader.LoadVisualisersAsync(_pluginFolder); }
        catch (Exception ex) { Debug.WriteLine($"exception loading visualisers: {ex.Message}"); }
        try { pluginAlgos = await _pluginLoader.LoadAlgorithmsAsync(_pluginFolder); }
        catch (Exception ex) { Debug.WriteLine($"exception loading plugins: {ex.Message}"); }

        // 1) Build VisualiserKey and DataType maps
        PluginRegistry.VisualiserKeys.Clear();
        PluginRegistry.VisualiserByDataType.Clear();
        var allVis = builtInVis.Concat(pluginVis).ToList();
        foreach (var v in allVis)
        {
            // VisualiserKey = Name by your requirements
            var visualiserKey = v.Name;
            PluginRegistry.VisualiserKeys.TryAdd(visualiserKey, v);

            // Find IVisualiser<T> and map T to this visualiser for fallback
            var iface = v.GetInterfaces().FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IVisualiser<>));
            if (iface != null)
            {
                var tArg = iface.GetGenericArguments()[0];
                PluginRegistry.VisualiserByDataType.TryAdd(tArg, v);
            }
        }
        PluginRegistry.Visualisers.Clear();
        PluginRegistry.Visualisers.AddRange(allVis);

        // 2) Build final Algorithm<->Visualiser pairs
        var allAlgos = builtInAlgos.Concat(pluginAlgos).ToList();
        PluginRegistry.Algorithms.Clear();
        PluginRegistry.Algorithms.AddRange(allAlgos);

        var menuItems = new List<AlgorithmMenuItem>();
        foreach (var algo in allAlgos)
        {
            // Look for [AlgorithmTag("VisualiserKey")]
            var tagAttr = algo.GetCustomAttributes<AlgorithmTagAttribute>()
                .FirstOrDefault(a => a.Tags?.Length > 0 && PluginRegistry.VisualiserKeys.ContainsKey(a.Tags[0]));

            Type? visType = null;
            string[] tags;

            if (tagAttr != null)
            {
                var key = tagAttr.Tags![0];
                visType = PluginRegistry.VisualiserKeys[key];
                tags = tagAttr.Tags;
            }
            else
            {
                // Fallback by generic argument type: IAlgorithm<T>, IVisualiser<T>
                var algoIface = algo.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAlgorithm<>));
                var tArg = algoIface?.GetGenericArguments()[0];
                if (tArg != null && PluginRegistry.VisualiserByDataType.TryGetValue(tArg, out visType!))
                {
                    tags = ["Default"];
                }
                else
                {
                    // Optionally, skip or handle missing visualisers here
                    continue;
                }
            }

            menuItems.Add(new AlgorithmMenuItem(
                algo.Name,
                algo,
                visType!,
                tags ?? ["Default"]));
        }

        // Update ObservableCollection on UI thread
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            AllAlgorithms.Clear();
            foreach (var mi in menuItems)
                AllAlgorithms.Add(mi);
            DiscoveryStatus = $"{AllAlgorithms.Count} algorithm(s) loaded";
        });
    }

    [RelayCommand]
    private void ToggleFlyout() => IsFlyoutOpen = !IsFlyoutOpen;

    [RelayCommand]
    private void SelectAlgorithm(AlgorithmMenuItem item) =>
        SelectedAlgorithm = item;

    [RelayCommand]
    private void Navigate()
    {
        if (SelectedAlgorithm is null) return;

        // instantiate the algo
        var type = Enumerable.First<Type>(SelectedAlgorithm.AlgorithmType.GetInterfaces());
        var algorithm = Activator.CreateInstance(SelectedAlgorithm.AlgorithmType)!;

        // instantiate the visualiser (we stored it)
        var visualiser = Activator.CreateInstance(SelectedAlgorithm.VisualiserType)!;

        // explanation view, controller, VM… same as before
        var dataType       = algorithm.GetType()
            .GetInterfaces()
            .First(i => i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IAlgorithm<>))
            .GetGenericArguments()[0];
        var controllerType  = typeof(AlgorithmController<>).MakeGenericType(dataType);
        var controller      = Activator.CreateInstance(
            controllerType,
            algorithm
        )!;
        
        var vmType = typeof(VisualiserViewModel<>).MakeGenericType(dataType);
        CurrentPage = Activator.CreateInstance(vmType, controller, visualiser)!;
        IsFlyoutOpen = false;
    }
}