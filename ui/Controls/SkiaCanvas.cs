using System;
using algo_vis.ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using SkiaSharp;

namespace algo_vis.ui.Controls;

public class SkiaCanvas : Control
{
  private SKBitmap? _bitmap;

  public static readonly StyledProperty<SKBitmap?> BitmapProperty =
    AvaloniaProperty.Register<SkiaCanvas, SKBitmap?>(
      nameof(Bitmap));

  public SKBitmap? Bitmap
  {
    get => _bitmap;
    set => SetValue(BitmapProperty, value);
  }
  
  public SkiaCanvas()
  {
    // whenever our layout bounds change, re-allocate the backing bitmap
    this.GetObservable(BoundsProperty).Subscribe(boundsRect =>
    {
      var w = (int)boundsRect.Width;
      var h = (int)boundsRect.Height;
      if (w > 0 && h > 0)
      {
        // dispose old
        _bitmap?.Dispose();

        // create a new one at exactly the control's pixel size
        _bitmap = new SKBitmap(w, h);
        Bitmap = _bitmap;        // will InvalidateVisual()
      }
    });
  }

  public override void Render(DrawingContext context)
  {
    if (Bitmap is null)
      return;

    // draw the pre-rendered bitmap into the control bounds
    var dest = new Rect(0, 0, Bounds.Width, Bounds.Height);
   
    context.Custom(new SkiaDrawOperation(dest, Bitmap));
  }
  static SkiaCanvas(){
    BitmapProperty.Changed.AddClassHandler<SkiaCanvas>((canvas, _) => canvas.InvalidateVisual());
  }
}

