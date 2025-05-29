using System;
using algo_vis.core.Interfaces;
using algo_vis.core.Models;
using algo_vis.ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using SkiaSharp;

namespace algo_vis.ui.Controls;

public class SkiaCanvas : Control
{
    public static readonly StyledProperty<SKBitmap?> BitmapProperty =
        AvaloniaProperty.Register<SkiaCanvas, SKBitmap?>(nameof(Bitmap));

    private readonly object _renderLock = new object();
    private SkiaVisualisationCanvas? _currentCanvas;

    public SKBitmap? Bitmap
    {
        get => GetValue(BitmapProperty);
        set => SetValue(BitmapProperty, value);
    }

    public SkiaCanvas()
    {
        // Recreate bitmap when control size changes
        this.GetObservable(BoundsProperty).Subscribe(OnBoundsChanged);
    }

    private void OnBoundsChanged(Rect boundsRect)
    {
        var w = (int)boundsRect.Width;
        var h = (int)boundsRect.Height;
        
        if (w <= 0 || h <= 0) return;
        if (w == Bitmap?.Width && h == Bitmap?.Height) return;

        lock (_renderLock)
        {
            // Dispose old resources
            _currentCanvas?.Dispose();
            _currentCanvas = null;
            Bitmap?.Dispose();

            // Create new bitmap at exact control size
            Bitmap = new SKBitmap(w, h);
        }
    }

    /// <summary>
    /// Renders visualization data using the provided visualiser.
    /// This is the main entry point for the render pipeline.
    /// </summary>
    public void RenderVisualization<T>(T data, IVisualiser<T> visualiser)
    {
        if (visualiser == null) throw new ArgumentNullException(nameof(visualiser));

        lock (_renderLock)
        {
            // Ensure we have a valid bitmap
            if (Bitmap == null) return;

            try
            {
                // Dispose old canvas and create new one for current bitmap
                _currentCanvas?.Dispose();
                _currentCanvas = new SkiaVisualisationCanvas(Bitmap);

                // Let the visualiser draw to our canvas
                visualiser.DrawData(data, _currentCanvas);

                // Ensure all drawing operations are committed
                _currentCanvas.Flush();
            }
            catch (ObjectDisposedException)
            {
                // Bitmap was disposed during rendering - skip this frame
                return;
            }
        }

        // Trigger UI refresh on main thread
        InvalidateVisual();
    }

    /// <summary>
    /// Clears the canvas to the specified color.
    /// </summary>
    public void Clear(Color32 backgroundColor = default)
    {
        lock (_renderLock)
        {
            if (Bitmap == null) return;

            try
            {
                _currentCanvas?.Dispose();
                _currentCanvas = new SkiaVisualisationCanvas(Bitmap);
                _currentCanvas.Clear(backgroundColor);
                _currentCanvas.Flush();
            }
            catch (ObjectDisposedException)
            {
                return;
            }
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        if (Bitmap?.IsNull != false) return;

        var dest = new Rect(0, 0, Bounds.Width, Bounds.Height);
        context.Custom(new SkiaDrawOperation(dest, Bitmap));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        lock (_renderLock)
        {
            _currentCanvas?.Dispose();
            _currentCanvas = null;
            Bitmap?.Dispose();
            Bitmap = null;
        }
        
        base.OnDetachedFromVisualTree(e);
    }
}