using System;
using System.Collections.Generic;

namespace algo_vis.ui.Services.Plugins;

/// <summary>
/// Holds discovered plugin types.
/// </summary>
public static class PluginRegistry
{
    public static List<Type> Algorithms   { get; } = [];
    public static List<Type> Visualisers { get; } = [];
    public static List<ConverterInfo> Converters  { get; } = [];

    // Maps "VisualiserKey" => IVisualiser<> type
    public static Dictionary<string, Type> VisualiserKeys { get; } = new();

    // Maps IVisualiser<T> (T) type to visualiser type (for fallback)
    public static Dictionary<Type, Type> VisualiserByDataType { get; } = new();
}

/// <summary>
/// Captures the TFrom → TTo converter relationship.
/// </summary>
public record ConverterInfo(Type From, Type To, Type ConverterType);