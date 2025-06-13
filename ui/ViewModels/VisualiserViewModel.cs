using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using algo_vis.core.Interfaces;
using algo_vis.core.Models;
using algo_vis.core.Types;
using algo_vis.ui.Controls;
using algo_vis.ui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace algo_vis.ui.ViewModels;

public partial class VisualiserViewModel<T> : ViewModelBase, IVisualiserViewModel
{
    private readonly AlgorithmController<T> _controller;
    private readonly IVisualiser<T> _visualiser;
    private SkiaCanvas? _skiaCanvas;
    private LodExplanation _lastLod;
    private bool _isComplete;

    [ObservableProperty] private VerbosityLevel _selectedLevel = VerbosityLevel.Brief;
    [ObservableProperty] private string _explanation = "";
    [ObservableProperty] private bool _canStepForward = true;
    [ObservableProperty] private bool _canStepBackward = false;

    public IRelayCommand NextStepCommand { get; }
    public IRelayCommand PreviousStepCommand { get; }
    
    // Interface implementation
    public IReadOnlyList<VerbosityLevel> VerbosityLevels { get; } = 
        new[] { VerbosityLevel.Brief, VerbosityLevel.Detailed }.ToImmutableList();

    public VisualiserViewModel(
        AlgorithmController<T> controller,
        IVisualiser<T> visualiser)
    {
        _controller = controller;
        _visualiser = visualiser;
        
        // Initialize commands
        NextStepCommand = new RelayCommand(OnNext, () => CanStepForward);
        PreviousStepCommand = new RelayCommand(OnPrevious, () => CanStepBackward);
        
        // Set initial state
        var initialState = _controller.CurrentAlgoState;
        _lastLod = initialState.Explanation;
        Explanation = _lastLod[SelectedLevel];
        _isComplete = initialState.IsComplete;
        
        UpdateNavigationState();
    }

    partial void OnSelectedLevelChanged(VerbosityLevel value)
    {
        Explanation = _lastLod[value];
    }

    // Interface implementation - takes SkiaCanvas UI control
    public void SetCanvas(SkiaCanvas canvas)
    {
        _skiaCanvas = canvas;
        // Render initial state
        RenderCurrentState();
    }

    private void OnNext()
    {
        if (!_controller.CanStepForward)
            return;

        var result = _controller.StepForward();
        UpdateStateFromResult(result);
        UpdateNavigationState();
    }

    private void OnPrevious()
    {
        if (!_controller.CanStepBackward)
            return;

        var result = _controller.StepBackward();
        UpdateStateFromResult(result);
        UpdateNavigationState();
    }

    private void UpdateStateFromResult(AlgorithmState<T> result)
    {
        _isComplete = result.IsComplete;
        _lastLod = result.Explanation;
        Explanation = _lastLod[SelectedLevel];
        RenderCurrentState();
    }

    private void RenderCurrentState()
    {
        if (_skiaCanvas == null) return;
        
        var currentData = _controller.CurrentAlgoState.Data;
        // Use the SkiaCanvas control's RenderVisualization method
        // This method creates a SkiaVisualisationCanvas internally and calls the visualiser
        _skiaCanvas.RenderVisualization(currentData, _visualiser);
    }

    private void UpdateNavigationState()
    {
        CanStepForward = _controller.CanStepForward;
        CanStepBackward = _controller.CanStepBackward;
        
        NextStepCommand.NotifyCanExecuteChanged();
        PreviousStepCommand.NotifyCanExecuteChanged();
    }
}

public interface IVisualiserViewModel
{
    string Explanation { get; }
    
    IReadOnlyList<VerbosityLevel> VerbosityLevels { get; }
    
    VerbosityLevel SelectedLevel { get; set; }
    IRelayCommand NextStepCommand { get; }
    IRelayCommand PreviousStepCommand { get; }
    bool CanStepForward { get; }
    bool CanStepBackward { get; }
    void SetCanvas(SkiaCanvas canvas);
}