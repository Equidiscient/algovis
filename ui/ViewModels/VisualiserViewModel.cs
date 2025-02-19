using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

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
    private WriteableBitmap _renderedOutput = new(new PixelSize(400,200), new Vector(96,96), PixelFormats.Bgra8888);
}