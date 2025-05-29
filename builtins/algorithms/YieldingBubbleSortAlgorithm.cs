using System;
using System.Linq;
using System.Collections.Generic;
using algo_vis.core;
using algo_vis.core.Attributes;
using algo_vis.core.Interfaces;
using algo_vis.core.Models;

namespace algo_vis.builtins.algorithms
{
    [AlgorithmTag("Built-in", "Sorting")]
    public class YieldingBubbleSortAlgorithm : IYieldingAlgorithm<int[]>
    {
       private int[] _data = [];
        private int _i, _j;
        private bool _sorted;
        private LodExplanation _lastExplanation;

        public void Initialize()
        {
            var bytes = new byte[20];
            new Random().NextBytes(bytes);
            _data = bytes.Select(x => (int)x).ToArray();
            _i = 0;
            _j = 0;
            _sorted = _data.Length <= 1;

            var msg = _sorted
                ? "Array trivially sorted"
                : $"Initialized bubble sort on {_data.Length} elements";

            _lastExplanation = new LodExplanation(new Dictionary<VerbosityLevel, string>
            {
                [VerbosityLevel.Brief] = msg,
                [VerbosityLevel.Detailed] = msg
            });
        }

        public bool NextStep()
        {
            // Legacy support - just execute the first yielded state
            var states = ExecuteStep().ToList();
            if (states.Any())
            {
                _lastExplanation = states.Last().Explanation;
                return !states.Last().IsComplete;
            }
            return false;
        }

        public IEnumerable<AlgorithmState<int[]>> ExecuteStep()
        {
            if (_sorted)
            {
                var doneMsg = "Sorting complete";
                yield return new AlgorithmState<int[]>(
                    GetDataToVisualize(),
                    new LodExplanation(new Dictionary<VerbosityLevel, string>
                    {
                        [VerbosityLevel.Brief] = doneMsg,
                        [VerbosityLevel.Detailed] = doneMsg
                    }),
                    isComplete: true
                );
                yield break;
            }

            int limit = _data.Length - _i - 1;
            if (_j < limit)
            {
                int a = _data[_j], b = _data[_j + 1];
                
                // Yield highlighting the comparison
                yield return new AlgorithmState<int[]>(
                    GetDataToVisualize(),
                    new LodExplanation(new Dictionary<VerbosityLevel, string>
                    {
                        [VerbosityLevel.Brief] = $"Comparing {a} and {b}",
                        [VerbosityLevel.Detailed] = $"Comparing elements at positions {_j} and {_j + 1}: {a} vs {b}"
                    }),
                    isSubStep: true
                );

                if (a > b)
                {
                    // Perform the swap
                    (_data[_j], _data[_j + 1]) = (b, a);
                    
                    // Yield the swap result
                    yield return new AlgorithmState<int[]>(
                        GetDataToVisualize(),
                        new LodExplanation(new Dictionary<VerbosityLevel, string>
                        {
                            [VerbosityLevel.Brief] = $"Swapped {a} and {b}",
                            [VerbosityLevel.Detailed] = $"Swapped {a} (idx {_j}) and {b} (idx {_j + 1}) because {a} > {b}"
                        })
                    );
                }
                else
                {
                    // No swap needed
                    yield return new AlgorithmState<int[]>(
                        GetDataToVisualize(),
                        new LodExplanation(new Dictionary<VerbosityLevel, string>
                        {
                            [VerbosityLevel.Brief] = "No swap needed",
                            [VerbosityLevel.Detailed] = $"No swap needed: {a} <= {b}"
                        })
                    );
                }

                _j++;
            }
            else
            {
                _j = 0;
                _i++;
                if (_i >= _data.Length - 1)
                {
                    _sorted = true;
                    var done = "Sorting complete";
                    yield return new AlgorithmState<int[]>(
                        GetDataToVisualize(),
                        new LodExplanation(new Dictionary<VerbosityLevel, string>
                        {
                            [VerbosityLevel.Brief] = done,
                            [VerbosityLevel.Detailed] = done
                        }),
                        isComplete: true
                    );
                }
                else
                {
                    var passMsg = $"Starting pass #{_i + 1}";
                    yield return new AlgorithmState<int[]>(
                        GetDataToVisualize(),
                        new LodExplanation(new Dictionary<VerbosityLevel, string>
                        {
                            [VerbosityLevel.Brief] = passMsg,
                            [VerbosityLevel.Detailed] = $"Completed pass #{_i}. Starting pass #{_i + 1} of bubble sort."
                        })
                    );
                }
            }
        }

        public LodExplanation GetExplanation() => _lastExplanation;

        public int[] GetDataToVisualize() => (int[])_data.Clone();
    }
}