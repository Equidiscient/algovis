using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using algo_vis.abstractions;
using algo_vis.abstractions.Interfaces;
using algo_vis.ui.Models;
using algo_vis.ui.Services.Plugins;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace algo_vis.ui.ViewModels
{
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
                    return it.Name.Contains(s, StringComparison.InvariantCultureIgnoreCase)
                           || it.Tags.Any(t => t.Contains(s, StringComparison.InvariantCultureIgnoreCase));
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
            var asm = Assembly.GetExecutingAssembly();
            var builtInAlgos = asm.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                            && t.GetInterfaces()
                                .Any(i => i.IsGenericType
                                       && i.GetGenericTypeDefinition() == typeof(IAlgorithm<>)))
                .ToArray();

            var builtInVis = asm.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                            && t.GetInterfaces()
                                .Any(i => i.IsGenericType
                                       && i.GetGenericTypeDefinition() == typeof(IDataVisualiser<>)))
                .ToArray();

            // 2) plugin-loaded
            Type[] pluginAlgos = [], pluginVis = [];
            try { pluginAlgos = await _pluginLoader.LoadAlgorithmsAsync(_pluginFolder); }
            catch(Exception ex) { Debug.WriteLine($"exception loading plugins: {ex.Message}"); }
            try { pluginVis   = await _pluginLoader.LoadVisualisersAsync(_pluginFolder); }
            catch(Exception ex) { Debug.WriteLine($"exception loading visualisers: {ex.Message}"); }

            // 3) merge
            var allAlgos = builtInAlgos.Concat(pluginAlgos);
            var allVis   = builtInVis  .Concat(pluginVis);

            // 4) match up by generic data‐type
            var menuItems = allAlgos
                .Select(algoType =>
                {
                    var dataType = algoType.GetInterfaces()
                        .First(i => i.IsGenericType
                                    && i.GetGenericTypeDefinition() == typeof(IAlgorithm<>))
                        .GetGenericArguments()[0];

                    var visType = allVis.FirstOrDefault(v =>
                        v.GetInterfaces().Any(i =>
                            i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IDataVisualiser<>)
                            && i.GetGenericArguments()[0] == dataType));

                    if (visType is null) return null;
                    var tags = algoType.GetCustomAttributes<AlgorithmTagAttribute>()
                        .SelectMany(a => a.Tags)
                        .DefaultIfEmpty("Default")
                        .ToArray();

                    return new AlgorithmMenuItem(algoType.Name, algoType, visType, tags);
                })
                .Where(x => x != null)
                .Cast<AlgorithmMenuItem>()
                .ToList();

            // 4) finally, push into your ObservableCollection on UI thread
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
            var algorithm = (IAlgorithm<object>)Activator.CreateInstance(
                SelectedAlgorithm.AlgorithmType)!;

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
                                      algorithm,
                                      visualiser
                                  )!;

            var vmType = typeof(VisualiserViewModel<>).MakeGenericType(dataType);
            CurrentPage = Activator.CreateInstance(vmType, controller)!;
            IsFlyoutOpen = false;
        }
    }
}