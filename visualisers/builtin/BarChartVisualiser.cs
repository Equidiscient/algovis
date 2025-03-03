using System;
using System.Linq;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace algo_vis.visualisers.builtin;

public class BarChartVisualiser : BaseVisualiser<int[]>
{
    public BarChartVisualiser(SKBitmap bitmap)
        : base(bitmap) { }
    
    protected override void DrawData(int[] data, SKCanvas canvas, int width, int height)
    {
        if (data.Length == 0)
            return;
        int maxVal = data.Max();

        // bar width & padding
        float barWidth = (float)width / data.Length;
        float barGap = barWidth * 0.25f;
        using var paint = new SKPaint();
        paint.Color = SKColors.Red;

        // drawing each data point
        for (int i = 0; i < data.Length; i++)
        {
            float fraction = maxVal == 0 ? 0 : (float)data[i] / maxVal;
            float barHeight = fraction * height;

            float x = i * barWidth + barGap/2;
            float y = height - barHeight;

            canvas.DrawRect(new SKRect(x, y, x + barWidth - barGap, height), paint);
        }
    }
}