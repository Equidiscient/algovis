using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace algo_vis.abstractions;

public enum VerbosityLevel
{
    Brief = 0,
    Detailed = 1
}

/// <summary>
/// Maps each verbosity level to its message and validates
/// that no level is missing at construction time.
/// </summary>
public readonly record struct LodExplanation
{
    private readonly IReadOnlyDictionary<VerbosityLevel, string> _explanations;

    public LodExplanation(IReadOnlyDictionary<VerbosityLevel, string> explanations)
    {
        if (explanations is null)
            throw new ArgumentNullException(nameof(explanations));

        // ensure every enum value is supplied
        foreach (var levelObj in Enum.GetValues(typeof(VerbosityLevel)))
        {
            if (levelObj is not VerbosityLevel level) continue;
            if (!explanations.ContainsKey(level))
                throw new ArgumentException($"Missing explanation for verbosity {level}", nameof(explanations));
        }

        _explanations = explanations;
    }

    /// <summary>
    /// Retrieves the message for the given verbosity.
    /// </summary>
    public string this[VerbosityLevel level] => _explanations[level];
}