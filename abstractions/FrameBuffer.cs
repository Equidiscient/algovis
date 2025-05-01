using System;

namespace algo_vis.abstractions
{
  /// <summary>
  /// An ARGB32 pixel‐buffer, for decoupling from SkiaSharp.
  /// </summary>
  public class FrameBuffer
  {
    public int Width  { get; }
    public int Height { get; }

    public uint[] Pixels { get; }

    public FrameBuffer(int width, int height)
    {
      if (width <= 0 || height <= 0)
        throw new ArgumentOutOfRangeException(nameof(width));
      Width  = width;
      Height = height;
      Pixels = new uint[width * height];
    }
  }
}