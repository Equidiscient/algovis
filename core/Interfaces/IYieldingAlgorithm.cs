using System.Collections.Generic;
using algo_vis.core.Models;

namespace algo_vis.core.Interfaces;

public interface IYieldingAlgorithm<T> : IAlgorithm<T>
{
    /// <summary>
    /// Execute one logical step, yielding intermediate states and explanations.
    /// Each yield represents a sub-step that should be visualized.
    /// </summary>
    IEnumerable<AlgorithmState<T>> ExecuteStep();
}

public class AlgorithmState<T>(T data, LodExplanation explanation, bool isComplete = false, bool isSubStep = false)
{
    public T Data { get; } = data;
    public LodExplanation Explanation { get; } = explanation;
    public bool IsComplete { get; } = isComplete;
    public bool IsSubStep { get; } = isSubStep;
}
