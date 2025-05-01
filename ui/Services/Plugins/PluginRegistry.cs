using System;
using System.Collections.Generic;

namespace algo_vis.ui.Services.Plugins;
/// <summary>
/// Holds discovered plugin types.
/// </summary>
public class PluginRegistry
{
    public List<Type> Algorithms   { get; } = [];
    public List<Type> Visualisers { get; } = [];
    public List<ConverterInfo> Converters  { get; } = [];
}

/// <summary>
/// Captures the TFrom → TTo converter relationship.
/// </summary>
public record ConverterInfo(Type From, Type To, Type ConverterType);