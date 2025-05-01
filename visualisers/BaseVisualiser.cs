using System;
using algo_vis.abstractions;
using algo_vis.abstractions.Interfaces;
using SkiaSharp;

namespace algo_vis.visualisers;

public abstract class BaseVisualiser<T> : IDataVisualiser<T>
{
    protected readonly SKBitmap Bitmap;

    protected BaseVisualiser(SKBitmap bitmap)
    {
        Bitmap = bitmap;
    }

    public void DisplayData(T data, FrameBuffer buffer)
    {
        unsafe
        {
            fixed (uint* ptr = buffer.Pixels)
            {
                using var surface = SKSurface.Create(
                    new SKImageInfo(buffer.Width, buffer.Height),
                    (IntPtr)ptr);
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);
                DrawData(data, canvas, buffer.Width, buffer.Height);
            }
        }
    }

    protected abstract void DrawData(T data, SKCanvas canvas, int width, int height);
}