namespace algo_vis.ui.ViewModels;

public class SortingVisualiserViewModel : VisualiserViewModel
{
    public SortingVisualiserViewModel()
    {
        PerformSortingStep();
    }

    private void PerformSortingStep()
    {
        Explanation = "Swapping array elements 2 and 3...";
    }

}