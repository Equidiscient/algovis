using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

public class SkiaDrawOperation : ICustomDrawOperation
{
    private readonly Rect _bounds;
    private readonly SKBitmap _bitmap;

    public SkiaDrawOperation(Rect bounds, SKBitmap bitmap)
    {
        _bounds = bounds;
        _bitmap = bitmap;
    }

    public void Dispose() { }

    public void Render(ImmediateDrawingContext context)
    {
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        if (leaseFeature is null) return;
        using var lease = leaseFeature.Lease();
        var canvas = lease.SkCanvas;
        if (canvas is not null && _bitmap is not null)
        {
            canvas.DrawBitmap(
                _bitmap,
                SKRect.Create(0, 0, _bitmap.Width, _bitmap.Height),
            SKRect.Create((float)_bounds.Left, (float)_bounds.Top, (float)_bounds.Width, (float)_bounds.Height));
        }
    }

    public Rect Bounds => _bounds;
    public bool HitTest(Point p) => true;
    public bool Equals(ICustomDrawOperation? other) => false;
}