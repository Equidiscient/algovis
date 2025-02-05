using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace algo_vis.ui.ViewModels
{
    public partial class MainMenuViewModel : ViewModelBase
    {
        // the collection of menu items
        public ObservableCollection<string> MenuItems { get; } =
            new()
            {
                "Visualize Sorting",
                "Visualize BFS",
                "Visualize DFS",
                "Exit"
            };

        [ObservableProperty]
        private string _selectedMenuItem;

        private readonly MainWindowViewModel _shell;

        public MainMenuViewModel(MainWindowViewModel shell)
        {
            _shell = shell;
        }
        
        [RelayCommand]
        private void Navigate()
        {
            switch (SelectedMenuItem)
            {
                case "Visualize Sorting":
                    _shell.NavigateTo(new SortingVisualiserViewModel());
                    break;

                case "Visualize BFS":
                    // _shell.NavigateTo(new BfsVisualizerViewModel());
                    break;

                case "Visualize DFS":
                    // _shell.NavigateTo(new DfsVisualizerViewModel());
                    break;

                case "Exit":
                    Environment.Exit(0);
                    break;
            }
        }
    }
}