using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using SkiaSharp;

namespace algo_vis.ui;

public class SkiaCanvas : Control
{
    private SKBitmap? _bitmap;

    public static readonly DirectProperty<SkiaCanvas, SKBitmap?> BitmapProperty =
        AvaloniaProperty.RegisterDirect<SkiaCanvas, SKBitmap?>(
            nameof(Bitmap),
            o => o.Bitmap,
            (o, v) => o.Bitmap = v);

    public SKBitmap? Bitmap
    {
        get => _bitmap;
        set
        {
            SetAndRaise(BitmapProperty, ref _bitmap, value);
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        if (_bitmap == null) return;

        var bounds = new Rect(0, 0, Bounds.Width, Bounds.Height);
        
        var customDrawOp = new SkiaDrawOperation(bounds, _bitmap);
        context.Custom(customDrawOp);
    }
}