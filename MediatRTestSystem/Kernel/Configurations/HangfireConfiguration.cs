namespace Kernel.Configurations
{
    public class HangfireConfiguration
    {
        public string? ConnectionString { get; set; }

        public string[]? Queues { get; set; }

        public int MaxDefaultWorkerCount { get; set; }
    }
}
