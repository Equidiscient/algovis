using algo_vis.ui.Models;
using algo_vis.visualisers;

namespace algo_vis.algorithms;

public class AlgorithmController<T>
{
    private IAlgorithm<T> _algorithm;
    private bool _isComplete;
    
    private IDataVisualiser<T> _dataVisualiser;
    private IExplanationView _explanationView;

    public AlgorithmController(IAlgorithm<T> algorithm, 
        IDataVisualiser<T> dataVisualiser, 
        IExplanationView explanationView)
    {
        _algorithm = algorithm;
        _dataVisualiser = dataVisualiser;
        _explanationView = explanationView;
    }

    public void InitializeAlgorithm(T parameters)
    {
        _algorithm.Initialize(parameters);
        _isComplete = false;
        UpdateUI();
    }

    // Called by the UI (next)
    public void NextStep()
    {
        if (_isComplete) return;
        bool canContinue = _algorithm.NextStep();
        if (!canContinue)
        {
            _isComplete = true;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        var data = _algorithm.GetDataToVisualize();
        var explanation = _algorithm.GetExplanation();
        
        _dataVisualiser.DisplayData(data);

        _explanationView.SetExplanation(explanation);
    }
}
