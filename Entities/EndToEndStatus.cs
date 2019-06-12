namespace FunctionalAreaCoverage.Entities
{
    public enum EndToEndStatus
    {
        Unknown,
        Cancelled,
        FailedQb,
        Blocked,
        FailedAutomationQb,
        Open,
        InQeReview,
        Backlog,
        InWriting,
        InExecution,
        ProductSignOff,
        PassedAutomationQb,
        AutomationBacklog,
        InAutomation,
        Done
    }
}
