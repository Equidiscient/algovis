using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace algo_vis.ui.ViewModels;

public abstract class VisualiserViewModel : ViewModelBase
{
    private string _explanation;
    public string Explanation
    {
        get => _explanation;
        set => SetProperty(ref _explanation, value);
    }
    
    private WriteableBitmap _renderedOutput;
    public WriteableBitmap RenderedOutput
    {
        get => _renderedOutput;
        set => SetProperty(ref _renderedOutput, value);
    }

    public VisualiserViewModel()
    {
        Explanation = "testing <> <b> </b> <script> ;'' \\n daw";
    }
}