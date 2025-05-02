using algo_vis.core.Models;

namespace algo_vis.ui.Models;

public class StepResult<T>
{
    public T      Data        { get; init; }
    public LodExplanation Explanation { get; init; }
    public bool   IsComplete  { get; init; }
}