using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace algo_vis.ui.ViewModels;

public partial class MarkdownRendererViewModel : ObservableObject
{
    private const string HomeResourceName = "algo_vis.ui.Assets.Guide.Landing.md";
        
    // Singleton
    private static MarkdownRendererViewModel? _instance;
    public static MarkdownRendererViewModel Instance => 
        _instance ??= new MarkdownRendererViewModel();

    [ObservableProperty]
    private string _markdown;

    [ObservableProperty]
    private ObservableCollection<Breadcrumb> _breadcrumbs = [];

    private MarkdownRendererViewModel()
    {
        Markdown = GetMarkdownFromResource(HomeResourceName);
    }

    [RelayCommand]
    public void LoadMarkdownFromResource(string resourceName)
    {
        Markdown = GetMarkdownFromResource(resourceName);
    }

    private string GetMarkdownFromResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
#if DEBUG
        var resourceNames = assembly.GetManifestResourceNames();
        Debug.WriteLine("All Embedded Resources:");
        foreach (var rName in resourceNames)
            Debug.WriteLine($" - {rName}");
#endif
        using var resStream = assembly.GetManifestResourceStream(resourceName) ?? throw new NullReferenceException($"Could not find embedded resource {resourceName}");
        AddBreadcrumbIfNew(resourceName);
        return new StreamReader(resStream).ReadToEnd();
    }

    private void AddBreadcrumbIfNew(string resourceName)
    {
        if (Breadcrumbs.All(b => b.ResourceName != resourceName))
        {
            Breadcrumbs.Add(new Breadcrumb(resourceName, BreadcrumbClickedCommand));
            UpdateActiveBreadcrumb();
        }
    }

    [RelayCommand]
    public void BreadcrumbClicked(Breadcrumb crumb)
    {
        int index = Breadcrumbs.IndexOf(crumb);
        if (index >= 0)
        {
            while (Breadcrumbs.Count > index + 1)
                Breadcrumbs.RemoveAt(Breadcrumbs.Count - 1);
            LoadMarkdownFromResource(crumb.ResourceName);
        }
    }

    private void UpdateActiveBreadcrumb()
    {
        for (int i = 0; i < Breadcrumbs.Count; i++)
            Breadcrumbs[i].IsActive = (i == Breadcrumbs.Count - 1);
    }
}

public record Breadcrumb(string ResourceName, ICommand Command) : INotifyPropertyChanged
{
    public string FriendlyName { get; } = ParseFriendlyName(ResourceName);

    private static string ParseFriendlyName(string resourceName)
    {
        var tokens = resourceName.Split('.');
        return string.Join(" > ", tokens.Skip(3).Take(tokens.Length - 4));
    }

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            _isActive = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}