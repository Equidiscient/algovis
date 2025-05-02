namespace algo_vis.core.Models;

public readonly struct Color32(byte a, byte r, byte g, byte b)
{
    public readonly byte A = a, R = r, G = g, B = b;
}
