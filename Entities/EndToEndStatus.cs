namespace FunctionalAreaCoverage.Entities
{
    public enum EndToEndStatus
    {
        Unknown,
        Cancelled,
        FailedQb,
        FailedE2eReview,
        Blocked,
        PendingTcReplacement,
        Open,
        InQeReview,
        E2eReviewSignOff,
        Backlog,
        InWriting,
        InExecution,
        ProductSignOff,
        FailedAutomationQb,
        PassedAutomationQb,
        AutomationBacklog,
        InAutomation,
        Done
    }
}
