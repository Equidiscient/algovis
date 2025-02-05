using System;
using algo_vis.ui.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace algo_vis.ui.Views;

public partial class VisualiserView : UserControl
{
    public VisualiserView()
    {
        InitializeComponent();
        
        OutputImage.GetObservable(BoundsProperty).Subscribe(bounds =>
        {
            if (bounds.Height * bounds.Width <= 0) return;
            var pixelSize = new PixelSize((int)bounds.Width, (int)bounds.Height);
            var dpi = new Vector(96, 96);
            var wb = new WriteableBitmap(pixelSize, dpi, Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);
            
            if (DataContext is VisualiserViewModel vm)
                vm.RenderedOutput = wb;
        });

    }
}