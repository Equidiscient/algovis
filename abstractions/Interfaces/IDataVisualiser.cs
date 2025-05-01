namespace algo_vis.abstractions.Interfaces;

public interface IDataVisualiser<T>
{
    /// <summary>
    /// Updates a FrameBuffer with a visualisation of the given data.
    /// </summary>
    void DisplayData(T data, FrameBuffer buffer);
}
