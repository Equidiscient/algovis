using System.Collections.Generic;
using algo_vis.core;
using algo_vis.core.Interfaces;
using System.Linq;
using algo_vis.core.Types;

namespace algo_vis.ui.Models;

public class AlgorithmController<T>
{
    private readonly IAlgorithm<T> _algorithm;
    private readonly IAlgorithmExecutionStrategy<T> _executionStrategy;
    private readonly List<AlgorithmBase<T>.Snapshot> _history = new();
    private int _historyIndex = -1;

    public AlgorithmController(IAlgorithm<T> algorithm)
    {
        _algorithm = algorithm;
        _executionStrategy = algorithm switch
        {
            IYieldingAlgorithm<T> yieldingAlgorithm => new YieldingExecutionStrategy<T>(yieldingAlgorithm),
            _ => new StandardExecutionStrategy<T>(algorithm)
        };
        
        _algorithm.Initialize();
        AddInitialState();
    }

    public AlgorithmState<T> CurrentAlgoState => _history[_historyIndex].AlgorithmState;
    
    public bool CanStepForward => 
        _historyIndex < _history.Count - 1 || _executionStrategy.CanExecuteStep();
    
    public bool CanStepBackward => _historyIndex > 0;

    public AlgorithmState<T> StepForward()
    {
        if (_historyIndex < _history.Count - 1)
        {
            _historyIndex++;
            _algorithm.RestoreState(_history[_historyIndex]);
            return CurrentAlgoState;
        }

        if (!_executionStrategy.CanExecuteStep())
        {
            return CurrentAlgoState;
        }

        _executionStrategy.ExecuteStep(_history, ref _historyIndex);
        return CurrentAlgoState;
    }

    public AlgorithmState<T> StepBackward()
    {
        if (!CanStepBackward)
            return CurrentAlgoState;

        if (_executionStrategy is YieldingExecutionStrategy<T> yieldingStrategy)
        {
            yieldingStrategy.ClearPendingSubSteps();
        }

        _historyIndex--;
        _algorithm.RestoreState(_history[_historyIndex]);
        return CurrentAlgoState;
    }

    private void AddInitialState()
    {
        var snapshot = _algorithm.CaptureState(isComplete: false);
        _history.Add(snapshot);
        _historyIndex = 0;
    }
}