namespace algo_vis.abstractions.Interfaces;

public interface IAlgorithm<T>
{
    /// <summary>
    /// Initialize the algorithm's data structures.
    /// Possibly parse user parameters (array size, graph structure, etc.).
    /// </summary>
    void Initialize(T initialState);

    /// <summary>
    /// Move the algorithm forward by one step.
    /// Return false if algorithm is complete.
    /// </summary>
    bool NextStep();

    /// <summary>
    /// Provide an explanation or description of what happened in the last step.
    /// </summary>
    string GetExplanation();

    /// <summary>
    /// Return any data that the framework can visualize.
    /// For instance, an array, a list of edges, a tree, etc.
    /// </summary>
    T GetDataToVisualize();
}
