using System.Collections.Generic;
using algo_vis.core;
using algo_vis.core.Interfaces;

namespace algo_vis.ui.Models;

public class AlgorithmController<T>
{
    private bool _isComplete;
    private readonly IAlgorithm<T> _algo;

    public AlgorithmController(IAlgorithm<T> algo)
    {
         algo.Initialize();
        _algo = algo;
    }

    public StepResult<T> Current =>
    new StepResult<T>
    {
      Data        = _algo.GetDataToVisualize(),
      Explanation = _algo.GetExplanation(),
      IsComplete  = _isComplete
    };

    public StepResult<T> Step()
    {
        if (!_isComplete && !_algo.NextStep())
            _isComplete = true;
        var lod    = _algo.GetExplanation();

        
        return new StepResult<T>
        {
            Data        = _algo.GetDataToVisualize(),
            Explanation = lod,
            IsComplete  = _isComplete
        };
    }
}