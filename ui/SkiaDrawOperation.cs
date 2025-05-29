using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace algo_vis.ui;

public class SkiaDrawOperation(Rect bounds, SKBitmap bitmap) : ICustomDrawOperation
{
    public void Dispose() { }

    public void Render(ImmediateDrawingContext context)
    {
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        if (leaseFeature is null) return;
        
        using var lease = leaseFeature.Lease();
        var canvas = lease.SkCanvas;
        
        if (canvas is null || bitmap?.IsNull != false) return;

        try
        {
            canvas.DrawBitmap(
                bitmap,
                SKRect.Create(0, 0, bitmap.Width, bitmap.Height),
                SKRect.Create((float)bounds.Left, (float)bounds.Top, (float)bounds.Width, (float)bounds.Height));
        }
        catch (System.Exception)
        {
            // Ignore rendering errors - bitmap may have been disposed during render
        }
    }

    public Rect Bounds => bounds;
    public bool HitTest(Point p) => bounds.Contains(p);
    public bool Equals(ICustomDrawOperation? other) => ReferenceEquals(this, other);
}