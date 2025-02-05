using algo_vis.algorithms.Models;

namespace algo_vis.algorithms.Orchestrator
{
    public abstract class BaseOrchestrator<T> 
    {
        private LogTracer _logTracer = new(); 
        public bool IsComplete { get; private set; }

        public abstract void InitialiseData(T parameters);

        public abstract void NextStep();

    }
}