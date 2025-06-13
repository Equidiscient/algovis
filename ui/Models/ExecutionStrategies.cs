using System.Collections.Generic;
using System.Linq;
using algo_vis.core;
using algo_vis.core.Interfaces;
using algo_vis.core.Types;

namespace algo_vis.ui.Models;

public interface IAlgorithmExecutionStrategy<T>
{
    bool CanExecuteStep();
    void ExecuteStep(List<AlgorithmBase<T>.Snapshot> history, ref int historyIndex);
}

public class StandardExecutionStrategy<T>(IAlgorithm<T> algorithm) : IAlgorithmExecutionStrategy<T>
{
    public bool CanExecuteStep() => !algorithm.CaptureState().AlgorithmState.IsComplete;

    public void ExecuteStep(List<AlgorithmBase<T>.Snapshot> history, ref int historyIndex)
    {
        bool hasNext = algorithm.NextStep();
        var snapshot = algorithm.CaptureState(isComplete: !hasNext);
        
        // Remove future history and add new snapshot
        if (historyIndex < history.Count - 1)
        {
            history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);
        }
        
        history.Add(snapshot);
        historyIndex = history.Count - 1;
    }
}

public class YieldingExecutionStrategy<T>(IYieldingAlgorithm<T> algorithm) : IAlgorithmExecutionStrategy<T>
{
    private readonly Queue<AlgorithmState<T>> _pendingSubSteps = new();

    public bool CanExecuteStep() => 
        _pendingSubSteps.Count > 0 || !algorithm.CaptureState().AlgorithmState.IsComplete;

    public void ExecuteStep(List<AlgorithmBase<T>.Snapshot> history, ref int historyIndex)
    {
        // Handle pending substeps first
        if (_pendingSubSteps.Count > 0)
        {
            var nextSubStep = _pendingSubSteps.Dequeue();
            var internalState = algorithm.CaptureState().InternalState;
            var snapshot = new AlgorithmBase<T>.Snapshot(nextSubStep, internalState);
            
            if (historyIndex < history.Count - 1)
            {
                history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);
            }
            
            history.Add(snapshot);
            historyIndex = history.Count - 1;
            return;
        }

        // Execute new step
        var states = algorithm.ExecuteStep().ToList();
        if (states.Count == 0) return;

        var mainState = states[0];
        var mainSnapshot = new AlgorithmBase<T>.Snapshot(mainState, algorithm.CaptureState().InternalState);
        
        if (historyIndex < history.Count - 1)
        {
            history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);
        }
        
        history.Add(mainSnapshot);
        historyIndex = history.Count - 1;

        // Queue substeps
        for (int i = 1; i < states.Count; i++)
        {
            _pendingSubSteps.Enqueue(states[i]);
        }
    }

    public void ClearPendingSubSteps() => _pendingSubSteps.Clear();
}