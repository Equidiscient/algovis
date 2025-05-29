using algo_vis.ui.Controls;
using algo_vis.ui.ViewModels;
using Avalonia;

namespace algo_vis.ui.Behaviors;

public static class CanvasConnectionBehavior
{
    public static readonly AttachedProperty<IVisualiserViewModel?> ViewModelProperty =
        AvaloniaProperty.RegisterAttached<SkiaCanvas, IVisualiserViewModel?>(
            "ViewModel", 
            typeof(CanvasConnectionBehavior));

    public static void SetViewModel(AvaloniaObject target, IVisualiserViewModel? value)
    {
        target.SetValue(ViewModelProperty, value);
    }

    public static IVisualiserViewModel? GetViewModel(AvaloniaObject target)
    {
        return target.GetValue(ViewModelProperty);
    }

    static CanvasConnectionBehavior()
    {
        ViewModelProperty.Changed.AddClassHandler<SkiaCanvas>(OnViewModelChanged);
    }

    private static void OnViewModelChanged(SkiaCanvas canvas, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is IVisualiserViewModel viewModel)
        {
            if (canvas.IsLoaded && canvas.Bounds.Width > 0 && canvas.Bounds.Height > 0)
            {
                viewModel.SetCanvas(canvas);
            }
            else
            {
                void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
                {
                    canvas.Loaded -= OnLoaded;
                    viewModel.SetCanvas(canvas);
                }
                canvas.Loaded += OnLoaded;
            }
        }
    }
}