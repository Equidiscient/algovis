using System;
using System.Runtime.InteropServices;
using algo_vis.abstractions;
using algo_vis.abstractions.Interfaces;
using algo_vis.ui.Models;
using algo_vis.ui.ViewModels;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;

namespace algo_vis.ui.ViewModels;

public interface IVisualiserViewModel
{
    SKBitmap RenderedOutput { get; }
    string   Explanation    { get; }
    IRelayCommand NextStepCommand { get; }
}

public partial class VisualiserViewModel<T> : ViewModelBase, IVisualiserViewModel
{
    private readonly AlgorithmController<T> _controller;
    private readonly IDataVisualiser<T>     _visualiser;
    private readonly FrameBuffer            _buffer;

    public VisualiserViewModel(
        AlgorithmController<T> controller,
        IDataVisualiser<T>     visualiser,
        FrameBuffer            buffer)
    {
        _controller  = controller;
        _visualiser  = visualiser;
        _buffer      = buffer;
        NextStepCommand = new RelayCommand(OnNext);
        _isComplete = false;
    }

    public IRelayCommand NextStepCommand { get; }

    [ObservableProperty] private SKBitmap _renderedOutput = new(400,200);
    [ObservableProperty] private string   _explanation   = string.Empty;
    private bool _isComplete;


    private void OnNext()
    {
        var result = _controller.Step();
        _isComplete = result.IsComplete;

        // render
        _visualiser.DisplayData(result.Data, _buffer);
        CopyBufferToBitmap(_buffer, RenderedOutput);

        // update
        Explanation = result.Explanation;
        OnPropertyChanged(nameof(RenderedOutput));
    }

    private unsafe void CopyBufferToBitmap(FrameBuffer fb, SKBitmap bmp)
    {
        fixed (uint* src = fb.Pixels)
        {
            var dst = bmp.GetPixels();
            long size = fb.Pixels.Length * sizeof(uint);
            Buffer.MemoryCopy(src, (void*)dst, size, size);
        }
    }
}