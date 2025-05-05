using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using algo_vis.core.Interfaces;
using algo_vis.core.Models;
using algo_vis.ui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;

namespace algo_vis.ui.ViewModels;

  public partial class VisualiserViewModel<T> : ViewModelBase, IVisualiserViewModel
  {
    private readonly AlgorithmController<T> _controller;
    private readonly IVisualiser<T>         _visualiser;

    // queue up the last full LodExplanation
    private LodExplanation _lastLod = default!;

    // track when algorithm is done
    private bool _isComplete;

    public VisualiserViewModel(
    AlgorithmController<T> controller,
    IVisualiser<T>         visualiser)
{
  _controller     = controller;
  _visualiser     = visualiser;
  NextStepCommand = new RelayCommand(OnNext);

  // 1) grab the very first state/explanation
  var init = _controller.Current;
  _lastLod    = init.Explanation;
  Explanation = _lastLod[SelectedLevel];

  // 2) whenever the bitmap is allocated, draw the INITIAL data
  PropertyChanged += (_, e) =>
  {
    if (e.PropertyName == nameof(RenderedOutput) 
        && RenderedOutput is { } bmp)
    {
      var canvas = new SkiaVisualisationCanvas(bmp);
      _visualiser.DrawData(init.Data, canvas);
      OnPropertyChanged(nameof(RenderedOutput)); // push the update back to the view
    }
  };

  SelectedLevel = VerbosityLevel.Brief;
}

    /// <summary>
    /// All available verbosity levels for the UI to choose.
    /// </summary>
    public IReadOnlyList<VerbosityLevel> VerbosityLevels => Enum.GetValues<VerbosityLevel>().AsReadOnly();

    [ObservableProperty]
    private VerbosityLevel _selectedLevel;
    
    partial void OnSelectedLevelChanged(VerbosityLevel old, VerbosityLevel @new)
    {
      // re-render the last explanation at the newly picked level
      Explanation = _lastLod[@new];
    }

    [ObservableProperty] private string     _explanation    = string.Empty;
    [ObservableProperty] private SKBitmap?  _renderedOutput;

    partial void OnRenderedOutputChanged(SKBitmap? value)
    {
      if(value is null) return;
      Debug.WriteLine("RenderedOutput changed:");
      var canvas = new SkiaVisualisationCanvas(value);
      _visualiser.DrawData(_controller.Current.Data, canvas);
    }

    public IRelayCommand NextStepCommand { get; }

    private void OnNext()
    {
      if (_isComplete || RenderedOutput is null)
        return;

      // run one step and grab the LodExplanation
      var result   = _controller.Step();
      _isComplete  = result.IsComplete;
      _lastLod     = result.Explanation;

      // pick the right string for the current verbosity
      Explanation = _lastLod[SelectedLevel];

      // draw the data into our SKBitmap
      var canvas = new SkiaVisualisationCanvas(RenderedOutput);
      _visualiser.DrawData(result.Data, canvas);

      // notify Avalonia that the bitmap has changed
      OnPropertyChanged(nameof(RenderedOutput));
    }
  }

  public interface IVisualiserViewModel
  {
    SKBitmap? RenderedOutput { get; }
    string Explanation       { get; }
    
    IReadOnlyList<VerbosityLevel> VerbosityLevels { get; }
    
    VerbosityLevel SelectedLevel { get; set;  }
    IRelayCommand NextStepCommand { get; }
  }