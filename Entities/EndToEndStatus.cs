namespace FunctionalAreaCoverage.Entities
{
    public enum EndToEndStatus
    {
        Unknown,
        Cancelled,
        FailedQb,
        PendingTcReplacement,
        FailedE2eReview,
        Open,
        E2eDefinition,
        InQeReview,
        E2eReviewSignOff,
        Blocked,
        DependencyBlock,
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
