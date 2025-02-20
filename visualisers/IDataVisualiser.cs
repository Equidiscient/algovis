namespace algo_vis.visualisers;

public interface IDataVisualiser<T>
{
    /// <summary>
    /// Updates a WriteableBitmap with a visualisation of the given data.
    /// </summary>
    void DisplayData(T data);
}
