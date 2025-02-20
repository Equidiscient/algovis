using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace algo_vis.visualisers;

public abstract class BaseVisualiser<T> : IDataVisualiser<T>
{
    protected readonly WriteableBitmap _bitmap;

    protected BaseVisualiser(WriteableBitmap bitmap)
    {
        _bitmap = bitmap;
    }

    public void DisplayData(T data)
    {
        using var lockedFrameBuffer = _bitmap.Lock();
        using var surface = SKSurface.Create(new SKImageInfo(
            lockedFrameBuffer.Size.Width, lockedFrameBuffer.Size.Height,
            SKColorType.Bgra8888, SKAlphaType.Premul));
        
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);
        
        DrawData(data, canvas, lockedFrameBuffer.Size.Width, lockedFrameBuffer.Size.Height);
        canvas.Flush();
    }
    
    protected abstract void DrawData(T data, SKCanvas canvas, int width, int height);
}