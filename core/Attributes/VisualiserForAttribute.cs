using System;

namespace algo_vis.core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class VisualiserForAttribute(Type algorithmType) : Attribute
{
  // <summary>
  // The algorithm type this visualiser is for.
  // </summary>
  public Type AlgorithmType { get; } = algorithmType;
  
  /// <summary>
  /// Off by default.
  /// If true, also register this visualiser
  /// for global fallback matching by data-type.
  /// If false, only pair it explicitly with AlgorithmType.
  /// </summary>
  public bool AllowGlobalFallback { get; set; } = false;
}