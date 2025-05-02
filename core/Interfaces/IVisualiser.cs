namespace algo_vis.core.Interfaces;

/// <summary>
/// A data‐visualiser that knows how to draw its T onto a canvas.
/// </summary>
public interface IVisualiser<T>
{
  /// <summary>
  /// Invoked once per step.  Use the canvas API to render your scene.
  /// </summary>
  void DrawData(T data, IVisualisationCanvas canvas);
}