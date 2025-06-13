using algo_vis.core.Models;

namespace algo_vis.core.Types;

public class AlgorithmState<T>(T data, LodExplanation explanation, bool isComplete = false, bool isSubStep = false)
{
    public T Data { get; } = data;
    public LodExplanation Explanation { get; } = explanation;
    public bool IsComplete { get; } = isComplete;
}