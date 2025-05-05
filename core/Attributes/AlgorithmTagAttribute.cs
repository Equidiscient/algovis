using System;

namespace algo_vis.core.Attributes;

/// <summary>
/// Tags the algorithm and optionally associates a visualiser by key (library-specific).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AlgorithmTagAttribute(string[] tags, string? visualiserKey = null) : Attribute
{
    public string[] Tags { get; } = tags;

    /// <summary>
    /// (Optional) Visualiser key for the main app to resolve via VisualiserLibrary
    /// </summary>
    public string? VisualiserKey { get; } = visualiserKey;

    public AlgorithmTagAttribute(params string[] tags) : this(tags, null)
    {
    }
}