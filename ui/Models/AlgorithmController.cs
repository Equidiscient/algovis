using algo_vis.abstractions;
using algo_vis.abstractions.Interfaces;

public class AlgorithmController<T>(IAlgorithm<T> algo)
{
    private bool _isComplete;

    public void Initialize(T parameters)
    {
        algo.Initialize(parameters);
        _isComplete = false;
    }

    public StepResult<T> Step()
    {
        if (!_isComplete && !algo.NextStep())
            _isComplete = true;

        return new StepResult<T>
        {
            Data        = algo.GetDataToVisualize(),
            Explanation = algo.GetExplanation(),
            IsComplete  = _isComplete
        };
    }
}

public class StepResult<T>
{
    public T      Data        { get; init; }
    public string Explanation { get; init; }
    public bool   IsComplete  { get; init; }
}