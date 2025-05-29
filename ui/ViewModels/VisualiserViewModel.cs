using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using algo_vis.core.Interfaces;
using algo_vis.core.Models;
using algo_vis.ui.Controls;
using algo_vis.ui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace algo_vis.ui.ViewModels;

public partial class VisualiserViewModel<T> : ViewModelBase, IVisualiserViewModel
{
    private readonly AlgorithmController<T> _controller;
    private readonly IVisualiser<T> _visualiser;
    private SkiaCanvas? _canvas;

    // queue up the last full LodExplanation
    private LodExplanation _lastLod = default!;

    // track when algorithm is done
    private bool _isComplete;

    public VisualiserViewModel(
        AlgorithmController<T> controller,
        IVisualiser<T> visualiser)
    {
        _controller = controller;
        _visualiser = visualiser;
        NextStepCommand = new RelayCommand(OnNext);

        // 1) grab the very first state/explanation
        var init = _controller.Current;
        _lastLod = init.Explanation;
        Explanation = _lastLod[SelectedLevel];

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
        
        // Re-render current data with new verbosity level if needed
        RenderCurrentData();
    }

    [ObservableProperty] 
    private string _explanation = string.Empty;

    public IRelayCommand NextStepCommand { get; }

    public void SetCanvas(SkiaCanvas canvas)
    {
        _canvas = canvas;
        
        // Render initial state immediately
        RenderCurrentData();
    }

    private void RenderCurrentData()
    {
        if (_canvas != null)
        {
            var currentData = _controller.Current.Data;
            _canvas.RenderVisualization(currentData, _visualiser);
        }
    }

    private void OnNext()
    {
        if (_isComplete || _canvas == null)
            return;

        var result = _controller.Step();
        _isComplete = result.IsComplete;
        _lastLod = result.Explanation;
        Explanation = _lastLod[SelectedLevel];

        // Render the new data
        _canvas.RenderVisualization(result.Data, _visualiser);
    }
}

public interface IVisualiserViewModel
{
    string Explanation { get; }
    
    IReadOnlyList<VerbosityLevel> VerbosityLevels { get; }
    
    VerbosityLevel SelectedLevel { get; set; }
    IRelayCommand NextStepCommand { get; }
    void SetCanvas(SkiaCanvas canvas);
}