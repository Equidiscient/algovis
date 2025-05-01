using System;

namespace algo_vis.abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AlgorithmTagAttribute : Attribute
    {
        public string[] Tags { get; }
        public AlgorithmTagAttribute(params string[] tags) => Tags = tags;
    }
}