using System.Collections.Generic;
using algo_vis.core;
using algo_vis.core.Interfaces;
using System.Linq;

namespace algo_vis.ui.Models;

public class AlgorithmController<T>
{
    private readonly IAlgorithm<T> _algorithm;
    private readonly Queue<AlgorithmState<T>> _pendingStates = new();
    private AlgorithmState<T> _current;

    public AlgorithmController(IAlgorithm<T> algorithm)
    {
        _algorithm = algorithm;
        _algorithm.Initialize();
        
        // Set initial state
        _current = new AlgorithmState<T>(
            _algorithm.GetDataToVisualize(),
            _algorithm.GetExplanation()
        );
    }

    public AlgorithmState<T> Current => _current;

    public AlgorithmState<T> Step()
    {
        // If we have pending sub-steps, return the next one
        if (_pendingStates.TryDequeue(out var pendingState))
        {
            _current = pendingState;
            return _current;
        }

        // Execute the next step
        if (_algorithm is IYieldingAlgorithm<T> yieldingAlgorithm)
        {
            // Get all yielded states for this step
            var states = yieldingAlgorithm.ExecuteStep().ToList();
            
            if (states.Any())
            {
                // Return the first state immediately
                _current = states.First();
                
                // Queue the rest for subsequent calls
                foreach (var state in states.Skip(1))
                {
                    _pendingStates.Enqueue(state);
                }
            }
        }
        else
        {
            // Fallback to legacy NextStep
            var hasNext = _algorithm.NextStep();
            _current = new AlgorithmState<T>(
                _algorithm.GetDataToVisualize(),
                _algorithm.GetExplanation(),
                isComplete: !hasNext
            );
        }

        return _current;
    }

    public bool HasPendingSubSteps => _pendingStates.Count > 0;
}