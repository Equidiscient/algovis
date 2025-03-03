using algo_vis.ui.ViewModels;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;

namespace algo_vis.ui.ViewModels;

public abstract partial class VisualiserViewModel : ViewModelBase
{
    private string _explanation;
    public string Explanation
    {
        get => _explanation;
        set => SetProperty(ref _explanation, value);
    }
    [ObservableProperty]
    internal SKBitmap _renderedOutput = new(400,200);
}