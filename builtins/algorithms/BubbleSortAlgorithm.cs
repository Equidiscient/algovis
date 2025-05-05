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
    public class BubbleSortAlgorithm : IAlgorithm<int[]>
    {
        private int[] _data = [];
        private int _i, _j;
        private bool _sorted;
        private LodExplanation _lastExplanation;

        public void Initialize()
        {
            var bytes = new byte[20];
            new Random().NextBytes(bytes);
            _data      = bytes.Select(x => (int)x).ToArray();
            _i         = 0;
            _j         = 0;
            _sorted    = _data.Length <= 1;

            var msg = _sorted
                ? "Array trivially sorted"
                : $"Initialized bubble sort on {_data.Length} elements";

            _lastExplanation = new LodExplanation(new Dictionary<VerbosityLevel,string>
            {
                [VerbosityLevel.Brief]    = msg,
                [VerbosityLevel.Detailed] = msg
            });
        }

        public bool NextStep()
        {
            if (_sorted)
            {
                var doneMsg = "Sorting complete";
                _lastExplanation = new LodExplanation(new Dictionary<VerbosityLevel,string>
                {
                  [VerbosityLevel.Brief]    = doneMsg,
                  [VerbosityLevel.Detailed] = doneMsg
                });
                return false;
            }

            int limit = _data.Length - _i - 1;
            if (_j < limit)
            {
                int a = _data[_j], b = _data[_j+1];
                string msg;
                if (a > b)
                {
                    (_data[_j], _data[_j+1]) = (b, a);
                    msg = $"Swapped {a} (idx {_j}) and {b} (idx {_j+1})";
                }
                else
                {
                    msg = $"Compared {a} and {b}, no swap";
                }

                _lastExplanation = new LodExplanation(new Dictionary<VerbosityLevel,string>
                {
                    [VerbosityLevel.Brief]    = msg,
                    [VerbosityLevel.Detailed] = msg
                });

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
                    _lastExplanation = new LodExplanation(new Dictionary<VerbosityLevel,string>
                    {
                        [VerbosityLevel.Brief]    = done,
                        [VerbosityLevel.Detailed] = done
                    });
                }
                else
                {
                    var passMsg = $"Starting pass #{_i+1}";
                    _lastExplanation = new LodExplanation(new Dictionary<VerbosityLevel,string>
                    {
                        [VerbosityLevel.Brief]    = passMsg,
                        [VerbosityLevel.Detailed] = passMsg
                    });
                }
            }

            return !_sorted;
        }

        public LodExplanation GetExplanation() => _lastExplanation;

        public int[] GetDataToVisualize() => (int[])_data.Clone();
    }
}