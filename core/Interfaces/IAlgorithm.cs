using algo_vis.core.Models;

namespace algo_vis.core.Interfaces;

public interface IAlgorithm<T>
{
    /// <summary>
    /// Initialize the algorithm's data structures.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Move the algorithm forward by one step.
    /// Return false if algorithm is complete.
    /// </summary>
    bool NextStep();

    /// <summary>
    /// Provide an explanation or description of what happened in the last step.
    /// </summary>
    LodExplanation GetExplanation();

    /// <summary>
    /// Return any data that the framework can visualize.
    /// For instance, an array, a list of edges, a tree, etc.
    /// </summary>
    T GetDataToVisualize();
    
    /// <summary>
    /// Set the algorithm's data to the specified state.
    /// Used for backward stepping and state restoration.
    /// </summary>
    void RestoreState(AlgorithmBase<T>.Snapshot snapshot);

    /// <summary>
    /// Capture the current state of the algorithm, including data and internal state marked by attribute.
    /// This snapshot is used for state restoration.
    /// </summary>
    /// <returns>
    /// A snapshot containing the algorithm's data and internal state.
    /// </returns>
    AlgorithmBase<T>.Snapshot CaptureState(bool isComplete = false, bool isSubStep = false);

    /// <summary>
    /// Reset the algorithm to its initial state without changing the data.
    /// Useful for restarting execution or after state restoration.
    /// </summary>
    void Reset();

}
