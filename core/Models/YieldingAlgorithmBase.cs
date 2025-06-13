using System;
using System.Collections.Generic;
using System.Linq;
using algo_vis.core.Interfaces;
using algo_vis.core.Types;

namespace algo_vis.core;

public abstract class YieldingAlgorithmBase<T> : AlgorithmBase<T>, IYieldingAlgorithm<T>
{
    /// <summary>
    /// Execute one logical step, yielding intermediate states and explanations.
    /// Each yield represents a sub-step that should be visualized.
    /// 
    /// BEHAVIOR CONTRACT:
    /// - MUST yield at least one state per call (never empty)
    /// - First yielded state represents the main step completion
    /// - Subsequent states represent intermediate visualization steps
    /// - Should not modify algorithm state between yields
    /// - IsComplete on the first state indicates algorithm completion
    /// </summary>
    /// <returns>Enumerable of states, with first being main step and rest being substeps</returns>
    public abstract IEnumerable<AlgorithmState<T>> ExecuteStep();

    [Obsolete("NextStep() is not supported for yielding algorithms. Use ExecuteStep() instead.", true)]
    public override bool NextStep() =>
        throw new InvalidOperationException("NextStep() is not supported for yielding algorithms. Use ExecuteStep() instead.");
}