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

        // 1) built-in in this assembly
        var asm          = Assembly.GetAssembly(typeof(BubbleSortAlgorithm))!;
        var builtInAlgos = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.GetInterfaces()
                         .Any(i => i.IsGenericType
                                   && i.GetGenericTypeDefinition() == typeof(IAlgorithm<>)))
            .ToArray();
        var builtInVis   = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.GetInterfaces()
                         .Any(i => i.IsGenericType
                                   && i.GetGenericTypeDefinition() == typeof(IVisualiser<>)))
            .ToArray();

        // 2) plugin-loaded
        Type[] pluginAlgos = [], pluginVis = [];
        try { pluginAlgos = await _pluginLoader.LoadAlgorithmsAsync(_pluginFolder); }
        catch (Exception ex) { Debug.WriteLine($"exception loading plugins: {ex.Message}"); }
        try { pluginVis   = await _pluginLoader.LoadVisualisersAsync(_pluginFolder); }
        catch (Exception ex) { Debug.WriteLine($"exception loading visualisers: {ex.Message}"); }

        // 3) merge into mutable pools
        var allAlgos        = builtInAlgos.Concat(pluginAlgos).ToList();
        var globalVisPool   = builtInVis.Concat(pluginVis).ToList();
        var remainingAlgos  = new List<Type>(allAlgos);
        var menuItems       = new List<AlgorithmMenuItem>();

        // 4a) per-assembly explicit bundling via VisualiserForAttribute
        foreach (var group in remainingAlgos.GroupBy(a => a.Assembly).ToList())
        {
            var algosInAsm = group.ToList();
            var visInAsm   = globalVisPool.Where(v => v.Assembly == group.Key).ToList();

            foreach (var algoType in algosInAsm)
            {
                // look for explicit attribute match
                var match = visInAsm.FirstOrDefault(visType =>
                    visType.GetCustomAttributes<VisualiserForAttribute>()
                           .Any(attr => attr.AlgorithmType == algoType));

                if (match != null)
                {
                    var attr = match.GetCustomAttribute<VisualiserForAttribute>()!;
                    var tags = algoType.GetCustomAttributes<AlgorithmTagAttribute>()
                                       .SelectMany(a => a.Tags)
                                       .DefaultIfEmpty("Default")
                                       .ToArray();

                    menuItems.Add(new AlgorithmMenuItem(
                        algoType.Name,
                        algoType,
                        match,
                        tags));

                    // remove from both pools as needed
                    remainingAlgos.Remove(algoType);
                    visInAsm.Remove(match);
                    if (!attr.AllowGlobalFallback)
                        globalVisPool.Remove(match);
                }
            }
        }

        // 4b) fallback cross-assembly match by data-type
        foreach (var algoType in remainingAlgos.ToList())
        {
            var dataT = algoType.GetInterfaces()
                                .First(i => i.IsGenericType
                                          && i.GetGenericTypeDefinition() == typeof(IAlgorithm<>))
                                .GetGenericArguments()[0];

            var visType = globalVisPool.FirstOrDefault(v =>
                v.GetInterfaces().Any(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IVisualiser<>)
                    && i.GetGenericArguments()[0] == dataT));

            if (visType != null)
            {
                var tags = algoType.GetCustomAttributes<AlgorithmTagAttribute>()
                                   .SelectMany(a => a.Tags)
                                   .DefaultIfEmpty("Default")
                                   .ToArray();

                menuItems.Add(new AlgorithmMenuItem(
                    algoType.Name,
                    algoType,
                    visType,
                    tags));

                globalVisPool.Remove(visType);
                remainingAlgos.Remove(algoType);
            }
        }

        // 5) push into your ObservableCollection on UI thread
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