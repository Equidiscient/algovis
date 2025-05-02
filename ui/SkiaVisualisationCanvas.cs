using System.Drawing;
using algo_vis.core.Interfaces;
using algo_vis.core.Models;
using SkiaSharp;

namespace algo_vis.ui;

public class SkiaVisualisationCanvas : IVisualisationCanvas
{
    private readonly SKBitmap _backbuffer;
    private readonly SKCanvas _skCanvas;

    public SkiaVisualisationCanvas(SKBitmap bmp)
    {
        _backbuffer = bmp;
        _skCanvas = new SKCanvas(_backbuffer);
    }

    public float Width => _backbuffer.Width;
    public float Height => _backbuffer.Height;

    public void Clear(Color32 color = default)
    {
        _skCanvas.Clear(new SKColor(color.R, color.G, color.B, color.A));
    }

    public void FillRect(RectF r, Color32 c)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(c.R, c.G, c.B, c.A),
            IsAntialias = true
        };
        _skCanvas.DrawRect(r.X, r.Y, r.Width, r.Height, paint);
    }

    public void DrawRect(RectF r, Color32 strokeColor = default, float strokeThickness = 1)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = strokeThickness,
            Color = new SKColor(strokeColor.R, strokeColor.G, strokeColor.B, strokeColor.A),
            IsAntialias = true
        };
        _skCanvas.DrawRect(r.X, r.Y, r.Width, r.Height, paint);
    }

    public void DrawLine(Point a, Point b, Color32 strokeColor = default, float strokeThickness = 1)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = strokeThickness,
            Color = new SKColor(strokeColor.R, strokeColor.G, strokeColor.B, strokeColor.A),
            IsAntialias = true
        };
        _skCanvas.DrawLine(a.X, a.Y, b.X, b.Y, paint);
    }

    public void DrawText(string text, Point origin, float fontSize = 12, Color32 fontColor = default)
    {
        using var paint = new SKPaint
        {
            Color = new SKColor(fontColor.R, fontColor.G, fontColor.B, fontColor.A),
            IsAntialias = true
        };
        using var font = new SKFont()
        {
            Typeface = SKTypeface.Default,
            Size = fontSize
        };
        // Note: Y is baseline; offset if you want top‐aligned text
        _skCanvas.DrawText(text, origin.X, origin.Y + fontSize, SKTextAlign.Center, font, paint);
    }

    public void FillCircle(Point center, float radius, Color32 fillColor = default)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(fillColor.R, fillColor.G, fillColor.B, fillColor.A),
            IsAntialias = true
        };
        _skCanvas.DrawCircle(center.X, center.Y, radius, paint);
    }

    // add primitives as needed
}