using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace algo_vis.visualisers;

public abstract class BaseVisualiser<T> : IDataVisualiser<T>
{
    protected readonly SKBitmap _bitmap;

    protected BaseVisualiser(SKBitmap bitmap)
    {
        _bitmap = bitmap;
    }

    public void DisplayData(T data)
    {
        using var surface = SKSurface.Create(new SKImageInfo(_bitmap.Width, _bitmap.Height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);
        
        DrawData(data, canvas, _bitmap.Width, _bitmap.Height);
        
        using var snapshot = surface.Snapshot();
        snapshot.ReadPixels(_bitmap.Info, _bitmap.GetPixels());
    }
    
    protected abstract void DrawData(T data, SKCanvas canvas, int width, int height);
}