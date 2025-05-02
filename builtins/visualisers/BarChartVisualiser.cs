using System.Drawing;
using System.Linq;
using algo_vis.builtins.algorithms;
using algo_vis.core.Attributes;
using algo_vis.core.Interfaces;
using algo_vis.core.Models;

namespace algo_vis.builtins.visualisers;

[VisualiserFor(typeof(BubbleSortAlgorithm))]
public class BarChartVisualiser() : IVisualiser<int[]>
{
  public void DrawData(int[] data, IVisualisationCanvas canvas)
  {
    canvas.Clear();  // defaults to white

    if (data.Length == 0)
      return;

    float width = canvas.Width;
    float height = canvas.Height;

    float barWidth = width / data.Length;
    float gap      = barWidth * 0.25f;
    int   maxVal   = data.Max();

    for (int i = 0; i < data.Length; i++)
    {
      float frac  = maxVal == 0 ? 0 : data[i] / (float)maxVal;
      float bh    = frac * height;
      var   rect  = new RectF(i*barWidth + gap/2, height-bh,
        barWidth-gap, bh);

      canvas.FillRect(rect, new(255,255,0,0));
    }
  }
}