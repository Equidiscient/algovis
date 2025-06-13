using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using algo_vis.core.Interfaces;
using algo_vis.core.Models;
using algo_vis.core.Types;

namespace algo_vis.core;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AlgorithmStateAttribute : Attribute { }


public abstract class AlgorithmBase<T> : IAlgorithm<T>
{
    public record Snapshot(AlgorithmState<T> AlgorithmState, Dictionary<string, object> InternalState)    {
        public AlgorithmState<T> AlgorithmState { get; } = AlgorithmState;
        public Dictionary<string, object> InternalState { get; } = InternalState;
    }

    protected T _data;

    public virtual void Reset() => ResetToInitialState();

    public virtual T GetDataToVisualize() => _data;

    /// <summary>
    /// Capture the current internal state of fields marked with [AlgorithmState]
    /// </summary>
    public Snapshot CaptureState(bool isComplete = false, bool isSubStep = false)
    {
        var algorithmState = new AlgorithmState<T>(_data, GetExplanation(), isComplete, isSubStep);
        return new Snapshot(algorithmState, CaptureInternalState());
    }

    
    private Dictionary<string, object> CaptureInternalState()
    {
        var state = new Dictionary<string, object>();
        var fields = GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<AlgorithmStateAttribute>() != null);
        
        foreach (var field in fields)
        {
            var value = field.GetValue(this);
            if (value != null)
            {
                state[field.Name] = value;
            }
        }
        
        return state;
    }
    
    /// <summary>
    /// Restore internal state from a dictionary of field names to values
    /// </summary>
    public virtual void RestoreState(Snapshot snapshot)
    {
        _data = snapshot.AlgorithmState.Data;
        var fields = GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<AlgorithmStateAttribute>() != null)
            .ToDictionary(f => f.Name, f => f);
        
        foreach (var kvp in snapshot.InternalState)
        {
            if (fields.TryGetValue(kvp.Key, out var field))
            {
                try
                {
                    field.SetValue(this, kvp.Value);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to restore field {kvp.Key}: {ex.Message}");
                }
            }
        }
    }

    
    /// <summary>
    /// Reset algorithm's internal state to initial conditions.
    /// Called after data restoration, Reset(), and optionally after Initialize().
    /// Override this to reset iteration variables, flags, etc.
    /// </summary>
    protected abstract void ResetToInitialState();

    public abstract void Initialize();
    public abstract bool NextStep();
    public abstract LodExplanation GetExplanation();
}