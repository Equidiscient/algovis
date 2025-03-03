using algo_vis.visualisers.builtin;
using Avalonia.Media.Imaging;

namespace algo_vis.ui.ViewModels;

public class BarChartViewModel : VisualiserViewModel
{
    private BarChartVisualiser _visualiser;
    public BarChartViewModel()
    {
        _visualiser = new BarChartVisualiser(RenderedOutput);
        
        var sampleData = new[] { 3, 1, 4, 1, 5, 9 };
        _visualiser.DisplayData(sampleData);
        Explanation = "this is a bar chart of sample data";
    }

    // update model undecided
    public void UpdateData(int[] newData)
    {
        _visualiser.DisplayData(newData);
        Explanation = "updated bar chart with new data";
    }
}