using System;

namespace algo_vis.ui.Models;

public class AlgorithmMenuItem(string name, Type algoType, Type visType, string[] tags)
{
    public string Name { get; } = name;
    public Type AlgorithmType { get; } = algoType;
    public Type VisualiserType { get; } = visType;
    public string[] Tags { get; } = tags;

    // group by namespace - may change to tag
    public string GroupKey => AlgorithmType.Namespace ?? "Built-In";
    
    public override string ToString() => Name;
}