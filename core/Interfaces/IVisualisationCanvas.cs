using System.Drawing;
using algo_vis.core.Models;

namespace algo_vis.core.Interfaces;

public interface IVisualisationCanvas
{
    float Width { get; }
    float Height { get; }
    
    /// <summary>Clears entire canvas to given color (default white).</summary>
    void Clear(Color32 background = default);

    /// <summary>Draws a filled rectangle.</summary>
    void FillRect(RectF r, Color32 fillColor = default);

    /// <summary>Draws an outlined rectangle.</summary>
    void DrawRect(
        RectF r,
        Color32 strokeColor = default,
        float strokeThickness = 1);

    /// <summary>Draws a line from A to B.</summary>
    void DrawLine(
        Point a,
        Point b,
        Color32 strokeColor = default,
        float strokeThickness = 1);

    /// <summary>Draws text at given origin.</summary>
    void DrawText(
        string text,
        Point origin,
        float fontSize = 12,
        Color32 fontColor = default);

    /// <summary>Draws a filled circle.</summary>
    void FillCircle(
        Point center,
        float radius,
        Color32 fillColor = default);
}

