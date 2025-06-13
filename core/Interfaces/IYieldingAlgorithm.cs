using System;
using System.Collections.Generic;
using algo_vis.core.Models;
using algo_vis.core.Types;

namespace algo_vis.core.Interfaces;

public interface IYieldingAlgorithm<T> : IAlgorithm<T>
{
    /// <summary>
    /// Execute one logical step, yielding intermediate states and explanations.
    /// Each yield represents a sub-step that should be visualized.
    /// </summary>
    IEnumerable<AlgorithmState<T>> ExecuteStep();

    [Obsolete("NextStep() is not supported for yielding algorithms. Use ExecuteStep() instead.", true)]
    new bool NextStep();
}
