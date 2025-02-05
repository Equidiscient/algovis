namespace algo_vis.algorithms.Models;

public class ArrayTracer
    {
        private string _title;
        
        public ArrayTracer(string title = "Array1DTracer")
        {
            _title = title;
        }
        
        public ArrayTracer Set(params object[] array1d)
        {
            // store
            return this;
        }
        
        public ArrayTracer Reset()
        {
            // reset
            return this;
        }
        
        public ArrayTracer Delay()
        {
            // implement delay
            return this;
        }
        
        public ArrayTracer Patch(int x, object v)
        {
            // highlight change at index x
            return this;
        }
        
        public ArrayTracer Depatch(int x)
        {
            // stop highlighting
            return this;
        }
        
        public ArrayTracer Select(int x)
        {
            // mark index x as selected
            return this;
        }
        
        public ArrayTracer Select(int sx, int ex)
        {
            // mark all indices from sx to ex
            return this;
        }
        
        public ArrayTracer Deselect(int x)
        {
            // unmark index x
            return this;
        }
        
        public ArrayTracer Deselect(int sx, int ex)
        {
            // unmark indices from sx to ex
            return this;
        }
    }

