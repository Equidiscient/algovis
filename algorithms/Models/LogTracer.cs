namespace algo_vis.algorithms.Models;

public class LogTracer
{
    private string _title;
    
    public LogTracer(string title = "")
    {
        _title = title;
    }
    
    public LogTracer Set(params object[] messages)
    {
        // process the messages
        return this;
    }
    
    public LogTracer Reset()
    {
        // clear data
        return this;
    }
    
    public LogTracer Delay()
    {
        // delay
        return this;
    }
    
    public LogTracer Print(string message)
    {
        return this;
    }
}
